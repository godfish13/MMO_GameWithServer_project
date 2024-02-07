using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.Tilemaps;
using System.IO;

#if UNITY_EDITOR            // #if : 조건부 전처리 컴파일 지시자 // if문의 조건 하에서만 컴파일되고 아니면 무시됨
using UnityEditor;
#endif                      // unityEditor일때만 사용하고 아닐땐 무시하도록 함

public class MapEditor
{

#if UNITY_EDITOR

    // % : ctrl   # : shift  & : alt
    [MenuItem("Tools/GenerateMapCollisions %#&g")]     // 유니티 에디터 위 메뉴창에 Tools항목 만들고 하위항목으로 GenerateMap 넣어줌
    private static void GenerateMap()    // GenerateMap의 기능 HelloWorld
    {
        GameObject[] maps = Resources.LoadAll<GameObject>("Prefabs/Map");

        foreach (GameObject map in maps)
        {
            Tilemap tm = Utils.FindComponentinChild<Tilemap>(map, "Tilemap_Collision", true);

            using (StreamWriter writer = File.CreateText($"Assets/Resources/Map/{map.name + "_Collisions"}.txt"))
            {
                writer.WriteLine(tm.cellBounds.xMax);
                writer.WriteLine(tm.cellBounds.xMin);
                writer.WriteLine(tm.cellBounds.yMax);
                writer.WriteLine(tm.cellBounds.yMin);

                for (int y = tm.cellBounds.yMax; y >= tm.cellBounds.yMin; y--)      // 텍스트 파일로 왼쪽위->오른쪽아래 로 내려가도록 작성하기
                {
                    for (int x = tm.cellBounds.xMin; x <= tm.cellBounds.xMax; x++)
                    {
                        TileBase tile = tm.GetTile(new Vector3Int(x, y, 0));
                        if (tile != null)
                            writer.Write("1");
                        else
                            writer.Write("0");
                    }
                    writer.WriteLine();
                }
            }
        }        
    }

#endif

}
