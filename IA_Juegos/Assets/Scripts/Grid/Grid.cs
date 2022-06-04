using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Vector2 = UnityEngine.Vector2;
using Vector3 = UnityEngine.Vector3;

/**
 * @class Grid
 * @brief Malla cuadrada con la que se divide el terreno.
 */

public class Grid
{
    private Node[,] _nodes;         // Matriz de nodos de la malla
    private int _numCellsX;         // Número de filas de la malla
    private int _numCellsY;         // Número de columnas de la malla
    private int _cellSize;          // Tamaño de cada celda de la malla
    private Vector2Int _center;     // Casilla central de la malla
    private Vector3 _worldLocation; // Posición de la malla en el espacio
    private List<Node> _path;       // Camino generado por el algoritmo LRTA*
    private List<Node> _localSpace; // Subespacio de búsqueda para el algoritmo LRTA*

    /**
     * @return Número de filas de la malla. 
     */
    public int NumCellsX
    {
        get { return _numCellsX; }
    }

    /**
     * @return Número de columnas de la malla. 
     */
    public int NumCellsY
    {
        get { return _numCellsY; }
    }

    /**
     * @return Tamaño de cada celda de la malla.
     */
    public int CellSize
    {
        get { return _cellSize; }
    }

    /**
     * @return Casilla central de la malla.
     */
    public Vector2Int Center
    {
        get { return _center; }
    }

    /**
     * @return Camino generado por el algoritmo LRTA*.
     */
    public List<Node> Path
    {
        get { return _path; }
        set { _path = value; }
    }
    
    /**
     * @return Subespacio de búsqueda para el algoritmo LRTA*.
     */
    public List<Node> LocalSpace
    {
        get { return _localSpace; }
        set { _localSpace = value; }
    }

    /**
     * @brief Constructor de la clase Grid.
     * @details Se construye la malla como una matriz cuadrada de celdas.
     */
    public Grid(Vector2 worldSize, Vector3 location, Transform floor, Transform influenceMap)
    {
        _cellSize = CalculateCellSize((int) worldSize.x, (int) worldSize.y);
        _numCellsX = (int) worldSize.x / _cellSize;
        _numCellsY = (int) worldSize.y / _cellSize;
        _nodes = new Node[_numCellsX, _numCellsY];
        _center = new Vector2Int((int) Mathf.Floor(_numCellsX / 2f),
            (int) Mathf.Floor(_numCellsY / 2f));
        _worldLocation = location;
        Node node;
        Vector2Int gridPosition;
        _path = new List<Node>();

        Transform[] currentRow = new Transform[2];
        Transform[] currentCell = new Transform[2];

        for (int i = 0; i < _numCellsX; i++)
        {
            for (int j = 0; j < _numCellsY; j++)
            {
                gridPosition = new Vector2Int(i, j) - _center;
                currentRow[0] = floor.GetChild(j);
                currentRow[1] = influenceMap.GetChild(i);
                currentCell[0] = currentRow[0].GetChild(i);
                currentCell[1] = currentRow[1].GetChild(j);
                node = new Node(GridToWorldPoint(gridPosition.x, gridPosition.y), gridPosition, _cellSize,
                    currentCell[0].gameObject, currentCell[1].gameObject);
                _nodes[i, j] = node;
            }
        }
    }

    /**
     * @brief Devuelve la celda en la posición ``(x,y)``.
     * @param[in] x Coordenada horizontal de la celda.
     * @param[in] y Coordenada vertical de la celda.
     * @return Celda en la posición ``(x,y)``$.
     */
    public Node GetNode(int x, int y)
    {
        if (x < 0 || x >= _numCellsX || y < 0 || y >= _numCellsY)
        {
            return null;
        }

        return _nodes[x, y];
    }

    /**
     * @brief Convierte una posición en la malla en una coordena del mundo.
     * @param[in] x Coordenada horizontal de la celda.
     * @param[in] y Coordenada vertical de la celda.
     * @return Posición en el mundo de esa celda.
     */
    public Vector3 GridToWorldPoint(int x, int y)
    {
        Vector3 hor = Vector3.right * (x * _cellSize);
        Vector3 ver = Vector3.forward * (y * _cellSize);

        // Desplazamos las celdas en función de si son impares o pares
        hor += Vector3.right * (0.5f * ((_numCellsX + 1) % 2) * _cellSize);
        ver += Vector3.forward * (0.5f * ((_numCellsY + 1) % 2) * _cellSize);
        
        return hor + ver + _worldLocation;
    }

    /**
     * @brief Convierte una coordenada del mundo en una posición de la malla.
     * @param[in] position Coordenada en el mundo.
     * @return Posición en la malla de esa coordenada.
     */
    public Vector2Int WorldToGridPoint(Vector3 position)
    {
        Vector3 pos = position - _worldLocation;
        return new Vector2Int((int) Mathf.Floor(pos.x / _cellSize), (int) Mathf.Floor(pos.z / _cellSize));
    }
    
    /**
     * @brief Devuelve la celda asociada a una posición de la malla.
     * @param[in] point Posición de la celda en la malla.
     * @return Celda de la malla en la posición dada.
     */
    public Node GridPointToNode(Vector2Int point)
    {
        return _nodes[point.x + _center.x, point.y + _center.y];
    }

