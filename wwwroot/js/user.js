var guid;
var showSelfPageUrl;
var showNextPageUrl;
var showPreviousPageUrl;
var ticketSelfPageUrl;
var ticketNextPageUrl;
var ticketPreviousPageUrl;

window.onload = function() {
	getParams();
	getUserInfo();
	getShowList("/api/shows")
	getTicketList("api/tickets/" + guid);
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
			// console.log(result);
			showShows(result);
			showShowPageInfo(result);
			getLinksForShowPage(result);
		}
	});
}

function showTicketPageInfo(result){
	var currentPage = result["currentPage"];
	var totalCount = result["totalCount"];
	var totalPages = result["totalPages"];
	document.getElementById("ticketPageInfo").innerHTML = "当前页数:" + currentPage + "/总页数:" + totalPages +
		" 共" + totalCount + "条数据";
	ShowPageNum = currentPage;
	ShowPageSize = result["pageSize"];
}

function getTicketList(url) {
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
			showTicket(result);
			getPageLink(result);
			showTicketPageInfo(result)
		}
	});
}

function showTicket(result) {
	var str = "<tr style='height: 50px;'>" +
		"<th style='width: 240px;'>电影名</th>" +
		"<th>时间</th>" +
		"<th style='width: 200px;'>影厅：座位号</th>" +
		"<th>价格</th>" +
		"<th>操作</th>" +
		"</tr>";
	var deleteLink;
	var linkedTickets = result["linkedTicketDtos"];
	for (var i = 0; i < linkedTickets.length; i++) {
		var linkedTicket = linkedTickets[i];
		var ticket = linkedTicket["data"];
		// console.log(ticket);
		var linksForTicket = linkedTicket["linksForTicket"];
		for (var j = 0; j < linksForTicket.length; j++) {
			var link = linksForTicket[j];
			if (link["method"] == "Delete") {
				deleteLink = "\"" + link["href"] + "\"";
			}
		}
	str += "<tr>" +
		"<td>"+ ticket["showDto"]["movieName"] +"</td>" +
		"<td>"+ ticket["showDto"]["dateTime"].split("T")[0]+ ticket["showDto"]["showNum"] +"</td>" +
		"<td style='width: 200px;'>"+ ticket["showDto"]["hallName"] +"："+ ticket["seatNum"] +"号</td>" +
		"<td>"+ ticket["showDto"]["price"] +"</td>" +
		"<td><a href='javascript:;' onclick='deleteTicket(" + deleteLink + ")'>退票</a></td>" +
		"</tr>";
			
	}
	if (linkedTickets.length < 5) {
		for (var i = 0; i < (5 - linkedTickets.length); i++) {
			str += "<tr style='height: 50px;'><td></td><td></td><td></td><td></td><td></td></tr>";
		}
	}
	document.getElementById("ticketTable").innerHTML = str;
}

function getPageLink(result){
	document.getElementById("ticketPrevious").disabled = true;
	document.getElementById("ticketNext").disabled = true;
	var pageLinks = result["pageLink"];
	for(var i = 0;i< pageLinks.length;i++){
		link = pageLinks[i];
		// console.log(link);
		if(link["rel"] == "self"){
			ticketSelfPageUrl = link["href"];
		}else if(link["rel"] == "next_page"){
			ticketNextPageUrl = link["href"];
			document.getElementById("ticketNext").disabled = false;
		}else if(link["rel"] == "previous_page"){
			ticketPreviousPageUrl = link["href"];
			document.getElementById("ticketPrevious").disabled = false;
		}
	}
}

function deleteTicket(url){
	console.log(url);
	$.ajax({
		beforeSend: function(request) {
			request.setRequestHeader("Accept", "application/json");
			request.setRequestHeader("Content-type", "application/json");
			request.setRequestHeader("guid", guid);
		},
		type: "delete", //设置请求类型
		url: url, //请求后台的url地址
		success: function(result) { //请求成功后的回调函数，data为后台返回的值
			// alert("退票成功");
			getTicketList(ticketSelfPageUrl);
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
	window.location.href = "/addTicket.html?guid=" + guid + "&showId=" + showId;
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

function ticketNextPage(){
	getTicketList(ticketNextPageUrl);
}

function ticketPreviousPage(){
	getTicketList(ticketPreviousPageUrl);
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
	// console.log(userInfo);
	document.getElementById("currentUser").innerHTML += userInfo["userDto"]["userName"];
}
