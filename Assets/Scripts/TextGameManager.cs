using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class TextGameManager : MonoBehaviour
{
    [Header("Player")]
    public int Health = 10;
    public int Stamina = 10;

    [Header("Game")]
    [SerializeField] [TextArea(3, 50)] private string m_gameText = string.Empty;
    [SerializeField] private int m_level = 1; 
    
    [Header("UI")]
    [SerializeField] private TextMeshProUGUI m_gameTextUI;
    [SerializeField] private TextMeshProUGUI m_healthTextUI;
    [SerializeField] private TextMeshProUGUI m_staminaTextUI;
    [SerializeField] private GameObject m_choicesGroupUI;

    [Space]
    [SerializeField] private List<GameObject> m_levelChoices = new();

    private readonly Dictionary<string, dynamic> m_flags = new();

    void Start()
    {
        Lvl1_Start();
    }

    private void Update()
    {
        UpdateUI();

        if (Health <= 0)
        {
            m_gameText = "You got too injured and succumbed to your wounds.";
            m_choicesGroupUI.SetActive(false);
        }
    }

    void UpdateUI()
    {
        m_healthTextUI.text = Health.ToString();
        m_staminaTextUI.text = Stamina.ToString();

        m_gameTextUI.text = m_gameText;
    }


    #region Level 1 Events

    public void Lvl1_Start()
    {
        SetLevel(1);
        m_gameText = "There is a door in front of me.\r\nI can feel a small breeze coming from the window beside the door.";
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
        else
        {
            m_gameText = "You open the door using the key.";
            GameObject.Find("btn_Choice1").GetComponentInChildren<TextMeshProUGUI>().text = "Go through door";
            SetFlag("is_door_open", true);
            Stamina -= 1;
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
        SetLevel(2);
        m_gameText = "You enter a dark hallway.\nThe air is humid and the walls and floor are wet\nYou hear clanking in the distance.";
    }

    #endregion

    public void SetLevel(int level)
    {
        m_level = level;
        for (int i = 0; i < m_levelChoices.Count; i++)
        {
            if (i == m_level - 1)
            {
                m_levelChoices[i].SetActive(true);
                continue;
            }

            m_levelChoices[i].SetActive(false);
        }
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
