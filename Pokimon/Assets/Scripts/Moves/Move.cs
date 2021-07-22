using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Move
{
    public MoveBase Base
    {
        get => _base;
        set => _base = value;
    }

    public int PP
    {
        get => _pp;
        set => _pp = value;
    }

    private MoveBase _base;
    private int _pp;
    
    // Constructor
    public Move(MoveBase moveBase)
    {
        Base = moveBase;
        PP = moveBase.PP;
    }
}
