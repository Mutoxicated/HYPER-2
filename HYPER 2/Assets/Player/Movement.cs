using UnityEngine;
using UnityEngine.InputSystem;
using Helpers;

public class Movement : MonoBehaviour
{
    [SerializeField] private Player player;
    [SerializeField] private Transform obj;
    [SerializeField] private Transform dir;
    [SerializeField, Range(1f,10f)] private float speed;
    [SerializeField, Range(1f,2f)] private float multiplier = 1.5f;
    
    private float towerRadius = 5f;

    private Vector2 scrollVector;
    private Vector2 moveVector;
    private Vector3 direction;
    private Vector3 hypothetical;

    private InputAction move;
    private InputAction faster;
    private InputAction scroll;

    public void DStart() {
        obj = obj == null ? transform : obj;
        dir = dir == null ? transform : dir;

        move = player.FindAction("Move");
        faster = player.FindAction("Faster");
        scroll = player.FindAction("Scroll");
    }

    public void Update() {
        hypothetical = obj.position;
        direction = Vector3.zero;
        moveVector = move.ReadValue<Vector2>();

        var forward = dir.transform.forward.normalized;
        
        scrollVector = scroll.ReadValue<Vector2>();
        if (scrollVector != Vector2.zero) {
            scrollVector.Normalize();
            direction = forward*scrollVector.y;

            hypothetical = Vector3.Lerp(hypothetical, hypothetical+direction*40f, Time.deltaTime);
        }
        forward.y = 0f;
        forward.Normalize();
        //Debug.Log(forward);
        
        direction = dir.transform.right*moveVector.x+forward*moveVector.y;
        direction.Normalize();

        if (faster.IsPressed()) {
            hypothetical = Vector3.Lerp(hypothetical, hypothetical+direction*speed*multiplier, Time.deltaTime);
        }else {
            hypothetical = Vector3.Lerp(hypothetical, hypothetical+direction*speed, Time.deltaTime);
        }
        var dist = Vector2.Distance(Vec3.ToV2XZ(hypothetical), Vec3.ToV2XZ(player.Manager.Tower.transform.position));
        if (dist < towerRadius) {
            var diff = dist-towerRadius;
            hypothetical = hypothetical+direction*diff;
        }
        if (dist > player.World.BorderRadius) {
            var diff = dist-player.World.BorderRadius;
            hypothetical = hypothetical-direction*diff;
        }
        obj.position = hypothetical;
    }
}
