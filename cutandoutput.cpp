#include <opencv2/imgproc.hpp>
#include <opencv2/highgui.hpp>
#include <iostream>
#include <opencv2/imgcodecs.hpp>
#include <sstream>
using namespace cv;
using namespace std;


Mat preprocess(Mat momel)
{
Mat squirrel;
cvtColor(momel,squirrel,COLOR_BGR2GRAY);
blur(squirrel,squirrel,Size(3,3));
return squirrel;
}

int main( int argc, char** argv)
{
char *p;
int Level=100,a=0;
double foodarea;
double area;
Mat src,src_clone;
vector <int> food(100);
vector <vector<Point> > contours;
vector <Vec4i> hierarchy;
vector <Mat> crops(100);

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

foodarea=src.rows*src.cols;
//cout<<"\n"<<foodarea;
src_clone=preprocess(src);

threshold(src_clone,src_clone,Level,255,THRESH_BINARY);
findContours(src_clone,contours,hierarchy,RETR_TREE, CHAIN_APPROX_SIMPLE, Point(0,0));

//This could be defined above but then it wouldn't be limited. I think its better this way so its optimized because it has a max size
vector<vector<Point> > contours_poly(contours.size());
vector<Rect> boundRect(contours.size());



//creating bounding rectangles relative to contours


for(int x=0;x<contours.size();x++)
{
approxPolyDP(Mat(contours[x]),contours_poly[x],3,true);
boundRect[x]=boundingRect(Mat(contours_poly[x]));
area=contourArea(contours[x]);
if (area>(foodarea/8))
{
//cout<<"area"<<area<<" a"<<a<<" food area"<<foodarea/8<<"\n";
food[a]=x;
a++;
}
}

//create cropped and output em
for(int x=0;x<a;x++)
{
//cout<<" x " <<x<<"\n";
crops[x]=src(boundRect[food[x]]);
ostringstream hakama;
hakama<<"output"<<x<<".jpg";
//cout<<hakama;
String filename(hakama.str());
//cout<<filename;
imwrite(filename,crops[x]);
}
/**/
imshow("SUM FUK?",src);
waitKey(0);
return(0);
}
