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
import java.security.MessageDigest;

public class Lab2P3Client 
{
	public static void main(String[] args)  {
		try {
			// this could be changed to an IP name or address other than the localhost
			Socket sock = new Socket("127.0.0.1",6014);

			// get the message to be encrypted
			String plainMessage = args[0];
			if (plainMessage == null)
			{
				// reject the user's request if they don't have a message
				System.out.println("Please include a message");
				System.exit(1);
			}


			// write data to the socket
			PrintWriter pout = new PrintWriter(sock.getOutputStream(), true);
			pout.println(plainMessage);

			// Get ready to recieve the encrypted message
			InputStream in = sock.getInputStream();
			BufferedReader bin = new BufferedReader(new InputStreamReader(in));

			// print the encrypted message
			String line;
			while( (line = bin.readLine()) != null)
				System.out.println(line);
				
			sock.close();
		}
		catch (IOException ioe) {
				System.err.println(ioe);
		}
	}
}
