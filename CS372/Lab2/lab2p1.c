// Lab2 Part 1
// Jared Petersen

// Simple program that uses the following system commands:
//	fork()
//	wait()
//	exit()
//	exec()

#include <stdio.h>
#include <stdlib.h>
#include <unistd.h>
#include <sys/types.h>
#include <sys/wait.h>
#include <time.h>

int main(void)
{
	// Fork the program, creating a child process
	pid_t pid = fork();

	if (pid > 0)
	{
		// Parent process
		printf("Parent process\n");
		// Wait for the child process to execute
		wait(NULL);
		// Child process has been executed, close the program
		printf("Close parent process\n");
		// Could have used return 0, but I'm just trying to meet
		// lab requirements
		exit(0);
	}
	else if (pid == 0)
	{
		// Child process
		printf("Child process\n");
		// List the items in the program execution directory
		execl("/bin/ls", "ls", "-l", (char *)0);
	}
	else
	{
		// Fork() failed
		printf("ERROR: System call \"fork()\" failed\n\n");
		// Close the entire program with an error
		return 1;
	}
	// Close the program
	return 0;
}
