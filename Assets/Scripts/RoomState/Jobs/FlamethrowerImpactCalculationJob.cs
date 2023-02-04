using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using Unity.Jobs;
using Unity.Mathematics;
using UnityEngine;

namespace DNA
{
    public struct FlamethrowerImpactCalculationJob : IJobParallelFor
    {
        [NativeDisableParallelForRestriction]
        public NativeArray<RoomState> states;
        [ReadOnly]
        public NativeArray<int2> indexList;
        [ReadOnly]
        public FlameGridBounds bounds;
        [ReadOnly]
        public int2 dimensions;
        [NativeDisableParallelForRestriction]
        public NativeArray<Color> pixels;

        #region Main Execution

        public void Execute(int index)
        {
            // Skip to next loop if point is outside array boundaries:
            if (indexList[index].x < 0 || indexList[index].x >= dimensions.x || indexList[index].y < 0 || indexList[index].y >= dimensions.y)
                return;

            // Check if point is inside the flamethrower hitbox:
            if (!IsPointInRectangle(indexList[index], bounds.a, bounds.b, bounds.c, bounds.d))
                return;

            CleanSpot(indexList[index].x, indexList[index].y);
        }

        private void CleanSpot(int x, int y)
        {
            // Only apply cleaning on overgrown floor areas:
            if (states[GetIndex(x, y, dimensions.x)] != RoomState.OVERGROWN_FLOOR)
                return;

            // Update room state at impact point to clean state:
            states[GetIndex(x, y, dimensions.x)] = RoomState.CLEAN_FLOOR;

            // Update texture at impact point:
            UpdateTexture(x, y, RoomState.CLEAN_FLOOR);
        }

        #endregion

        #region Pixels

        public void UpdateTexture(int x, int y, RoomState state)
        {
            // Update pixel at given position:
            pixels[GetIndex(x, y, dimensions.x)] = CalculateStateColor(state);
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

        #endregion

        #region Helper Methods

        private bool IsPointInRectangle(int2 p, int2 x, int2 y, int2 z, int2 w)
        {
            return (IsLeft(x, y, p) > 0 && IsLeft(y, z, p) > 0 && IsLeft(z, w, p) > 0 && IsLeft(w, x, p) > 0);
        }

        private float IsLeft(int2 pointA, int2 pointB, int2 pointC)
        {
            return ((pointB.x - pointA.x) * (pointC.y - pointA.y) - (pointC.x - pointA.x) * (pointB.y - pointA.y));
        }

        public int GetIndex(int x, int y, int xDimension)
        {
            return (y * xDimension) + x;
        }

        #endregion
    }
}