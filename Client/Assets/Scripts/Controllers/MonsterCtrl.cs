using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Define;

public class MonsterCtrl : CreatureCtrl
{
    // Monster류 gameobject 부착

    protected override void Init()
    {
        base.Init();
    }

    protected override void UpdateCtrl()
    {
        GetDirInput();
        base.UpdateCtrl();
    }

    private void LateUpdate()
    {
        Camera.main.transform.position = new Vector3(transform.position.x, transform.position.y, -10);
    }

    void GetDirInput()  // 키 입력 시 이동방향과 도착지점 설정
    {
        if (State == CreatureState.Moving)  // 이동중이 아니면 입력을 못받게 설정
            return;

        Vector3Int destPos = _cellPos;

        if (Input.GetKey(KeyCode.W))
        {
            Dir = MoveDir.Up;
            destPos += Vector3Int.up;
        }
        else if (Input.GetKey(KeyCode.S))
        {
            Dir = MoveDir.Down;
            destPos += Vector3Int.down;
        }
        else if (Input.GetKey(KeyCode.A))
        {
            Dir = MoveDir.Left;
            destPos += Vector3Int.left;
        }
        else if (Input.GetKey(KeyCode.D))
        {
            Dir = MoveDir.Right;
            destPos += Vector3Int.right;
        }
        else
        {
            Dir = MoveDir.None;
        }

        if (Managers.mapMgr.CanGo(destPos))     // 이동 가능한 좌표인지 체크 후 이동
        {
            _cellPos = destPos;
        }
        State = CreatureState.Moving;   // 벽에 막혀있는 Arrived 상태에서도 애니메이션 업데이트 가능하도록 이동키 입력받으면 일단 Moving상태 지정
    }
}
