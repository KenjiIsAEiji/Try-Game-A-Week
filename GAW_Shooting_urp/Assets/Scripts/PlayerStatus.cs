using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerStatus : MonoBehaviour
{
    public float PlayerMaxLp = 500f;
    public float PlayerLp;

    [SerializeField] float BaseBulletDamage = 1f;
    [SerializeField] float BaseRaizerDamege = 5f;

    [SerializeField] float BulletUseLp = 1f;
    [SerializeField] float RaizerUseLp = 40f;
    [SerializeField] float WarningPointRaito = 0.2f;
    [SerializeField] ParticleSystem HealFx;
    [SerializeField] GameObject deadFx;
    [SerializeField] AudioSource LpGetAudio;

    public bool IsDead = false;
    public bool LpWarning = false;

    // Start is called before the first frame update
    void Start()
    {
        PlayerLp = PlayerMaxLp;
    }

    // Update is called once per frame
    void Update()
    {
        if (PlayerLp <= 0)
        {
            PlayerLp = 0;
            IsDead = true;
        }
        else if ((PlayerLp / PlayerMaxLp) <= WarningPointRaito)
        {
            LpWarning = true;
        }
        else
        {
            LpWarning = false;
        }
    }

    public void Damage(float damageLp)
    {
        PlayerLp -= damageLp;

        //addDeadFX
        if (PlayerLp <= 0) Instantiate(deadFx, transform.position, transform.rotation);
    }

    public void UseBullet()
    {
        if (!LpWarning)
        {
            PlayerLp -= BulletUseLp;
        }
    }

    public void UseRaizer()
    {
        if (!LpWarning)
        {
            PlayerLp -= RaizerUseLp;
        }
    }

    public float GetBulletDamege()
    {
        return LpWarning ? (BaseBulletDamage / 2) : BaseBulletDamage;
    }
    
    public float GetRaizerDamege()
    {
        return LpWarning ? (BaseRaizerDamege / 2) : BaseRaizerDamege;
    }

    public void AddLp(float point)
    {
        if(PlayerLp + point >= PlayerMaxLp)
        {
            PlayerLp = PlayerMaxLp;
        }
        else
        {
            PlayerLp += point;
        }
        HealFx.Play();
        LpGetAudio.Play();
    }
}
