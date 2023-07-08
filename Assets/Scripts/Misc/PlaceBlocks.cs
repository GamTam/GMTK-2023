using UnityEngine;
using UnityEngine.InputSystem;

public class PlaceBlocks : MonoBehaviour
{
    [SerializeField] private GameObject _blockInstance;

    private void Update()
    {
        if (Mouse.current.leftButton.wasPressedThisFrame)
        {
            Vector2 mousePos = Camera.main.ScreenToWorldPoint(Mouse.current.position.value);

            RaycastHit2D hit = Physics2D.Raycast(mousePos, Vector2.zero, 0f);
            if (!hit) {
                GameObject obj = Instantiate(_blockInstance);
                obj.transform.position = new Vector2(Mathf.Floor(mousePos.x), Mathf.Floor(mousePos.y));
            }
        }

        if (Mouse.current.rightButton.wasPressedThisFrame)
        {
            RaycastHit2D hit = Physics2D.Raycast(Camera.main.ScreenToWorldPoint(Mouse.current.position.value), Vector2.zero, 0f);
            if (hit) {
                if (hit.collider.gameObject.CompareTag("Wall")) Destroy(hit.collider.gameObject);
            }
        }
    }
}
