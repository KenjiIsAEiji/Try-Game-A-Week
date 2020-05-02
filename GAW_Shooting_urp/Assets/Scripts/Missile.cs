using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Missile : MonoBehaviour
{
    Rigidbody rigidbody;
    Transform target;

    [SerializeField] float ratio;
    [SerializeField] float velosity;
    [SerializeField] float DamagePoint = 50f;
    [SerializeField] GameObject hitFx;

    // Start is called before the first frame update
    void Start()
    {
        rigidbody = GetComponent<Rigidbody>();
        target = GameObject.FindGameObjectWithTag("Player").transform;
    }

    // Update is called once per frame
    private void FixedUpdate()
    {
        Vector3 diff = target.position - transform.position;

        Quaternion target_rot = Quaternion.LookRotation(diff);

        Quaternion q = target_rot * Quaternion.Inverse(transform.rotation);

        Vector3 torque = new Vector3(q.x, q.y, q.z) * ratio;

        rigidbody.AddTorque(torque);

        rigidbody.velocity = transform.forward * velosity;
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            collision.gameObject.GetComponent<PlayerStatus>().Damage(DamagePoint);
        }

        GameObject fx = Instantiate(hitFx, transform.position, transform.rotation);
        Destroy(fx, 1f);

        Destroy(this.gameObject);
    }
}
