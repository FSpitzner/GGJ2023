using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using Unity.Jobs;
using Unity.Mathematics;
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
        private RoomScanner scanner = null;
        [SerializeField]
        private RoomTextureGenerator textureGenerator = null;
        [SerializeField]
        private PercentageBar percentageBar = null;
        #endregion

        #region Internal Variables
        private NativeArray<RoomState> states;
        private int2 dimensions;
        private int availableFloorSpots = 0;
        private int overgrownSpots = 0;
        private float overgrownPercentage = 0f;
        private NativeList<int2> circleOffsets;
        #endregion

        #region Properties
        public Vector2 RoomStartBoundary { get { return roomStartBoundary; }}
        public Vector2 RoomEndBoundary { get { return roomEndBoundary; }}
        public int StateDensity { get { return scanDensity; } }
        public int2 StatesDimensions { get { return dimensions; } }
        #endregion

        #region Setup

        private void Start()
        {
            // Initialize states array by scanning the room for floor and wall areas:
            states = scanner.ScanRoom(out dimensions);

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
            circleOffsets = new NativeList<int2>(Allocator.Persistent);
            int threshold = playerImpactRadius * playerImpactRadius;
            int2 vec = new int2();
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

        #region Destroy

        private void OnDestroy()
        {
            states.Dispose();
            circleOffsets.Dispose();
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

        #region Impacts

        public void ApplyPlayerImpact(Vector3 impactPosition)
        {
            // Remap impact position to array index:
            int2 impactIndex = PositionToGrid(impactPosition);

            // Mark impact spot as overgrown:
            /*OvergrowSpot(impactIndex.x, impactIndex.y;*/

            // Iterate all spots that are inside the given circle radius around the impact:
            NativeArray<Color> pixels = new NativeArray<Color>(textureGenerator.Pixels, Allocator.TempJob);
            PlayerImpactCalculationJob job = new PlayerImpactCalculationJob
            {
                states = states,
                centerIndex = impactIndex,
                offsets = circleOffsets,
                dimensions = dimensions,
                pixels = pixels
            };
            JobHandle handle = job.Schedule(circleOffsets.Length, 10);
            handle.Complete();
            textureGenerator.Pixels = pixels.ToArray();
            /*textureGenerator.PixelData = pixels;*/
            pixels.Dispose();

            textureGenerator.UpdateFloorMaterial();
            textureGenerator.WriteToFile();
        }

        public void ApplyFlamethrowerImpact(FlameBounds flameBounds)
        {

        }
        #endregion

        #region Counting

        private void CountAvailableFloorSpots()
        {
            availableFloorSpots = 0;

            // Iterate all spots:
            for (int y = 0; y < dimensions.y; y++)
            {
                for (int x = 0; x < dimensions.x; x++)
                {
                    // Count all spots that are occupied by floor area:
                    if (states[GetIndex(x, y, dimensions.x)] == RoomState.CLEAN_FLOOR || states[GetIndex(x, y, dimensions.x)] == RoomState.OVERGROWN_FLOOR)
                        availableFloorSpots++;
                }
            }
        }

        private void CountOvergrownSpots()
        {
            overgrownSpots = 0;

            // Iterate all spots:
            for (int y = 0; y < dimensions.y; y++)
            {
                for (int x = 0; x < dimensions.x; x++)
                {
                    // Count all spots that are occupied by floor area:
                    if (states[GetIndex(x, y, dimensions.x)] == RoomState.OVERGROWN_FLOOR)
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
            if (percentageBar != null)
                percentageBar.Percentage = overgrownPercentage;
        }

        #endregion

        #region Texture

        private void GenerateTexture()
        {
            if (textureGenerator == null)
                return;

            textureGenerator.GenerateTexture(states, dimensions);
        }

        #endregion

        #region Helper Methods

        public int2 PositionToGrid(Vector3 position)
        {
            // Remap physical position inside room boundaries to state array:
            return new int2
            {
                x = Mathf.RoundToInt(position.x.RemapExclusive(roomStartBoundary.x, roomEndBoundary.x, dimensions.x - 1, 0)),
                y = Mathf.RoundToInt(position.z.RemapExclusive(roomStartBoundary.y, roomEndBoundary.y, dimensions.y - 1, 0))
            };
        }

        public static int GetIndex(int x, int y, int xDimension)
        {
            return (y * xDimension) + x;
        }

        #endregion
    }
}