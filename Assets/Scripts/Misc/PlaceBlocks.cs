using System;
using System.Collections.Generic;
using NaughtyAttributes;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

public class PlaceBlocks : MonoBehaviour
{
    public LevelInfoSO LevelInfo;
    
    [SerializeField] private GameObject _blockInstance;
    [SerializeField] private Camera _cam;
    [SerializeField] private GameObject _wall;
    [SerializeField] private GameObject _winSpot;
    [SerializeField] private GameObject _arrowBlock;
    [SerializeField] private GameObject _arrowContainer;
    [SerializeField] private GameObject _stageEnd;
    [SerializeField] private GameObject _nextStageButton;
    
    [Space]
    [SerializeField] private TMP_Text _headerText;
    [SerializeField] private TMP_Text _timerText;
    [SerializeField] private TMP_Text _blockText;

    [Space] [SerializeField] [ReadOnly] private int _blockCount;
    [SerializeField] [ReadOnly] private List<GameObject> _realBlocks;
    [SerializeField] [ReadOnly] private List<GameObject> _levelBlocks;
    [SerializeField] [ReadOnly] private List<GameObject> _arrowList;
    [SerializeField] [ReadOnly] private Vector2 _gridSize = new Vector2(18, 10);
    [SerializeField] [ReadOnly] private float _stageEndTimer = 0f;

    private float _timer;
    private PlayerController _player;
    private bool _ignoreInput;

    private void Start()
    {
        _player = FindObjectOfType<PlayerController>();
        if (LevelInfo != null) _blockCount = LevelInfo.BlockCount;
        else _blockCount = Int32.MaxValue;

        if (LevelInfo != null)
        {
            _headerText.text = $"{LevelInfo.StageHeader}\n{LevelInfo.StageSubheader}";
            _timerText.text = "00:00:00";
            
            if (LevelInfo.NextLevel == null) _nextStageButton.SetActive(false);
            else _nextStageButton.SetActive(true);

            for (int i = 0; i < LevelInfo.LevelGrid.rows.Length; i++)
            {
                for (int j = 0; j < LevelInfo.LevelGrid.rows[i].row.Length; j++)
                {
                    GameObject obj = null;
                    switch (LevelInfo.LevelGrid.rows[i].row[j])
                    {
                        case TileTypes.E:
                            obj = Instantiate(_winSpot);
                            obj.transform.position = new Vector3(j - (_gridSize.x / 2), -(i - (_gridSize.y / 2)) - 1, obj.transform.position.z);
                            _levelBlocks.Add(obj);
                            break;
                        case TileTypes.W:
                            obj = Instantiate(_wall);
                            obj.transform.position = new Vector3(j - (_gridSize.x / 2), -(i - (_gridSize.y / 2)) - 1, obj.transform.position.z);
                            _levelBlocks.Add(obj);
                            break;
                        case TileTypes.L:
                            obj = Instantiate(_blockInstance);
                            obj.transform.position = new Vector3(j - (_gridSize.x / 2), -(i - (_gridSize.y / 2)) - 1, obj.transform.position.z);
                            _realBlocks.Add(obj);
                            break;
                        case TileTypes.P:
                            _player.SetStartPos(new Vector2(j - (_gridSize.x / 2), -(i - (_gridSize.y / 2)) - 1), LevelInfo);
                            break;
                    }
                }
            }
        }

        foreach (MoveDirections dir in LevelInfo.MoveQueue)
        {
            GameObject obj = Instantiate(_arrowBlock, _arrowContainer.transform);
            ArrowImageController arrow = obj.GetComponent<ArrowImageController>();
            arrow.UpdateSprite(dir);
            _arrowList.Add(obj);
            _player._arrows.Add(arrow);
        }

        Globals.MusicManager.Play("Puzzle");
    }

    private void LateUpdate()
    {
        if (_player.enabled) _timer += Time.deltaTime;
        else
        {
            _stageEndTimer += Time.deltaTime;
            if (_stageEndTimer >= 1.25f)
            {
                _stageEnd.SetActive(true);
            }
        }
        TimeSpan time = TimeSpan.FromSeconds(_timer);
        if (LevelInfo != null) _headerText.text = $"{LevelInfo.StageHeader}\n{LevelInfo.StageSubheader}";
        _timerText.text = time.ToString("hh':'mm':'ss");
        _blockText.text = $"x{_blockCount}";

        _ignoreInput = false;
    }

    public void PlaceOrRemoveBlock(Vector2 normalPos, PointerEventData.InputButton button)
    {
        if (_ignoreInput) return;
        
        Vector2 mousePos = new Vector2(normalPos.x * _gridSize.x, normalPos.y * _gridSize.y);
        mousePos = new Vector2(mousePos.x - (_gridSize.x / 2), mousePos.y - (_gridSize.y / 2));
        
        if (button == PointerEventData.InputButton.Left)
        {
            RaycastHit2D hit = Physics2D.Raycast(mousePos, Vector2.zero, 0f);
            if (!hit && _blockCount > 0)
            {
                GameObject obj = Instantiate(_blockInstance);
                obj.transform.position = new Vector2(Mathf.Floor(mousePos.x), Mathf.Floor(mousePos.y));
                _realBlocks.Add(obj);
                _blockCount -= 1;

                System.Random rand = new System.Random();
                int soundIndex = rand.Next(3) + 1;

                Globals.SoundManager.Play("place_" + soundIndex);
            }
        } 
        else if (button == PointerEventData.InputButton.Right)
        {
            RaycastHit2D hit =
                Physics2D.Raycast(mousePos, Vector2.zero, 0f);
            if (hit)
            {
                if (hit.collider.gameObject.CompareTag("Wall"))
                {
                    if (_realBlocks.Contains(hit.collider.gameObject)) _realBlocks.Remove(hit.collider.gameObject);
                    hit.collider.gameObject.GetComponent<Animator>().Play("Close");
                    _blockCount += 1;
                    
                    System.Random rand = new System.Random();
                    int soundIndex = rand.Next(3) + 1;

                    Globals.SoundManager.Play("take_" + soundIndex);
                }
            }
        }
    }

