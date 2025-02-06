using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackState : BaseState
{

    private float moveTimer;
    private float losePlayerTimer;
    private float shotTimer;



    public override void Enter()
    {
        
    }

    public override void Exit()
    {
        
    }

    public override void Perform()
    {
        if (enemy.CanSeePlayer()) // Player is seen
        {
            losePlayerTimer = 0; // Reset lose player timer
            moveTimer += Time.deltaTime;
            shotTimer += Time.deltaTime;

            // Face the player
            Vector3 lookDirection = (enemy.Player.transform.position - enemy.transform.position).normalized;
            Quaternion lookRotation = Quaternion.LookRotation(lookDirection);
            enemy.transform.rotation = Quaternion.Slerp(enemy.transform.rotation, lookRotation, Time.deltaTime * 5f);

            // Shoot at the player
            if (shotTimer > enemy.fireRate)
            {
                Shoot();
            }

            // Random movement to make the enemy dynamic
            if (moveTimer > Random.Range(3, 7))
            {
                enemy.Agent.SetDestination(enemy.transform.position + (Random.insideUnitSphere * 5));
                moveTimer = 0;
            }
            enemy.LastKnownPos = enemy.Player.transform.position;
        }
        else
        {
            losePlayerTimer += Time.deltaTime;

            if (losePlayerTimer > 1) // Grace period before transitioning states
            {
                stateMachine.ChangeState(new SearchState());
            }
        }
    }

    public void Shoot()
    {
        if (!enemy.isShooting) // Prevent overlapping bursts
        {
            enemy.StartCoroutine(BurstFire());
        }
    }

    private IEnumerator BurstFire()
    {
        enemy.isShooting = true; // Prevent re-triggering
        int xcount = Random.Range(1, 6);
        for (int i = 0; i < xcount; i++) // Fire 3 bullets
        {
            FireBullet();
            enemy.gunShot.Play(); // Play gunshot sound
            yield return new WaitForSeconds(0.08f); // Small delay between shots
        }

        yield return new WaitForSeconds(2f); // 2-second cooldown before next burst
        enemy.isShooting = false; // Allow shooting again
    }

    private void FireBullet()
    {
        Transform gunbarrel = enemy.gunBarrel;
        GameObject bullet = GameObject.Instantiate(Resources.Load("Prefabs/Bullet") as GameObject, gunbarrel.position, enemy.transform.rotation);
        Vector3 shootDirection = (enemy.Player.transform.position - gunbarrel.position).normalized;
        bullet.GetComponent<Rigidbody>().velocity = shootDirection * 40;
        Debug.Log("Shoot!!!!");
    }
}
