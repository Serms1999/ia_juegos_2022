using System;
using System.Collections.Generic;
using UnityEngine;

public class UnidadInfanteria : AgentNPC
{
    private new void Awake()
    {
        base.Awake();
        terrainCosts = new Dictionary<TerrainType, float>();
    }
    
    private new void Start()
    {
        base.Start();
        _mass = 1f;
        _maxSpeed = 3f;
        _maxRotation = 2f;
        _maxAcceleration = 2f;
        _maxAngularAcc = 2f;
        _maxForce = 4f;
        _baseDamage = 10f;
        _attackRange = 6f;
        _attackSpeed = 4f;
        _hpMax = 200;
        _hpCurrent = 200;
        
        GenerateCostsDict();
    }

    private new void Update()
    {
        base.Update();
    }

    private void GenerateCostsDict()
    {
        terrainCosts.Add(TerrainType.Floor, 1f);
        terrainCosts.Add(TerrainType.Stones, 1.5f);
        terrainCosts.Add(TerrainType.Grass, 5f);
        terrainCosts.Add(TerrainType.Sand, 10f);
        terrainCosts.Add(TerrainType.Lava, 15f);
        terrainCosts.Add(TerrainType.Water, Mathf.Infinity);
    }
}