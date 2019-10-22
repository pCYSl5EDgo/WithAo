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
    public sealed class InputValidation : MonoBehaviour
    {
        [SerializeField] MessagePanel messagePanel;
        MessagePanel _messagePanel;

        public string CheckInputString(string inputText, GameObject deployer)
        {
            if (string.IsNullOrEmpty(inputText))
            {
                SetCautionName(deployer);
                return null;
            }

            //取り敢えず英数字のみ許容、文字数は1～7
            var firstLength = inputText.Length;
            var pattern = new Regex(@"^[a-z0-9]+$");
            var match = pattern.Match(inputText);
            if (!(1 <= match.Length && match.Length <= 7))
            {
                SetCautionName(deployer);
                return null;
            }
            return match.ToString();
        }

        private void SetCautionName(GameObject deployer)
        {
            if (_messagePanel == null)
            {
                _messagePanel = Instantiate(messagePanel, deployer.transform);
                _messagePanel.Initialized("Please enter between 1 and 7 characters", null);
            }
        }

        public int CheckInputNumber(string inputText, GameObject deployer, bool isNpcCount = false)
        {
            var number = isNpcCount ? 0 : 1;
            var isSuccess = Int32.TryParse(inputText, out number);
            if (!isSuccess || number < 0)
            {
                SetCautionNumber(deployer);
            }
            return number;
        }

        private void SetCautionNumber(GameObject deployer)
        {
            if(_messagePanel == null)
            {
                _messagePanel = Instantiate(messagePanel, deployer.transform);
                _messagePanel.Initialized("Number : Minimum 1 Maximum 2147483647", null);
            }
        }
    }
}

