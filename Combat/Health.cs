using System;
using UnityEngine;
using Mirror;

public class Health : NetworkBehaviour
{
    [SerializeField] private int maxHealth = 100;
    
    [SyncVar(hook = nameof(HandleHealthUpdated))]    
    private int currentHealth;
    
    public event Action ServerOnDie;
    public event Action<int, int> ClientOnHealthUpdated;
    
    #region Server
    public override void OnStartServer()
    {
        currentHealth = maxHealth;
        UnitBase.ServerOnPlayerDie += ServerHandlePlayerDie;        
    }
    public override void OnStopServer()
    {
        UnitBase.ServerOnPlayerDie -= ServerHandlePlayerDie;
    }
    [Server]
    private void ServerHandlePlayerDie(int connectionId)
    {
        if (connectionToClient.connectionId != connectionId) { return; }
        DealDamage(currentHealth);
    }
    [Server]
    public void DealDamage(int damageAmount)
    {
        //If its already dead, don't do anything
        if (currentHealth == 0) { return; }
        //Then calculate the new health
        currentHealth = Mathf.Max(currentHealth - damageAmount, 0);
        //If they are now !Dead then return
        if (currentHealth != 0) { return; }
        //But if they are dead, tell the server
        ServerOnDie?.Invoke();
    }
    #endregion
    
    #region Client
    private void HandleHealthUpdated(int oldHealth, int newHealth)
    {
        ClientOnHealthUpdated?.Invoke(newHealth, maxHealth);
    }
    #endregion
}