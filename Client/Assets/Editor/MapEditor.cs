using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.Tilemaps;
using System.IO;

#if UNITY_EDITOR            // #if : ���Ǻ� ��ó�� ������ ������ // if���� ���� �Ͽ����� �����ϵǰ� �ƴϸ� ���õ�
using UnityEditor;
#endif                      // unityEditor�϶��� ����ϰ� �ƴҶ� �����ϵ��� ��

public class MapEditor
{

#if UNITY_EDITOR

    // % : ctrl   # : shift  & : alt
    [MenuItem("Tools/GenerateMapCollisions %#&g")]     // ����Ƽ ������ �� �޴�â�� Tools�׸� ����� �����׸����� GenerateMap �־���
    private static void GenerateMap()    // GenerateMap�� ���
    {
        GameObject[] maps = Resources.LoadAll<GameObject>("Prefabs/Map");

        foreach (GameObject map in maps)
        {
            Tilemap tmbase = Utils.FindComponentinChild<Tilemap>(map, "Tilemap_Base", true);    // base ������ + collision üũ
            Tilemap tm = Utils.FindComponentinChild<Tilemap>(map, "Tilemap_Collision", true);

            using (StreamWriter writer = File.CreateText($"Assets/Resources/Map/{map.name + "_Collisions"}.txt"))
            {
                writer.WriteLine(tmbase.cellBounds.xMax);
                writer.WriteLine(tmbase.cellBounds.xMin);
                writer.WriteLine(tmbase.cellBounds.yMax);
                writer.WriteLine(tmbase.cellBounds.yMin);

                for (int y = tmbase.cellBounds.yMax; y >= tmbase.cellBounds.yMin; y--)      // �ؽ�Ʈ ���Ϸ� ������->�����ʾƷ� �� ���������� �ۼ��ϱ�
                {
                    for (int x = tmbase.cellBounds.xMin; x <= tmbase.cellBounds.xMax; x++)
                    {
                        TileBase tile = tm.GetTile(new Vector3Int(x, y, 0));    // collision ���� üũ
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
