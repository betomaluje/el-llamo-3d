using UnityEngine;

public class PlayerAnimations : MonoBehaviour
{
    public Animator anim;

    private CharacterController controller;
    public Transform playerPosFix;
    
    void Start()
    {
        controller = GetComponentInParent<CharacterController>();
    }

    private void Update()
    {
        playerPosFix.localPosition = Vector3.zero;
    }

    void FixedUpdate()
    {        
        anim.SetBool("Jump_b", !controller.isGrounded);
        anim.SetBool("Grounded", controller.isGrounded);        
    }

    public void WalkRunAnim(float speed)
    {
        anim.SetFloat("Speed_f", speed);
    }

    public void ShootAnim(bool shooting)
    {
        anim.SetInteger("WeaponType_int", shooting ? 1 : 0);
        anim.SetBool("Shoot_b", shooting);
    }

    public void DieAnim()
    {
        anim.SetBool("Death_b", true);
    }

}
