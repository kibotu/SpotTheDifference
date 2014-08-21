using System;
using UnityEngine;

namespace Assets.Sources
{
    public class FloodFill
    {
        /** 
         * Fills the selected pixel and all surrounding pixels of the same color with the fill color. 
         * @param img image on which operation is applied 
         * @param fillColor color to be filled in 
         * @param loc location at which to start fill 
         * @throws IllegalArgumentException if loc is out of bounds of the image 
         */
        public static Rect floodFill(Texture2D img, Color fillColor, Vector2 loc)
        {
            if (loc.x < 0 || loc.x >= img.width || loc.y < 0 || loc.y >= img.height) throw new Exception();

            var old = img.GetPixel((int) loc.x, (int) loc.y);

            // Checks trivial case where loc is of the fill color  
            if (old.Equals(fillColor))
                return new Rect();

            Boundings = new Rect();
            floodLoop(img, (int)loc.x, (int)loc.y, fillColor, old);
            return Boundings;
        }

        public static Rect Boundings = new Rect();

        // Recursively fills surrounding pixels of the old color  
        private static void floodLoop(Texture2D img, int x, int y, Color fill, Color old)
        {
            var bounds = new Rect {width = img.width, height = img.height};

            // finds the left side, filling along the way  
            var fillL = x;
            do
            {
                img.SetPixel(fillL, y, fill);
                fillL--;
            } while (fillL >= 0 && img.GetPixel(fillL, y).Equals(old));
            fillL++;

            // find the right right side, filling along the way  
            var fillR = x;
            do
            {
                img.SetPixel(fillR, y, fill);
                fillR++;
            } while (fillR < bounds.width - 1 && img.GetPixel(fillR, y).Equals(old));
            fillR--;

            // checks if applicable up or down  
            for (var i = fillL; i <= fillR; i++)
            {
                if (y > 0 && img.GetPixel(i, y - 1).Equals(old)) floodLoop(img, i, y - 1, fill, old);
                if (y < bounds.height - 1 && img.GetPixel(i, y + 1).Equals(old)) floodLoop(img, i, y + 1, fill, old);
            }

            Boundings.x = Math.Min(bounds.width, fillL);
            Boundings.width = Math.Max(Boundings.width, Math.Abs(fillL - fillR));
            Boundings.y = Math.Max(Boundings.y, y);
            Boundings.height = Math.Min(Boundings.y, y);
        }
    }
}
