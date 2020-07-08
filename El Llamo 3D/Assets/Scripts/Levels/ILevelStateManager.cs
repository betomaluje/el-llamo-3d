using System;
using System.Collections;
using TMPro;
using UnityEngine;

namespace Llamo.Level
{
    public abstract class ILevelStateManager : MonoBehaviour
    {
        [SerializeField] protected TextMeshProUGUI countdownText;
        [SerializeField] protected int countdown = 5;

        [Header("Round Countdown")]
        [SerializeField] protected TextMeshProUGUI roundCountdownText;
        [SerializeField] protected int roundTime = 45;

        public GameState State { get; protected set; }

        public Action<GameState> OnLevelStateChange = delegate { };

        protected int currentCountdown;

        protected bool isGamePaused = true;

        protected void Countdown()
        {
            if (State == GameState.starting)
            {
                if (currentCountdown == 0)
                {
                    // countdown is 0, start the game
                    countdownText.text = "Go!";
                    State = GameState.started;
                    Debug.Log("Started");
                    OnLevelStateChange(State);
                }
                else
                {
                    countdownText.text = currentCountdown.ToString();
                    currentCountdown = currentCountdown - 1;
                }
            }
            else
            {
                // clear main text and stop timer
                countdownText.text = "";
                CancelInvoke("Countdown");
            }
        }

        protected void PauseGame()
        {
            //Time.timeScale = 0;
            Debug.Log("Game Paused");
            isGamePaused = true;
        }

        protected void ResumeGame()
        {
            //Time.timeScale = 1;
            Debug.Log("Game Resumed");
            isGamePaused = false;
        }

        protected IEnumerator StartCountdown(int countdownValue = 10)
        {
            int roundCountdown = countdownValue;
            while (roundCountdown > 0)
            {
                roundCountdownText.SetText(roundCountdown.ToString());
                yield return new WaitForSeconds(1.0f);
                roundCountdown--;
            }

            State = GameState.finished;
            roundCountdownText.SetText("0");
            countdownText.SetText("Round ended! Press Enter");
        }
    }

}