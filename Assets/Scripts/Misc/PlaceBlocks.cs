using NaughtyAttributes;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlaceBlocks : MonoBehaviour
{
    public LevelInfoSO LevelInfo;
    
    [SerializeField] private GameObject _blockInstance;

    [Space] [SerializeField] [ReadOnly] private int _blockCount;

    private void Awake()
    {
        _blockCount = LevelInfo.BlockCount;
    }

    private void Update()
    {
        if (Mouse.current.leftButton.wasPressedThisFrame)
        {
            Vector2 mousePos = Camera.main.ScreenToWorldPoint(Mouse.current.position.value);

            RaycastHit2D hit = Physics2D.Raycast(mousePos, Vector2.zero, 0f);
            if (!hit && _blockCount > 0) {
                GameObject obj = Instantiate(_blockInstance);
                obj.transform.position = new Vector2(Mathf.Floor(mousePos.x), Mathf.Floor(mousePos.y));
                _blockCount -= 1;
            }
        }

        if (Mouse.current.rightButton.wasPressedThisFrame)
        {
            RaycastHit2D hit = Physics2D.Raycast(Camera.main.ScreenToWorldPoint(Mouse.current.position.value), Vector2.zero, 0f);
            if (hit) {
                if (hit.collider.gameObject.CompareTag("Wall"))
                {
                    Destroy(hit.collider.gameObject);
                    _blockCount += 1;
                }
            }
        }
    }
}
