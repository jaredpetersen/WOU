package sixteenPuzzle;

import java.awt.Dimension;
import java.awt.BorderLayout;

import javax.swing.*;

/**
 * User interface for the 16 Puzzle Solver
 * @author Jared Petersen
 *
 */
public class GUI extends JFrame
{
	/**
	 * GUI Constructor
	 */
	public GUI()
	{
		// Create window with title
		setTitle("16 Puzzle");
		//setSize(290, 340);
		setSize(370, 420);
		
		// Set the location of the window to the center of the screen
        setLocationRelativeTo(null);
        
        // Do not allow the user to resize the window
        setResizable(false);
        
        // Close the application completely when the user hits "X"
        setDefaultCloseOperation(EXIT_ON_CLOSE);
        
        // Add the puzzle display JPanel
        GUIDrawPanel dpnl = new GUIDrawPanel();
        dpnl.setPreferredSize(new Dimension(370, 370));
        add(dpnl, BorderLayout.PAGE_START);
        
        // Add the puzzle state input textbox
        JTextField textField = new JTextField(22);
        add(textField, BorderLayout.LINE_START);
        
        // Add the solve button
        JButton solveButton = new JButton("SOLVE");
        add(solveButton, BorderLayout.LINE_END);
	}
	
}
