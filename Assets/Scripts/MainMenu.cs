using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class MainMenu : MonoBehaviour
{
    public List<string> MainMenuTexts;

    [SerializeField] TextMeshProUGUI m_menuText;

    void OnEnable() {
        var rnd = MainMenuTexts[Random.Range(0, MainMenuTexts.Count)];
        m_menuText.text = rnd;
    }
}
