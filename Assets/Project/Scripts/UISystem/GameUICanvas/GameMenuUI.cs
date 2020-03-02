using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameMenuUI : DefaultUI
{
    public void Initialize(List<Item> _DropList)
    {
        Transform t = transform.Find("Background/DropList/Grid");
        if (t)
        {
            for (int i = 0; i < t.childCount; ++i)
            {
                if (_DropList.Count - 1 >= i)
                {
                    UnityEngine.UI.Image image = t.GetChild(i).GetComponent<UnityEngine.UI.Image>();
                    image.sprite = _DropList[i].m_ItemImage;
                }
                else
                {
                    t.GetChild(i).gameObject.SetActive(false);
                }
            }
        }

        gameObject.SetActive(false);
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
