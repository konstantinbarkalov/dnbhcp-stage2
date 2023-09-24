using UnityEngine;
namespace Crowd
{

public class PlayerProjection : MonoBehaviour
{
    [SerializeField]
    private Transform player;
    private Transform projectionTransform;
    private Vector3 projectionPosition;
    private RaycastHit hit;
    private int layerMask;

    private void Start()
    {
        player = GameObject.FindWithTag("Player").transform;
        projectionTransform = GetComponent<Transform>();
        layerMask = (1 << 6);
    }

    private void LateUpdate()
    {
        CalculateFloorPosition();
        projectionTransform.position = projectionPosition;
    }

    private void CalculateFloorPosition()
    {
        Physics.Raycast(
            player.position,
            transform.TransformDirection(Vector3.down),
            out hit,
            Mathf.Infinity,
            layerMask
        );
        projectionPosition = hit.point;
    }
}
}