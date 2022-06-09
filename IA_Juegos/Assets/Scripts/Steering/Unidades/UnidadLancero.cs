using System;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
public class UnidadLancero: AgentNPC
{
    private new void Awake()
    {
        base.Awake();
        terrainCosts = new Dictionary<TerrainType, float>();
    }
    
    // Start is called before the first frame update
    private new void Start()
    {
        base.Start();
        _mass = 3f;
        _maxSpeed = 3f;
        _maxRotation = 2f;
        _maxAcceleration = 2f;
        _maxAngularAcc = 2f;
        _maxForce = 4f;
        _baseDamage = 40f;
        _attackRange = 6f;
        _attackSpeed = 4f;
        _hpMax = 250;
        _hpCurrent = 250;
        
        GenerateCostsDict();
    }

    // Update is called once per frame
    private new void Update()
    {
        base.Update();
    }
    
    private void GenerateCostsDict()
    {
        terrainCosts.Add(TerrainType.Floor, 1f);
        terrainCosts.Add(TerrainType.Stones, 1.5f);
        terrainCosts.Add(TerrainType.Sand, 3f);
        terrainCosts.Add(TerrainType.Grass, 5f);
        terrainCosts.Add(TerrainType.Lava, 15f);
        terrainCosts.Add(TerrainType.Water, Mathf.Infinity);
    }
}



