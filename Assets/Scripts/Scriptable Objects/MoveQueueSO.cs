using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Move Queue", menuName = "GMTK 2023/Move Queue")]
public class MoveQueueSO : ScriptableObject
{
    public List<MoveDirections> MoveQueue;
}

public enum MoveDirections
{
    Up,
    Down,
    Left,
    Right
}
