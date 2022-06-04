/**
 * @class Árbitro
 * @brief Árbrito que pondera los steerings a aplicar.
 */
public class ArbitroPonderado : Arbitro
{
    /**
     * @brief Devuelve el steering ponderado.
     * @param[in] behaviour Comportamiento.
     * @param[in] steering Steering a ponderar.
     * @return Steering ponderado.
     */
    public override Steering GetSteering(SteeringBehaviour behaviour, Steering steering)
    {
        Steering steer = new Steering();
        
        // Ponderamos los valores del steering
        steer.linear = behaviour.Weight * steering.linear;
        steer.angular = behaviour.Weight * steering.angular;
        
        // Devolvemos el resultado
        return steer;
    }
}
