var guid;
var showSelfPageUrl;
var showNextPageUrl;
var showPreviousPageUrl;

window.onload = function() {
	getParams();
	getUserInfo();
	getShowList("/api/shows")
}

function getShowList(url) {
	$.ajax({
		beforeSend: function(request) {
			request.setRequestHeader("Accept", "application/vnd.cinemaTicketing.hateoas+json");
			request.setRequestHeader("Content-type", "application/json");
		},
		type: "get", //设置请求类型
		url: url, //请求后台的url地址
		// data: "", //请求参数，是key-value形式的，如 {name:"jason"}
		success: function(result) { //请求成功后的回调函数，data为后台返回的值
			console.log(result);
			showShows(result);
			showShowPageInfo(result);
			getLinksForShowPage(result);
		}
	});
}

function showShowPageInfo(result) {
	var currentPage = result["currentPage"];
	var totalCount = result["totalCount"];
	var totalPages = result["totalPages"];
	document.getElementById("showPageInfo").innerHTML = "当前页数:" + currentPage + "/总页数:" + totalPages +
		" 共" + totalCount + "条数据";
	ShowPageNum = currentPage;
	ShowPageSize = result["pageSize"];
}

function showShows(result) {
	var str = " <tr style='height: 50px;'>" +
		"<th>电影名</th>" +
		"<th>场次</th>" +
		"<th>日期</th>" +
		"<th>影厅</th>" +
		"<th>价格</th>" +
		"<th>操作</th>" +
		"</tr>";
	var shows = result["linkedShowDtos"];

	for (var i = 0; i < shows.length; i++) {
		show = shows[i]["data"];
		var showId = show["id"];
		str += " <tr style='height: 50px;'>" +
			"<td>" + show["movieName"] + "</td>" +
			"<td>" + show["showNum"] + "</td>" +
			"<td>" + show["dateTime"].split('T')[0] + "</td>" +
			"<td>" + show["hallName"] + "</td>" +
			"<td>" + show["price"] + "</td>" +
			"<td><a href='javascript:;' onclick='jumpToAddTicket(" + showId + ")'>" + "购票" +
			"</a></td>";
	}
	if (shows.length < 5) {
		for (var i = 0; i < (5 - shows.length); i++) {
			str += "<tr style='height: 50px;'><td></td><td></td><td></td><td></td><td></td><td></td><td></td></tr>";
		}
	}
	document.getElementById("showTable").innerHTML = str;
}

function jumpToAddTicket(showId) {
	window.location.href="/addTicket.html?guid="+guid+"&showId="+showId;
}

// 获取翻页链接
function getLinksForShowPage(result) {
	var linksForShows = result["linksForShows"]
	document.getElementById("showNext").disabled = true;
	document.getElementById("showPrevious").disabled = true;
	// console.log(links);
	for (var i = 0; i < linksForShows.length; i++) {
		link = linksForShows[i];
		if (link["rel"] == "next_page") {
			document.getElementById("showNext").disabled = false;
			showNextPageUrl = link["href"];
		}
		if (link["rel"] == "previous_page") {
			document.getElementById("showPrevious").disabled = false;
			showPreviousPageUrl = link["href"];
		}
		if (link["rel"] == "self") {
			showSelfPageUrl = link["href"];
		}
	}
}

function getParams() {
	guid = $.query.get("guid");
}

function showPreviousPage() {
	getShowList(showPreviousPageUrl);
}

function showNextPage() {
	getShowList(showNextPageUrl);
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
