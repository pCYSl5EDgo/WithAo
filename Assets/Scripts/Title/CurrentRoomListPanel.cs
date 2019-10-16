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
    public sealed class CurrentRoomListPanel : MonoBehaviourPunCallbacks
    {
        [SerializeField] RectTransform content;

        [SerializeField] CurrentRoomIcon currentRoomIcon;

        [Inject] private PunNetwork punNetwork;

        public void OnClickClose() => gameObject.SetActive(false);

        private Dictionary<string, CurrentRoomIcon> activeEntries = new Dictionary<string, CurrentRoomIcon>();
        private Stack<CurrentRoomIcon> inactiveEntries = new Stack<CurrentRoomIcon>();

        private void OnEnable()
        {
            var roomList = punNetwork.RoomList;
            foreach (var info in roomList)
            {
                Debug.Log($"表示１：{info.Name}");
                CurrentRoomIcon entry;
                if (activeEntries.TryGetValue(info.Name, out entry))
                {
                    Debug.Log($"ありました");
                    if (!info.RemovedFromList)
                    {
                        Debug.Log($"更新");
                        // リスト要素を更新する
                        entry.Activate(info);
                    }
                    else
                    {
                        Debug.Log($"削除");
                        // リスト要素を削除する
                        activeEntries.Remove(info.Name);
                        entry.Deactivate();
                        inactiveEntries.Push(entry);
                    }
                }
                else if (!info.RemovedFromList)
                {
                    Debug.Log($"ありません");
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

