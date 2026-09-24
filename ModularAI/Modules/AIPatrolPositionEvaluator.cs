using UnityEngine;

public class AIPatrolPositionEvaluator : AIModule
{
    // TODO: REMOVE THIS AND CHANGE FOR BELOW
    [SerializeField] Transform[] possiblePatrolPositions;
    // TODO: ADAPT TO DIFFERENT CHARACTER TRAITS, ETC!!!
    public Vector3 GetRandomPatrolPosition()
    {
        return possiblePatrolPositions[Random.Range(0, possiblePatrolPositions.Length)].position;
    }
}