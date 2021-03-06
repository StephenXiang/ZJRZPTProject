// JavaScript Document


var dialogInstace, onMoveStartId, mousePos = { x: 0, y: 0 };	//	用于记录当前可拖拽的对象

// var zIndex = 9000;

//	获取元素对象	
function g(id) { return document.getElementById(id); }

//	自动居中元素（el = Element）
function autoCenter(el) {
    var bodyW = document.documentElement.clientWidth;
    var bodyH = document.documentElement.clientHeight;

    var elW = el.offsetWidth;
    var elH = el.offsetHeight;

    el.style.left = (bodyW - elW) / 2 + 'px';
    el.style.top = (bodyH - elH) / 2 + 'px';

}

//	自动扩展元素到全部显示区域
function fillToBody(el) {
    el.style.width = document.documentElement.clientWidth + 'px';
    el.style.height = document.documentElement.clientHeight + 'px';
}

//	Dialog实例化的方法
function Dialog(dragId, moveId) {

    var instace = {};

    instace.dragElement = g(dragId);	//	允许执行 拖拽操作 的元素
    instace.moveElement = g(moveId);	//	拖拽操作时，移动的元素

    instace.mouseOffsetLeft = 0;			//	拖拽操作时，移动元素的起始 X 点
    instace.mouseOffsetTop = 0;			//	拖拽操作时，移动元素的起始 Y 点

    instace.dragElement.addEventListener('mousedown', function (e) {

        var e = e || window.event;

        dialogInstace = instace;
        instace.mouseOffsetLeft = e.pageX - instace.moveElement.offsetLeft;
        instace.mouseOffsetTop = e.pageY - instace.moveElement.offsetTop;

        onMoveStartId = setInterval(onMoveStart, 10);
        return false;
        // instace.moveElement.style.zIndex = zIndex ++;
    })

    return instace;
}

//	在页面中侦听 鼠标弹起事件
document.onmouseup = function (e) {
    dialogInstace = false;
    clearInterval(onMoveStartId);
}
document.onmousemove = function (e) {
    var e = window.event || e;
    mousePos.x = e.clientX;
    mousePos.y = e.clientY;


    e.stopPropagation && e.stopPropagation();
    e.cancelBubble = true;
    e = this.originalEvent;
    e && (e.preventDefault ? e.preventDefault() : e.returnValue = false);

    document.body.style.MozUserSelect = 'none';
}

function onMoveStart() {

    var instace = dialogInstace;
    if (instace) {

        var maxX = document.documentElement.clientWidth - instace.moveElement.offsetWidth;
        var maxY = document.documentElement.clientHeight - instace.moveElement.offsetHeight;

        instace.moveElement.style.left = Math.min(Math.max((mousePos.x - instace.mouseOffsetLeft), 0), maxX) + "px";
        instace.moveElement.style.top = Math.min(Math.max((mousePos.y - instace.mouseOffsetTop), 0), maxY) + "px";

    }

}





//	重新调整对话框的位置和遮罩，并且展现
function showDialog() {
    g('dialogMove').style.display = 'block';
    g('mask').style.display = 'block';
    $('#login-passwords').val("");
    autoCenter(g('dialogMove'));
    fillToBody(g('mask'));
}

//	关闭对话框
function hideDialog() {
    g('dialogMove').style.display = 'none';
    g('mask').style.display = 'none';
}

//	重新调整对话框的位置和遮罩，并且展现
function showDialogReg() {
    g('reg').style.display = 'block';
    g('mask').style.display = 'block';
    $('#regist-pwd').val("");
    $('#regist-pwd-confirm').val("");
    autoCenter(g('reg'));
    fillToBody(g('mask'));
}

//	关闭对话框
function hideDialogReg() {
    g('reg').style.display = 'none';
    g('mask').style.display = 'none';
}

function showLoginedInfo(username) {
    g('head_l_info').style.display = 'block';
    $('#logined-username').html(username);
    g('head_l_con').style.display = 'none';
}
function hideLoginedInfo() {
    g('head_l_info').style.display = 'none';
    $('#logined-username').html("");
    g('head_l_con').style.display = 'block';
}


//	侦听浏览器窗口大小变化
//window.onresize = showDialog;

Dialog('dialogDrag', 'dialogMove');
Dialog('dialogDragreg', 'reg');

//默认设置弹出层启动
//showDialog();


$("#closeAd1").click(function () {
    $("#newpro").hide();
});



