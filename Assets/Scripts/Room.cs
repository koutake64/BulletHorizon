using UnityEngine;
using UnityEngine.UI;
using Photon.Realtime;
using Photon.Pun;

public class Room : MonoBehaviour
{
    public Text buttonText;

    private RoomInfo info;

    public void RegisterRoomDetails(RoomInfo info)
    {
        // ���[�����i�[
        this.info = info;

        // UI
        buttonText.text = this.info.Name;
    }

    // ���̃��[���{�^�����Ǘ����Ă��郋�[���ɎQ��
    public void OpenRoom()
    {
        // ���[���Q���֐����Ăяo��
        PhotonMng.instance.JoinRoom(info);
    }
}
