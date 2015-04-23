from pyopenscad import *
from math import *

def main():
     # Objects used in the union
     diam = 40
     rad = diam / 2
     theta = 0
     x = 0
     y = 0
     step = 5
     objList = []
     while theta < 360:
          x = rad*cos(theta)
          y = rad*sin(theta)
          objList.append(translate(Sphere(1,fn=100), x, y, 0))
          theta = theta + step
     obj = union(objList)
     save('necklace.scad',obj)

if __name__ == '__main__':
     main()
