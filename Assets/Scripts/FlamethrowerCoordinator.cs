using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DNA
{
    public class FlamethrowerCoordinator : MonoBehaviour
    {
        #region Internal Variables
        private Flamethrower[] flamethrowers = null;
        private int index = 0;
        #endregion

        #region Setup

        void Start()
        {
             flamethrowers = FindObjectsOfType<Flamethrower>();
        }

        #endregion

        #region Update

        void Update()
        {
            if (flamethrowers == null)
                return;

            flamethrowers[index].UseFlamethrower();

            index = (index + 1) % flamethrowers.Length;
        }

        #endregion
    }
}