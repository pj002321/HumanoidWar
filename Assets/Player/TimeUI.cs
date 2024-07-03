using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

namespace Timer
{
    public class TimeUI : MonoBehaviour
    {
        [SerializeField] TextMeshProUGUI timerText;
        [SerializeField] TextMeshProUGUI gameoverText;
        public static float elapsedTime = 180f;
        int minutes;
        int seconds;

        #region UnityMethods
        private void Awake()
        {
            resetTime();
        }

     
        private void Update()
        {
            if (elapsedTime <= 0)
            {
                Debug.Log("TimeOver");
                timerText.text = string.Format("{0:00}:{0:00}", 0, 0);
                gameoverText.gameObject.SetActive(true);
                elapsedTime = 0;     
            }
            else
            {
                gameoverText.gameObject.SetActive(false);
                elapsedTime -= Time.deltaTime;
                minutes = Mathf.FloorToInt(elapsedTime / 60);
                seconds = Mathf.FloorToInt(elapsedTime % 60);
                timerText.text = string.Format("{0:00}:{1:00}", minutes, seconds);
            }
        }
        #endregion UnityMethods

        public void resetTime()
        {
            elapsedTime = 180f;
        }
    }
}
