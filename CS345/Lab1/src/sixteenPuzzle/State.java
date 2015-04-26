package sixteenPuzzle;

import java.util.Arrays;

/**
 * 16 Puzzle States
 * 
 * The puzzle is a 16 integer array
 * { 0   1   2   3
 *   4   5   6   7
 *   8   9   10  11
 *   12  13  14  15 }
 * where 0 represents the blank space in the puzzle
 * 
 * @author Jared Petersen
 **/
public class State
{
	// Puzzle state
	private int[] puzzle = new int[16];
	// Location of the 0 in the puzzle
	private int spaceIndex;
	// Moves that have been executed
	private String moves = "";
	
	/**
	 * State constructor
	 * Just initialize the object fields upon initializing
	 * 
	 * @param puzzle Array that stores the state of the puzzle
	 * @param spaceIndex Index of the empty space in the puzzle array
	 * @param moves The moves that have been executed
	 */
	public State(int[] puzzle, int spaceIndex, String moves)
	{
		this.puzzle = puzzle;
		this.spaceIndex = spaceIndex;
		this.moves = moves;
	}
	
	/**
	 * Get the current puzzle
	 * @return The current puzzle
	 **/
	public int[] getPuzzle()
	{
		return puzzle;
	}
	
	/**
	 * Get the moves that have been executed
	 * @return The moves that have been executed
	 **/
	public String getMoves()
	{
		return moves;
	}
	
	/**
	 * Get the index of the empty space in the puzzle array
	 * @return Index of the empty space in the puzzle array
	 **/
	public int getSpaceIndex()
	{
		return spaceIndex;
	}
	
	/**
	 * Set the index of the empty space in the puzzle array
	 */
	public void setSpaceIndex(int newIndex)
	{
		spaceIndex = newIndex;
	}
	
	/**
	 * Check to see if moving the space up is a valid move
	 * @return True if the moving the space up is a valid move
	 */
	public boolean upValid()
	{
		if (getSpaceIndex() > 3)
		{
			if (getMoves().length() == 0)
			{
				return true;
			}
			else if (getMoves().charAt(getMoves().length() - 2) != 'D')
			{
				return true;
			}
			else
			{
				return false;
			}
		}
		else
		{
			return false;
		}
	}
	
	/**
	 * Move the space up one
	 **/
	public void up()
	{
		// Variables necessary for the swap
		int spaceIndex = getSpaceIndex();
		int newSpaceIndex = spaceIndex - 4;
		int swapValue = puzzle[newSpaceIndex];
		
		// Swap the necessary indexes in accordance to the rules
		puzzle[newSpaceIndex] = 0;
		puzzle[spaceIndex] = swapValue;
		
		// Add the executed move to the moves list
		addMove("U");
		
		// Update the space index
		setSpaceIndex(newSpaceIndex);
	}
	
	/**
	 * Check to see if moving the space down is a valid move
	 * @return True if the moving the space down is a valid move
	 */
	public boolean downValid()
	{
		if (getSpaceIndex() < 12)
		{
			if (getMoves().length() == 0)
			{
				return true;
			}
			else if (getMoves().charAt(getMoves().length() - 2) != 'U')
			{
				return true;
			}
			else
			{
				return false;
			}
		}
		else
		{
			return false;
		}
	}
	
	/**
	 * Move the space down one
	 **/
	public void down()
	{
		// Variables necessary for the swap
		int spaceIndex = getSpaceIndex();
		int newSpaceIndex = spaceIndex + 4;
		int swapValue = puzzle[newSpaceIndex];
		
		// Swap the necessary indexes in accordance to the rules
		puzzle[newSpaceIndex] = 0;
		puzzle[spaceIndex] = swapValue;
		
		// Add the executed move to the moves list
		addMove("D");
		
		// Update the space index
		setSpaceIndex(newSpaceIndex);
	}
	
	/**
	 * Check to see if moving the space left is a valid move
	 * @return True if the moving the space left is a valid move
	 */
	public boolean leftValid()
	{
		if (getSpaceIndex() % 4 != 0)
		{
			if (getMoves().length() == 0)
			{
				return true;
			}
			else if (getMoves().charAt(getMoves().length() - 2) != 'R')
			{
				return true;
			}
			else
			{
				return false;
			}
		}
		else
		{
			return false;
		}
	}
	
	/**
	 * Move the space left one
	 **/
	public void left()
	{	
		// Variables necessary for the swap
		int spaceIndex = getSpaceIndex();
		int newSpaceIndex = spaceIndex - 1;
		int swapValue = puzzle[newSpaceIndex];
		
		// Swap the necessary indexes in accordance to the rules
		puzzle[newSpaceIndex] = 0;
		puzzle[spaceIndex] = swapValue;
		
		// Add the executed move to the moves list
		addMove("L");
		
		// Update the space index
		setSpaceIndex(newSpaceIndex);
	}
	
	/**
	 * Check to see if moving the space right is a valid move
	 * @return True if the moving the space right is a valid move
	 */
	public boolean rightValid()
	{
		if (getSpaceIndex() % 4 != 3)
		{
			if (getMoves().length() == 0)
			{
				return true;
			}
			else if (getMoves().charAt(getMoves().length() - 2) != 'L')
			{
				return true;
			}
			else
			{
				return false;
			}
		}
		else
		{
			return false;
		}
	}
	
	/**
	 * Move the space right one
	 **/
	public void right()
	{
		// Variables necessary for the swap
		int spaceIndex = getSpaceIndex();
		int newSpaceIndex = spaceIndex + 1;
		int swapValue = puzzle[newSpaceIndex];
		
		// Swap the necessary indexes in accordance to the rules
		puzzle[newSpaceIndex] = 0;
		puzzle[spaceIndex] = swapValue;
		
		// Add the executed move to the moves list
		addMove("R");
		
		// Update the space index
		setSpaceIndex(newSpaceIndex);
	}
	
	/**
	 * Add move to the moves list
	 * @param String newMove The move that will be added to the list
	 */
	private void addMove(String newMove)
	{
		moves = moves + newMove + ',';
	}
	
	/**
	 * Check to see if the puzzle has reached a goal state
	 * @return True if the puzzle has been solved
	 **/
	public boolean isGoalState()
	{
		// Puzzle goal state
		int[] goalState = {0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15};
		
		// Check to see if the puzzle's state is equal to the goal state
		//if (Arrays.deepEquals(puzzle, goalState))
		if (Arrays.equals(puzzle, goalState))
		{
			return true;
		}
		else
		{
			return false;
		}
	}
	
	/**
	 * Print out the moves
	 **/
	public void printMoves()
	{
		// Print out the moves (minus the extra comma and space at the end)
		System.out.println(getMoves().substring(0, getMoves().length() - 1));
	}
	
	/**
	 * Print out the puzzle state
	 */
	public void printState()
	{
		System.out.println("{ " + puzzle[0] + " " + puzzle[1] + " " + puzzle[2] + " " + puzzle[3]);
		System.out.println("  " + puzzle[4] + " " + puzzle[5] + " " + puzzle[6] + " " + puzzle[7]);
		System.out.println("  " + puzzle[8] + " " + puzzle[9] + " " + puzzle[10] + " " + puzzle[11]);
		System.out.println("  " + puzzle[12] + " " + puzzle[13] + " " + puzzle[14] + " " + puzzle[15] + " }");
		System.out.println();
	}
	
}
