var guid;
var hallId;
window.onload = function() {
	guid = $.query.get("guid");
	hallId = $.query.get("hallId");
	// console.log("hallId = " + hallId + " guid = " + guid);
	getHallInfo();
}

function getHallInfo() {
	if ((guid == null) || guid == "") {
		alert("未登录！");
		return;
	}
	$.ajax({
		type: "get", //设置请求类型
		url: "/api/halls/" + hallId, //请求后台的url地址
		success: function(data) { //请求成功后的回调函数，data为后台返回的值
			console.log(data);
			document.getElementById("name").value = data["name"];
			document.getElementById("seats").value = data["seats"];
		}
	});
}

function goBack() {
	window.location.href = "/admin.html?guid=" + guid;
}

function submit() {
	data = getForm();
	console.log(data);
	sendRequest(data);
}

function getForm() {
	var hallToAdd = {
		"id":hallId,
		"name": form.name.value,
		"seats": form.seats.value
	}
	hallToAddJson = JSON.stringify(hallToAdd);
	return hallToAddJson;
}

function sendRequest(hallToSubmit){
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
		// async:false,
		type: "put", //设置请求类型
		url: "/api/halls/", //请求后台的url地址
		data: hallToSubmit, //请求参数，是key-value形式的，如 {name:"jason"}m
		success: function(result) { //请求成功后的回调函数，data为后台返回的值
			alert("修改成功");
		},
		error: function(XMLHttpRequest, textStatus, errorThrown) {
			if (XMLHttpRequest.status == 401) {
				alert("只允许管理员进行此操作！");
			}
		}
	});
}
