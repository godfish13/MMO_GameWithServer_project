using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Define;

public class ArrowCtrl : CreatureCtrl
{
    protected override void Init()
    {
        // 화살 방향 설정
        switch (_lastDir)
        {
            case MoveDir.Up:
                transform.rotation = Quaternion.Euler(0, 0, 0);
                break;
            case MoveDir.Down:
                transform.rotation = Quaternion.Euler(0, 0, 180);
                break;
            case MoveDir.Right:
                transform.rotation = Quaternion.Euler(0, 0, -90);
                break;
            case MoveDir.Left:
                transform.rotation = Quaternion.Euler(0, 0, 90);
                break;
        }

        base.Init();
    }

    protected override void UpdateAnim()
    {
        // No Animation
    }

    protected override void UpdateIdle()
    {
        if (_dir != MoveDir.None)
        {
            Vector3Int destPos = CellPos;

            switch (_dir)
            {
                case MoveDir.Up:
                    destPos += Vector3Int.up;
                    break;
                case MoveDir.Down:
                    destPos += Vector3Int.down;
                    break;
                case MoveDir.Right:
                    destPos += Vector3Int.right;
                    break;
                case MoveDir.Left:
                    destPos += Vector3Int.left;
                    break;
            }

            State = CreatureState.Moving;   // 벽에 막혀있는 Arrived 상태에서도 애니메이션 업데이트 가능하도록 이동키 입력받으면 일단 Moving상태 지정
            if (Managers.mapMgr.CanGo(destPos))     // 이동 가능한 좌표인지 체크 후 이동
            {
                GameObject go = Managers.objectMgr.SearchPos(destPos);
                if (go == null)
                {
                    CellPos = destPos;
                }
                else
                {
                    // 피격판정
                    Debug.Log(go.name);

                    CreatureCtrl cc = go.GetComponent<CreatureCtrl>();
                    if (cc != null)
                    {
                        cc.OnDamaged();
                    }

                    Managers.resourceMgr.Destroy(gameObject);   // 화살 삭제
                }
            }
            else
            {
                Managers.resourceMgr.Destroy(gameObject);
            }
        }
    }
}
