using UnityEngine;
using Mirror;

public class UnitProjectile : NetworkBehaviour
{
    [SerializeField] private Rigidbody rb = null;
    [SerializeField] private int damageToDeal = 20;
    [SerializeField] private float destroyAfterSeconds = 5f;
    [SerializeField] private float launchForce = 10f;
    
    private void Start()
    {
        rb.velocity = transform.forward * launchForce;        
    }
    public override void OnStartServer()
    {
        Invoke(nameof(DestroySelf), destroyAfterSeconds);
    }
    [ServerCallback]
    private void OnTriggerEnter(Collider other)
    {
        //Don't do anything if you hit your own building or units
        if (other.TryGetComponent<NetworkIdentity>(out NetworkIdentity networkIdentity))
        {
            if (networkIdentity.connectionToClient == connectionToClient) { return; }           
        }
        //If you hit someone elses units or buildings; DealDamage();
        if (other.TryGetComponent<Health>(out Health health))
        {
            health.DealDamage(damageToDeal);            
        }
        //Whenever you hit anything that does not belong to anyone, wall, terrain or whatever
        DestroySelf();
    }
    
    [Server]
    private void DestroySelf()
    {
        NetworkServer.Destroy(gameObject);
    }
}