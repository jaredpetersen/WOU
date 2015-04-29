package test;

import static org.junit.Assert.*;
import java.util.Arrays;

import org.junit.Before;
import org.junit.Test;

import sixteenPuzzle.State;

/**
 * Confirm the operators move the squares around as expected
 * 
 * Archive File Name: JTPL1
 * @author Jared Petersen
 */
public class StateOperators 
{
	// Set up the initial testing states
	State upState;
	State downState;
	State leftState;
	State rightState;
	State upValid;
	State downValid;
	State leftValid;
	State rightValid;
	// Set up the initial puzzle
	int[] initialPuzzle = {4, 1, 2, 3, 5, 6, 10, 7, 8, 0, 9, 11, 12, 13, 14, 15};
	
	@Before
	public void setUp() throws Exception
	{
		// Initialize all of the test states
		upState = new State(initialPuzzle, 9, "");
		downState = new State(initialPuzzle, 9, "");
		leftState = new State(initialPuzzle, 9, "");
		rightState = new State(initialPuzzle, 9, "");
		upValid = new State(initialPuzzle, 9, "");
		downValid = new State(initialPuzzle, 9, "");
		leftValid = new State(initialPuzzle, 9, "");
		rightValid = new State(initialPuzzle, 9, "");
	}

	@Test
	public void moveUp()
	{
		// Desired result
		int[] solutionState = {4, 1, 2, 3, 5, 0, 10, 7, 8, 6, 9, 11, 12, 13, 14, 15};
		
		// Move the 0 spot up
		upState.up();
		
		// Check if the output matches the desired result
		if (!Arrays.equals(solutionState, upState.getPuzzle()))
		{
			fail();
		}
	}
	
	@Test
	public void moveDown()
	{
		// Desired result
		int[] solutionState = {4, 1, 2, 3, 5, 6, 10, 7, 8, 13, 9, 11, 12, 0, 14, 15};
		
		// Move the 0 spot down
		downState.down();
		
		// Check if the output matches the desired result
		if (!Arrays.equals(solutionState, downState.getPuzzle()))
		{
			fail();
		}
	}
	
	@Test
	public void moveLeft() 
	{
		// Desired result
		int[] solutionState = {4, 1, 2, 3, 5, 6, 10, 7, 0, 8, 9, 11, 12, 13, 14, 15};
		
		// Move the 0 spot left
		leftState.left();
		
		/// Check if the output matches the desired result
		if (!Arrays.equals(solutionState, leftState.getPuzzle()))
		{
			fail();
		}
	}
	
	@Test
	public void moveRight() 
	{
		// Desired result
		int[] solutionState = {4, 1, 2, 3, 5, 6, 10, 7, 8, 9, 0, 11, 12, 13, 14, 15};
		
		// Move the 0 spot right
		rightState.right();
		
		// Check if the output matches the desired result
		if (!Arrays.equals(solutionState, rightState.getPuzzle()))
		{
			fail();
		}
	}
	
	@Test
	public void validUp() 
	{
		// Move the zero space down
		upValid.down();
		// Check if moving it back is valid
		if (upValid.upValid())
		{
			fail();
		}
	}
	
	@Test
	public void validDown() 
	{
		// Move the zero space up
		downValid.up();
		// Check if moving it back is valid
		if (downValid.downValid())
		{
			fail();
		}
	}
	
	@Test
	public void validLeft() 
	{
		// Move the zero space right
		leftValid.right();
		// Check if moving it back is valid
		if (leftValid.leftValid())
		{
			fail();
		}
	}
	
	@Test
	public void validRight() 
	{
		// Move the zero space left
		rightValid.left();
		// Check if moving it back is valid
		if (rightValid.rightValid())
		{
			fail();
		}
	}

}
