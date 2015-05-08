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
			$status = $_POST['status'];


		// SQL Statement
		//$sql = "SELECT * FROM Users WHERE userID = $user";
		//$sql = "SELECT title, releaseDate, consoleName, esrbRatingInitial as esrb FROM Videogames, ConsolesVideogames, Consoles, ESRB, Users, OwnedGames WHERE Videogames.videogameID = ConsolesVideogames.videogameID and ConsolesVideogames.consoleID = Consoles.consoleID and ConsolesVideogames.esrbID = ESRB.esrbID and Users.userID = $user and Users.userID = OwnedGames.userID and Videogames.videogameID = OwnedGames.videogameID";
		$sql = "INSERT INTO OwnedGames (quantity, userID, videogameID, completionStatusID) VALUES (1, $user, $game, $status)";

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