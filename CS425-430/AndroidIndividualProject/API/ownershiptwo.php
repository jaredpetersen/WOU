<?php
header('Content-Type: application/json');
include('config.php');

if (isset($_POST['key'])) {
	$key = $_POST['key'];

	if ($key == $androidauthentication) {
		

		// POST data from the URL
		if (isset($_POST['user']) && isset($_POST['game'])) {
			$user = $_POST['user'];
			$game = $_POST['game'];
			$status = $_POST['status'];

		
			// SQL Statement
			$sql0 = "SELECT * FROM OwnedGames WHERE videogameID = $game AND userID = $user";
			$sql1 = "INSERT INTO OwnedGames (quantity, userID, videogameID, completionStatusID) VALUES (1, $user, $game, $status)";
			$sql2 = "UPDATE OwnedGames SET completionStatusID = $status WHERE videogameID = $game AND userID = $user";
		
			// Create connection
			$conn = new mysqli($servername, $dbusername, $dbpassword, $dbname);

			// Check connection
			if ($conn->connect_error) {
    				die("Connection Failed");
			}

			$count = $conn->query($sql0);
		
			if ($count->num_rows > 0) {
				echo 'update';
				$result = $conn->query($sql2);
			}
			else {
				echo 'insert';
				$result = $conn->query($sql1);
			}

			$conn->close();
		}
	}
}
else {
	echo "Access Denied";
}
?>