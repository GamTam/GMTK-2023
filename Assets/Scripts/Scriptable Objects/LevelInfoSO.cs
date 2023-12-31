using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Level Info", menuName = "GMTK 2023/Level Info")]
public class LevelInfoSO : ScriptableObject
{
    public string StageHeader;
    public string StageSubheader;
    public bool FacingLeft;
    [Space]
    public int BlockCount;
    public LevelInfoSO NextLevel;
    public TileData LevelGrid;
    public List<MoveDirections> MoveQueue;
}

[System.Serializable]
public class TileData
{
    [System.Serializable]
    public struct RowData
    {
        public TileTypes[] row;
    }

    public RowData[] rows = new RowData[10];
}

public enum TileTypes
{
    O,
    W,
    P,
    E,
    L
}

public enum MoveDirections
{
    Up,
    Down,
    Left,
    Right
}
