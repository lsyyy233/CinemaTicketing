var guid;
var date;
var hallId;
var showId;
var movieId;
var showNumber;
var price;

window.onload = function() {
	console.clear();
	guid = $.query.get("guid");
	showId = $.query.get("showId");
	getShowInfo();
}

function getShowInfo() {
	$.ajax({
		type: "get", //设置请求类型
		url: "/api/shows/" + showId, //请求后台的url地址
		success: function(result) { //请求成功后的回调函数，data为后台返回的值
			console.log(result);
			//显示movie到前台
			movieId = result["movieId"];
			getMovies();
			//显示date到前台
			date = "" + result["dateTime"].split("T")[0];
			document.all.date.value = date;
			//显示hall到前台
			hallId = result["hallId"];
			getHalls(date);
			showNumber = result["showNum"];
			getShows();
			price = result["price"];
			console.log(price);
			document.getElementById("price").value = price;

		}
	});
}

function goBack() {
	window.location.href = "../admin.html?" + "guid=" + guid;
}

function submit() {
	movieId = form.movies.value;
	if (movieId == null || movieId == undefined || movieId == "") {
		alert("请选择电影");
	}
	date = form.date.value;
	if (date == null || date == undefined || date == "") {
		alert("请选择日期");
	}
	hallId = form.halls.value;
	if (halls == null || halls == undefined || halls == "") {
		alert("请选择影厅");
	}
	showNumber = form.showNum.value;
	if (showNumber == null || showNumber == undefined || showNumber == "") {
		alert("请选择场次");
	}
	price = form.price.value;
	if (price == null || price == undefined || price == "") {
		alert("请输入价格");
	}
	var showToUpdateJson = {
		"id": showId,
		"dateTime": date,
		"showNum": showNumber,
		"movieId": movieId,
		"hallId": hallId,
		"price": price
	}
	showToUpdateJson = JSON.stringify(showToUpdateJson);
	console.log(showToUpdateJson);
	sendRequest(showToUpdateJson);
}

function sendRequest(showToUpdateJson) {
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
		type: "put", //设置请求类型
		url: "/api/shows/", //请求后台的url地址
		data: showToUpdateJson, //请求参数，是key-value形式的，如 {name:"jason"}m
		success: function(result) { //请求成功后的回调函数，data为后台返回的值
			console.log(result);
			// getMovies();
			getHalls()
			getShows()
			alert("修改成功");
		},
		error: function(XMLHttpRequest, textStatus, errorThrown) {
			if (XMLHttpRequest.status == 401) {
				alert("只允许管理员进行此操作！");
			}
		}
	});
}

function getHalls() {
	console.log(date);
	$.ajax({
		type: "get", //设置请求类型
		url: "/api/halls/date/" + date, //请求后台的url地址
		success: function(result) { //请求成功后的回调函数，data为后台返回的值
			var str = "<option style='display: none'></option>";
			for (var i = 0; i < result.length; i++) {
				var hall = result[i];
				str += "<option value =\"" + hall["id"] + "\">" + hall["name"] + "</option>";
			}
			document.getElementById("halls").innerHTML = str;
			document.all.halls.value = hallId;
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
			document.getElementById("movies").innerHTML = str;
			document.all.movies.value = movieId;
		}
	});
}

function getShows() {
	console.clear();
	$.ajax({
		type: "get", //设置请求类型
		url: "/api/shows/available?hallId=" + hallId + "&date=" + date + "&showId=" + showId,
		success: function(result) { //请求成功后的回调函数，data为后台返回的值
			var str = "<option style='display: none'></option>";
			for (var i = 0; i < result.length; i++) {
				var showNum = result[i];
				console.log(showNum);
				str += "<option value =\"" + showNum + "\">" + showNum + "</option>";
			}
			document.getElementById("showNum").innerHTML = str;
			// document.getElementById("showNum").innerHTML += "<option value =\"" + showNumber + "\">" + showNumber + "</option>";
			var options = document.getElementById("showNum").getElementsByTagName("option");
			for (var i = 0; i < options.length; i++) {
				option = options[i];
				if (option.value == showNumber) {
					option.selected = true;
				}
			}
		}
	});
}

function dateChange() {
	date = document.getElementById("date").value;
	getHalls(date);
}

function hallChange() {
	console.log(hallId);
	hallId = document.getElementById("halls").value;
	getShows();
}
