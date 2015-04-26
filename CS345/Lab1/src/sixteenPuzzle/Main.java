package sixteenPuzzle;
/**
 * 16 Puzzle Solver
 * Uses state-based search to solve the 16 puzzle
 * 
 * @author Jared Petersen
 **/
public class Main 
{
	/**
	 * Main method, entry point to the application
	 **/
	public static void main(String[] args)
	{
		int[] initialPuzzle = {1, 2, 6, 3, 4, 0, 10, 7, 8, 5, 9, 11, 12, 13, 14, 15};
		PuzzleSolver puzzleSolver = new PuzzleSolver(initialPuzzle);
		puzzleSolver.solvePuzzle();
		puzzleSolver.getMoves();
	}

}
