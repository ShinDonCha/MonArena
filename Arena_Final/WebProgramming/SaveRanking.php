<?php
	$rank_nick = $_POST["Input_Nick"];
	$my_uid = $_POST["Input_UID"];

	$con = mysqli_connect("localhost", "dhosting", "wjsdur#20140318", "dhosting");
	
	if(!$con)
		die("Could not Connect(Login)".mysqli_connect_error());
	
	$Rankcheck = mysqli_query($con, "SELECT * FROM MonArena_RankData WHERE User_Nick = '" . $rank_nick . "'");
	$Mycheck = mysqli_query($con, "SELECT * FROM MonArena_RankData WHERE Unique_ID = '" . $my_uid . "'");

	$Ranknumrows = mysqli_num_rows($Rankcheck);
	$Mynumrows = mysqli_num_rows($Mycheck);

	if($Ranknumrows == 0 || $Mynumrows == 0)
		die("Rank or My numrows does not exist. \n");

	$Rankrow = mysqli_fetch_assoc($Rankcheck);		//RankData에 있는 해당 랭커의 열 정보
	$Myrow = mysqli_fetch_assoc($Mycheck);		//RankData에 있는 나의 열 정보

	if(!$Rankrow || !$Myrow)
		die("Rank or My row does not exist. \n");

	$Rank_Num = $Rankrow["User_Rank"];	
	$My_Num = $Myrow["User_Rank"];

	if($Rank_Num > $My_Num)
		die("Rank > My. \n");

	mysqli_query($con, "UPDATE MonArena_RankData SET `User_Rank` = '".$Rank_Num."'
					WHERE `Unique_ID` = '".$my_uid."' ");	//나의 랭크 변경

	$Search_Num = $Rank_Num + 1;
	$numcheck = mysqli_query($con, "SELECT * FROM MonArena_RankData WHERE User_Rank = '" . $Search_Num . "'");
	$searchnumrows = mysqli_num_rows($numcheck);
	while($searchnumrows != 0)
	{
		$Search_Num = $Search_Num + 1;
		$searchrow = mysqli_fetch_assoc($numcheck);

		$numcheck = mysqli_query($con, "SELECT * FROM MonArena_RankData WHERE User_Rank = '" . $Search_Num . "'");
		$searchnumrows = mysqli_num_rows($numcheck);

		mysqli_query($con, "UPDATE MonArena_RankData SET `User_Rank` = '".$Search_Num."'
						WHERE `Unique_ID` = '".$searchrow["Unique_ID"]."' ");
	}

	$Down_Num = $Rank_Num + 1;
	mysqli_query($con, "UPDATE MonArena_RankData SET `User_Rank` = '".$Down_Num."'
						WHERE `Unique_ID` = '".$Rankrow["Unique_ID"]."' ");
		
	echo ("SaveSuccess");	
	mysqli_close($con);
?>
