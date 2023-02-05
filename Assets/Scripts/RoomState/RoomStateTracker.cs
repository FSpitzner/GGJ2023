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
        private int playerImpactRadius = 50;
        [SerializeField]
        [Min(1)]
        private int playerStartImpactRadius = 100;
        [SerializeField]
        [Min(1)]
        private int flameRadius = 20;
        [SerializeField]
        [Min(1)]
        private int bombRadius = 80;

        [Header("References")]
        [SerializeField]
        private RoomScanner scanner = null;
        [SerializeField]
        private RoomTextureGenerator textureGenerator = null;
        [SerializeField]
        private LevelProgress levelProgress = null;
        #endregion

        #region Internal Variables
        private NativeArray<RoomState> states;
        private int2 dimensions;
        private int availableFloorSpots = 0;
        private int overgrownSpots = 0;
        private float overgrownPercentage = 0f;
        private NativeList<int2> playerStartCircleOffsets;
        private NativeList<int2> playerCircleOffsets;
        private NativeList<int2> flameCircleOffsets;
        private NativeList<int2> bombCircleOffsets;
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
            CalculatePlayerCircleOffsets();
            CalculatePlayerStartCircleOffsets();
            CalculateFlameCircleOffsets();
            CalculateBombCircleOffsets();
        }

        private void CalculatePlayerCircleOffsets()
        {
            // Pre-calculate offset index values to be accessed to create circular pattern in the array:
            playerCircleOffsets = new NativeList<int2>(Allocator.Persistent);
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
                        playerCircleOffsets.Add(vec); vec.x = -vec.x; // i,j
                        playerCircleOffsets.Add(vec); vec.y = -vec.y; // -i,j
                        playerCircleOffsets.Add(vec); vec.x = -vec.x; // -i,-j
                        playerCircleOffsets.Add(vec);                 // i,-j
                    }
                }
            }
        }

        private void CalculatePlayerStartCircleOffsets()
        {
            // Pre-calculate offset index values to be accessed to create circular pattern in the array:
            playerStartCircleOffsets = new NativeList<int2>(Allocator.Persistent);
            int threshold = playerStartImpactRadius * playerStartImpactRadius;
            int2 vec = new int2();
            for (int i = 0; i < playerStartImpactRadius; i++)
            {
                vec.x = i;
                for (int j = 0; j < playerStartImpactRadius; j++)
                {
                    if (i * i + j * j < threshold)
                    {
                        vec.y = j;
                        playerStartCircleOffsets.Add(vec); vec.x = -vec.x; // i,j
                        playerStartCircleOffsets.Add(vec); vec.y = -vec.y; // -i,j
                        playerStartCircleOffsets.Add(vec); vec.x = -vec.x; // -i,-j
                        playerStartCircleOffsets.Add(vec);                 // i,-j
                    }
                }
            }
        }

        private void CalculateFlameCircleOffsets()
        {
            // Pre-calculate offset index values to be accessed to create circular pattern in the array:
            flameCircleOffsets = new NativeList<int2>(Allocator.Persistent);
            int threshold = flameRadius * flameRadius;
            int2 vec = new int2();
            for (int i = 0; i < flameRadius; i++)
            {
                vec.x = i;
                for (int j = 0; j < flameRadius; j++)
                {
                    if (i * i + j * j < threshold)
                    {
                        vec.y = j;
                        flameCircleOffsets.Add(vec); vec.x = -vec.x; // i,j
                        flameCircleOffsets.Add(vec); vec.y = -vec.y; // -i,j
                        flameCircleOffsets.Add(vec); vec.x = -vec.x; // -i,-j
                        flameCircleOffsets.Add(vec);                 // i,-j
                    }
                }
            }
        }

        private void CalculateBombCircleOffsets()
        {
            // Pre-calculate offset index values to be accessed to create circular pattern in the array:
            bombCircleOffsets = new NativeList<int2>(Allocator.Persistent);
            int threshold = bombRadius * bombRadius;
            int2 vec = new int2();
            for (int i = 0; i < bombRadius; i++)
            {
                vec.x = i;
                for (int j = 0; j < bombRadius; j++)
                {
                    if (i * i + j * j < threshold)
                    {
                        vec.y = j;
                        bombCircleOffsets.Add(vec); vec.x = -vec.x; // i,j
                        bombCircleOffsets.Add(vec); vec.y = -vec.y; // -i,j
                        bombCircleOffsets.Add(vec); vec.x = -vec.x; // -i,-j
                        bombCircleOffsets.Add(vec);                 // i,-j
                    }
                }
            }
        }

        #endregion

        #region Destroy

        private void OnDestroy()
        {
            states.Dispose();
            playerCircleOffsets.Dispose();
            playerStartCircleOffsets.Dispose();
            flameCircleOffsets.Dispose();
            bombCircleOffsets.Dispose();
        }

        #endregion

        #region Update

        private void Update()
        {
            // Count overgrown spots and calculate percentage:
            CountOvergrownSpots();
            CalculateOvergrownPercentage();
        }

        private void OnDrawGizmosSelected()
        {
            Gizmos.DrawLine(new Vector3(roomStartBoundary.x, 0, roomStartBoundary.y), new Vector3(roomStartBoundary.x, 0, roomEndBoundary.y));
            Gizmos.DrawLine(new Vector3(roomStartBoundary.x, 0, roomEndBoundary.y), new Vector3(roomEndBoundary.x, 0, roomEndBoundary.y));
            Gizmos.DrawLine(new Vector3(roomEndBoundary.x, 0, roomStartBoundary.y), new Vector3(roomEndBoundary.x, 0, roomEndBoundary.y));
            Gizmos.DrawLine(new Vector3(roomStartBoundary.x, 0, roomStartBoundary.y), new Vector3(roomEndBoundary.x, 0, roomStartBoundary.y));
        }
        #endregion

        #region Circle Impacts

        public void ApplyPlayerImpact(Vector3 impactPosition)
        {
            // Remap impact position to array index:
            int2 impactIndex = PositionToGrid(impactPosition);

            // Mark impact spot as overgrown:
            /*OvergrowSpot(impactIndex.x, impactIndex.y;*/
            // Iterate all spots that are inside the given circle radius around the impact:
            NativeArray<Color> pixels = new NativeArray<Color>(textureGenerator.Pixels, Allocator.TempJob);
            CircleCalculationJob job = new CircleCalculationJob
            {
                states = states,
                centerIndex = impactIndex,
                offsets = playerCircleOffsets,
                dimensions = dimensions,
                pixels = pixels,
                originState = RoomState.CLEAN_FLOOR,
                targetState = RoomState.OVERGROWN_FLOOR
            };
            JobHandle handle = job.Schedule(playerCircleOffsets.Length, 10);
            handle.Complete();
            textureGenerator.Pixels = pixels.ToArray();
            /*textureGenerator.PixelData = pixels;*/
            pixels.Dispose();

            textureGenerator.UpdateFloorMaterial();
            /*textureGenerator.WriteToFile();*/
        }

        public void ApplyPlayerStartImpact(Vector3 impactPosition)
        {
            // Remap impact position to array index:
            int2 impactIndex = PositionToGrid(impactPosition);

            // Mark impact spot as overgrown:
            /*OvergrowSpot(impactIndex.x, impactIndex.y;*/
            // Iterate all spots that are inside the given circle radius around the impact:
            NativeArray<Color> pixels = new NativeArray<Color>(textureGenerator.Pixels, Allocator.TempJob);
            CircleCalculationJob job = new CircleCalculationJob
            {
                states = states,
                centerIndex = impactIndex,
                offsets = playerStartCircleOffsets,
                dimensions = dimensions,
                pixels = pixels,
                originState = RoomState.CLEAN_FLOOR,
                targetState = RoomState.OVERGROWN_FLOOR
            };
            JobHandle handle = job.Schedule(playerStartCircleOffsets.Length, 10);
            handle.Complete();
            textureGenerator.Pixels = pixels.ToArray();
            /*textureGenerator.PixelData = pixels;*/
            pixels.Dispose();

            textureGenerator.UpdateFloorMaterial();
            /*textureGenerator.WriteToFile();*/
        }

        public void ApplyFlamethrowerCircle(Vector3 impactPosition)
        {
            // Remap impact position to array index:
            int2 impactIndex = PositionToGrid(impactPosition);

            // Iterate all spots that are inside the given circle radius around the impact:
            NativeArray<Color> pixels = new NativeArray<Color>(textureGenerator.Pixels, Allocator.TempJob);
            CircleCalculationJob job = new CircleCalculationJob
            {
                states = states,
                centerIndex = impactIndex,
                offsets = flameCircleOffsets,
                dimensions = dimensions,
                pixels = pixels,
                originState = RoomState.OVERGROWN_FLOOR,
                targetState = RoomState.CLEAN_FLOOR
            };
            JobHandle handle = job.Schedule(flameCircleOffsets.Length, 10);
            handle.Complete();
            textureGenerator.Pixels = pixels.ToArray();
            pixels.Dispose();

            textureGenerator.UpdateFloorMaterial();
            /*textureGenerator.WriteToFile();*/
        }

        public void ApplyBombCircle(Vector3 impactPosition)
        {
            // Remap impact position to array index:
            int2 impactIndex = PositionToGrid(impactPosition);

            // Iterate all spots that are inside the given circle radius around the impact:
            NativeArray<Color> pixels = new NativeArray<Color>(textureGenerator.Pixels, Allocator.TempJob);
            CircleCalculationJob job = new CircleCalculationJob
            {
                states = states,
                centerIndex = impactIndex,
                offsets = bombCircleOffsets,
                dimensions = dimensions,
                pixels = pixels,
                originState = RoomState.OVERGROWN_FLOOR,
                targetState = RoomState.CLEAN_FLOOR
            };
            JobHandle handle = job.Schedule(bombCircleOffsets.Length, 10);
            handle.Complete();
            textureGenerator.Pixels = pixels.ToArray();
            pixels.Dispose();

            textureGenerator.UpdateFloorMaterial();
            /*textureGenerator.WriteToFile();*/
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
            if (levelProgress != null)
                levelProgress.Progress = overgrownPercentage;
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

        #region Getters

        public RoomState GetState(int x, int y)
        {
            return states[GetIndex(x, y, dimensions.x)];
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

        public Vector3 GridToPosition(int xPosition, int yPosition)
        {
            return new Vector3
            {
                x = ((float)xPosition).RemapExclusive(dimensions.x - 1, 0, roomStartBoundary.x, roomEndBoundary.x),
                y = 0,
                z = ((float)yPosition).RemapExclusive(dimensions.y - 1, 0, roomStartBoundary.y, roomEndBoundary.y)
            };
        }

        public static int GetIndex(int x, int y, int xDimension)
        {
            return (y * xDimension) + x;
        }

        #endregion
    }
}