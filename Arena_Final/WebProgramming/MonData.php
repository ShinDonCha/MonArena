<?php
	$moncount = $_POST["MonCount"];	//스크립트에 있는 키값으로 들어온 값을 받아옴
	$m_name= array();
	for($i = 0; $i < $moncount; $i++)
	{
	   $m_name[$i] = $_POST["Mon" . (string)$i];
	}

	$con = mysqli_connect("localhost", "dhosting", "wjsdur#20140318", "dhosting");
	//"localhost"  <-- 같은 서버 내

	if(!$con)
		die("Could not Connect(MonData)".mysqli_connect_error());
	//연결 실패했을 경우 이 스크립트를 닫아주겠다는 뜻	

	$RowDatas = array();
	$output = array();

	echo "[";
	for($k = 0; $k < $moncount; $k++)
	{
	      $check = mysqli_query($con, "SELECT * FROM MonArena_MonData WHERE MonName = '". $m_name[$k] ."'");

	      $numrows = mysqli_num_rows($check);
	      if($numrows == 0)
	        { //mysqli_num_rows() 함수는 데이터베이스에서 쿼리를 보내서 나온 레코드의 개수를 알아낼 때 쓰임
	        //즉 0 이라는 뜻은 해당 조건을 못 찾았다는 뜻

		die("Data does not exist. \n");
	        }

	      $row = mysqli_fetch_assoc($check);	//현재 MonName에 해당하는 행의 내용을 가져온다.
	      if($row)
	      {		
		
		$RowDatas[$k] = array();
		$RowDatas[$k]["MonsterName"] = $row["MonName"];	
		//row를 통해 닷홈의 nick_name정보를 가져와서 nick_name이라는 키값의 배열을 가지는 변수 RowDatas에 넣어준다.
		$RowDatas[$k]["AttackType"] = $row["AttackType"];
		$RowDatas[$k]["HP"] = $row ["HP"];
		$RowDatas[$k]["AttackDmg"] = $row ["AttackDmg"];
		$RowDatas[$k]["DefPower"] = $row ["DefPower"];
		$RowDatas[$k]["MDefPower"] = $row ["MDefPower"];
		$RowDatas[$k]["AttackSpd"] = $row ["AttackSpd"];
		$RowDatas[$k]["MoveSpd"] = $row ["MoveSpd"];		
		$RowDatas[$k]["AttackRange"] = $row ["AttackRange"];
		$output[$k] = json_encode($RowDatas[$k], JSON_UNESCAPED_UNICODE);	//PHP 5.4이상에서 JSON형식 생성

		echo $output[$k];		//클라이언트로 전달		
		
	      }
	}
	echo "]";
	echo "\n";	
	echo "Success!!";
	mysqli_close($con);
?>