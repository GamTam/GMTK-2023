using System;
using System.Collections.Generic;
using NaughtyAttributes;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Serialization;

public class PlayerController : MonoBehaviour
{
    [SerializeField] public bool _readingQueue;
    [SerializeField] private Shake _shake;
    [SerializeField] private float _moveDelay = 0.5f;
    [SerializeField] private GameObject _confetti;
    
    [Space]
    [SerializeField] private float _charSpeed;
    [SerializeField] private float _speedUpFactor = 2;

    [Space]
    [SerializeField] [ReadOnly] private LevelInfoSO _levelInfo;
    [SerializeField] [ReadOnly] private int _currentQueuePos = 0;
    [SerializeField] [ReadOnly] private float _timeUntilNextMove = 0;
    [SerializeField] [ReadOnly] private float _velocity = 0;
    [SerializeField] [ReadOnly] private bool _canWin = false;

    private Vector2 _startingPos;
    private bool _flipXAtStart;
    private SpriteRenderer _spr;
    private bool _moving;
    private Vector2 _dir;
    private Vector2 _prevMoveVector;
    private GameObject _confettiInstance;

    private PlayerInput _input;
    private InputAction _move;
    private List<MoveDirections> _moveQueue = new List<MoveDirections>();
    [HideInInspector] public List<ArrowImageController> _arrows = new List<ArrowImageController>();

    private void Awake()
    {
        _startingPos = transform.position;
        _input = FindObjectOfType<PlayerInput>();
        _move = _input.actions["Main/Move"];
        _spr = GetComponentInChildren<SpriteRenderer>();
    }
    
    private void LateUpdate()
    {
        if (_canWin && !_moving && _timeUntilNextMove <= 0 && _currentQueuePos == _moveQueue.Count)
        {
            Globals.SoundManager.Play("win");
            enabled = false;
            _confettiInstance = Instantiate(_confetti);
            return;
        }
        
        Vector2 pos = transform.position;
        _velocity += Time.deltaTime * _speedUpFactor;
        pos += _dir * ((Mathf.Min(_charSpeed, _velocity)) * Time.deltaTime);
        transform.position = pos;
        
        if (_levelInfo == null)
        {
            if (!_moving) _timeUntilNextMove -= Time.deltaTime;
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

            _arrows[_currentQueuePos].StartFade(Color.gray);
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
        enabled = true;
        _canWin = false;
        if (_confettiInstance != null) Destroy(_confettiInstance);
        foreach (ArrowImageController arrow in _arrows)
        {
            arrow.StartFade(Color.white);
        }
    }

    public void SetStartPos(Vector2 pos, LevelInfoSO levelInfo)
    {
        _startingPos = pos;
        transform.position = pos;
        _levelInfo = levelInfo;
        _flipXAtStart = _levelInfo.FacingLeft;
        _spr.flipX = _flipXAtStart;
        _moveQueue = _levelInfo.MoveQueue;
    }

    public void StartMovement()
    {
        _readingQueue = true;
    }

    private void OnTriggerEnter2D(Collider2D col)
    {
        if (col.gameObject.CompareTag("Finish"))
        {
            _canWin = true;
        }
    }
    
    private void OnTriggerExit2D(Collider2D col)
    {
        if (col.gameObject.CompareTag("Finish"))
        {
            _canWin = false;
        }
    }
}
