using System.Collections.Generic;
using UnityEngine;

public class UnidadCaballeria : AgentNPC
{
    private new void Awake()
    {
        base.Awake();
        terrainCosts = new Dictionary<TerrainType, float>();
    }

    private new void Start()
    {
        base.Start();
        _mass = 4f;
        _maxSpeed = 7f;
        _maxRotation = 5f;
        _maxAcceleration = 4f;
        _maxAngularAcc = 10f;
        _maxForce = 3f;
        _baseDamage = 30f;
        _attackRange = 6f;
        _attackSpeed = 4f;
        _hpMax = 130;
        _hpCurrent = 130;
        
        GenerateCostsDict();
    }

    private new void Update()
    {
        base.Update();
    }

    private void GenerateCostsDict()
    {
        terrainCosts.Add(TerrainType.Grass, 1f);
        terrainCosts.Add(TerrainType.Stones, 1.5f);
        terrainCosts.Add(TerrainType.Floor, 5f);
        terrainCosts.Add(TerrainType.Sand, 10f);
        terrainCosts.Add(TerrainType.Lava, 15f);
        terrainCosts.Add(TerrainType.Water, Mathf.Infinity);
    }
}