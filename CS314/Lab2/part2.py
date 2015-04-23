from pyopenscad import *
from math import *
from random import randrange
import datetime

# Default angles/coordinates
# Changing these will change the overall shape
a1 = 90
a2 = 0
a3 = 0
x = 3
y = 2.5
z = 3

# Tuple holds the shapes used
shape_collection = Cube(4, 4, 4), Cylinder(5,2,2, fn=100)

# List holds the overall combine shape
overall_shape = [shape_collection[0], translate(rotate(shape_collection[1],a1,a2,a3),x,y,z)]

# Overall object that is output
objList = []

def new_shape(mx=0, my=0, mz=0, rx=0, ry=0, rz=0):
     """Add the new shape object to the overall list
        
        Argument Keywords:
        mx -- Movement x coordinate
        my -- Movement y coordinate
        mz -- Movement z coordinate
        rx -- Angle x degrees
        ry -- Angle y degrees
        rz -- Angle z degress"""
     objList.append(rotate((translate(difference(overall_shape), mx, my, mz)), rx, ry, rz))

def main():      
     # Randomly add the same shape in other locations and at different angles 100 times
     for x in xrange (0, 99):
          # Move coordinates
          mx = randrange(0,20)
          my = randrange(0,20)
          mz = randrange(0,20)
          # Rotation angles
          rx = randrange(0,20)
          ry = randrange(0,360)
          rz = randrange(0,360)
          # Create the new shape
          new_shape(mx, my, mz, rx, ry, rz)
     
     # Lottery time
     # If the seconds section of the current's time is 30, make a sphere
     if (datetime.datetime.now().strftime("%S") == '30'):
          # Jackpot!
          objList.append(Sphere(8,fn=100))

     # Else, just scale up one of the other objects
     else:
          # Create a larger version of the weird shape
          objList.append(scale(difference(overall_shape),4,4,4))

     # Output the OpenSCAD code
     obj = union(objList)
     save('part2.scad',obj)

if __name__ == '__main__':
     main()
