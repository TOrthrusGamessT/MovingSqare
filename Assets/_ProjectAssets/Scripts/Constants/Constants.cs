using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Constants
{
    public static float platformSpeed = 2f;
    public static Vector3 littleBarrierAppearPosition = new Vector3(-0.75f, 1.26f, 0);
    public static Vector3 mediumBarrierAppearPosition = new Vector3(-0.59f, 1.26f, 0);
    public static Vector3 bigBarrierAppearPosition = new Vector3(-0.4100001f, 1.26f, 0);

    [System.Serializable]
    public class SoundClips
    {
        public Sounds name;
        public AudioClip audioClip;
    }

    public enum GeometryFigure
    {
        Square,
        Circle,
        Hexagon
    }

    public enum Directions
    {
        E,
        V,
        W,
        S
    }

    public enum Sounds
    {
        DestroyEnemy,
        PickCoin,
        PlayerDeath,
        PickLife,
        PlayerGetHit
    }

    public enum BarrierType
    {
        LittleBarrier,
        MediumBarrier,
        BigBarrier
    }

    public enum BarrierPosition
    {
        Left,
        Right
    }

    [System.Serializable]
    public class BarrierSet
    {
        public BarrierType barrierType;
        public BarrierPosition barrierPosition;

    }
}
