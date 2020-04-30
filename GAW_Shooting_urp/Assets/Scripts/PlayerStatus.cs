using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerStatus : MonoBehaviour
{
    public float PlayerMaxLp = 500f;
    public float PlayerLp;

    [SerializeField] float BulletUseLp = 1f;
    [SerializeField] float RaizerUseLp = 40f;

    // Start is called before the first frame update
    void Start()
    {
        PlayerLp = PlayerMaxLp;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Damage(float damageLp)
    {
        PlayerLp -= damageLp;
    }

    public void UseBullet()
    {
        PlayerLp -= BulletUseLp;
    }

    public void UseRaizer()
    {
        PlayerLp -= RaizerUseLp;
    }
}
