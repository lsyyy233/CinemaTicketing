var movieNextPageUrl;
var moviePreviousUrl;
var movieThisPageUrl;

function getMovieList(url) {
	// console.clear();
	$.ajax({
		beforeSend: function(request) {
			request.setRequestHeader("Accept", "application/vnd.cinemaTicketing.hateoas+json");
			request.setRequestHeader("Content-type", "application/json");
		},
		type: "get", //设置请求类型
		url: url, //请求后台的url地址
		// data: "", //请求参数，是key-value形式的，如 {name:"jason"}
		success: function(result) { //请求成功后的回调函数，data为后台返回的值
			showMovies(result);
			getLinksForMoviePage(result);
			showMoviePageInfo(result);
		}
	});
}
//显示翻页信息
function showMoviePageInfo(result) {
	var currentPage = result["currentPage"];
	var totalCount = result["totalCount"];
	var totalPages = result["totalPages"];
	document.getElementById("moviePageInfo").innerHTML = "当前页数:" + currentPage + "/总页数:" + totalPages + " 共" + totalCount +
		"条数据";
}
//显示电影信息
function showMovies(result) {
	movies = result["linkedMovieDto"];
	var str = "<tr style='height: 50px; '><th style='width: 155px;'>电影名</th>"
	str += "<th colspan='3' style='width: 600px;' >简介</th>"
	str += "<th style='width: 100px;' >当前状态</th>"
	str += "<th colspan='2' style='width: 114px;'>操作</th></tr>"
	for (var i = 0; i < movies.length; i++) {
		var deleteLink;
		var links = movies[i]["links"];
		// alert(i + " " + links.length);
		for (var j = 0; j < links.length; j++) {
			// alert(links[i]);
			if (links[j]["method"] == "Delete") {
				deleteLink = "\"" + links[j]["href"] + "\"";
			}
		}
		movie = movies[i]["data"];
		// console.log(movie);
		str += "<tr style='height: 50px;'>";
		str += "<td>" + movie["name"] + "</td>";
		str += "<td colspan='3' align='left'>" + setString(movie["introduction"], 70) + "</td>";
		str += "<td>";
		if (movie["releaseStatu"] == 0) {
			str += "暂无场次";
		} else if (movie["releaseStatu"] == 1) {
			str += "正在上映";
		} else if (movie["releaseStatu"] == 2) {
			str += "已经下映";
		}
		// alert(deleteLink);
		str += "</td>";
		// str += "<td><a href='baidu.com'>" + "下映" + "</a></td>";
		str += "<td><a href='javascript:;' onclick='editMovie(" + movie["id"] + ")'>" + "修改" +
			"</a></td>";
		str += "<td><a href='javascript:;' onclick='deleteMovie(" + deleteLink + ")'>" + "删除" +
			"</a></td>";
		// str += "<td><a href='/employees.html?companyId=" + companies[i]["id"] + "'>Employees</a></td>";
		str += "</tr>";
	}
	if (movies.length < 5) {
		for (var i = 0; i < (5 - movies.length); i++) {
			str += "<tr style='height: 50px;'><td></td><td colspan='3'></td><td></td><td></td><td></td></tr>";
		}
	}
	document.getElementById("movieTable").innerHTML = str;
}
//修改movie
function editMovie(movieId){
	window.location.href = "/editMovie.html?movieId="+movieId+"&guid="+guid;
}
//删除movie
function deleteMovie(deleteLink) {
	console.log("deleteLink = " + deleteLink);
	$.ajax({
		beforeSend: function(request) {
			request.setRequestHeader("guid", guid);
		},
		type: "delete", //设置请求类型
		url: deleteLink, //请求后台的url地址
		success: function(result) { //请求成功后的回调函数
			getMovieList(movieThisPageUrl);
		}
	});
}
// 获取翻页链接
function getLinksForMoviePage(result) {
	var linksForMovies = result["linksForMovies"]
	document.getElementById("movieNext").disabled = true;
	document.getElementById("moviePrevious").disabled = true;
	// console.log(links);
	for (var i = 0; i < linksForMovies.length; i++) {
		link = linksForMovies[i];

		// console.log("link = " + JSON.stringify(link));
		if (link["rel"] == "next_page") {
			document.getElementById("movieNext").disabled = false;
			movieNextPageUrl = link["href"];
			// console.log("nextUrl = " + nextPageUrl);
		}
		if (link["rel"] == "previous_page") {
			document.getElementById("moviePrevious").disabled = false;
			moviePreviousUrl = link["href"];
			// console.log("previousUrl = " + previousPageUrl);
		}
		if (link["rel"] == "self") {
			movieThisPageUrl = link["href"];
			// console.log("nextUrl = " + nextPageUrl);
		}
	}
}
//跳转到下一页
function movieNextPage() {
	getMovieList(movieNextPageUrl);
}
//跳转到上一页
function moviePreviousPage() {
	getMovieList(moviePreviousUrl);
}
//跳转到添加Movie
function jumpToAddMovie() {
	window.location.href = "/addMovie.html" + "?guid=" + guid;
}
//截取字符串
function setString(str, len) {
	var strlen = 0;
	var s = "";
	for (var i = 0; i < str.length; i++) {
		if (str.charCodeAt(i) > 128) {
			strlen += 2;
		} else {
			strlen++;
		}
		s += str.charAt(i);
		if (strlen >= len) {
			return s + "...";
		}
	}
	return s;
}
