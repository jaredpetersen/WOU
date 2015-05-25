import java.util.Arrays;

/**
 * N Queens Solver
 * 
 * Archive File Name: JTPL2
 * @author Jared Petersen
 */
public class NQueens 
{
	/* Chess Board
	 * Assuming n = 4, this is the solution
	 * 
	 * 3| |Q| | |
	 * 2| | | |Q|
	 * 1|Q| | | |
	 * 0| | |Q| |
	 * 
	 * Solution output: [1, 3, 0, 2]
	 */
	private int[] board;
	
	/**
	 * Constructor
	 * @param n The size of the board
	 */
	public NQueens(int n)
	{
		// Create the board
		board = new int[n];
	}
	
	/**
	 * Check if you can put a queen in the given space
	 * @param k Given chess board space
	 * @return The position's validity
	 */
	private boolean validPosition(int k)
	{
		for (int i = 0; i <= k - 1; i++)
		{
			// If two queens are on the same row or same diagonal
			if ((board[i] == board[k]) || (Math.abs(board[i] - board[k]) == Math.abs(i - k)))
			{
				return false;
			}
		}
		return true;
	}
	
	/**
	 * Solve the N Queens problem
	 * @param n The size of the board
	 */
	public void placeQueens(int n)
	{
		board[0] = 0;
		int k = 0; // Start with Queen 0 on row 0
				
		while (k < n)
		{
			while ((k < n) && (validPosition(k) == false))
			{
				// Advance this queen one row
				board[k] = board[k] + 1;
			}
						
			if ((k == n - 1) && (board[k] < n))
			{
				// All the queens are validly placed
				System.out.println("Solution: " + Arrays.toString(board));
				break;
			}
			else if ((k < n - 1) && (board[k] < n))
			{
				// Try to place the next queen
				k = k + 1;
				board[k] = 0;
			}
			else
			{
				k = k - 1;
				if (k < 0)
				{
					System.out.println("No solutions possible");
					break;
				}
				else
				{
					// Advance the backtracked queen one space
					board[k] = board[k] + 1;
				}
			}
		}
	}
}
