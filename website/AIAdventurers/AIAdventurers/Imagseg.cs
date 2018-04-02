using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Emgu.CV.CvEnum;
using Emgu.CV.Util;
using Emgu.CV;



namespace AIAdventurers
{
    public class Imagseg
    { 


        Mat preprocess(Mat momel)
        {
            
            Mat squirrel = new Mat();
            CvInvoke.CvtColor(momel, squirrel, ColorConversion.Bgr2Bgra);
            CvInvoke.Blur(squirrel, squirrel, Size(3,3), Point(-1,1));
            return squirrel;
        }

       
        public List<String> segmentation(string imagepath)
        {
            List<String> segmentedImages = new List<String>();
            string imageDirectory = new DirectoryInfo(imagepath).Name;
            int Level = 100, a = 0, largest = 0, largest_I;
            double foodarea;
            double area;
            Mat src = new Mat();
            Mat src_clone = new Mat();
            VectorOfInt food = new VectorOfInt();
            VectorOfVectorOfPoint contours = new VectorOfVectorOfPoint();
            Mat hierarchy = new Mat();
            Mat crops = new Mat();                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                             ccvx;

            src = CvInvoke.Imread(imagepath, IMREAD_COLOR);
            foodarea = src.rows * src.cols;
            src_clone = preprocess(src);
            imshow("Canny", src_clone);
            waitKey(0);
            findContours(src_clone, contours, hierarchy, RETR_TREE, CHAIN_APPROX_SIMPLE, Point(0, 0));

            
            vector<vector<Point>> contours_poly(contours.Size);
            vector<Rect> boundRect(contours.Size);

            for (int x = 0; x < contours.Size; x++)
            {
                approxPolyDP(Mat(contours[x]), contours_poly[x], 3, true);
                boundRect[x] = boundingRect(Mat(contours_poly[x]));
                //area=contourArea(contours[x]);
                //cout<<"hello"<<area;

                //#pragma omp critical (largest,largest_I)
                if (contourArea(contours[x]) > (foodarea / 128))
                {
                    cout << "area" << contourArea(contours[x]) << " a" << a << " food area" << foodarea / 16 << "\n";
                    food.push_back(x);
                    if (contourArea(contours[x]) > largest && contourArea(contours[x]) < (foodarea * .95))
                    {
                        largest = contourArea(contours[x]);
                        largest_I = x;
                    }
                }

                //create cropped and output em
                //#pragma omp parallel for
                for (int x = 0; x < food.size(); x++)
                {
                    //cout<<" x " <<x<<"\n";
                    crops = src(boundRect[food[x]]);
                    ostringstream hakama;
                    hakama << "output" << x << ".jpg";
                    //cout<<hakama;
                    String filename(hakama.str);
                    //cout<<filename;
                    imwrite(filename, crops);
                }
                /**/
                // we do not need the user to see this. 
            //    drawContours(src, contours, largest_I, Scalar(0, 255, 0), 8);
                  imwrite("Platebound.png", src);
            //  imshow("Special Plate?", src);
            //    waitKey(0);
            }






        }




    }
}