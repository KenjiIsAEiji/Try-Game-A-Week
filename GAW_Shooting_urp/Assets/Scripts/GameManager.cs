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

    [SerializeField] GameObject NormalEnemyModel;
    [SerializeField] GameObject MiddleEnemyModel;

    [Header("Player参照用")]
    [SerializeField] PlayerController player;
    [SerializeField] PlayerStatus status;

    [Header("Score表示&HUD全般")]
    [SerializeField] CanvasGroup EndCanvasGroup;
    [SerializeField] CanvasGroup HudCanvasGroup;
    [SerializeField] CanvasGroup StartCanvasGroup;
    [SerializeField] Text WaveText;
    [SerializeField] Text ResultText;
    [SerializeField] Text SpaceTo;

    [Header("Beacon&Enemyリスト")]
    [SerializeField] List<SpawnBeacon> beacons;
    public List<GameObject> Enemys;

    [Header("Wave調整用パラメータ")]
    [SerializeField] int BaseSpawnEnemys = 5;
    int spawnEnemys;
    [SerializeField] int MiddleEnemyParcent = 10;
    [SerializeField] int LpBonusWaves = 5;
    
    bool resulted = false;

    [Header("スコア基底ポイント")]
    [SerializeField] int nomalPoint = 10;
    [SerializeField] int middlePoint = 35;
    [SerializeField] int WaveBonus = 100;
    [Header("Audio")]
    [SerializeField] AudioSource fightBGM;
    [SerializeField] AudioSource startBGM;

    //スコア算出用変数
    int nomalDestroy = 0;
    int middleDestroy = 0;
    int WaveCount = -1;
    string scoreTxt;

    // Start is called before the first frame update
    void Start()
    {
        player = FindObjectOfType<PlayerController>();
        status = FindObjectOfType<PlayerStatus>();
        spawnEnemys = BaseSpawnEnemys - 1;
        gameState = GameState.Ready;
        StartUIAnimation();
        startBGM.Play();
    }

    // Update is called once per frame
    void Update()
    {
        switch (gameState)
        {
            case GameState.Ready:
                resulted = false;
                nomalDestroy = 0;
                middleDestroy = 0;
                WaveCount = -1;

                if (Keyboard.current.spaceKey.isPressed)
                {
                    PlayerStatusReset();
                    gameState = GameState.Play;
                    ToPlayAnimation();
                    fightBGM.Play();
                }

                break;
            case GameState.Play:
                resulted = false;

                if(Enemys.Count <= 0)
                {
                    spawnEnemys++;
                    SpawnPointChoice(spawnEnemys);
                    WaveCount++;

                    if(WaveCount % LpBonusWaves == 0)
                    {
                        status.AddLp(status.PlayerMaxLp / 0.75f);
                    }

                    NextWaveAnimation();

                    Debug.Log("Wave "+ WaveCount);

                }

                if (status.IsDead)
                {
                    gameState = GameState.End;
                    GameOverAnimation();
                }

                break;
            case GameState.End:
                int score = (nomalDestroy * nomalPoint) + (middleDestroy * middlePoint) + (WaveCount * WaveBonus);

                scoreTxt = "nomalEnemy Destroied  " + (nomalDestroy * nomalPoint).ToString() +"p\n";
                scoreTxt += "middleEnemy Destroied  " + (middleDestroy * middlePoint).ToString() +"p\n";
                scoreTxt += "Wave Bonus  " + WaveCount + " Waves  " + (WaveCount * WaveBonus).ToString() + "p\n";
                scoreTxt += "<size= 60>Score  " + score + "p</size>";
                
                player.enabled = false;
                player.GetComponent<Collider>().enabled = false;
                player.GetComponent<Rigidbody>().isKinematic = true;

                if (Keyboard.current.enterKey.isPressed && resulted)
                {
                    SceneManager.LoadScene(0);
                    gameState = GameState.Ready;
                }

                break;
        }
        SpaceTo.enabled = resulted;

    }

    void PlayerStatusReset()
    {
        player.magazine = player.MaxBullets;
        status.PlayerLp = status.PlayerMaxLp;
    }

    //

    void NextWaveAnimation()
    {
        string txt = "Wave " + WaveCount;
        Sequence s = DOTween.Sequence();
        s.Append(WaveText.DOText(txt, 2.5f, true, ScrambleMode.All));
        s.Append(WaveText.DOText("", 2.5f, true, ScrambleMode.All));
        s.Play().OnComplete(() =>
        {
            WaveText.text = "";
        });
    }
    
    void StartUIAnimation()
    {
        Sequence sequence = DOTween.Sequence();
        sequence.Append(StartCanvasGroup.DOFade(1f, 0.1f));
        sequence.Join(EndCanvasGroup.DOFade(0f, 0.1f));
        sequence.Join(HudCanvasGroup.DOFade(0f, 0.1f));
        sequence.Play().OnComplete(() =>
        {
            StartCanvasGroup.alpha = 1;
            EndCanvasGroup.alpha = 0;
            HudCanvasGroup.alpha = 0;
        });
    }

    void ToPlayAnimation()
    {
        Sequence sequence = DOTween.Sequence();
        sequence.Append(StartCanvasGroup.DOFade(0f, 0.1f));
        sequence.Join(HudCanvasGroup.DOFade(1f, 0.1f));
        sequence.Join(startBGM.DOFade(0, 0.1f));
        sequence.Join(fightBGM.DOFade(1f, 0.1f));

        sequence.Play().OnComplete(() =>
        {
            StartCanvasGroup.alpha = 0;
            EndCanvasGroup.alpha = 0;
            HudCanvasGroup.alpha = 1;
            startBGM.Stop();
        });
    }

    void GameOverAnimation()
    {
        Time.timeScale = 0.1f;

        EndCanvasGroup.DOFade(1f, 0.1f).SetDelay(0.2f).OnComplete(() => { 
            
            Time.timeScale = 1f;
            ResultUIAnimation();

        });
    }

    void ResultUIAnimation()
    {
        ResultText.DOText(scoreTxt, scoreTxt.Length * 0.01f).SetEase(Ease.Linear).OnComplete(() => {

            resulted = true;
        
        });
    }

    void SpawnPointChoice(int enemys)
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
                Spawn(spawn);
            }
        }

        for (int k = 0; k < odd; k++)
        {
            Transform spawn = choiceBeacons[k].transform;
            Spawn(spawn);
        }
    }

    private void Spawn(Transform point)
    {
        GameObject enemy;

        int dice = Random.Range(1, 101);

        if(dice <= MiddleEnemyParcent)
        {
            enemy = MiddleEnemyModel;
        }
        else
        {
            enemy = NormalEnemyModel;
        }

        Enemys.Add(Instantiate(enemy, point.position, point.rotation));
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
