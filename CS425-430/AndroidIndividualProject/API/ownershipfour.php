<?php
header('Content-Type: application/json');
include('config.php');

if (isset($_GET['key'])) {
	$key = $_GET['key'];

	if ($key == $androidauthentication) {
		
		// Get data from the URL
		if (isset($_GET['user'])) {
			$user = $_GET['user'];
			$status = $_GET['status'];
		}

		else {
			// Default user is just the initial
			$user = 0;
		}
		
		if ($status == 4) {
			$sql = "SELECT status, count(*) as gameCount FROM OwnedGames, CompletionStatus WHERE OwnedGames.completionStatusId = CompletionStatus.completionStatusId and OwnedGames.userId = $user group by status order by status asc";
		}
		else {
			// SQL Statement
			$sql = "SELECT status, count(*) as gameCount FROM OwnedGames, CompletionStatus WHERE OwnedGames.completionStatusId = CompletionStatus.completionStatusId and OwnedGames.userId = $user and OwnedGames.completionStatusId = $status group by status order by status asc";
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