<%@ Page Language="C#" Inherits="System.Web.Mvc.ViewPage" %>

<!DOCTYPE html>

<html>
<head runat="server">
    <meta name="viewport" content="width=device-width" />
    <title>功能导航页</title>
    <script src="Scripts/lib/html5media.min.js"></script>
    <link rel="stylesheet" href="/Content/assets/hivideo.css" />
    <script type="text/javascript" src="/Content/assets/hivideo.js"></script>
    <style type="text/css">
        .main-wrap {
            margin: 0 auto;
            min-width: 320px;
            max-width: 640px;
        }

            .main-wrap video {
                width: 100%;
            }
    </style>
    <script>

        //--创建页面监听，等待微信端页面加载完毕 触发音频播放
        document.addEventListener('DOMContentLoaded', function () {
            function audioAutoPlay() {
                var audio = document.getElementById('video');
                audio.play();
                document.addEventListener("WeixinJSBridgeReady", function () {
                    audio.play();
                }, false);
            }
            audioAutoPlay();
        });
        //--创建触摸监听，当浏览器打开页面时，触摸屏幕触发事件，进行音频播放
        document.addEventListener('touchstart', function () {
            function audioAutoPlay() {
                var audio = document.getElementById('video');
                audio.play();
            }
            audioAutoPlay();
        });
    </script>
</head>
<body>
    <div style="float: left; margin-right: 100px;">
        <h2>驾培服务</h2>
        <ul>
            <li><a target="_blank" href="/Apply/Index">驾校报名</a>
            <li><a target="_blank" href="/Study/Index">预约学车</a>
            <li><a target="_blank" href="/Exam/Index">我的考试</a>
            <li><a target="_blank" href="/Training/nav">考场实训</a>
            <li><a target="_blank" href="/WithDriving/nav">陪驾服务</a>
        </ul>
    </div>
    <div style="float: left; margin-right: 100px;">
        <h2>车辆服务</h2>
        <ul>
            <li><a target="_blank" href="/Shop/List">汽车商城</a>
            <li><a target="_blank" href="/VsInsurance/Index">预约保险</a>
            <li><a target="_blank" href="/Audit/nav">年检服务</a>
            <li><a target="_blank" href="/TakeAudit/Index">上门代审</a>
            <li><a target="_blank" href="/Apply/Index">违章咨询</a>
        </ul>
    </div>
    <div style="float: left; margin-right: 100px;">
        <h2>会员中心</h2>
        <ul>
            <li><a target="_blank" href="/Mycenter/Index">会员中心</a>
            <li><a target="_blank" href="/MaCenter/Login?oauthflag=1">教练工作平台</a>
            <li><a target="_blank" href="/ExamPlaceCenter/Login?oauthflag=1">考场工作平台</a>
            <li><a target="_blank" href="/MasterCenter/Login?oauthflag=1">管理员工作平台</a>
            <li><a target="_blank" href="/Account/Login?oauthflag=1">登陆</a>
            <li><a target="_blank" href="/Account/Login?sig=out">清除缓存</a>
            <li><a target="_blank" href="https://pro.modao.cc/app/0hEERyc52W7cKRPStYaLYGW5jlC4KRi#screen=s32D4F793DF1510274033594">前台原型</a>
            <li><a target="_blank" href="https://pro.modao.cc/app/ts4bG3k7QUirMhQ1JeH5tPgJkDBvNkG#screen=s497E8828E01511900776029">后台原型</a>
        </ul>
    </div>
    <div style="clear: both"></div>
    <%--  <video  src="222.mp4" controls  autoplay ></video>--%>
    <%--  <div class="main-wrap" style="height:420px">
        <video ishivideo="true" autoplay="true" isrotate="false" autohide="true">
            <source src="11.mp4" type="video/mp4">
        </video>
    </div>--%>
</body>
</html>
