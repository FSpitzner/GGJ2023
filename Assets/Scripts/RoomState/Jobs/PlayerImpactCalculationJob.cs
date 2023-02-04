using System.Collections;
using System.Collections.Generic;
using Unity.Burst;
using Unity.Collections;
using Unity.Jobs;
using Unity.Mathematics;
using UnityEngine;

namespace DNA
{
    [BurstCompile]
    public struct PlayerImpactCalculationJob : IJobParallelFor
    {
        [NativeDisableParallelForRestriction]
        public NativeArray<RoomState> states;
        [ReadOnly]
        public int2 centerIndex;
        [ReadOnly]
        public NativeList<int2> offsets;
        [ReadOnly]
        public int2 dimensions;
        [NativeDisableParallelForRestriction]
        public NativeArray<Color> pixels;

        #region States

        public void Execute(int index)
        {
            // Calculate index by using pre-calculated offset:
            int2 targetIndex = new int2(centerIndex.x + offsets[index].x, centerIndex.y + offsets[index].y);

            // Skip to next loop if point is outside array boundaries:
            if (targetIndex.x < 0 || targetIndex.x >= dimensions.x || targetIndex.y < 0 || targetIndex.y >= dimensions.y)
                return;

            // Apply overgrowth:
            OvergrowSpot(targetIndex.x, targetIndex.y);
        }

        private void OvergrowSpot(int x, int y)
        {
            // Only apply overgrowth on clean floor areas:
            if (states[GetIndex(x, y, dimensions.x)] != RoomState.CLEAN_FLOOR)
                return;

            // Update room state at impact point to overgrown state:
            states[GetIndex(x, y, dimensions.x)] = RoomState.OVERGROWN_FLOOR;

            // Update texture at impact point:
            UpdateTexture(x, y, RoomState.OVERGROWN_FLOOR);
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

        public int GetIndex(int x, int y, int xDimension)
        {
            return (y * xDimension) + x;
        }

        #endregion
    }
}