using Photon.Pun;
using System.Collections.Generic;
using Photon.Realtime;
using UnityEngine;
using Zenject;

namespace AoAndSugi
{
    public class PunNetwork : MonoBehaviourPunCallbacks
    {
        [Inject] private WaitPanel waitPanel;

        public List<RoomInfo> RoomList{ get; private set; }

        private void Start()
        {
            DontDestroyOnLoad(this);
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

        // 部屋に入室した時
        public override void OnJoinedRoom()
        {
            Debug.Log("OnJoinedRoom");
            waitPanel.UpdatePlayerCount();
        }

        // 他のプレイヤーが入室してきた時
        public override void OnPlayerEnteredRoom(Player newPlayer)
        {
            Debug.Log("新しくプレイヤーが入室しました！");
            Debug.Log($"現在の人数:{PhotonNetwork.CurrentRoom.PlayerCount}");
            waitPanel.UpdatePlayerCount();
        }

        // 他のプレイヤーが退室した時
        public override void OnPlayerLeftRoom(Player otherPlayer)
        {
            Debug.Log("プレイヤーが退出しました");
            Debug.Log($"現在の人数:{PhotonNetwork.CurrentRoom.PlayerCount}");
            waitPanel.UpdatePlayerCount();
        }
    }
}