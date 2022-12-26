using System;
using System.Collections.Generic;
using NtlStudio.TreasureHunters.Model;
using TreasureHunters;
using UnityEngine;

public class Enemies : MonoBehaviour
{
    public GameObject EnemyPrefab;

    private static GameClient Game => GameClient.Instance();

    private List<GameObject> _enemies = new();

    public void ShowEnemy(int x, int y)
    {
        var enemyObject = Instantiate(EnemyPrefab, transform);
        enemyObject.transform.position = new Vector3(x, y);
        _enemies.Add(enemyObject);
    }

    public void ClearEnemies()
    {
        _enemies.Clear();
    }

    void Start()
    {
        Debug.Assert(EnemyPrefab != null);
    }
}
