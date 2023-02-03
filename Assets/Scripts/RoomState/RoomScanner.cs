using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DNA
{
    public class RoomScanner : MonoBehaviour
    {
        #region Inspector Variables
        [Header("Settings")]
        [SerializeField]
        private Vector2 scanStartPoint = Vector2.zero;
        [SerializeField]
        private Vector2 scanEndPoint = Vector2.zero;
        [SerializeField]
        [Tooltip("Number of scan points per unit")]
        [Min(1)]
        private int scanDensity = 1;
        [SerializeField]
        private LayerMask scanLayers;

        [Header("References")]
        [SerializeField]
        private RoomStateTracker tracker;
        [SerializeField]
        private RoomTextureGenerator textureGenerator;
        #endregion

        #region Internal Variables
        private int floorLayer;
        private int wallLayer;
        #endregion

        #region Setup

        private void Start()
        {
            // Get layer index by name:
            floorLayer = LayerMask.NameToLayer("Floor");
            wallLayer = LayerMask.NameToLayer("Wall");

            ScanRoom();
        }

        private void ScanRoom()
        {
            // Calculate room size:
            Vector2 roomSize = new Vector2(Mathf.Max(scanEndPoint.x - scanStartPoint.x, 0f), Mathf.Max(scanEndPoint.y - scanStartPoint.y, 0f));
            Debug.Log("Room Size: " + roomSize);

            // Calculate number of scans per row/column:
            Vector2Int numberOfScans = new Vector2Int(Mathf.CeilToInt(roomSize.x) * scanDensity, Mathf.CeilToInt(roomSize.y) * scanDensity);
            Debug.Log("Number of Scans: " + numberOfScans);

            // Create state array:
            RoomState[,] states = new RoomState[numberOfScans.x, numberOfScans.y];

            // Iterate over the room and scan every spot for floor or walls spot:
            for (int y = 0; y < numberOfScans.y; y++)
            {
                for (int x = 0; x < numberOfScans.x; x++)
                {
                    // Set state of scanned spot depending on the surface that was hit:
                    states[x, y] = Scan(x, y);
                }
            }

            // Output room texture to file:
            if (textureGenerator != null)
                textureGenerator.GenerateTexture(states);
        }

        private RoomState Scan(int x, int y)
        {
            // Calculate physical position of the scan inside the room:
            Vector3 scanOrigin = new Vector3(scanStartPoint.x + ((float)x * (1f / (float)scanDensity)), 100f, scanStartPoint.y + ((float)y * (1f / (float)scanDensity)));

            // Raycast at position to be scanned:
            Ray ray = new Ray(scanOrigin, Vector3.down);
            if (Physics.Raycast(ray, out RaycastHit hitInfo, 1000f, scanLayers))
            {
                // Detect which layer was hit (floor or wall):
                int hitLayer = hitInfo.collider.gameObject.layer;

                // Floor was hit:
                if (hitLayer == floorLayer)
                    return RoomState.CLEAN_FLOOR;

                // Wall was hit:
                if (hitLayer == wallLayer)
                    return RoomState.WALL;

                // No matching layer was hit:
                return RoomState.EMPTY;
            }
            else
            {
                // No object was hit:
                return RoomState.EMPTY;
            }
        }

        #endregion
    }
}