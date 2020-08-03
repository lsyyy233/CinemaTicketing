var hallSelfPageUrl;
var hallNextPageUrl;
var hallPreviousPageUrl;



function getHallList(url) {
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
			showHalls(result);
			showHallPageInfo(result);
			getLinksForHallPage(result);
		}
	});
}

function showHallPageInfo(result) {
	var currentPage = result["currentPage"];
	var totalCount = result["totalCount"];
	var totalPages = result["totalPages"];
	document.getElementById("hallPageInfo").innerHTML = "当前页数:" + currentPage + "/总页数:" + totalPages + " 共" + totalCount +
		"条数据";
}

function showHalls(result) {
	var str = " <tr style='height: 50px;'>" +
		"<th style='width: 150px;'>名称</th>" +
		"<th style='width: 100px;'>座位数</th>" +
		"<th colspan='2'' style='width: 120px;'>操作</th>" +
		"</tr>";
	var halls = result["linkedHallDtos"];
	for (var i = 0; i < halls.length; i++) {
		hall = halls[i]["data"];
		//获取删除链接
		var deleteUrl = "\"" + halls[i]["linksForHall"][0]["href"] + "\"";
		//显示到前台
		str += "<tr style='height: 50px;'>";
		str += "<td>" + hall["name"] + "</td>";
		str += "<td>" + hall["seats"] + "</td>";
		str += "<td><a href='javascript:;' onclick='editHall(" + hall["id"] + ")'>" + "修改" +
			"</a></td>";
		str += "<td><a href='javascript:;' onclick='deleteHall(" + deleteUrl + ")'>" + "删除" +
			"</a></td>";
		// str += "<td><a href='/employees.html?companyId=" + companies[i]["id"] + "'>Employees</a></td>";
		str += "</tr>";
	}
	if (halls.length < 5) {
		for (var i = 0; i < (5 - halls.length); i++) {
			str += "<tr style='height: 50px;'></td><td></td><td></td><td></td><td></td></tr>";
		}
	}
	document.getElementById("hallTable").innerHTML = str;
}

function editHall(hallId) {
	alert("hallId = " + hallId);
}
//删除hall
function deleteHall(deleteLink) {
	console.log("deleteLink = " + deleteLink);
	$.ajax({
		beforeSend: function(request) {
			request.setRequestHeader("guid", guid);
		},
		type: "delete", //设置请求类型
		url: deleteLink, //请求后台的url地址
		success: function(result) { //请求成功后的回调函数
			getHallList(hallSelfPageUrl);
		}
	});

}

function jumpToAddHall() {

}

function hallPreviousPage() {

}

function hallNextPage() {

}

// 获取翻页链接
function getLinksForHallPage(result) {
	var linksForHalls = result["linksForHalls"]
	document.getElementById("Next").disabled = true;
	document.getElementById("Previous").disabled = true;
	// console.log(links);
	for (var i = 0; i < linksForHalls.length; i++) {
		link = linksForHalls[i];

		// console.log("link = " + JSON.stringify(link));
		if (link["rel"] == "next_page") {
			document.getElementById("Next").disabled = false;
			hallNextPageUrl = link["href"];
			// console.log("nextUrl = " + nextPageUrl);
		}
		if (link["rel"] == "previous_page") {
			document.getElementById("Previous").disabled = false;
			hallPreviousPageUrl = link["href"];
			// console.log("previousUrl = " + previousPageUrl);
		}
		if (link["rel"] == "self") {
			hallSelfPageUrl = link["href"];
			// console.log("nextUrl = " + nextPageUrl);
		}
	}
}
