using System.Collections;
using System.Collections.Generic;
using Unity.Jobs;
using UnityEngine;

namespace DNA
{
    public struct FlamethrowerImpactCalculationJob : IJobParallelFor
    {
        public void Execute(int index)
        {
            throw new System.NotImplementedException();
        }

        #region Helper Methods

        private float IsLeft(Vector2 pointA, Vector2 pointB, Vector2 pointC)
        {
            return ((pointB.x - pointA.x) * (pointC.y - pointA.y) - (pointC.x - pointA.x) * (pointB.y - pointA.y));
        }

        private bool PointInRectangle(Vector2 x, Vector2 y, Vector2 z, Vector2 w, Vector2 p)
        {
            return (IsLeft(x, y, p) > 0 && IsLeft(y, z, p) > 0 && IsLeft(z, w, p) > 0 && IsLeft(w, x, p) > 0);
        }

        #endregion
    }
}