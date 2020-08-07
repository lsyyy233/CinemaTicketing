var ticketSelfPageUrl;
var ticketNextPageUrl;
var ticketPreviousPageUrl;

function getTicketList(url) {
	$.ajax({
		beforeSend: function(request) {
			request.setRequestHeader("Accept", "application/vnd.cinemaTicketing.hateoas+json");
			request.setRequestHeader("Content-type", "application/json");
		},
		type: "get", //设置请求类型
		url: url, //请求后台的url地址
		success: function(result) { //请求成功后的回调函数，data为后台返回的值
			// console.log(result);
			showTicket(result);
			getTicketPageLink(result);
			showTicketPageInfo(result)
		}
	});
}

function showTicketPageInfo(result) {
	var currentPage = result["currentPage"];
	var totalCount = result["totalCount"];
	var totalPages = result["totalPages"];
	document.getElementById("ticketPageInfo").innerHTML = "当前页数:" + currentPage + "/总页数:" + totalPages +
		" 共" + totalCount + "条数据";
	ShowPageNum = currentPage;
	ShowPageSize = result["pageSize"];
}

function getTicketPageLink(result) {
	document.getElementById("ticketPrevious").disabled = true;
	document.getElementById("ticketNext").disabled = true;
	var pageLinks = result["pageLink"];
	for (var i = 0; i < pageLinks.length; i++) {
		link = pageLinks[i];
		// console.log(link);
		if (link["rel"] == "self") {
			ticketSelfPageUrl = link["href"];
		} else if (link["rel"] == "next_page") {
			ticketNextPageUrl = link["href"];
			document.getElementById("ticketNext").disabled = false;
		} else if (link["rel"] == "previous_page") {
			ticketPreviousPageUrl = link["href"];
			document.getElementById("ticketPrevious").disabled = false;
		}
	}
}

function deleteTicket(url) {
	// console.log(url);
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
		},
		error: function(XMLHttpRequest, textStatus, errorThrown) {
			console.log(XMLHttpRequest.status);
		}
	});
}

function showTicket(result) {
	var str = "<tr style='height: 50px;'>" +
		"<th style='width: 180px;'>用户名</th>" +
		"<th style='width: 180px;'>电影名</th>" +
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
		// console.log(linkedTicket);
		var linksForTicket = linkedTicket["linksForTicket"];
		for (var j = 0; j < linksForTicket.length; j++) {
			var link = linksForTicket[j];
			if (link["method"] == "Delete") {
				deleteLink = "\"" + link["href"] + "\"";
			}
		}
		str += "<tr>" +
			"<td>" + ticket["userDto"]["userName"] + "</td>" +
			"<td>" + ticket["showDto"]["movieName"] + "</td>" +
			"<td>" + ticket["showDto"]["dateTime"].split("T")[0] + ticket["showDto"]["showNum"] + "</td>" +
			"<td style='width: 200px;'>" + ticket["showDto"]["hallName"] + "：" + ticket["seatNum"] + "号</td>" +
			"<td>" + ticket["showDto"]["price"] + "</td>" +
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

function ticketNextPage() {
	getTicketList(ticketNextPageUrl);
}

function ticketPreviousPage() {
	getTicketList(ticketPreviousPageUrl);
}
