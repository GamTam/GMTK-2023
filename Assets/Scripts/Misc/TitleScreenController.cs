using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TitleScreenController : MonoBehaviour
{
    [SerializeField] private GameObject _fade;
    
    void Start()
    {
        Globals.MusicManager.Play("Puzzle");
    }

    public void BeginLevelLoad()
    {
         Instantiate(_fade).GetComponent<LevelLoader>().LevelToLoad = "Main";
    }
}
