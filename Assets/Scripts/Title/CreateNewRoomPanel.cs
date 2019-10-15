using Photon.Pun;
using Photon.Realtime;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Zenject;
using System.Collections.Generic;

namespace AoAndSugi
{
    public sealed class CreateNewRoomPanel : MonoBehaviour
    {
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
            if(string.IsNullOrEmpty(inputText)){
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
            CreateNewRoom(match.ToString());
        }

        public void OnClickAutoButton()
        {
            CreateNewRoom(null);
        }

        private void CreateNewRoom(string roomName)
        {
            //オプション設定
            var option = new RoomOptions()
            {
                MaxPlayers = 20,
                IsVisible = true,
                CustomRoomProperties = new ExitGames.Client.Photon.Hashtable() {
                    { "DisplayName", $"test" },
                    { "Message", "誰でも参加OK!" }
                   },
                CustomRoomPropertiesForLobby = new[] {
                    "DisplayName",
                    "Message"
                   }
            };

            var a = PhotonNetwork.CreateRoom(roomName, option);
            if (a)
            {
                Debug.Log("成功");
            }
            else
            {
                Debug.Log("失敗");
            }
        }
    }
}

