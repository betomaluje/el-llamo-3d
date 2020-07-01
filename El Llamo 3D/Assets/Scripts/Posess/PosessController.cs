using UnityEngine;

public class PosessController : MonoBehaviour
{
    [SerializeField] private GameObject[] toToggle;

    private CharacterController controller;
    private LocalMouseLook localMouseLook;
    private LocalPlayerMovement localPlayerMovement;
    private LocalInputHandler localInputHandler;
    private PosessHandler posessHandler;

    private BoxCollider col;

    private void Start()
    {
        localMouseLook = GetComponent<LocalMouseLook>();
        localPlayerMovement = GetComponent<LocalPlayerMovement>();
        localInputHandler = GetComponent<LocalInputHandler>();
        posessHandler = GetComponent<PosessHandler>();

        col = GetComponent<BoxCollider>();
        controller = GetComponent<CharacterController>();

        DisableComponents();
    }

    public void EnableComponents()
    {
        ToggleGameObjects(true);

        localMouseLook.enabled = true;
        localPlayerMovement.enabled = true;
        controller.enabled = true;
        localInputHandler.enabled = true;
        posessHandler.enabled = true;

        col.enabled = false;

        Debug.Log(gameObject.name + " posessed!");
    }

    public void DisableComponents()
    {
        ToggleGameObjects(false);

        localMouseLook.enabled = false;
        localPlayerMovement.enabled = false;
        controller.enabled = false;
        localInputHandler.enabled = false;
        posessHandler.enabled = false;

        col.enabled = true;
    }

    private void ToggleGameObjects(bool enabled)
    {
        foreach (var go in toToggle)
        {
            go.SetActive(enabled);
        }
    }
}
