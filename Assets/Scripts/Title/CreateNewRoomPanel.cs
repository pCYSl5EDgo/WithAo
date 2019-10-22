using Photon.Pun;
using Photon.Realtime;
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
    public sealed class CreateNewRoomPanel : MonoBehaviour
    {
        [Inject] private WaitPanel waitPanel;
        [Inject] private InputValidation inputValidation;
        [Inject] private ConnectingPanel connectingPanel;

        [SerializeField] private MessagePanel messagePanel;
        MessagePanel _messagePanel;

        [SerializeField] private TMP_InputField roomNameField;
        [SerializeField] private TMP_InputField playerCountField;
        [SerializeField] private TMP_InputField npcCountField;
        [SerializeField] private TMP_InputField widthField;
        [SerializeField] private TMP_InputField heightField;
        [SerializeField] private TMP_InputField foodStorageField;
        [SerializeField] private TMP_InputField energySupplyLocationField;
        [SerializeField] private TMP_InputField matchTimeField;

        [SerializeField] private TextMeshProUGUI trueText;
        [SerializeField] private TextMeshProUGUI falseText;

        [SerializeField] private Button playerCountNext;
        [SerializeField] private Button playerCountPrev;
        [SerializeField] private Button npcCountNext;
        [SerializeField] private Button npcCountPrev;
        [SerializeField] private Button widthNext;
        [SerializeField] private Button widthPrev;
        [SerializeField] private Button heightNext;
        [SerializeField] private Button heightPrev;
        [SerializeField] private Button matchTimeNext;
        [SerializeField] private Button matchTimePrev;
        [SerializeField] private Button foodStorageCountNext;
        [SerializeField] private Button foodStorageCountPrev;
        [SerializeField] private Button energySupplyLocationCountNext;
        [SerializeField] private Button energySupplyLocationCountPrev;
        [SerializeField] private Button isPrivateButton;

        private readonly ReactiveProperty<byte> playerCount = new ReactiveProperty<byte>(1);
        private readonly ReactiveProperty<byte> npcCount = new ReactiveProperty<byte>(0);
        private readonly ReactiveProperty<int> width = new ReactiveProperty<int>(500);
        private readonly ReactiveProperty<int> height = new ReactiveProperty<int>(500);
        private readonly ReactiveProperty<int> matchTime = new ReactiveProperty<int>(10);
        private readonly ReactiveProperty<uint> energySupplyLocationCount = new ReactiveProperty<uint>(1);
        private readonly ReactiveProperty<bool> isPrivate = new ReactiveProperty<bool>(false);

        readonly byte MaxPlayerCount = 16;

        public void OnClickClose() => gameObject.SetActive(false);

        private void Awake()
        {
            roomNameField.ActivateInputField();
            playerCountField.ActivateInputField();
            npcCountField.ActivateInputField();
            widthField.ActivateInputField();
            heightField.ActivateInputField();
            matchTimeField.ActivateInputField();
            foodStorageField.ActivateInputField();
            energySupplyLocationField.ActivateInputField();

            playerCount.Subscribe(_count => { playerCountField.text = _count.ToString(); });
            npcCount.Subscribe(_count => { npcCountField.text = _count.ToString(); });
            width.Subscribe(_count => { widthField.text = _count.ToString(); });
            height.Subscribe(_count => { heightField.text = _count.ToString(); });
            matchTime.Subscribe(_count => { matchTimeField.text = _count.ToString(); });
            energySupplyLocationCount.Subscribe(_count => { energySupplyLocationField.text = _count.ToString(); });
            isPrivate.Subscribe(_isPrivate =>
            {
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
            matchTimeNext.onClick.AddListener(() => matchTime.Value++);
            matchTimePrev.onClick.AddListener(() => matchTime.Value--);
            energySupplyLocationCountNext.onClick.AddListener(() => energySupplyLocationCount.Value++);
            energySupplyLocationCountPrev.onClick.AddListener(() => energySupplyLocationCount.Value--);
            isPrivateButton.onClick.AddListener(() => isPrivate.Value = !isPrivate.Value);
        }

        public void OnEndEdit()
        {
            var correctText = inputValidation.CheckInputString(roomNameField.text, this.gameObject);
            if (string.IsNullOrEmpty(correctText)) return;

            if (!IsEnabelCreate())
            {
                if (_messagePanel == null)
                {
                    _messagePanel = Instantiate(messagePanel, this.gameObject.transform);
                    _messagePanel.Initialized("PlayerCount And NpcCount → Number : Up to 20 in total \n Other → Number: Minimum 1 Maximum 2000000000", null);
                    return;
                }
            }
            CreateNewRoom(correctText);
        }

        private bool IsEnabelCreate()
        {
            return (1 <= playerCount.Value
                    && playerCount.Value <= MaxPlayerCount
                    && npcCount.Value <= MaxPlayerCount - 1
                    && (playerCount.Value + npcCount.Value) <= MaxPlayerCount
                    && 1 <= width.Value && width.Value <= 2000000000
                    && 1 <= height.Value && height.Value <= 2000000000
                    && 1 <= matchTime.Value && matchTime.Value <= 2000000000
                    && 1 <= energySupplyLocationCount.Value && energySupplyLocationCount.Value <= 2000000000);
        }

        private void CreateNewRoom(string roomName)
        {
            //オプション設定
            var option = new RoomOptions()
            {
                MaxPlayers = playerCount.Value,
                IsVisible = !isPrivate.Value,
                CustomRoomProperties = new ExitGames.Client.Photon.Hashtable()
                {
                    {"DisplayName", $"{roomName}"},
                    {"PlayerCount", new MaxTeamCount() {Value = (int) (playerCount.Value)}},
                    {"NpcCount", new MaxTeamCount() {Value = (int) (npcCount.Value)}},
                    {nameof(BoardSize), new BoardSize() {Value = new int2() {x = width.Value, y = height.Value}}},
                    {nameof(MatchTime), new MatchTime() {Value = matchTime.Value}},
                    {"EnergySupplyLocationCount", energySupplyLocationCount.Value},
                },
                CustomRoomPropertiesForLobby = new[]
                {
                    "DisplayName",
                    "PlayerCount",
                    "NpcCount",
                    "BordSize",
                    "MatchTime",
                    "FoodStorageCount",
                    "EnergySupplyLocationCount",
                }
            };

            StartCoroutine(EnterRoom(roomName, option));
        }

        private IEnumerator EnterRoom(string roomName, RoomOptions option)
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
            var isSuccess = PhotonNetwork.CreateRoom(roomName, option);
            if (!isSuccess)
            {
                if (_messagePanel == null)
                {
                    _messagePanel = Instantiate(messagePanel, this.gameObject.transform);
                    _messagePanel.Initialized("Failed to create room. \n Try a different room name", null);
                }
            }
            else
            {
                waitPanel.gameObject.SetActive(true);
            }
        }

        public void OnPlayerCountValueChanged()
        {
            var count = inputValidation.CheckInputNumber(playerCountField.text, this.gameObject);
            if (MaxPlayerCount < count + npcCount.Value)
            {
                playerCountField.enabled = false;
                count = MaxPlayerCount - npcCount.Value;
                playerCount.Value = (byte) (count);
                if (_messagePanel == null)
                {
                    _messagePanel = Instantiate(messagePanel, this.gameObject.transform);
                    _messagePanel.Initialized("Number : Up to 16 in total",
                        () => { playerCountField.enabled = true; });
                }
            }
            else
            {
                playerCount.Value = (byte) (count);
            }
        }

        public void OnNpcCountValueChanged()
        {
            var count = inputValidation.CheckInputNumber(npcCountField.text, this.gameObject, true);
            if (MaxPlayerCount < count + playerCount.Value)
            {
                npcCountField.enabled = false;
                count = MaxPlayerCount - playerCount.Value;
                if (_messagePanel == null)
                {
                    _messagePanel = Instantiate(messagePanel, this.gameObject.transform);
                    _messagePanel.Initialized("Number : Up to 16 in total",
                        () =>
                        {
                            npcCountField.enabled = true;
                            npcCount.Value = (byte) (count);
                        });
                }
            }
            else
            {
                npcCount.Value = (byte) (count);
            }
        }

        public void OnWidthCountValueChanged()
        {
            var count = inputValidation.CheckInputNumber(widthField.text, this.gameObject);
            width.Value = count;
        }

        public void OnHeightCountValueChanged()
        {
            var count = inputValidation.CheckInputNumber(heightField.text, this.gameObject);
            height.Value = count;
        }

        public void OnMatchTimeCountValueChanged()
        {
            var count = inputValidation.CheckInputNumber(matchTimeField.text, this.gameObject);
            matchTime.Value = count;
        }

        public void OnEnergySupplyLocationCountValueChanged()
        {
            var count = inputValidation.CheckInputNumber(energySupplyLocationField.text, this.gameObject);
            energySupplyLocationCount.Value = (uint) count;
        }
    }
}