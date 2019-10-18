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
    public sealed class CreateNewRoomPanel : MonoBehaviour
    {
        [Inject] private WaitPanel waitPanel;

        [SerializeField] TMP_InputField field;

        [SerializeField] TextMeshProUGUI playerCountText;
        [SerializeField] TextMeshProUGUI npcCountText;
        [SerializeField] TextMeshProUGUI widthText;
        [SerializeField] TextMeshProUGUI heightText;
        [SerializeField] TextMeshProUGUI trueText;
        [SerializeField] TextMeshProUGUI falseText;

        [SerializeField] Button playerCountNext;
        [SerializeField] Button playerCountPrev;
        [SerializeField] Button npcCountNext;
        [SerializeField] Button npcCountPrev;
        [SerializeField] Button widthNext;
        [SerializeField] Button widthPrev;
        [SerializeField] Button heightNext;
        [SerializeField] Button heightPrev;
        [SerializeField] Button isPrivateButton;

        [SerializeField] MessagePanel messagePanel;

        ReactiveProperty<byte> playerCount = new ReactiveProperty<byte>();
        ReactiveProperty<int> npcCount = new ReactiveProperty<int>();
        ReactiveProperty<int> width = new ReactiveProperty<int>();
        ReactiveProperty<int> height = new ReactiveProperty<int>();
        ReactiveProperty<bool> isPrivate = new ReactiveProperty<bool>();

        public void OnClickClose() => gameObject.SetActive(false);

        private void Start()
        {
            field.ActivateInputField();

            playerCount.SkipLatestValueOnSubscribe().Subscribe(_count => { playerCountText.text = _count.ToString(); });
            npcCount.SkipLatestValueOnSubscribe().Subscribe(_count => { npcCountText.text = _count.ToString(); });
            width.SkipLatestValueOnSubscribe().Subscribe(_count => { widthText.text = _count.ToString(); });
            height.SkipLatestValueOnSubscribe().Subscribe(_count => { heightText.text = _count.ToString(); });
            isPrivate.SkipLatestValueOnSubscribe().Subscribe(_isPrivate => {
                trueText.gameObject.SetActive(_isPrivate);
                falseText.gameObject.SetActive(!_isPrivate);
            });

            playerCountNext.onClick.AddListener(() => playerCount.Value++);
            playerCountPrev.onClick.AddListener(() => playerCount.Value--);
            npcCountNext.onClick.AddListener(() => npcCount.Value++);
            npcCountPrev.onClick.AddListener(() => npcCount.Value--);
            widthNext.onClick.AddListener(() => width.Value++);
            widthPrev.onClick.AddListener(() => width.Value--);
            heightNext.onClick.AddListener(() => height.Value++);
            heightPrev.onClick.AddListener(() => height.Value--);
            isPrivateButton.onClick.AddListener(() => isPrivate.Value = !isPrivate.Value);
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

        private void CreateNewRoom(string roomName)
        {
            //オプション設定
            var option = new RoomOptions()
            {
                MaxPlayers = playerCount.Value,
                IsVisible = !isPrivate.Value,
                CustomRoomProperties = new ExitGames.Client.Photon.Hashtable() {
                    { "DisplayName", $"{ roomName }" },
                    { "PlayerCount", new MaxTeamCount(){ Value = playerCount.Value } },
                    { "NpcCount", new MaxTeamCount(){ Value = npcCount.Value } },
                    { "BordSize", new BoardSize(){ Value = new int2(){x = width.Value, y = height.Value }} },
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

            waitPanel.gameObject.SetActive(true);
        }

        public void OnClickLeave() {

            Debug.Log("部屋を出ます");
            PhotonNetwork.LeaveRoom();
        }
    }
}

