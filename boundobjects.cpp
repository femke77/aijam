#include "opencv2/imgcodecs.hpp"
#include "opencv2/highgui.hpp"
#include "opencv2/imgproc.hpp"
#include "opencv2/opencv.hpp"
#include <iostream>

using namespace cv;
using namespace std;

int main(int argc, char** argv)
{
char *p;
RNG rng(12345);
int Level=100,largest=0,largest2=0,largest_I,largest2_I;
double area;
Mat src,src_grey,src_clone,cropped;
vector <vector<Point> > contours;
vector <Vec4i> hierarchy;

if (argc>2)
{
long arg=strtol(argv[2],&p,10);
Level=arg;
}

src=imread(argv[1],IMREAD_COLOR);

if (src.empty())
{
cerr<<"bruh! you no pic!"<<endl;
return -1;
}

src_clone=src.clone();
//const char* window_name="result";
//namedWindow(window_name,WINDOW_AUTOSIZE);
Mat tresh_O,canny_output,canny_print,drawing,print;

cvtColor(src,src_grey,COLOR_BGR2GRAY);
blur(src_grey,src_grey,Size(3,3));

//Canny(src_grey, canny_output, 100, 100*2, 3);

threshold(src_grey,tresh_O,Level,255,THRESH_BINARY);
findContours(tresh_O,contours,hierarchy, RETR_TREE, CHAIN_APPROX_SIMPLE, Point(0,0));
//drawing=Mat::zeros(canny_output.size(),CV_8UC3);
//print=Mat::zeros(canny_output.size(),CV_8UC3);
vector<vector<Point> > contours_poly( contours.size() );
vector<Rect> boundRect( contours.size() );
vector<Point2f>center( contours.size() );
vector<float>radius( contours.size() );

for(int x=0;x<contours.size();x++)
{
approxPolyDP(Mat(contours[x]),contours_poly[x],3,true);
boundRect[x]=boundingRect(Mat(contours_poly[x]));
//minEnclosingCircle((Mat)contours_poly[x],center[x],radius[x]);
area=contourArea(contours[x]);
if (area>largest2)
{
largest2=area;
largest2_I=x;
if (area>largest)
{
largest2=largest;
largest2_I=largest_I;
largest=area;
largest_I=x;
}
//largest_I=x;
}
}
drawing=Mat::zeros(tresh_O.size(),CV_8UC3);


//drawContours(src_clone,contours,largest_I,Scalar(133,133,133),2);
for (int x=0;x<contours.size();x++)
{
Scalar color=Scalar( rng.uniform(0,255),rng.uniform(0,255),rng.uniform(0,255));
//drawContours(drawing,contours,(int)x,color,2,8,hierarchy,0,Point());
//drawContours(print,contours,(int)i,color,2,8,hierarchy,0,Point());
rectangle(src_clone,boundRect[x].tl(),boundRect[x].br(),color,2,8,0);
//circle code here
}
const char* window_name="result";
namedWindow(window_name,WINDOW_AUTOSIZE);
Rect dimensions;
dimensions.x=boundRect[largest2_I].x;
dimensions.y=boundRect[largest2_I].y;
dimensions.width=boundRect[largest2_I].width;
dimensions.height=boundRect[largest2_I].height;

cropped=src(dimensions);
//String a=boundRect[largest2_I];
//cout<<boundRect[largest2_I].height;

imshow("result",cropped);
waitKey(0);
imwrite("../../Desktop/AI-JAM/OutputT.jpg",src_clone);
imwrite ("../../Desktop/AI-JAM/OutputCropped.jpg",cropped);


return(0);
}
