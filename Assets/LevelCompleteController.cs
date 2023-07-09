using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LevelCompleteController : MonoBehaviour
{
    [SerializeField] private TMP_Text[] _text;
    [SerializeField] private Image[] _images;
    [SerializeField] private GameObject _fade;

    public void ResetMenu()
    {
        foreach (TMP_Text text in _text)
        {
            text.alpha = 0;
        }

        foreach (Image img in _images)
        {
            img.color = Color.clear;
        }

        _fade.transform.localScale = Vector3.zero;
        
        gameObject.SetActive(false);
    }
}
