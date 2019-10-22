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
using System;

namespace AoAndSugi
{
    public sealed class CreateNewRoomPanel : MonoBehaviour
    {
        [Inject] private WaitPanel waitPanel;
        [Inject] private InputValidation inputValidation;

        [SerializeField] MessagePanel messagePanel;
        MessagePanel _messagePanel;

        [SerializeField] TMP_InputField roomNameField;
        [SerializeField] TMP_InputField playerCountField;
        [SerializeField] TMP_InputField npcCountField;
        [SerializeField] TMP_InputField widthField;
        [SerializeField] TMP_InputField heightField;
        [SerializeField] TMP_InputField foodStorageField;
        [SerializeField] TMP_InputField energySupplyLocationField;
        [SerializeField] TMP_InputField matchTimeField;
        
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
        [SerializeField] Button matchTimeNext;
        [SerializeField] Button matchTimePrev;
        [SerializeField] Button foodStorageCountNext;
        [SerializeField] Button foodStorageCountPrev;
        [SerializeField] Button energySupplyLocationCountNext;
        [SerializeField] Button energySupplyLocationCountPrev;
        [SerializeField] Button isPrivateButton;

        ReactiveProperty<byte> playerCount = new ReactiveProperty<byte>(1);
        ReactiveProperty<byte> npcCount = new ReactiveProperty<byte>(0);
        ReactiveProperty<int> width = new ReactiveProperty<int>(500);
        ReactiveProperty<int> height = new ReactiveProperty<int>(500);
        ReactiveProperty<int> matchTime = new ReactiveProperty<int>(10);
        ReactiveProperty<uint> foodStorageCount = new ReactiveProperty<uint>(1);
        ReactiveProperty<uint> energySupplyLocationCount = new ReactiveProperty<uint>(1);
        ReactiveProperty<bool> isPrivate = new ReactiveProperty<bool>(false);

        readonly byte MaxPlayerCount = 16;

        public void OnClickClose() => gameObject.SetActive(false);

        private void Start()
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
            foodStorageCount.Subscribe(_count => { foodStorageField.text = _count.ToString(); });
            energySupplyLocationCount.Subscribe(_count => { energySupplyLocationField.text = _count.ToString(); });
            isPrivate.Subscribe(_isPrivate => {
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
            foodStorageCountNext.onClick.AddListener(() => foodStorageCount.Value++);
            foodStorageCountPrev.onClick.AddListener(() => foodStorageCount.Value--);
            energySupplyLocationCountNext.onClick.AddListener(() => energySupplyLocationCount.Value++);
            energySupplyLocationCountPrev.onClick.AddListener(() => energySupplyLocationCount.Value--);
            isPrivateButton.onClick.AddListener(() => isPrivate.Value = !isPrivate.Value);
        }
        
        public void OnEndEdit()
        {
            var correctText = inputValidation.CheckInputString(roomNameField.text, this.gameObject);
            if (!string.IsNullOrEmpty(correctText))
            {
                if (!IsEnabelCreate())
                {
                    _messagePanel = Instantiate(messagePanel, this.gameObject.transform);
                    _messagePanel.Initialized("PlayerCount And NpcCount → Number : Up to 20 in total \n Other → Number: Minimum 1 Maximum 2000000000", null);
                    return;
                }
                CreateNewRoom(correctText);
            }
        }

        private bool IsEnabelCreate()
        {
            return (1 <= playerCount.Value && playerCount.Value <= MaxPlayerCount
                && 0 <= npcCount.Value && npcCount.Value <= MaxPlayerCount-1
                && (playerCount.Value + npcCount.Value) <= MaxPlayerCount
                && 1 <= width.Value && width.Value <= 2000000000
                && 1 <= height.Value && height.Value <= 2000000000
                && 1 <= matchTime.Value && matchTime.Value <= 2000000000
                && 1 <= foodStorageCount.Value && foodStorageCount.Value <= 2000000000
                && 1 <= energySupplyLocationCount.Value && energySupplyLocationCount.Value <= 2000000000);
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
                    { "PlayerCount", new MaxTeamCount(){ Value = (int)(playerCount.Value) } },
                    { "NpcCount", new MaxTeamCount(){ Value = (int)(npcCount.Value) } },
                    { "BordSize", new BoardSize(){ Value = new int2(){ x = width.Value, y = height.Value }} },
                    { "MatchTime", new MatchTime(){ Value = matchTime.Value } },
                    { "FoodStorageCount", foodStorageCount.Value },
                    { "EnergySupplyLocationCount", energySupplyLocationCount.Value },
                   },
                CustomRoomPropertiesForLobby = new[] {
                    "DisplayName",
                    "PlayerCount",
                    "NpcCount",
                    "BordSize",
                    "MatchTime",
                    "FoodStorageCount",
                    "EnergySupplyLocationCount",
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
                _messagePanel = Instantiate(messagePanel, this.gameObject.transform);
                _messagePanel.Initialized("Failed to create room. \n Try a different room name", null);
            }

            waitPanel.gameObject.SetActive(true);
        }

        public void OnPlayerCountValueChanged()
        { 
            var count = inputValidation.CheckInputNumber(playerCountField.text, this.gameObject);
            if (MaxPlayerCount < count + npcCount.Value)
            {
                playerCountField.enabled = false;
                count = MaxPlayerCount - npcCount.Value;
                //playerCount.Value = 20;//一度別の値を入れないと値が正常に反映されないことがある
                playerCount.Value = (byte)(count);
                if (_messagePanel == null)
                {
                    _messagePanel = Instantiate(messagePanel, this.gameObject.transform);
                    _messagePanel.Initialized("Number : Up to 16 in total", 
                        () => {
                            playerCountField.enabled = true;
                        });
                }
            }
            else
            {
                //playerCount.Value = 20;//一度別の値を入れないと値が正常に反映されないことがある
                playerCount.Value = (byte)(count);
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
                        () => {
                            npcCountField.enabled = true;
                            npcCount.Value = (byte)(count);
                        });
                }
            }
            else
            {
                npcCount.Value = (byte)(count);
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

        public void OnFoodStorageCountValueChanged()
        {
            var count = inputValidation.CheckInputNumber(foodStorageField.text, this.gameObject);
            foodStorageCount.Value = (uint)count;
        }

        public void OnEnergySupplyLocationCountValueChanged()
        {
            var count = inputValidation.CheckInputNumber(energySupplyLocationField.text, this.gameObject);
            energySupplyLocationCount.Value = (uint)count;
        }
    }
}

