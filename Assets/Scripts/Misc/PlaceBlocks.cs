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
            Debug.Log(mousePos);

            GameObject obj = Instantiate(_blockInstance);
            obj.transform.position = new Vector2(Mathf.Floor(mousePos.x), Mathf.Floor(mousePos.y));
        }
    }
}
