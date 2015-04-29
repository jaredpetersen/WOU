
/**
 * Time-of-day server listening to port 6013.
 *
 * Figure 3.21
 *
 * @author Silberschatz, Gagne, and Galvin. 
 * Operating System Concepts  - Ninth Edition
 * Copyright John Wiley & Sons - 2013.
 *
 * Modified by Jared Petersen
 */
 
import java.net.*;
import java.io.*;
import java.security.MessageDigest;

public class Lab2P3Server 
{
	public static void main(String[] args)  {
		try {
			ServerSocket sock = new ServerSocket(6014);

			// now listen for connections
			while (true) {
				Socket client = sock.accept();
				// we have a connection

				InputStream in = client.getInputStream();
				BufferedReader bin = new BufferedReader(new InputStreamReader(in));
				String message = bin.readLine();
				
				MessageDigest messageDigest = MessageDigest.getInstance("MD5");
				messageDigest.update(message.getBytes());
				String encryptedMessage = new String(messageDigest.digest());
				
				PrintWriter pout = new PrintWriter(client.getOutputStream(), true);
				// write the Date to the socket
				pout.println(encryptedMessage);

				// close the socket and resume listening for more connections
				client.close();
			}
		}
		catch (Exception e) {
			System.out.println(e);
		}
	}
}
