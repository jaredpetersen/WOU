package sixteenPuzzle;

import java.awt.Color;
import java.awt.Font;
import java.awt.Graphics;

import javax.swing.JPanel;

/**
 * Draws the squares in the GUI window
 * 
 * @author Jared Petersen
 */
@SuppressWarnings("serial")
public class GUIDrawPanel extends JPanel
{
	/**
	 * Main method of sorts for the GUI
	 **/
	@Override
    public void paintComponent(Graphics g) {
        
        super.paintComponent(g);
        boolean zeroSpace = false;
        
        int k = 0;
        
        // Draw all of the columns
        for (int j = 0; j < 4; j++)
        {
	        // Draw a single column
	        for (int i = 0; i < 4; i++)
	        {
	        	if (i == 0 && j == 0)
	        	{
	        		zeroSpace = true;
	        	}
	        	
	        	drawSquare(g, k, zeroSpace, 10 + 90*i, 10 + 90*j);
	        	
	        	zeroSpace = false;
	        	k++;
	        }
        }
    }
	
	/**
	 * Draw a square in the window
	 **/
	private void drawSquare(Graphics g, int numberSquare, boolean zeroSpace, int xCor, int yCor)
	{
		// Determine what color should be used for the squares
		if (zeroSpace)
		{
			// Set the zero space to gray
			g.setColor(Color.gray);
		}
		else
		{
			// Set the regular block to red
			g.setColor(Color.red);
		}
		
		// Draw the outline
		g.drawRect(xCor, yCor, 80, 80);
		
		// Fill the shape
		g.fillRect(xCor, yCor, 80, 80);
		
		// Write the proper number in white
		g.setColor(Color.white);
		g.setFont(new Font("Arial", Font.PLAIN, 32));
		String outputNumber;
		// Two digit numbers and one digit numbers look wonky
		if (numberSquare < 10)
		{
			// One digit number, add an additional space to make it a "two digit" number
			outputNumber = " " + numberSquare;
		}
		else
		{
			// Two digit number, just make it a string
			outputNumber = "" + numberSquare;
		}
		// Draw the actual string in the center of the square
		// Specific coordinates found through trial and error
		g.drawString(outputNumber, xCor + 20, yCor + 47);
	}
}
