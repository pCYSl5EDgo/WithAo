using Photon.Pun;
using System.Collections.Generic;
using Photon.Realtime;
using UnityEngine;

namespace AoAndSugi
{
    public class PunNetwork : MonoBehaviourPunCallbacks
    {
        public List<RoomInfo> RoomList{ get; private set; }

        private void Start()
        {
            Debug.Log("接続");
            PhotonNetwork.ConnectUsingSettings();
        }

        public override void OnConnectedToMaster()
        {
            Debug.Log("ロビーに入るメソッド");
            if (PhotonNetwork.IsConnected)
            {
                Debug.Log("ロビーに入るよ");
                PhotonNetwork.JoinLobby();
            }
        }

        // ルームリストが更新された時に呼ばれるコールバック
        public override void OnRoomListUpdate(List<RoomInfo> roomList)
        {
            Debug.Log("作られました！");
            Debug.Log($"要素;{roomList.Count}");
            foreach (var room in roomList)
            {
                Debug.Log(room.Name);
            }
            RoomList = roomList;
        }
    }
}