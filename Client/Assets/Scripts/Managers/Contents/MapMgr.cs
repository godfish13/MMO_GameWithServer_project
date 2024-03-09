using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.ExceptionServices;
using UnityEngine;
using UnityEngine.Rendering.VirtualTexturing;

public struct Pos   // 왼쪽 위 -> 오른쪽 아래 순서이기에 y, x로 지정
{
    public Pos(int y, int x) { Y = y; X = x; }
    public int Y;
    public int X;
}

public struct PQNode : IComparable<PQNode>
{
    public int F;
    public int G;
    public int Y;
    public int X;

    public int CompareTo(PQNode other)
    {
        if (F == other.F)
            return 0;
        return (F < other.F) ? 1 : -1;
    }
}

public class MapMgr
{
    // 맵 로드 삭제 및 맵 로드 시 collision 파일을 추출하여 이동가능 영역 구분
    
    public Grid CurrentGrid { get; private set; }

    public int MaxX { get; set; }
    public int MinX { get; set; }
    public int MaxY { get; set; }
    public int MinY { get; set; }

    public int SizeX { get { return MaxX - MinX + 1; } }    // 맵 x 좌우 크기
    public int SizeY { get { return MaxY - MinY + 1; } }    // 맵 y 상하 크기

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

    #region A* PathFinding

    // U D L R
    int[] _deltaY = new int[] { 1, -1, 0, 0 };
    int[] _deltaX = new int[] { 0, 0, -1, 1 };
    int[] _cost = new int[] { 10, 10, 10, 10 };

    public List<Vector3Int> FindPath(Vector3Int startCellPos, Vector3Int destCellPos, bool ignoreDestCollision = false) // ignoreDestCollsion : 목표지점에 Collsion있는것 무시할지말지 결정
    {
        List<Pos> path = new List<Pos>();
        // 점수 매기기
        // F = G + H
        // F = 최종 점수 (작을 수록 좋음, 경로에 따라 달라짐)
        // G = 시작점에서 해당 좌표까지 이동하는데 드는 비용 (작을 수록 좋음, 경로에 따라 달라짐)
        // H = 목적지에서 얼마나 가까운지 (작을 수록 좋음, 고정) (Collsion과 관계없이 목표물까지의 상하좌우 거리)

        // (y, x) 이미 방문했는지 여부 (방문 = closed 상태)
        bool[,] closed = new bool[SizeY, SizeX]; // CloseList

        // (y, x) 가는 길을 한 번이라도 발견했는지
        // 발견X => MaxValue
        // 발견O => F = G + H
        int[,] open = new int[SizeY, SizeX]; // OpenList
        for (int y = 0; y < SizeY; y++)
            for (int x = 0; x < SizeX; x++)
                open[y, x] = Int32.MaxValue;

        Pos[,] parent = new Pos[SizeY, SizeX];

        // 오픈리스트에 있는 정보들 중에서, 가장 좋은 후보를 빠르게 뽑아오기 위한 도구
        PriorityQueue<PQNode> pq = new PriorityQueue<PQNode>();

        // CellPos -> ArrayPos
        Pos pos = Cell2Pos(startCellPos);
        Pos dest = Cell2Pos(destCellPos);

        // 시작점 발견 (예약 진행)
        open[pos.Y, pos.X] = 10 * (Math.Abs(dest.Y - pos.Y) + Math.Abs(dest.X - pos.X));
        pq.Push(new PQNode() { F = 10 * (Math.Abs(dest.Y - pos.Y) + Math.Abs(dest.X - pos.X)), G = 0, Y = pos.Y, X = pos.X });
        parent[pos.Y, pos.X] = new Pos(pos.Y, pos.X);

        while (pq.Count > 0)
        {
            // 제일 좋은 후보를 찾는다
            PQNode node = pq.Pop();
            // 동일한 좌표를 여러 경로로 찾아서, 더 빠른 경로로 인해서 이미 방문(closed)된 경우 스킵
            if (closed[node.Y, node.X])
                continue;

            // 방문한다
            closed[node.Y, node.X] = true;
            // 목적지 도착했으면 바로 종료
            if (node.Y == dest.Y && node.X == dest.X)
                break;

            // 상하좌우 등 이동할 수 있는 좌표인지 확인해서 예약(open)한다
            for (int i = 0; i < _deltaY.Length; i++)
            {
                Pos next = new Pos(node.Y + _deltaY[i], node.X + _deltaX[i]);

                // 유효 범위를 벗어났으면 스킵
                // 벽으로 막혀서 갈 수 없으면 스킵
                if (!ignoreDestCollision || next.Y != dest.Y || next.X != dest.X)
                {
                    if (CanGo(Pos2Cell(next)) == false) // CellPos
                        continue;
                }

                // 이미 방문한 곳이면 스킵
                if (closed[next.Y, next.X])
                    continue;

                // 비용 계산
                int g = 10;// node.G + _cost[i]; // 상하좌우 모두 같은 비용으로 움직이므로 그냥 0으로 설정
                int h = 10 * ((dest.Y - next.Y) * (dest.Y - next.Y) + (dest.X - next.X) * (dest.X - next.X));
                // 다른 경로에서 더 빠른 길 이미 찾았으면 스킵
                if (open[next.Y, next.X] < g + h)
                    continue;

                // 예약 진행
                open[dest.Y, dest.X] = g + h;
                pq.Push(new PQNode() { F = g + h, G = g, Y = next.Y, X = next.X });
                parent[next.Y, next.X] = new Pos(node.Y, node.X);
            }
        }

        return CalcCellPathFromParent(parent, dest);
    }

    List<Vector3Int> CalcCellPathFromParent(Pos[,] parent, Pos dest)
    {
        List<Vector3Int> cells = new List<Vector3Int>();

        int y = dest.Y;
        int x = dest.X;
        while (parent[y, x].Y != y || parent[y, x].X != x)
        {
            cells.Add(Pos2Cell(new Pos(y, x)));
            Pos pos = parent[y, x];
            y = pos.Y;
            x = pos.X;
        }
        cells.Add(Pos2Cell(new Pos(y, x)));
        cells.Reverse();

        return cells;
    }

    Pos Cell2Pos(Vector3Int cell)
    {
        // CellPos -> ArrayPos
        return new Pos(MaxY - cell.y, cell.x - MinX);
    }

    Vector3Int Pos2Cell(Pos pos)
    {
        // ArrayPos -> CellPos
        return new Vector3Int(pos.X + MinX, MaxY - pos.Y, 0);
    }

    #endregion
}
