#include <opencv2/imgproc.hpp>
#include <opencv2/highgui.hpp>
#include <iostream>
#include <opencv2/imgcodecs.hpp>
#include <sstream>
#include <stdio.h>
using namespace cv;
using namespace std;




int main( int argc, char** argv)
{
FILE *fp;
Mat img=imread("1.jpg");
Mat src=img.clone();
Mat Blackie;
Size size(1280,1280);
char *p;
int momel=100;
RNG rng(12345);

fp=fopen("bg.txt", "w+");

if (argc>1)
{
long arg=strtol(argv[1],&p,10);
momel=arg;
}

for(int x=1;x<=momel;x++)
{

src.setTo(Scalar(rng.uniform(0,255),rng.uniform(0,255),rng.uniform(0,255)));
resize(src,src,size);
cvtColor(src,Blackie,COLOR_BGR2GRAY);
ostringstream hakama;
hakama<<"BW"<<x<<".jpg";
//cout<<hakama;
String filename(hakama.str());
//cout<<filename;
imwrite(filename,Blackie);

fprintf(fp,"./BW%d.jpg\n",x);


}
//waitKey(0);
fclose(fp);
return(0);
}
