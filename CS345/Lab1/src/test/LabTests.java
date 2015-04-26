package test;

import static org.junit.Assert.*;

import org.junit.Test;

import sixteenPuzzle.PuzzleSolver;

/**
 * Test the cases listed in the lab assignment
 * @author Jared Petersen
 */
public class LabTests {

	@Test
	public void test1() {
		// Set up the initial puzzle
		int[] initialPuzzle = {4, 1, 2, 3, 5, 6, 10, 7, 8, 0, 9, 11, 12, 13, 14, 15};
		// Set up the puzzle solver
		PuzzleSolver puzzleSolver = new PuzzleSolver(initialPuzzle, 9);
		
		// Solve the puzzle and make sure that the moves used is the desired solution
		if (puzzleSolver.solvePuzzle() == false || 
			!puzzleSolver.getMoves().equals("R,U,L,L,U"))
		{
			fail();
		}
		
	}
	
	@Test
	public void test2() {
		// Set up the initial puzzle
		int[] initialPuzzle = {1, 2, 6, 3, 4, 0, 10, 7, 8, 5, 9, 11, 12, 13, 14, 15};
		// Set up the puzzle solver
		PuzzleSolver puzzleSolver = new PuzzleSolver(initialPuzzle, 5);
		
		// Solve the puzzle
		if (puzzleSolver.solvePuzzle() == false || 
			!puzzleSolver.getMoves().equals("D,R,U,U,L,L"))
		{
			fail();
		}
		
	}
	
	@Test
	public void test3() {
		// Set up the initial puzzle
		int[] initialPuzzle = {4, 1, 2, 3, 8, 0, 5, 7, 9, 10, 6, 11, 12, 13, 14, 15};
		// Set up the puzzle solver
		PuzzleSolver puzzleSolver = new PuzzleSolver(initialPuzzle, 5);
		
		// Solve the puzzle
		if (puzzleSolver.solvePuzzle() == false || 
			!puzzleSolver.getMoves().equals("R,D,L,L,U,U"))
		{
			fail();
		}
		
	}
	
	@Test
	public void test4() {
		// Set up the initial puzzle
		int[] initialPuzzle = {1, 2, 6, 3, 4, 5, 0, 7, 12, 8, 10, 11, 13, 14, 9, 15};
		// Set up the puzzle solver
		PuzzleSolver puzzleSolver = new PuzzleSolver(initialPuzzle, 6);
		
		// Solve the puzzle
		if (puzzleSolver.solvePuzzle() == false || 
			!puzzleSolver.getMoves().equals("D,D,L,L,U,R,R,U,U,L,L"))
		{
			fail();
		}
		
	}

}
