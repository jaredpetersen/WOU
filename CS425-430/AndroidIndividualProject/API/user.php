<?php
header('Content-Type: application/json');
include('config.php');

if (isset($_GET['key'])) {
	$key = $_GET['key'];

	if ($key == $androidauthentication) {
		
		// Get data from the URL
		if (isset($_GET['user'])) {
			$user = $_GET['user'];
		}

		else {
			// Default user is just the initial
			$user = 0;
		}

		if ($user == 0) {

			if ((isset($_GET['username'])) && (isset($_GET['password'])))
			{
				$username = $_GET['username'];
				$password = $_GET['password'];
				$sql = "SELECT * FROM Users WHERE userName = '$username' AND password = '$password'";
			}
			else
			{

				// SQL Statement
				$sql = "SELECT * FROM Users";
			}
		}

		else {
			// SQL Statement
			$sql = "SELECT * FROM Users WHERE userID = $user";
		}

		// Create connection
		$conn = new mysqli($servername, $dbusername, $dbpassword, $dbname);

		// Check connection
		if ($conn->connect_error) {
    			die("Connection Failed");
		}

		$result = $conn->query($sql);

		// Get the data
		while($row = $result->fetch_assoc()) {
			$output[] = $row;
		}

		echo '{"results": ';

		// Output data as a JSON
		echo(json_encode($output));

		echo '}';

		$conn->close();
	}
}
else {
	echo "Access Denied";
}
?>