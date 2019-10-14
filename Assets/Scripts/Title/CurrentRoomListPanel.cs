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

        public void OnClickClose() => gameObject.SetActive(false);

        private Dictionary<string, CurrentRoomIcon> activeEntries = new Dictionary<string, CurrentRoomIcon>();
        private Stack<CurrentRoomIcon> inactiveEntries = new Stack<CurrentRoomIcon>();

        // ルームリストが更新された時に呼ばれるコールバック
        public override void OnRoomListUpdate(List<RoomInfo> roomList)
        {
            Debug.Log("作られました！");
            foreach (var info in roomList)
            {
                CurrentRoomIcon entry;
                if (activeEntries.TryGetValue(info.Name, out entry))
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

