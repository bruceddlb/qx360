<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html>
<head>
    <meta http-equiv="Content-Type" content="text/html; charset=gb2312" />
    <title>公交导航的数据接口</title>
    <script type="text/javascript" src="http://api.map.baidu.com/api?key=59db371659c04947a1ff044e80565718&v=1.1&services=true">
    </script>
    <script src="Content/scripts/jquery/jquery-2.0.3.min.js"></script>
</head>
<body>
    lng:<input id="text1" type="text" value="" />
    lat:<input id="text2" type="text" value="" />
    <input id="btn" type="button" value="查询" />
    <input id="text3" type="text" value="悦来酒家" />
    <input id="btn1" type="button" value="地址查询" />

    <div style="width: 520px; height: 200px; border: 1px solid gray" contenteditable="true" id="info"></div>
    <div style="width: 800px; height: 500px; border: 1px solid gray" id="container">
    </div>

</body>
</html>
<script type="text/javascript">

    var map = new BMap.Map("container");
    map.centerAndZoom(new BMap.Point(121.528599, 31.217681), 12);
    //map.centerAndZoom(new BMap.Point(114.309531, 30.59619), 50);
    //将控件添加到地图，一个控件实例只能向地图中添加一次  
    map.addControl(new BMap.MapTypeControl());
    //设置地图显示的城市，默认为武汉  
    map.setCurrentCity("宜昌");
    //map.centerAndZoom('宜昌伍家岗', 11);
    //map.addControl(new BMap.ScaleControl());
    //map.addControl(new BMap.OverviewMapControl());
    //var ctrl_nav = new BMap.NavigationControl({ anchor: BMAP_ANCHOR_TOP_LEFT, type: BMAP_NAVIGATION_CONTROL_LARGE });
    //map.addControl(ctrl_nav);

    //map.enableDragging(); //启用地图拖拽事件，默认启用(可不写)
    //map.enableScrollWheelZoom(); //启用地图滚轮放大缩小
    //map.enableDoubleClickZoom(); //启用鼠标双击放大，默认启用(可不写)
    //map.enableKeyboard(); //启用键盘上下左右键移动地图


    map.addEventListener("click", function () {
        var center = map.getCenter();
        document.getElementById("info").innerHTML = center.lng + ", " + center.lat;
    });

    // 编写自定义函数，创建标注   
    function addMarker(point) {

        // 创建图标对象   
        var myIcon = new BMap.Icon("http://api.map.baidu.com/img/markers.png", new BMap.Size(23, 25), {
            // 指定定位位置。   
            // 当标注显示在地图上时，其所指向的地理位置距离图标左上   
            // 角各偏移10像素和25像素。您可以看到在本例中该位置即是   
            // 图标中央下端的尖角位置。   
            offset: new BMap.Size(10, 25),
            // 设置图片偏移。   
            // 当您需要从一幅较大的图片中截取某部分作为标注图标时，您   
            // 需要指定大图的偏移位置，此做法与css sprites技术类似。   
            imageOffset: new BMap.Size(0, 0 - 1 * 25)   // 设置图片偏移   
        });

        // 创建标注对象并添加到地图   
        var marker = new BMap.Marker(point);
        map.addOverlay(marker);

        //移除标注
        marker.addEventListener("click", function () {
            //            map.removeOverlay(marker);
            //            marker.dispose();

            var opts = {
                width: 250,     // 信息窗口宽度
                height: 100,     // 信息窗口高度
                title: "lng:" + point.lng + "lat:" + point.lat  // 信息窗口标题
            }

            var infoWindow = new BMap.InfoWindow("", opts);  // 创建信息窗口对象
            marker.openInfoWindow(infoWindow, this.point);      // 打开信息窗口

        });
    }


    $(document).ready(function () {
        $("#btn").click(function () {
            //var point = new BMap.Point(parseFloat($("#text1").val()) + 0.0065, parseFloat($("#text2").val()) + 0.0065);
            //var point = new BMap.Point(parseFloat($("#text1").val()), parseFloat($("#text2").val()));
            var point = new BMap.Point(121.528599, 31.217681);
            alert(point);
            addMarker(point);
        });
        $("#btn1").click(function () {
            document.getElementById("info").innerHTML = "";
            var LocalSearch = new BMap.LocalSearch(map, {
                renderOptions: { map: map }
            });
            LocalSearch.search($("#text3").val());
            LocalSearch.setSearchCompleteCallback(function () {
                var item = LocalSearch.getResults();
                for (var i = 0; i < item.getNumPois() ; i++) {
                    var LocalResultPoi = item.getPoi(i);
                    if (LocalResultPoi) {
                        var point = new BMap.Point((LocalResultPoi.point.lat), (LocalResultPoi.point.lng));
                        // alert(point)
                        addMarker(point);
                    }
                    document.getElementById("info").innerHTML = document.getElementById("info").innerHTML + LocalResultPoi.title + "," + LocalResultPoi.point.lat + "," + LocalResultPoi.point.lng + LocalResultPoi.address + "<br>";
                }
            });


        });

    });

</script>

