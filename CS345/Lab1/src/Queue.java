/**
 * Queue Data Structure
 * 
 * @author Jared Petersen
 **/
public class Queue
{
	/** Fields **/
	// Queue head and tail
	Node head; // Front of the queue (exit)
	Node tail;		// Back of the queue (entrance)
	// Queue size
	int size = 0;
	
	/**
	 * Add an item to the back of the queue
	 * @param val The value that is being stored at the new queue location
	 **/
	public void add(int val)
	{
		if (size == 0)
		{
			// The queue is empty
			// Make the new node the head and the tail
			head = new Node();
			head.value = val;
			tail = head;
		}
		else if (size == 1)
		{
			// The queue only has one item in it
			// Make the new node the tail
			tail = new Node();
			tail.value = val;
			tail.next = head;
			head.prev = tail;
		}
		else
		{
			// The queue has multiple nodes in it
			
			// Put the tail node into a new node
			Node tempNode = tail;
			
			// Set the tail equal to the new node that is being inserted
			tail = new Node();
			tail.value = val;
			
			// Update the pointers
			tempNode.prev = tail;
			tail.next = tempNode;
		}
		
		// Update the queue size
		size++;
	}
	
	/**
	 * Remove the item located at the front of the queue
	 * @return The value of the queue item being removed
	 **/
	public int remove()
	{
		// Store the value of the head for output
		int headVal = head.value;
		// Set the head to equal the previous node
		head = head.prev;
		
		// Update the queue size
		size--;
		
		return headVal;
	}
	
	/**
	 * Get the size of the queue
	 * @return The size of the queue
	 **/
	public int size()
	{
		return size;
	}
	
	/**
	 * Checks to see if the queue is empty or not
	 * @return If the queue is empty or not
	 */
	public boolean isEmpty()
	{
		if (size() > 0)
		{
			// Queue has at least one item
			return false;
		}
		else 
		{
			// Queue does not have any items
			return true;
		}
	}
	
	
	/**
	 * Inner Node class
	 * Each node represents an item in the queue
	 **/
	class Node 
	{
		Node next;
		Node prev;
		int value;
	}
}
