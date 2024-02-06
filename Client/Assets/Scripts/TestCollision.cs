using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class TestCollision : MonoBehaviour
{
    public Tilemap _tilemap;
    public TileBase _tile;

    private void Start()
    {
        _tilemap = gameObject.GetComponent<Tilemap>();
    }

    void Update()
    {
        List<Vector3Int> blocked = new List<Vector3Int>();

        foreach (Vector3Int pos in _tilemap.cellBounds.allPositionsWithin)
        {
            TileBase tile = _tilemap.GetTile(pos);
            if (tile != null)
                blocked.Add(pos);
        }
    }
}
