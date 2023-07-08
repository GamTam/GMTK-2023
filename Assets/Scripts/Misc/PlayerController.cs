using NaughtyAttributes;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Serialization;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private bool _readingQueue;
    [SerializeField] private Shake _shake;
    [SerializeField] private float _moveDelay = 0.5f;
    
    [Space]
    [SerializeField] private float _charSpeed;
    [SerializeField] private float _speedUpFactor = 2;

    [Space]
    [SerializeField] [ReadOnly] private LevelInfoSO _levelInfo;
    [SerializeField] [ReadOnly] private int _currentQueuePos = 0;
    [SerializeField] [ReadOnly] private float _timeUntilNextMove = 0;
    [SerializeField] [ReadOnly] private float _velocity = 0;

    private Vector2 _startingPos;
    private bool _flipXAtStart;
    private SpriteRenderer _spr;
    private bool _moving;
    private Vector2 _dir;
    private Vector2 _prevMoveVector;

    private PlayerInput _input;
    private InputAction _move;

    private void Start()
    {
        _startingPos = transform.position;
        _input = FindObjectOfType<PlayerInput>();
        _move = _input.actions["Main/Move"];
        _levelInfo = FindObjectOfType<PlaceBlocks>().LevelInfo;
        _spr = GetComponentInChildren<SpriteRenderer>();
        _flipXAtStart = _spr.flipX;
        Globals.MusicManager.Play("Puzzle");
    }
    
    private void LateUpdate()
    {
        Vector2 pos = transform.position;
        _velocity += Time.deltaTime * _speedUpFactor;
        pos += _dir * ((Mathf.Min(_charSpeed, _velocity)) * Time.deltaTime);
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
                    _spr.flipX = true;
                    break;
                case MoveDirections.Right:
                    _dir = Vector2.right;
                    _spr.flipX = false;
                    break;
            }

            _velocity = 0;
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

        if (_dir.x > 0) _spr.flipX = false;
        else if (_dir.x < 0) _spr.flipX = true;
        _velocity = 0;
    }

    private void OnCollisionEnter2D(Collision2D other)
    {
        if (other.gameObject.CompareTag("Wall") || other.gameObject.CompareTag("Static Wall"))
        {
            _dir = Vector2.zero;
            _moving = false;
            
            Vector2 pos = transform.position;
            pos = new Vector2(Mathf.Round(pos.x), Mathf.Round(pos.y));
            transform.position = pos;
            _timeUntilNextMove = _moveDelay;
            _shake.enabled = true;
            _shake.maxShakeDuration = 0.25f;
            _shake.multiplier = 0.25f * Mathf.Min(_velocity * 0.25f, _charSpeed);
            Globals.SoundManager.Play("hit");
        }
    }

    public void ResetPosition()
    {
        transform.position = _startingPos;
        _dir = Vector2.zero;
        _moving = false;
        _spr.flipX = _flipXAtStart;
        _readingQueue = false;
        _currentQueuePos = 0;
    }

    public void StartMovement()
    {
        _readingQueue = true;
    }
}
