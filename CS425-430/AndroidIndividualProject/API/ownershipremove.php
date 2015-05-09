<?php
header('Content-Type: application/json');
include('config.php');

if (isset($_POST['key'])) {
	$key = $_POST['key'];

	if ($key == $androidauthentication) {
		
		// Get data from the URL
		if (isset($_POST['user']) && isset($_POST['game'])) {
			$user = $_POST['user'];
			$game = $_POST['game'];

		// SQL Statement
		$sql = "DELETE FROM OwnedGames WHERE userID = $user and videogameID = $game";

		// Create connection
		$conn = new mysqli($servername, $dbusername, $dbpassword, $dbname);

		// Check connection
		if ($conn->connect_error) {
    			die("Connection Failed");
		}

		$result = $conn->query($sql);

		echo '{"results": {[]} }';

		$conn->close();
		}
	}
}
else {
	echo "Access Denied";
}
?>