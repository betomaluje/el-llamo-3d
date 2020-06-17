using UnityEngine;

public class EnemyPortal : MonoBehaviour
{
    [SerializeField] private float timeToTouchPlayer = 1f;

    private Collider collider;

    private bool alreadyShaking = false;

    // Start is called before the first frame update
    void Start()
    {
        collider = GetComponent<Collider>();

        SoundManager.instance.Play("Portal");
        Invoke("RemoveCollider", timeToTouchPlayer);
    }

    private void ShakeCamera()
    {
        CameraShake cameraShake = Camera.main.GetComponent<CameraShake>();
        cameraShake.actionShakeCamera();
    }

    private void RemoveCollider()
    {
        Destroy(collider);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!alreadyShaking && other.gameObject.CompareTag("Player"))
        {
            alreadyShaking = true;
            ShakeCamera();
        }
    }
}
