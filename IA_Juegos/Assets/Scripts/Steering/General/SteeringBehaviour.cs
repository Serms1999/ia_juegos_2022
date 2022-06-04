using UnityEngine;

/**
 * @class SteeringBehaviour
 * @brief Representación de un comportamiento.
 */
[RequireComponent(typeof(AgentNPC))]
public class SteeringBehaviour : MonoBehaviour
{
    
    protected string nameSteering = "no steering";  // Nombre del comportamiento.
    [SerializeField]
    protected Agent target;                         // Objetivo.
    [SerializeField]
    protected float weight;                         // Peso del comportamiento.

    /**
     * @return Objetivo.
     */
    public Agent Target
    {
        get { return target; }
        set { target = value; }
    }

    /**
     * @return Peso del comportamiento.
     */
    public float Weight
    {
        get { return weight; }
        set { weight = value; }
    }

    /**
     * @return Nombre del comportamiento.
     */
    public string NameSteering
    {
        set { nameSteering = value; }
        get { return nameSteering; }
    }

    /**
     * @brief Calcula el comportamiento.
     * @param[in] agent Agente al que se le aplica el comportamiento.
     * @return Steering con las aceleraciones del comportamiento.
     */
    public virtual Steering GetSteering(Agent agent)
    {
        return null;
    }
}
