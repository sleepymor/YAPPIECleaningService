using UnityEngine;
using UnityEngine.Tilemaps;

[CreateAssetMenu(fileName = "TrunkColliderTile", menuName = "Tiles/Trunk Collider Tile")]
public class TrunkColliderTile : TileBase
{
    public Sprite treeSprite;

    public override void GetTileData(Vector3Int position, ITilemap tilemap, ref TileData tileData)
    {
        tileData.sprite = treeSprite;
        tileData.colliderType = Tile.ColliderType.Sprite;
        tileData.flags = TileFlags.LockAll;
    }
}
