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

    private Vector2 moveVector;
    private Vector3 direction;
    private Vector3 hypothetical;

    private InputAction move;
    private InputAction faster;

    private void Start() {

        obj = obj == null ? transform : obj;
        dir = dir == null ? transform : dir;

        move = player.FindAction("Move");
        faster = player.FindAction("Faster");
    }

    public void Update() {
        moveVector = move.ReadValue<Vector2>();

        var forward = dir.transform.forward.normalized;
        forward.y = 0f;

        direction = dir.transform.right*moveVector.x+forward*moveVector.y;

        
        if (faster.IsPressed()) {
            hypothetical = Vector3.Lerp(obj.position, obj.position+direction*speed*multiplier, Time.deltaTime);
        }else {
            hypothetical = Vector3.Lerp(obj.position, obj.position+direction*speed, Time.deltaTime);
        }
        var dist = Vector2.Distance(Vec3.ToV2XZ(hypothetical), Vec3.ToV2XZ(player.Manager.Tower.transform.position));
        if (dist < towerRadius && moveVector.y > 0) {
            var diff = dist-towerRadius;
            hypothetical = hypothetical+direction*diff;
        }
        obj.position = hypothetical;
    }
}
