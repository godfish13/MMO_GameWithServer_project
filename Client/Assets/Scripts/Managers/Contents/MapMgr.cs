using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.ExceptionServices;
using UnityEngine;
using UnityEngine.Rendering.VirtualTexturing;

public class MapMgr
{
    // �� �ε� ���� �� �� �ε� �� collision ������ �����Ͽ� �̵����� ���� ����
    
    public Grid CurrentGrid { get; private set; }

    public int MaxX { get; set; }
    public int MinX { get; set; }
    public int MaxY { get; set; }
    public int MinY { get; set; }

    bool[,] _collsion;

    public bool CanGo(Vector3Int cellPos)
    {
        if (cellPos.x < MinX || cellPos.x > MaxX || cellPos.y < MinY || cellPos.y > MaxY)
            return false;

        int y = MaxY - cellPos.y;   // _collsion[y, x]�� ��ġ�� cellPos�� �°� ���
        int x = cellPos.x - MinX;   

        return !_collsion[y, x];    // 1�̸� collision�̹Ƿ� �̵��Ұ�, 0�̸� �̵�����
    }

    public void LoadMap(int mapId)
    {
        DestroyMap();

        string mapName = "Map_" + mapId.ToString("000");    //ToString("000") : mapId�� 1�� ��� 001, 21�ϰ�� 021 ���·� �������
        GameObject go = Managers.resourceMgr.Instantiate($"Map/{mapName}");
        go.name = "Map";

        GameObject collision = Utils.FindComponentinChild(go, "Tilemap_Collision", true);
        if (collision != null)
            collision.SetActive(false);

        CurrentGrid = go.GetComponent<Grid>();

        // Collision ���� ���� 
        TextAsset txt = Managers.resourceMgr.Load<TextAsset>($"Map/{mapName}_Collisions");
        StringReader reader = new StringReader(txt.text);

        MaxX = int.Parse(reader.ReadLine());    // txt�� �� ���� �о int�� �Ľ��Ͽ� MaxX�� ����
        MinX = int.Parse(reader.ReadLine());    // txt�� �� ���� �� �о int�� �Ľ��Ͽ� MinX�� ����
        MaxY = int.Parse(reader.ReadLine());    // ReadLine�� WriteLineó�� ���پ� �а� �Ѱ���
        MinY = int.Parse(reader.ReadLine());

        int xCount = MaxX - MinX + 1;   // + 1�� �� ������ ���� 1 ����
        int yCount = MaxY - MinY + 1;   // ���� Max Min �Ѵ� ������ ���ԵǾ �׷� �������� �� ������

        _collsion = new bool[yCount, xCount];   // ���������� ���������� ������� �����ϱ� ���ϰ� (y,x)�� �־���

        for (int y = 0; y < yCount; y++)
        {
            string line = reader.ReadLine();
            for (int x = 0; x < xCount; x++) 
            {
                _collsion[y, x] = line[x] == '1' ? true : false;    //���� 1�̸� true,�ƴϸ� false �ֱ�
            }
        }
    }

    public void DestroyMap()
    {
        GameObject map = GameObject.Find("Map");

        if (map != null)
        {
            GameObject.Destroy(map);
            CurrentGrid = null;
        }
    }
}
