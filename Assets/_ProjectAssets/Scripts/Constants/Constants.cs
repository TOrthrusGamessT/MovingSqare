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

    public static List<Color> brightColorPalette = new List<Color>
    {
        Color.HSVToRGB(0f, 0.4f, 1f),        // More saturated Red
        Color.HSVToRGB(240f/360f, 0.4f, 1f), // More saturated Blue
        Color.HSVToRGB(60f/360f, 0.4f, 1f),  // More saturated Yellow
        Color.HSVToRGB(300f/360f, 0.4f, 1f), // More saturated Magenta
        Color.HSVToRGB(30f/360f, 0.4f, 1f),  // More saturated Orange`
        Color.HSVToRGB(270f/360f, 0.4f, 1f), // More saturated Purple
        Color.HSVToRGB(330f/360f, 0.4f, 1f), // More saturated Pink
        Color.HSVToRGB(45f/360f, 0.4f, 1f),  // More saturated  Gold
        Color.HSVToRGB(200f/360f, 0.4f, 1f)  // More saturated Sky Blue
    };

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
