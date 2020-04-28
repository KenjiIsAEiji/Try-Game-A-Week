using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class PlayerController : MonoBehaviour
{
    public Vector2 Velocity { get; set; }
    public Vector2 LookPosition { get; set; }
    public bool FireFlag { get; set; }
    public bool Fire2Flag { get; set; }



    [Header("弾丸発射関連")]
    [SerializeField] GameObject Bullet;
    [SerializeField] Transform firePoint;
    [SerializeField] ParticleSystem fireFX;
    [SerializeField] float fireRaito = 0.1f;
    [SerializeField] float bulletVelocity = 10f;
    [SerializeField] float RecoilRange = 5f;
    private bool fireTrigger = false;

    [Header("追尾レーザ関連")]
    [SerializeField] GameObject Raizer;
    private bool fire2Trigger = false;
    [SerializeField] Transform pointer;
    [SerializeField] LayerMask TargettingLayer;
    [SerializeField] List<Transform> TargetEnemys;
    private bool targetting = false;

    Rigidbody PlayerRb;

    [Header("プレイヤー移動関連")]
    [SerializeField] float PlayerSpeed = 10f;
    [SerializeField] Transform charaRay;
    [SerializeField] float charaRayRange = 0.2f;
    private bool isGrounded;
    private float defaultDrag;

    [SerializeField] Camera MainCam;
    Plane plane = new Plane();
    private float distance = 0;


    // Start is called before the first frame update
    void Start()
    {
        PlayerRb = GetComponent<Rigidbody>();
        defaultDrag = PlayerRb.drag;
    }

    private void Update()
    {
        Ray ray = MainCam.ScreenPointToRay(LookPosition);

        plane.SetNormalAndPosition(Vector3.up, transform.localPosition);
        if (plane.Raycast(ray, out distance))
        {
            Vector3 lookPoint = ray.GetPoint(distance);
            pointer.position = lookPoint;
            transform.LookAt(lookPoint);
        }

        if (Fire2Flag)
        {
            RaycastHit hit;
            if(Physics.Linecast(transform.position,pointer.position, out hit,TargettingLayer))
            {
                Transform hitTarget = hit.transform;
                if (!TargetEnemys.Contains(hitTarget))
                {
                    TargetEnemys.Add(hitTarget);
                    hitTarget.GetComponent<EnemyController>().TargetFromPlayer();
                }
            }
            targetting = true;
        }
        else
        {
            if (targetting)
            {
                RaizerFire();
                targetting = false;
            }
        }

        if (FireFlag)
        {
            if (!fireTrigger)
            {
                StartCoroutine(FireTimer());
                fireTrigger = true;
            }
        }
        else
        {
            fireTrigger = false;
            StopCoroutine(FireTimer());
            firePoint.localEulerAngles = Vector3.zero;
        }

        
        
        if (Physics.Linecast(charaRay.position, (charaRay.position - transform.up * charaRayRange)))
        {
            isGrounded = true;
        }
        else
        {
            isGrounded = false;
        }
        Debug.DrawLine(charaRay.position, (charaRay.position - transform.up * charaRayRange));

        
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        Vector3 force = new Vector3(Velocity.x, 0, Velocity.y) * PlayerSpeed;
        if (isGrounded)
        {
            PlayerRb.drag = defaultDrag;
            PlayerRb.AddForce(force * PlayerRb.mass * PlayerRb.drag / (1f - PlayerRb.drag * Time.fixedDeltaTime));
        }
        else
        {
            PlayerRb.drag = 0f;
        }
    }

    void BulletFire()
    {
        GameObject bullet = Instantiate(Bullet, firePoint.position, firePoint.rotation);
        bullet.GetComponent<Rigidbody>().AddForce(firePoint.forward * bulletVelocity);

        fireFX.Play();

        Destroy(bullet, 1f);

        float recoil = Random.Range(-RecoilRange, RecoilRange);
        firePoint.localRotation = Quaternion.AngleAxis(recoil, Vector3.up);
    }

    void RaizerFire()
    {
        for(int i = 0; i < TargetEnemys.Count; i++)
        {
            GameObject raizer = Instantiate(Raizer, firePoint.position, firePoint.rotation);

            raizer.GetComponent<Raizer>().target = TargetEnemys[i];
        }
        TargetEnemys = new List<Transform>();
    }

    IEnumerator FireTimer()
    {
        while (true)
        {
            Debug.Log("Fire!");
            BulletFire();
            yield return new WaitForSeconds(fireRaito);
            if (!FireFlag) yield break;
        }
    }
}
