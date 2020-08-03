var guid;
var movieId;
window.onload = function() {
	guid = $.query.get("guid");
	movieId = $.query.get("movieId");
	// console.log("movieId = " + movieId + " guid = " + guid);
	getMovieInfo();
}

function getMovieInfo() {
	if ((guid == null) || guid == "") {
		alert("未登录！");
		return;
	}
	$.ajax({
		type: "get", //设置请求类型
		url: "/api/movies/" + movieId, //请求后台的url地址
		success: function(data) { //请求成功后的回调函数，data为后台返回的值
			console.log(data);
			document.getElementById("name").value = data["name"];
			document.getElementById("introduction").innerHTML = data["introduction"];
			if(data["releaseStatu"] ==2){
				$("#isUnderTheHit_true").prop("checked",true);
			}else{
				$("#isUnderTheHit_false").prop("checked",true);
			}
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
	var movieToAdd = {
		"id":movieId,
		"name": form.name.value,
		"introduction": form.introduction.value,
		"isUnderTheHit" : $("input[name='isUnderTheHit']:checked").val()
	}
	movieToAddJson = JSON.stringify(movieToAdd);
	return movieToAddJson;
}

function sendRequest(movieToSubmit){
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
		url: "/api/movies/", //请求后台的url地址
		data: movieToSubmit, //请求参数，是key-value形式的，如 {name:"jason"}m
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
