using Photon.Pun;
using Photon.Realtime;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;
using Zenject;
using AoAndSugi.Game.Models;
using Unity.Mathematics;

namespace AoAndSugi
{
    public sealed class CurrentRoomIcon : MonoBehaviour
    {
        [Inject] private WaitPanel waitPanel;

        [SerializeField] RectTransform rectTransform;
        [SerializeField] TextMeshProUGUI roomName;
        [SerializeField] TextMeshProUGUI mapSize;
        [SerializeField] TextMeshProUGUI playerCount;
        [SerializeField] TextMeshProUGUI npcCount;
        [SerializeField] TextMeshProUGUI matchTime;

        public void OnClickButton() {
            PhotonNetwork.JoinRoom(roomName.text);
            waitPanel.gameObject.SetActive(true);
        }

        public void Activate(RoomInfo info)
        {
            roomName.text = info.Name;
            playerCount.text = $"{info.PlayerCount}/{info.MaxPlayers}";

            object value = null;
            if (info.CustomProperties.TryGetValue("BordSize", out value))
            {
                int2 size = ((BoardSize)value).Value;
                mapSize.text = $"{ size.x }*{ size.y }";
            }
            value = null;
            if (info.CustomProperties.TryGetValue("NpcCount", out value))
            {
                int count = ((MaxTeamCount)value).Value;
                npcCount.text = $"{ count }";
            }
            value = null;
            if (info.CustomProperties.TryGetValue("MatchTime", out value))
            {
                int time = ((MatchTime)value).Value;
                matchTime.text = $"{ time }";
            }
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

