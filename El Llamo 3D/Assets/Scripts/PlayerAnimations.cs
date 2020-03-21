using UnityEngine;

public class PlayerAnimations : MonoBehaviour
{
    
    [SerializeField] private PlayerAnimationTrigger playerAnimationsTrigger;

    public Animator anim;
    public Transform playerPosFix;

    private CharacterController controller;

    void Start()
    {
        controller = GetComponentInParent<CharacterController>();

        playerAnimationsTrigger.throwDoneCallback = () =>
        {
            ResetThrow();
        };
        
    }

    private void Update()
    {
        playerPosFix.localPosition = Vector3.zero;
    }

    void FixedUpdate()
    {
        anim.SetBool("onGround", controller.isGrounded);
    }

    public void WalkRunAnim(float speed)
    {
        anim.SetFloat("Speed", speed);
    }

    public void ShootAnim(bool shooting)
    {
        anim.SetBool("isShooting", shooting);
    }

    public void Throw() 
    {
        Debug.Log("animated throwing");
        anim.SetBool("Throw", true);
    }

    public void ResetThrow() 
    {
        Debug.Log("animated reset throwing");
        anim.SetBool("Throw", false);
    }

    public void DieAnim()
    {
        anim.SetBool("Dead", true);
    }

    public void Revive()
    {
        anim.SetBool("Dead", false);
    }

}
