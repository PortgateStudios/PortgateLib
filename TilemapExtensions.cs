using UnityEngine;
using UTM = UnityEngine.Tilemaps;

namespace PortgateLib
{
	public static class TilemapExtensions
	{
		public static Vector2Int GetCoordinates(this UTM.Tilemap tilemap, Vector3 position)
		{
			return tilemap.WorldToCell(position).ToVector2IntXY();
		}
	}
}