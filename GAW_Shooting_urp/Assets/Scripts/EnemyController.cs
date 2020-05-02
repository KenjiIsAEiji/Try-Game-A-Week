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
    [SerializeField] EnemyData Data;
    //[SerializeField] GameObject EnemyBullet;
    [SerializeField] GameObject EnergyCellModel;
    [SerializeField] Transform firePoint;

    private bool attacking = false;

    [SerializeField] GameObject DeadFX;

    Transform playerTransform;
    
    // Start is called before the first frame update
    void Start()
    {
        playerTransform = GameObject.FindGameObjectWithTag("Player").transform;
        EnemyHelth = MaxHelth;

        agent.speed = Data.EnemyMaxSpeed;
    }

    // Update is called once per frame
    void Update()
    {
        agent.SetDestination(playerTransform.position);

        Vector3 distance = transform.position - playerTransform.position;
        if(distance.magnitude <= Data.AimDistance)
        {
            NavMeshHit hit;
            if(!agent.Raycast(playerTransform.position,out hit))
            {
                agent.speed = Data.EnemyMaxSpeed / 2;
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
                agent.speed = Data.EnemyMaxSpeed;
                agent.updateRotation = true;
                attacking = false;
                StopCoroutine(AttackTimer());
            }
        }
        else
        {
            agent.speed = Data.EnemyMaxSpeed;
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
            yield return new WaitForSeconds(Data.FireRaito);
            if (!attacking) yield break;
        }
    }
    
    void BulletFire()
    {
        GameObject bullet = Instantiate(Data.BulletModel, firePoint.position, firePoint.rotation);
        bullet.GetComponent<Rigidbody>().AddForce(firePoint.forward * Data.BulletSpeed, ForceMode.Impulse);
        Destroy(bullet, Data.BulletLifeTime);
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

            GameManager.Instance.Enemys.Remove(this.gameObject);
            GameManager.Instance.EnemyDestroyed(Data.MiddleEnemyType);

            Destroy(this.gameObject);
            Destroy(obj, 3f);

            float daice = Random.Range(0, 100);
            if(daice <= Data.EnergyCellPercent)
            {
                Instantiate(EnergyCellModel, transform.position, transform.rotation);
            }

        }
    }
}
