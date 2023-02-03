using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DNA
{
    public class RoomStateTracker : MonoBehaviour
    {
        #region Inspector Variables
        [Header("Settings")]
        [SerializeField]
        private Vector2 roomStartBoundary = Vector2.zero;
        [SerializeField]
        private Vector2 roomEndBoundary = Vector2.zero;
        [SerializeField]
        [Tooltip("Number of scan points per unit")]
        [Min(0)]
        private int scanDensity = 1;

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

            // Update room state at impact point to overgrown state:
            states[impactIndex.x, impactIndex.y] = RoomState.OVERGROWN_FLOOR;

            // Update texture at impact point:
            textureGenerator.UpdateTexture(impactIndex.x, impactIndex.y, RoomState.OVERGROWN_FLOOR);
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
                x = Mathf.RoundToInt(position.x.RemapExclusive(roomStartBoundary.x, roomEndBoundary.x, 0, states.GetLength(0) - 1)),
                y = Mathf.RoundToInt(position.y.RemapExclusive(roomStartBoundary.y, roomEndBoundary.y, 0, states.GetLength(1) - 1))
            };
        }

        #endregion
    }
}