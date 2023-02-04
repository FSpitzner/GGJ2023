using System.Collections;
using System.Collections.Generic;
using System.IO;
using Unity.Collections;
using Unity.Mathematics;
using UnityEngine;

namespace DNA
{
    public class RoomTextureGenerator : MonoBehaviour
    {
        #region Inspector Variables
        [SerializeField]
        private Material floorMaterial = null;
        #endregion

        #region Internal Variables
        private Texture2D texture;
        #endregion

        #region Properties
        public Color[] Pixels
        {
            get
            {
                return texture.GetPixels();
            }
            set
            {
                texture.SetPixels(value);
            }
        }
        #endregion

        public void GenerateTexture(NativeArray<RoomState> states, int2 dimensions)
        {
            // Create empty texture with dimensions matching the state array:
            texture = new Texture2D(dimensions.x, dimensions.y, TextureFormat.RGB24, false);

            // Iterate over every pixel in texture:
            for (int y = 0; y < texture.height; y++)
            {
                for (int x = 0; x < texture.width; x++)
                {
                    // Set pixel color depending on room state:
                    texture.SetPixel(x, y, CalculateStateColor(states[RoomStateTracker.GetIndex(x, y, dimensions.x)]));
                }
            }

            // Write texture to file for debug purposes:
            WriteToFile();
        }

        private Color CalculateStateColor(RoomState state)
        {
            // Red channel: 0 = floor; 255 = wall or nothing
            // Green channel: 0 = clean floor; 255 = overgrown floor

            switch (state)
            {
                case RoomState.EMPTY:
                    return Color.red;
                case RoomState.CLEAN_FLOOR:
                    return Color.black;
                case RoomState.OVERGROWN_FLOOR:
                    return Color.green;
                case RoomState.WALL:
                    return Color.red;
                default:
                    return Color.red;
            }
        }

        public void WriteToFile()
        {
            if (texture == null)
                return;

            byte[] textureBytes = texture.EncodeToPNG();
            string filePath = string.Format("{0}/{1}/{2}{3}", Application.dataPath, "TextureOutput", "test", ".png");
            File.WriteAllBytes(filePath, textureBytes);
        }

        #region Material

        public void UpdateFloorMaterial()
        {
            if (floorMaterial == null)
                return;

            //floorMaterial.SetTexture("_OvergrowthMask", texture);
        }

        #endregion
    }
}