/**
 * 16 Puzzle States
 * 
 * @author Jared Petersen
 **/
public class State 
{
	/** Fields **/
	/*
	 * The puzzle goes like this:
	 * [ 0   1   2   3
	 *   4   5   6   7
	 *   8   9   10  11
	 *   12  13  14  15 ]
	 * where 0 represents the blank space in the puzzle
	 */
	Integer[] puzzle = new Integer[16];
	// The location of the 0 in the puzzle
	int spaceIndex;
	// Moves that have been executed
	String moves = "";
	
	/**
	 * State constructor
	 * Just initialize the object fields upon initializing
	 * 
	 * @param puzzle Array that stores the state of the puzzle
	 * @param spaceIndex Index of the empty space in the puzzle array
	 * @param moves The moves that have been executed
	 */
	public State(Integer[] puzzle, int spaceIndex, String moves)
	{
		this.puzzle = puzzle;
		this.spaceIndex = spaceIndex;
		this.moves = moves;
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
	 * Move the space up one
	 **/
	public void up()
	{
		// TO DO
	}
	
	/**
	 * Move the space down one
	 **/
	public void down()
	{
		// TO DO
	}
	
	/**
	 * Move the space left one
	 **/
	public void left()
	{
		// TO DO
	}
	
	/**
	 * Move the space right one
	 **/
	public void right()
	{
		// TO DO
	}
	
	
	
	/**
	 * Check to see if the puzzle has reached a goal state
	 * @return
	 **/
	public boolean isGoalState()
	{
		// TO DO
		return false;
	}
	
	/**
	 * Print out the moves
	 **/
	public void printMoves()
	{
		// Print out the moves (minus the extra comma and space at the end)
		System.out.println(moves.substring(0, moves.length() - 2));
	}
	
}
