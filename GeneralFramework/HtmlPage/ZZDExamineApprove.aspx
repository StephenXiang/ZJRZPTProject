﻿<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="ZZDExamineApprove.aspx.cs" Inherits="GeneralFramework.HtmlPage.ZZDExamineApprove" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <title>周转贷审批</title>
    <link rel="stylesheet" type="text/css" href="../jquery-easyui-1.5.1/themes/bootstrap/easyui.css" />
    <link rel="stylesheet" type="text/css" href="../jquery-easyui-1.5.1/themes/icon.css" />
    <link rel="stylesheet" type="text/css" href="../jquery-easyui-1.5.1/demo/demo.css" />
    <script type="text/javascript" src="../jquery-easyui-1.5.1/jquery.min.js"></script>
    <script type="text/javascript" src="../jquery-easyui-1.5.1/jquery.easyui.min.js"></script>
    <script type="text/javascript" src="../jquery-easyui-1.5.1/locale/easyui-lang-zh_CN.js"></script>
    <script src="../Scripts/jquery.cookie.js"></script>
    <script type="text/javascript">
        var UserInfo;
        UserInfo = JSON.parse($.cookie('UserInfo'));
        $(function () {
            $('#DetialInfo').hide();
            $('#statusdiv').hide();
            $('#zzdiddiv').hide();
            $('#ZZDFeedBackdlg').dialog('close');
            loadzzddg();
        });

        function loadzzddg() {
            $('#ZZD_dg').datagrid({
                url: '../../WebServer/ZZDExamineApproveWebService.ashx?Method=GetZZDDataTable',
                queryParams: { UserName: UserInfo["name"] },
                rownumbers: true,
                singleSelect: true,
                striped: true,
                title: '周转贷列表',
                fitColumns: true,
                pagination: true,
                pagePosition: 'bottom',
                loadMsg: '正在加载数据...请稍后',
                pageNumber: 1,
                pageSize: 10,
                pageList: [10, 20, 30],
                columns: [[
                                    { field: 'Id', title: 'ID', width: 0, hidden: true, align: 'center' },
                                    { field: 'EnterpriseId', title: '企业ID', width: 0, hidden: true, align: 'center' },
                                    { field: 'EnterpriseName', title: '企业名称', width: 300, align: 'center' },
                                    { field: 'OriginalQuota', title: '原贷款金额(单位：万元)', width: 300, align: 'center' },
                                    { field: 'BankName', title: '原贷款银行', width: 300, align: 'center' },
                                    { field: 'ThisQuota', title: '本次贷款金额(单位：万元)', width: 300, align: 'center' },
                                    { field: 'PublishDate', title: '发布时间', width: 300, align: 'center' },
                                    {
                                        field: 'Status', title: '受理状态', width: 300, align: 'center', formatter:
                                            function (value, row, index) {
                                                if (value == 0) {
                                                    return '待受理';
                                                } else if (value == 1) {
                                                    return '受理中';
                                                } else if (value == 2) {
                                                    return '已受理';
                                                } else if (value == 3) {
                                                    return '受理成功';
                                                } else if (value == 4) {
                                                    return '受理不成功';
                                                }
                                            }
                                    }
                ]],
                onClickRow: (function (index, row) {
                    if (row.Status == 0) {
                        loadEnterpriseInfo(row.EnterpriseId);
                        loadEnterpriseFiancialInfo(row.EnterpriseId);
                        loadZZDInfo(row.Id);
                        EditZZDStatus('1', row.Id);

                    } else {
                        loadEnterpriseInfo(row.EnterpriseId);
                        loadEnterpriseFiancialInfo(row.EnterpriseId);
                        loadZZDInfo(row.Id);
                    }
                }),
                onDblClickRow: (function (index, row) {
                    if (row.Status == 1) {
                        $.messager.confirm('提示', '请确认该企业是否符合贵银行的周转贷条件！是否确认受理？', function (r) {
                            if (r) {
                                EditZZDStatus('2', row.Id);
                            } else {

                            }
                        });
                    } else if (row.Status == 2) {
                        $.messager.defaults = { ok: "是", cancel: "否" };
                        $.messager.confirm('提示', '该企业的周转贷请求是否受理成功？', function (r) {
                            if (r) {
                                EditZZDStatus('3', row.Id);
                            } else {
                                $.messager.defaults = { ok: "确定", cancel: "取消" };
                                $('#statustext').textbox('setValue', '4');
                                $('#zzdidtext').textbox("setValue", row.Id);
                                $('#ZZDFeedBackdlg').dialog('open');
                            }
                        });
                    }
                })
            }).datagrid("getPager").pagination({
                beforePageText: '第', //页数文本框前显示的汉字    
                afterPageText: '页/{pages}页',
                displayMsg: '共{total}条记录',
                onBeforeRefresh: function () {
                    return true;
                }
            });;
        }

        function EditZZDStatus(status, zzdid) {
            $.ajax({
                type: "get",
                async: true,
                url: "../../WebServer/ZZDExamineApproveWebService.ashx?Method=EditZZDStatus",
                data: { status: status, zzdid: zzdid },
                dataType: "text",
                success: function (text) {
                    if (text == "True") {
                        loadzzddg();
                    } else {
                        $.messager.alert('提示', '周转贷数据状态修改失败!', "info");
                    }
                }
            });
        }

        function EditZZDStatusAndFeedback() {
            if ($('#statustext').textbox("getValue") == "") {
                $.messager.alert('提示', '单据状态获取失败!', "info");
                $('#ZZDFeedBackdlg').dialog('close');
            }
            else if ($('#zzdidtext').textbox("getValue") == "") {
                $.messager.alert('提示', '单据编号获取失败!!', "info");
                $('#ZZDFeedBackdlg').dialog('close');
            }
            else if ($('#Feebacktext').textbox("getValue") == "") {
                $.messager.alert('提示', '拒绝原因必须填写!', "info");
            }
            else {
                $.ajax({
                    type: "get",
                    async: false,
                    url: "../../WebServer/ZZDExamineApproveWebService.ashx?Method=EditZZDStatusAndFeedback",
                    data: { status: $('#statustext').textbox("getValue"), zzdid: $('#zzdidtext').textbox("getValue"), Feedback: $('#Feebacktext').textbox("getValue") },
                    dataType: "text",
                    success: function (text) {
                        if (text == "True") {
                            $('#ZZDFeedBackdlg').dialog('close');
                            loadzzddg();
                        } else {
                            $.messager.alert('提示', '周转贷数据状态修改失败!', "info");
                        }
                    }
                });
            }

        }

        function loadEnterpriseInfo(EnterpriseId) {
            $.ajax({
                type: "Post",
                dataType: "json",
                async: true,
                data: { EnterpriseId: EnterpriseId },
                url: "../../WebServer/ZZDExamineApproveWebService.ashx?Method=GetEnterpriseInfoForEnterpriseId",
                success: function (json) {
                    var Enterprise = json;
                    if (Enterprise["DataCount"] == 0) {
                        $('#DetialInfo').hide();
                        $.messager.alert('提示', '获取企业信息失败，请重新选中企业融资数据行!', "info");
                    } else {
                        $('#DetialInfo').show();
                        var Code = "";
                        $('#labEnterpriseName').html(Enterprise["Name"]);
                        $('#labCode').html(Enterprise["Code"]);
                        $('#labRegistType').html(Enterprise["RegistType"]);
                        $('#labProfession').html(Enterprise["Profession"]);
                        $('#labEnterpriseType').html(Enterprise["EnterpriseType"]);
                        $('#labRegistRegion').html(Enterprise["RegistRegion"]);
                        $('#labHuanping').html(Enterprise["Huanping"]);
                        $('#labRegFinance').html(Enterprise["RegFinance"]);
                        $('#labRegFinanceMt').html(Enterprise["RegFinanceMt"]);
                        $('#labBusiness').html(Enterprise["Business"]);
                        $('#labMainProduction').html(Enterprise["MainProduction"]);
                        $('#labCreateTime').html(Enterprise["CreateTime"]);
                        $('#labJuridicalPerson').html(Enterprise["JuridicalPerson"]);
                        $('#labConectionPerson').html(Enterprise["ConectionPerson"]);
                        $('#labConectionTelephone').html(Enterprise["ConnectionTelephone"]);
                        $('#labDesc').html(Enterprise["Desc"]);
                        Code = Enterprise["Code"];
                        if (Code == undefined || Code == "") {
                            $('#BusinessLicenseImgTD').hide();
                        } else {
                            $('#BusinessLicenseImg').attr('href', '../../WebServer/EnterpriseService.ashx?Method=LoadBusinessLicenseImg&Code=' + Code + '');
                            $('#BusinessLicenseImgTD').show();
                        }
                    }
                }
            });
        }
        function loadEnterpriseFiancialInfo(EnterpriseId) {
            $.ajax({
                type: "get",
                async: true,
                url: '../../WebServer/ZZDExamineApproveWebService.ashx?Method=GetEnterpriseFinanceInfoByEnterpriseId',
                data: { EnterpriseId: EnterpriseId },
                dataType: "json",
                success: function (json) {
                    EnterpriseFiancia = json;
                    if (EnterpriseFiancia["DataCount"] == 0) {
                        $('#DetialInfo').hide();
                        $.messager.alert('提示', '获取企业信息失败，请重新选中企业融资数据行!', "info");
                    } else {
                        $('#DetialInfo').show();
                        var thisyearMonths = EnterpriseFiancia["thisyear"].split('-');
                        $('#llYear').html(EnterpriseFiancia["llyear"] + "年");
                        $('#lYear').html(EnterpriseFiancia["lyear"] + "年");
                        $('#thisYearMonth').html(thisyearMonths[0] + "年" + thisyearMonths[1] + "月");
                        $('#llzczeLab').html(EnterpriseFiancia["llzcze"] + "&nbsp万元");
                        $('#llfzzeLab').html(EnterpriseFiancia["llfzze"] + "&nbsp万元");
                        $('#llsyzqyLab').html(EnterpriseFiancia["llsyzqy"] + "&nbsp万元");
                        $('#llxssrLab').html(EnterpriseFiancia["llxssr"] + "&nbsp万元");
                        $('#llyszkLab').html(EnterpriseFiancia["llyszk"] + "&nbsp万元");
                        $('#lljlrLab').html(EnterpriseFiancia["lljlr"] + "&nbsp万元");
                        $('#lzczeLab').html(EnterpriseFiancia["lzcze"] + "&nbsp万元");
                        $('#lfzzeLab').html(EnterpriseFiancia["lzcze"] + "&nbsp万元");
                        $('#lsyzqyLab').html(EnterpriseFiancia["lsyzqy"] + "&nbsp万元");
                        $('#lxssrLab').html(EnterpriseFiancia["lxssr"] + "&nbsp万元");
                        $('#lyszkLab').html(EnterpriseFiancia["lyszk"] + "&nbsp万元");
                        $('#ljlrLab').html(EnterpriseFiancia["ljlr"] + "&nbsp万元");
                        $('#thiszczeLab').html(EnterpriseFiancia["thiszcze"] + "&nbsp万元");
                        $('#thisfzzeLab').html(EnterpriseFiancia["thisfzze"] + "&nbsp万元");
                        $('#thissyzqyLab').html(EnterpriseFiancia["thissyzqy"] + "&nbsp万元");
                        $('#thisxssrLab').html(EnterpriseFiancia["thisxssr"] + "&nbsp万元");
                        $('#thisyszkLab').html(EnterpriseFiancia["thisyszk"] + "&nbsp万元");
                        $('#thisjlrLab').html(EnterpriseFiancia["thisjlr"] + "&nbsp万元");
                    }
                },
                error: function (json) {
                    if (UserInfo["role"] == 4) {
                        $('#EnterpriseFinanceInfoBtn').show();
                    }
                }
            });
        }

        function loadZZDInfo(zzdid) {
            $.ajax({
                type: "get",
                async: true,
                url: '../../WebServer/ZZDExamineApproveWebService.ashx?Method=GetZZDInfoByZZDID',
                data: { ZZDID: zzdid },
                dataType: "json",
                success: function (json) {
                    EnterpriseZZDInfo = json;
                    if (EnterpriseZZDInfo["DataCount"] == 0) {
                        $('#DetialInfo').hide();
                        $.messager.alert('提示', '获取企业信息失败，请重新选中企业融资数据行!', "info");
                    } else {
                        $('#DetialInfo').show();
                        $('#labydkyh').html(EnterpriseZZDInfo["ydkyh"]);
                        $('#labkhjlxm').html(EnterpriseZZDInfo["khjlxm"]);
                        $('#labkhjllxdh').html(EnterpriseZZDInfo["khjllxdh"]);
                        $('#labydkje').html(EnterpriseZZDInfo["ydkje"] + "&nbsp万元");
                        $('#labdkdqsj').html(EnterpriseZZDInfo["dkdqsj"]);
                        $('#labzbh').html(EnterpriseZZDInfo["zbh"]);
                        $('#labbcdkje').html(EnterpriseZZDInfo["bcdkje"] + "&nbsp万元");
                    }
                },
                error: function (json) {
                    if (UserInfo["role"] == 4) {
                        $('#EnterpriseFinanceInfoBtn').show();
                    }
                }
            });
        }
    </script>
