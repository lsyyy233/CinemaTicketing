var guid;

window.onload = function() {
	getParams();
	getUserInfo();
	getMovieList("/api/movies/");
	getHallList("/api/halls");
	getShowList("/api/shows");
	getTicketList("api/tickets/" + guid);
}

function getParams() {
	guid = $.query.get("guid");
}

function getUserInfo() {
	$.ajax({
		type: "get", //设置请求类型
		url: "/api/users/" + guid, //请求后台的url地址
		data: "", //请求参数，是key-value形式的，如 {name:"jason"}
		success: function(data) { //请求成功后的回调函数，data为后台返回的值
			showUserInfo(data);
		}
	});
}

function showUserInfo(userInfo) {
	console.log(userInfo);
	document.getElementById("currentUser").innerHTML += userInfo["userDto"]["userName"];
}


