using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class MainShowWaveInfoDlg : GUIBase
{
    public TextMeshProUGUI textEnemyCnt;
    public TextMeshProUGUI textWave;
    public TextMeshProUGUI textBulletCnt;
    public TextMeshProUGUI textScore;

    public Button btnPause;

    void Start()
    {
        Game.Instance.EventNextWaveBegin += OnWaveBegin;
        Game.Instance.EventNextWaveEnd += OnWaveEnd;
        Game.Instance.GameSpawner.EventWaveChanged += OnNewWave;
        Game.Instance.EventLoadSceneBegin += OnSceneLoadBegin;
        Game.Instance.EventLoadSceneEnd += OnSceneLoadEnd;
        Game.Instance.GamePlayer.GunController.EventBulletCntChanged += OnBulletCntChanged;
        Game.Instance.ScoreManager.EventScoreChange += OnScoreChanged;
        transform.SetAsFirstSibling();
        OnScoreChanged(0);
        btnPause.onClick.AddListener(OnClickPause);
    }

    private void OnDestroy()
    {
        if (Game.Instance != null)
        {
            Game.Instance.EventNextWaveBegin -= OnWaveBegin;
            Game.Instance.EventNextWaveEnd -= OnWaveEnd;
            Game.Instance.GameSpawner.EventWaveChanged -= OnNewWave;
            Game.Instance.GamePlayer.GunController.EventBulletCntChanged -= OnBulletCntChanged;
            Game.Instance.ScoreManager.EventScoreChange -= OnScoreChanged;
            Game.Instance.EventLoadSceneBegin -= OnSceneLoadBegin;
            Game.Instance.EventLoadSceneEnd -= OnSceneLoadEnd;
        }
    }

    void OnClickPause()
    {
        if (Game.Instance != null)
        {
            Game.Instance.Pause();
            Game.Instance.GUI.Show("MenuDlg");
        }
    }

    void OnSceneLoadBegin()
    {
        Game.Instance.GamePlayer.GunController.EventBulletCntChanged -= OnBulletCntChanged;
    }

    void OnSceneLoadEnd()
    {
        Game.Instance.GamePlayer.GunController.EventBulletCntChanged += OnBulletCntChanged;
    }

    void OnWaveBegin()
    {
        textEnemyCnt.gameObject.SetActive(false);
        textWave.gameObject.SetActive(false);
        textBulletCnt.gameObject.SetActive(false);
    }

    void OnWaveEnd()
    {
        textEnemyCnt.gameObject.SetActive(true);
        textWave.gameObject.SetActive(true);
        textBulletCnt.gameObject.SetActive(true);
    }

    public void OnNewWave(int waveNum, int enemyCnt)
    {
        textEnemyCnt.text = string.Format("Enemy Count:{0}", enemyCnt < 0? "infinite": enemyCnt);
        textWave.text = string.Format("Wave {0}", waveNum);
    }

    void OnBulletCntChanged(int bulletCnt)
    {
        textBulletCnt.text = string.Format("Bullet Count:{0}", bulletCnt);
    }

    void OnScoreChanged(int score)
    {
        textScore.text = string.Format("Score: {0}", Game.Instance.ScoreManager.score);
    }
}
