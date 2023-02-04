using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DNA
{
    public class RoomStateTracker : MonoBehaviour
    {
        #region Inspector Variables
        [Header("Scan")]
        [SerializeField]
        private Vector2 roomStartBoundary = Vector2.zero;
        [SerializeField]
        private Vector2 roomEndBoundary = Vector2.zero;
        [SerializeField]
        [Tooltip("Number of scan points per unit")]
        [Min(0)]
        private int scanDensity = 1;

        [Header("Gameplay")]
        [SerializeField]
        [Min(1)]
        private int playerImpactRadius = 1;

        [Header("References")]
        [SerializeField]
        private RoomScanner scanner;
        [SerializeField]
        private RoomTextureGenerator textureGenerator;
        #endregion

        #region Internal Variables
        private RoomState[,] states;
        private int availableFloorSpots = 0;
        private int overgrownSpots = 0;
        private float overgrownPercentage = 0f;
        private List<Vector2Int> circleOffsets;
        #endregion

        #region Properties
        public Vector2 RoomStartBoundary { get { return roomStartBoundary; }}
        public Vector2 RoomEndBoundary { get { return roomEndBoundary; }}
        public int StateDensity { get { return scanDensity; } }
        #endregion

        #region Setup

        private void Start()
        {
            // Initialize states array by scanning the room for floor and wall areas:
            states = scanner.ScanRoom();

            // Count all spots that are occupied by floor area:
            CountAvailableFloorSpots();

            // Generate initial floor texture:
            GenerateTexture();

            // Pre-calculate offset index values to be accessed to create circular pattern in the array:
            CalculateCircleOffsets();
        }

        private void CalculateCircleOffsets()
        {
            // Pre-calculate offset index values to be accessed to create circular pattern in the array:
            circleOffsets = new List<Vector2Int>();
            int threshold = playerImpactRadius * playerImpactRadius;
            Vector2Int vec = new Vector2Int();
            for (int i = 0; i < playerImpactRadius; i++)
            {
                vec.x = i;
                for (int j = 0; j < playerImpactRadius; j++)
                {
                    if (i * i + j * j < threshold)
                    {
                        vec.y = j;
                        circleOffsets.Add(vec); vec.x = -vec.x; // i,j
                        circleOffsets.Add(vec); vec.y = -vec.y; // -i,j
                        circleOffsets.Add(vec); vec.x = -vec.x; // -i,-j
                        circleOffsets.Add(vec);                 // i,-j
                    }
                }
            }
        }

        #endregion

        #region Update

        private void Update()
        {
            // Count overgrown spots and calculate percentage:
            CountOvergrownSpots();
            CalculateOvergrownPercentage();
        }

        #endregion

        #region Player Impact

        public void ApplyPlayerImpact(Vector3 impactPosition, float impactRadius)
        {
            // Remap impact position to array index:
            Vector2Int impactIndex = PositionToGrid(impactPosition);

            // Mark impact spot as overgrown:
            OvergrowSpot(impactIndex.x, impactIndex.y);

            // Iterate all spots that are inside the given circle radius around the impact:
            foreach (Vector2Int offset in circleOffsets)
            {
                // Calculate index by using pre-calculated offset:
                Vector2Int targetIndex = new Vector2Int(impactIndex.x + offset.x, impactIndex.y + offset.y);

                // Skip to next loop if point is outside array boundaries:
                if (targetIndex.x < 0 || targetIndex.x >= states.GetLength(0) || targetIndex.y < 0 || targetIndex.y >= states.GetLength(1))
                    continue;

                // Apply overgrowth:
                OvergrowSpot(targetIndex.x, targetIndex.y);
            }

            textureGenerator.WriteToFile();
        }

        private void OvergrowSpot(int x, int y)
        {
            // Update room state at impact point to overgrown state:
            states[x, y] = RoomState.OVERGROWN_FLOOR;

            // Update texture at impact point:
            textureGenerator.UpdateTexture(x, y, RoomState.OVERGROWN_FLOOR);
        }

        #endregion

        #region Counting

        private void CountAvailableFloorSpots()
        {
            availableFloorSpots = 0;

            // Iterate all spots:
            for (int y = 0; y < states.GetLength(1); y++)
            {
                for (int x = 0; x < states.GetLength(0); x++)
                {
                    // Count all spots that are occupied by floor area:
                    if (states[x, y] == RoomState.CLEAN_FLOOR || states[x, y] == RoomState.OVERGROWN_FLOOR)
                        availableFloorSpots++;
                }
            }
        }

        private void CountOvergrownSpots()
        {
            overgrownSpots = 0;

            // Iterate all spots:
            for (int y = 0; y < states.GetLength(1); y++)
            {
                for (int x = 0; x < states.GetLength(0); x++)
                {
                    // Count all spots that are occupied by floor area:
                    if (states[x, y] == RoomState.OVERGROWN_FLOOR)
                        overgrownSpots++;
                }
            }
        }

        #endregion

        #region Overgrowth Percentage

        private void CalculateOvergrownPercentage()
        {
            // Calculate percentage:
            overgrownPercentage = (float)overgrownSpots / (float)availableFloorSpots;

            // Display in UI:
            // TODO
        }

        #endregion

        #region Texture

        private void GenerateTexture()
        {
            if (textureGenerator == null)
                return;

            textureGenerator.GenerateTexture(states);
        }

        #endregion

        #region Helper Methods

        public Vector2Int PositionToGrid(Vector3 position)
        {
            // Remap physical position inside room boundaries to state array:
            return new Vector2Int
            {
                x = Mathf.RoundToInt(position.x.RemapExclusive(roomStartBoundary.x, roomEndBoundary.x, states.GetLength(0) - 1, 0)),
                y = Mathf.RoundToInt(position.z.RemapExclusive(roomStartBoundary.y, roomEndBoundary.y, states.GetLength(1) - 1, 0))
            };
        }

        #endregion
    }
}