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
    
    [Space]
    [SerializeField] private TMP_Text _headerText;
    [SerializeField] private TMP_Text _blockText;

    [Space] [SerializeField] [ReadOnly] private int _blockCount;
    [SerializeField] [ReadOnly] private List<GameObject> _realBlocks;
    [SerializeField] [ReadOnly] private List<GameObject> _levelBlocks;
    [SerializeField] [ReadOnly] private Vector2 _gridSize = new Vector2(18, 10);

    private float _timer;
    private PlayerController _player;

    private void Start()
    {
        _player = FindObjectOfType<PlayerController>();
        if (LevelInfo != null) _blockCount = LevelInfo.BlockCount;
        else _blockCount = Int32.MaxValue;

        if (LevelInfo != null)
        {
            _headerText.text = $"{LevelInfo.StageHeader}\n{LevelInfo.StageSubheader}\n00:00:00";

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
                        case TileTypes.P:
                            _player.SetStartPos(new Vector2(j - (_gridSize.x / 2), -(i - (_gridSize.y / 2)) - 1), LevelInfo);
                            break;
                    }
                }
            }
        }

        Globals.MusicManager.Play("Puzzle");
    }

    private void Update()
    {
        _timer += Time.deltaTime;
        TimeSpan time = TimeSpan.FromSeconds(_timer);
        if (LevelInfo != null) _headerText.text = $"{LevelInfo.StageHeader}\n{LevelInfo.StageSubheader}\n{time.ToString("hh':'mm':'ss")}";
        _blockText.text = $"x{_blockCount}";
    }

    public void PlaceOrRemoveBlock(Vector2 normalPos, PointerEventData.InputButton button)
    {
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
                if (_realBlocks.Contains(hit.collider.gameObject))
                {
                    _realBlocks.Remove(hit.collider.gameObject);
                    Destroy(hit.collider.gameObject);
                    _blockCount += 1;
                    
                    System.Random rand = new System.Random();
                    int soundIndex = rand.Next(3) + 1;

                    Globals.SoundManager.Play("take_" + soundIndex);
                }
            }
        }
    }

    public void KillAllBlocks()
    {
        foreach (GameObject obj in _realBlocks)
        {
            Destroy(obj);
            _blockCount += 1;
        }

        Globals.SoundManager.Play("reset");
        _realBlocks = new List<GameObject>();
    }
}
