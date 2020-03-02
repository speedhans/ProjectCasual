using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameDeadUI : MonoBehaviour
{
    public void Initialize()
    {
        Disable();
    }

    public void Enable()
    {
        gameObject.SetActive(true);
    }

    public void Disable()
    {
        gameObject.SetActive(false);
    }

    public void ReviveButton()
    {
        MessageBox.CreateTwoButtonType("캐릭터를 부활시킵니다. (현재 버전에서는 재화가 소비되지 않습니다)", "YES", Revive, "NO");
    }

    void Revive() // 재화 소비 작업 필요
    {
        GameManager.Instance.m_MyCharacter.Revive();
        Disable();
    }

    public void RetireButton()
    {
        MessageBox.CreateTwoButtonType("진행중인 방에서 나가시겠습니까? 스테미나는 사용된 스테미나에서 1을 제외하고 복구됩니다", "YES", Retire, "NO");
    }

    public void Retire()
    {
        NetworkManager.Instance.RoomController.LeaveRoom();
        SceneManager.Instance.LoadScene("LobbyScene");
    }
}
