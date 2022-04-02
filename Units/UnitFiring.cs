using UnityEngine;
using Mirror;

public class UnitFiring : NetworkBehaviour
{
    [SerializeField] private Targeter targeter = null;
    [SerializeField] private GameObject projectilePrefab = null;
    [SerializeField] private Transform projectileSpawnPoint = null;
    [SerializeField] private float fireRange = 5f;
    [SerializeField] private float fireRate = 1f;
    [SerializeField] private float rotSpeed = 20f;
    
    private float lastFireTime;
    
    [ServerCallback]
    private void Update()
    {
        Targetable target = targeter.GetTarget();
        
        if (target == null) { return; }
        
        if (!CanFireAtTarget()) { return; }
        Quaternion targetRotation =
            Quaternion.LookRotation(target.transform.position - transform.position);
            
        transform.rotation = Quaternion.RotateTowards(
            transform.rotation, targetRotation, rotSpeed * Time.deltaTime);
            
        if (Time.time > (1 / fireRate) + lastFireTime)
        {
            Quaternion projectileRot = Quaternion.LookRotation(
                target.GetAimAtPoint().position - projectileSpawnPoint.position);
                
            GameObject projectileInstance = Instantiate(
                projectilePrefab, projectileSpawnPoint.position, projectileRot);
                
            //Check who has ownership of the projectile
            NetworkServer.Spawn(projectileInstance, connectionToClient);
            
            lastFireTime = Time.time;
        }
    }
    [Server]
    private bool CanFireAtTarget()
    {
        return (targeter.GetTarget().transform.position - transform.position).sqrMagnitude
            <= fireRange * fireRange;
    }
}