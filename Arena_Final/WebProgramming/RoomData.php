<?php	
	$master_name = $_POST["MNick"];

	$con = mysqli_connect("localhost", "dhosting", "wjsdur#20140318", "dhosting");
	
	if(!$con)
		die("Could not Connect(RoomMon)".mysqli_connect_error());

	$check = mysqli_query($con, "SELECT * FROM MonArena_UserData WHERE User_Nick = '". $master_name ."'");

	$numrows = mysqli_num_rows($check);

	 if($numrows == 0)
	    { 
	         die("Data does not exist.");
	    }

	 $row = mysqli_fetch_assoc($check);
	 if($row)
	    {			
	          $rowdatas = array();
	          $rowdatas["MonList"] = $row["MonsterList"];	
	          $rowdatas["MonStar"] = $row["MonStarForce"];		
	          $output = json_encode($rowdatas, JSON_UNESCAPED_UNICODE);
	          echo $output;		//클라이언트로 전달		
	    }
	echo "\n";	
	echo "Success!!";
	mysqli_close($con);
?>