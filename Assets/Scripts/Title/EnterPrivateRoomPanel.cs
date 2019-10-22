using Photon.Pun;
using Photon.Realtime;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Zenject;
using System.Collections;
using UniRx;
using Unity.Mathematics;
using AoAndSugi.Game.Models;

namespace AoAndSugi
{
    public sealed class EnterPrivateRoomPanel : MonoBehaviour
    {
        [Inject] private WaitPanel waitPanel;

        [Inject] private InputValidation inputValidation;

        [Inject] private ConnectingPanel connectingPanel;

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
                StartCoroutine(EnterPrivateRoom(correctText));
            }
        }

        private IEnumerator EnterPrivateRoom(string roomName)
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
            PhotonNetwork.JoinRoom(roomName);
            waitPanel.gameObject.SetActive(true);
        }
    }
}

