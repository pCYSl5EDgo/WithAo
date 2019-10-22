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
            if (PhotonNetwork.IsConnected == false)
            {
                MyCustomType.Register();
                PhotonNetwork.ConnectUsingSettings();
            }
        }

        public override void OnConnectedToMaster()
        {
            if (PhotonNetwork.IsConnected)
            {
                PhotonNetwork.JoinLobby();
            }
        }

        // ルームリストが更新された時に呼ばれるコールバック
        public override void OnRoomListUpdate(List<RoomInfo> roomList)
        {
            RoomList = roomList;
        }

        // 部屋に入室した時
        public override void OnJoinedRoom()
        {
            waitPanel.UpdatePlayerCount();
        }

        // 他のプレイヤーが入室してきた時
        public override void OnPlayerEnteredRoom(Player newPlayer)
        {
            waitPanel.UpdatePlayerCount();
        }

        // 他のプレイヤーが退室した時
        public override void OnPlayerLeftRoom(Player otherPlayer)
        {
            waitPanel.UpdatePlayerCount();
        }
    }
}