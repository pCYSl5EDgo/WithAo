using Photon.Pun;
using Photon.Realtime;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Zenject;
using System.Collections.Generic;
using UniRx;
using Unity.Mathematics;
using AoAndSugi.Game.Models;

namespace AoAndSugi
{
    public sealed class EnterPrivateRoomPanel : MonoBehaviour
    {
        [Inject] private WaitPanel waitPanel;

        [SerializeField] TMP_InputField field;

        [SerializeField] MessagePanel messagePanel;

        public void OnClickClose() => gameObject.SetActive(false);

        private void Start()
        {
            field.ActivateInputField();
        }

        //TODO:後でまとめる
        public void OnEndEdit()
        {
            var inputText = field.text;
            if (string.IsNullOrEmpty(inputText))
            {
                return;
            }

            //取り敢えず英数字のみ許容、文字数は1～7
            var firstLength = inputText.Length;
            var pattern = new Regex(@"^[a-z0-9]+$");
            var match = pattern.Match(inputText);
            if (!(1 <= match.Length && match.Length <= 7))
            {
                var panel = Instantiate(messagePanel, this.transform);
                panel.Initialized("Please enter between 1 and 7 characters", null);
                return;
            }
            EnterPrivateRoom(match.ToString());
        }

        private void EnterPrivateRoom(string roomName)
        {
            PhotonNetwork.JoinRoom(roomName);

            waitPanel.gameObject.SetActive(true);
        }

        public void OnClickLeave()
        {
            Debug.Log("部屋を出ます");
            PhotonNetwork.LeaveRoom();
        }
    }
}

