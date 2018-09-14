

/**************导出 状态提示功能 add by bruced  begin**********************************/

var iscompleted = true; //提交状态
function htmlToElement(html) {
    var div = document.createElement("div");
    div.innerHTML = html;
    var element = div.firstChild;
    div.removeChild(element);
    return element;
}

//创建iframe
function createIframe(id) {
    var iframe = htmlToElement('<iframe src="javascript:false" name="' + id + '" />');
    iframe.setAttribute("id", id);
    iframe.style.display = "none";
    return iframe;
}
//获取对象
function getIframeDocument(iframe) {
    return iframe.contentDocument ? iframe.contentDocument : iframe.contentWindow.document;
}
//创建From
function createForm(iframeTarget, actionUrl, cacheid) {
    var form = htmlToElement('<form method="post" lang="ca" enctype="multipart/form-data"></form>');
    form.setAttribute("action", actionUrl);
    form.setAttribute("target", iframeTarget.getAttribute("name"));
    var input = createInput("cacheid", "cacheid", "hidden");
    input.setAttribute("value", cacheid);
    form.appendChild(input);
    var submit = createInput('bruced', 'bruced', 'submit');
    form.appendChild(submit);
    return form;
}
//创建控件
function createInput(id, name, type) {
    var input = document.createElement("input");
    input.setAttribute("id", id);
    input.setAttribute("name", name);
    input.setAttribute("type", type);
    return input;
}
//执行方法
function doProgressExport(para, url, iscompletedMsg,tipsId) {
    $("#" + tipsId).fadeIn();
    if (!iscompleted) {
        $("#" + iscompletedMsg).show("fast");
        setTimeout(function () { $("#" + iscompletedMsg).hide("fast"); }, 3000);
        return false;
    }
    iscompleted = false;
    //debugger;
    var iframe = createIframe("exportIframe");
    var iframeTarget = createIframe("iframeTarget");
    var actionUrl = url;
    var cacheid = "cacheid_" + (Math.random() * 100000000);
    var form = createForm(iframeTarget, actionUrl, cacheid);
    //创建多个参数
    if (para) {
        var paraarr = para.split("$$");
        for (var i = 0; i < paraarr.length; i++) {
            var paraitem = paraarr[i].split("|");
            var name = paraitem[0];
            var value = paraitem[1];
            var inputdata = createInput(name, name, "hidden");
            inputdata.setAttribute("value", encodeURI(value));
            form.appendChild(inputdata);
        }
    }
    document.body.appendChild(iframe);
    iframe.appendChild(iframeTarget);
    iframe.appendChild(form);
    form.submit();
    var state = "processing";
    $("#" + tipsId).html("<span class='' style='color:#FF6600;'><img src='/Content/Images/stateloading.gif' alt='加载中' style='height:20px;width:20px;vertical-align:-6px;' class='ml mr' />正在执行...</span>");
    //进度监听
    var progressWatcher = function () {
        $.ajax({
            type: "POST",
            cache: false,
            url: "/Resources/Ajax/AjaxImportState.ashx",
            data: { "action": "queryProcessing", "cacheid": cacheid },
            success: function (data) {
                var state = data.split("|")[0];
                var currentRow = data.split("|")[1];
                var totalRow = data.split("|")[2];
                switch (state) {
                    case "processing":
                        $("#" + tipsId).html("<span style='color:#FF6600;'><img src='/Content/Images/stateloading.gif' alt='加载中' style='height:20px;width:20px;vertical-align:-6px;' class='ml mr' />正在处理...</span>");
                        state = "processing";
                        setTimeout(progressWatcher, 2000);
                        break;
                    case "IsBackground":
                        $("#" + tipsId).html("<font color='#4ab618'>程序后台处理中...</font>");
                        $.get("/Resources/Ajax/AjaxImportState.ashx", { "action": "removeCache", "cacheid": cacheid });
                        state = "IsBackground";
                        cacheid = "cache" + (Math.random() * 100000000);
                        iscompleted = true;
                        //隐藏提示
                        setTimeout(function () { hideMsg(tipsId) }, 2000);
                        break;
                    case "done":
                        $("#" + tipsId).html("<font color='#4ab618'>处理完毕...</font>");
                        $.get("/Resources/Ajax/AjaxImportState.ashx", { "action": "removeCache", "cacheid": cacheid });
                        state = "done";
                        cacheid = "cache" + (Math.random() * 100000000);
                        iscompleted = true;
                        //隐藏提示
                        setTimeout(function () { hideMsg(tipsId) }, 2000);

                        break;
                    case "error":
                    default:
                        cacheid = "cache" + (Math.random() * 100000000);
                        state = "error";
                        $("#" + tipsId).html("<font color='red'>数据处理失败,操作未完成</font>");
                        iscompleted = true;
                        break;
                }
            },
            error: function (msg) {
                alert(msg.responseText);
            }
        });
    }

    var hideMsg = function (id) {
        $("#" + id).fadeOut();
    }
    setTimeout(progressWatcher, 100);
}
/**************导出 状态提示功能 add by bruced    end**********************************/