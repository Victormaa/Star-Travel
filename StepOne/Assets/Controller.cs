using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Controller : MonoBehaviour
{
    public float mouseSensitivityX = 250f;
    public float mouseSensitivityY = 250f;
    public float inputDelay = 0.1f;

    private Transform CameraR;
    private float VerticalLookR;
    private float mouseXInput;
    private float mouseYInput;

    public float moveSpeed = 5.0f;
    private Vector3 moveDir;
    private Vector3 targetAmount;
    private Vector3 moveAmount;
    private float forwardInput;
    private float turnInput;
    private Vector3 smoothMoveVel;
    public float jumpForce = 220;

    private bool isGrounded;
    public LayerMask groundMask;

    private new Rigidbody rigidbody;

    // Start is called before the first frame update
    void Start()
    {
        rigidbody = this.GetComponent<Rigidbody>();
        CameraR = Camera.main.transform;
    }

    // Update is called once per frame
    void Update()
    {
        GetInput();
        LookControl();
        //moveDir = new Vector3(Input.GetAxisRaw("Horizontal"),0, Input.GetAxisRaw("Vertical")).normalized;
    }

    private void FixedUpdate()
    {    
       Move();   
       // rigidbody.MovePosition(rigidbody.position + transform.TransformDirection(moveDir * movespeed * Time.deltaTime));
    }

    private void LookControl()
    {  
        VerticalLookR = Mathf.Clamp(VerticalLookR, -60, 60);
        CameraR.localEulerAngles = Vector3.left * VerticalLookR;
    }

    private void GetInput()
    {
        mouseXInput = Input.GetAxis("Mouse X");
        mouseYInput = Input.GetAxis("Mouse Y");
        forwardInput = Input.GetAxisRaw("Vertical");
        turnInput = Input.GetAxisRaw("Horizontal");

        transform.Rotate(Vector3.up * mouseXInput * Time.deltaTime * mouseSensitivityX);
        VerticalLookR += mouseYInput * Time.deltaTime * mouseSensitivityY;

        moveDir = new Vector3(turnInput, 0, forwardInput).normalized;
        targetAmount = moveDir * moveSpeed;
        moveAmount = Vector3.SmoothDamp(moveAmount, targetAmount, ref smoothMoveVel, 0.15f);

        if (Input.GetButtonDown("Jump"))
        {
            Jump();
        }
        isGrounded = false;
        Ray ray = new Ray(transform.position, -transform.up);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit,0.1f,groundMask))
        {
                isGrounded = true;
        }
        // keyboard input;
    }

    private void Jump()
    {
        if (isGrounded)
        {
            rigidbody.AddForce(transform.up * jumpForce);
        }
    }
    private void Move()
    {
        rigidbody.MovePosition(rigidbody.position + transform.TransformDirection(moveAmount) * Time.fixedDeltaTime);
    }
}
