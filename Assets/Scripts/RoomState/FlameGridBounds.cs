using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

namespace DNA
{
    public struct FlameGridBounds
    {
        public int2 a;
        public int2 b;
        public int2 c;
        public int2 d;

        #region Properties

        public int MinX
        {
            get
            {
                return math.min(a.x, math.min(b.x, math.min(c.x, d.x)));
            }
        }

        public int MaxX
        {
            get
            {
                return math.max(a.x, math.max(b.x, math.max(c.x, d.x)));
            }
        }

        public int MinY
        {
            get
            {
                return math.min(a.y, math.min(b.y, math.min(c.y, d.y)));
            }
        }

        public int MaxY
        {
            get
            {
                return math.max(a.y, math.max(b.y, math.max(c.y, d.y)));
            }
        }

        public int Width
        {
            get
            {
                return MaxX - MinX;
            }
        }

        public int Height
        {
            get
            {
                return MaxY - MinY;
            }
        }

        #endregion
    }
}