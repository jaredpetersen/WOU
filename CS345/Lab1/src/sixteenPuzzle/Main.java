package sixteenPuzzle;

import javax.swing.*;

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
		// Initialize GUI
		JFrame f = new GUI();
	    f.setVisible(true);
		// Initialize back-end puzzle solver
	    // TODO Have the gui start the puzzle solver
		PuzzleSolver puzzleSolver = new PuzzleSolver(initialPuzzle);
		// TODO Make sure the program is accurately checking the goal state
		puzzleSolver.solvePuzzle();
		puzzleSolver.getMoves();
	}

}
