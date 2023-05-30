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
    
    [Header("UI")]
    [SerializeField] private TextMeshProUGUI m_gameTextUI;
    [SerializeField] private TextMeshProUGUI m_healthTextUI;
    [SerializeField] private TextMeshProUGUI m_staminaTextUI;

    void Start()
    {

    }

    private void Update()
    {
        UpdateUI();
    }

    void UpdateUI()
    {
        m_healthTextUI.text = Health.ToString();
        m_staminaTextUI.text = Stamina.ToString();

        m_gameTextUI.text = m_gameText;
    }


    #region Game Events

    public void Lvl1_OpenDoor()
    {
        if (Stamina <= 0)
        {
            m_gameText = "You're tired";
            return;
        }
        m_gameText = "The door won't budge.";
        Stamina -= 1;
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
        m_gameText = "You wait. [10STA]";
        Stamina = 10;
    }

    #endregion
}
