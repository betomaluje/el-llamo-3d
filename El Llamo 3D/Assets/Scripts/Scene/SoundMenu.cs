using SWNetwork;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SoundMenu : MonoBehaviour
{
    [Header("General Settings")]
    [SerializeField] private KeyCode showMenu = KeyCode.P;
    [SerializeField] private GameObject settingsPanel;
    [SerializeField] private Button settingsButton;
    [SerializeField] private Sprite settingsSprite;
    [SerializeField] private Sprite closeSprite;
    [SerializeField] private TextMeshProUGUI exitButton;

    [Space]
    [Header("SFX")]
    [SerializeField] private Slider sfxSlider;
    [SerializeField] private TextMeshProUGUI sfxValue;

    [Space]
    [Header("Songs")]
    [SerializeField] private Slider soundsSlider;
    [SerializeField] private TextMeshProUGUI songsValue;

    [HideInInspector]
    public const string PREFS_SFX = "sound_sfx";
    [HideInInspector]
    public const string PREFS_SONG = "sound_song";

    private bool isPanelActive;
    private int currentScene = 0;

    private void Awake()
    {
        currentScene = SceneManager.GetActiveScene().buildIndex;

        if (currentScene == 0)
        {
            exitButton.text = "Back";
        }
        else
        {
            exitButton.text = "Go To Lobby";
        }
    }

    void Start()
    {
        isPanelActive = settingsPanel.active;

        sfxSlider.minValue = 0;
        sfxSlider.maxValue = 1f;

        soundsSlider.minValue = 0;
        soundsSlider.maxValue = 1f;
    }

    private void Update()
    {
        if (Input.GetKeyDown(showMenu))
        {
            ToggleSettingsPanel();
        }
    }

    public void RestoreValues()
    {
        float sfxSavedPrefs = PlayerPrefs.GetFloat(PREFS_SFX, SoundManager.instance.GetVolumeForType(SoundType.SFX));
        float songSavedPrefs = PlayerPrefs.GetFloat(PREFS_SONG, SoundManager.instance.GetVolumeForType(SoundType.SONG));

        sfxSlider.value = sfxSavedPrefs;
        soundsSlider.value = songSavedPrefs;

        sfxValue.text = ConvertValueToString(sfxSavedPrefs);
        songsValue.text = ConvertValueToString(songSavedPrefs);
    }

    private string ConvertValueToString(float value)
    {
        return (Mathf.Round(value * 10f)).ToString();
    }

    public void SaveSettings()
    {
        PlayerPrefs.SetFloat(PREFS_SFX, sfxSlider.value);
        PlayerPrefs.SetFloat(PREFS_SONG, soundsSlider.value);
    }

    public void SfxValueChanged(float value)
    {
        SoundManager.instance.SetVolumeForType(SoundType.SFX, value);
        sfxValue.text = ConvertValueToString(value);
    }

    public void SoundsValueChanged(float value)
    {
        SoundManager.instance.SetVolumeForType(SoundType.SONG, value);
        songsValue.text = ConvertValueToString(value);
    }

    public void PlayTestSFX()
    {
        SoundManager.instance.Play("Jump");
    }

    public void ToggleSettingsPanel()
    {
        isPanelActive = !isPanelActive;
        settingsPanel.SetActive(isPanelActive);

        if (isPanelActive)
        {
            RestoreValues();
            if (settingsButton != null)
            {
                settingsButton.image.sprite = closeSprite;
            }
        }
        else
        {
            SaveSettings();
            if (settingsButton != null)
            {
                settingsButton.image.sprite = settingsSprite;
            }
        }

        if (currentScene != 0)
        {
            if (isPanelActive)
            {
                Cursor.visible = true;
                Cursor.lockState = CursorLockMode.None;
            }
            else
            {
                Cursor.visible = false;
                Cursor.lockState = CursorLockMode.Locked;
            }
        }
    }

    public void OnExitClicked()
    {
        if (currentScene == 0)
        {
            // in the lobby, quit app
            ExitGame();
        }
        else
        {
            // not in lobby, go to lobby
            GoToLobby();
        }
    }

    private void GoToLobby()
    {
        // disconnect from current room and then go to lobby
        Debug.Log("End of round!");

        if (GameSettings.instance.usingNetwork)
        {
            NetworkClient.Lobby.LeaveRoom((successful, error) =>
            {
                if (successful)
                {
                    Debug.Log("Left room");
                    LevelLoader levelLoader = GetComponent<LevelLoader>();
                    levelLoader.LoadLevel(Level.LevelNumber.Lobby);
                }
                else
                {
                    Debug.Log("Failed to leave room " + error);
                }
            });
        }
        else
        {
            Debug.Log("Left room");
            LevelLoader levelLoader = GetComponent<LevelLoader>();
            levelLoader.LoadLevel(Level.LevelNumber.Lobby);
        }
    }

    private void ExitGame()
    {
        Application.Quit();
    }
}
