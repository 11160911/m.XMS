(function ($) {
    //"use strict";
    var chkStr = "";
    var tmpRealData = "";
    var blLoginchk;
    var tmpIPdata = "";  //測試IP(假的)
    var SysDate = "";
    var Initdoc = function () {
        blLoginchk = true;
        $('#btnChkOK').click(function () { btnChkOK_click(); });
        $('#SystemSetup').click(function () { SystemSetup_click(); });
        $('#SebtSave').click(function () { SebtSave_click(); });
        $('#SebtCancel').click(function () { SebtCancel_click(); });
        $('#btSave').click(function () { btSave_click(); });
        $('#btCancel').click(function () { btCancel_click(); });

        $('#CompanyName').val();

        LoginCompany();

        BeforeInit();
        
        $('#btSignIn').click(function () {
            $('#icrpwd').hide();
            $('#chklogin').hide();
            $('#doublelogin').hide();
            LoginSys();
        });

        $('#password').keypress(function (e) {
            $('#icrpwd').hide();
            $('#chklogin').hide();
            $('#doublelogin').hide();
            if (e.which == 13) {
                LoginSys();
            }
        });
    };

    let btnChkOK_click = function () {
        window.location.href = "Login" + sessionStorage.getItem('isamcomp');
    };

    let SystemSetup_click = function () {
        $('.msg-valid').hide();
        $('#modal_SeSetting .modal-title').text('系統設定');
        $('#modal_SeSetting .btn-danger').text('確定');
        $('#SeID').val('');
        $('#SePWD').val('');
        $('#modal_SeSetting').modal('show');
    }

    let SebtSave_click = function () {
        if (($('#SeID').val().toLowerCase() == "admin" && $('#SePWD').val() == "Yewtek") | ($('#SeID').val().toLowerCase() == "isamweb" && $('#SePWD').val() == "IsamWeb01")) {
            $('#modal_SeSetting').modal('hide');
            btnSetCookie_click();
        } else {
            DyAlert("帳號或密碼錯誤，\n請重新輸入!!");
            return;
        }
    }

    let btnSetCookie_click = function () {
        $('.msg-valid').hide();
        $('#modal_SetKey .modal-title').text('系統設定');
        $('#modal_SetKey .btn-danger').text('確定');

        $('#oldCookie').val('');
        $('#newCookie').val('');
        var oldkey = localStorage.getItem('MCkey');
        if (oldkey != null) {
            $('#oldCookie').val(oldkey);
        }

        $('#modal_SetKey').modal('show');
    };

    let SebtCancel_click = function () {
        $('#modal_SeSetting').modal('hide');
    }

    let btSave_click = function () {
        var blCookieExist = false;
        if ($('#oldCookie').val() != "") {
            blCookieExist = true;
        }
        if (!blCookieExist) {
            if ($('#newCookie').val() == "" | $('#newCookie').val() == null) {
                DyAlert("新金鑰欄位必須輸入資料!!", function () { $('#newCookie').focus() });
                return;
            }
            localStorage.setItem("MCkey", $('#newCookie').val());
        } else {
            if ($('#newCookie').val() == "" | $('#newCookie').val() == null) {
                DyAlert("新金鑰欄位必須輸入資料!!", function () { $('#newCookie').focus() });
                return;
            }
            localStorage.removeItem('MCkey');
            localStorage.setItem("MCkey", $('#newCookie').val());
        }

        var cData = {
            keyData: localStorage.getItem('MCkey'),
            CompanyID: $('#CompanyID').val()
        }
        PostToWebApi({ url: "api/ChkkeyData", data: cData, success: AfterChkkeyData });
    }

    var AfterChkkeyData = function (data) {
        if (ReturnMsg(data, 0) != "ChkkeyDataOK") {
            DyAlert(ReturnMsg(data, 1), function () {
                localStorage.removeItem('MCkey');
                $('#modal_SetKey').modal('hide');
                BeforeInit();
            });
        }
        else {
            var dtCkno1 = data.getElementsByTagName('dtCkno1');
            DyAlert("金鑰設定完成!");
            $('#modal_SetKey').modal('hide');
            BeforeInit();
        }
    }

    let btCancel_click = function () {
        $('#modal_SetKey').modal('hide');
        BeforeInit();
    };

    var BeforeInit = function () {
        $('#checklist').text('');
        chkStr = "盤點系統啟動中…………\n\n"
        $('#checklist').text(chkStr);

        tmpRealData = localStorage.getItem('MCkey');
        if (tmpRealData == null || tmpRealData == "") {
            chkStr = "盤點系統啟動中…………\n\n金鑰錯誤，無法登入!";
            $('#checklist').text(chkStr);
            $('#btnChkOK').hide();
            if ($('#chkerrbtndiv').attr('hidden') == undefined) {
                $('#chkerrbtndiv').show();
            }
            else {
                $('#chkerrbtndiv').removeAttr('hidden');
            }
        } else {
            var cData = {
                keyData: tmpRealData,
                CompanyID: $('#CompanyID').val()
            }
            PostToWebApi({ url: "api/GetkeyData", data: cData, success: AfterGetkeyData });
        }
    }

    var AfterGetkeyData = function (data) {
        if (data == null) { blLoginchk = false; }
        if (blLoginchk) {
            if (ReturnMsg(data, 0) != "GetkeyDataOK") {
                DyAlert(ReturnMsg(data, 1));
                blLoginchk = false;
            }
            else {
                var dtkey = data.getElementsByTagName('dtkey');
                var dtCk = data.getElementsByTagName('dtCkno');
                //var dtWh = data.getElementsByTagName('dtWh');
                if (dtkey.length == 0 || dtCk.length == 0) {
                    blLoginchk = false;
                } else {
                    if (GetNodeValue(dtCk[0], 'KeyData') != GetNodeValue(dtkey[0], 'Realkey')) {
                        blLoginchk = false;
                    } else {
                        //$('#Shop').html(GetNodeValue(dtWh[0], 'ShopName'));
                        //$('#ckno').html(GetNodeValue(dtWh[0], 'ckno'));
                        //sessionStorage.setItem("ST_SName", GetNodeValue(dtWh[0], "ST_SName"));
                        //$.getJSON("https://api.ipify.org?format=jsonp&callback=?",
                        //    function (json) {
                        //        tmpIPdata = json.ip;
                        //    }
                        //);
                        SysDate = GetNodeValue(dtkey[0], 'SysDate');
                    }
                }
            }
        }

        if (!blLoginchk) {
            chkStr = "盤點系統啟動中…………\n\n金鑰錯誤，無法登入!";
            $('#checklist').text(chkStr);
            $('#btnChkOK').hide();
            if ($('#chkerrbtndiv').attr('hidden') == undefined) {
                $('#chkerrbtndiv').show();
            }
            else {
                $('#chkerrbtndiv').removeAttr('hidden');
            }
        } else {
            Checkmpckno(dtCk);
        }
        //setTimeout(function () {
        //    if (!blLoginchk) {
                
        //        chkStr = "盤點系統啟動中…………\n\n金鑰錯誤，無法登入!";
        //        $('#checklist').text(chkStr);
        //        $('#btnChkOK').hide();
        //        if ($('#chkerrbtndiv').attr('hidden') == undefined) {
        //            $('#chkerrbtndiv').show();
        //        }
        //        else {
        //            $('#chkerrbtndiv').removeAttr('hidden');
        //        }
        //    } else {
        //        Checkmpckno(dtCk);
        //    }
        //}, 3500);
    }

    var Checkmpckno = function (data) {
        //if (GetNodeValue(data[0], 'IP') != tmpIPdata) {
        //    blLoginchk = false;
        //    chkStr = "盤點系統啟動中…………\n\nIP錯誤，無法登入!";
        //}
        if (blLoginchk) {
            if (GetNodeValue(data[0], 'Status') != "1") {
                blLoginchk = false;
                chkStr = "盤點系統啟動中…………\n\n設備狀態錯誤(已停用)，無法登入!";
            } else {
                if (GetNodeValue(data[0], 'SDate') == "") {
                    blLoginchk = false;
                } else {
                    if (GetNodeValue(data[0], 'SDate') > SysDate) {
                        //alert("sysdate:" + SysDate);
                        blLoginchk = false;
                    } else {
                        if (GetNodeValue(data[0], 'EDate') != "" && GetNodeValue(data[0], 'EDate') <= SysDate) {
                            blLoginchk = false;
                        } else {
                            chkStr = "盤點系統啟動中…………\n\n登入盤點系統檢查完成………";
                            $('#checklist').text(chkStr);
                            if ($('#frmLogin').attr('hidden') == undefined) {
                                $('#frmLogin').show();
                            }
                            else {
                                $('#frmLogin').removeAttr('hidden');
                            }
                            $('#frmChkBeforeLogin').hide();

                            //var cData = {
                            //    shop: $('#Shop').html().split(' ')[0],
                            //    ckno: $('#ckno').html()
                            //}
                            //PostToWebApi({ url: "api/GetmpLoginRec", data: cData, success: AfterGetmpLoginRec });

                        }
                    }
                }
                if (!blLoginchk) chkStr = "盤點系統啟動中…………\n\n設備已結束使用，無法登入!";
            }

        }
        if (!blLoginchk) {
            $('#checklist').text(chkStr);
            $('#btnChkOK').hide();
            if ($('#chkerrbtndiv').attr('hidden') == undefined) {
                $('#chkerrbtndiv').show();
            }
            else {
                $('#chkerrbtndiv').removeAttr('hidden');
            }
        }
    }

    var LoginCompany = function () {
        var pData = {
            CompanyID: $('#CompanyID').val(),
        };
        PostToWebApi({ url: "api/GetCompanyName", data: pData, success: GetCompanySuccess, error: LoginError });
    }

    var GetCompanySuccess = function (data) {
        if (ReturnMsg(data, 0) == "GetCompanyNameOK") {
            var dtC = data.getElementsByTagName('dtCompanyName');
            $('#CompanyName').text(GetNodeValue(dtC[0], 'ChineseName'));
        }
    }

    var LoginSys = function () {
        var pData = {
            USERID: $('#username').val(),
            PASSWORD: $('#password').val(),
            CompanyID: $('#CompanyID').val(),
        };
        PostToWebApi({ url: "api/LoginSys", data: pData, success: LoginSuccess, error: LoginError });
    };

    var LoginSuccess = function (data) {
        if (ReturnMsg(data, 0) == "LoginSysOK") {
            var dtE = data.getElementsByTagName('dtEmployee');
            var dtL = data.getElementsByTagName('dtLogin');
            var token = GetNodeValue(dtE[0], "token");
            var companyid = window.location.search;
            var logindt = GetNodeValue(dtL[0], "LoginDT");
            sessionStorage.setItem("token", token);
            sessionStorage.setItem("isamcomp", companyid);
            sessionStorage.setItem("LoginDT", logindt)
            window.location.replace("menu");
        }
        else if (ReturnMsg(data, 1) == "密碼錯誤") {
            //alert("密碼錯誤 : " + ReturnMsg(data,0))
            $('#icrpwd').show();
        }
        else if (ReturnMsg(data, 1) == "重複登入") {
            $('#chklogin').show();
        }
        else if (ReturnMsg(data, 1) == "二次登入") {
            $('#doublelogin').show();
        }
        else if (ReturnMsg(data, 0) == "Exception") {
            DyAlert(ReturnMsg(data, 1));
        }
        else {
            DyAlert(ReturnMsg(data, 1));
        }
    };

    var LoginError = function (data) {
        DyAlert(ReturnMsg(data, 1));
    };
    Initdoc();

})(jQuery);



