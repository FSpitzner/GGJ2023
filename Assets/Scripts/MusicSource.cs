using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DNA
{
    public class MusicSource : MonoBehaviour
    {
        private void Awake()
        {
            DontDestroyOnLoad(this);
        }
    }
}