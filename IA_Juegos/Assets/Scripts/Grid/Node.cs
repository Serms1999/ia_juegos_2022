using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using UnityEngine;

/**
 * @enum Tipo de terreno.
 */
public enum TerrainType
{
    Stones, /**< Piedras */
    Sand,   /**< Arena */
    Water,  /**< Agua */
    Floor,  /**< Suelo */
    Lava,   /**< Lava */
    Grass   /**< Hierba */
}

/**
 * @enum Equipos.
 */
public enum Teams
{
    None = 0,   /**< Ninguno */
    TeamA = 1,  /**< Equipo A */
    TeamB = -1  /**< Equipo B */
}

/**
 * @enum Tipo de mapa.
 */
public enum MapType
{
    Influence,      /**< Influencia */
    Tension,        /**< Tensión */
    Vulnerability   /**< Vulnerabilidad */
}

/**
 * @class Node
 * @brief Celda de la malla que cubre el mapa.
 */
public class Node
{
    // Informacion del nodo
    private Vector2Int _gridPosition;
    private Vector3 _wordlPosition;
    
    // Informacion sobre el pathfinding
    private float _hCost;
    private float _gCost;
    private bool _passable;
    private Node _parent;

    // Objetos que representa
    private GameObject _floorCell;
    private GameObject _influenceCell;
    
    // Informacion sobre el terreno
    private TerrainType _terrain;
    private static Dictionary<String, TerrainType> _nameToTerrain = new Dictionary<string, TerrainType>()
    {
        { "Brown Stony Light", TerrainType.Stones },
        { "Brown Stony", TerrainType.Stones },
        { "Floor 1", TerrainType.Floor },
        { "Grass", TerrainType.Grass },
        { "Grey Stones", TerrainType.Stones },
        { "Lava", TerrainType.Lava },
        { "Sandy Orange", TerrainType.Sand },
        { "Sandy", TerrainType.Sand },
        { "Water Deep Blue", TerrainType.Water },
        { "Water Light Blue", TerrainType.Water }
    };
    
    // Informacion sobre la influencia
    private Dictionary<Teams, float> _influences;

    /**
     * @return Posición de la celda en la malla.
     */
    public Vector2Int GridPosition
    {
        get { return _gridPosition; }
        set { _gridPosition = value; }
    }

    /**
     * @return Posición de la celda en el mundo.
     */
    public Vector3 WorldPosition
    {
        get { return _wordlPosition; }
        set { _wordlPosition = value; }
    }

    /**
     * @return Coste heurístico de la celda.
     */
    public float HCost
    {
        get { return _hCost; }
        set { _hCost = value; }
    }

    /**
     * @return Coste actual de la celda.
     */
    public float GCost
    {
        get { return _gCost; }
        set { _gCost = value; }
    }

    /**
     * @return true si la celda no tiene ningún muro encima.
     */
    public bool Passable
    {
        get { return _passable; }
        set { _passable = value; }
    }

    /**
     * @return Celda anterior en el camino.
     */
    public Node Parent
    {
        get { return _parent; }
        set { _parent = value; }
    }

    /**
     * @return Coste completo de la celda.
     */
    public float FCost
    {
        get { return _gCost + _hCost; }
    }
    
    /**
     * @return Tipo de terreno de la celda.
     */
    public TerrainType TerrainType
    {
        get { return _terrain; }
    }
    
    /**
     * @return Influencia global de la celda.
     */
    public float InfluenceValue
    {
        get { return _influences[Teams.TeamA] - _influences[Teams.TeamB]; }
    }

    /**
     * @return Valor de tensión de la celda.
     */
    public float TensionValue
    {
        get { return (_influences[Teams.TeamA] + _influences[Teams.TeamB]) / 2f; }
    }
    
    /**
     * @return Valor de vulnerabilidad de la celda.
     */
    public float VulnerabilityValue
    {
        get { return TensionValue - Mathf.Abs(InfluenceValue); }
    }

