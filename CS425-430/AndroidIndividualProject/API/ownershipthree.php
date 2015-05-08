<?php
header('Content-Type: application/json');
include('config.php');

if (isset($_GET['key'])) {
	$key = $_GET['key'];

	if ($key == $androidauthentication) {
		
		// Get data from the URL
		if (isset($_GET['user'])) {
			$user = $_GET['user'];
			$game = $_GET['game'];
		}

		else {
			// Default user is just the initial
			$user = 0;
		}

		// SQL Statement
		//$sql = "SELECT * FROM Users WHERE userID = $user";
		$sql = "SELECT videogameID, OwnedGames.completionStatusID, status FROM OwnedGames, CompletionStatus WHERE videogameID = $game and userID = $user and OwnedGames.completionStatusID = CompletionStatus.completionStatusID";

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