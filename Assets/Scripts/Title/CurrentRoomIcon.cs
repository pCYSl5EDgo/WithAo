using Photon.Pun;
using Photon.Realtime;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

namespace AoAndSugi
{
    public sealed class CurrentRoomIcon : MonoBehaviour
    {
        [SerializeField] RectTransform rectTransform;
        [SerializeField] TextMeshProUGUI roomName;
        [SerializeField] TextMeshProUGUI count;

        public void OnClickButton() {
            PhotonNetwork.JoinRoom(roomName.text);
        }

        public void Activate(RoomInfo info)
        {
            roomName.text = info.Name;
            count.text = $"{info.PlayerCount}/{info.MaxPlayers}";
        }

        public void Deactivate()
        {
            this.gameObject.SetActive(false);
        }

        public CurrentRoomIcon SetAsLastSibling()
        {
            rectTransform.SetAsLastSibling();
            return this;
        }
    }
}

