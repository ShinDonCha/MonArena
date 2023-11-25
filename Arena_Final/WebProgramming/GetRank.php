<?php
	$user_uid = $_POST["Input_UID"];

	$con = mysqli_connect("localhost", "dhosting", "wjsdur#20140318", "dhosting");
	
	if(!$con)
		die("Could not Connect(Login)".mysqli_connect_error());	

	$check = mysqli_query($con, "SELECT * FROM MonArena_RankData WHERE Unique_ID = '" . $user_uid . "'");

	$numrows = mysqli_num_rows($check);

	if($numrows == 0)
		die("ID does not exist. \n");

	$row = mysqli_fetch_assoc($check);
	$RankList = array();

	if(!$row)
		die("row does not exist. \n");
	
	echo "[";
	for($i = $row["User_Rank"] -3; $i <= $row["User_Rank"] +3; $i++)
	{		

	
		$ListCheck = mysqli_query($con, "SELECT * FROM MonArena_RankData WHERE User_Rank = '" . $i . "'");
		$numrows = mysqli_num_rows($ListCheck);
		if($numrows != 0)
		{			
			$ListRow = mysqli_fetch_assoc($ListCheck);
			$ListID = $ListRow["Unique_ID"];
			
			$ListCheck = mysqli_query($con, "SELECT * FROM MonArena_UserData WHERE Unique_ID = '" . $ListID . "'");
			$ListRow = mysqli_fetch_assoc($ListCheck);
			$RankList[$i]["User_Nick"] = $ListRow["User_Nick"];
			$RankList[$i]["User_Crt"] = $ListRow["User_CrtNum"];
			$RankList[$i]["User_Rank"] = $i;
			$RankList[$i]["DefDeck"] = $ListRow["DefMonList"];
			$RankList[$i]["DefStarF"] = $ListRow["DefStarForce"];
			$output[$i] = json_encode($RankList[$i], JSON_UNESCAPED_UNICODE);
			echo $output[$i];
		}
	}
	echo "]";	
	echo "\n";	
	echo "Success";
	mysqli_close($con);
?>
