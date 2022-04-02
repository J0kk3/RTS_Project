using UnityEngine;
using Mirror;

public class Targeter : NetworkBehaviour
{
    private Targetable target;
    
    public Targetable GetTarget()
    {
        return target;
    }
    public override void OnStartServer()
    {
        GameOverHandler.ServerOnGameOver += ServerHandleGameOver;
    }
    public override void OnStopServer()
    {
        GameOverHandler.ServerOnGameOver -= ServerHandleGameOver;
    }
    [Command]
    public void CmdSetTarget(GameObject targetGameObject)
    {
        if (!targetGameObject.TryGetComponent<Targetable>(out Targetable newtarget)) { return; }
        target = newtarget;
    }
    public void ClearTarget()
    {
        target = null;
    }
    [Server]
    private void ServerHandleGameOver()
    {
        ClearTarget();
    }
}