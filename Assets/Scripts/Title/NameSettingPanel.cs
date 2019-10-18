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

        [Inject] private InputValidation inputValidation;

        [SerializeField] TMP_InputField field;

        public void OnClickClose() => gameObject.SetActive(false);

        public void OnClickNext() {
            OnEndEdit();
            matchingPanel.gameObject.SetActive(true);
        }
        
        public void OnEndEdit()
        {
            var correctText = inputValidation.CheckInputString(field.text, this.gameObject);
            if (!string.IsNullOrEmpty(correctText))
            {
                PhotonNetwork.LocalPlayer.NickName = correctText;
                matchingPanel.gameObject.SetActive(true);
            }
        }
    }
}

