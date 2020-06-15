using SWNetwork;
using UnityEngine;

public class HideIfNotMine : MonoBehaviour
{
    [SerializeField] private GameObject[] gameObjects;
    [SerializeField] private NetworkID networkID;

    // Start is called before the first frame update
    private void Start()
    {
        if (!networkID.IsMine)
        {
            foreach (GameObject go in gameObjects)
            {
                go.SetActive(false);
            }
        }
    }
}
