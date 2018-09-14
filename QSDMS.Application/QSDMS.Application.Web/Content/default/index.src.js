//定义全局变量存储菜单数组
var tablist = [];
var navJson = {}
$(document).ready(function () {
    //点击最小菜单栏
    $(".navbar-minimalize").click(function () {
        $.dmsindex.minimaze();
    });

    //绑定加载和缩小事件
    $(window).unbind("load resize").bind("load resize", function () {
        if ($(this).width() < 769) {
            $("body").addClass("mini-navbar");
            $(".navbar-static-side").fadeIn();
            $(".navbar-minimalize").click();
            $.dmsindex.minimaze();
        }
        //内容自动变化宽高
        $('.mainContent').width("100%").height($(window).height() - 150);
    });

    //全屏控制按钮事件
    $('.fullscreen').on('click', function () {
        if (!$(this).attr('fullscreen')) {
            $(this).attr('fullscreen', 'true');
            $.dmsindex.requestFullScreen();
        } else {
            $(this).removeAttr('fullscreen')
            $.dmsindex.exitFullscreen();
        }
    });
})

//处理方法
$.dmsindex = {
    load: function () {
        window.setTimeout(function () {
            $('#ajax-loader').fadeOut();
            Loading(false);
        }, 100);
        //加载菜单
        $.dmsindex.loadMenu();
    },
    jsonWhere: function (data, action) {
        if (action == null) return;
        var reval = new Array();
        $(data).each(function (i, v) {
            if (action(v)) {
                reval.push(v);
            }
        })
        return reval;
    },
    //加载菜单
    loadMenu: function () {
        //功能对象
        navJson = {};
        tablist = $("#tab_list_add").Tab({
            items: [
                { id: 'cfc43ec6-937c-415e-b2f8-85a9664553c9', title: '欢迎首页', closed: false, icon: 'fa fa fa-desktop', url: contentPath + '/Home/Desktop' },
            ],
            tabScroll: true,
            //width: $(window).width() - $("#side-menu").width(),//页面宽度-菜单的宽度
            //height: $(window).height() - $("#main-footer").height() * 2,
            tabcontentWidth: $(window).width() - $("#side-menu").width(),//页面宽度-菜单的宽度

            addEvent: function (item) {
                if (item.closed) {
                    $.post(contentPath + "/Home/VisitModule", { moduleId: item.id, moduleName: item.title, moduleUrl: item.url }, function (data) { });
                }
            },
            leaveEvent: function (item) {
            },
            currentEvent: function (moduleId) {
                $.cookie('currentmoduleId', moduleId, { path: "/", expires: 7 });
            }
        });
        //加载菜单       
        var data = authorizeMenuData;//获取查询对象        
        var _html = "";
        $.each(data, function (i) {
            var row = data[i];
            if (row.ParentId == "0") {
                if (i == 0) {
                    _html += '<li class="nav active">';
                } else {
                    _html += '<li class="nav">';
                }
                var childNodes = $.dmsindex.jsonWhere(data, function (v) { return v.ParentId == row.ModuleId });
                if (childNodes.length > 0) {
                    _html += '<a class="" data-id="" href="javascript:void(0)">'
                    _html += '<i class="' + row.Icon + '"></i><span class="nav-label">' + row.FullName + '</span><span class="fa arrow"></span>'
                    _html += '</a>'

                } else {
                    _html += '<a class="menuItem" data-id="' + row.ModuleId + '" href="javascript:void(0)">'
                    _html += '<i class="' + row.Icon + '"></i><span class="nav-label">' + row.FullName + '</span>'
                    _html += '</a>'
                    navJson[row.ModuleId] = row;

                }
                if (childNodes.length > 0) {
                    _html += '<ul class="nav nav-second-level">';
                    $.each(childNodes, function (i) {
                        var subrow = childNodes[i];
                        var subchildNodes = $.dmsindex.jsonWhere(data, function (v) { return v.ParentId == subrow.ModuleId });
                        _html += '<li>';
                        if (subchildNodes.length > 0) {
                            _html += '<a href="#"><i class="' + subrow.Icon + '"></i>' + subrow.FullName + '';
                            _html += '<span class="fa arrow"></span>';
                            _html += '</a>';
                            _html += '<ul class="nav nav-third-level">';
                            $.each(subchildNodes, function (i) {
                                var subchildNodesrow = subchildNodes[i];
                                _html += '<li><a class="menuItem" data-id="' + subchildNodesrow.ModuleId + '" href="javascript:void(0)"><i class="' + subchildNodesrow.Icon + '"></i>' + subchildNodesrow.FullName + '</a></li>';

                                navJson[subchildNodesrow.ModuleId] = subchildNodesrow;
                            });
                            _html += '</ul>';

                        } else {
                            _html += '<a class="menuItem" data-id="' + subrow.ModuleId + '" href="javascript:void(0)"><i class="' + subrow.Icon + '"></i>' + subrow.FullName + '</a>';

                            navJson[subrow.ModuleId] = subrow;
                        }
                        _html += '</li>';
                    });
                    _html += '</ul>';
                }
                _html += '</li>'
            }
        });
        //添加内容
        $("#side-menu").empty().append(_html);
        //添加事件
        $("#side-menu").metisMenu();

        //当左边菜单最小时候点击元素打开菜单
        $("#side-menu li").each(function () {
            var $this = $(this);
            $this.bind("click", function () {
                if ($("body").hasClass("mini-navbar")) {
                    $(".navbar-minimalize").trigger("click");
                }
            })
        });

        //点击打开功能页面
        $("#side-menu li a[class='menuItem']").bind("click", function () {
            //添加移除样式
            var $this = $(this).parent();
            $this.siblings().removeClass("active");
            $this.addClass("active");
            var id = $(this).attr("data-id");
            var data = navJson[id];
            if (data) {
                tablist.newTab({ id: id, title: data.FullName, closed: true, icon: data.Icon, url: contentPath + data.UrlAddress });
            }
        })
        //滚动条 加了滚动条，当页面最下化的时候，鼠标移到菜单，子菜单不弹出
        //$(".sidebar-collapse").slimScroll({
        //    height: "100%",
        //    railOpacity: 0.9,
        //    alwaysVisible: !1
        //})

    },//全屏
    requestFullScreen: function () {
        var de = document.documentElement;
        if (de.requestFullscreen) {
            de.requestFullscreen();
        } else if (de.mozRequestFullScreen) {
            de.mozRequestFullScreen();
        } else if (de.webkitRequestFullScreen) {
            de.webkitRequestFullScreen();
        }

    },
    //退出全屏
    exitFullscreen: function () {
        var de = document;
        if (de.exitFullscreen) {
            de.exitFullscreen();
        } else if (de.mozCancelFullScreen) {
            de.mozCancelFullScreen();
        } else if (de.webkitCancelFullScreen) {
            de.webkitCancelFullScreen();
        }
    },
    //退出系统
    indexOut: function () {
        dialogConfirm("注：您确定要退出本次登录吗？", function (r) {
            if (r) {
                Loading(true, "正在安全退出...");
                window.setTimeout(function () {
                    $.ajax({
                        url: contentPath + "/Login/OutLogin",
                        type: "post",
                        dataType: "json",
                        success: function (data) {
                            window.location.href = contentPath + "/Login/Default";
                        }
                    });
                }, 500);
            }
        });
    },
    //关闭tab选项卡
    removeTab: function (type) {
        var Id = tabiframeId().substr(12);
        var $tab = $(".tab-div");
        var $tabContent = $(".mainContent");
        switch (type) {
            case "reloadCurrent":
                // $.currentIframe().window.reload();
                $.currentIframe().window.location.href = $.currentIframe().window.location.href;
                break;
            case "closeCurrent":
                remove(Id);
                break;
            case "closeAll":
                $tab.find("div.tab_close").each(function () {
                    var id = $(this).parents('.inner').attr('id');
                    remove(id);
                });
                break;
            case "closeOther":
                $tab.find("div.tab_close").each(function () {
                    var id = $(this).parents('.inner').attr('id');
                    if (Id != id) {
                        remove(id);
                    }
                });
                break;
            default:
                break;
        }
        //移除对象
        function remove(id) {
            var li_index = $tab.find('li').length;
            $tab.find("#" + id).parents('li').remove();
            $tabContent.find('#tabs_iframe_' + id).remove();
            $tab.find('li:eq(' + (li_index - 2) + ')').find('table td:eq(1)').trigger("click");
        }
    },
    scrollTab: function (flag) {
        ////当前的left
        //var $tab = $(".tab");
        //var left = Number($tab.css('left').replace('px', ''));
        ////当前选择的tab宽度
        //var wd = ($tab.find("li[class='on']").css("width").replace('px', ''));
        ////所有tab宽度
        //var tabcontentWh = 0;
        //if (flag) {
        //    left = left + Number(wd);
        //    $tab.animate({ 'left': parseInt(left) });
        //} else {
        //    left = left - Number(wd);
        //    $tab.animate({ 'left': parseInt(left) });
        //}
        if (flag) {
            //向左滚动
            var nav = $("#tab_list_add").find("ul");
            var left = parseInt(nav.css("margin-left"));
            var wwidth = parseInt($(".page-content").width());
            var navwidth = parseInt(nav.width());
            var allshowleft = -(navwidth - wwidth + 100);
            if (allshowleft !== left && navwidth > wwidth - 100) {
                var temp = (left - 500);
                nav.animate({
                    "margin-left": (temp < allshowleft ? allshowleft : temp) + "px"
                },
                150)
            }
        } else {
            //向右滚动
            var nav = $("#tab_list_add").find("ul");
            var left = parseInt(nav.css("margin-left"));
            if (left !== 0) {
                nav.animate({
                    "margin-left": (left + 500 > 0 ? 0 : (left + 500)) + "px"
                },
                150)
            }
        }

    },
    minimaze: function () {
        $("body").toggleClass("mini-navbar");
        $("body").hasClass("mini-navbar") ? $("body").hasClass("fixed-sidebar") ? ($("#side-menu").hide(), setTimeout(function () {
            $("#side-menu").fadeIn(500)
        }, 300)) : $("#side-menu").removeAttr("style") : ($("#side-menu").hide(), setTimeout(function () { $("#side-menu").fadeIn(500) }, 100));

    }
}
//tab
$.fn.Tab = function (options) {
    var cfg = {
        items: [],
        width: '100%',
        height: '100%',
        tabcontentWidth: 300,
        tabWidth: 100,
        tabHeight: 40.5,
        tabScroll: false,
        tabScrollWidth: 19,
        tabClass: 'tab',
        tabContentClass: 'mainContent',
        tabClassOn: 'on',
        tabClassOff: 'off',
        tabClassClose: 'tab_close',
        tabClassInner: 'inner',
        tabClassInnerLeft: 'innerLeft',
        tabClassInnerMiddle: 'innerMiddle',
        tabClassInnerRight: 'innerRight',
        tabClassText: 'text',
        tabClassScrollLeft: 'scroll-left',
        tabClassScrollRight: 'scroll-right',
        tabClassDiv: 'tab-div',
        addEvent: null,
        leaveEvent: null,
        currentEvent: null
    };
    //默认显示第一个li
    var displayLINum = 0;
    $.extend(cfg, options);
    //判断是不是有隐藏的tab
    var tW = cfg.tabWidth * cfg.items.length;
    cfg.tabScroll ? tW -= cfg.tabScrollWidth * 2 : null;
    //tabDiv,该div是自动增加的
    var tab = $('<div />').attr('id', 'jquery_tab_div').height(cfg.tabHeight).addClass(cfg.tabClass).append('<ul />');
    //tab target content
    var tabContent = $('<div />').addClass(cfg.tabContentClass).width(cfg.tabcontentWidth).height(cfg.height - cfg.tabHeight);
    var ccW = (cfg.items.length * cfg.tabWidth) - cfg.width;
    var tabH = '';
    //增加一个tab下的li得模板
    var tabTemplate = '';
    tabTemplate = '<table class="' + cfg.tabClassInner + '"  id="{0}" border="0" cellpadding="0" cellspacing="0"><tr>' + '<td class="' + cfg.tabClassInnerLeft + '"></td>'
			+ '<td class="' + cfg.tabClassInnerMiddle + '"><span class="' + cfg.tabClassText + '"><i class="fa {2}"></i>&nbsp;{1}</span></td>' + '<td class="' + cfg.tabClassInnerMiddle + '"><div class="' + cfg.tabClassClose + ' ' + cfg.tabClassClose + '_noselected"></i></div></td>' + '<td class="' + cfg.tabClassInnerRight + '"></td>'
			+ '</tr></table>';
    var scrollTab = function (o, flag) {
        //当前的left
        var displayWidth = Number(tab.css('left').replace('px', ''));
        !displayWidth ? displayWidth = 0 : null;
        //显示第几个LI
        var displayNum = 0;
        var DW = 0;
        var left = 0;
        if (flag && displayWidth == 0) {
            return;
        } if (flag) {
            displayLINum--;
            left = displayWidth + tab.find('li').eq(displayLINum).width();
            left > 0 ? left = 0 : null;
        } else {
            var _rigth = $(".tab ul").width() - parseInt(tab.offset().left) * -1 - cfg.tabcontentWidth - 82;
            var _step = tab.find('li').eq(displayLINum).width();
            if (_rigth > 0) {
                if (_rigth < _step) {
                    _step = _rigth;
                }
            } else {
                return;
            }
            left = displayWidth - _step;
            displayLINum++;
        }
        if (left == 0) {
            tab.animate({ 'left': parseInt(-19) });
        } else {
            tab.animate({ 'left': parseInt(left) });
        }
    }
    function removeTab(item) {
        tab.find("#" + item.id).parents('li').remove();
        tabContent.find('#tabs_iframe_' + item.id).remove();
    }
    function addTab(item) {
        if (item.replace == true) {
            removeTab(item);
        }

        if (tab.find("#" + item.id).length == 0) {
            Loading(true);
            var innerString = tabTemplate.replace("{0}", item.id).replace("{1}", item.title).replace("{2}", item.icon);
            var liObj = $('<li class="off"></li>');
            liObj.append(innerString).appendTo(tab.find('ul')).find('table td:eq(1)').click(function () {
                liObj.Contextmenu();
                //判断当前是不是处于激活状态
                if (liObj.hasClass(cfg.tabClassOn)) return;

                var activeLi = liObj.parent().find('li.' + cfg.tabClassOn);
                activeLi.removeClass().addClass(cfg.tabClassOff);

                $(this).next().find('div').removeClass().addClass(cfg.tabClassClose);
                liObj.removeClass().addClass(cfg.tabClassOn);

                tabContent.find('iframe').hide().removeClass(cfg.tabClassOn);
                tabContent.find('#tabs_iframe_' + liObj.find('table').attr('id')).show().addClass(cfg.tabClassOn);

                cfg.currentEvent(liObj.find('.inner').attr('id'));
            })
            if (item.url) {
                var $iframe = $("<iframe id=\"tabs_iframe_" + item.id + "\" height=\"100%\" width=\"100%\" src=\"" + item.url + "\" frameBorder=\"0\"></iframe>")
                tabContent.append($iframe);
                $iframe.load(function () {
                    window.setTimeout(function () {
                        Loading(false);
                    }, 200);
                });
            }
            if (item.closed) {
                liObj.find('td:eq(2)').find('div').click(function () {
                    var li_index = tab.find('li').length;
                    removeTab(item);
                    tab.find('li:eq(' + (li_index - 2) + ')').find('table td:eq(1)').trigger("click");
                    cfg.leaveEvent(item);
                }).hover(function () {
                    if (liObj.hasClass(cfg.tabClassOn)) return;
                    $(this).removeClass().addClass(cfg.tabClassClose);
                }, function () {
                    if (liObj.hasClass(cfg.tabClassOn)) return;
                    $(this).addClass(cfg.tabClassClose + '_noselected');
                });
            }
            else {
                liObj.find('td:eq(2)').html('');
            }
            tab.find('li:eq(' + (tab.find('li').length - 1) + ')').find('table td:eq(1)').trigger("click");
            cfg.addEvent(item);
        } else {
            tab.find('li').removeClass('on').addClass('off');
            tab.find("#" + item.id).parent().removeClass('off').addClass('on');
            tabContent.find('iframe').hide().removeClass('on');
            tabContent.find('#tabs_iframe_' + item.id).show().addClass('on');
        }
        //判断容器的宽度，自动移位到当前的选项卡
        var nav = $("#tab_list_add").find("ul");
        var wwidth = parseInt($(".page-content").width());
        var navwidth = parseInt(nav.width());
        if (wwidth - 100 < navwidth) {
            nav.animate({
                "margin-left": "-" + (navwidth - wwidth + 100) + "px"
            },
            150)
        }
    }
    function newTab(item) {
        $.cookie('currentmoduleId', item.id, { path: "/", expires: 7 });
        addTab(item);
        var nW = $(".tab ul").width() - 4;
        if (nW > cfg.width) {
            if (!cfg.tabScroll) {
                cfg.tabScroll = true;
                scrollLeft = $('<div class="' + cfg.tabClassScrollLeft + '"><i></i></div>').click(function () {
                    scrollTab(scrollLeft, true);
                });
                srcollRight = $('<div class="' + cfg.tabClassScrollRight + '"><i></i></div>').click(function () {
                    scrollTab(srcollRight, false);
                });
                cW -= cfg.tabScrollWidth * 2;
                tabContenter.width(cW);
                scrollLeft.insertBefore(tabContenter);
                srcollRight.insertBefore(tabContenter);
            }
            var _left = cfg.width - nW;
            tab.animate({ 'left': _left - 43 });
            displaylicount = tab.find('li').length;
        }
    }
    $.each(cfg.items, function (i, item) {
        addTab(item);
    });
    var scrollLeft, srcollRight;
    if (cfg.tabScroll) {
        scrollLeft = $('<div class="' + cfg.tabClassScrollLeft + '"><i></i></div>').click(function () {
            scrollTab($(this), true);
        });
        srcollRight = $('<div class="' + cfg.tabClassScrollRight + '"><i></i></div>').click(function () {
            scrollTab($(this), false);
        });
        cfg.width -= cfg.tabScrollWidth * 2;
    }
    var container = $('<div />').css({
        'position': 'relative',
        'width': cfg.width,
        'height': cfg.tabHeight
    }).append(scrollLeft).append(srcollRight).addClass(cfg.tabClassDiv);
    var tabContenter = $('<div />').css({
        'width': cfg.width,
        'height': cfg.tabHeight,
        'float': 'left'
    }).append(tab);
    var obj = $(this).append(tabH).append(container.append(tabContenter));//.append(tabContent);    
    obj.closest(".content-tabs").next().append(tabContent);
    //点击第一
    tab.find('li:first td:eq(1)').click();
    return obj.extend({ 'addTab': addTab, 'newTab': newTab });
};
//共子页面调用方法
function OpenNav(id, para) {
    if (id) {
        if (tablist != null && navJson != null) {
            var data = navJson[id];
            if (para) {

                var parajson = JSON.stringify(para);
                parajson = encodeURIComponent(parajson);
                tablist.newTab({ id: id, title: data.FullName, closed: true, icon: data.Icon, url: contentPath + data.UrlAddress + "?queryjson=" + parajson + "" });
                //queryJson: JSON.stringify(queryJson)
            } else {

                tablist.newTab({ id: id, title: data.FullName, closed: true, icon: data.Icon, url: contentPath + data.UrlAddress });
            }
        }
    } else {
        alert("无效菜单项编号");
    }

}