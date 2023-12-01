<?php
	$Input_ID = $_POST["Creat_ID"];
	$Input_PW = $_POST["Creat_PW"];

	$con = mysqli_connect("localhost", "dhosting", "wjsdur#20140318", "dhosting");

	if(!$con)
		die("Could not Connect".mysqli_connect_error());

	$check = mysqli_query($con, "SELECT * FROM MonArena_UserData WHERE User_ID = '".$Input_ID."'");

	$numrows = mysqli_num_rows($check);

	if($numrows != 0)
		die("ID does exist. \n");

	$Unicheck = mysqli_query($con, "SELECT * FROM MonArena_UserData WHERE Unique_ID");
	$Uni_ID = mysqli_num_rows($Unicheck);
	$Uni_ID += 1;
	$UserNick = "신규 유저" . (String)$Uni_ID;
	$Fund = 1500;

	$Result = mysqli_query($con, "INSERT INTO MonArena_UserData (User_ID, User_PW, User_Nick, Unique_ID, User_Gold) VALUES ('$Input_ID', '$Input_PW', '$UserNick', '$Uni_ID', '$Fund')");
	$RankResult = mysqli_query($con, "INSERT INTO MonArena_RankData (Unique_ID, User_Rank, User_Nick) VALUES ('$Uni_ID', '$Uni_ID', '$UserNick')");	

	if($Result && $RankResult)
		echo "Creat Success.\n";
	else
		echo "Creat Error.\n";

	mysqli_close($con);
?>