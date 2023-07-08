using UnityEngine;
using UnityEngine.EventSystems;

public class RenderTextureRaycast : MonoBehaviour, IPointerClickHandler
{
    [SerializeField] protected Camera UICamera;
    [SerializeField] protected RectTransform RawImageRectTrans;
    [SerializeField] protected PlaceBlocks _placeBlocks;
    
    public void OnPointerClick(PointerEventData eventData)
    {
        Vector2 localPoint;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(RawImageRectTrans, eventData.position, UICamera, out localPoint);
        Vector2 normalizedPoint = Rect.PointToNormalized(RawImageRectTrans.rect, localPoint);
        _placeBlocks.PlaceOrRemoveBlock(normalizedPoint, eventData.button);
    }
}