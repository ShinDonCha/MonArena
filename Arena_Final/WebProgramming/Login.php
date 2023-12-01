<?php
	$user_id = $_POST["User_ID"];
	$user_pw = $_POST["User_PW"];

	$con = mysqli_connect("localhost", "dhosting", "wjsdur#20140318", "dhosting");
	
	if(!$con)
		die("Could not Connect(Login)".mysqli_connect_error());	

	$check = mysqli_query($con, "SELECT * FROM MonArena_UserData WHERE User_ID = '" . $user_id . "'");

	$numrows = mysqli_num_rows($check);

	if($numrows == 0)
		die("ID does not exist. \n");

	$row = mysqli_fetch_assoc($check);
	
	if($row)
	{
	   if($user_pw == $row["User_PW"])
	   {
		$rowdatas = array();
		$rowdatas["User_Nick"] = $row["User_Nick"];
		$rowdatas["Unique_ID"] = $row["Unique_ID"];
		$rowdatas["User_Crt"] = $row["User_CrtNum"];
		$rowdatas["User_Lv"] = $row["User_Level"];
		$rowdatas["User_Gold"] = $row["User_Gold"];
		$rowdatas["User_ATime"] = $row["User_ATime"];
		$rowdatas["User_Stage"] = $row["User_Stage"];
		$rowdatas["MonList"] = $row["MonsterList"];
		$rowdatas["MonStar"] = $row["MonStarForce"];
		$rowdatas["CombatDeck"] = $row["CombatList"];
		$rowdatas["CombatStarF"] = $row["CombatStarForce"];
		$rowdatas["DefDeck"] = $row["DefMonList"];
		$rowdatas["DefStarF"] = $row["DefStarForce"];
		$rowdatas["RankDeck"] = $row["RankMonList"];
		$rowdatas["RankStarF"] = $row["RankStarForce"];
		
		$output = json_encode($rowdatas, JSON_UNESCAPED_UNICODE);
		echo $output;
	   }
	   else
	   {
		die("The PW doesn't fit.");
	   }
	}
	echo "Success";
	mysqli_close($con);
?>
