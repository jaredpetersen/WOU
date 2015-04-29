/**
 * Client program requesting current date from server.
 *
 * Figure 3.22
 *
 * @author Silberschatz, Gagne and Galvin
 * Operating System Concepts  - Eighth Edition
 *
 * Modified by Jared Petersen
 */ 

import java.net.*;
import java.io.*;

public class Client 
{
	Socket socket;
	boolean error;
	
	public Client(String ipAddress, int port) 
	{
		try {
			socket = new Socket(ipAddress,port);
		}
		catch (IOException ioe) {
			error = true;
		}
	}
	
	/**
	 * Send a message to the server
	 * @param message
	 */
	public String sendMessage(String message)
	{
		String response = "";
		try
		{
			// get the message to be encrypted
			String plainMessage = message;
			if (plainMessage == null)
			{
				// reject the user's request if they don't have a message
				return "Please include a message";
				
			}
	
	
			// write data to the socket
			PrintWriter pout = new PrintWriter(socket.getOutputStream(), true);
			pout.println(plainMessage);
	
			// Get ready to recieve the encrypted message
			InputStream in = socket.getInputStream();
			BufferedReader bin = new BufferedReader(new InputStreamReader(in));
	
			// print the encrypted message
			
			String line;
			while( (line = bin.readLine()) != null)
				response = response + line;
				
		}
		catch (Exception e)
		{
			return "Connection failed";
		}

		return response;
	}
	
	/**
	 * Close the socket
	 */
	public void closeSocket()
	{
		try 
		{
			socket.close();
		} 
		catch (Exception e)
		{
			e.printStackTrace();
		}
	}
}
