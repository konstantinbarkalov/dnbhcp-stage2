using UnityEngine;

public class ZombieHand : MonoBehaviour
{
    private ZombieRagdollBehaviour callback;

    public void CatchPlayer(Rigidbody player, ZombieRagdollBehaviour callback)
    {
        this.callback = callback;
        FixedJoint joint = gameObject.AddComponent<FixedJoint>();
        joint.connectedBody = player;
        joint.breakForce = 50;
    }

    private void OnJointBreak(float breakForce){
        callback.LosePlayer();    
    }
}
