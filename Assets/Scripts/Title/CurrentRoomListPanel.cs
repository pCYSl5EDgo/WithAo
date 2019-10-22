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
    public sealed class CurrentRoomListPanel : MonoBehaviour
    {
        [SerializeField] private RectTransform content;

        [SerializeField] private CurrentRoomIcon currentRoomIcon;

        [Inject] private PunNetwork punNetwork;

        public void OnClickClose() => gameObject.SetActive(false);

        private readonly Dictionary<string, CurrentRoomIcon> activeEntries = new Dictionary<string, CurrentRoomIcon>();
        private readonly Stack<CurrentRoomIcon> inactiveEntries = new Stack<CurrentRoomIcon>();

        private void OnEnable()
        {
            var roomList = punNetwork.RoomList;
            if(roomList is null) return;
            foreach (var info in roomList)
            {
                if (activeEntries.TryGetValue(info.Name, out var entry))
                {
                    if (!info.RemovedFromList)
                    {
                        // リスト要素を更新する
                        entry.Activate(info);
                    }
                    else
                    {
                        // リスト要素を削除する
                        activeEntries.Remove(info.Name);
                        entry.Deactivate();
                        inactiveEntries.Push(entry);
                    }
                }
                else if (!info.RemovedFromList)
                {
                    // リスト要素を追加する
                    entry = (inactiveEntries.Count > 0)
                        ? inactiveEntries.Pop().SetAsLastSibling()
                        : Instantiate(currentRoomIcon, content);
                    entry.Activate(info);
                    activeEntries.Add(info.Name, entry);
                }
            }
        }
    }
}

