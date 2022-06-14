using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using Unity.VisualScripting;
using UnityEngine;
using Object = System.Object;
using Random = System.Random;

/**
 * @class AgentNPC
 * @brief Representación de un agente controlado por la máquina.
 */
public class AgentNPC : Agent
{
    // Este será el steering final que se aplique al personaje.
    public Steering steer;
    private List<SteeringBehaviour> listSteerings;
    protected bool _selected;
    protected GameObject gameManager;
    protected Arbitro _arbitro;
    protected bool _inFormation;
    protected FormationManager _formation;
    protected float _baseDamage;
    [SerializeField]
	protected float _attackRange;
    protected float _attackSpeed;
    protected float _timerAttack;
	protected float _hpMax;
    [SerializeField]
    protected float _hpCurrent;
    protected float deathTimer = 5f;
    protected float healSpeed = 1f;
    protected float healTimer;
    protected float captureTimer;
    protected float captureSpeed = 2f;

    protected Grid grid;
    protected Building baseTeam;
    protected Building healingPoint;
    protected Building enemyBase;

    protected State _state;

    /**
     * @return Controlador de formaciones.
     */
    public FormationManager Formation
    {
        get { return _formation; }
        set { _formation = value; }
    }

    /**
     * @return Árbitro.
     */
    public Arbitro Arbitro
    {
        get { return _arbitro; }
        set { _arbitro = value; }
    }

    /**
     * @return Devuelve si el agente está en formación.
     */
    public bool InFormation
    {
        get { return _inFormation; }
        set { _inFormation = value; }
    }

    /**
     * @return Estado del agente.
     */
    public State State
    {
        get { return _state; }
        set { _state = value; }
    }

    /**
     * @brief Añade un comportamiento al agente
     * @param[in] steeringName Nombre del comportamiento
     * @param[in] target Objetivo del comportamiento.
     * @return true si se ha podido añadir el comportamiento.
     */
    public bool AddSteering(string steeringName, Agent target)
    {
        if (!SteeringNames.CheckSteeringName(steeringName))
        {
            return false;
        }
        
        SteeringBehaviour newSteering = (SteeringBehaviour) this.gameObject.AddComponent(Type.GetType(steeringName));
        newSteering.Weight = 1f;
        newSteering.Target = target;
        newSteering.enabled = true;
        listSteerings.Add(newSteering);
        return true;
    }
    
    /**
     * @brief Elimina un comportamiento.
     * @param[in] steeringName Comportamiento a eliminar.
     */
    public void RemoveSteering(string steeringName)
    {
        List<SteeringBehaviour> steering =
            listSteerings.Where(s => s.NameSteering.Equals(steeringName)).ToList();

        if (steering.Count > 0)
        {
            if (steering[0].Target != null && steering[0].Target.gameObject.CompareTag("Auxiliar"))
            {
                Destroy(steering[0].Target.gameObject);
            }
            Destroy(steering[0]);
        }
    }

    /**
     * @brief Elimina todos los comportamientos excepto unos dados.
     * @param[in] listSteering Comportamientos a eliminar.
     */
    public void RemoveAllSteeringsExcept(List<string> listSteering)
    {
        List<SteeringBehaviour> steeringToRemove = 
            listSteerings.Where(s => !listSteering.Contains(s.NameSteering)).ToList();

        foreach (SteeringBehaviour steering in steeringToRemove)
        {
            listSteerings.Remove(steering);
            if (steering.Target != null && steering.Target.gameObject.CompareTag("Auxiliar"))
            {
                Destroy(steering.Target.gameObject);
            }
            
            if (steering.name == SteeringNames.PathFindingA)
            {
                path = null;
            }
            Destroy(steering);
        }

        this.ResetMovement();
    }
    

    // Cambiar color del personaje cuando se selecciona
    protected void ChangeGameObjectMaterial()
    {
        Renderer[] r = GetComponentsInChildren<Renderer>();
        Color c;
        Material m;
        
        switch (_selected)
        {
            case true:
            {
                c = Color.blue;
                break;
            } 
            case false:
            {
                c = Color.red;
                break;
            }
        }

        foreach (Renderer rs in r)
        {
            m = rs.material;
            m.color = c;
            rs.material = m;
        }
    }


    /**
     * @brief Personaje seleccionado.
     */
    public void PlayerSelected()
    {
        _selected = !_selected;
        ChangeGameObjectMaterial();
        GameController controller = gameManager.GetComponent<GameController>();
        switch (_selected)
        {
            case true:
            {
                controller.AddSelectedPlayer(gameObject);
                break;
            }
            case false:
            {
                controller.RemoveSelectedPlayer(gameObject);
                break;
            }
        }
    }

