using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LevelLoader : MonoBehaviour
{
    [SerializeField] private Image _fade;

    public string LevelToLoad;
    
    IEnumerator Start()
    {
        DontDestroyOnLoad(gameObject);
        yield return null;

        float time = 0f;
        float duration = 1f;

        while (time < duration)
        {
            time += Time.deltaTime;
            _fade.color = Color.Lerp(Color.clear, Color.black, time / duration);
            yield return null;
        }

        _fade.color = Color.black;
        yield return null;

        SceneManager.LoadScene(LevelToLoad);
        
        time = 0f;
        while (time < duration)
        {
            time += Time.deltaTime;
            _fade.color = Color.Lerp(Color.black, Color.clear, time / duration);
            yield return null;
        }
        
        Destroy(gameObject);
    }
}
