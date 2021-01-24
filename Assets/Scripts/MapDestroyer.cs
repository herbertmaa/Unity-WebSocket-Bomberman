using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class MapDestroyer : MonoBehaviour
{
    public Tilemap tilemap;

    public Tile wallTile;
    public Tile borderTile;
    public Tile destructibleTile;

    public GameObject explosionPrefab;

    public void Explode(Vector2 pos)
    {

        Vector3Int originalCell = tilemap.WorldToCell(pos);
        ExplodeBlocks(originalCell);
        if (ExplodeBlocks(originalCell + new Vector3Int(0, 1, 0)))
        {
            ExplodeBlocks(originalCell + new Vector3Int(0, 2, 0));
        }
        if(ExplodeBlocks(originalCell + new Vector3Int(0, -1, 0)))
        {
            ExplodeBlocks(originalCell + new Vector3Int(0, -2, 0));
        }
        if(ExplodeBlocks(originalCell + new Vector3Int(1, 0, 0)))
        {
            ExplodeBlocks(originalCell + new Vector3Int(2, 0, 0));
        }
        if(ExplodeBlocks(originalCell + new Vector3Int(-1, 0, 0)))
        {
            ExplodeBlocks(originalCell + new Vector3Int(-2, 0, 0));
        }

    }

    bool ExplodeBlocks (Vector3Int cell)
    {
        Tile tile =tilemap.GetTile<Tile>(cell);
        if (tile == wallTile || tile == borderTile)
        {
            return false;
        }
        Vector3 pos = tilemap.GetCellCenterWorld(cell);
        Instantiate(explosionPrefab, pos, Quaternion.identity);
        if (tile == destructibleTile)
        {
            tilemap.SetTile(cell, null);
            return false;
        }

        
        return true;
    }
}
