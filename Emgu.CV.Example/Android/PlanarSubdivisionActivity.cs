﻿using System;

using Android.App;
using Android.Content;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.OS;
using Android.Graphics;

//using System.Drawing;
using Emgu.CV;
using Emgu.CV.Structure;
using PlanarSubdivisionExample;
//using Android.Graphics.Bitmap;

namespace AndroidExamples
{
   [Activity(Label = "Planar Subdivision")]
   public class PlanarSubdivisionActivity : ButtonMessageImageActivity
   {
      public PlanarSubdivisionActivity()
         : base("Planar Subdivision")
      {
      }

      protected override void OnCreate(Bundle bundle)
      {
         base.OnCreate(bundle);

         /*
         OnButtonClick += delegate
         {
            Bitmap bmp = Bitmap.CreateBitmap(2, 4, Bitmap.Config.Rgb565);
            Canvas c = new Canvas(bmp);
            int[] values = new int[bmp.Width * bmp.Height]; 
            
            c.DrawColor(new Color(255,0,0));
            bmp.GetPixels(values, 0, bmp.Width, 0, 0, bmp.Width, bmp.Height);
            Image<Bgra, Byte> image = new Image<Bgra, byte>(bmp);
            image.SetValue(new Bgra(255, 0, 0, 255));
            Bitmap bmp2 = image.Bitmap;
            int[] values2 = new int[bmp2.Width * bmp2.Height];
            bmp2.GetPixels(values2, 0, bmp2.Width, 0, 0, bmp2.Width, bmp2.Height);
            bool equals = values[0] == values2[0] ;
         };*/
         
         OnButtonClick += delegate
         {
            int maxValue = 600, pointCount = 30;
            //SetImageBitmap(DrawSubdivision.Draw(maxValue, pointCount).Bitmap);
            Triangle2DF[] delaunayTriangles;
            VoronoiFacet[] voronoiFacets;
            Random r = new Random((int)(DateTime.Now.Ticks & 0x0000ffff));
            
            DrawSubdivision.CreateSubdivision(maxValue, pointCount, out delaunayTriangles, out voronoiFacets);

            //create an image for display purpose
            Image<Bgr, Byte> image = new Image<Bgr, byte>((int)maxValue, (int)maxValue);

            //Draw the voronoi Facets
            foreach (VoronoiFacet facet in voronoiFacets)
            {
               System.Drawing.Point[] polyline = Array.ConvertAll<System.Drawing.PointF, System.Drawing.Point>(facet.Vertices, System.Drawing.Point.Round);

               //Draw the facet in color
               image.FillConvexPoly(
                   polyline,
                   new Bgr(r.NextDouble() * 120, r.NextDouble() * 120, r.NextDouble() * 120)
                   );

               //draw the points associated with each facet in red
               image.Draw(new CircleF(facet.Point, 5.0f), new Bgr(System.Drawing.Color.Red), 0);
            }

            Bitmap bmp = image.ToBitmap();
            using (Canvas c = new Canvas(bmp))
            using (Paint p = new Paint())
            {
               p.StrokeWidth = 2;
               p.Color = Android.Graphics.Color.Black;
               p.SetStyle(Paint.Style.Stroke);

               //Draw the voronoi facets
               foreach (VoronoiFacet facet in voronoiFacets)
                  if (facet.Vertices.Length > 2)
                  {
                     System.Drawing.PointF p0 = facet.Vertices[facet.Vertices.Length - 1];
                     for (int i = 0; i < facet.Vertices.Length; i++)
                     {
                        System.Drawing.PointF p1 = facet.Vertices[i];
                        c.DrawLine(p0.X, p0.Y, p1.X, p1.Y, p);
                        p0 = p1;
                     }
                  }

               p.Color = Android.Graphics.Color.White;
               p.StrokeWidth = 2;
               //Draw the Delaunay triangulation
               foreach (Triangle2DF triangle in delaunayTriangles)
               {
                  System.Drawing.PointF[] points = triangle.GetVertices();
                  System.Drawing.PointF p0 = points[points.Length - 1];
                  for (int i = 0; i < points.Length; i++)
                  {
                     System.Drawing.PointF p1 = points[i];
                     c.DrawLine(p0.X, p0.Y, p1.X, p1.Y, p);
                     p0 = p1;
                  }
               }
            }
            SetImageBitmap(bmp);

         };
      }
   }
}
