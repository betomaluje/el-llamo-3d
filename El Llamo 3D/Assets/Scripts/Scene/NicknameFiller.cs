using UnityEngine;
using UnityEngine.UI;

public class NicknameFiller : MonoBehaviour
{
    [SerializeField] NicknameGenerator nicknameGenerator;
    [SerializeField] InputField nicknameText;

    private void OnEnable()
    {
        GenerateRandomNickName();
    }

    public void GenerateRandomNickName()
    {
        string nickname = nicknameGenerator.Generate(Random.Range(3, 12));
        nicknameText.text = nickname;
    }

}
