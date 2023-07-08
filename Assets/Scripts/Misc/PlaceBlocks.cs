using NaughtyAttributes;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

public class PlaceBlocks : MonoBehaviour
{
    public LevelInfoSO LevelInfo;
    
    [SerializeField] private GameObject _blockInstance;
    [SerializeField] private Camera _cam;
    [SerializeField] private Vector2 _gridSize;

    [Space] [SerializeField] [ReadOnly] private int _blockCount;

    private void Awake()
    {
        _blockCount = LevelInfo.BlockCount;
    }

    public void PlaceOrRemoveBlock(Vector2 normalPos, PointerEventData.InputButton button)
    {
        Vector2 mousePos = new Vector2(normalPos.x * _gridSize.x, normalPos.y * _gridSize.y);
        mousePos = new Vector2(mousePos.x - (_gridSize.x / 2), mousePos.y - (_gridSize.y / 2));
        Debug.Log(mousePos);
        
        if (button == PointerEventData.InputButton.Left)
        {
            RaycastHit2D hit = Physics2D.Raycast(mousePos, Vector2.zero, 0f);
            if (!hit && _blockCount > 0)
            {
                GameObject obj = Instantiate(_blockInstance);
                obj.transform.position = new Vector2(Mathf.Floor(mousePos.x), Mathf.Floor(mousePos.y));
                _blockCount -= 1;
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
                    Destroy(hit.collider.gameObject);
                    _blockCount += 1;
                }
            }
        }
    }
}
