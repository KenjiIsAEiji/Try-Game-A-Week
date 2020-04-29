using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Raizer : MonoBehaviour
{
    Vector3 velocity;
    Vector3 position;
    public Transform target;
    [SerializeField] float period;

    
    // Start is called before the first frame update
    void Start()
    {
        period += Random.Range(-(period - 1f), 0f);
        position = transform.position;
        velocity = -transform.forward * 10f;
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 accleration = Vector3.zero;

        Vector3 diff = target.position - position;

        accleration += (diff - velocity * period) * 2f / (period * period);

        period -= Time.deltaTime;

        if(period < 0f)
        {
            Destroy(this.gameObject);
            target.transform.GetComponent<EnemyController>().Damage(5);
            return;
        }

        velocity += accleration * Time.deltaTime;
        position += velocity * Time.deltaTime;
        transform.position = position;
    }
}
