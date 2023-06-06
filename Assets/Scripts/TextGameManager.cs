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
    [Header("Player")]
    public int Health = 10;
    public int Stamina = 10;

    [Header("Game")]
    [SerializeField] [TextArea(3, 50)] private string m_gameText = string.Empty;
    private bool m_hasGameStarted = false;
    private bool m_paused = false;

    [Header("UI")]
    [SerializeField] private TextMeshProUGUI m_menuTextUI;
    [SerializeField] private TextMeshProUGUI m_gameTextUI;
    [SerializeField] private TextMeshProUGUI m_healthTextUI;
    [SerializeField] private TextMeshProUGUI m_staminaTextUI;
    [SerializeField] private GameObject m_choicesGroupUI;
    [SerializeField] private GameObject m_menuUI;
    [SerializeField] private GameObject m_gameUI;

    [Space]
    [SerializeField] private GameObject m_choiceButtonPrefab;
    
    private readonly Dictionary<string, dynamic> m_flags = new();

    public void StartGame()
    {
        m_hasGameStarted = true;
        m_menuUI.SetActive(false);
        m_gameUI.SetActive(true);
        Lvl1_Start();
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

        if (m_hasGameStarted)
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


    #region Level 1 Events

    public void Lvl1_Start()
    {
        m_gameText = "There is a door in front of me.\r\nI can feel a small breeze coming from the window beside the door.";

        AddChoices(new()
        {
            new( "Open door", Lvl1_OpenDoor ),
            new( "Open window", Lvl1_OpenWindow ),
            new( "Wait", Lvl1_Wait ),
        });
    }

    public void Lvl1_OpenDoor()
    {
        if (Stamina <= 0)
        {
            m_gameText = "You're tired";
            return;
        }

        if (!GetFlag<bool>("has_key")) 
        {
            m_gameText = "The door won't budge.";
            Stamina -= 1;
        }
        else if (!GetFlag<bool>("is_door_open"))
        {
            m_gameText = "You open the door using the key.";
            SetFlag("is_door_open", true);

            UpdateChoice(0, "Go through door");
            Stamina -= 1;

            return;
        }

        if (GetFlag<bool>("is_door_open"))
        {
            Lvl2_Start();
        }
    }

    public void Lvl1_OpenWindow()
    {
        if (Stamina <= 0)
        {
            m_gameText = "You're too tired to do this";
            return;
        }

        m_gameText = "You try to push the window open but it is sealed shut. \nYou fall down trying to push it open. [-1HP, -5STA]";
        Health -= 1;
        Stamina -= 5;
    }

    public void Lvl1_Wait()
    {
        SetFlag("wait_counter", GetFlag<int>("wait_counter") + 1);
        if (GetFlag<int>("wait_counter") >= 5)
        {
            m_gameText = "A key has slid underneath the door. [+1 key]";
            SetFlag("has_key", true);
            return;
        }

        var _waitText = GetFlag<int>("wait_counter") > 1 ? $"You waited {GetFlag<int>("wait_counter")} times." : "You wait.";

        m_gameText = $"{_waitText} \nYou feel rested. [10STA]";
        Stamina = 10;
    }
    #endregion

    #region Level 2 Events

    public void Lvl2_Start()
    {
        m_gameText = "You enter a dark hallway.\nThe air is humid and the walls and floor are wet\nYou hear clanking in the distance.";

        AddChoices(new()
        {
            new( "Walk forward", () => { }),
        });
    }

    #endregion

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

}
