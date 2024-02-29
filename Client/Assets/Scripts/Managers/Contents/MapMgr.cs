using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.ExceptionServices;
using UnityEngine;
using UnityEngine.Rendering.VirtualTexturing;

public class MapMgr
{
    // 맵 로드 삭제 및 맵 로드 시 collision 파일을 추출하여 이동가능 영역 구분
    
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

        int y = MaxY - cellPos.y;   // _collsion[y, x]의 위치를 cellPos와 맞게 계산
        int x = cellPos.x - MinX;   

        return !_collsion[y, x];    // 1이면 collision이므로 이동불가, 0이면 이동가능
    }

    public void LoadMap(int mapId)
    {
        DestroyMap();

        string mapName = "Map_" + mapId.ToString("000");    //ToString("000") : mapId가 1일 경우 001, 21일경우 021 형태로 만들어줌
        GameObject go = Managers.resourceMgr.Instantiate($"Map/{mapName}");
        go.name = "Map";

        GameObject collision = Utils.FindComponentinChild(go, "Tilemap_Collision", true);
        if (collision != null)
            collision.SetActive(false);

        CurrentGrid = go.GetComponent<Grid>();

        // Collision 관련 파일 
        TextAsset txt = Managers.resourceMgr.Load<TextAsset>($"Map/{mapName}_Collisions");
        StringReader reader = new StringReader(txt.text);

        MaxX = int.Parse(reader.ReadLine());    // txt의 맨 윗줄 읽어서 int로 파싱하여 MaxX에 저장
        MinX = int.Parse(reader.ReadLine());    // txt의 그 다음 줄 읽어서 int로 파싱하여 MinX에 저장
        MaxY = int.Parse(reader.ReadLine());    // ReadLine이 WriteLine처럼 한줄씩 읽고 넘겨줌
        MinY = int.Parse(reader.ReadLine());

        int xCount = MaxX - MinX + 1;   // + 1은 걍 세보셈 보면 1 딸림
        int yCount = MaxY - MinY + 1;   // 대충 Max Min 둘다 양쪽이 포함되어서 그럼 귀찮으니 걍 세보셈

        _collsion = new bool[yCount, xCount];   // 좌측위부터 오른쪽으로 순서대로 진행하기 편하게 (y,x)로 넣어줌

        for (int y = 0; y < yCount; y++)
        {
            string line = reader.ReadLine();
            for (int x = 0; x < xCount; x++) 
            {
                _collsion[y, x] = line[x] == '1' ? true : false;    //값이 1이면 true,아니면 false 넣기
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
