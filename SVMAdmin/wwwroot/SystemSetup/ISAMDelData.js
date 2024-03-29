﻿var PageISAMDelData = function (ParentNode) {

    let EditMode = "";
    let DelType = "";

    let ChkLogOut_1 = function (AfterChkLogOut_1) {
        var LoginDT = sessionStorage.getItem('LoginDT');
        var cData = {
            LoginDT: LoginDT
        }
        PostToWebApi({ url: "api/js/ChkLogOut", data: cData, success: AfterChkLogOut_1 });
    };

    let AfterDelISAMData = function (data) {
        if (ReturnMsg(data, 0) != "DelISAMDataOK") {
            DyAlert(ReturnMsg(data, 1));
        }
        else {
            DyAlert("資料清除完成!");
            return;
        }
    }

    let CallDelISAMData = function () {
        ChkLogOut_1(CallDelISAMData_1);
    };

    let CallDelISAMData_1 = function (data) {
        if (ReturnMsg(data, 0) != "ChkLogOutOK") {
            DyAlert(ReturnMsg(data, 1));
        }
        else {
            var dtLogin = data.getElementsByTagName('dtLogin');
            if (GetNodeValue(dtLogin[0], "LogOutType") != "") {
                window.location.href = "Login" + sessionStorage.getItem('isamcomp');
            }
            else {
                Timerset(sessionStorage.getItem('isamcomp'));
                var cData = {
                    Type: EditMode,
                    Shop: $('#lblShop2').html().split(' ')[0],
                    DelType: DelType
                }
                PostToWebApi({ url: "api/SystemSetup/DelISAMData", data: cData, success: AfterDelISAMData });
            }
        }
    };

    let btAll_click = function () {
        ChkLogOut(sessionStorage.getItem('isamcomp'));
        Timerset(sessionStorage.getItem('isamcomp'));
        EditMode = "T,C,D";
        DelType = "";
        DyConfirm("確認要清除全部資料？  一但確認，將清除本作業店櫃所有盤點、條碼蒐集、出貨/調撥資料!!", CallDelISAMData, DummyFunction);
    }

    let btDelivery_click = function () {
        ChkLogOut(sessionStorage.getItem('isamcomp'));
        Timerset(sessionStorage.getItem('isamcomp'));
        EditMode = "D";
        DelType = "";
        DyConfirm("確認要清除出貨/調撥資料？  一但確認，將清除本作業店櫃所有出貨/調撥資料!!", CallDelISAMData, DummyFunction);
    }

    let btDelivery_E_click = function () {
        ChkLogOut(sessionStorage.getItem('isamcomp'));
        Timerset(sessionStorage.getItem('isamcomp'));
        EditMode = "D";
        DelType = "E";
        DyConfirm("確認要清除出貨/調撥資料？  一但確認，將清除本作業店櫃人員所有出貨/調撥資料!!", CallDelISAMData, DummyFunction);
    }

    let btCollect_click = function () {
        ChkLogOut(sessionStorage.getItem('isamcomp'));
        Timerset(sessionStorage.getItem('isamcomp'));
        EditMode = "C";
        DelType = "";
        DyConfirm("確認要清除條碼蒐集資料？  一但確認，將清除本作業店櫃所有條碼蒐集資料!!", CallDelISAMData, DummyFunction);
    }

    let btCollect_E_click = function () {
        ChkLogOut(sessionStorage.getItem('isamcomp'));
        Timerset(sessionStorage.getItem('isamcomp'));
        EditMode = "C";
        DelType = "E";
        DyConfirm("確認要清除條碼蒐集資料？  一但確認，將清除本作業店櫃人員所有條碼蒐集資料!!", CallDelISAMData, DummyFunction);
    }

    let btBIN_click = function () {
        ChkLogOut(sessionStorage.getItem('isamcomp'));
        Timerset(sessionStorage.getItem('isamcomp'));
        EditMode = "T";
        DelType = "";
        DyConfirm("確認要清除盤點資料？  一但確認，將清除本作業店櫃所有盤點資料!!", CallDelISAMData, DummyFunction);
    }

    let btBIN_E_click = function () {
        ChkLogOut(sessionStorage.getItem('isamcomp'));
        Timerset(sessionStorage.getItem('isamcomp'));
        EditMode = "T";
        DelType = "E";
        DyConfirm("確認要清除盤點資料？  一但確認，將清除本作業店櫃人員所有盤點資料!!", CallDelISAMData, DummyFunction);
    }


    //#region FormLoad
    let afterGetInitISAMDelData = function (data) {
        //AssignVar();
        EditMode = "Q";
        //tbDetail = $('#pgISAM02Mod #tbISAM02Mod tbody');

        $('#lblShop2').html(GetNodeValue(data[0], "STName"));
        if ($('#pgISAMDelData').attr('hidden') == undefined) {
            $('#pgISAMDelData').show();
        }
        else {
            $('#pgISAMDelData').removeAttr('hidden');
        }

        $('#btBIN').click(function () { btBIN_click(); });
        $('#btCollect').click(function () { btCollect_click(); });
        $('#btDelivery').click(function () { btDelivery_click(); });

        $('#btBIN_E').click(function () { btBIN_E_click(); });
        $('#btCollect_E').click(function () { btCollect_E_click(); });
        $('#btDelivery_E').click(function () { btDelivery_E_click(); });
        $('#btAll').click(function () { btAll_click(); });

    };


    let AfterGetWhName = function (data) {
        if (ReturnMsg(data, 0) != "GetWhNameOK") {
            DyAlert(ReturnMsg(data, 1));
        }
        else {
            var dtWh = data.getElementsByTagName('dtWh');
            //alert(GetNodeValue(dtWh[0], "ST_ID"));
            if (GetNodeValue(dtWh[0], "STName") == "") {
                DyAlert("請確認店櫃(" + GetNodeValue(dtWh[0], "WhNo") + ")是否為允許作業之店櫃!", DummyFunction);
                return;
            }
            afterGetInitISAMDelData(dtWh);
        }
    };

    let afterGetPageInitBefore = function (data) {
        if (ReturnMsg(data, 0) != "GetPageInitBeforeOK") {
            DyAlert(ReturnMsg(data, 1));
        }
        else {
            var dtISAMWh = data.getElementsByTagName('dtComp');
            //alert(GetNodeValue(dtISAMWh[0], "WhNo") );
            if (dtISAMWh.length == 0) {
                DyAlert("無符合資料!", DummyFunction);
                return;
            }
            else if (GetNodeValue(dtISAMWh[0], "WhNo") == null | GetNodeValue(dtISAMWh[0], "WhNo") == "") {
                DyAlert("請先至店號設定進行作業店櫃設定!", DummyFunction);
                return;
            }
            else if (GetNodeValue(dtISAMWh[0], "WhNo") != "") {
                PostToWebApi({ url: "api/SystemSetup/GetWhName", success: AfterGetWhName });
            }

        }
    };


    let afterLoadPage = function () {
        PostToWebApi({ url: "api/SystemSetup/GetPageInitBefore", success: afterGetPageInitBefore });
    };
//#endregion

    if ($('#pgISAMDelData').length == 0) {
        AllPages = new LoadAllPages(ParentNode, "SystemSetup/ISAMDelData", ["pgISAMDelData"], afterLoadPage);
    };
}