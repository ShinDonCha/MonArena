<?php
	$user_uid = $_POST["Input_UID"];		//스트립트에 있는 키값으로 들어온 값을 받아옴
	$user_level = $_POST["Input_Level"];

	$con = mysqli_connect("localhost", "dhosting", "wjsdur#20140318", "dhosting");

	if(!$con)
		die("Could not Connect".mysqli_connect_error());

	$check = mysqli_query($con, "SELECT * FROM MonArena_UserData WHERE Unique_ID = '". $user_uid ."'");

	$numrows = mysqli_num_rows($check);
	if($numrows == 0)
	{ 
		die("UID does not exist. \n");
	}
	
	$row = mysqli_fetch_assoc($check);
	if($row)	
	{
		mysqli_query($con, "UPDATE MonArena_UserData SET `User_Level` = '".$user_level."'

		WHERE `Unique_ID` = '".$user_uid."' ");		
		
		echo ("SaveSuccess");
	}
	mysqli_close($con);
?>