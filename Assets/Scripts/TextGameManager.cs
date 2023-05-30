using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class TextGameManager : MonoBehaviour
{
    [Header("Player")]
    public float Health = 100f;
    public float Stamina = 100f;

    [Header("Game")]
    [SerializeField] [TextArea(3, 50)] private string m_storyText = string.Empty;
    
    [Header("UI")]
    [SerializeField] private TextMeshProUGUI m_gameTextUI;
    [SerializeField] private TextMeshProUGUI m_healthTextUI;
    [SerializeField] private TextMeshProUGUI m_staminaTextUI;

    void Start()
    {
        m_healthTextUI.text = Health.ToString();
        m_staminaTextUI.text = Stamina.ToString();

        m_gameTextUI.text = m_storyText;
    }

    void Update()
    {
        
    }
}
