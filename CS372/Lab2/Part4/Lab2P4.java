import java.awt.*;
import java.awt.event.ActionEvent;
import java.awt.event.ActionListener;

import javax.swing.*;

/**
 * User interface for the Server Client
 * 
 * @author Jared Petersen
 */
@SuppressWarnings("serial")
public class Lab2P4 extends JFrame implements ActionListener
{
	// GUI input text fields
	private JTextField ipAddressTextField;
	private JTextField portTextField;
	private JTextField queryTextField;
	private JTextField resultTextField;
	private String ipAddress;
	private String port;
	private String serverQuery;
	private String serverResult;
	private Client client;
	
	/**
	 * Main Method
	 * @param args
	 */
	public static void main(String[] args)
	{
		JFrame f = new Lab2P4();
		f.pack();
	    f.setVisible(true);
	}
	
	/**
	 * Constructor
	 */
	public Lab2P4()
	{
		// Create window with title
		setTitle("Server Client");
		setSize(300, 200);
		
		// Set the location of the window to the center of the screen
        setLocationRelativeTo(null);
        
        // Do not allow the user to resize the window
        // We don't want to have to worry about resizing everything
        setResizable(false);
        
        // Close the application completely when the user hits "X"
        setDefaultCloseOperation(EXIT_ON_CLOSE);
        
        // GUI layout manager
        GridBagLayout gridbag = new GridBagLayout();
        setLayout(gridbag);
        GridBagConstraints c = new GridBagConstraints();
        setLayout(gridbag);
        c.weightx = 1;
        c.weighty = 1;
        
        // IP Address
        c.gridx = 0;
        c.gridy = 0;
        ipAddressTextField = new JTextField(20);
        ipAddressTextField.setText("Server Address");
        c.anchor = GridBagConstraints.FIRST_LINE_START;
        add(ipAddressTextField, c);
        
        // Port
        c.gridwidth = 1;
        c.gridx = 0;
        c.gridy = 1;
        c.anchor = GridBagConstraints.LINE_START;
        portTextField = new JTextField(20);
        portTextField.setText("Port");
        add(portTextField, c);
        
        // Connect
        c.gridwidth = 1;
        c.gridx = 2;
        c.gridy = 1;
        JButton connectButton = new JButton("Connect");
        connectButton.addActionListener(this);
        connectButton.setActionCommand("connect");
        add(connectButton, c);
        
        // Server query
        c.gridwidth = 2;
        c.gridx = 0;
        c.gridy = 2;
        c.anchor = GridBagConstraints.LAST_LINE_START;
        queryTextField = new JTextField(20);
        queryTextField.setText("Server Query");
        add(queryTextField, c);
        
        // Server result
        c.gridwidth = 2;
        c.gridx = 0;
        c.gridy = 3;
        resultTextField = new JTextField(20);
        resultTextField.setText("Server Result");
        add(resultTextField, c);
        
        // Query button
        c.gridwidth = 1;
        c.gridx = 2;
        c.gridy = 3;
        c.anchor = GridBagConstraints.LINE_START;
        JButton queryButton = new JButton("Query");
        queryButton.addActionListener(this);
        queryButton.setActionCommand("query");
        add(queryButton, c);
        
	}
	
	/**
	 * Perform the button actions
	 */
	public void actionPerformed(ActionEvent e) 
	{
		if ("connect".equals(e.getActionCommand()))
		{	
			if (client != null)
			{
				client.closeSocket();
			}
			
			// Get the input puzzle as a string
			ipAddress = ipAddressTextField.getText().toString();
			port = portTextField.getText().toString();
			
			int portNum = Integer.parseInt(port);
			
			client = new Client(ipAddress, portNum);
			
		}
		else if ("query".equals(e.getActionCommand()))
		{
			if (client != null)
			{
				serverQuery = queryTextField.getText().toString();
				client.sendMessage(serverQuery);
			}
		}
	}
	
}
