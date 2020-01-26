using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class LobbyDefaultUI : MonoBehaviour
{
    LobbyCanvasUI m_LobbyCanvasUI;

    [SerializeField]
    TMP_Text m_LevelText;
    [SerializeField]
    TMP_Text m_StaminLeftText;
    [SerializeField]
    TMP_Text m_StaminRightText;
    [SerializeField]
    UnityEngine.UI.Slider m_StaminaSlider;
    [SerializeField]
    TMP_Text m_MoneyText;
    [SerializeField]
    TMP_Text m_JewelText;

    public void Initialize(LobbyCanvasUI _LobbyCanvasUI)
    {
        m_LobbyCanvasUI = _LobbyCanvasUI;
    }

    public void StatusButton()
    {
        m_LobbyCanvasUI.ResetUIDepth();
        m_LobbyCanvasUI.GetStatusUI().Open();
    }

    public void InventoryButton()
    {
        m_LobbyCanvasUI.ResetUIDepth();
        m_LobbyCanvasUI.GetInventoryUI().Open();
    }

    public void MenuButton()
    {
        m_LobbyCanvasUI.ResetUIDepth();
        m_LobbyCanvasUI.GetMenuUI().Open();
    }

    public void BackButton()
    {
        if (m_LobbyCanvasUI.GetCurrentUIDepth() < 1)
        {
            MessageBox.CreateTwoButtonType("게임을 종료하시겠습니까?", "YES", GameManager.Instance.QuitGame, "NO");
            return;
        }
        m_LobbyCanvasUI.CloseLastUIDepth();
    }

    public void SetLevel(int _Level)
    {
        m_LevelText.text = "Lv " + _Level.ToString();
    }

    public void SetStamina(int _CurrentValue, int _MaxValue)
    {
        m_StaminLeftText.text = _CurrentValue.ToString();
        m_StaminRightText.text = _MaxValue.ToString();
        m_StaminaSlider.value = Mathf.Clamp01((float)_CurrentValue / (float)_MaxValue);
    }

    public void SetMoney(int _Money)
    {
        m_MoneyText.text = _Money.ToString();
    }

    public void Jewel(int _Jewel)
    {
        m_JewelText.text = _Jewel.ToString();
    }
}
