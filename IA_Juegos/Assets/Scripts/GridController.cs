using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

/**
 * @class GridController
 * @brief Controlador de la malla del mapa.
 */
public class GridController : MonoBehaviour
{
    private Grid _grid;                                 // Malla del mapa.
    [SerializeField] private GameObject floor;          // Objeto del mapa principal.
    [SerializeField] private GameObject influenceMap;   // Objeto del mapa de influencia.
    [SerializeField] private bool drawGrid;             // Controlador de los gizmos
    private const string CrossIcon = "cross.tiff";      // Dibujo de cruz.

    [SerializeField] [Range(0f,1f)]
    private float decayValue = 0.45f;                   // Factor de decaimiento de la influencia

    private MapType[] _mapTypes;                        // Tipos de mapas tácticos.
    private int _mapIndex;                              // Índice del mapa táctico mostrado.
    [SerializeField] 
    private MapType mapType = MapType.Influence;        // Mapa táctico actual.
    [SerializeField]
    private Text mapName;                              // Nombre del mapa táctico actual.

    /**
     * @return Malla del mapa.
     */
    public Grid Grid
    {
        get { return _grid; }
    }

    private void Awake()
    {
        // Obtemos el tamaño combinado de todas las celdas
        Bounds combinedBounds = new Bounds(floor.transform.position, Vector3.zero);
        Renderer[] renderers;
        Renderer r = floor.GetComponent<Renderer>();
        if (r != null)
        {
            renderers = new Renderer[] {r};
        }
        else
        {
            renderers = floor.GetComponentsInChildren<Renderer>();
        }

        foreach (Renderer render in renderers)
        {
            combinedBounds.Encapsulate(render.bounds);
        }

        Vector3 worldSize = combinedBounds.size;
        _grid = new Grid(new Vector2(worldSize.x, worldSize.z), floor.transform.position, floor.transform,
            influenceMap.transform);
        
        _mapTypes = Enum.GetValues(typeof(MapType)).Cast<MapType>().ToArray();

        _mapIndex = 0;
        mapType = _mapTypes[_mapIndex];
        ChangeMapName();
    }

    private void Start()
    {
        InvokeRepeating(nameof(DecayInfluence), 0f, 0.5f);
        InvokeRepeating(nameof(UpdateTeamsInfluence), 0f, 0.5f);
    }

    private void Update()
    {
        _grid.UpdateGridNodes();
    }

    private void DecayInfluence()
    {
        _grid.DecayInfluence(decayValue);
    }

    // Actualizamos la influencia de los equipos
    private void UpdateTeamsInfluence()
    {
        List<Teams> teamsList = Enum.GetValues(typeof(Teams)).Cast<Teams>().
            Where(t => !t.Equals(Teams.None)).ToList();

        Transform teamTransform;
        float[,] newInfluence;
        Vector2Int pos;

        foreach (Teams team in teamsList)
        {
            teamTransform = GameObject.Find(team.ToString()).transform;
            newInfluence = GetLastInfluence(team);

            foreach (Transform child in teamTransform)
            {
                if (!child.gameObject.activeSelf)
                {
                    continue;
                }

                pos = _grid.WorldToGridPoint(child.position);
                Vector2Int auxPos = new Vector2Int(pos.y, pos.x);
                ExtendInfluence(newInfluence, auxPos, TypeToInfluence(child.tag));
            }
        
            _grid.UpdateCellsInfluence(team, newInfluence, mapType);
        }
        
        // Actualizamos la influencia de las estructuras
        Building[] buildings = GameObject.Find("Buildings").GetComponentsInChildren<Building>();
        foreach (Building building in buildings)
        {
            if (building.Team.Equals(Teams.None))
            {
                continue;
            }

            newInfluence = GetLastInfluence(building.Team);
            foreach (Vector2Int buildingCell in building.Cells)
            {
                ExtendInfluence(newInfluence, buildingCell - _grid.Center, TypeToInfluence("Building"));
            }
            
            _grid.UpdateCellsInfluence(building.Team, newInfluence, mapType);
        }
    }
    
    // Influencia de un objeto.
    private float TypeToInfluence(string type)
    {
        switch (type)
        {
            case "Player":
            {
                return 0.75f;
            }
            case "Building":
            {
                return 0.25f;
            }
            case "NPC":
            {
                return 0.5f;
            }
            default:
            {
                return 0f;
            }
        }
    }

    // Extender influencia
    private void ExtendInfluence(float[,] newInfluence, Vector2Int position, float influence)
    {
        Vector2Int pos = position + _grid.Center;
        int x = pos.x;
        int y = pos.y;
        float distance;
        
        for (int i = 0; i < _grid.NumCellsX; i++)
        {
            for (int j = 0; j < _grid.NumCellsY; j++)
            {
                distance = Mathf.Max(Mathf.Abs(x - i), Mathf.Abs(y - j));
                newInfluence[i, j] += influence / Mathf.Pow(1f + distance, 1.25f);
            }
        }
    }

    // Obtener la influencia del equipo team en el cálculo anterior
    private float[,] GetLastInfluence(Teams team)
    {
        float[,] lastInfluence = new float[_grid.NumCellsX, _grid.NumCellsY];
        Vector2Int gridPos;

        for (int i = 0; i < _grid.NumCellsX; i++)
        {
            for (int j = 0; j < _grid.NumCellsY; j++)
            {
                gridPos = new Vector2Int(i, j);
                gridPos -= _grid.Center;
                lastInfluence[i, j] = _grid.GetInfluenceFromGridPoint(gridPos, team);
            }
        }

        return lastInfluence;
    }
    
    /**
     * @brief Intercambia el mapa táctico que se muestra.
     */
    public void SwitchInfluenceView()
    {
        _mapIndex = (_mapIndex + 1) % _mapTypes.Length;
        mapType = _mapTypes[_mapIndex];
        ChangeMapName();
    }
    
    // Cambia el nombre del mapa
    private void ChangeMapName()
    {
        string text = "";
        switch (mapType)
        {
            case MapType.Influence:
            {
                text = "Influencia";
                break;
            }
            case MapType.Vulnerability:
            {
                text = "Vulnerabilidad";
                break;
            }
            case MapType.Tension:
            {
                text = "Tensión";
                break;
            }
        }

        mapName.text = "MAPA ACTUAL: " + text;
    }
    
    private void OnDrawGizmos()
    {
        if (drawGrid && _grid != null)
        {
            Gizmos.color = Color.blue;
            Vector3 pos;
            Node n;
			
            for (int i = 0; i < _grid.NumCellsX; i++)
            {
                for (int j = 0; j < _grid.NumCellsY; j++)
                {
                    n = _grid.GetNode(i, j);
                    pos = n.WorldPosition;
                    pos.y = 0.5f;
                    Gizmos.DrawWireCube(pos, new Vector3(1f, 0f, 1f) * _grid.CellSize);
                    if (!n.Passable)
                    {
                        pos.y = 2f;
                        Gizmos.DrawIcon(pos, CrossIcon, true);
                    }
                }
            }
            
            if (_grid.Path != null)
            {
                foreach (Node node in _grid.Path)
                {
                    pos = node.WorldPosition;
                    pos.y = 0.5f;
                    Gizmos.color = Color.yellow;
                    Gizmos.DrawCube(pos, new Vector3(1f, 0f, 1f) * _grid.CellSize);
                    Gizmos.color = Color.blue;
                    Gizmos.DrawWireCube(pos, new Vector3(1f, 0f, 1f) * _grid.CellSize);
                    Gizmos.color = Color.yellow;
                }
            }
        }
    }
}
