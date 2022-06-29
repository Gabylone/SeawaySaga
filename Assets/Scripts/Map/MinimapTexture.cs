using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MinimapTexture : MonoBehaviour {

    public static MinimapTexture Instance;

    public Image targetImage;
    public Color color_VisitedSea;
    public Color color_DiscoveredVoid;
	public Color color_UnvisitedSea;
    public Color color_InRange;


    public Image gridImage;

    private void Awake()
    {
        Instance = this;
    }

	public void UpdateBackgroundImage () {

		Texture2D texture = new Texture2D (MapGenerator.Instance.GetMapHorizontalScale, MapGenerator.Instance.GetMapVerticalScale);

		texture.filterMode = FilterMode.Point;
		texture.anisoLevel = 0;
		texture.mipMapBias = 0;
		texture.wrapMode = TextureWrapMode.Clamp;

		texture.Reinitialize (MapGenerator.Instance.GetMapHorizontalScale, MapGenerator.Instance.GetMapVerticalScale);

		for (int x = 0; x < MapGenerator.Instance.GetMapHorizontalScale; x++) {
			for (int y = 0; y < MapGenerator.Instance.GetMapVerticalScale; y++) {

				Chunk chunk = Chunk.GetChunk (new Coords (x, y));

				switch (chunk.state) {
                case ChunkState.UndiscoveredVoid:
				case ChunkState.UndiscoveredSea:
				case ChunkState.UndiscoveredIsland:
						texture.SetPixel (x, y, color_UnvisitedSea);
					break;
				case ChunkState.DiscoveredSea:
				case ChunkState.DiscoveredIsland:
				case ChunkState.VisitedIsland:
						texture.SetPixel (x, y, color_VisitedSea);
						//texture.SetPixel(x, y, color_DiscoveredVoid);
					break;
					case ChunkState.DiscoveredVoid:
						texture.SetPixel(x, y, color_DiscoveredVoid);
						break;
					default:
					break;
				}
			}
		}

        int currentShipRange = DisplayMinimap.Instance.GetCurrentShipRange;

        for (int x = -currentShipRange; x <= currentShipRange; x++)
        {
            for (int y = -currentShipRange; y <= currentShipRange; y++)
            {
                Coords c = Boats.Instance.playerBoatInfo.coords + new Coords(x, y);
                if (c.OutOfMap())
                    continue;

				if (Chunk.GetChunk(c).state == ChunkState.DiscoveredVoid)
					continue;
                //Debug.Log("eh ?");
                texture.SetPixel(c.x, c.y, color_InRange);
            }
        }

        texture.Apply ();

		targetImage.sprite = Sprite.Create ( texture, new Rect (0, 0, MapGenerator.Instance.GetMapHorizontalScale,  MapGenerator.Instance.GetMapVerticalScale) , Vector2.one * 0.5f );

	}
}
