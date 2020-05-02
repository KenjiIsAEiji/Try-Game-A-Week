using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName ="MyScriptableObjct/Create EnemyData")]
public class EnemyData : ScriptableObject
{
    [SerializeField] string name;
    public bool MiddleEnemyType;
    public float EnemyMaxSpeed = 2f;
    public float AimDistance = 8f;
    public float FireRaito = 0.4f;
    public float BulletSpeed = 10f;
    public float EnergyCellPercent = 10f;

    public GameObject BulletModel;
    public float BulletLifeTime;
}