    protected virtual void Awake()
    {
        this.steer = new Steering();
        _selected = false;

        // Construye una lista con todos las componenen del tipo SteeringBehaviour.
        // La llamaremos listSteerings
        // Usa GetComponents<>()
        listSteerings = new List<SteeringBehaviour>(GetComponents<SteeringBehaviour>());
        
        // Conseguimos al arbitro
        _arbitro = GetComponent<Arbitro>();
        
        // Inicializamos el equipo
        try
        {
            team = (Teams) Enum.Parse(typeof(Teams), this.transform.parent.name); 
        }
        catch (NullReferenceException)
        {
            team = Teams.None;
        }
        
        switch (team)
        {
            case Teams.TeamA:
            {
                baseTeam = GameObject.Find("BaseA").GetComponent<Building>();
                healingPoint = GameObject.Find("HealingPointA").GetComponent<Building>();
                enemyBase = GameObject.Find("BaseB").GetComponent<Building>();
                break;
            }
            case Teams.TeamB:
            {
                baseTeam = GameObject.Find("BaseB").GetComponent<Building>();
                healingPoint = GameObject.Find("HealingPointB").GetComponent<Building>();
                enemyBase = GameObject.Find("BaseA").GetComponent<Building>();
                break;
            }
        }
        
        gameManager = GameObject.Find("GameController");
        _timerAttack = _attackSpeed;
        healTimer = healSpeed;
        this._state = State.Attack;
    }

    // Use this for initialization
    protected void Start()
    {
        ResetMovement();
        grid = gameManager.GetComponent<GridController>().Grid;
        _state = State.Attack;
    }

    // Update is called once per frame
    public new void Update()
    {
        if (dead)
        {
            return;
        }
        ApplySteering();
    }

    /**
     * @brief Comprueba si un personaje es el líder de su formación.
     * @return true si es el líder.
     */
    public bool AmILeader()
    {
        return _formation.AmILeader(this);
    }
    
    protected void ApplySteering()
    {
        // Actualizar las propiedades para Time.deltaTime según NewtonEuler
        // La actualización de las propiedades se puede hacer en LateUpdate()
        // Evitamos que el steering modifique la componente Y
        steer.linear.y = 0f;
        
        // Velocity
        Velocity += steer.linear * GetVelocityFromTerrain() * Time.deltaTime;
        if (Velocity.magnitude > MaxSpeed)
        {
            Velocity = Velocity.normalized * MaxSpeed;
        }
        
        // Rotation
        Rotation += steer.angular * Time.deltaTime;
        if (Rotation > MaxRotation)
        {
            Rotation = MaxRotation;
        }
        
        // Position
        Position += Velocity * Time.deltaTime;
        // Orientation
        Orientation += Rotation * Time.deltaTime;

        // Aplicar las actualizaciones a la componente Transform
        transform.rotation = new Quaternion(); //Quaternion.identity;
        transform.Rotate(Vector3.up, Orientation);
    }

    protected TerrainType GetActualTerrain()
    {
        Vector2Int aux = grid.WorldToGridPoint(Position);
        Vector2Int gridPos = new Vector2Int(aux.y, aux.x);

        return grid.GridPointToNode(gridPos).TerrainType;
    }

    protected virtual float GetVelocityFromTerrain()
    {
        return 1f;
    }

    public virtual void LateUpdate()
    {
        if (dead)
        {
            deathTimer -= Time.deltaTime;
            if (deathTimer <= 0)
            {
                Revive();
                Debug.Log("Personaje resucitado");
            }
            return;
        }
        
        // Reseteamos el steering final.
        this.steer = new Steering();

        // Recorremos cada steering
        foreach (SteeringBehaviour behavior in listSteerings)
            GetSteering(behavior);
    }

    protected void GetSteering(SteeringBehaviour behavior)
    {
        // Calcula el steeringbehaviour
        Steering kinematic = behavior.GetSteering(this);

        // Usar kinematic con el árbitro desesado para combinar todos los SteeringBehaviour.
        // Llamaremos kinematicFinal a la aceleraciones finales.
        Steering kinematicFinal = Arbitro.GetSteering(behavior,kinematic);

        // El resultado final se guarda para ser aplicado en el siguiente frame.
        this.steer += kinematicFinal;
    }
    
    // Moverse hacia una estructura.
    protected void MoveToTarget(Building building)
    {
        this.RemoveAllSteeringsExcept(new List<string>()
        {
            SteeringNames.LookingWhereYoureGoing
        });
        
        PathFindingA steering = gameObject.AddComponent<PathFindingA>();
        Vector2Int end = new Vector2Int(building.Center.y, building.Center.x);
        steering.StartNode = grid.WorldPointToNode(Position);
        steering.EndNode = grid.GridPointToNode(end - grid.Center);
        steering.DistanceMethod = DistanceMethods.Chebychev;
        steering.Weight = 5f;
        steering.enabled = true;
        listSteerings.Add(steering);
    }

