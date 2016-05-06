using System;
using System.Diagnostics; // For the stopwatch
using System.Threading.Tasks; // For the Parallel

namespace Lab1
{
    class Program
    {

        static void Main(string[] args)
        {
            // Declare the fields as instructed in the lab
            const int size = 20000000;
            int[] intArray = new int[size];

            // Introduce the application
            System.Console.WriteLine("Concurrent Systems Lab 1\nJared Petersen\n");
            // Found the processor count variable from https://msdn.microsoft.com/en-us/library/system.environment.processorcount(v=vs.110).aspx
            System.Console.WriteLine("# of Processors = " + Environment.ProcessorCount + "\n");

            // Fill the integer array with random numbers
            intArray = fillRandomArray(intArray);

            // Find the largest number sequentially
            findLargestNumberSequential(intArray);

            // Find the largest number in parallel using a lock
            findLargestNumberParallelLock(intArray);

            // Find the largest number in parallel using a lock and chunking/CC
            findLargestNumberParallelLockChunk(intArray, 1);
            findLargestNumberParallelLockChunk(intArray, 10);
            findLargestNumberParallelLockChunk(intArray, 100);
            findLargestNumberParallelLockChunk(intArray, 500);
            findLargestNumberParallelLockChunk(intArray, 1000);
            findLargestNumberParallelLockChunk(intArray, 20000000);


            // Find the largest number in parallel using TLS
            findLargestNumberParallelTLS(intArray);

            // Find the largest number in parallel using TLS and chunking/CC
            findLargestNumberParallelTLSChunk(intArray, 1);
            findLargestNumberParallelTLSChunk(intArray, 10);
            findLargestNumberParallelTLSChunk(intArray, 100);
            findLargestNumberParallelTLSChunk(intArray, 500);
            findLargestNumberParallelTLSChunk(intArray, 1000);
            findLargestNumberParallelTLSChunk(intArray, 20000000);

            // Job is done, make sure to the let user see the results (
            System.Console.WriteLine("Press Any Key to Exit");
            Console.ReadKey();
        }

        // Fill the integer array with random numbers
        static int[] fillRandomArray(int[] intArray)
        {
            // Random number generator (Found from http://stackoverflow.com/questions/2706500/how-do-i-generate-a-random-int-number-in-c)
            Random rnd = new Random();

            // Iterate over the array and add random numbers to it
            for (int i = 0; i < intArray.Length; i++)
            {
                // Put the random number into the array
                intArray[i] = rnd.Next();

                // For printing the array, comment this out for > 20 units
                //System.Console.WriteLine(intArray[i]);
            }

            // For printing the array (makes it so the text looks nicer in the console), comment this out for > 20 units
            //System.Console.WriteLine();

            // Return the array so that we can continue using it
            return intArray;
        }

        // Find the largest number in the array sequentially
        static void findLargestNumberSequential(int[] intArray)
        {
            // Largest number in the array
            int largestNumber = intArray[0];

            // Let the user know that we're starting to look
            System.Console.WriteLine("SEQUENTIAL - Executing Now");

            // Start counting the time (from the lab example)
            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();

            for (int i = 1; i < intArray.Length; i++)
            {
                // Check if it's the largest number
                if (intArray[i] > largestNumber)
                {
                    // Found the largest number
                    largestNumber = intArray[i];
                }
            }

            // Stop counting the time
            stopwatch.Stop();

            // Notify the user of the results
            System.Console.WriteLine("SEQUENTIAL - Largest Number: " + largestNumber);
            System.Console.WriteLine("SEQUENTIAL - Execution Time: " + stopwatch.ElapsedMilliseconds + "\n");
        }

        // Find the largest number in the array in parallel
        static void findLargestNumberParallelLock(int[] intArray)
        {
            // Largest number in the array
            int largestNumber = intArray[0];

            // Set up the Parallel loop for full parallelism
            ParallelOptions options = new ParallelOptions();
            options.MaxDegreeOfParallelism = -1;

            // Set up the object lock
            Object obj = new Object();

            // Let the user know that we're starting to look
            System.Console.WriteLine("PARALLEL LOCK - Executing Now");

            // Start counting the time (from the lab example)
            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();

            // Loop over the data in parallel using a Parallel For and Lock
            Parallel.For(0, intArray.Length, options, i =>
            {
                // Critical section
                lock (obj)
                {
                    // Check if it's the largest number
                    if (intArray[i] > largestNumber)
                    {
                        // Found the new local biggest number
                        largestNumber = intArray[i];
                    }
                }
            });

            // Stop counting the time
            stopwatch.Stop();

            // Notify the user of the results
            System.Console.WriteLine("PARALLEL LOCK - Largest Number: " + largestNumber);
            System.Console.WriteLine("PARALLEL LOCK - Execution Time: " + stopwatch.ElapsedMilliseconds + "\n");
        }

