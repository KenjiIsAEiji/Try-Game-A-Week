using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class GameManager : SingletonMonoBehaviour<GameManager>
{
    public enum GameState
    {
        Ready,
        Play,
        End
    }

    [SerializeField] GameState gameState;

    [SerializeField] GameObject EnemyModel;

    [Header("Player参照用")]
    [SerializeField] PlayerController player;
    [SerializeField] PlayerStatus status;

    [Header("Score表示")]
    [SerializeField] CanvasGroup canvasGroup;
    [SerializeField] Text ScoreText;

    [Header("Beaco&Enemyリスト")]
    [SerializeField] List<SpawnBeacon> beacons;
    public List<GameObject> Enemys;

    [Header("Wave調整用パラメータ")]
    [SerializeField] int testSpawnEnemys = 5;
    bool resulted = false;

    [Header("スコア基底ポイント")]
    [SerializeField] int nomalPoint = 10;
    [SerializeField] int middlePoint = 35;
    [SerializeField] int WaveBonus = 100;

    //スコア算出用変数
    [SerializeField] int nomalDestroy = 0;
    [SerializeField] int middleDestroy = 0;
    int WaveCount = -1;

    // Start is called before the first frame update
    void Start()
    {
        player = FindObjectOfType<PlayerController>();
        status = FindObjectOfType<PlayerStatus>();
    }

    // Update is called once per frame
    void Update()
    {
        switch (gameState)
        {
            case GameState.Ready:
                //resulted = false;

                break;
            case GameState.Play:
                resulted = false;

                if(Enemys.Count <= 0)
                {
                    EnemySpawn(testSpawnEnemys);
                    WaveCount++;
                    Debug.Log("Wave "+ WaveCount);
                }

                if (status.IsDead)
                {
                    gameState = GameState.End;
                    GameOverAnimation();
                }

                break;
            case GameState.End:
                player.enabled = false;
                player.GetComponent<Collider>().enabled = false;
                player.GetComponent<Rigidbody>().isKinematic = true;

                Debug.Log("Game Over");
                Debug.Log("nomalEnemy Destroied" + nomalDestroy + "*" + nomalPoint + "=" + (nomalDestroy * nomalPoint).ToString());
                Debug.Log("Wave Bonus" + WaveCount + "*" + WaveBonus + "=" + (WaveCount * WaveBonus).ToString());

                if (Keyboard.current.spaceKey.isPressed && resulted)
                {
                    SceneManager.LoadScene(0);
                    gameState = GameState.Play;
                }

                break;
        }
    }

    void GameOverAnimation()
    {
        Time.timeScale = 0.1f;
        canvasGroup.DOFade(1f, 0.1f).SetDelay(0.2f).OnComplete(() => { 
            
            Time.timeScale = 1f;
        });


    }

    void EnemySpawn(int enemys)
    {
        //spawnPoint choice
        Vector3 playerPosition = player.gameObject.transform.position;

        int minIndex = 0;
        List<SpawnBeacon> choiceBeacons = new List<SpawnBeacon>();

        for(int i = 1; i < beacons.Count; i++)
        {
            if (beacons[minIndex].GetDistance(playerPosition) > beacons[i].GetDistance(playerPosition))
            {
                choiceBeacons.Add(beacons[minIndex]);
                minIndex = i;
            }
            else
            {
                choiceBeacons.Add(beacons[i]);
            }
                
        }

        //spawning

        int odd = enemys % choiceBeacons.Count;

        int quotient = (int)((enemys - odd) / choiceBeacons.Count);

        for (int i = 0; i < quotient; i++)
        {
            for (int j = 0; j < choiceBeacons.Count; j++)
            {
                Transform spawn = choiceBeacons[j].transform;
                GameObject enemy = Instantiate(EnemyModel, spawn.position, spawn.rotation);
                Enemys.Add(enemy);
            }
        }

        for (int k = 0; k < odd; k++)
        {
            Transform spawn = choiceBeacons[k].transform;
            GameObject enemy = Instantiate(EnemyModel, spawn.position, spawn.rotation);
            Enemys.Add(enemy);
        }
    }

    public void EnemyDestroyed(bool middleType)
    {
        if (middleType)
        {
            middleDestroy++;
        }
        else
        {
            nomalDestroy++;
        }
    }
}
