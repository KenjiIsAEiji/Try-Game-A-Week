using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class PlayerController : MonoBehaviour
{
    public Vector2 Velocity { get; set; }
    public Vector2 LookPosition { get; set; }

    Rigidbody PlayerRb;

    [SerializeField] float PlayerSpeed = 10f;

    [SerializeField] Transform charaRay;
    [SerializeField] float charaRayRange = 0.2f;
    private bool isGrounded;
    private float defaultDrag;

    [SerializeField] Camera MainCam;
    Plane plane = new Plane();
    private float distance = 0;


    // Start is called before the first frame update
    void Start()
    {
        PlayerRb = GetComponent<Rigidbody>();
        defaultDrag = PlayerRb.drag;
    }

    private void Update()
    {
        if (Physics.Linecast(charaRay.position, (charaRay.position - transform.up * charaRayRange)))
        {
            isGrounded = true;
        }
        else
        {
            isGrounded = false;
        }
        Debug.DrawLine(charaRay.position, (charaRay.position - transform.up * charaRayRange));

        Ray ray = MainCam.ScreenPointToRay(LookPosition);

        plane.SetNormalAndPosition(Vector3.up, transform.localPosition);
        if(plane.Raycast(ray,out distance))
        {
            Vector3 lookPoint = ray.GetPoint(distance);
            transform.LookAt(lookPoint);
        }
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        Vector3 force = new Vector3(Velocity.x, 0, Velocity.y) * PlayerSpeed;
        if (isGrounded)
        {
            PlayerRb.drag = defaultDrag;
            PlayerRb.AddForce(force * PlayerRb.mass * PlayerRb.drag / (1f - PlayerRb.drag * Time.fixedDeltaTime));
        }
        else
        {
            PlayerRb.drag = 0f;
        }
    }
}
