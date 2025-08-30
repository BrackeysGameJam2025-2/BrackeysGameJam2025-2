using System;
using KinematicCharacterController;
using KinematicCharacterController.Examples;
using UnityEngine;

public class PlyrAnimations : MonoBehaviour
{
    ExampleCharacterController characterController;
    KinematicCharacterMotor kinamaticMotor;
    
    Animator animator;
    Rigidbody rb;
    float animSmooth = 12;
    float velocityZ = 4;
    void Start()
    {
        animator = GetComponentInChildren<Animator>();
        characterController = GetComponent<ExampleCharacterController>();
        kinamaticMotor = GetComponent<KinematicCharacterMotor>();
        kinamaticMotor.BaseVelocity = new Vector3(4, 0, 0);
        rb = GetComponent<Rigidbody>();

    }


    void Update()
    {
        Debug.Log(kinamaticMotor.BaseVelocity);
        if (kinamaticMotor.BaseVelocity.magnitude <= 4 && kinamaticMotor.BaseVelocity.magnitude > 0.2f)
        {
            velocityZ = 4;
        }
        else if (Input.GetKey(KeyCode.LeftShift))
        {
            velocityZ = 8;
        }

        else if (Input.GetKeyUp(KeyCode.LeftShift))
        {
            velocityZ = 4;
        }

        if (kinamaticMotor.BaseVelocity.magnitude < 0.2f && velocityZ >=0)
        {
            velocityZ -= Time.deltaTime * animSmooth;
        }
        GetVelotity();
    }

    void GetVelotity()
    {
        animator.SetFloat("VelocityZ", velocityZ);
    }
}
