using System;
using System.Collections.Generic;
using UnityEngine;

public class UnidadArquero: AgentNPC
{
    private new void Awake()
    {
        base.Awake();
        terrainCosts = new Dictionary<TerrainType, float>();
    }
    
    private new void Start()
    {
        base.Start();
        _mass = 2f;
        _maxSpeed = 3f; 
        _maxRotation = 2f;
        _maxAcceleration = 2f;
        _maxAngularAcc = 2f;
        _maxForce = 2f;
        _baseDamage = 20f;
        _attackRange = 14f;
        _attackSpeed = 4f;
        _hpMax = 100;
        _hpCurrent = 100;
        
        GenerateCostsDict();
    }

    private new void Update()
    {
        base.Update();
    }

    private void GenerateCostsDict()
    {
        terrainCosts.Add(TerrainType.Floor, 1f);
        terrainCosts.Add(TerrainType.Grass, 1f);
        terrainCosts.Add(TerrainType.Stones, 1.5f);
        terrainCosts.Add(TerrainType.Sand, 3f);
        terrainCosts.Add(TerrainType.Lava, 15f);
        terrainCosts.Add(TerrainType.Water, Mathf.Infinity);
    }
    
    protected override float GetVelocityFromTerrain()
    {
        TerrainType terrain = GetActualTerrain();
        switch (terrain)
        {
            case TerrainType.Stones:
            {
                return 0.9f;
            }
            case TerrainType.Sand:
            {
                return 0.6f;
            }
            case TerrainType.Water:
            {
                return 0f;
            }
            case TerrainType.Lava:
            {
                return 0.02f;
            }
            default:
            {
                return 1f;
            }
        }
    }
}
