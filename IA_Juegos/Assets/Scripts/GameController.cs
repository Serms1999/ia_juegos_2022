using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

/**
 * @class GameController
 * @brief Controlador del juego.
 */
public class GameController : MonoBehaviour
{
    private List<GameObject> _selectedPlayers;      // Lista de jugadores seleccionados.
    private WaypointsPath waypointsPath;                             // Camino de waypoints para PathFollowing
    private CollisionDetector _collisionDetector;   // Controlador de colisiones
    private List<FormationManager> _formations;     // Lista de formaciones
    private Canvas endGamePanel;                    // Pantalla de final de juego
    private bool activoEstados;
    private GameObject States = GameObject.Find("States");

    private void Awake()
    {
        // Fijamos los fps a 60
        Application.targetFrameRate = 60;
        States.SetActive(false);
        activoEstados = false;
        _selectedPlayers = new List<GameObject>();
        _collisionDetector = new CollisionDetector();
        _formations = new List<FormationManager>();
        endGamePanel = GameObject.Find("Final").GetComponent<Canvas>();
        endGamePanel.enabled = false;


        GameObject waypoints = GameObject.Find("Waypoints");
        if (waypoints != null)
        {
            waypointsPath = waypoints.GetComponent<WaypointsPath>();
        }
    }

    /**
     * @return Controlador de colisiones.
     */
    public CollisionDetector CollisionDetector
    {
        get { return _collisionDetector; }
    }
    
    /**
     * @brief Añade a un jugador a la lista de seleccionados.
     */
    public void AddSelectedPlayer(GameObject player)
    {
        _selectedPlayers.Add(player);
    }
    
    /**
     * @brief Quita a un jugador de la lista de seleccionados.
     */
    public void RemoveSelectedPlayer(GameObject player)
    {
        _selectedPlayers.Remove(player);
    }

    /**
     * @brief Devuelve una lista con los agentes vivos más cercanos a uno dado.
     * @pram[in] agent Agente que pide la lista.
     * @param[in] distance Distancia a tener en cuenta.
     * @return Lista con los agentes situados a una distancia ``distance`` del agente ``agent``.
     */
    public List<Agent> GetNearAgents(Agent agent, float distance)
    {
        List<Agent> allAgents = GameObject.FindGameObjectsWithTag("NPC").ToList()
            .Select(a => a.GetComponent<Agent>()).ToList();

        return new List<Agent>(allAgents.Where(a => !a.Equals(agent) && !a.Dead &&
                                                    Vector3.Distance(a.Position, agent.Position) <= distance)
            .OrderBy(a => Vector3.Distance(a.Position, agent.Position)));
    }

    /**
     * @brief Devuelve una lista con los enemigos más cercanos a uno dado.
     * @pram[in] agent Agente que pide la lista.
     * @param[in] distance Distancia a tener en cuenta.
     * @return Lista con los enemigos situados a una distancia ``distance`` del agente ``agent``.
     */
    public List<Agent> GetNearEnemies(Agent agent, float distance)
    {
        List<Agent> allAgents = GetNearAgents(agent, distance);
        return allAgents.Where(a => !a.Team.Equals(agent.Team)).ToList();
    }

    // Cambia el modo del equipo team
    protected void SwitchTeamMode(Teams team, State state)
    {
        List<AgentNPC> teamAgents = GameObject.FindGameObjectsWithTag("NPC").ToList()
            .Select(a => a.GetComponent<AgentNPC>())
            .Where(a => a.Team.Equals(team))
            .ToList();

        foreach (AgentNPC agent in teamAgents)
        {
            agent.State = state;
        }
    }

    /**
     * @brief Pone al equipo A en modo ataque.
     */
    public void AttackModeA()
    {
        SwitchTeamMode(Teams.TeamA, State.Attack);
    }
    
    /**
     * @brief Pone al equipo B en modo ataque.
     */
    public void AttackModeB()
    {
        SwitchTeamMode(Teams.TeamB, State.Attack);
    }
    
    /**
     * @brief Pone al equipo A en modo defensa.
     */
    public void DefenseModeA()
    {
        SwitchTeamMode(Teams.TeamA, State.Defense);
    }
    
    /**
     * @brief Pone al equipo B en modo defensa.
     */
    public void DefenseModeB()
    {
        SwitchTeamMode(Teams.TeamB, State.Defense);
    }

    /**
     * @brief Pone a todos los equipos en modo guerra total.
     */
    public void TotalWar()
    {
        SwitchTeamMode(Teams.TeamA, State.TotalWar);
        SwitchTeamMode(Teams.TeamB, State.TotalWar);
    }

