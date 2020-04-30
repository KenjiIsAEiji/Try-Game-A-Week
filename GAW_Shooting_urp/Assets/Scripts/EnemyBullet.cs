using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyBullet : MonoBehaviour
{
    [SerializeField] GameObject nomalHitFx;
    [SerializeField] GameObject playerHitFx;

    [SerializeField] float PlayerDamagePoint = 10f;

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnCollisionEnter(Collision collision)
    {
        GameObject obj = Instantiate(nomalHitFx,transform.position,transform.rotation);
        Destroy(obj, 3f);
        if (collision.gameObject.CompareTag("Player"))
        {
            collision.gameObject.GetComponent<PlayerStatus>().Damage(PlayerDamagePoint);
            GameObject fx = Instantiate(playerHitFx, transform.position, transform.rotation);
            Destroy(fx, 3f);
        }

        Destroy(this.gameObject);
    }
}
