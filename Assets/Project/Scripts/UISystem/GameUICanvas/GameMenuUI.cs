using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameMenuUI : DefaultUI
{
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
