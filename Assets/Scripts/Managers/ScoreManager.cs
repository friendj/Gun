using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScoreManager : Singleton<ScoreManager>
{
    float _score;

    public event System.Action<int> EventScoreChange;

    float lastGetScoreTime;
    float streakGetScoreTime = 2f;
    int streakTime = 0;

    public int score
    {
        get
        {
            return (int)_score;
        }
    }

    protected override void Awake()
    {
        base.Awake();
        DontDestroyOnLoad(gameObject);
    }

    protected void Start()
    {
        Enemy.EventEnemyDeath += OnEnemyDeath;
    }

    private void OnEnemyDeath()
    {
        _score += 5;

        if (Time.time < lastGetScoreTime + streakGetScoreTime)
        {
            streakTime += 1;
            _score += streakTime;
        }
        else
        {
            streakTime = 0;
        }
        lastGetScoreTime = Time.time;

        if (EventScoreChange != null)
        {
            EventScoreChange((int)_score);
        }
    }
}
