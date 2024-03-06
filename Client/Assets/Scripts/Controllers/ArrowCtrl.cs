using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Define;

public class ArrowCtrl : CreatureCtrl
{
    protected override void Init()
    {
        // ȭ�� ���� ����
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

            State = CreatureState.Moving;   // ���� �����ִ� Arrived ���¿����� �ִϸ��̼� ������Ʈ �����ϵ��� �̵�Ű �Է¹����� �ϴ� Moving���� ����
            if (Managers.mapMgr.CanGo(destPos))     // �̵� ������ ��ǥ���� üũ �� �̵�
            {
                GameObject go = Managers.objectMgr.SearchPos(destPos);
                if (go == null)
                {
                    CellPos = destPos;
                }
                else
                {
                    // �ǰ�����
                    Debug.Log(go.name);

                    CreatureCtrl cc = go.GetComponent<CreatureCtrl>();
                    if (cc != null)
                    {
                        cc.OnDamaged();
                    }

                    Managers.resourceMgr.Destroy(gameObject);   // ȭ�� ����
                }
            }
            else
            {
                Managers.resourceMgr.Destroy(gameObject);
            }
        }
    }
}
