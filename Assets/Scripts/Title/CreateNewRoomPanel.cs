using Photon.Pun;
using Photon.Realtime;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Zenject;

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

        public void OnEndEdit()
        {
            var inputText = field.text;

            //取り敢えず英数字のみ許容、文字数は1～7
            var firstLength = inputText.Length;
            var pattern = new Regex(@"^[a-z0-9]+$");
            var match = pattern.Match(inputText);
            if (!(1 <= match.Length && match.Length <= 7))
            {
                var panel = Instantiate(messagePanel, this.transform);
                panel.Initialized("Please enter between 1 and 7 characters", null);
            }
        }

        private void CreateNewRoom()
        {
            PhotonNetwork.CreateRoom(
                   null, // 自動的にユニークなルーム名を生成する
               new RoomOptions()
               {
                   MaxPlayers = 4,
                   CustomRoomProperties = new ExitGames.Client.Photon.Hashtable() {
                    { "DisplayName", $"{PhotonNetwork.NickName}の部屋" },
                    { "Message", "誰でも参加OK!" }
                   },
                   CustomRoomPropertiesForLobby = new[] {
                    "DisplayName",
                    "Message"
                   }
               }
           );
        }

       
    }
}

