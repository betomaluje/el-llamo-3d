using Llamo.Level;
using SWNetwork;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Basic lobby matchmaking implementation.
/// </summary>
public class Lobby : MonoBehaviour
{
    [Header("Main Network")]
    [SerializeField] private int maxPlayers = 2;
    [SerializeField] private int playerTimeToLive = 60;
    [SerializeField] private LevelLoader levelLoader;

    [Space]
    [Header("Other Settings")]
    /// <summary>
    /// Button for checking into SocketWeaver services
    /// </summary>
    public Button registerButton;

    /// <summary>
    /// Button for joining or creating room
    /// </summary>
    public Button playButton;

    /// <summary>
    /// Button for entering custom playerId
    /// </summary>
    public InputField customPlayerIdField;

    public TextMeshProUGUI errorLogText;

    public Level.LevelNumber selectedLevel = Level.LevelNumber.Lobby;

    private void Start()
    {
        // Add an event handler for the OnRoomReadyEvent
        NetworkClient.Lobby.OnRoomReadyEvent += Lobby_OnRoomReadyEvent;

        // Add an event handler for the OnFailedToStartRoomEvent
        NetworkClient.Lobby.OnFailedToStartRoomEvent += Lobby_OnFailedToStartRoomEvent;

        // Add an event handler for the OnLobbyConnectedEvent
        NetworkClient.Lobby.OnLobbyConnectedEvent += Lobby_OnLobbyConncetedEvent;

        // allow player to register
        registerButton.gameObject.SetActive(true);
        playButton.gameObject.SetActive(false);
    }

    private void onDestroy()
    {
        // remove the handlers
        NetworkClient.Lobby.OnRoomReadyEvent -= Lobby_OnRoomReadyEvent;
        NetworkClient.Lobby.OnFailedToStartRoomEvent -= Lobby_OnFailedToStartRoomEvent;
        NetworkClient.Lobby.OnLobbyConnectedEvent -= Lobby_OnLobbyConncetedEvent;
    }

    /* Lobby events handlers */
    private void Lobby_OnRoomReadyEvent(SWRoomReadyEventData eventData)
    {
        Debug.Log("Room is ready: roomId= " + eventData.roomId);
        // Room is ready to join and its game servers have been assigned.
        ConnectToRoom();
    }

    private void LogError(string error)
    {
        errorLogText.text = error;
    }

    private void Lobby_OnFailedToStartRoomEvent(SWFailedToStartRoomEventData eventData)
    {
        Debug.Log("Failed to start room: " + eventData);
        LogError("Failed to start room: " + eventData.roomId);
    }

    private void Lobby_OnLobbyConncetedEvent()
    {
        Debug.Log("Lobby connected");
        RegisterPlayer();
    }

    /* UI event handlers */
    /// <summary>
    /// Register button was clicked
    /// </summary>
    public void Register()
    {
        string customPlayerId = customPlayerIdField.text;

        if (customPlayerId != null && customPlayerId.Length > 0)
        {
            // use the user entered playerId to check into SocketWeaver. Make sure the PlayerId is unique.
            NetworkClient.Instance.CheckIn(customPlayerId, (bool ok, string error) =>
             {
                 if (!ok)
                 {
                     Debug.LogError("Check-in failed: " + error);
                     LogError("Check-in failed: " + error);
                 }
             });
        }
        else
        {
            // use a randomly generated playerId to check into SocketWeaver.
            NetworkClient.Instance.CheckIn((bool ok, string error) =>
            {
                if (!ok)
                {
                    Debug.LogError("Check-in failed: " + error);
                    LogError("Check-in failed: " + error);
                }
            });
        }
    }

    /// <summary>
    /// Play button was clicked
    /// </summary>
    public void Play()
    {
        // Here we use the JoinOrCreateRoom method to get player into rooms quickly.                
        NetworkClient.Lobby.JoinOrCreateRoom(true, maxPlayers, playerTimeToLive, HandleJoinOrCreatedRoom);
    }

    /* Lobby helper methods*/
    /// <summary>
    /// Register the player to lobby
    /// </summary>
    private void RegisterPlayer()
    {
        NetworkClient.Lobby.Register((successful, reply, error) =>
        {
            if (successful)
            {
                Debug.Log("Registered " + reply);

                if (reply.started)
                {
                    // player is already in a room and the room has started.
                    // We can connect to the room's game servers now.
                    ConnectToRoom();
                }
                else
                {
                    // allow player to join or create room
                    playButton.gameObject.SetActive(true);
                    registerButton.gameObject.SetActive(false);
                }
            }
            else
            {
                Debug.Log("Failed to register " + error);
                LogError("Failed to register ");
            }
        });
    }

    /// <summary>
    /// Callback method for NetworkClient.Lobby.JoinOrCreateRoom().
    /// </summary>
    /// <param name="successful">If set to <c>true</c> <paramref name="successful"/>, the player has joined or created a room.</param>
    /// <param name="reply">Reply.</param>
    /// <param name="error">Error.</param>
    private void HandleJoinOrCreatedRoom(bool successful, SWJoinRoomReply reply, SWLobbyError error)
    {
        if (successful)
        {
            Debug.Log("Joined or created room " + reply);

            // the player has joined a room which has already started.
            if (reply.started)
            {
                ConnectToRoom();
            }
            else if (NetworkClient.Lobby.IsOwner)
            {
                // the player did not find a room to join
                // the player created a new room and became the room owner.
                StartRoom();
            }
        }
        else
        {
            Debug.Log("Failed to join or create room " + error);
            LogError("Failed to join or create room ");
        }
    }

    /// <summary>
    /// Start local player's current room. Lobby server will ask SocketWeaver to assign suitable game servers for the room.
    /// </summary>
    private void StartRoom()
    {
        NetworkClient.Lobby.StartRoom((okay, error) =>
        {
            if (okay)
            {
                // Lobby server has sent request to SocketWeaver. The request is being processed.
                // If socketweaver finds suitable server, Lobby server will invoke the OnRoomReadyEvent.
                // If socketweaver cannot find suitable server, Lobby server will invoke the OnFailedToStartRoomEvent.
                Debug.Log("Started room");
            }
            else
            {
                Debug.Log("Failed to start room " + error);
                LogError("Failed to start room ");
            }
        });
    }

    /// <summary>
    /// Connect to the game servers of the room.
    /// </summary>
    private void ConnectToRoom()
    {
        NetworkClient.Instance.ConnectToRoom(HandleConnectedToRoom);
    }

    /// <summary>
    /// Callback method NetworkClient.Instance.ConnectToRoom();
    /// </summary>
    /// <param name="connected">If set to <c>true</c>, the client has connected to the game servers successfully.</param>
    private void HandleConnectedToRoom(bool connected)
    {
        if (connected)
        {
            Debug.Log("Connected to room");
            OnEverythingReady();
        }
        else
        {
            Debug.Log("Failed to connect to room");
            LogError("Failed to connect to room");
        }
    }

    public void SetLevel(Level level)
    {
        selectedLevel = level.levelNumber;
        GameSettings.instance.SetLevel(level);
    }

    private void OnEverythingReady()
    {
        levelLoader.LoadLevel(selectedLevel);
    }
}