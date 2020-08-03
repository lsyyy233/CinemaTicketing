var guid;
// var pageNum;
// var pageSize;

window.onload = function() {
	console.clear();
	guid = $.query.get("guid");
	// pageNum = $.query.get("pageNum");
	// pageSize = $.query.get("pageSize");
}

function addHall() {
	getForm();
}

function getForm() {
	var hallToAdd = {
		"name": form.name.value,
		"seats": form.seats.value
	}
	hallToAddJson = JSON.stringify(hallToAdd);
	sendRequest(hallToAddJson);
}

function goBack() {
	window.location.href = "../admin.html?" + "guid=" + guid;
}

function sendRequest(hallToAdd) {
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
		type: "post", //设置请求类型
		url: "/api/halls/", //请求后台的url地址
		data: hallToAdd, //请求参数，是key-value形式的，如 {name:"jason"}m
		success: function(result) { //请求成功后的回调函数，data为后台返回的值
			console.log(result);
			alert("添加成功");
		},
		error: function(XMLHttpRequest, textStatus, errorThrown) {
			if (XMLHttpRequest.status == 401) {
				alert("只允许管理员进行此操作！");
			}
			// alert(XMLHttpRequest.readyState);
			// alert(textStatus);
		}
	});
}
