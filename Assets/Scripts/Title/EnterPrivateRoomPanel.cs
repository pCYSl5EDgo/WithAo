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

        [Inject] private InputValidation inputValidation;

        [SerializeField] TMP_InputField field;

        public void OnClickClose() => gameObject.SetActive(false);

        private void Start()
        {
            field.ActivateInputField();
        }

        //TODO:後でまとめる
        public void OnEndEdit()
        {
            var correctText = inputValidation.CheckInputString(field.text, this.gameObject);
            if (!string.IsNullOrEmpty(correctText))
            {
                EnterPrivateRoom(correctText);
            }
        }

        private void EnterPrivateRoom(string roomName)
        {
            if (PhotonNetwork.InLobby)
            {
                PhotonNetwork.JoinRoom(roomName);
                waitPanel.gameObject.SetActive(true);
            }
        }
    }
}

