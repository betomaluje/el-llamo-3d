using SWNetwork;
using System;
using System.Collections;
using TMPro;
using UnityEngine;

namespace Llamo.Level
{
    public class LevelStateManager : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI countdownText;
        [SerializeField] private int countdown = 5;

        [Header("Round Countdown")]
        [SerializeField] private TextMeshProUGUI roundCountdownText;
        [SerializeField] private int roundTime = 45;

        #region first countdown
        private RoomPropertyAgent roomPropertyAgent;
        private const string PLAYER_PRESSED_ENTER = "PlayersPressedEnter";

        [Serializable]
        public enum GameState { waiting, starting, started, finished };
        public GameState State { get; private set; }

        public Action<GameState> OnLevelStateChange = delegate { };

        private int currentCountdown;

        private bool isGamePaused = true;
        #endregion

        private int roundCountdown;

        private void Start()
        {
            roomPropertyAgent = GetComponent<RoomPropertyAgent>();

            Debug.Log("Players needed to start this scene: " + GameSettings.instance.numberOfPlayers);
        }

        private void Update()
        {
            switch (State)
            {
                case GameState.waiting:
                    if (Input.GetKeyUp(KeyCode.Return))
                    {
                        // start the countdown
                        Debug.Log("Starting...");
                        State = GameState.starting;

                        // Modify the PlayersPressedEnter sync property.
                        int playerPressedEnter = roomPropertyAgent.GetPropertyWithName(PLAYER_PRESSED_ENTER).GetIntValue();
                        roomPropertyAgent.Modify(PLAYER_PRESSED_ENTER, playerPressedEnter + 1);
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

        private void Countdown()
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

        // GameDataManager events
        public void OnPlayersPressedEnterValueChanged()
        {
            int playerPressedEnter = roomPropertyAgent.GetPropertyWithName(PLAYER_PRESSED_ENTER).GetIntValue();

            // check if all players has pressed Enter
            if (playerPressedEnter == GameSettings.instance.numberOfPlayers)
            {
                // start the countdown
                InvokeRepeating("Countdown", 0.0f, 1.0f);
                currentCountdown = countdown;
            }
        }

        public void OnPlayersPressedEnterValueReady()
        {
            int playerPressedEnter = roomPropertyAgent.GetPropertyWithName(PLAYER_PRESSED_ENTER).GetIntValue();

            // check if all players has pressed Enter
            if (playerPressedEnter == GameSettings.instance.numberOfPlayers)
            {
                // the player probably got disconnected from the room
                // If all players has pressed the Enter key, the game has started already.
                State = GameState.started;
                Debug.Log("Started");
            }
        }

        public void OnPlayersPressedEnterValueConflict(SWSyncConflict conflict, SWSyncedProperty property)
        {
            // If players pressed the Key at the same time, we might get conflict
            // The game server will receive two requests to change the PlayersPressEnter value from 0 to 1
            // The game server will accept the first request and change PlayersPressEnter value to 1
            // The second request will fail and player who sent the second request will get a confict
            int remotePlayerPressed = (int)conflict.remoteValue;

            // Add 1 to the remote PlayerPressedEnter value to resolve the conflict.
            int resolvedPlayerPressed = remotePlayerPressed + 1;

            property.Resolve(resolvedPlayerPressed);
        }

        private void PauseGame()
        {
            //Time.timeScale = 0;
            Debug.Log("Game Paused");
            isGamePaused = true;
        }

        private void ResumeGame()
        {
            //Time.timeScale = 1;
            Debug.Log("Game Resumed");
            isGamePaused = false;
        }

        public IEnumerator StartCountdown(int countdownValue = 10)
        {
            roundCountdown = countdownValue;
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