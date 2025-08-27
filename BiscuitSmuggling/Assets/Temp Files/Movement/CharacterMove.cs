using UnityEngine;
using UnityEngine.Assertions.Must;
using UnityEngine.Rendering;
using UnityEngine.UIElements;

public class CharacterMove : MonoBehaviour
{
    [SerializeField] float moveSpeed;
    Rigidbody rb;
    Vector3 moveInput;
    
   

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
       
    }
    void Start()
    {


    }


    void Update()
    {
        moveInput = Vector2.zero;

        if (Input.GetKey(KeyCode.W))
        {
            moveInput.z = 1;
        }

        if (Input.GetKey(KeyCode.S))
        {
            moveInput.z = -1;
        }

        if (Input.GetKey(KeyCode.A))
        {
            moveInput.x = -1;
        }

        if (Input.GetKey(KeyCode.D))
        {
            moveInput.x = 1;
        }

        moveInput = moveInput.normalized;

        float moveX = moveInput.x * moveSpeed;

        float moveZ = moveInput.z * moveSpeed;

        rb.linearVelocity = new Vector3(moveX, 0, moveZ);
    }
    
        
    
}
