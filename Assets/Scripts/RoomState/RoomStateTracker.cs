using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

namespace DNA
{
    public class RoomStateTracker : MonoBehaviour
    {
        #region Internal Variables
        // States of all possible spots.
        // 0 = empty area
        // 1 = clean floor
        // 2 = overgrown floor
        // 3 = wall
        private int2[,] area;
        #endregion
    }
}