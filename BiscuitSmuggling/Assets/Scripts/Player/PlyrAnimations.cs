using KinematicCharacterController;
using KinematicCharacterController.Examples;
using UnityEngine;

public class PlyrAnimations : MonoBehaviour
{
    private ExampleCharacterController characterController;
    private KinematicCharacterMotor kinamaticMotor;

    private Animator animator;
    private Rigidbody rb;
    private float animSmooth = 12;
    private float velocityZ = 4;
    private void Start()
    {
        animator = GetComponentInChildren<Animator>();
        characterController = GetComponent<ExampleCharacterController>();
        kinamaticMotor = GetComponent<KinematicCharacterMotor>();
        kinamaticMotor.BaseVelocity = new Vector3(4, 0, 0);
        rb = GetComponent<Rigidbody>();

    }


    private void Update()
    {
        if (kinamaticMotor.BaseVelocity.magnitude <= 4 && kinamaticMotor.BaseVelocity.magnitude > 0.2f)
        {
            velocityZ = 4;
        }
        else if (Input.GetKey(KeyCode.LeftShift) && kinamaticMotor.BaseVelocity.magnitude >= 0.2)
        {
            velocityZ = 8;
        }

        else if (Input.GetKeyUp(KeyCode.LeftShift))
        {
            velocityZ = 4;
        }

        if (kinamaticMotor.BaseVelocity.magnitude < 0.2f && velocityZ >= 0)
        {
            velocityZ -= Time.deltaTime * animSmooth;
        }
        
        GetVelotity();
    }

    private void GetVelotity()
    {
        animator.SetFloat("VelocityZ", velocityZ);
    }
}
