using System.Collections;
using System.Collections.Generic;
using System.IO;
using Unity.Mathematics;
using UnityEngine;

namespace DNA
{
    public class RoomTextureGenerator : MonoBehaviour
    {
        #region Internal Variables
        private Texture2D texture;
        #endregion

        public void GenerateTexture(RoomState[,] roomState)
        {
            // Create empty texture with dimensions matching the state array:
            texture = new Texture2D(roomState.GetLength(0), roomState.GetLength(1), TextureFormat.RGB24, false);

            // Iterate over every pixel in texture:
            for (int y = 0; y < texture.height; y++)
            {
                for (int x = 0; x < texture.width; x++)
                {
                    // Set pixel color depending on room state:
                    texture.SetPixel(x, y, CalculateStateColor(roomState[x, y]));
                }
            }

            // Write texture to file for debug purposes:
            WriteToFile();
        }

        public void UpdateTexture(int x, int y, RoomState state)
        {
            // Update pixel at given position:
            texture.SetPixel(x, y, CalculateStateColor(state));

            // Write texture to file for debug purposes:
            /*WriteToFile();*/
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
    }
}