        // Find the largest number in the array in parallel
        static void findLargestNumberParallelLockChunk(int[] intArray, int chunk)
        {
            // Convert the chunk to deal with the per core count
            // eg - 100 chunks per CC with 2 cores -> 200 chunk
            int newChunk = chunk * Environment.ProcessorCount;

            // Chunking breaks if the chunk is more than the actual size of the array
            if (newChunk > intArray.Length)
            {
                newChunk = intArray.Length;
            }

            // Largest number in the array
            int largestNumber = intArray[0];

            // Set up the Parallel loop for full parallelism
            ParallelOptions options = new ParallelOptions();
            options.MaxDegreeOfParallelism = -1;

            // Set up the object lock
            Object obj = new Object();

            // Let the user know that we're starting to look
            System.Console.WriteLine("PARALLEL LOCK CHUNK (" + chunk + ") - Executing Now");

            // Start counting the time (from the lab example)
            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();

            // Loop over the data in parallel using a Parallel For and Lock
            // Chunking logic taken from the lab example
            Parallel.For(0, intArray.Length / newChunk, options, ii =>
            {
                int nSIZE = (ii + 1) * newChunk;
                for (int i = ii * newChunk; i < nSIZE; i++)
                {
                    // Critical section
                    lock (obj)
                    {
                        // Check if it's the largest number
                        if (intArray[i] > largestNumber)
                        {
                            // Found the new local biggest number
                            largestNumber = intArray[i];
                        }
                    }
                }
            });

            // Stop counting the time
            stopwatch.Stop();

            // Notify the user of the results
            System.Console.WriteLine("PARALLEL LOCK CHUNK (" + chunk + ") - Largest Number: " + largestNumber);
            System.Console.WriteLine("PARALLEL LOCK CHUNK (" + chunk + ") - Execution Time: " + stopwatch.ElapsedMilliseconds + "\n");
        }

        // Find the largest number in the array in parallel using TLS
        static void findLargestNumberParallelTLS(int[] intArray)
        {
            // Largest number in the array
            int largestNumber = intArray[0];

            // Set up the Parallel loop for full parallelism
            ParallelOptions options = new ParallelOptions();
            options.MaxDegreeOfParallelism = -1;

            // Set up the object lock
            Object obj = new Object();

            // Let the user know that we're starting to look
            System.Console.WriteLine("PARALLEL TLS - Executing Now");

            // Start counting the time (from the lab example)
            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();

            // Loop over the data in parallel using thread local storage
            // Got a starting point on this from the slides
            // nIdx = index for the thread
            // nTLSValue = largest number in the thread
            Parallel.For(0, intArray.Length, options, () => intArray[0], (int nIdx, ParallelLoopState ls, int nTLSValue) =>
            {
                // Check if it's the largest number
                if (intArray[nIdx] > nTLSValue)
                {
                    // Found the new local biggest number
                    nTLSValue = intArray[nIdx];
                }

                // Continue searching with the new local biggest number
                return nTLSValue;
            },
                // Have to use lock here since there isn't an interlocked operation that will make this work
            value => { lock (obj) { if (value > largestNumber) largestNumber = value; } });

            // Stop counting the time
            stopwatch.Stop();

            // Notify the user of the results
            System.Console.WriteLine("PARALLEL TLS - Largest Number: " + largestNumber);
            System.Console.WriteLine("PARALLEL TLS - Execution Time: " + stopwatch.ElapsedMilliseconds + "\n");
        }

        // Find the largest number in the array in parallel using TLS and chunking
        static void findLargestNumberParallelTLSChunk(int[] intArray, int chunk)
        {
            // Convert the chunk to deal with the per core count
            // eg - 100 chunks per CC with 2 cores -> 200 chunk
            int newChunk = chunk * Environment.ProcessorCount;

            // Chunking breaks if the chunk is more than the actual size of the array
            if (newChunk > intArray.Length)
            {
                newChunk = intArray.Length;
            }

            // Largest number in the array
            int largestNumber = intArray[0];

            // Set up the Parallel loop for full parallelism
            ParallelOptions options = new ParallelOptions();
            options.MaxDegreeOfParallelism = -1;

            // Set up the object lock
            Object obj = new Object();

            // Let the user know that we're starting to look
            System.Console.WriteLine("PARALLEL TLS CHUNK (" + chunk + ") - Executing Now");

            // Start counting the time (from the lab example)
            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();

            // Loop over the data in parallel using thread local storage
            // Got a starting point on this from the slides
            // nIdx = index for the thread
            // nTLSValue = largest number in the thread
            Parallel.For(0, intArray.Length / newChunk, options, () => intArray[0], (int nIdx, ParallelLoopState ls, int nTLSValue) =>
            {
                int nSIZE = (nIdx + 1) * newChunk;
                for (int i = nIdx * newChunk; i < nSIZE; i++)
                {
                    // Check if it's the largest number
                    if (intArray[i] > nTLSValue)
                    {
                        // Found the new local biggest number
                        nTLSValue = intArray[i];
                    }
                }

                // Continue searching with the new local biggest number
                return nTLSValue;
            },
                // Have to use lock here since there isn't an interlocked operation that will make this work
            value => { lock (obj) { if (value > largestNumber) largestNumber = value; } });

            // Stop counting the time
            stopwatch.Stop();

            // Notify the user of the results
            System.Console.WriteLine("PARALLEL TLS CHUNK (" + chunk + ") - Largest Number: " + largestNumber);
            System.Console.WriteLine("PARALLEL TLS CHUNK (" + chunk + ") - Execution Time: " + stopwatch.ElapsedMilliseconds + "\n");
        }

    }
}
