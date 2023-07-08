using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Level Info", menuName = "GMTK 2023/Level Info")]
public class LevelInfoSO : ScriptableObject
{
    public string StageHeader;
    public string StageSubheader;
    public int BlockCount;
    public List<MoveDirections> MoveQueue;
}

public enum MoveDirections
{
    Up,
    Down,
    Left,
    Right
}
