using System;
using System.Diagnostics; // For the stopwatch
using System.Threading.Tasks; // For the Parallel

namespace Lab3
{
    class Program
    {
        static void Main(string[] args)
        {
            // Create an array and fill it
            int[] intArray = new int[1000];
            intArray = fillArray(intArray);

            // Perform Sieve of Eratosthenes
            SeqSieve(intArray);

            // Finished, let the user read the response
            System.Console.WriteLine("\nPress Any Key to Exit");
            Console.ReadKey();
        }

        // Fill the array with ones
        static int[] fillArray(int[] intArray)
        {
            for (int i = 0; i < intArray.Length; i++)
            {
                if (i == 0 || i == 1) intArray[i] = 0;
                else intArray[i] = 1;
            }

            return intArray;
        }

        // Sequential Sieve of Eratosthenes
        static void SeqSieve(int[] intArray)
        {
            // Array address is the number, content is its status as a prime

            // Loop over the numbers (for prime searching)
            for (int i = 2; i < intArray.Length; i++)
            {
                // Don't bother looking if we already know the number is not a prime
                if (intArray[i] != 0)
                {
                    // Look at the rest of the array to see if it is a 
                    // multiple of the number we are looking at
                    for (int j = i + 1; j < intArray.Length; j++)
                    {
                        // Check if multiple
                        if (j % i == 0)
                        {
                            // Is a multiple, mark it as such
                            intArray[j] = 0;
                        }
                    }
                }
            }

            // Loop over our array and only print out the primes
            for (int k = 0; k < intArray.Length; k++)
            {
                // Check if prime
                if (intArray[k] == 1)
                {
                    // Is a prime, print it out
                    Console.WriteLine(k);
                }
            }
        }
    }
}
