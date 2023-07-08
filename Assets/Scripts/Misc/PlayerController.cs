using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private float _charSpeed;

    private bool _moving;
    private Vector2 _dir;

    private PlayerInput _input;
    private InputAction _move;

    private void Start()
    {
        _input = FindObjectOfType<PlayerInput>();
        _move = _input.actions["Main/Move"];
    }
    
    private void LateUpdate()
    {
        Vector2 pos = transform.position;
        pos += _dir * (_charSpeed * Time.deltaTime);
        transform.position = pos;
        
        if (_moving) return;
        if (_move.ReadValue<Vector2>() == Vector2.zero) return;

        _moving = true;
        _dir = _move.ReadValue<Vector2>().normalized;
    }

    private void OnCollisionEnter2D(Collision2D other)
    {
        if (other.gameObject.CompareTag("Wall"))
        {
            _dir = Vector2.zero;
            _moving = false;
            
            Vector2 pos = transform.position;
            pos = new Vector2(Mathf.Round(pos.x), Mathf.Round(pos.y));
            transform.position = pos;
        }
    }
}