</head>
<body>
    <body class="easyui-layout">
        <div data-options="region:'center'" style="height: 100%; width: 100%; border: 0px solid red;">
            <div>
                <table id="ZZD_dg" data-options="
                iconCls: 'icon-save',
                buttons: '#RZ_dg-buttons'
            ">
                </table>
            </div>
            <div id="DetialInfo" class="easyui-panel" title="详细信息" style="width: 100%; height: 420px; padding: 10px;">
                <div class="easyui-layout" data-options="fit:true">
                    <div data-options="region:'west',split:false" style="width: 35%; padding: 10px;">
                        <h2>企业信息</h2>
                        <div class="easyui-layout" data-options="fit:false" style="height: 280px; width: 98%; padding-bottom: 5px;">
                            <div data-options="region:'center'" style="width: 33%; padding: 10px">

                                <table border="0" style="width: 100%; border-collapse: separate; border-spacing: 0px 6px;">
                                    <tr>
                                        <td style="text-align: left;">企业名称:</td>
                                        <td style="text-align: left;">
                                            <label id="labEnterpriseName" />
                                            无</td>
                                        <td style="text-align: left;">信用代码:</td>
                                        <td style="text-align: left;">
                                            <label id="labCode" />
                                            无</td>
                                    </tr>
                                    <tr>
                                        <td style="text-align: left;">企业类型:</td>
                                        <td style="text-align: left;">
                                            <label id="labEnterpriseType" />
                                            无</td>
                                        <td style="text-align: left;">行业分类:</td>
                                        <td style="text-align: left;">
                                            <label id="labProfession" />
                                            无</td>
                                    </tr>
                                    <tr>
                                        <td style="text-align: left;">主要产品:</td>
                                        <td style="text-align: left;">
                                            <label id="labMainProduction" />
                                            无</td>
                                        <td style="text-align: left;">环评结果:</td>
                                        <td style="text-align: left;">
                                            <label id="labHuanping" />
                                            无</td>

                                    </tr>
                                    <tr>
                                        <td style="text-align: left;">注册类型:</td>
                                        <td style="text-align: left;">
                                            <label id="labRegistType" />
                                            无</td>
                                        <td style="text-align: left;">注册地区:</td>
                                        <td style="text-align: left;">
                                            <label id="labRegistRegion" />
                                            无</td>
                                    </tr>
                                    <tr>
                                        <td style="text-align: left;">注册资金:</td>
                                        <td style="text-align: left;">
                                            <label id="labRegFinance" />
                                            无</td>
                                        <td style="text-align: left;">注册币种:</td>
                                        <td style="text-align: left;">
                                            <label id="labRegFinanceMt" />
                                            无</td>
                                    </tr>
                                    <tr>

                                        <td style="text-align: left;">营业范围:</td>
                                        <td style="text-align: left;">
                                            <label id="labBusiness" />
                                            无</td>
                                        <td style="text-align: left;">成立时间:</td>
                                        <td style="text-align: left;">
                                            <label id="labCreateTime" />
                                            无</td>
                                    </tr>
                                    <tr>
                                        <td style="text-align: left;">法人代表:</td>
                                        <td style="text-align: left;">
                                            <label id="labJuridicalPerson" />
                                            无</td>
                                        <td style="text-align: left;">营业执照:</td>
                                        <td id="BusinessLicenseImgTD" style="text-align: left;" colspan="5">
                                            <a id="BusinessLicenseImg" name="BusinessLicenseImg" target="_blank" href="">点击查看</a>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td style="text-align: left;">联系人姓名:</td>
                                        <td style="text-align: left;">
                                            <label id="labConectionPerson" />
                                            无</td>
                                        <td style="text-align: left;">联系人电话:</td>
                                        <td style="text-align: left;">
                                            <label id="labConectionTelephone" />
                                            无</td>
                                    </tr>
                                    <tr>
                                        <td style="text-align: left;">企业简介:</td>
                                        <td style="text-align: left;" colspan="5">
                                            <label id="labDesc" />
                                            无</td>

                                    </tr>
                                    <tr></tr>


                                </table>
                            </div>
                        </div>
                    </div>
                    <div data-options="region:'center'" style="padding: 10px">
                        <h2>财务信息</h2>
                        <div class="easyui-layout" data-options="fit:false" style="height: 280px; width: 99%; padding-bottom: 5px;">
                            <div data-options="region:'west'" style="width: 33%; padding: 10px">
                                <h3>
                                    <label id="llYear">无</label></h3>
                                <table border="0" style="width: 100%; border-collapse: separate; border-spacing: 0px 6px;">
                                    <tr>
                                        <td style="text-align: right;">资产总额:</td>
                                        <td style="text-align: right;">
                                            <label id="llzczeLab">无</label></td>
                                    </tr>
                                    <tr>
                                        <td style="text-align: right;">负债总额:</td>
                                        <td style="text-align: right;">
                                            <label id="llfzzeLab">无</label></td>
                                    </tr>
                                    <tr>
                                        <td style="text-align: right;">销售收入:</td>
                                        <td style="text-align: right;">
                                            <label id="llxssrLab">无</label></td>
                                    </tr>
                                    <tr>
                                        <td style="text-align: right;">应收账款:</td>
                                        <td style="text-align: right;">
                                            <label id="llyszkLab">无</label></td>
                                    </tr>
                                    <tr>
                                        <td style="text-align: right;">净利润:</td>
                                        <td style="text-align: right;">
                                            <label id="lljlrLab">无</label></td>
                                    </tr>
                                    <tr>
                                        <td style="text-align: right;">所有者权益:</td>
                                        <td style="text-align: right;">
                                            <label id="llsyzqyLab">无</label></td>
                                    </tr>

                                </table>
                            </div>
                            <div data-options="region:'east'" style="width: 33%; padding: 10px">
                                <h3>
                                    <label id="thisYearMonth">无</label></h3>
                                <table border="0" style="width: 100%; border-collapse: separate; border-spacing: 0px 6px;">
                                    <tr>
                                        <td style="text-align: right;">资产总额:</td>
                                        <td style="text-align: right;">
                                            <label id="lzczeLab">无</label></td>
                                    </tr>
                                    <tr>
                                        <td style="text-align: right;">负债总额:</td>
                                        <td style="text-align: right;">
                                            <label id="lfzzeLab">无</label></td>
                                    </tr>
                                    <tr>
                                        <td style="text-align: right;">销售收入:</td>
                                        <td style="text-align: right;">
                                            <label id="lxssrLab">无</label></td>
                                    </tr>
                                    <tr>
                                        <td style="text-align: right;">应收账款:</td>
                                        <td style="text-align: right;">
                                            <label id="lyszkLab">无</label></td>
                                    </tr>
                                    <tr>
                                        <td style="text-align: right;">净利润:</td>
                                        <td style="text-align: right;">
                                            <label id="ljlrLab">无</label></td>
                                    </tr>
                                    <tr>
                                        <td style="text-align: right;">所有者权益:</td>
                                        <td style="text-align: right;">
                                            <label id="lsyzqyLab">无</label></td>
                                    </tr>

                                </table>
                            </div>
                            <div data-options="region:'center'" style="width: 33%; padding: 10px">
                                <h3>
                                    <label id="lYear">无</label></h3>
                                <table border="0" style="width: 100%; border-collapse: separate; border-spacing: 0px 6px;">
                                    <tr>
                                        <td style="text-align: right;">资产总额:</td>
                                        <td style="text-align: right;">
                                            <label id="thiszczeLab">无</label></td>
                                    </tr>
                                    <tr>
                                        <td style="text-align: right;">负债总额:</td>
                                        <td style="text-align: right;">
                                            <label id="thisfzzeLab">无</label></td>
                                    </tr>
                                    <tr>
                                        <td style="text-align: right;">销售收入:</td>
                                        <td style="text-align: right;">
                                            <label id="thisxssrLab">无</label></td>
                                    </tr>
                                    <tr>
                                        <td style="text-align: right;">应收账款:</td>
                                        <td style="text-align: right;">
                                            <label id="thisyszkLab">无</label></td>
                                    </tr>
                                    <tr>
                                        <td style="text-align: right;">净利润:</td>
                                        <td style="text-align: right;">
                                            <label id="thisjlrLab">无</label></td>
                                    </tr>
                                    <tr>
                                        <td style="text-align: right;">所有者权益:</td>
                                        <td style="text-align: right;">
                                            <label id="thissyzqyLab">无</label></td>
                                    </tr>

                                </table>
                            </div>
                        </div>

                    </div>
                    <div data-options="region:'east'" style="width: 30%; padding: 10px">
                        <h2>周转贷需求</h2>
                        <div class="easyui-layout" data-options="fit:false" style="height: 280px; width: 98%;">
                            <div data-options="region:'center'" style="width: 33%; padding: 10px">



                                <table border="0" style="width: 100%; border-collapse: separate; border-spacing: 0px 6px;">
                                    <tr>
                                        <td style="text-align: left;">原贷款银行:</td>
                                        <td style="text-align: left;">
                                            <label id="labydkyh" />
                                            无</td>
                                    </tr>
                                    <tr>
                                        <td style="text-align: left;">客户经理姓名:</td>
                                        <td style="text-align: left;">
                                            <label id="labkhjlxm" />
                                            无</td>
                                    </tr>
                                    <tr>
                                        <td style="text-align: left;">客户经理联系电话:</td>
                                        <td style="text-align: left;">
                                            <label id="labkhjllxdh" />
                                            无</td>
                                    </tr>

                                    <tr>
                                        <td style="text-align: left;">原贷款金额:</td>
                                        <td style="text-align: left;">
                                            <label id="labydkje" />
                                            无</td>
                                    </tr>
                                    <tr>
                                        <td style="text-align: left;">贷款到期时间:</td>
                                        <td style="text-align: left;">
                                            <label id="labdkdqsj" />
                                            无</td>
                                    </tr>
                                    <tr>
                                        <td style="text-align: left;">主办行(经办行):</td>
                                        <td style="text-align: left;">
                                            <label id="labzbh" />
                                            无</td>
                                    </tr>
                                    <tr>
                                        <td style="text-align: left;">本次贷款金额:</td>
                                        <td style="text-align: left;">
                                            <label id="labbcdkje" />
                                            无</td>
                                    </tr>
                                </table>
                            </div>
                        </div>

                    </div>
                </div>
            </div>
        </div>
        <div id="ZZDFeedBackdlg" class="easyui-dialog" title="拒绝受理原因" style="text-align: center; width: 500px; height: 200px; padding: 10px"
            data-options="
                iconCls: 'icon-save',
                buttons: '#ZZDFeedBackdlg-buttons'
            ">
            <div id="statusdiv" style="margin-bottom: 20px">
                <input class="easyui-textbox" id="statustext" name="statustext" style="width: 80%; height: 80px;" data-options="label:'状态:'">
            </div>
            <div id="zzdiddiv" style="margin-bottom: 20px">
                <input class="easyui-textbox" id="zzdidtext" name="zzdidtext" style="width: 80%; height: 80px;" data-options="label:'周转贷ID:'">
            </div>
            <div style="margin-bottom: 20px">
                <input class="easyui-textbox" id="Feebacktext" name="Feebacktext" style="width: 80%; height: 80px;" data-options="label:'拒绝原因:',multiline:true">
            </div>
        </div>
        <div id="ZZDFeedBackdlg-buttons">
            <a href="javascript:void(0)" class="easyui-linkbutton" onclick="EditZZDStatusAndFeedback()">保存</a>
            <a href="javascript:void(0)" class="easyui-linkbutton" onclick="javascript: $('#ZZDFeedBackdlg').dialog('close')">关闭
            </a>
        </div>
    </body>
</body>
</html>
