using Photon.Pun;
using Photon.Realtime;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Zenject;
using System.Collections.Generic;
using UniRx;

namespace AoAndSugi
{
    public sealed class CreateNewRoomPanel : MonoBehaviour
    {
        [SerializeField] TMP_InputField field;

        [SerializeField] TextMeshProUGUI playerCountText;
        [SerializeField] TextMeshProUGUI npcCountText;
        [SerializeField] TextMeshProUGUI widthText;
        [SerializeField] TextMeshProUGUI heightText;

        [SerializeField] Button playerCountNext;
        [SerializeField] Button playerCountPrev;
        [SerializeField] Button npcCountNext;
        [SerializeField] Button npcCountPrev;
        [SerializeField] Button widthNext;
        [SerializeField] Button widthPrev;
        [SerializeField] Button heightNext;
        [SerializeField] Button heightPrev;

        [SerializeField] MessagePanel messagePanel;

        ReactiveProperty<int> playerCount = new ReactiveProperty<int>();

        public void OnClickClose() => gameObject.SetActive(false);

        private void Start()
        {
            field.ActivateInputField();

            playerCount.SkipLatestValueOnSubscribe().Subscribe(_count => { playerCountText.text = _count.ToString(); });

            playerCountNext.onClick.AddListener(() => playerCount.Value++);
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
            CreateNewRoom(match.ToString(), 1, 1, 1, 1);
        }

        public void OnClickAutoButton()
        {
            //なくす
        }



        private void CreateNewRoom(string roomName, byte playerCount, int npcCount, int height, int width)
        {
            //オプション設定
            var option = new RoomOptions()
            {
                MaxPlayers = playerCount,
                IsVisible = true,
                CustomRoomProperties = new ExitGames.Client.Photon.Hashtable() {
                    { "DisplayName", $"{ roomName }" },
                    { "PlayerCount", $"{ playerCount }"},
                    { "NpcCount", $"{ npcCount }" },
                    { "Height", $"{ height }" },
                    { "Width", $"{ width }" },
                   },
                CustomRoomPropertiesForLobby = new[] {
                    "DisplayName",
                    "PlayerCount",
                    "NpcCount",
                    "Height",
                    "Width"
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

        public void OnClickLeave() {

            Debug.Log("部屋を出ます");
            PhotonNetwork.LeaveRoom();
        }
    }
}