    private void Update()
    {
        // Comprobamos que se ha hecho click
        if (Input.GetMouseButtonDown(0))
        {
            Ray mousePos = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hitInfo;
            AgentNPC agent;

            // Comprobamos que se ha clickado en el suelo
            if (Physics.Raycast(mousePos, out hitInfo) && hitInfo.collider != null)
            {
                if (hitInfo.collider.CompareTag("Floor"))
                {
                    foreach (GameObject npc in _selectedPlayers)
                    {
                        // Borramos los steerings anteriores que no nos interesen
                        agent = npc.GetComponent<AgentNPC>();
                        agent.RemoveAllSteeringsExcept(new List<string>()
                        {
                            SteeringNames.LookingWhereYoureGoing,
                            SteeringNames.WallAvoidance
                        });

                        if (agent.InFormation && !agent.AmILeader())
                        {
                            agent.AddSteering(SteeringNames.LeaderFollowing, null);
                            agent.AddSteering(SteeringNames.LookingWhereYoureGoing, null);
                            LeaderFollowing steering = agent.GetComponent<LeaderFollowing>();
                            steering.Leader = agent.Formation.Leader;
                            steering.ArriveWeight = 50f;
                            steering.FleeWeight = 10f;
                            steering.SeparationWeight = 50f;
                            steering.GameController = this;
                            steering.AuxLeaderDistance = 3f;
                        }
                        else
                        {
                            // Creamos un agente auxiliar
                            GameObject auxiliar = new GameObject("DetectPlayer");
                            Agent target = auxiliar.AddComponent<Agent>();
                            auxiliar.tag = "Auxiliar";

                            // Le asignamos la posicion deseada
                            target.Position = hitInfo.point;

                            // Añadimos el Seek hacia el agente auxiliar
                            agent.AddSteering(SteeringNames.Arrive, target);

                            if (agent.InFormation)
                            {
                                agent.Formation.NewPoint = target.Position;
                                agent.Formation.FormationDone = false;
                            }
                        }
                    }
                }
                else if (hitInfo.collider.CompareTag("NPC"))
                {
                    hitInfo.collider.gameObject.GetComponent<AgentNPC>().PlayerSelected();
                }
            }
        }

        
        if (Input.GetKeyDown(KeyCode.D))
        {
            // Deselecionamos a todos los personajes
            List<GameObject> agents = new List<GameObject>(_selectedPlayers);
            foreach (GameObject agent in agents)
            {
                agent.GetComponent<AgentNPC>().PlayerSelected();
            }
        }


        if (Input.GetKeyDown(KeyCode.F))
        {
            // Mandamos crear una formación en triángulo
            FormationManager formation = new FormationManager();
            formation.Pattern = new FormationTriangulo();
            AgentNPC agent;
            
            foreach (GameObject go in _selectedPlayers)
            {
                agent = go.GetComponent<AgentNPC>();
                agent.InFormation = formation.AddCharacter(agent);
            }
            formation.UpdateSlots();
            _formations.Add(formation);
        }
        if (Input.GetKeyDown(KeyCode.E))
        {
            if (activoEstados)
            {
                activoEstados = false;
                States.SetActive(activoEstados);
            }
            else
            {
                activoEstados = true;
                States.SetActive(activoEstados);
                

            }

        }
        if (Input.GetKeyDown(KeyCode.C))
        {
            // Mandamos crear una formación en columnas
            FormationManager formation = new FormationManager();
            formation.Pattern = new FormationColumnas();
            AgentNPC agent;
            
            foreach (GameObject go in _selectedPlayers)
            {
                agent = go.GetComponent<AgentNPC>();
                agent.InFormation = formation.AddCharacter(agent);
            }
            formation.UpdateSlots();
            _formations.Add(formation);
        }
        
        foreach (FormationManager formation in _formations)
        {
            if (formation.LeaderAtNewPoint())
            {
                formation.UnfollowLeader();
                formation.UpdateSlots();
            }
        }
    }

    /**
     * @brief Terminar el juego.
     */
    public void ExitGame()
    {
        Application.Quit(0);
    }

    /**
     * @brief Muestra una pantalla de final del juego cuando un equipo pierde.
     * @param[in] team Equipo perdedor.
     */
    public void EndGame(Teams team)
    {
        endGamePanel.enabled = true;
        Time.timeScale = 0f;

        switch (team)
        {
            case Teams.TeamA:
            {
                Text textField = endGamePanel.GetComponentInChildren<Text>();
                textField.text = "VICTORIA EQUIPO B";
                break;
            }
            case Teams.TeamB:
            {
                Text textField = endGamePanel.GetComponentInChildren<Text>();
                textField.text = "VICTORIA EQUIPO A";
                break;
            }
        }
    }
}
