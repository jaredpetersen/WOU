from pyopenscad import *
from math import *

def main():
     # Height of the crystal     
     z_dim = 0
     # Max height of crystal (# vertical circles)
     max_height = 3
     # Offset counter for the crystal
     offset = 0
     # Holds the overall object
     objList = []

     # Build the base but change the height and offset each iteration
     while z_dim < max_height:   
          # X and Y dimension of the object
          x_dim = 0 + offset
          y_dim = 0 + offset
          # Iterative count for the spheres
          sphere_count = 0
          # Max number of spheres per square
          sphere_max_count = 100
          # Used to determine when the x dimension of the base should end
          switch_number = sqrt(sphere_max_count)
          switch_count = switch_number

          # Create the square
          while sphere_count < sphere_max_count:          
               if switch_count == sphere_count:
                    # Switch to a new row
                    y_dim += 2
                    x_dim = 0 + offset
                    switch_count = switch_count + switch_number
               objList.append(translate(Sphere(1,fn=100), x_dim, y_dim, z_dim))
               # Iterate the counter variables
               x_dim += 2
               sphere_count += 1

          # Increase the height by one
          z_dim += 1
          # Offset the square by one
          offset += 1

     obj = union(objList)
     save('crystal.scad',obj)

if __name__ == '__main__':
     main()
