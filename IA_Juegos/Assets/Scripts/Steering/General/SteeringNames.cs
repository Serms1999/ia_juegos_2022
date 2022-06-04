using System.Collections.Generic;

/**
 * @class SteeringNames
 * @brief Clase que contiene los nombres de todos los comportamientos.
 */
public static class SteeringNames
{
    public const string Align = "Align";
    public const string AntiAlign = "Anti Align";
    public const string Arrive = "Arrive";
    public const string Face = "Face";
    public const string Flee = "Flee";
    public const string LeaderFollowing = "LeaderFollowing";
    public const string LookingWhereYoureGoing = "LookingWhereYoureGoing";
    public const string PathFindingLrta = "PathFindingLRTA";
    public const string PathFindingA = "PathFindingA";
    public const string PathFollowing = "PathFollowing";
    public const string Pursue = "Pursue";
    public const string Seek = "Seek";
    public const string Separation = "Separation";
    public const string VelocityMatching = "VelocityMatching";
    public const string WallAvoidance = "WallAvoidance";
    public const string Wander = "Wander";

    private static List<string> _allSteerings = new List<string>()
    {
        Align, AntiAlign, Arrive, Face, Flee, LeaderFollowing, LookingWhereYoureGoing, PathFindingLrta,
        PathFindingA, PathFollowing, Pursue, Seek, Separation, VelocityMatching, WallAvoidance, Wander
    };

    /**
     * @brief Comprueba si un comportamiento existe.
     * @param[in] steeringName Nombre del comportamiento a comprobar.
     * @return true si el comportamiento existe.
     */
    public static bool CheckSteeringName(string steeringName)
    {
        return _allSteerings.Contains(steeringName);
    }
}