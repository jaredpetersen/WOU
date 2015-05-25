import java.io.BufferedReader;
import java.io.IOException;
import java.io.InputStreamReader;

public class UserInterface 
{

	/**
	 * User interface for the N Queens Solver
	 * 
	 * Archive File Name: JTPL2
	 * @author Jared Petersen
	 */
	public void start()
	{
		printIntro();
		getInput();
	}
	
	/**
	 * Print the intro message to the console
	 */
	private static void printIntro()
	{
		System.out.println("*************************************************");
		System.out.println("NQueens Solver");
		System.out.println("Created by Jared Petersen");
		System.out.println("*************************************************\n");
	}
	
	private void getInput()
	{
		try
		{
			InputStreamReader isr = new InputStreamReader(System.in);
			BufferedReader br = new BufferedReader(isr);
			System.out.println("Please type the size of the chess board:");
			try 
			{
				// Get the user input
				int boardSize = Integer.parseInt(br.readLine());
				
				if (boardSize == 0)
				{
					// Board cannot exist
					throw new NumberFormatException();
				}
				
				// Solve the NQueens problem for the input
				NQueens nQueens = new NQueens(boardSize);
				nQueens.placeQueens(boardSize);
			}
			// Catches both non-integer values and negative integers
			catch(NumberFormatException|NegativeArraySizeException inputException)
			{
				// User did not input a valid int, let's try this again
				System.out.println("Not a valid board size");
				getInput();
			}
			
			// Close the application
			System.exit(0);
		}
		catch (IOException ioe)
		{
			ioe.printStackTrace();
			System.exit(1);
		}
	}
}
