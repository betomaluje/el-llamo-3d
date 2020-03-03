using UnityEngine;
using TMPro;
using BetoMaluje.Sikta;

public class AimDebug : MonoBehaviour
{
    public TextMeshProUGUI aimPointText;
    public TextMeshProUGUI networkAimPointText;
    
    private bool isPaused = false;

    public void Setup(PlayerGrab playerGrab)
    {
        playerGrab.aimPointUpdate = (aimPoint) => {
            if (!isPaused) {
                Debug.Log("local: " + aimPoint);
                aimPointText.text = "local: " + aimPoint;
            }            
        };

        playerGrab.networkAimPointUpdate = (aimPoint) => {
            if (!isPaused) {
                Debug.Log("network: " + aimPoint);
                networkAimPointText.text = "network: " + aimPoint;
            }            
        };  
    }

    private void Update() 
    {
        if (Input.GetKeyDown(KeyCode.Space)) {
            isPaused = !isPaused;
        }

        // clear the console
        if (Input.GetKeyDown(KeyCode.L)) {
            aimPointText.text = "";
            networkAimPointText.text = "";
        }
    }
}
