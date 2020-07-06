using UnityEngine;
using SWNetwork;

public class LevelReady : MonoBehaviour
{
    RoomPropertyAgent roomPropertyAgent;
    const string PLAYER_PRESSED_ENTER = "PlayersPressedEnter";

    [System.Serializable]
    public enum GameState { waiting, starting, started, finished };
    public GameState State { get => _state; private set => _state = value; }
    private GameState _state;

    [SerializeField] private int countdown = 5;
    private int countdown_;

    private void Start()
    {
        roomPropertyAgent = GetComponent<RoomPropertyAgent>();

        Debug.Log("Players needed to start this scene: " + GameSettings.instance.numberOfPlayers);
    }

    private void Update()
    {
        if (State == GameState.waiting)
        {
            if (Input.GetKeyUp(KeyCode.Return))
            {
                // start the countdown
                Debug.Log("Starting...");
                State = GameState.starting;

                // Modify the PlayersPressedEnter sync property.
                int playerPressedEnter = roomPropertyAgent.GetPropertyWithName(PLAYER_PRESSED_ENTER).GetIntValue();
                roomPropertyAgent.Modify(PLAYER_PRESSED_ENTER, playerPressedEnter + 1);
            }
        }
        else if (State == GameState.finished)
        {
            if (Input.GetKeyUp(KeyCode.Return))
            {
                Debug.Log("Quiting...");
                FindObjectOfType<SoundMenu>().OnExitClicked();
            }
        }
    }

    private void Countdown()
    {
        if (State == GameState.starting)
        {
            Debug.Log("Countdown: " + countdown_);
            if (countdown_ == 0)
            {
                // countdown is 0, start the game
                //guiManager.SetMainText("Go");
                State = GameState.started;
                Debug.Log("Started");
            }
            else
            {
                //guiManager.SetMainText(countdown_.ToString());
                countdown_ = countdown_ - 1;
            }
        }
        else
        {
            // clear main text and stop timer
            //guiManager.SetMainText("");
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
            countdown_ = countdown;
        }
    }

    public void OnPlayersPressedEnterValueReady()
    {
        int playerPressedEnter = roomPropertyAgent.GetPropertyWithName(PLAYER_PRESSED_ENTER).GetIntValue();

        // check if all players has pressed Enter
        if (playerPressedEnter == 2)
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
