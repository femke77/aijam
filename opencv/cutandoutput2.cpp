#include <opencv2/imgproc.hpp>
#include <omp.h>
#include <opencv2/highgui.hpp>
#include <iostream>
#include <opencv2/imgcodecs.hpp>
#include <sstream>
#include <math.h>
//#include "json/json.h"
//#include <stdio>

using namespace cv;
using namespace std;


Mat preprocess(Mat momel)
{
Mat squirrel;
cvtColor(momel,squirrel,COLOR_BGR2GRAY);
blur(squirrel,squirrel,Size(3,3));
return squirrel;
}
double platesize()
{
int  value=10;
FILE *fr;
fr=fopen("config.txt","r+");
fscanf(fr,"%d",&value);
cout<<"Pi "<<M_PI;
return 4*M_PI * pow(2,value);
}


int main( int argc, char** argv)
{
char *p;
int Level=100,plate,largest=0,largest_I;
double foodarea;
double area;
Mat src,src_clone;
vector <int> food;
vector <vector<Point> > contours;
vector <Vec4i> hierarchy;
Mat crops;

//if the user wants to specify the threshold
if (argc>2)
{
long arg=strtol(argv[2],&p,10);
Level=arg;
if (Level>255)
{
Level=255;
}
}

//load the image to src and send out an error report if it doesn't exist
src=imread(argv[1],IMREAD_COLOR);
if (src.empty())
{
cerr<<"Bruh! you no pic!"<<endl;
return -1;
}
plate=platesize();
foodarea=src.rows*src.cols;
src_clone=preprocess(src);
threshold(src_clone,src_clone,Level,255,THRESH_BINARY);
//Canny(src_clone,src_clone,Level,255,5,false);
//imshow("Canny",src_clone);
//waitKey(0);
findContours(src_clone,contours,hierarchy,RETR_TREE, CHAIN_APPROX_SIMPLE, Point(0,0));

//This could be defined above but then it wouldn't be limited. I think its better this way so its optimized because it has a max size
vector<vector<Point> > contours_poly(contours.size());
vector<Rect> boundRect(contours.size());


//creating bounding rectangles relative to contours
//#pragma omp parallel for critical (largest,largest_I)
//fk it im scared to lose data
for(int x=0;x<contours.size();x++)
{
approxPolyDP(Mat(contours[x]),contours_poly[x],3,true);
boundRect[x]=boundingRect(Mat(contours_poly[x]));
//area=contourArea(contours[x]);
//cout<<"hello"<<area;

//#pragma omp critical (largest,largest_I)
if (contourArea(contours[x])>(foodarea/128))
{
//cout<<"area"<<contourArea(contours[x])<<" a"<<a<<" food area"<<foodarea/16<<"\n";
food.push_back(x);
if (contourArea(contours[x])>largest && contourArea(contours[x])<(foodarea*.95))
{
largest=contourArea(contours[x]);
largest_I=x;
}
}
}

//create cropped and output em
#pragma omp parallel for
for(int x=0;x<food.size();x++)
{
//cout<<" x " <<x<<"\n";

crops=src(boundRect[food[x]]);
ostringstream hakama;
hakama<<"output"<<x<<".jpg";
//cout<<hakama;
String filename(hakama.str());
//cout<<filename;
imwrite(filename,crops);

}
/**/
drawContours(src,contours,largest_I,Scalar(0,255,0),8);
imwrite("Platebound.png",src);
//imshow("Special Plate?",src);
//waitKey(0);
return(0);
}
