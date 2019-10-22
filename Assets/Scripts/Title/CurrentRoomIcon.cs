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
using System.Collections;

namespace AoAndSugi
{
    public sealed class CurrentRoomIcon : MonoBehaviour
    {
        [Inject] private WaitPanel waitPanel;

        [Inject] private ConnectingPanel connectingPanel;

        [SerializeField] MessagePanel messagePanel;
        MessagePanel _messagePanel;

        [SerializeField] RectTransform rectTransform;
        [SerializeField] TextMeshProUGUI roomName;
        [SerializeField] TextMeshProUGUI mapSize;
        [SerializeField] TextMeshProUGUI playerCount;
        [SerializeField] TextMeshProUGUI npcCount;
        [SerializeField] TextMeshProUGUI matchTime;
        [SerializeField] TextMeshProUGUI foodStorageCount;
        [SerializeField] TextMeshProUGUI energySupplyLocationCount;

        public void OnClickButton() {
            StartCoroutine(EnterRoom());
        }

        private IEnumerator EnterRoom()
        {
            if (!PhotonNetwork.InLobby)
            {
                connectingPanel.gameObject.SetActive(true);
                while (!PhotonNetwork.InLobby)
                {
                    yield return null;
                }
                connectingPanel.gameObject.SetActive(false);
            }
            var isSuccess = PhotonNetwork.JoinRoom(roomName.text);
            if (isSuccess)
            {
                waitPanel.gameObject.SetActive(true);
            }
            else
            {
                if (_messagePanel == null)
                {
                    _messagePanel = Instantiate(messagePanel, this.gameObject.transform);
                    _messagePanel.Initialized("Failed to enter room", null);
                }
            }
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
            value = null;
            if (info.CustomProperties.TryGetValue("FoodStorageCount", out value))
            {
                uint count = (uint)value;
                foodStorageCount.text = $"{ count }";
            }
            value = null;
            if (info.CustomProperties.TryGetValue("EnergySupplyLocationCount", out value))
            {
                uint count = (uint)value;
                energySupplyLocationCount.text = $"{ count }";
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

