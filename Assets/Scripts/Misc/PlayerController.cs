using NaughtyAttributes;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Serialization;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private float _charSpeed;
    [SerializeField] private bool _readingQueue;
    [SerializeField] private float _moveDelay = 0.5f;

    [Space]
    [SerializeField] [ReadOnly] private LevelInfoSO _levelInfo;
    [SerializeField] [ReadOnly] private int _currentQueuePos = 0;
    [SerializeField] [ReadOnly] private float _timeUntilNextMove = 0;
    private bool _moving;
    private Vector2 _dir;
    private Vector2 _prevMoveVector;

    private PlayerInput _input;
    private InputAction _move;

    private void Start()
    {
        _input = FindObjectOfType<PlayerInput>();
        _move = _input.actions["Main/Move"];
        _levelInfo = FindObjectOfType<PlaceBlocks>().LevelInfo;
    }
    
    private void LateUpdate()
    {
        Vector2 pos = transform.position;
        pos += _dir * (_charSpeed * Time.deltaTime);
        transform.position = pos;
        
        if (_levelInfo == null)
        {
            GetInput();
            _prevMoveVector = _move.ReadValue<Vector2>().normalized;
        }
        else if (_readingQueue)
        {
            if (_moving) return;
            if (_timeUntilNextMove > 0)
            {
                _timeUntilNextMove -= Time.deltaTime;
                return;
            }
            if (_currentQueuePos >= _levelInfo.MoveQueue.Count) return;

            _moving = true;
            switch (_levelInfo.MoveQueue[_currentQueuePos])
            {
                case MoveDirections.Up:
                    _dir = Vector2.up;
                    break;
                case MoveDirections.Down:
                    _dir = Vector2.down;
                    break;
                case MoveDirections.Left:
                    _dir = Vector2.left;
                    break;
                case MoveDirections.Right:
                    _dir = Vector2.right;
                    break;
            }

            _currentQueuePos += 1;
        }
    }

    private void GetInput()
    {
        if (_moving) return;
        if (_prevMoveVector != Vector2.zero) return;
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
            _timeUntilNextMove = _moveDelay;
        }
    }
}
