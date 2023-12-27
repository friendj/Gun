using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spawner : Singleton<Spawner>
{
    public bool devMode;

    public Waves waves;
    public Enemy[] enemy;

    Waves.Wave currentWave;
    int currentWaveNumber;

    int enemiesRemainingToSpawn;
    int enemiesRemainingAlive;
    float nextSpawnTime;

    MapGenerator map;

    LivingEntity player;
    Transform playerTrans;
    Vector3 playerPos;
    bool isPlayerComping;
    float playerCampingDis = 1.5f;
    float nextCheckCampingTime = 0;
    float checkCampingTime = 2f;

    bool isBeginWave;

    bool isDisable;

    public System.Action<int ,int> EventWaveChanged;

    private void Start()
    {
        player = FindObjectOfType<Player>();
        if (player != null)
        {
            playerTrans = player.transform;
            playerPos = playerTrans.position;
            player.EventOnDeath += OnPlayerDeath;
        }
        nextCheckCampingTime = checkCampingTime;

        map = FindObjectOfType<MapGenerator>();
        NextWave();

        if (Game.Instance != null)
        {
            Game.Instance.EventNextWaveCenter += NextWaveCenter;
            Game.Instance.EventNextWaveEnd += NextWaveEnd;
        }
    }

    protected override void OnDestroy()
    {
        if (Game.Instance != null)
        {
            Game.Instance.EventNextWaveCenter -= NextWaveCenter;
            Game.Instance.EventNextWaveEnd -= NextWaveEnd;
        }
    }

    private void Update()
    {
        if (isDisable)
            return;
        if (!isBeginWave)
        {
            return;
        }
        CheckCampingTime();
        Spawn();

        if (devMode && Input.GetKeyDown(KeyCode.K))
        {
            StopCoroutine("CreateEnemy");
            foreach (Enemy enemy in FindObjectsOfType<Enemy>())
                Destroy(enemy.gameObject);
            NextWave();
        }
    }

    void CheckCampingTime()
    {
        if (player == null)
        {
            isPlayerComping = false;
            return;
        }
        if (Time.time > nextCheckCampingTime)
        {
            nextCheckCampingTime = Time.time + checkCampingTime;
            isPlayerComping = Vector3.Distance(playerPos, playerTrans.position) < playerCampingDis;
            playerPos = playerTrans.position;
        }
    }

    void Spawn()
    {
        if ((enemiesRemainingToSpawn > 0 || currentWave.infinite) && Time.time > nextSpawnTime)
        {
            enemiesRemainingToSpawn--;
            nextSpawnTime = Time.time + currentWave.timeBetweenSpawns;

            StartCoroutine("CreateEnemy");
        }
    }

    IEnumerator CreateEnemy()
    {
        Transform tile;
        if (isPlayerComping)
            tile = map.GetTileByPos(playerPos);
        else
            tile = map.GetRandomOpenTile();

        Tile tileScript = tile.GetComponent<Tile>();

        int playTime = 8;
        int playAnimTime = 0;

        while(playAnimTime < playTime)
        {
            tileScript.SetCreateEnemyColor();
            yield return new WaitForSeconds(0.1f);
            tileScript.SetNormalColor();
            yield return new WaitForSeconds(0.1f);
            playAnimTime += 1;
        }

        int enemyIndex = Random.Range(0, enemy.Length);
        Enemy spawnedEnemy = Instantiate(enemy[enemyIndex], tile.position + Vector3.up, Quaternion.identity) as Enemy;
        spawnedEnemy.SetBaseMatColor(currentWave.skinColor);
        spawnedEnemy.EventOnDeath += OnEnemyDeath;
    }

    void OnEnemyDeath()
    {
        enemiesRemainingAlive--;
        if (EventWaveChanged != null)
            EventWaveChanged(currentWaveNumber, enemiesRemainingAlive);
        if (enemiesRemainingAlive <= 0 && !currentWave.infinite)
        {
            NextWave();
        }
    }

    void OnPlayerDeath()
    {
        isDisable = true;
    }

    void NextWave()
    {
        if (Game.Instance == null)
        {
            isBeginWave = true;
            currentWave = waves.GetWave(1);
            map.GenerateMap(0);
            return;
        }
        if (currentWaveNumber > 0)
            Game.Instance.AudioManager.PlaySound2D("LevelComplete");
        GUIBase ui = Game.Instance.GUI.Show("NextWaveDlg");
        NextWaveDlg nextWaveDlg = ui.GetComponent<NextWaveDlg>();
        if (currentWaveNumber > 0)
        {
            nextWaveDlg.ShowIn();
        }
        else
        {
            NextWaveCenter();
        }
        isBeginWave = false;
    }

    void NextWaveCenter()
    {
        GUIBase ui = Game.Instance.GUI.Show("NextWaveDlg");
        NextWaveDlg nextWaveDlg = ui.GetComponent <NextWaveDlg>();
        nextWaveDlg.ShowOut();
        currentWaveNumber++;
        map.GenerateMap(currentWaveNumber - 1);
        currentWave = waves.GetWave(currentWaveNumber - 1);
        enemiesRemainingToSpawn = currentWave.enemyCount;
        enemiesRemainingAlive = enemiesRemainingToSpawn;
        // set player in MapCenter
        player.transform.position = new Vector3(0, 1, 0);
        ui = Game.Instance.GUI.Show("MainShowWaveInfoDlg");
        MainShowWaveInfoDlg mainShowWaveInfoDlg = ui.GetComponent<MainShowWaveInfoDlg>();
        mainShowWaveInfoDlg.OnNewWave(currentWaveNumber, currentWave.enemyCount);
        if (EventWaveChanged != null)
        {
            EventWaveChanged(currentWaveNumber, currentWave.enemyCount);
        }
    }

    void NextWaveEnd()
    {
        GUIBase ui = Game.Instance.GUI.Show("InfoTipsDlg");
        if (ui != null)
        {
            InfoTipsDlg infoTipsDlg = ui.GetComponent<InfoTipsDlg>();
            infoTipsDlg.SetText(string.Format("- Wave {0} -\nEnemy Count: {1}", currentWaveNumber, currentWave.infinite ? "infinite" : currentWave.enemyCount));
        }
        isPlayerComping = false;
        isBeginWave = true;
    }
}
