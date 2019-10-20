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
                return null;
            }

            //取り敢えず英数字のみ許容、文字数は1～7
            var firstLength = inputText.Length;
            var pattern = new Regex(@"^[a-z0-9]+$");
            var match = pattern.Match(inputText);
            if (!(1 <= match.Length && match.Length <= 7))
            {
                if(_messagePanel == null)
                {
                    _messagePanel = Instantiate(messagePanel, deployer.transform);
                    _messagePanel.Initialized("Please enter between 1 and 7 characters", null);
                }
                return null;
            }
            return match.ToString();
        }

        public int CheckInputNumber(string inputText, GameObject deployer, bool isNpcCount = false)
        {
            var correctCount = isNpcCount ? 0 : 1;
            if (string.IsNullOrEmpty(inputText))
            {
                return correctCount;
            }

            var firstLength = inputText.Length;
            var pattern = new Regex(@"^[0-9]+$");
            var match = pattern.Match(inputText);
            if (10 < match.Length)
            {
                correctCount = isNpcCount ? 0 : 1;
                SetCautionNumber(deployer);
            } else if (10 == match.Length)
            {
                var str = match.ToString().ToCharArray();
                var isOk = true; 
                for (int i = 0; i <= 10; ++i)
                {
                    if(i == 0)
                    {
                        isOk = (str[0] <= '2' && '1' <= str[0]);
                    }
                    else
                    {
                        if(str[i] != '0')isOk = false;
                    }
                }
                if (!isOk)
                {
                    correctCount = isNpcCount ? 0 : 1;
                    SetCautionNumber(deployer);
                }
            }
            else
            {
                correctCount = Int32.Parse(match.ToString());
            }
            Debug.Log("返すよ");
            Debug.Log(correctCount);
            return correctCount;
        }

        private void SetCautionNumber(GameObject deployer)
        {
            if(_messagePanel == null)
            {
                _messagePanel = Instantiate(messagePanel, deployer.transform);
                _messagePanel.Initialized("Number : Minimum 1 Maximum 2000000000", null);
            }
        }
    }
}

