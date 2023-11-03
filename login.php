<?php	
set_time_limit(2400);	
ini_set('session.gc_maxlifetime', 2400);		
if($_SERVER['REQUEST_METHOD'] == 'POST'){
	$username = $_POST['userID'];		
	$password = $_POST['password'];
	$value = md5($username."".$password);		
	setcookie("NPPv2", $value);				
	echo('test body');	
}