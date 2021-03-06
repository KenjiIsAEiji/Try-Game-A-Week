﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Raizer : MonoBehaviour
{
    Vector3 velocity;
    Vector3 position;
    public Transform target;
    [SerializeField] float period;
    [SerializeField] GameObject hitFx;

    public float DamegePoint;
    
    // Start is called before the first frame update
    void Start()
    {
        period += Random.Range(-(period * .5f), 0f);
        position = transform.position;
        velocity = transform.up * 10f;
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
            target.transform.GetComponent<EnemyController>().Damage(DamegePoint);

            GameObject obj = Instantiate(hitFx, transform.position, transform.rotation);
            Destroy(obj, 1f);
            return;
        }

        velocity += accleration * Time.deltaTime;
        position += velocity * Time.deltaTime;
        transform.position = position;
    }
}
