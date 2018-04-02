using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Emgu.CV.CvEnum;
using Emgu.CV.Util;
using Emgu.CV;
using System.Drawing;
using System.IO;



namespace AIAdventurers
{
    public class Imagseg
    {
        double height, width;


        Mat preprocess(Mat momel)
        {
            
            Mat squirrel = new Mat();
            CvInvoke.CvtColor(momel, squirrel, ColorConversion.Bgr2Bgra);
            CvInvoke.Blur(squirrel, squirrel, new Size(3,3), new Point(-1,1));
            return squirrel;
        }

       
        public List<Result> segmentation(string imagepath)
        {
            List<Result> segmentedImages = new List<Result>();
            string imageDirectory = new DirectoryInfo(imagepath).Name;
            int Level = 100, a = 0;
            double largest = 0, largest_I;
            double foodarea;
            double area;
            Mat src = new Mat();
            Mat src_clone = new Mat();
            //      VectorOfInt food = new VectorOfInt();
            int[] food = new int[5];
            VectorOfVectorOfPoint contours = new VectorOfVectorOfPoint();
            Mat hierarchy = new Mat();
            Rectangle[] boundRect = new Rectangle[contours.Size];                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                      

            src = CvInvoke.Imread(imagepath);
            double platearea = 84.3; //surface area in inches 
            foodarea = src.Rows * src.Cols;
            src_clone = preprocess(src);
            CvInvoke.Threshold(src_clone, src_clone, Level, 255, 0);
            CvInvoke.FindContours(src_clone, contours, hierarchy, RetrType.Tree, ChainApproxMethod.ChainApproxSimple, new Point(0,0));

            VectorOfVectorOfPoint contours_poly = new VectorOfVectorOfPoint(contours.Size);
           // VectorOfRect boundRect = new VectorOfRect(contours.Size);
           

            for (int x = 0; x < contours.Size; x++)
            {
                CvInvoke.ApproxPolyDP(contours[x], contours_poly[x], 3, true);
                boundRect[x] = CvInvoke.BoundingRectangle(contours_poly[x]);
                
                //area=contourArea(contours[x]);
                //cout<<"hello"<<area;

                //#pragma omp critical (largest,largest_I)
                if (CvInvoke.ContourArea(contours[x]) > (foodarea / 128))
                {

                    food[a] = x; 
                    a++;
                    if (CvInvoke.ContourArea(contours[x]) > largest && CvInvoke.ContourArea(contours[x]) < (foodarea * .95))
                    {
                        largest = CvInvoke.ContourArea(contours[x]);
                        largest_I = x;
                    }
                }

                //create cropped and output em
                for (int y = 0; y < a; y++)
                {
                    //cout<<" x " <<x<<"\n";
                    
                    if (CvInvoke.ContourArea(contours[food[y]]) == largest)
                    {
                         continue;
                    }
                   
                    Mat crops = new Mat(src, boundRect[food[y]]);

                    CvInvoke.Imwrite("crops.png", crops);
                    // fprintf(fs, "\nfilename :%s\n filepath: %s\n surfacearea: %f", filename, filename, contourArea(contours[food[x]]));
                    height = boundRect[food[y]].Height * largest / platearea; //inch squared
                    width = boundRect[food[y]].Width * largest / platearea;

                }

                
                
               
                /**/
                // we do not need the user to see this. 
                //    drawContours(src, contours, largest_I, Scalar(0, 255, 0), 8);
             //   CvInvoke.Imwrite("Platebound.png", src );
            //  imshow("Special Plate?", src);
            //    waitKey(0);


            }


            return segmentedImages;





        }




    }
}