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
    public bool ReloadButton { get; set; }
    public bool SwitchWepon { get; set; }
    private bool swithTrigger = false;

    [SerializeField] PlayerStatus status;

    [Header("弾丸発射関連")]
    [SerializeField] GameObject Bullet;
    [SerializeField] Transform firePoint;
    [SerializeField] ParticleSystem fireFX;
    [SerializeField] float fireRaito = 0.1f;
    [SerializeField] float bulletVelocity = 10f;
    [SerializeField] float RecoilRange = 5f;
    private bool fireTrigger = false;

    [Header("追尾レーザ関連")]
    public bool RaizerMode = false;
    [SerializeField] GameObject Raizer;
    [SerializeField] Transform pointer;
    [SerializeField] LayerMask TargettingLayer;
    [SerializeField] List<Transform> TargetEnemys;
    private bool targetting = false;
    public float RecastTime = 45f;
    public float Casting = 0;

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

    [Header("弾丸管理")]
    public int MaxBullets = 35;
    public int magazine = 0;
    public bool reloading = false;
    [SerializeField] float ReloadTime = 5f;


    // Start is called before the first frame update
    void Start()
    {
        PlayerRb = GetComponent<Rigidbody>();
        defaultDrag = PlayerRb.drag;
        magazine = MaxBullets;
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

        if (SwitchWepon)
        {
            if (!swithTrigger)
            {
                swithTrigger = true;
                RaizerMode = !RaizerMode;
            }
        }
        else
        {
            swithTrigger = false;
        }

        if(Casting < RecastTime)
        {
            Casting += Time.deltaTime;
        }
        else
        {
            Casting = RecastTime;
        }

        if (RaizerMode)
        {
            if (FireFlag)
            {
                if (Casting >= RecastTime)
                {
                    RaycastHit hit;
                    if (Physics.Linecast(transform.position, pointer.position, out hit, TargettingLayer))
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
            }
            else
            {
                if (targetting)
                {
                    RaizerFire();
                    targetting = false;
                    Casting = 0;
                }
            }
        }
        else
        {
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
        }

        if (ReloadButton && !reloading)
        {
            StartCoroutine(Reload());
        }

        if (Physics.Linecast(charaRay.position, (charaRay.position - transform.up * charaRayRange)))
        {
            isGrounded = true;
        }
        else
        {
            isGrounded = false;
        }
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
        if(magazine > 0 && !reloading)
        {
            GameObject bullet = Instantiate(Bullet, firePoint.position, firePoint.rotation);
            bullet.GetComponent<Rigidbody>().AddForce(firePoint.forward * bulletVelocity);
            bullet.GetComponent<BulletFXs>().DamegePoint = status.GetBulletDamege();

            magazine--;
            status.UseBullet();

            fireFX.Play();

            Destroy(bullet, 1f);

            float recoil = Random.Range(-RecoilRange, RecoilRange);
            firePoint.localRotation = Quaternion.AngleAxis(recoil, Vector3.up);
        }
    }

    void RaizerFire()
    {
        status.UseRaizer();
        for(int i = 0; i < TargetEnemys.Count; i++)
        {
            GameObject raizer = Instantiate(Raizer, firePoint.position, firePoint.rotation);
            Raizer rai = raizer.GetComponent<Raizer>();

            rai.target = TargetEnemys[i];
            rai.DamegePoint = status.GetRaizerDamege();
        }
        TargetEnemys = new List<Transform>();
    }

    IEnumerator Reload()
    {
        reloading = true;
        yield return new WaitForSeconds(ReloadTime);
        magazine = MaxBullets;
        reloading = false;
        yield break;
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
