using UnityEngine;

namespace Llamo.Level
{
    public class LocalLevelStateManager : ILevelStateManager
    {
        private void Update()
        {
            switch (State)
            {
                case GameState.waiting:
                    if (Input.GetKeyUp(KeyCode.Return))
                    {
                        // start the countdown
                        Debug.Log("Starting...");
                        countdownText.text = "Waiting for players";
                        State = GameState.starting;

                        // start the countdown
                        InvokeRepeating("Countdown", 0.0f, 1.0f);
                        currentCountdown = countdown;
                    }
                    break;
                case GameState.started:
                    if (isGamePaused)
                    {
                        ResumeGame();
                        StartCoroutine(StartCountdown(roundTime));
                    }
                    break;
                case GameState.finished:
                    if (Input.GetKeyUp(KeyCode.Return))
                    {
                        Debug.Log("Quiting...");
                        FindObjectOfType<SoundMenu>().OnExitClicked();
                    }
                    break;
            }
        }
    }
}
