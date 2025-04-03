using UnityEngine;

public class SearchState : BaseState
{
    private float searchTimer;
    private float searchDuration = 5f; // Time to look around

    public override void Enter()
    {
        enemy.Agent.SetDestination(enemy.LastKnownPos);
        searchTimer = 0;
    }

    public override void Perform()
    {
        enemy.DetectPlayerActions(); // Always listen for gunshots/running

        if (enemy.CanSeePlayer())
        {
            enemy.LastKnownPos = enemy.Player.transform.position; // Update to player's current location
            enemy.Agent.SetDestination(enemy.LastKnownPos);
            stateMachine.ChangeState(new AttackState());
        }
        else if (enemy.Agent.remainingDistance < 0.2f)
        {
            searchTimer += Time.deltaTime;
            if (searchTimer >= searchDuration)
            {
                stateMachine.ChangeState(new PatrolState());
            }
        }
    }

    public override void Exit()
    {
    }
}