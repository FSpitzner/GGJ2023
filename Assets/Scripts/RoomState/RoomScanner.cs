using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using Unity.Mathematics;
using UnityEngine;

namespace DNA
{
    public class RoomScanner : MonoBehaviour
    {
        #region Inspector Variables
        
        [Header("Settings")]
        [SerializeField]
        private LayerMask scanLayers;

        [Header("References")]
        [SerializeField]
        private RoomStateTracker tracker;
        #endregion

        #region Internal Variables
        private int floorLayer;
        private int wallLayer;

        Vector2 scanStartPoint;
        Vector2 scanEndPoint;
        int scanDensity;
        #endregion

        #region Scan

        public NativeArray<RoomState> ScanRoom(out int2 dimensions)
        {
            // Get layer index by name:
            floorLayer = LayerMask.NameToLayer("Floor");
            wallLayer = LayerMask.NameToLayer("Wall");

            // Get scan settings from RoomStateTracker:
            scanStartPoint = tracker.RoomStartBoundary;
            scanEndPoint = tracker.RoomEndBoundary;
            scanDensity = tracker.StateDensity;

            // Calculate room size:
            Vector2 roomSize = new Vector2(Mathf.Max(scanEndPoint.x - scanStartPoint.x, 0f), Mathf.Max(scanEndPoint.y - scanStartPoint.y, 0f));

            // Calculate number of scans per row/column:
            int2 numberOfScans = new int2(Mathf.CeilToInt(roomSize.x) * scanDensity, Mathf.CeilToInt(roomSize.y) * scanDensity);
            dimensions = numberOfScans;

            // Create state array:
            NativeArray<RoomState> states = new NativeArray<RoomState>(numberOfScans.x * numberOfScans.y, Allocator.Persistent);

            // Iterate over the room and scan every spot for floor or walls spot:
            for (int y = 0; y < numberOfScans.y; y++)
            {
                for (int x = 0; x < numberOfScans.x; x++)
                {
                    // Set state of scanned spot depending on the surface that was hit:
                    states[RoomStateTracker.GetIndex(x, y, dimensions.x)] = Scan(x, y);
                }
            }

            return states;
        }

        private RoomState Scan(int x, int y)
        {
            // Calculate physical position of the scan inside the room:
            /*Vector3 scanOrigin = new Vector3(
                scanStartPoint.x + ((float)x * (1f / (float)scanDensity)),
                100f,
                scanStartPoint.y + ((float)y * (1f / (float)scanDensity)));*/
            Vector3 scanOrigin = new Vector3(
                scanEndPoint.x - ((float)x * (1f / (float)scanDensity)),
                100f,
                scanEndPoint.y - ((float)y * (1f / (float)scanDensity)));

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