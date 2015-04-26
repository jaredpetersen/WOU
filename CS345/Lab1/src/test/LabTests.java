package test;

import static org.junit.Assert.*;

import org.junit.Test;

import sixteenPuzzle.PuzzleSolver;

public class LabTests {

	@Test
	public void test1() {
		int[] initialPuzzle = {4, 1, 2, 3, 5, 6, 10, 7, 8, 0, 9, 11, 12, 13, 14, 15};
		PuzzleSolver puzzleSolver = new PuzzleSolver(initialPuzzle);
		
		if (puzzleSolver.solvePuzzle() == true)
		{
			// Check to see if the puzzle solution is correct
			if (!puzzleSolver.getMoves().equals("R,U,L,L,U,"))
			{
				fail();
			}
		}
		else
		{
			fail();
		}
		
	}
	
	@Test
	public void test2() {
		int[] initialPuzzle = {1, 2, 6, 3, 4, 0, 10, 7, 8, 5, 9, 11, 12, 13, 14, 15};
		PuzzleSolver puzzleSolver = new PuzzleSolver(initialPuzzle);
		
		if (puzzleSolver.solvePuzzle() == true)
		{
			// Check to see if the puzzle solution is correct
			if (!puzzleSolver.getMoves().equals("D,R,U,U,L,L,"))
			{
				System.out.println(puzzleSolver.getMoves());
				fail();
			}
		}
		else
		{
			fail();
		}
		
	}
	
	@Test
	public void test3() {
		int[] initialPuzzle = {4, 1, 2, 3, 8, 0, 5, 7, 9, 10, 6, 11, 12, 13, 14, 15};
		PuzzleSolver puzzleSolver = new PuzzleSolver(initialPuzzle);
		
		if (puzzleSolver.solvePuzzle() == true)
		{
			// Check to see if the puzzle solution is correct
			if (!puzzleSolver.getMoves().equals("R,D,L,L,U,U,"))
			{
				System.out.println(puzzleSolver.getMoves());
				fail();
			}
		}
		else
		{
			fail();
		}
		
	}
	
	@Test
	public void test4() {
		int[] initialPuzzle = {1, 2, 6, 3, 4, 5, 0, 7, 12, 8, 10, 11, 13, 14, 9, 15};
		PuzzleSolver puzzleSolver = new PuzzleSolver(initialPuzzle);
		
		if (puzzleSolver.solvePuzzle() == true)
		{
			// Check to see if the puzzle solution is correct
			if (!puzzleSolver.getMoves().equals("D,D,L,L,U,R,R,U,U,L,L,"))
			{
				System.out.println(puzzleSolver.getMoves());
				fail();
			}
		}
		else
		{
			fail();
		}
		
	}

}
