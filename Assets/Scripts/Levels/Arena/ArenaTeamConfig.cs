using System;
using System.Collections.Generic;
using System.Linq;
using BasicTools.ButtonInspector;
using UnityEngine;

public class ArenaTeamConfig: MonoBehaviour
{
    public List<Transform> spawnPoints;
    public Randomizer<Transform> spawnPointsRandomizer;

    [Button("Use Children As Spawn Points", "UseChildrenAsSpawnPoints")]
    public bool useChildrenAsSpawnPoints_Btn;

    public void Awake()
    {
        Init();
    }

    void Init()
    {
        spawnPointsRandomizer = new Randomizer<Transform>(spawnPoints);
    }

    public void UseChildrenAsSpawnPoints()
    {
        spawnPoints = GetComponentsInChildren<Transform>().ToList();
        spawnPoints.Remove(transform);
    }
}