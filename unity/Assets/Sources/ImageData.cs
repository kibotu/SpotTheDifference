using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json.Linq;
using UnityEngine;

namespace Assets.Sources
{
    public class ImageData {

        public string Url;
        public Vector2 Dimension;
        public ArrayList Spots;

        public void SetDifferences(List<JArray> differences)
        {
            Spots = new ArrayList(differences.Count);

            foreach (var t in differences)
                Spots.Add(new Spot() { Position = new Vector2((float) t[0], (float) t[1]), IsAvailable = true });
        }

        public bool HasHit(Vector2 position, float tolerance)
        {
            foreach (var t in Spots)
            {
                var spot = ((Spot) t);
                if(!spot.IsAvailable) continue;
                if (!(Vector2.Distance(spot.Position, position) < tolerance)) continue;
                spot.IsAvailable = false;
                return true;
            }
            return false;
        }

        public void SetDimension(JArray dimension)
        {
            Dimension = new Vector2((float) dimension[0], (float) dimension[1]);
        }

        public bool HasSpotsLeft()
        {
            return Spots.Cast<Spot>().Any(spot => spot.IsAvailable);
        }
    }
}