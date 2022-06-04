public class Heuristic
{
    private Node _goalNode;
    private Distance _distance;

    public Node GoalNode
    {
        get { return _goalNode; }
    }

    public Distance Distance
    {
        get { return _distance; }
    }

    public Heuristic(Node goal, DistanceMethods d)
    {
        _goalNode = goal;
        switch (d)
        {
            case DistanceMethods.Chebychev:
            {
                _distance = new Chebychev();
                break;
            }
            case DistanceMethods.Euclidea:
            {
                _distance = new Euclidea();
                break;
            }
            case DistanceMethods.Manhattan:
            {
                _distance = new Manhattan();
                break;
            }
        }
    }

    public float EstimateCost(Node node)
    {
        return _distance.GetDistance(node, _goalNode);
    }
    
    public float EstimateCost(Node node, Node goal)
    {
        return _distance.GetDistance(node, goal);
    }
}
