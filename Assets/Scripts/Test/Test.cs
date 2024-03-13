using LitJson;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Test
{
    public class Test : MonoBehaviour
    {



        // Start is called before the first frame update
        void Start()
        {
            HashSet<int> int_set = new();
            for (int i=0; i<10; i++)
                int_set.Add(i);
            var itor = int_set.GetEnumerator();
            itor.MoveNext();
            int_set.Remove(itor.Current);
            Debug.Log(itor.Current);
            /*            while (itor.MoveNext())
                            Debug.Log(itor.Current);*/
        }

        // Update is called once per frame
        void Update()
        {

        }

    }
}

