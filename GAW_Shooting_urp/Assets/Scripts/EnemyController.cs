using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class EnemyController : MonoBehaviour
{
    [SerializeField] NavMeshAgent agent;
    [SerializeField] GameObject TargetMark;

    [SerializeField] float MaxHelth = 5;
    public float EnemyHelth;

    [Header("コントロールパラメータ")]
    [SerializeField] float MaxSpeed = 15f;
    [SerializeField] GameObject EnemyBullet;
    [SerializeField] float AimDistance = 10f;
    [SerializeField] float fireRaito = 0.4f;
    [SerializeField] float bulletSpeed = 10f;
    [SerializeField] Transform firePoint;

    private bool attacking = false;

    [SerializeField] GameObject DeadFX;

    Transform playerTransform;
    
    // Start is called before the first frame update
    void Start()
    {
        playerTransform = GameObject.FindGameObjectWithTag("Player").transform;
        EnemyHelth = MaxHelth;

        agent.speed = MaxSpeed;
    }

    // Update is called once per frame
    void Update()
    {
        agent.SetDestination(playerTransform.position);

        Vector3 distance = transform.position - playerTransform.position;
        if(distance.magnitude <= AimDistance)
        {
            NavMeshHit hit;
            if(!agent.Raycast(playerTransform.position,out hit))
            {
                agent.speed = MaxSpeed / 2;
                agent.updateRotation = false;
                transform.LookAt(playerTransform.position);
                if (!attacking)
                {
                    attacking = true;
                    StartCoroutine(AttackTimer());
                }
            }
            else
            {
                agent.speed = MaxSpeed;
                agent.updateRotation = true;
                attacking = false;
                StopCoroutine(AttackTimer());
            }
        }
        else
        {
            agent.speed = MaxSpeed;
            agent.updateRotation = true;
            attacking = false;
            StopCoroutine(AttackTimer());
        }
    }

    IEnumerator AttackTimer()
    {
        while (true)
        {
            BulletFire();
            yield return new WaitForSeconds(fireRaito);
            if (!attacking) yield break;
        }
    }
    
    void BulletFire()
    {
        GameObject bullet = Instantiate(EnemyBullet, firePoint.position, firePoint.rotation);
        bullet.GetComponent<Rigidbody>().AddForce(firePoint.forward * bulletSpeed, ForceMode.Impulse);
        Destroy(bullet, 1f);
    }

    public void TargetFromPlayer()
    {
        TargetMark.SetActive(true);
    }

    public void Damage(float damagePoint)
    {
        EnemyHelth -= damagePoint;
        if (EnemyHelth <= 0)
        {
            GameObject obj = Instantiate(DeadFX, transform.position, Quaternion.LookRotation(Vector3.up));
            Destroy(this.gameObject);
            Destroy(obj, 3f);
        }
    }
}
