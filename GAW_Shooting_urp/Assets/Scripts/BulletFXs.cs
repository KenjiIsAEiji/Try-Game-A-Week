using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletFXs : MonoBehaviour
{
    [SerializeField] GameObject nomalHitPaticle;
    [SerializeField] GameObject EnemyHitPaticle;

    private void OnCollisionEnter(Collision collision)
    {
        ContactPoint contact = collision.contacts[0];
        NomalHitFX(contact.point, Quaternion.LookRotation(contact.normal));
        Destroy(this.gameObject);
    }

    void NomalHitFX(Vector3 point,Quaternion rotation)
    {
        Instantiate(nomalHitPaticle, point,rotation);
    }

}
