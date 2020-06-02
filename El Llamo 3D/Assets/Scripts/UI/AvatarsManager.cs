using SWNetwork;
using System.Collections.Generic;
using UnityEngine;

public class AvatarsManager : MonoBehaviour
{
    [SerializeField] private KeyCode toggleKey;
    [SerializeField] private GameObject contentPanel;
    [SerializeField] private PlayerAvatar[] playerAvatars;

    public Dictionary<string, string> playersDict;
    private bool isShown = false;

    private RoomRemoteEventAgent roomRemoteEventAgent;
    private const char SEPARATOR = '-';

    private void Awake()
    {
        playersDict = new Dictionary<string, string>();
        roomRemoteEventAgent = GetComponent<RoomRemoteEventAgent>();

        isShown = contentPanel.active;

        foreach (PlayerAvatar playerAvatar in playerAvatars)
        {
            playerAvatar.Hide();
        }

        GetPlayersInCurrentRoom();
    }

    private void Update()
    {
        if (Input.GetKeyDown(toggleKey))
        {
            isShown = !isShown;
            contentPanel.SetActive(isShown);
        }
    }

    public void OnUpdateName(SWNetworkMessage msg)
    {
        string nicknames = msg.PopUTF8LongString();

        if (nicknames.Length > 0)
        {
            int currentPlayerIndex = 0;

            string[] allNicknames = nicknames.Split(SEPARATOR);

            foreach (string nick in allNicknames)
            {
                if (!string.IsNullOrEmpty(nick))
                {
                    playersDict[nick] = nick;
                    playerAvatars[currentPlayerIndex].SetupPlayer(nick, GenerateColor());
                    currentPlayerIndex++;
                }
            }
        }
    }

    public void GetPlayersInCurrentRoom()
    {
        NetworkClient.Lobby.GetPlayersInRoom((successful, reply, error) =>
        {
            if (successful)
            {
                string allNicknames = "";

                foreach (SWPlayer player in reply.players)
                {
                    string nickname = player.id.Split('-')[1];
                    allNicknames = nickname + SEPARATOR + allNicknames;
                }

                SWNetworkMessage msg = new SWNetworkMessage();
                msg.PushUTF8LongString(allNicknames);
                roomRemoteEventAgent.Invoke(PlayerName.NICKNAME_PROPERTY, msg);
            }
            else
            {
                Debug.Log("Failed to get players " + error);
            }
        });
    }

    private Color GenerateColor()
    {
        return new Color(
             Random.Range(0f, 1f),
             Random.Range(0f, 1f),
             Random.Range(0f, 1f)
         );
    }

}
