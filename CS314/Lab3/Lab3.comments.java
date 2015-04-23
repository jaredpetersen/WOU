							//Beginning of program unit
import java.io.*;			// This statement gives access to all the class files in the java.io package.

/**
 *    Here's a version that gives examples of the level of detail I'm expecting from the documentation aspect of the lab. You'll need to
 *    do this level of work for the C version for an A on the documentation portion of the Lab.<p>
 *
 *    Printed in landscape mode so you can have room to write comments
 */

public class Lab3			// Class declaration.  Declares a new class (and thus a new type) called Lab3.  It has public access which means objects
							// of type Lab3 can be instantiated and used from outside the current package, and public static methods or data members
							// can be accessed as well.
{
							// The following are declarations of instance variables of various types.  The first 3 are of primitive types while the
							// remaining 4 are of Object types.  These are all private to this class and thus have only local visibility (scope local
							// to this class.)  The primitive types cause allocation of memory resources for their respective types.  This is determined
							// at compile time.  The Object types also cause allocation of memory resources (determined at compile time) but allocation
							// is limited to a reference type.  No allocation (on the heap) for Objects is performed at this time.
	
	private double fs;
	private double fc;
	private int M;
	private File inputFile;
	private File outputFile;
	private DataInputStream in;
	private DataOutputStream out;

	private double[] filter;	// allocation of an array reference.  No actual array is allocated.  Reference type determined at compile time.


							// Declaration of a method called main.  This method is available publicly with or without the creation of a Lab3 object
							// It accepts a reference to an array of String objects and returns a void type.
	public static void main( String[] args )
	{
		Lab3 lab = new Lab3( args );// Instantiates a Lab3 object using the given array of Strings (reference passed by value to the constructor).
									// Causes memory allocation on the heap for the creation of the Lab3 object.  The lifetime of this object is uncertain
									// as it is deallocated by the garbage collector at runtime.  However, it is referenced by a variable called lab which
									// is local in scope and lifetime to the main method.  The Lab3 object becomes free for garbage collection as soon as
									// the main method finishes.  Since this is not a graphical application, this will happen as soon as the Lab3 constructor
									// returns. Assignment statement assigns the address of the newly instantiated object into the reference called lab.
									// How this reference holds and manages the address is yet to be determined.
	}

							// Constructor for this class.  Accepts a copy of a reference to an array of Strings as a parameter
	public Lab3( String[] args )
	{
							// Conditional.  Dot operator is used to access the length variable inside the array object.  Arrays are treated as objects
							// in Java and are not a unique type.
		if ( args.length != 5 )
		{
			printHelp();	// Invoke the printHelp() instance method from this very object.  Implicit call to this.printHelp()
			System.exit( 1 );	// Invoke the static method exit() from the System class.  Access to the System class is implicit since it is in 
							// the java.lang package, which does not have to be imported.
		}
							// Beginning of an exception handling block
		try
		{
							// The following lines perform assignment, object instantiation, invocation of methods via type names (static)
			inputFile = new File( args[0] );
			outputFile = new File( args[1] );		// Array access in Java is via square brackets and an integer index (0 based)
			fs = Double.parseDouble( args[2] );
			fc = Double.parseDouble( args[3] );
			M = Integer.parseInt( args[4] );
			
							
			if ( M % 2 != 1 )	// Under a conditional statement, an Exception object is instantiated and "thrown" out of this try block
								// A thrown exception skips all code in the current block.  It can then "escape" out of the current method
								// or constructor and move up the stack.  This is if it is not "caught" by an appropriately declared catch
								// block
				throw new Exception( "Filter length must be odd." );

								// New features here are String object literals and the use of the String concatenation operator, which is built
								// in to the Java language.
			System.out.println( "Filtering with: fs = " + fs + "\tfc = " + fc +
				"\tfilter length = " + M );

			if ( !inputFile.canRead() )
				throw new Exception( "Cannot read from input file." );

			if ( outputFile.exists() )
			{
				System.out.println( "Deleting old output file and creating empty new one" );
				outputFile.delete();
				outputFile.createNewFile();		// Throws exceptions and so must be written inside a try - catch block, or in a method declared
												// to throw exceptions.
			}
			in = new DataInputStream( new BufferedInputStream( new FileInputStream( inputFile ) ) );
			out = new DataOutputStream( new BufferedOutputStream( new FileOutputStream( outputFile ) ) );
		}
		
								// Catch block.  This is a language construct designed to provide a place to handle an exception event.  Exceptions
								// are inherently a runtime, and thus dynamic, feature.  They are dynamically type checked and methods of exception
								// classes can be invoked through the event object polymorphically.  This is the case here, where event objects of 
								// type IOException or FileNotFoundException could be thrown by code within the try block.
		
		catch ( Exception e )
		{
			System.out.println( "Problem... " );
			e.printStackTrace();
			printHelp();
			System.exit( 1 );
		}

		/*
		    ----------------------- Create Filter -----------------------------
		  */
		filter = this.createLowPassFilter( M, fs, fc );

		/*
		    ----------------------- Perform reading, filtering and writing ----
		  */
		// Create a working array to hold the values we read in from the input audio file
		double[] work = new double[M];
		// We have 2 bytes per data value, so we have this many data values
		int N = (int) ( inputFile.length() ) / 2;

		try
		{
			int pos = M - 1;// starting position in work array
			// Process data values one by one
			for ( int i = 0; i < N; ++i )
			{
				int v = in.readShort();
				// After reading, convert from little-endian to big-endian
				v = ( ( v << 8 ) & 0x0000FF00 | ( ( v >>> 8 ) & 0x000000FF ) );
				if ( ( v & 0x00008000 ) == 0x00008000 )
					v = v | 0xFFFF0000;
				work[pos] = (double) v;// Store audio value in work array
				double calc = 0.0;// Temporary variable for performing the sum
				// Iterate over length of filter and add up terms
				// This is also called a convolution
				for ( int j = 0; j < M; ++j )
				{
					calc += filter[j] * work[( pos - j + M ) % M];
				}
				// Increment our position in the work array, modulo M
				pos = ( pos + 1 ) % M;
				// Convert back to little-endian before writing
				v = (int) calc;
				out.writeShort( (short) ( ( ( v << 8 ) & 0x0000FF00 ) | ( ( v >>> 8 ) & 0x000000FF ) ) );
			}
			// Clean up input and output streams
			in.close();
			out.flush();
			out.close();
		}
		catch ( IOException exc )
		{
			exc.printStackTrace();
		}
	}

	/**
	 *    Create a low pass filter in an array.
	 *
	 *@param    length      Description
	 *@param    sampleRate  Description
	 *@param    cutoffFreq  Description
	 *@return               Description
	 */
	private double[] createLowPassFilter( int length, double sampleRate, double cutoffFreq )
	{
		double[] filter = new double[length];
		int mid = ( length - 1 ) / 2;// Index of zero point (middle of filter array)
		double lam = cutoffFreq * Math.PI / sampleRate;// Normalized frequency

		// Low pass filter.   Filters are symmetric so we only have to iterate over half
		filter[mid] = lam / Math.PI;
		for ( int i = 0; i < mid; ++i )
		{
			filter[i] = Math.sin( ( i - mid ) * lam ) / ( ( i - mid ) * Math.PI );
			filter[length - 1 - i] = filter[i];
		}
		return filter;
	}

	/**    Print the help message. */
	private static void printHelp()
	{
		System.out.println( "Five arguments are needed: InputFilename, OutputFilename, Sample rate, cutoff frequency in Hz, and length of filter to use" );
		System.out.println( "Filter length must be an odd number." );
		System.out.println( "For example: $ java Lab3 in.raw out.raw 44100 1250 101" );
	}
}// End class Lab3

