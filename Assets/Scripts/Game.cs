using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using static UI;

public class Game : Singleton<Game>
{
    public GameObject uiManagerPrefab;
    private UIManager _uiManager;
    private AudioManager _audioManager;
    private ScoreManager _scoreManager;
    private AssetBundleManager _assetBundleManager;

    public LivingEntity player;
    public Player gamePlayer;

    public System.Action EventNextWaveEnd;
    public System.Action EventNextWaveCenter;
    public System.Action EventNextWaveBegin;
    public System.Action EventLoadSceneBegin;
    public System.Action EventLoadSceneEnd;

    [Header("Music")]
    public AudioClip mainTheme;
    public AudioClip menuTheme;

    [Header("AudioDictionary")]
    public AudioDictionary audioDictionary;

    protected override void Awake()
    {
        if (Instance == null)
        {
            FindPlayer();
            DontDestroyOnLoad(gameObject);
        }
        SceneManager.sceneLoaded += OnSceneLoaded;
        base.Awake();
    }

    protected override void OnDestroy()
    {
        base.OnDestroy();
    }

    private void Start()
    {
        AudioManager.PlayMusic(mainTheme, 2);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            AudioManager.PlayMusic(menuTheme, 3);
        }
        if (Input.GetKeyDown(KeyCode.C))
        {
            AudioManager.PlayMusic(mainTheme, 3);
        }
        if (Input.GetKeyDown(KeyCode.G))
        {
            GameOver();
        }
    }

    private void GameOver()
    {
        GUI.Show("GameOverUI");
        //GUI.Show("gui/gameoverui.unity3d", "GameOverUI");
    }

    public UIManager GUI
    {
        get
        {
            if (_uiManager == null)
            {
                if (uiManagerPrefab == null)
                {
                    GameObject obj = new GameObject("UIManager");
                    obj.transform.parent = transform;
                    _uiManager = obj.AddComponent<UIManager>();
                }
                else
                {
                    _uiManager = Instantiate(uiManagerPrefab, transform).GetComponent<UIManager>();
                }
            }
            return _uiManager;
        }
    }

    public AudioManager AudioManager
    {
        get
        {
            if (_audioManager == null)
            {
                GameObject audio = new GameObject("AudioManager");
                _audioManager = audio.AddComponent<AudioManager>();
                _audioManager.SetAudioDict(audioDictionary);
            }
            return _audioManager;
        }
    }

    public AssetBundleManager AssetBundleManager
    {
        get
        {
            if (_assetBundleManager == null)
            {
                GameObject abObj = new GameObject("AssetBundleManager");
                _assetBundleManager = abObj.AddComponent<AssetBundleManager>();
            }
            return _assetBundleManager;
        }
    }

    public ScoreManager ScoreManager
    {
        get
        {
            if (_scoreManager == null)
            {
                GameObject score = new GameObject("scoreManager");
                _scoreManager = score.AddComponent<ScoreManager>();
            }
            return _scoreManager;
        }
    }

    public Spawner GameSpawner
    {
        get
        {
            return Spawner.Instance;
        }
    }

    public Player GamePlayer
    {
        get
        {
            return gamePlayer;
        }
    }

    private void FindPlayer()
    {
        player = FindObjectOfType<Player>();
        if (player != null)
        {
            gamePlayer = player.GetComponent<Player>();
            player.EventOnDeath += GameOver;
        }
    }

    public void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        FindPlayer();
        if (EventLoadSceneEnd != null)
        {
            EventLoadSceneEnd();
        }
    }

    public void StartNewGame()
    {
        if (EventLoadSceneBegin != null)
        {
            EventLoadSceneBegin();
        }
        SceneManager.LoadScene("Game1");
    }

    public void NextWaveCenter()
    {
        if (EventNextWaveCenter != null)
        {
            EventNextWaveCenter();
        }
    }

    public void NewWaveEnd()
    {
        if (EventNextWaveEnd != null)
        {
            EventNextWaveEnd();
        }
    }

    public void NextWaveBegin()
    {
        if (EventNextWaveBegin != null)
        {
            EventNextWaveBegin();
        }
    }
}
