// Lab3.c
// Jared Petersen

/*  C-Language starter and example implementation of the audio filter program Lab3.java

    Compile and run like this:

        gcc -Wall Lab3.c -lm -o Lab3.out
        ./Lab3.out in.raw out.raw 44100 1250 101

    The -lm causes the resulting executable to be linked with the math library, which is needed
    for the math.h below.  Run
        man gcc
    for help on any other options.
*/

#include <stdio.h>
#include <stdlib.h>
#include <sys/stat.h>
#include <math.h>
#define PI 3.1415926535897931

/* ..... Structure definition ......*/
struct run_info
{
	FILE * in;		// Pointer to input file
	FILE * out;		// Pointer to output file
	off_t file_size;    // Size of input file in bytes
	double fs;		// Audio file sample rate
	double fc;		// Cutoff frequency for the low pass filter
	int M;			// Filter length
};

/* .....  Function prototypes .......*/
void print_usage( void );
struct run_info get_arguments_files( int, char** );
double *createLowPassFilter( int, double, double );


/*************************************
 *	Main entry point
 *************************************/
int main( int argc, char * argv[] )
{
	if( argc < 6 )
	{
		print_usage();
		exit(EXIT_FAILURE);
	}

	// Parse command line arguments and open files.  Return values of arguments
	struct run_info info = get_arguments_files( argc, argv );

	// Print run information to the user
	printf( "Input file \"%s\" has size %d bytes\n", argv[1], (int)info.file_size );
	printf( "  corresponding to %d data values.\n", (int)info.file_size/2 );
	printf( "Filtering with fs = %f, fc = %f, M = %d\noutput to \"%s\"\n",
							info.fs, info.fc, info.M, argv[2] );

	// This is the kind of comment I'm requesting --
	// Make sure a short int is 16 bits. sizeof() is an operator in C and is
	// built-in.  It is not a library function so doesn't need to be included
	// It returns a value of type size_t, which ....(fill in here)
	printf( "sizeof( short int ) = %lu\n",sizeof(short int) );

	/* .............. Create filter .................*/

	double *filter = createLowPassFilter(info.M, info.fs, info.fc);

	/* ........ Read file, filter and write file ....*/
	/* .. This starter version only just reads in the input
	      and writes it directly to the output, in blocks of 1000,
		  with no filtering   */
	// Allocate storage on the heap for an array for work
	short int *work = (short int *) malloc(1000*16);

	if( work == NULL )
	{
		printf( "Memory allocation failed. Fatal error.\n" );
		// Must clean up manually
		if( fclose(info.in) != 0 || fclose(info.out) != 0 )
			printf( "Error closing one or both files." );
		exit(EXIT_FAILURE);
	}

	int n;
	// dividing by 2 because we have 2 bytes per data value
	n = info.file_size/2;
	int pos = info.M - 1;
	int i;
	for( i = 0; i < n; i++ )
	{
		// fread reads in the file and returns the number of
		// items read successfully
		int v = fread(work, 2, 1, info.in);
		if( v != 1 )
		{
			// Since v did not equal 1, the file reading did
			// not work... Quit the program.
			printf("A fatal file reading error has occured");
			exit(EXIT_FAILURE);
		}

		double calc = 0.0;

		int j;
		for( j = 0; j < info.M; j++ )
		{
			calc += filter[j] * work[( pos - j + info.M ) % info.M];
		}
		work[pos] = calc;
		pos = ( pos + 1 ) % info.M;
		fwrite( work, 2, 1, info.out );
	}

	/* ................. Clean up ....................*/
	// Freeing work results in an error:
	//	free(): invalid next size (normal): <INSERT MEMORY ADDRESS HERE> *** Aborted (core dumped)
	// Not sure why...
	// free( work );
	free( filter );

	if( fclose(info.in) != 0 || fclose(info.out) != 0 )
		printf( "Error closing one or both files." );
	exit(EXIT_SUCCESS);
}

/* ......... Filter Function ........... */
double *createLowPassFilter( int length, double sampleRate, double cutoffFreq)
{
	// Malloc allocates some space for the array
	double *filter = malloc(length);

	// Check to see if we were able to allocate enough memory
	if(filter == NULL)
	{
		printf( "Memory allocation failed. Fatal Error." );
		exit(EXIT_FAILURE);
	}

	int mid = ( length - 1) / 2;
	double lam = cutoffFreq * PI / sampleRate;

	// Have to declare the counter before the For loop in Ansi C
	int i;
	for(i = 0; i < mid; i++)
	{
		filter[i] = sin( (i - mid) * lam ) / ( (i - mid) * PI);
		filter[length - 1 - i] = filter[i];
	}

	return filter;
};

/* ......... Print usage statement ......... */
void print_usage( void )
{
	printf( "Problem with input arguments. Use like this:\n" );
	printf( "$ ./a.out in.raw out.raw 44100 1250 101" );
}

/* ......... Parse command line arguments into data.  Place
			 parameters into a struct. */
struct run_info get_arguments_files( int argc, char * argv[] )
{
	struct run_info info;
	struct stat file_status;
	if ( stat(argv[1], &file_status) != 0) 
	{
		printf( "System could not get file status information." );
		exit(EXIT_FAILURE);
	}
     
	info.file_size = file_status.st_size;

	if( (info.in = fopen( argv[1], "rb" )) == NULL )
	{
		printf( "Could not open input file \"%s\" for reading.", argv[1] );
		exit(EXIT_FAILURE);
	}

	if( (info.out = fopen( argv[2], "wb" )) == NULL )
	{
		printf( "Could not open or create output file \"%s\" for writing.", argv[2] );
		exit(EXIT_FAILURE);
	}

	char * end;
	if( (info.fs = strtod( argv[3], &end )) == 0 )
	{
		printf( "Could not parse fs parameter." );
		exit(EXIT_FAILURE);
	}
	if( (info.fc = strtod( argv[4], &end )) == 0 )
	{
		printf( "Could not parse fs parameter." );
		exit(EXIT_FAILURE);
	}
	if( (info.M = strtod( argv[5], &end )) == 0 )
	{
		printf( "Could not parse fs parameter." );
		exit(EXIT_FAILURE);
	}
	return info;
}
