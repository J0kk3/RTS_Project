using UnityEngine;
using Mirror;

public class Targetable : NetworkBehaviour
{
    [SerializeField] private Transform aimAtPoint = null;
    
    public Transform GetAimAtPoint()
    {
        return aimAtPoint;
    }
}