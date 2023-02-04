using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using Unity.Jobs;
using UnityEngine;

public struct RoomTextureEditJob : IJobParallelFor
{
    public NativeArray<Color> pixels;

    public void Execute(int index)
    {

    }
}