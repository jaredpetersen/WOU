// systemcalls.c
// Jared Petersen

// Simple program that takes advantage of the Linux system calls fork() and pid()

#include <stdio.h>
#include <stdlib.h>
#include <unistd.h>
#include <sys/types.h>
#include <sys/wait.h>
#include <time.h>

// Function prototypes
void generate_random(void);

int main(void)
{
	printf( "\nIn this program, the parent process controls\n"
		"the output of the program status messages.\n"
		"Random number generation is handled by the\n"
		"child process.\n\n"
		"------- Key -------\n"
                "p = parent process\n"
                "c = child process\n"
                "-------------------\n\n");
	
	// Fork the program, creating a child process
	pid_t pid = fork();
	
	if (pid == 0)
	{
		// Child process
		// Generate 10 random numbers between 0 and 100
		int i;
		for (i = 0; i <= 5; i++)
		{
			generate_random();
			if (i < 5)
			{
				// Only sleep between random number generation
				// Have to sleep in order to have proper random number generation
				sleep(1);
			}
		}
		printf("\n");
	}
	else if (pid > 0)
	{
		// Parent Process
		// Print the program status text
		printf("p: Generating random numbers...\n\n");

		// Wait for child process to finish...
		int returnStatus;
		waitpid(0, &returnStatus, 0);

		if (returnStatus == 0)
		{
			// Child process executed successfully
			printf("p: Random number generation complete\n\n");
		}
		else
		{
			// Child process encountered an error somewhere
			printf("ERROR: Child process encountered an error\n\n");
		}
	}
	else
	{
		printf("ERROR: System call \"fork()\" failed\n\n");
		return 1;
	}

	return 0;
}

void generate_random(void)
{
	srand(time(NULL));
	int randomNumber = rand();
	printf("c: Random Number = %d\n", randomNumber);
}