$(".prosinb").click(function () {
    $(".prosinb").removeClass("limit");
    $(".prosinb").addClass("nomal");
    $(this).removeClass("nomal");
    $(this).addClass("limit");
});
$(".prosinc").click(function () {
    $(".prosinc").removeClass("limit");
    $(".prosinc").addClass("nomal");
    $(this).removeClass("nomal");
    $(this).addClass("limit");
});
$(".prosind").click(function () {
    $(".prosind").removeClass("limit");
    $(".prosind").addClass("nomal");
    $(this).removeClass("nomal");
    $(this).addClass("limit");
});
$(".prosine").click(function () {
    $(".prosine").removeClass("limit");
    $(".prosine").addClass("nomal");
    $(this).removeClass("nomal");
    $(this).addClass("limit");
});

function initLogin() {
    if ($.cookie('UserInfo') != undefined && $.cookie('UserInfo') != "" && $.cookie('UserInfo') != null) {
        var userInfo = JSON.parse($.cookie('UserInfo'));
        if (userInfo != null) {
            var uname = userInfo["name"];
            $.ajax({
                type: "get",
                dataType: "text",
                async: true,
                data: { 'UserName': uname },
                url: "../../WebServer/UserLoginWebService.ashx?Method=IsLogined",
                success: function (text) {
                    if (text == "True") {
                        showLoginedInfo(uname);
                    } else {
                    }
                }
            });
        }
    }
}

function login() {
    var uname = $.trim($("#login-username").val());
    var pwd = $.trim($("#login-passwords").val());
    if (uname === "" || uname == null) {
        alert("请输入用户名");
        return false;
    }
    if (pwd === "" || pwd == null) {
        alert("请输入密码");
        return false;
    }
    $.ajax({
        type: "Post",
        dataType: "json",
        async: false,
        data: { UserName: uname, Pwd: pwd },
        url: "../../WebServer/UserLoginWebService.ashx?Method=Login",
        success: function (json) {
            if (json["status"] == true) {
                $.cookie('UserInfo', JSON.stringify(json), { path: "../" });
                hideDialog();
                showLoginedInfo(uname);
            } else {
                alert("登录失败！" + (json["msg"] === "" ? "" : json["msg"]));
            }
        }
    });
}


function login2() {
    var uname = $.trim($("#login-username").val());
    var pwd = $.trim($("#login-passwords").val());
    if (uname === "" || uname == null) {
        alert("请输入用户名");
        return false;
    }
    if (pwd === "" || pwd == null) {
        alert("请输入密码");
        return false;
    }
    $.ajax({
        type: "Post",
        dataType: "json",
        async: false,
        data: { UserName: uname, Pwd: pwd },
        url: "../../WebServer/UserLoginWebService.ashx?Method=Login",
        success: function (json) {
            if (json["status"] == true) {
                $.cookie('UserInfo', JSON.stringify(json), { path: "/" });
                hideDialog();
                showLoginedInfo(uname);
            } else {
                alert("登录失败！" + (json["msg"] === "" ? "" : json["msg"]));
            }
        }
    });
}


function regist() {
    var uname = $.trim($("#regist-username").val());
    var pwd = $.trim($("#regist-pwd").val());
    var pwdc = $.trim($("#regist-pwd-confirm").val());
    if (uname === "" || uname == null) {
        alert("请输入用户名");
        return false;
    }
    if (pwd === "" || pwd == null) {
        alert("请输入密码");
        return false;
    }
    if (pwd !== pwdc) {
        alert("两次输入的密码不一致");
        return false;
    }
    $.ajax({
        type: "Post",
        dataType: "json",
        async: false,
        data: { UserName: uname, Pwd: pwd },
        url: "../../WebServer/UserLoginWebService.ashx?Method=Regist",
        success: function (json) {
            if (json["status"] === true) {
                alert("注册成功");
                hideDialogReg();
            } else {
                var msg = json["msg"] === "" ? "注册失败" : json["msg"];
                if (msg == "用户名已存在") {
                    alert("该用户名已经在965808平台注册过，可直接登陆");
                    toLogin();
                } else {
                    alert(msg);
                }
            }
        }
    });
}

function gotoUserBackgourd() {
    window.location.href = "../Manager/Index.aspx";
}

function exitLogin() {
    $.removeCookie('UserInfo');
    hideLoginedInfo();
}

function toLogin() {
    hideDialogReg();
    showDialog();
}
function toRegist() {
    hideDialog();
    showDialogReg();
}