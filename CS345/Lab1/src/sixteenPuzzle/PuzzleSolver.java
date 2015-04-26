package sixteenPuzzle;

/**
 * 16 Puzzle Solver
 * 
 * @author Jared Petersen
 **/
public class PuzzleSolver 
{
	// Puzzle setup variables
	Queue queue = new Queue();
	int[] initialPuzzle;
	State state;
	// Puzzle solution variables
	State puzzleSolution;
	boolean puzzleSolved = false;
	
	/**
	 * Constructor
	 */
	public PuzzleSolver(int[] initialPuzzle, int zeroIndex)
	{
		// Set up the puzzle
		queue = new Queue();
		this.initialPuzzle = initialPuzzle;
		// Add the initial state
		state = new State(this.initialPuzzle, zeroIndex, "");
		queue.add(state);
	}
	
	/**
	 * Solve the puzzle
	 */
	public boolean solvePuzzle()
	{
		// Begin Solving Puzzle
		while(!queue.isEmpty() && !puzzleSolved)
		{
			// Remove the state from the queue
			State queueState = queue.remove();
			
			// Check if the space can be moved
			if (queueState.upValid())
			{
				State queueStateClone = new State(queueState.getPuzzle().clone(), queueState.getSpaceIndex(), queueState.getMoves());
				queueStateClone.up();
				
				// Check if the new move results in the goal state
				if (queueStateClone.isGoalState())
				{
					// A solution has been found
					puzzleSolution = queueStateClone;
					puzzleSolved = true;
				}
				else
				{
					// A solution has not been found
					// Add the state to the queue for further processing
					queue.add(queueStateClone);
				}
			}
			
			if (queueState.downValid())
			{
				State queueStateClone = new State(queueState.getPuzzle().clone(), queueState.getSpaceIndex(), queueState.getMoves());
				queueStateClone.down();
				
				// Check if the new move results in the goal state
				if (queueStateClone.isGoalState())
				{
					// A solution has been found
					puzzleSolution = queueStateClone;
					puzzleSolved = true;
				}
				else
				{
					// A solution has not been found
					// Add the state to the queue for further processing
					queue.add(queueStateClone);
				}
			}
			
			if (queueState.leftValid())
			{
				State queueStateClone = new State(queueState.getPuzzle().clone(), queueState.getSpaceIndex(), queueState.getMoves());
				queueStateClone.left();
				
				// Check if the new move results in the goal state
				if (queueStateClone.isGoalState())
				{
					// A solution has been found
					puzzleSolution = queueStateClone;
					puzzleSolved = true;
				}
				else
				{
					// A solution has not been found
					// Add the state to the queue for further processing
					queue.add(queueStateClone);
				}
			}
			
			if (queueState.rightValid())
			{
				State queueStateClone = new State(queueState.getPuzzle().clone(), queueState.getSpaceIndex(), queueState.getMoves());
				queueStateClone.right();
				
				// Check if the new move results in the goal state
				if (queueStateClone.isGoalState())
				{
					// A solution has been found
					puzzleSolution = queueStateClone;
					puzzleSolved = true;
				}
				else
				{
					// A solution has not been found
					// Add the state to the queue for further processing
					queue.add(queueStateClone);
				}
			}
		}
		
		System.out.println("Puzzle Solved: ");
		puzzleSolution.printMoves();
		
		return puzzleSolved;
	}
	
	public String getMoves()
	{
		return puzzleSolution.getMoves();
	}
}
