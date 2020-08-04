function login(){
	username = userAccount.value;
	password = userPwd.value;
	sendRequest(username,password);
}
function sendRequest(username,password){
	console.clear();
	var data = "{\"userName\":\"" + username + "\",\"password\":\"" + password + "\"}";
	// console.log(data);
	$.ajax({
		type: "POST",
		url: "api/users/login",
		contentType: "application/json;charset=utf-8",
		data: data,
		dataType: "json",
		success: function (data,textStatus) {
			// alert("提交成功" + JSON.stringify(message));
			var guid = data["guid"];
			var userDto = data["userDto"];
			console.log(userDto);
			if(userDto["userType"] == 0){
				window.location.href = "/admin.html?guid="+guid;
			}
			else(window.location.href="/user.html?guid="+guid);
			console.log(textStatus);
		},
		error: function (message) {
			// alert("提交失败" + JSON.stringify(message));
		}
	});
}