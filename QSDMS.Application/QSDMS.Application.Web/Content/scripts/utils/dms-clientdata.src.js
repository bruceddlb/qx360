$(function () {
    $.getclientdata();
})
var clientdataItem = [];
var clientorganizeData = [];
var clientdepartmentData = [];
var clientpostData = [];
var clientroleData = [];
var clientuserGroup = [];
var clientuserData = [];
var authorizeMenuData = [];
var authorizeButtonData = [];
var authorizeColumnData = [];
$.getclientdata = function () {
    $.ajax({
        url: contentPath + "/ClientData/GetClientDataJson",
        type: "post",
        dataType: "json",
        async: false,
        success: function (data) {
            console.log(JSON.stringify(data));
            clientdataItem = data.dataItem;
            clientorganizeData = data.organize;
            clientdepartmentData = data.department;
            clientpostData = data.post;
            clientroleData = data.role;
            clientuserGroup = data.userGroup;
            clientuserData = data.user;
            authorizeMenuData = data.authorizeMenu;//菜单
            authorizeButtonData = data.authorizeButton;//按钮
            authorizeColumnData = data.authorizeColumn;//字段
            //alert(JSON.stringify(data.authorizeButton));
        },
        error: function (XMLHttpRequest, textStatus, errorThrown) {
            dialogMsg(errorThrown, -1);
        }
    });
}