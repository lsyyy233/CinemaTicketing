var guid;
var showId;
var movieId;

window.onload = function() {
	getParams();
	getShowInfo();
	getSeats();
}

function getParams() {
	guid = $.query.get("guid");
	showId = $.query.get("showId");
}

function getShowInfo() {
	$.ajax({
		type: "get", //设置请求类型
		url: "/api/shows/" + showId, //请求后台的url地址
		success: function(result) { //请求成功后的回调函数，data为后台返回的值
			console.log(result);
			movieName = result["movieName"];
			document.getElementById("movie").value = movieName;
			date = "" + result["dateTime"].split("T")[0];
			document.all.date.value = date;
			hallName = result["hallName"];
			document.getElementById("hall").value = hallName;
			showNumber = result["showNum"];
			document.getElementById("showNum").value = showNumber;
			price = result["price"];
			document.getElementById("price").value = price;

		}
	});
}

function getSeats() {
	$.ajax({
		type: "get", //设置请求类型
		url: "/api/tickets/" + showId + "/seats", //请求后台的url地址
		success: function(result) { //请求成功后的回调函数，data为后台返回的值
			// console.log(result);
			var str = "<option style='display: none'></option>";
			for (var i = 0; i < result.length; i++) {
				var seatNum = result[i];
				str += "<option value =\"" + seatNum + "\">" + seatNum + "</option>";
			}
			document.getElementById("seatNum").innerHTML = str;
		}
	});
}

function addTicket() {
	var ticketToAdd = {
		"guid": guid,
		"showId": showId,
		"seatNum": document.all.seatNum.value
	}
	var ticketToAddJson = JSON.stringify(ticketToAdd);
	$.ajax({
		beforeSend: function(request) {
			// request.setRequestHeader("guid", guid);
			request.setRequestHeader("Accept", "application/json");
			request.setRequestHeader("Content-type", "application/json");
		},
		type: "post", //设置请求类型
		url: "/api/tickets", //请求后台的url地址
		data:ticketToAddJson,
		success: function(result) { //请求成功后的回调函数，data为后台返回的值
		// console.log(result);
			alert("购票成功");
			getSeats();
		}
	});
}

function goBack(){
	window.location.href = "/user.html?" + "guid=" + guid;
}
