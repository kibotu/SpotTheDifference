using System.Collections.Generic;
using Newtonsoft.Json.Linq;
using UnityEngine;

namespace Assets.Sources
{
    public class ImageData {

        public Vector2[] Differences;
        public string Url;

        public void SetDifferences(List<JArray> differences)
        {
            Differences = new Vector2[differences.Count];

            for (var i = 0;  i < differences.Count; ++i)
                Differences[i] = new Vector2((float) differences[i][0], (float) differences[i][1]); 
        }

        public bool HasHit(Vector2 position, float tolerance)
        {
            foreach (Vector2 value in Differences)
            {
                Debug.Log(position + " ? " + value +" => " + Vector2.Distance(value, position) + " < " + tolerance);
                if (Vector2.Distance(value, position) < tolerance) return true;
            }
            return false;
        }
    }
}