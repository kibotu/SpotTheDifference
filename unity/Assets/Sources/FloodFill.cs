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

            Boundings = new Rect(){x = img.width, y = img.height};
            FloodLoop(img, (int)loc.x, (int)loc.y, fillColor, old);
            const float tolerance = 0f;
            Boundings.x -= tolerance;
            Boundings.y -= tolerance;
            Boundings.height = (Boundings.height - Boundings.y) + tolerance;
            Boundings.width = Boundings.width - Boundings.x;
            Boundings.x = Mathf.Clamp(Boundings.x, 0, Boundings.x);
            Boundings.y = Mathf.Clamp(Boundings.y, 0, Boundings.y);
            return Boundings;
        }

        public static Rect Boundings = new Rect();

        // Recursively fills surrounding pixels of the old color  
        private static void FloodLoop(Texture2D img, int x, int y, Color fill, Color old)
        {
            // finds the left side, filling along the way  
            var fillL = x;
            do
            {
                img.SetPixel(fillL, y, fill);
                Boundings.x = Mathf.Min(Boundings.x, fillL);
                Boundings.y = Mathf.Min(Boundings.y, y);
                Boundings.height = Mathf.Max(Boundings.height, y);
                Boundings.width = Mathf.Max(Boundings.width, x);
                Boundings.width = Mathf.Max(Boundings.width, fillL);
                fillL--;
            } while (fillL >= 0 && EqualColorWithTolerance(img.GetPixel(fillL, y),old));
            fillL++;

            // find the right right side, filling along the way  
            var fillR = x;
            do
            {
                img.SetPixel(fillR, y, fill);

                Boundings.x = Mathf.Min(Boundings.x, fillL);
                Boundings.y = Mathf.Min(Boundings.y, y);
                Boundings.height = Mathf.Max(Boundings.height, y);
                Boundings.width = Mathf.Max(Boundings.width, x);
                Boundings.width = Mathf.Max(Boundings.width, fillL);
                Boundings.width = Mathf.Max(Boundings.width, fillR);
                fillR++;
            } while (fillR < img.width - 1 && EqualColorWithTolerance(img.GetPixel(fillR, y),old));
            fillR--;

            // checks if applicable up or down  
            for (var i = fillL; i <= fillR; i++)
            {
                if (y > 0 && EqualColorWithTolerance(img.GetPixel(i, y - 1),old)) FloodLoop(img, i, y - 1, fill, old);
                if (y < img.height - 1 && EqualColorWithTolerance(img.GetPixel(i, y + 1),old)) FloodLoop(img, i, y + 1, fill, old);
            }

        }

        public static bool EqualColorWithTolerance(Color a, Color b)
        {
            return a.Equals(b);
//            return Mathf.Abs(NGUIMath.ColorToInt(a) - NGUIMath.ColorToInt(b)) < 1000 ;
        }
    }
}
