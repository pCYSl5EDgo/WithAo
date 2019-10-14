using Photon.Pun;
using Photon.Realtime;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Zenject;

namespace AoAndSugi
{
    public sealed class CurrentRoomListPanel : MonoBehaviour
    {
        [SerializeField] RectTransform content;

        public void OnClickClose() => gameObject.SetActive(false);

    }
}

