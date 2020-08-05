function regeist(){
	username = userAccount.value;
	password = userPwd.value;
	var data = {
		"userName":username,
		"password":password
	}
	var dataJson = JSON.stringify(data);
	$.ajax({
		type: "POST",
		url: "api/users/",
		contentType: "application/json;charset=utf-8",
		data: dataJson,
		dataType: "json",
		success: function (data,textStatus) {
			alert("注册成功")
		},
		error: function(XMLHttpRequest, textStatus, errorThrown) {
			if (XMLHttpRequest.status == 409) {
				alert("该用户名已存在！");
			}
			// alert(XMLHttpRequest.readyState);
			// alert(textStatus);
		}
	});
}