using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System;
using System.Drawing.Text;

public class Choice
{
    public string Text { get; set; }
    public Action OnClick { get; set; }

    public Choice(string text, Action onClick)
    {
        Text = text;
        OnClick = onClick;
    }
}

public class TextGameManager : MonoBehaviour
{
    public const int MAX_HEALTH = 10;
    public const int MAX_STAM = 10;

    [Header("Player")]
    public int Health = 10;
    public int Stamina = 10;

    [Header("Game")]
    [SerializeField] [TextArea(3, 50)] private string m_gameText = string.Empty;
    private bool m_hasGameStarted = false;
    private bool m_isGameOver = false;
    private bool m_paused = false;

    [Header("UI")]
    [SerializeField] private TextMeshProUGUI m_menuTextUI;
    [SerializeField] private TextMeshProUGUI m_gameOverTextUI;
    [SerializeField] private TextMeshProUGUI m_gameTextUI;
    [SerializeField] private TextMeshProUGUI m_healthTextUI;
    [SerializeField] private TextMeshProUGUI m_staminaTextUI;
    [SerializeField] private GameObject m_choicesGroupUI;

    [Header("Screens")]
    [SerializeField] private GameObject m_menuUI;
    [SerializeField] private GameObject m_gameUI;
    [SerializeField] private GameObject m_gameOverUI;

    [Space]
    [SerializeField] private GameObject m_choiceButtonPrefab;

    [Header("Scares")]
    [SerializeField] GameObject m_scareUI;
    [SerializeField] List<Sprite> m_scareSprites;
    
    private readonly Dictionary<string, dynamic> m_flags = new();

    void L1_Start()
    {
        m_gameText = "You wake up in your room. A storm is brewing.";
        AddChoices(new()
        {
            new( "Close your eyes", L1_CloseYourEyes ),
            new( "Get out of bed", L1_GetOutOfBed ),
        });
    }

    void L1_CloseYourEyes()
    {
        var closeEyes = GetFlag<int>("close_eyes_cntr");
        SetFlag("close_eyes_cntr", closeEyes + 1);

        m_gameText = "You wake up again. The storm is getting heavier.";

        if (closeEyes == 2)
            m_gameText = "You wake up once more. The storm stopped, and your legs feel heavy.";
        if (closeEyes > 2)
            m_gameText = "You can't. Something feels off.";
    }

    void L1_GetOutOfBed()
    {
        m_gameText = "You feel very groggy and your body feels heavy. It's like something is on your back";

        AddChoices(new()
        {
            new( "Look around", L1_LookAround ),
            new( "Go back to sleep", L1_CloseYourEyes ), 
        });
    }

    void L1_LookAround()
    {
        m_gameText = "You have a tall mirror. Beside that is a window, you can smell the rain. On the far side of the room is your door.";
        AddChoices(new()
        {
            new( "Check the mirror", L1_CheckMirror ),
            new( "Look outside of window", L1_CheckWindow ),
            new( "Leave your room", L1_LeaveRoom ),
        });
    }

    void L1_CheckMirror()
    {
        if (GetFlag<int>("ending") == 1)
        {
            m_gameText = "There is no mark on your neck.";
            AddChoices(new()
            {
                new( "...", L1_MonsterJumpscare )
            });

            return;
        }

        m_gameText = "You see a red hand mark on your neck.";
        AddChoices(new()
        {
            new( "Inspect mark", L1_InspectMark ),
        });
    }

    void L1_InspectMark()
    {
        m_gameText = "You touch the mark. You suddenly get transported in a dark room. You hear faint whispers around you.";
        AddChoices(new()
        {
            new( "Walk around", L1_WalkAround )
        });
    }

    void L1_WalkAround()
    {
        m_gameText = "You walk around the room. It feels rather large.";
        Stamina -= 1;
        AddChoices(new()
        {
            new( "Look around", () => { m_gameText = "There is nothing to see."; } ),
            new( "Keep walking", L1_KeepWalking ),
        }) ;
    }

    void L1_KeepWalking()
    {
        m_gameText = "You keep walking. You see a figure in the distance.";
        Stamina -= 3;
        AddChoices(new()
        {
            new( "Go towards it", L1_GoToMonster ),
            new( "Ignore it", L1_IgnoreMonster ),
        });
    }

    void L1_GoToMonster()
    {
        m_gameText = "The figure starts to look less like a human. You're now standing in front of it. You really can't tell what it is.";

        AddChoices(new()
        {
            new( "...", L1_MonsterJumpscare )
        });
    }

    /// <summary>
    /// Handles all jumpscares
    /// </summary>
    /// TODO add jumpscare
    void L1_MonsterJumpscare()
    {
        var ending = GetFlag<int>("ending");
        if (ending == 1)
        {
            m_gameText = "You see the figure standing behind you through the mirror";
            AddChoices(new()
            {
                new( "Accept it", () => { GameOver("It jumps towards you. You die"); })
            });
            return;
        }
        else if (ending == 2)
        {
            GameOver("It sprints towards you and pushes you off the window. You die");
            return;
        }

        m_gameText = "It pounces on you. You wake up. It was just a nightmare. You get out of bed";
        Health -= 5;
        Stamina = MAX_STAM;
        SetFlag("ending", 1);
        AddChoices(new()
        {
            new( "Check the mirror", L1_CheckMirror ),
            new( "Look outside of window", L1_CheckWindow ),
            new( "Leave your room", L1_LeaveRoom ),
        });
    }

