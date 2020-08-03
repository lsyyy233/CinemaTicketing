var showSelfPageUrl;
var showNextPageUrl;
var showPreviousPageUrl;

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
		"<th colspan='2'>操作</th>" +
		"</tr>";
	var shows = result["linkedShowDtos"];

	for (var i = 0; i < shows.length; i++) {
		//获取删除链接
		var deleteUrl = "\"" + shows[i]["linksForShow"][0]["href"] + "\"";

		show = shows[i]["data"];
		str += " <tr style='height: 50px;'>" +
			"<td>" + show["movieName"] + "</td>" +
			"<td>" + show["showNum"] + "</td>" +
			"<td>" + show["dateTime"].split('T')[0] + "</td>" +
			"<td>" + show["hallName"] + "</td>" +
			"<td>" + show["price"] + "</td>" +
			"<td><a href='javascript:;' onclick='editShow(" + show["id"] + ")'>" + "修改" +
				"</a></td>" +
			"<td><a href='javascript:;' onclick='deleteShow(" + deleteUrl + ")'>" + "删除" +
				"</a></td>";
	}
	if (shows.length < 5) {
		for (var i = 0; i < (5 - shows.length); i++) {
			str += "<tr style='height: 50px;'><td></td><td></td><td></td><td></td><td></td><td></td><td></td></tr>";
		}
	}
	document.getElementById("showTable").innerHTML = str;
}

//删除show
function deleteShow(deleteLink) {
	if ((guid == null) || guid == "") {
		alert("未登录！");
		return;
	}
	$.ajax({
		beforeSend: function(request) {
			request.setRequestHeader("guid", guid);
		},
		type: "delete", //设置请求类型
		url: deleteLink, //请求后台的url地址
		success: function(result) { //请求成功后的回调函数
			getShowList(showSelfPageUrl);
		}
	});

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

function jumpToAddShow() {
	window.location.href = "/addShow.html" + "?guid=" + guid;
}

function showPreviousPage() {
	getShowList(showPreviousPageUrl);
}

function showNextPage() {
	getShowList(showNextPageUrl);
}

function editShow(showId){
	window.location.href="/editShow.html?guid=" + guid+"&showId="+showId;
}
