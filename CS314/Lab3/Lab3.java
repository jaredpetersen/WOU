import java.io.*;

/**
 *    CS314 Lab 3: Filtering an audio file.<p>
 *
 *    Start with any audio file in just about any format. Re-format it to raw data output using the
 *    sox utility. For example to convert audio.wav<p>
 *
 *    $ sox audio.wav -r 44100 -s -b 16 -c 1 audio.raw <p>
 *
 *    The raw data will be in 2's complement 16 bit values (-s and -b 16), sampled at 44100 Hz (-r), in
 *    only one channel (-c 1). This Java file reads in this .raw file and performs digital filtering
 *    on it. The filtering implemented here is a low pass filter. You specify a cutoff frequency.
 *    The filter passes sound in the file below this frequency and rejects higher frequencies. The
 *    filter requires a filter length. Bigger numbers do a better job of filtering, but slow things
 *    down.<p>
 *
 *    After filtering write out a new audio file in .raw format. Then use sox to turn it back into
 *    something you can listen to. <p>
 *
 */

public class Lab3
{
	private double fs;			// Sample rate in Hz
	private double fc;			// Filter cutoff frequency in Hz
	private int M;				// Length of filter
	private File inputFile;
	private File outputFile;
	private DataInputStream in;
	private DataOutputStream out;

	private double[] filter;	// Array used to hold filter values


	/**
	 *    The main program. Command line parameters specify the file name, cutoff frequency
	 *
	 *@param    args  See printHelp() for usage
	 */
	public static void main( String[] args )
	{
		Lab3 lab = new Lab3( args );// So we don't have to make everything static
	}

	/**
	 *    Constructor for the Lab3 object
	 *
	 *@param    args  Description
	 */
	public Lab3( String[] args )
	{
		if ( args.length != 5 )
		{
			printHelp();
			System.exit( 1 );
		}
		try
		{
			inputFile = new File( args[0] );	// Input filename
			outputFile = new File( args[1] );	// Output filename	
			fs = Double.parseDouble( args[2] );	// Sample rate in Hz
			fc = Double.parseDouble( args[3] );	// cutoff frequency in Hz
			M = Integer.parseInt( args[4] );	// Filter length
			if ( M % 2 != 1 )
				throw new Exception( "Filter length must be odd." );

			System.out.println( "Filtering with: fs = " + fs + "\tfc = " + fc +
				"\tfilter length = " + M );

			// Make sure we can read it
			if ( !inputFile.canRead() )
				throw new Exception( "Cannot read from input file." );
			// Make sure we can write new file, delete file with this name if it is there
			if ( outputFile.exists() )
			{
				System.out.println( "Deleting old output file and creating empty new one" );
				outputFile.delete();
				outputFile.createNewFile();
			}
			// Set up input and output streams
			in = new DataInputStream( new BufferedInputStream( new FileInputStream( inputFile ) ) );
			out = new DataOutputStream( new BufferedOutputStream( new FileOutputStream( outputFile ) ) );
		}
		catch ( Exception e )
		{
			System.out.println( "Problem... " );
			e.printStackTrace();
			printHelp();
			System.exit( 1 );
		}

		/*----------------------- Create Filter -----------------------------*/
		filter = this.createLowPassFilter(M, fs, fc);
		

		/*----------------------- Perform reading, filtering and writing ----*/
		// Create a working array to hold the values we read in from the input audio file
		double[] work = new double[M];
		// We have 2 bytes per data value, so we have this many data values
		int N = (int) ( inputFile.length() ) / 2;

		try
		{
			int pos = M - 1;		// starting position in work array
			// Process data values one by one
			for ( int i = 0; i < N; ++i )
			{
				int v = in.readShort();
				// After reading, convert from little-endian to big-endian, if necessary
				v = ( ( v << 8 ) & 0x0000FF00 | ( ( v >>> 8 ) & 0x000000FF ) );
				if ( ( v & 0x00008000 ) == 0x00008000 )
					v = v | 0xFFFF0000;
				work[pos] = (double) v;			// Store audio value in work array
				double calc = 0.0;				// Temporary variable for performing the sum
				// Iterate over length of filter and add up terms
				// This is also called a convolution
				for ( int j = 0; j < M; ++j )
				{
					calc += filter[j] * work[( pos - j + M ) % M];
				}
				// Increment our position in the work array, modulo M
				pos = ( pos + 1 ) % M;
				// Convert back to little-endian before writing, if necessary
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
	
	/* Create a low pass filter array. */
	private double[] createLowPassFilter( int length, double sampleRate, double cutoffFreq )
	{
		double[] filter = new double[length];
		int mid = ( length - 1 ) / 2;					// Index of zero point (middle of filter array)
		double lam = cutoffFreq * Math.PI / sampleRate;	// Normalized frequency

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
		System.out.println( "Filter length must be an odd number.");
		System.out.println( "For example: $ java Lab3 in.raw out.raw 44100 1250 101" );
	}
} // End class Lab3