    void L1_IgnoreMonster()
    {
        m_gameText = "The figure starts sprinting towards you.";
        AddChoices(new()
        {
            new( "Run away", L1_RunAway ),
            new( "Wait for it", () => GameOver("It catches up to you. You die.")),
        });
    }

    void L1_RunAway()
    {
        m_gameText = "It's still chasing you. It's getting closer.";
        AddChoices(new()
        {
            new( "Keep running", () => GameOver("It catches up to you. You die.")),
            new( "Run faster", L1_RunFaster )
        });
    }

    void L1_RunFaster()
    {
        m_gameText = "You manage to outrun it. You reach a door and you open it. You wake up, get out of bed, and leave the room immediately";
        AddChoices(new()
        {
            new( "...", () => GameOver("It was just a nightmare. You go on with your day, but that lingering feeling is still there, like someone is watching you")),
        });
    }

    void L1_CheckWindow()
    {
        m_gameText = "You open the window. Everything is dead silent. You look back and see a silhouette blocking your door.";
        SetFlag("door_monster", true);
        AddChoices(new()
        {
            new( "Stare at them", L1_StareAtMonster ),
            new( "Go back", L1_IgnoreDoorMonster )
        }) ;
    }

    void L1_StareAtMonster()
    {
        m_gameText = "They start groaning. It gets louder and louder as you keep staring. You can't take your eyes off.";
        SetFlag("ending", 2);
        AddChoices(new()
        {
            new( "...", L1_MonsterJumpscare )
        });
    }

    void L1_IgnoreDoorMonster()
    {
        m_gameText = "You have a tall mirror. On the far side of the room is your door, but someone is blocking it. They are still groaning.";
        AddChoices(new()
        {
            new( "Check the mirror", L1_CheckMirror ),
            new( "Look outside of window", L1_CheckWindow ),
            new( "Leave your room", L1_LeaveRoom ),
        });
    }

    void L1_LeaveRoom()
    {
        m_gameText = GetFlag<bool>("door_monster") ? "Doesn't seem like a good idea. The silhouette is still there." : "The door is unlocked, but you can't seem to get it to open";
    }

    void GameOver(string message)
    {
        m_isGameOver = true;
        m_hasGameStarted = false;

        StartCoroutine(Scare(message));
    }

    IEnumerator Scare(string message)
    {
        var rndScare = m_scareSprites[UnityEngine.Random.Range(0, m_scareSprites.Count)];
        m_scareUI.GetComponent<Image>().sprite = rndScare;
        m_gameUI.SetActive(false);
        m_scareUI.SetActive(true);

        yield return new WaitForSeconds(5);

        m_gameOverTextUI.text = message;
        m_gameOverUI.SetActive(true);
        m_scareUI.SetActive(false);
    }

    #region Util Functions
    public void StartGame()
    {
        m_hasGameStarted = true;
        m_menuUI.SetActive(false);
        m_gameUI.SetActive(true);

        L1_Start();
    }

    public void GoToMenu()
    {
        m_hasGameStarted = false;
        m_isGameOver = false;
        m_menuUI.SetActive(true);
        m_gameUI.SetActive(false);
        m_gameOverUI.SetActive(false);
    }

    public void QuitGame()
    {
        Application.Quit();
    }

    private void Update()
    {
        UpdateUI();

        if (Health <= 0)
        {
            m_gameText = "You got too injured and succumbed to your wounds.";
            m_choicesGroupUI.SetActive(false);
        }

        if (m_hasGameStarted && !m_isGameOver)
        {
            if (Input.GetKeyDown(KeyCode.Escape))
                m_paused = !m_paused;
        }
    }

    void UpdateUI()
    {
        m_healthTextUI.text = Health.ToString();
        m_staminaTextUI.text = Stamina.ToString();

        m_gameTextUI.text = m_gameText;

        if (!m_hasGameStarted) return;

        m_menuUI.SetActive(m_paused);
        m_gameUI.SetActive(!m_paused);
        m_menuTextUI.text = m_paused ? "Paused" : "Game";
    }

    public void AddChoices(List<Choice> choices)
    {
        foreach (Transform child in m_choicesGroupUI.transform)
        {
            Destroy(child.gameObject);
        }

        foreach (var choice in choices)
        {
            var btn = Instantiate(m_choiceButtonPrefab, m_choicesGroupUI.transform).GetComponent<Button>();
            btn.GetComponentInChildren<TextMeshProUGUI>().text = choice.Text;

            btn.onClick.AddListener(() => choice.OnClick());
        }
    }
    public void UpdateChoice(int choiceIndex, string choiceText)
    {
        var btn = m_choicesGroupUI.transform.GetChild(choiceIndex);
        btn.GetComponentInChildren<TextMeshProUGUI>().text = choiceText;
    }

    public void UpdateChoice(int choiceIndex, Choice newChoice)
    {
        var btn = m_choicesGroupUI.transform.GetChild(choiceIndex);
        btn.GetComponent<Button>().onClick.RemoveAllListeners();
        btn.GetComponent<Button>().onClick.AddListener(() => newChoice.OnClick());

        btn.GetComponentInChildren<TextMeshProUGUI>().text = newChoice.Text;
    }

    public T GetFlag<T>(string key)
    {
        if (m_flags.ContainsKey(key))
            return m_flags[key];

        SetFlag(key, default(T));
        return m_flags[key];
    }

    public void SetFlag(string key, dynamic value)
    {
        if (m_flags.ContainsKey(key))
            m_flags[key] = value;
        else
            m_flags.Add(key, value);
    }

    #endregion
}