    public void KillAllBlocks(bool restartTimer)
    {
        foreach (GameObject obj in _realBlocks)
        {
            if (!restartTimer && obj != null) obj.GetComponent<Animator>().Play("Close");
            else Destroy(obj);
        }

        _blockCount = LevelInfo.BlockCount;
        if (!restartTimer)
        {
            _realBlocks = new List<GameObject>();
            for (int i = 0; i < LevelInfo.LevelGrid.rows.Length; i++)
            {
                for (int j = 0; j < LevelInfo.LevelGrid.rows[i].row.Length; j++)
                {
                    GameObject obj = null;
                    switch (LevelInfo.LevelGrid.rows[i].row[j])
                    {
                        case TileTypes.L:
                            obj = Instantiate(_blockInstance);
                            obj.transform.position = new Vector3(j - (_gridSize.x / 2), -(i - (_gridSize.y / 2)) - 1,
                                obj.transform.position.z);
                            _realBlocks.Add(obj);
                            break;
                    }
                }
            }
        }

        Globals.SoundManager.Play("reset");
        if (restartTimer)
        {
            _timer = 0f;
            _stageEndTimer = 0f;
        }
    }

    public void ResetLevel()
    {
        foreach (GameObject obj in _realBlocks)
        {
            Destroy(obj);
        }

        _blockCount = LevelInfo.BlockCount;
        _realBlocks = new List<GameObject>();
        for (int i = 0; i < LevelInfo.LevelGrid.rows.Length; i++)
        {
            for (int j = 0; j < LevelInfo.LevelGrid.rows[i].row.Length; j++)
            {
                GameObject obj = null;
                switch (LevelInfo.LevelGrid.rows[i].row[j])
                {
                    case TileTypes.L:
                        obj = Instantiate(_blockInstance);
                        obj.transform.position = new Vector3(j - (_gridSize.x / 2), -(i - (_gridSize.y / 2)) - 1,
                            obj.transform.position.z);
                        _realBlocks.Add(obj);
                        break;
                }
            }
        }

        Globals.SoundManager.Play("reset");
        _timer = 0f;
        _stageEndTimer = 0f;
    }

    public void NextLevel()
    {
        KillAllBlocks(true);
        
        LevelInfo = LevelInfo.NextLevel;
        if (LevelInfo != null) _blockCount = LevelInfo.BlockCount;
        else _blockCount = Int32.MaxValue;

        if (LevelInfo != null)
        {
            _headerText.text = $"{LevelInfo.StageHeader}\n{LevelInfo.StageSubheader}";
            _timerText.text = "00:00:00";

            foreach (GameObject obj in _levelBlocks)
            {
                if (obj != null) Destroy(obj);
            }

            _levelBlocks = new List<GameObject>();
            
            for (int i = 0; i < LevelInfo.LevelGrid.rows.Length; i++)
            {
                for (int j = 0; j < LevelInfo.LevelGrid.rows[i].row.Length; j++)
                {
                    GameObject obj = null;
                    switch (LevelInfo.LevelGrid.rows[i].row[j])
                    {
                        case TileTypes.E:
                            obj = Instantiate(_winSpot);
                            obj.transform.position = new Vector3(j - (_gridSize.x / 2), -(i - (_gridSize.y / 2)) - 1, obj.transform.position.z);
                            _levelBlocks.Add(obj);
                            break;
                        case TileTypes.W:
                            obj = Instantiate(_wall);
                            obj.transform.position = new Vector3(j - (_gridSize.x / 2), -(i - (_gridSize.y / 2)) - 1, obj.transform.position.z);
                            _levelBlocks.Add(obj);
                            break;
                        case TileTypes.L:
                            Debug.Log("a");
                            obj = Instantiate(_blockInstance);
                            obj.transform.position = new Vector3(j - (_gridSize.x / 2), -(i - (_gridSize.y / 2)) - 1, obj.transform.position.z);
                            _realBlocks.Add(obj);
                            break;
                        case TileTypes.P:
                            _player.SetStartPos(new Vector2(j - (_gridSize.x / 2), -(i - (_gridSize.y / 2)) - 1), LevelInfo);
                            break;
                    }
                }
            }
            
            if (LevelInfo.NextLevel == null) _nextStageButton.SetActive(false);
            else _nextStageButton.SetActive(true);
        }

        foreach (ArrowImageController arrow in _player._arrows)
        {
            Destroy(arrow.gameObject);
        }
        
        _player._arrows = new List<ArrowImageController>();
        
        foreach (MoveDirections dir in LevelInfo.MoveQueue)
        {
            GameObject obj = Instantiate(_arrowBlock, _arrowContainer.transform);
            ArrowImageController arrow = obj.GetComponent<ArrowImageController>();
            arrow.UpdateSprite(dir);
            _arrowList.Add(obj);
            _player._arrows.Add(arrow);
        }

        _ignoreInput = true;
    }
}
