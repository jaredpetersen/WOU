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
	public static void main(String[] args) {
		// Just testing out the Queue class
		Queue queue = new Queue();
		queue.add(1);
		queue.add(2);
		queue.add(3);
		System.out.println(queue.size());
		queue.remove();
		System.out.println(queue.size());
	}

}
