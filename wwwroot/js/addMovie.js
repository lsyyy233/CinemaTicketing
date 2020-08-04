var guid;
window.onload = function() {
	guid = $.query.get("guid");
	console.log(guid);
}

function addMovie() {
	console.clear();
	getForm();
}

function getForm() {
	var movieToAdd = {
		"name": form.name.value,
		"introduction": form.introduction.value
	}
	movieToAddJson = JSON.stringify(movieToAdd);
	console.log(movieToAddJson);
	sendRequest(movieToAddJson);
}

function goBack() {
	window.location.href = "/admin.html?guid=" + guid;
}

function sendRequest(movieToAdd) {
	if ((guid == null) || guid == "") {
		alert("未登录！");
		return;
	}
	$.ajax({
		beforeSend: function(request) {
			request.setRequestHeader("guid", guid);
			request.setRequestHeader("Accept", "application/json");
			request.setRequestHeader("Content-type", "application/json");
		},
		type: "post", //设置请求类型
		url: "/api/movies/", //请求后台的url地址
		data: movieToAdd, //请求参数，是key-value形式的，如 {name:"jason"}m
		success: function(result) { //请求成功后的回调函数，data为后台返回的值
			console.log(result);
			alert("添加成功");
		},
		error: function(XMLHttpRequest, textStatus, errorThrown) {
			if (XMLHttpRequest.status == 401) {
				alert("只允许管理员进行此操作！");
			}
		}
	});
}
