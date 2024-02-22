using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Define
{
    public enum CreatureState
    {
        Idle,
        Moving,
        Arrived,    // 이동 중 목표지점에 도달했지만 아직 이동중일 경우 Idle애니메이션으로 업데이트 되지 않도록 설정하기 위한 임시 목표지점 도달 상태
        Skill,
        Dead,
    }

    public enum MoveDir
    {
        None,
        Up,
        Down,
        Left,
        Right,
    }

    public enum Layer
    {
        Monster = 6,
        Ground = 7,
        Block = 8,
    }

    public enum Scene
    {
        UnKnown,    // default
        LogIn,
        Lobby,      // Select character
        InGame,
    }

    public enum Sound
    {
        Bgm,
        EffectSound,
        MaxCount,           // Sound 종류 갯수(현재 Bgm, EffectSound 2개 => Sound enum의 제일 마지막 값인 MaxCount의 현 int값이 2이므로 Sound 종류 갯수를 표시해줌)
    }

    public enum UIEvent
    {
        Click,
        Drag,
    }

    public enum MouseEvent
    {
        Press,
        Click,
    }

    public enum CameraMode
    {
        QuaterView,
    }
}
