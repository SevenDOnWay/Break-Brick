using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class BrickData {
    public enum BrickType {
        Normal,
        Split,
        x2health,
    }

    public int col;
    public int row;
    public int health;
    public BrickType type;

    public BrickData( int col, int row, int health, BrickType type = BrickType.Normal ) {
        this.col = col;
        this.row = row;
        this.health = health;
        this.type = type;
    }
}



