using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ArrowImageController : MonoBehaviour
{
    [SerializeField] private Sprite[] _directions;

    private Image _image;

    private void Awake()
    {
        _image = GetComponent<Image>();
    }

    public void UpdateSprite(MoveDirections dir)
    {
        Sprite spr = null;

        switch (dir)
        {
            case MoveDirections.Up:
                spr = _directions[0];
                break;
            case MoveDirections.Down:
                spr = _directions[1];
                break;
            case MoveDirections.Left:
                spr = _directions[2];
                break;
            case MoveDirections.Right:
                spr = _directions[3];
                break;
        }
        
        _image.sprite = spr;
    }

    public void StartFade(Color color)
    {
        StartCoroutine(FadeOut(color));
    }

    private IEnumerator FadeOut(Color color)
    {
        float time = 0;
        float duration = 0.25f;

        Color startColor = _image.color;

        while (time < duration)
        {
            time += Time.deltaTime;
            _image.color = Color.Lerp(startColor, color, time / duration);
            yield return null;
        }

        _image.color = color;
    }
}