    /**
     * @brief Devuelve la celda de la malla dada una coordenada del mundo.
     * @param[in] position Coordenada en el mundo.
     * @return Celda de la malla en la coordenada dada.
     * @see GridPointToNode(Vector2Int)
     * @see WorldToGridPoint(Vector3)
     */
    public Node WorldPointToNode(Vector3 position)
    {
        return GridPointToNode(WorldToGridPoint(position));
    }

    /**
     * @brief Actualiza el estado de una celda a pasable o no pasable en función de si hay un muro en ella.
     */
    public void UpdateGridNodes()
    {
        Collider[] collisions;
        Node node;

        for (int i = 0; i < _numCellsX; i++)
        {
            for (int j = 0; j < _numCellsY; j++)
            {
                node = _nodes[i, j];
                collisions = Physics.OverlapBox(node.WorldPosition, Vector3.one * _cellSize / 2f);
                if (collisions.Length == 0 || collisions.Any(n => n.CompareTag("Wall")))
                {
                    node.Passable = false;
                    return;
                }

                node.Passable = true;
            }
        }
    }
    
    // Calcula el tamaño de la celda para la malla.
    private int CalculateCellSize(int width, int height)
    {
        // Calculamos los divisores de cada uno
        List<int> divisorsA = CalculateDivisors(width);
        List<int> divisorsB = CalculateDivisors(height);

        // Calculamos los divisores comunes
        List<int> commonDivisors = divisorsA.Intersect(divisorsB).ToList();

        if (commonDivisors.Count == 1)
        {
            return 1;
        }
        return commonDivisors.ElementAt(1);
    }
    
    // Calcula Calcula los divisores del número ``n``.
    private List<int> CalculateDivisors(int n)
    {
        if (n <= 0)
        {
            return null;
        }
        List<int> divisors = new List<int>();
        for (int i = 1; i <= Mathf.Sqrt(n); i++)
        {
            if (n % i == 0)
            {
                divisors.Add(i);
                if (i != n / i)
                {
                    divisors.Add(n / i);
                }
            }
        }
        divisors.Sort();
        return new List<int>(divisors);
    }

    /**
     * @brief Comprueba que dos celdas estén unidas.
     * @details Se comprueba si están al lado y que no estén en diagonal.
     * @param[in] n1 Celda 1.
     * @param[in] n2 Celda 2.
     * @return true si están al lado y no están en diagonal.
     */
    public bool ConnectedNodes(Node n1, Node n2)
    {
        int dX = Mathf.Abs(n1.GridPosition.x - n2.GridPosition.x);
        int dY = Mathf.Abs(n1.GridPosition.y - n2.GridPosition.y);

        return dX <= 1 && dY <= 1 && dX != dY;
    }

    /**
     * @brief Calcula las celdas vecinas de otra dada.
     * @param[in] node Celda.
     * @return Lista de celdas vecinas de node.
     */
    public List<Node> GetNeighbours(Node node)
    {
        Node n;
        List<Node> neighbours = new List<Node>();

        for (int i = -1; i <= 1; i++)
        {
            for (int j = -1; j <= 1; j++)
            {
                try
                {
                    n = GridPointToNode(node.GridPosition + new Vector2Int(i, j));
                }
                catch (IndexOutOfRangeException)
                {
                    n = null;
                }
                
                if (n != null && ConnectedNodes(node, n) && n.Passable)
                {
                    neighbours.Add(n);
                }
            }
        }

        return new List<Node>(neighbours);
    }

    /**
     * @brief Devuelve el valor de influencia de una celda.
     * @param[in] gridPos Posición de la celda.
     * @param[in, opt] team Equipo sobre el que se busca la influencia. Default None.
     * @return Influencia general de la celda en la posición gridPos o sólo la influencia del equipo
     * team en caso de no ser None.
     */
    public float GetInfluenceFromGridPoint(Vector2Int gridPos, Teams team = Teams.None)
    {
        Node n = GridPointToNode(gridPos);
        if (!team.Equals(Teams.None))
        {
            return n.TeamInfluence(team);
        }

        return n.InfluenceValue;
    }

    /**
     * @brief Baja la influencia de las celdas en un factor decayValue.
     */
    public void DecayInfluence(float decayValue)
    {
        foreach (Node node in _nodes)
        {
            node.DecayInfluence(decayValue);
        }
    }

    /**
     * @brief Actualiza la influencia de las celdas de la malla.
     * @param[in] team Equipo sobre el que se actualiza la influencia.
     * @param[in] newInfluence Nuevos valores de influencia.
     * @param[in] mapType Mapa sobre el que se actualiza la influencia.
     */
    public void UpdateCellsInfluence(Teams team, float[,] newInfluence, MapType mapType)
    {
        Vector2Int gridPos;
        foreach (Node node in _nodes)
        {
            gridPos = node.GridPosition;
            gridPos += Center;
            node.UpdateInfluence(team, newInfluence[gridPos.x, gridPos.y], mapType);
        }
    }
}