    // Matar al personaje.
    protected void DeadNPC()
    {
        this.gameObject.GetComponent<UniBT.BehaviorTree>().enabled = false;
        this.RemoveAllSteeringsExcept(new List<string>()
        {
            SteeringNames.LookingWhereYoureGoing
        });
        
        Vector3 pos = Position;
        pos.y = -5;
        Position = pos;
        path = null;
        dead = true;
    }

    // Revivir al personaje.
    protected void Revive()
    {
        dead = false;
        if (baseTeam == null)
        {
            return;
        }

        Vector2Int gridPos = baseTeam.Center - grid.Center;
        Position = grid.GridToWorldPoint(gridPos.y, gridPos.x);
        _hpCurrent = _hpMax;
        deathTimer = 5f;
        this.gameObject.GetComponent<UniBT.BehaviorTree>().enabled = true;
    }

    // Recibir daño.
    protected void GetDamage(float damage)
    {
        _hpCurrent -= damage;
        if (_hpCurrent <= 0)
        {
            Debug.Log("Personaje muerto");
            DeadNPC();
        }
    }

    /**
     * @brief Atacar a un enemigo.
     * @param[in] enemy Enemigo.
     */
    public void AttackEnemy(AgentNPC enemy)
    {
        if (Mathf.Approximately(_timerAttack, _attackSpeed))
        {
            this.RemoveAllSteeringsExcept(new List<string>()
            {
                SteeringNames.LookingWhereYoureGoing
            });
            
            Random random = new Random();
            float damage = ((float) random.NextDouble() * (1f - 0.8f) + 0.8f) * _baseDamage;
        
            enemy.GetDamage(damage);
            _timerAttack -= Time.deltaTime;
        }
        else if (_timerAttack > 0 && _timerAttack < _attackSpeed)
        {
            _timerAttack -= Time.deltaTime;
        }
        else
        {
            _timerAttack = _attackSpeed;
        }
    }

    // Obtenemos una lista de enemigos a una distancia a la que podemos atacar.
    protected List<Agent> CheckNearEnemies()
    {
        GameController controller = gameManager.GetComponent<GameController>();

        return controller.GetNearEnemies(this, 14f);
    }

    /**
     * @brief Comprueba si hay enemigos cerca del agente.
     */
    public bool IsNearEnemy()
    {
        return CheckNearEnemies().Count > 0;
    }

    /**
     * @brief Obtiene al enemigo más cercano.
     * @return Enemigo más cercano.
     */
    public AgentNPC GetNearestEnemy()
    {
        return (AgentNPC) CheckNearEnemies().First();
    }

    /**
     * @brief Volver a defender la base.
     */
    public void Defend()
    {
        MoveToTarget(baseTeam);
    }
    
    /**
     * @brief Ir a capturar la base enemiga.
     */
    public void GoToEnemyBase()
    {
        MoveToTarget(enemyBase);
    }

    // Comprueba si el personaje está en la estructura.
    protected bool OnBuilding(Building building)
    {
        List<Vector2Int> buildingNodes = building.Cells;
        
        Vector2Int aux = grid.WorldToGridPoint(Position) + grid.Center;
        Vector2Int gridPos = new Vector2Int(aux.y, aux.x);
        
        return buildingNodes.Contains(gridPos);
    }

    /**
     * @brief Comprueba si el agente está en su punto de curación.
     * @return true si está en el punto de curación.
     */
    public bool OnHealingPoint()
    {
        return OnBuilding(healingPoint);
    }
    
    /**
     * @brief Comprueba si el agente está en su base.
     * @return true si está en su base.
     */
    public bool OnTeamBase()
    {
        return OnBuilding(baseTeam);
    }
    
    /**
     * @brief Comprueba si el agente está en la base enemiga.
     * @return true si está en la base enemiga.
     */
    public bool OnEnemyBase()
    {
        return OnBuilding(enemyBase);
    }

    /**
     * @brief Se mueve al punto de curación.
     */
    public void GoHealing()
    {
        MoveToTarget(healingPoint);
    }

