using TMPro;
using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public class PlayerAvatar
{
    [SerializeField] private TextMeshProUGUI nicknameText;
    [SerializeField] private Image avatarImage;
    [SerializeField] private Sprite defaultAvatar;

    public void SetupPlayer(string playerName, Color color)
    {
        nicknameText.text = playerName;
        avatarImage.sprite = defaultAvatar;
        avatarImage.color = color;

        Show();
    }

    public void Hide()
    {
        nicknameText.transform.parent.gameObject.SetActive(false);
    }

    public void Show()
    {
        nicknameText.transform.parent.gameObject.SetActive(true);
    }
}
