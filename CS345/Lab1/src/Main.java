/**
 * 16 Puzzle Solver
 * Uses state-based search to solve the 16 puzzle
 * 
 * @author Jared Petersen
 **/
public class Main 
{
	// Fields
	static State puzzleSolution;
	static boolean puzzleSolved = false;

	/**
	 * Main method, entry point to the application
	 **/
	public static void main(String[] args) {
		
		Queue queue = new Queue();
		Integer[] initialPuzzle = {4, 1, 2, 3, 5, 6, 10, 7, 8, 0, 9, 11, 12, 13, 14, 15};
		State state = new State(initialPuzzle, 9, "");
		
		// Add the initial state
		queue.add(state);
		
		//System.out.println("start:");
		//state.printState();
		
		while(!queue.isEmpty() && !puzzleSolved)
		{
			// Remove the state from the queue
			State queueState = queue.remove();
			
			// Check if the space can be moved
			if (queueState.upValid())
			{
				State queueStateClone = new State(queueState.puzzle.clone(), queueState.getSpaceIndex(), queueState.getMoves());
				queueStateClone.up();
				//queueStateClone.printState();
				
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
				State queueStateClone = new State(queueState.puzzle.clone(), queueState.getSpaceIndex(), queueState.getMoves());
				queueStateClone.down();
				//queueStateClone.printState();
				
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
				State queueStateClone = new State(queueState.puzzle.clone(), queueState.spaceIndex, queueState.moves);
				queueStateClone.left();
				//queueStateClone.printState();
				
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
				State queueStateClone = new State(queueState.puzzle.clone(), queueState.spaceIndex, queueState.moves);
				queueStateClone.right();
				//queueStateClone.printState();
				
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
		
		puzzleSolution.printMoves();
	}

}
