using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BetoMaluje.Sikta
{
    public class Utils : MonoBehaviour
    {
        public static T GetComponentInParents<T>(GameObject startObject) where T : Component
        {
            T returnObject = null;
            GameObject currentObject = startObject;
            while(!returnObject)
            {
                if (currentObject == currentObject.transform.root) return null;
                currentObject = (GameObject) currentObject.transform.parent.gameObject;
                returnObject = currentObject.GetComponent<T>();
            }
            return returnObject;
        }
    }
}