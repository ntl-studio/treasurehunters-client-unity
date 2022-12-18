using System;
using System.Collections.Generic;
using NtlStudio.TreasureHunters.Model;
using TreasureHunters;
using UnityEngine;

public class Enemies : MonoBehaviour
{
    public GameObject PlayerPrefab;

    private static GameClient Game => GameClient.Instance();

    private readonly Dictionary<string, GameObject> _enemyViews = new();

    void Start()
    {
        Debug.Assert(PlayerPrefab);

        foreach (var enemy in Game.Enemies)
        {
            var enemyObject = Instantiate(PlayerPrefab, transform);
            enemyObject.SetActive(false);

            var spriteRenderer = enemyObject.GetComponent<SpriteRenderer>();
            Debug.Assert(spriteRenderer);
            spriteRenderer.color = UnityEngine.Color.yellow;

            _enemyViews.Add(enemy.Key, enemyObject);
        }

        Game.OnUpdateVisibleArea += UpdateEnemiesVisibility;
        Game.OnEndMove += UpdateEnemiesVisibility;
    }

    void UpdateEnemiesVisibility()
    {
        var visibilityArea = Game.CurrentVisibleArea();

        foreach (var enemy in Game.Enemies)
        {
            _enemyViews[enemy.Key].SetActive(false);

            var playerPos = Game.PlayerPosition;
            var enemyPosition = enemy.Value;

            if (Math.Abs(enemyPosition.X - playerPos.X) <= 1 &&
                    Math.Abs(enemyPosition.Y - playerPos.Y) <= 1)
            {
                if (!visibilityArea[Math.Abs(playerPos.X - enemyPosition.X - 1), Math.Abs(playerPos.Y - enemyPosition.Y - 1)]
                        .HasFlag(FieldCell.Invisible))
                {
                    _enemyViews[enemy.Key].SetActive(true);
                }
            }
        }
    }
}
