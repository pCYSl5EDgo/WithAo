using Photon.Pun;
using Photon.Realtime;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;
using Zenject;

namespace AoAndSugi
{
    public sealed class NameSettingPanel : MonoBehaviour
    {
        [Inject] private MatchingPanel matchingPanel;

        [SerializeField] MessagePanel messagePanel;

        [SerializeField] TMP_InputField field;

        public void OnClickClose() => gameObject.SetActive(false);

        public void OnClickNext() {
            OnEndEdit();
            matchingPanel.gameObject.SetActive(true);
        }

        //TODO:後でまとめる
        public void OnEndEdit()
        {
            var inputText = field.text;
            if (string.IsNullOrEmpty(inputText))
            {
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
            PhotonNetwork.LocalPlayer.NickName = match.ToString();
            matchingPanel.gameObject.SetActive(true);
        }
    }
}

