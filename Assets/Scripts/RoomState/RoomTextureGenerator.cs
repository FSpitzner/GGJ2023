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

        public Texture2D GenerateTexture(RoomState[,] roomState)
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

            // Write texture to file:
            WriteToFile();

            return null;
        }

        private Color CalculateStateColor(RoomState state)
        {
            switch (state)
            {
                case RoomState.EMPTY:
                    return Color.black;
                case RoomState.CLEAN_FLOOR:
                    return Color.black;
                case RoomState.OVERGROWN_FLOOR:
                    return Color.white;
                case RoomState.WALL:
                    return Color.red;
                default:
                    return Color.black;
            }
        }

        private void WriteToFile()
        {
            if (texture == null)
                return;

            byte[] textureBytes = texture.EncodeToPNG();
            string filePath = string.Format("{0}/{1}/{2}{3}", Application.dataPath, "TextureOutput", "test", ".png");
            File.WriteAllBytes(filePath, textureBytes);
        }
    }
}