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

    [SerializeField] GameObject DeadFX;

    Transform playerTransform;
    
    // Start is called before the first frame update
    void Start()
    {
        playerTransform = GameObject.FindGameObjectWithTag("Player").transform;
        EnemyHelth = MaxHelth;
    }

    // Update is called once per frame
    void Update()
    {
        agent.SetDestination(playerTransform.position);
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
