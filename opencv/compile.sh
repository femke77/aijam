echo"Compiling $1 as  $2"
g++ $1 `pkg-config --cflags --libs opencv` -o $2
