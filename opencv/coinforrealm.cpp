#include <opencv2/opencv.hpp>
#include <sstream>
#include <opencv2/imgproc.hpp>
#include <opencv2/imgcodecs.hpp>
using namespace std;
using namespace cv;

int main (int argc, char** argv)
{
//if (argc>1)
Mat src = imread(argv[1],IMREAD_COLOR);
cvtColor(src,src,COLOR_BGR2GRAY);
double coinsize=src.rows*src.cols;
CascadeClassifier coinforme("cascade.xml");
vector <Rect> coin;
//coinsize=coinsize;
int a=(int)coinsize;
coinforme.detectMultiScale(src,coin,1.05,5,0 |CASCADE_SCALE_IMAGE,Size(a,a)); 

for (int x=0;x<coin.size();x++)
{
cout<<"detected";
Mat temp=src(coin[x]);
ostringstream strike;
strike<<"coin"<<x<<".jpg";
String filen(strike.str());
imwrite(filen,temp);
imshow("bruh!",temp);
waitKey(0);
}

return 0;
}
