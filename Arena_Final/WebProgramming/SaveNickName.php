<?php

	$user_uid = $_POST["Input_UID"];
	$user_nick = $_POST["Input_Nick"];

	$con = mysqli_connect("localhost", "dhosting", "wjsdur#20140318", "dhosting");

	if(!$con)
		die("Could not Connect".mysqli_connect_error());

	$check = mysqli_query($con, "SELECT * FROM MonArena_UserData WHERE User_Nick = '". $user_nick ."'");

	$numrows = mysqli_num_rows($check);

	if($numrows == 0)
	{	
		$check = mysqli_query($con, "SELECT * FROM MonArena_UserData WHERE Unique_ID = '". $user_uid ."'");
		$numrows = mysqli_num_rows($check);
	}
	else		
		die("NickName does exist. \n");

	$row = mysqli_fetch_assoc($check);

	if($row)	
	{
		mysqli_query($con, "UPDATE MonArena_UserData SET `User_Nick` = '".$user_nick."'
		WHERE `Unique_ID` = '".$user_uid."' ");		
		mysqli_query($con, "UPDATE MonArena_RankData SET `User_Nick` = '".$user_nick."'
		WHERE `Unique_ID` = '".$user_uid."' ");		
		
		echo ("SaveSuccess");
	}
	mysqli_close($con);
?>