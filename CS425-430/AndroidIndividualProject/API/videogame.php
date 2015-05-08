<?php
header('Content-Type: application/json');
include('config.php');

if (isset($_GET['key'])) {
	$key = $_GET['key'];

	if ($key == $androidauthentication) {
		
		// Get data from the URL
		if (isset($_GET['esrb'])) {
			$esrb = $_GET['esrb'];
		}

		else {
			// Default esrb rating is just the initial
			$esrb = "initial";
		}

		if ($esrb == "initial") {
			$esrbColumn = "esrbRatingInitial";
		}

		elseif ($esrb == "full") {
			$esrbColumn = "esrbRatingFull";
		}

		elseif ($esrb == "both") {
			$esrbColumn = "concat(esrbRatingInitial, ' for ', esrbRatingFull)";
		}

		// Something else
		else {
			exit("Not a valid API call");
		}
		
		if (isset($_GET['search'])) {
			$search = $_GET['search'];
			// SQL Statement
			$sql = "SELECT Videogames.videogameID, title, releaseDate, consoleName, $esrbColumn as esrb FROM Videogames, ConsolesVideogames, Consoles, ESRB WHERE Videogames.videogameID = ConsolesVideogames.videogameID and ConsolesVideogames.consoleID = Consoles.consoleID and ConsolesVideogames.esrbID = ESRB.esrbID and ((title LIKE '%$search%') or (consoleName LIKE '%$search%'))";
		}

		else {
			// SQL Statement
			$sql = "SELECT Videogames.videogameID, title, videogameID, releaseDate, consoleName, $esrbColumn as esrb FROM Videogames, ConsolesVideogames, Consoles, ESRB WHERE Videogames.videogameID = ConsolesVideogames.videogameID and ConsolesVideogames.consoleID = Consoles.consoleID and ConsolesVideogames.esrbID = ESRB.esrbID";
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

		if (empty($output)) {
			echo '[]';
		}
		else {
			// Output data as a JSON
			echo json_encode($output);
		}

		echo '}';

		$conn->close();
	}
}
else {
	echo "Access Denied";
}
?>