    /**
     * @brief El personaje recibe vida cada cierto tiempo.
     */
    public void Heal()
    {
        if (_hpCurrent >= _hpMax)
        {
            return;
        }

        if (Mathf.Approximately(healTimer, healSpeed))
        {
            this.RemoveAllSteeringsExcept(new List<string>()
            {
                SteeringNames.LookingWhereYoureGoing
            });
            
            _hpCurrent = Mathf.Min(_hpCurrent + 10f, _hpMax);
            healTimer -= Time.deltaTime;
        }
        else if (healTimer > 0f && healTimer < healSpeed)
        {
            healTimer -= Time.deltaTime;
        }
        else
        {
            healTimer = healSpeed;
        }
    }

    /**
     * @brief Comprueba si el agente tiene poca vida.
     * @return true si los puntos de vida se han reducido a menos de la mitad.
     */
    public bool LowHP()
    {
        return _hpCurrent < _hpMax / 2f;
    }
    
    /**
     * @brief Comprueba si el agente tiene la vida al completo.
     * @return true si los puntos de vida tiene valor máximo.
     */
    public bool FullHP()
    {
        return Mathf.Approximately(_hpCurrent, _hpMax);
    }

    
    // Comprueba si el agente se está moviendo hacia una estructura.
    protected bool GoingToBuilding(Building building)
    {
        if (path == null || path.Count==0)
        {
            return false;
        }

        Vector2Int n = path.Last().GridPosition;
        Vector2Int aux = building.Center - grid.Center;

        return n.Equals(new Vector2Int(aux.y, aux.x));
    }

    /**
     * @brief Comprueba si el agente está moviendose a su punto de curación.
     * @return true si está moviendose a el punto de curación.
     */
    public bool GoingToHealingPoint()
    {
        return GoingToBuilding(healingPoint);
    }
    
    /**
     * @brief Comprueba si el agente está moviendose hacia su base.
     * @return true si está moviendose hacia su base.
     */
    public bool GoingToTeamBase()
    {
        return GoingToBuilding(baseTeam);
    }
    
    /**
     * @brief Comprueba si el agente está moviendose a la base enemiga.
     * @return true si está moviendose hacia la base enemiga.
     */
    public bool GoingToEnemyBase()
    {
        return GoingToBuilding(enemyBase);
    }
    
    // Comprueba si el personaje está lejos de una estructura
    protected bool FarFromBuilding(Building building)
    {
        Node n1 = grid.WorldPointToNode(Position);
        Vector2Int buildingCenter = building.Center - grid.Center;
        Node n2 = grid.GridPointToNode(new Vector2Int(buildingCenter.y, buildingCenter.x));

        int dx = Mathf.Abs(n1.GridPosition.x - n2.GridPosition.x);
        int dy = Mathf.Abs(n1.GridPosition.y - n2.GridPosition.y);

        return Mathf.Max(dx, dy) > 30f;
    }
    
    // Comprueba si el personaje está cerca de una estructura
    protected bool NearToBuilding(Building building)
    {
        Node n1 = grid.WorldPointToNode(Position);
        Vector2Int buildingCenter = building.Center - grid.Center;
        Node n2 = grid.GridPointToNode(new Vector2Int(buildingCenter.y, buildingCenter.x));

        int dx = Mathf.Abs(n1.GridPosition.x - n2.GridPosition.x);
        int dy = Mathf.Abs(n1.GridPosition.y - n2.GridPosition.y);

        return Mathf.Max(dx, dy) < 10f;
    }

    /**
     * @brief Comprueba si el agente está lejos de su base.
     * @return true si está lejos.
     */
    public bool FarFromTeamBase()
    {
        return FarFromBuilding(baseTeam);
    }

    /**
     * @brief Comprueba si el agente está cerca de su base.
     * @return true si está cerca.
     */
    public bool NearToTeamBase()
    {
        return NearToBuilding(baseTeam);
    }
    
    /**
     * @brief Comprueba si el agente está lejos de la base enemiga.
     * @return true si está lejos.
     */
    public bool FarFromEnemyBase()
    {
        return FarFromBuilding(enemyBase);
    }
    
    /**
     * @brief Comprueba si el agente está cerca de la base enemiga.
     * @return true si está cerca.
     */
    public bool NearToEnemyBase()
    {
        return NearToBuilding(enemyBase);
    }

    
    /**
     * @brief El agente aplica una cantidad de puntos de captura cada cierto tiempo a la base enemiga.
     */
    public void CaptureEnemyBase()
    {
        if (Mathf.Approximately(captureTimer, captureSpeed))
        {
            this.RemoveAllSteeringsExcept(new List<string>()
            {
                SteeringNames.LookingWhereYoureGoing
            });
            
            enemyBase.GetCapturePoints(5);
            captureTimer -= Time.deltaTime;
        }
        else if (captureTimer > 0f && captureTimer < captureSpeed)
        {
            captureTimer -= Time.deltaTime;
        }
        else
        {
            captureTimer = captureSpeed;
        }
    }
}
