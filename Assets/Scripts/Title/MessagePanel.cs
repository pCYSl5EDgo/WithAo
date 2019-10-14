using Photon.Pun;
using Photon.Realtime;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

namespace AoAndSugi
{
    public sealed class MessagePanel : MonoBehaviour
    {
        [SerializeField] TextMeshProUGUI text;
        private Action CloseAction;

        public void OnClickClose()
        {
            CloseAction?.Invoke();
            Destroy(this.gameObject);
        }

        public void Initialized(string message, Action CloseAction)
        {
            text.text = message;
            this.CloseAction = CloseAction;
            this.gameObject.SetActive(true);
        }
    }
}

