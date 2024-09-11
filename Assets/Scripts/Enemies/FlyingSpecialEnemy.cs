using UnityEngine;
using UnityEngine.AI;

public class FlyingSpecialEnemy : EnemiesScript
{
    public override void AttackPlayer()
    {

        //base.AttackPlayer();
        animator.SetBool("Walking", false);
        //Make sure enemy doesn't move
        agent.SetDestination(transform.position);

        if (!alreadyAttacked)
        {

           
                animator.SetTrigger("Attack");
                //Make sure enemy doesn't move

                transform.LookAt(player);
                
                ///Attack code here
                Rigidbody rb = Instantiate(projectile, shootPoint.position, Quaternion.identity).GetComponent<Rigidbody>();
                rb.AddForce(transform.forward * fowardForce, ForceMode.Impulse);
                rb.AddForce(transform.up * upwardForce, ForceMode.Impulse);
                ///End of attack code
                
                alreadyAttacked = true;
                Invoke(nameof(ResetAttack), timeBetweenAttacks);
            
        }
    }


}
