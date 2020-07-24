using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace EPOOutline
{
    public class VisibilityListener : MonoBehaviour
    {
        public Action<bool> OnVisibilityChanged;

        private bool destroyed = false;

        private void OnBecameVisible()
        {
            if (OnVisibilityChanged != null)
                OnVisibilityChanged.Invoke(true);
        }

        private void OnBecameInvisible()
        {
            if (OnVisibilityChanged != null)
                OnVisibilityChanged.Invoke(false);
        }
    }
}