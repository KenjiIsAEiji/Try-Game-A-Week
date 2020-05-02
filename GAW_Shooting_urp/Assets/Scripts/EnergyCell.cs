using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnergyCell : MonoBehaviour
{
    [SerializeField] float HealEnergy = 100f;
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            collision.gameObject.GetComponent<PlayerStatus>().AddLp(HealEnergy);

            Destroy(this.gameObject);
        }
    }
}
