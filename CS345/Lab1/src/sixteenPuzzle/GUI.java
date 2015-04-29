package sixteenPuzzle;

import java.awt.Dimension;
import java.awt.BorderLayout;
import java.awt.event.ActionEvent;
import java.awt.event.ActionListener;

import javax.swing.*;

/**
 * User interface for the 16 Puzzle Solver
 * 
 * Archive File Name: JTPL1
 * @author Jared Petersen
 */
@SuppressWarnings("serial")
public class GUI extends JFrame implements ActionListener
{
	// Puzzle solver variables
	private int[] initialPuzzle = new int[16];
	private Integer zeroIndex;
	// GUI input text field for puzzle state
	private JTextField puzzleInputTextField;
	
	/**
	 * GUI Constructor
	 */
	public GUI()
	{
		// Create window with title
		setTitle("16 Puzzle");
		setSize(370, 420);
		
		// Set the location of the window to the center of the screen
        setLocationRelativeTo(null);
        
        // Do not allow the user to resize the window
        // We don't want to have to worry about resizing everything
        setResizable(false);
        
        // Close the application completely when the user hits "X"
        setDefaultCloseOperation(EXIT_ON_CLOSE);
        
        // Add the puzzle display JPanel
        GUIDrawPanel dpnl = new GUIDrawPanel();
        dpnl.setPreferredSize(new Dimension(370, 370));
        add(dpnl, BorderLayout.PAGE_START);
        
        // Add the puzzle state input textbox
        puzzleInputTextField = new JTextField(22);
        add(puzzleInputTextField, BorderLayout.LINE_START);
        
        // Add the solve button
        JButton solveButton = new JButton("SOLVE");
        solveButton.addActionListener(this);
        solveButton.setActionCommand("solve");
        add(solveButton, BorderLayout.LINE_END);
	}
	
	/**
	 * Perform the button actions
	 */
	public void actionPerformed(ActionEvent e) 
	{
		if ("solve".equals(e.getActionCommand()))
		{	
			// Get the input puzzle as a string
			String initialPuzzleString = puzzleInputTextField.getText().toString();
			
			// Convert the initial puzzle string to an array of Strings
			String[] initialPuzzleStringArray = initialPuzzleString
					.substring(0, initialPuzzleString
					.length()).split(",");
			
			
			// Error message, just in case
			String errorMessage = "Input formatted incorrectly";
			
			// Convert the string array to an integer array
			for (int i = 0; i < initialPuzzleStringArray.length; i++)
			{
			    try
			    {
			    	int newArrayIndexValue = Integer.parseInt(initialPuzzleStringArray[i]);
			    	
			    	// Make sure the number coming in is not invalid
			    	if (newArrayIndexValue > 16)
			        {
			        	// The input string is not properly formatted
			        	puzzleInputTextField.setText(errorMessage);
			        	// Get out of here
				    	return;
			        }
			    	else
			    	{
			    		// Put the new integer into the array
			    		initialPuzzle[i] = newArrayIndexValue;
			    	}
			    	
			    }
			    catch(NumberFormatException nfe){
			        // The input string is not properly formatted
			    	puzzleInputTextField.setText(errorMessage);
			    	// Get out of here
			    	return;
			    }
			}
			
			// Find where the zero index is located
			findZeroIndex(initialPuzzle);
			
			// Make sure an index does exist and that there are not duplicate indices
			if (zeroIndex != null)
			{
				System.out.println(zeroIndex.toString());
				// Set up the puzzle solver and solve it
				PuzzleSolver puzzleSolver = new PuzzleSolver(initialPuzzle, zeroIndex.intValue());
				puzzleSolver.solvePuzzle();
				//4,1,2,3,5,6,10,7,8,0,9,11,12,13,14,15
				
				// Output the solution to the text box
				puzzleInputTextField.setText(puzzleSolver.getMoves());
			}
			else
			{
				// Display an error message in the text box
				puzzleInputTextField.setText(errorMessage);
			}
			
			// Set everything back to normal to prepare to solve another puzzle
			zeroIndex = null;
			initialPuzzle = new int[16];
			
		}
	}
	
	private void findZeroIndex(int[] array)
	{
		
		for (int i = 0; i < array.length; i++)
		{
			// Check if the array index is equal to zero and
			// if the index variable is null
			if (array[i] == 0 && zeroIndex == null)
			{
				// Assign the array index to the index variable
				zeroIndex = i;
			}
			else if (array[i] == 0 && zeroIndex != null)
			{
				// Multiple zeros in the input array
				// Assign the index variable to null so that we can display
				// an error message
				zeroIndex = null;
				return;
			}
		}
	}
	
}