    /**
     * @brief Devuelve la influencia de un equipo en la celda.
     * @param[in] team Equipo
     * @return Influencia del equipo team en la celda.
     */
    public float TeamInfluence(Teams team)
    {
        return _influences[team];
    }

    /**
     * @brief Constructor de la clase Node.
     */
    public Node(Vector3 wordlPosition, Vector2Int gridPosition, float cellSize,
        GameObject floorCell, GameObject influenceCell)
    {
        this._wordlPosition = wordlPosition;
        this._gridPosition = gridPosition;
        this._passable = CheckPassable(cellSize);
        this._gCost = 0f;

        this._floorCell = floorCell;
        this._influenceCell = influenceCell;
        this._terrain = GetTerrainType();

        this._influences = new Dictionary<Teams, float>();
        foreach (Teams team in Enum.GetValues(typeof(Teams)).Cast<Teams>())
        {
            this._influences.Add(team, 0f);
        }
    }

    // Comprueba si hay un muro en la celda
    private bool CheckPassable(float cellSize)
    {
        UnityEngine.Collider[] collisions;
        collisions = Physics.OverlapBox(this._wordlPosition, Vector3.one * cellSize / 2f);
        if (collisions.Length == 0 || collisions.Any(n => n.CompareTag("Wall")))
        {
            return false;
        }

        return true;
    }
    
    // Obtiene el terreno de la celda
    private TerrainType GetTerrainType()
    {
        String materialName = _floorCell.GetComponent<Renderer>().material.name;
        Regex reMaterialName = new Regex(@"(.*) \(Instance\)");
        Match match = reMaterialName.Match(materialName);
        String terrainName = match.Groups[1].Value;
        return _nameToTerrain[terrainName];
    }
    
    /**
     * 
     */
    public void UpdateInfluence(Teams team, float value, MapType mapType)
    {
        _influences[team] = Mathf.Min(Mathf.Abs(value), 1f);
        ChangeCellColor(mapType);
    }

    /**
     * @brief Baja la influencia de la celda.
     * @param[in] decayValue Factor en el que se reduce la influencia.
     */
    public void DecayInfluence(float decayValue)
    {
        foreach (Teams team in Enum.GetValues(typeof(Teams)).Cast<Teams>())
        {
            _influences[team] *= decayValue;
        }
    }

    // Cambia el color de la celda de influencia
    private void ChangeCellColor(MapType mapType)
    {
        Color color = GetInfluenceColor(mapType);
        Renderer[] r = _influenceCell.GetComponents<Renderer>();
        Material m;

        foreach (Renderer rs in r)
        {
            m = rs.material;
            m.color = color;
            rs.material = m;
        }
    }

    // Obtiene el color para el mapa de influencia.
    private Color GetInfluenceColor(MapType mapType)
    {
        Color color = Color.gray;
        float value = 1f;
        switch (mapType)
        {
            case MapType.Influence:
            {
                switch (InfluenceValue)
                {
                    case < 0:
                    {
                        color = Color.red;
                        break;
                    }
                    case > 0:
                    {
                        color = Color.blue;
                        break;
                    }
                    default:
                    {
                        color = Color.gray;
                        break;
                    }
                }

                value = Mathf.Abs(InfluenceValue);
                break;
            }
            case MapType.Tension:
            {
                color = Color.blue;
                value = TensionValue / 2f;
                break;
            }
            case MapType.Vulnerability:
            {
                color = Color.red;
                value = VulnerabilityValue;
                break;
            }
        }
        
        return new Color(color.r * value, color.g * value, color.b * value);
    }
    
    protected bool Equals(Node other)
    {
        return _gridPosition.Equals(other._gridPosition);
    }

    public override bool Equals(object obj)
    {
        if (ReferenceEquals(null, obj)) return false;
        if (ReferenceEquals(this, obj)) return true;
        if (obj.GetType() != this.GetType()) return false;
        return Equals((Node) obj);
    }

    public override int GetHashCode()
    {
        return _gridPosition.GetHashCode();
    }
}
