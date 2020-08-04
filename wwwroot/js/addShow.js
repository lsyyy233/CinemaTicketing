var guid;
var date;
var hallId;

window.onload = function() {
	console.clear();
	guid = $.query.get("guid");
	getMovies();
	// getHalls();
}

function goBack() {
	window.location.href = "../admin.html?" + "guid=" + guid;
}

function addShow() {
	var movieId = form.movie.value;
	if (movieId == null || movieId == undefined || movieId == "") {
		alert("请选择电影");
	}
	var showNum = form.showNum.value;
	if (showNum == null || showNum == undefined || showNum == "") {
		alert("请选择场次");
	}
	var price = form.price.value;
	if (price == null || price == undefined || price == "") {
		alert("请输入价格");
	}
	var showToAdd = {
		"dateTime": date,
		"showNum": showNum,
		"movieId": movieId,
		"hallId": hallId,
		"price": price
	}
	showToAddJson = JSON.stringify(showToAdd);
	console.log(showToAddJson);
	sendRequest(showToAddJson);
}

function sendRequest(showToAddJson) {
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
		url: "/api/shows/", //请求后台的url地址
		data: showToAddJson, //请求参数，是key-value形式的，如 {name:"jason"}m
		success: function(result) { //请求成功后的回调函数，data为后台返回的值
			console.log(result);
			// getMovies();
			getHalls()
			getShows()
			alert("添加成功");
		},
		error: function(XMLHttpRequest, textStatus, errorThrown) {
			if (XMLHttpRequest.status == 401) {
				alert("只允许管理员进行此操作！");
			}
		}
	});
}

function getHalls() {
	$.ajax({
		type: "get", //设置请求类型
		url: "/api/halls/date/" + date, //请求后台的url地址
		success: function(result) { //请求成功后的回调函数，data为后台返回的值
			var str = "<option style='display: none'></option>";
			for (var i = 0; i < result.length; i++) {
				var hall = result[i];
				str += "<option value =\"" + hall["id"] + "\">" + hall["name"] + "</option>";
			}
			document.getElementById("hall").innerHTML = str;
		}
	});
}

function getMovies() {
	$.ajax({
		type: "get", //设置请求类型
		url: "/api/movies/notUnderTheHit/", //请求后台的url地址
		success: function(result) { //请求成功后的回调函数，data为后台返回的值
			var str = "<option style='display: none'></option>";
			for (var i = 0; i < result.length; i++) {
				var movie = result[i];
				str += "<option value =\"" + movie["id"] + "\">" + movie["name"] + "</option>";
			}
			document.getElementById("movie").innerHTML = str;
		}
	});
}

function getShows() {
	$.ajax({
		type: "get", //设置请求类型
		url: "/api/shows/available?hallId="+hallId+"&date="+date, //请求后台的url地址
		success: function(result) { //请求成功后的回调函数，data为后台返回的值
			var str = "<option style='display: none'></option>";
			for (var i = 0; i < result.length; i++) {
				var showNum = result[i];
				str += "<option value =\"" + showNum + "\">" + showNum + "</option>";
			}
			document.getElementById("showNum").innerHTML = str;
		}
	});
}

function dateChange() {
	date = document.getElementById("date").value;
	getHalls(date);
}

function hallChange() {
	hallId = document.getElementById("hall").value;
	getShows();
}
