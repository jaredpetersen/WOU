# See http://docs.python.org/2/library/string.html#format-specification-mini-language
# for Python string formatting mini-language

'''
    A set of simple classes and functions to allow the creation of OpenSCAD
    files using Python.  Done in a very simple OOP way; not engineered 
    in a Pythonic way.  Used as starting code for CS 314 Lab 2
'''

class Sphere:
    '''A sphere at the origin of the coordinate system.'''
    
    template = 'sphere( r = {radius}, $fa = {fa}, $fs = {fs}, $fn = {fn} );'
    
    def __init__(self, radius, fa=12, fs=2, fn=0):
        '''Initialize a sphere to the given values.  The special
        parameters fa, fs and fn have default values the same as
        in OpenSCAD'''
        # store parameters in a dictionary
        self.a = 5
        self.params = dict()
        self.params['radius'] = radius
        self.params['fa']     = fa          # Minimum angle in degrees for a fragment
        self.params['fs']     = fs          # Angle in mm
        self.params['fn']     = fn          # Resolution
        
    def __str__(self):
        ''' Return a string representation of this object.
            The equivalent of toString() in Java
        '''
        return Sphere.template.format(**self.params)

class Cylinder:
    '''A cylinder at the origin of the coordinate system.'''
    
    template = 'cylinder( h = {height}, r1 = {radius1}, r2 = {radius2}, $fa = {fa}, $fs = {fs}, $fn = {fn} );'
    
    def __init__(self, height, r1, r2, fa=12, fs=2, fn=0):
        '''Initialize a cylinder to the given values.  The special
        parameters fa, fs and fn have default values the same as
        in OpenSCAD'''
        # store parameters in a dictionary
        self.a = 7
        self.params = dict()
        self.params['height'] = height      # Height of the cylinder
        self.params['radius1'] = r1         # Radius of the top section of the cylinder
        self.params['radius2'] = r2         # Radius of the bottom section of the cylinder
        self.params['fa']     = fa          # Minimum angle in degrees for a fragment
        self.params['fs']     = fs          # Angle in mm
        self.params['fn']     = fn          # Resolution
        
    def __str__(self):
        ''' Return a string representation of this object.
            The equivalent of toString() in Java
        '''
        return Cylinder.template.format(**self.params)

class Cube:
    '''A cube at the origin of the coordinate system.'''
    
    template = 'cube( size = [ {x}, {y}, {z} ]);'
    
    def __init__(self, x, y=1, z=1):
        '''Initialize a cube to the given values.'''
        # store parameters in a dictionary
        self.a = 4
        self.params = dict()
        self.params['x'] = x
        self.params['y'] = y
        self.params['z'] = z
        
    def __str__(self):
        ''' Return a string representation of this object.
            The equivalent of toString() in Java
        '''
        return Cube.template.format(**self.params)

def difference( objList ):
    '''
     Take the difference between the first object in a list
	and the remaining items.  Returns a string.
    '''
    # First make sure we're dealing with strings
    objListStrings = []
    for obj in objList:
        if not isinstance(obj,str):
            obj = str(obj)
        objListStrings.append(obj)
    contents = " ".join(objListStrings)
    return 'difference(){{ {} }}'.format(contents)

def union( objList ):
    '''
        Unite the first object in a list and the remaining
        items.Returns a string.
    '''
    # First make sure we're dealing with strings
    objListStrings = []
    for obj in objList:
        if not isinstance(obj,str):
            obj = str(obj)
        objListStrings.append(obj)
    contents = " ".join(objListStrings)
    return 'union(){{ {} }}'.format(contents)

def translate(obj, x, y, z ):
    '''
        Translate an object by the given amounts.
        Returns a string.
    '''
    # if it isn't a string yet, turn it into one
    if not isinstance(obj,str):
        obj = str(obj)
    # now wrap it in the translation code
    return 'translate(v=[{0},{1},{2}]){{ {3} }}'.format(x, y, z, obj)

def rotate(obj, x, y, z ):
    '''
        Translate an object by the given amounts.
        Returns a string.
    '''
    # if it isn't a string yet, turn it into one
    if not isinstance(obj,str):
        obj = str(obj)
    # now wrap it in the translation code
    return 'rotate([{0},{1},{2}]) {3}'.format(x, y, z, obj)

def scale(obj, x, y, z ):
    '''
        Scale an object by the given amounts.
        Returns a string.
    '''
    # if it isn't a string yet, turn it into one
    if not isinstance(obj,str):
        obj = str(obj)
    # now wrap it in the translation code
    return 'scale(v=[{0},{1},{2}]){{ {3} }}'.format(x, y, z, obj)

def save(pathToNewFile, obj):
    with open(pathToNewFile,'w') as fout:
        fout.write(str(obj))
        
