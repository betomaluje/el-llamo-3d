using SWNetwork;
using UnityEngine;

namespace Llamo.Level
{
    public class LevelStateManager : ILevelStateManager
    {
        #region first countdown
        private RoomPropertyAgent roomPropertyAgent;
        private const string PLAYER_PRESSED_ENTER = "PlayersPressedEnter";
        #endregion

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
                        countdownText.text = "Waiting for players";
                        State = GameState.starting;

                        // Modify the PlayersPressedEnter sync property.
                        int playerPressedEnter = roomPropertyAgent.GetPropertyWithName(PLAYER_PRESSED_ENTER).GetIntValue();
                        playerPressedEnter += 1;
                        roomPropertyAgent.Modify(PLAYER_PRESSED_ENTER, playerPressedEnter);

                        Debug.Log("Starting... " + playerPressedEnter);
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

        // GameDataManager events
        public void OnPlayersPressedEnterValueChanged()
        {
            int playerPressedEnter = roomPropertyAgent.GetPropertyWithName(PLAYER_PRESSED_ENTER).GetIntValue();

            Debug.Log("enter changed... " + playerPressedEnter);

            // check if all players has pressed Enter
            if (playerPressedEnter >= GameSettings.instance.numberOfPlayers)
            {
                // start the countdown
                InvokeRepeating("Countdown", 0.0f, 1.0f);
                currentCountdown = countdown;
            }
        }

        public void OnPlayersPressedEnterValueReady()
        {
            int playerPressedEnter = roomPropertyAgent.GetPropertyWithName(PLAYER_PRESSED_ENTER).GetIntValue();

            Debug.Log("enter ready... " + playerPressedEnter);

            // check if all players has pressed Enter
            if (playerPressedEnter >= GameSettings.instance.numberOfPlayers)
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
    }
}