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
        $('#SebtCancel').click(function () { SebtCancel_click(); });
        $('#SebtSave').click(function () { SebtSave_click(); });
        $('#btSave').click(function () { btSave_click(); });
        $('#btCancel').click(function () { btCancel_click(); });

        $('#btSubmitOTP').click(function () { SendOTP(); });
        $('#txtOTP').keypress(function (e) {
            if (e.which == 13) {
                SendOTP();
            }
        });

        $('#CompanyName').val();
        LoginCompany();

        $('#btSignIn').click(function () {
            $('#icrpwd').hide();
            $('#locklogin').hide();
            $('#expiredlogin').hide();
            $('#upwdlogin').hide();
            LoginSys();
        });

        $('#password').keypress(function (e) {
            $('#icrpwd').hide();
            $('#locklogin').hide();
            $('#expiredlogin').hide();
            $('#upwdlogin').hide();
            if (e.which == 13) {
                LoginSys();
            }
        });

    };

    //另一種取設備ID方法
    //var getdeviceID = function () {
    //    let UserAgent = navigator.userAgent;
    //    let ScreenResolution = window.screen.width + 'x' + window.screen.height;
    //    let timezoneOffset = new Date().getTimezoneOffset();
    //    let uniqueId = UserAgent + ScreenResolution + timezoneOffset;
    //    return uniqueId;
    //}

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
        $('#username').val($('#username').val().trim());

        var pData = {
            USERID: $('#username').val(),
            PASSWORD: $('#password').val(),
            CompanyID: $('#CompanyID').val(),
        };
        PostToWebApi({ url: "api/LoginSys", data: pData, success: LoginSuccess, error: LoginError });
    };

    var LoginSuccess = function (data) {
        if (ReturnMsg(data, 0) == "LoginSysOK") {
            let user = data.getElementsByTagName('dtEmployee')[0];
            $('#CompanyID').val(GetNodeValue(user, 'CompanyCode'))
            CheckOTP(data);
            //var dtE = data.getElementsByTagName('dtEmployee');
            //var token = GetNodeValue(dtE[0], "token");
            //var companyid = window.location.search;
            //sessionStorage.setItem("token", token);
            //sessionStorage.setItem("isamcomp", companyid);
            //window.location.replace("menu");
        }
        else if (ReturnMsg(data, 1) == "帳號錯誤") {
            $('#icrpwd').show();
        }
        else if (ReturnMsg(data, 1) == "帳號鎖定") {
            $('#locklogin').show();
        }
        else if (ReturnMsg(data, 1) == "帳號失效") {
            $('#expiredlogin').show();
        }
        else if (ReturnMsg(data, 1) == "密碼錯誤") {
            $('#upwdlogin').show();
        }
        else if (ReturnMsg(data, 0) == "Exception") {
            DyAlert(ReturnMsg(data, 1));
        }
        else {
            DyAlert(ReturnMsg(data, 1));
        }
    };

    let CheckOTP = function (data) {
        let user = data.getElementsByTagName('dtEmployee')[0];
        //增加判斷若登入者為系統管理員，則跳過OTP驗證
        if (GetNodeValue(user, 'UID').toLowerCase() == GetNodeValue(user, 'CompanyCode').toLowerCase()) {
            var pData = {
                USERID: $('#username').val(),
                PASSWORD: $('#password').val(),
                CompanyID: $('#CompanyID').val(),
                GID: localStorage.getItem('GID')
            };
            PostToWebApi({ url: "api/SendOTP_EDDMS", data: pData, success: afterSendOTP_EDDMS, error: LoginError });
        }
        //20240626 增加判斷若登入者為圖片設計廠商，則跳過OTP驗證
        else if (GetNodeValue(user, 'UID').toLowerCase() == "amber") {
            var pData = {
                USERID: $('#username').val(),
                PASSWORD: $('#password').val(),
                CompanyID: $('#CompanyID').val(),
                GID: localStorage.getItem('GID')
            };
            PostToWebApi({ url: "api/SendOTP_EDDMS", data: pData, success: afterSendOTP_EDDMS, error: LoginError });
        }
        else {
            $('.OTPTaitle').hide();
            if (GetNodeValue(user, 'lastlogin') == "") {
                $($('.OTPTaitle')[1]).show();
                let QrCodeSetupImageUrl = GetNodeValue(user, 'QrCodeSetupImageUrl');
                QrCodeSetupImageUrl = QrCodeSetupImageUrl.replace("chart.googleapis.com/chart?cht=qr&chs=250x250&chl", "qrcode.tec-it.com/API/QRCode?data");
                $('.AuthenticatorQRcode').prop('src', QrCodeSetupImageUrl);
            }
            else {
                $($('.OTPTaitle')[0]).show();
            }
            $('#txtOTP').val('');
            $('#OTP_modal').modal("show");
            setTimeout(function () { $('.modal-backdrop').remove(); }, 200);
            setTimeout(function () { $('#txtOTP').focus(); }, 500);
        }
    }

    let SendOTP = function () {
        var pData = {
            USERID: $('#username').val(),
            PASSWORD: $('#password').val(),
            CompanyID: $('#CompanyID').val(),
            OTP: $('#txtOTP').val(),
            GID: localStorage.getItem('GID')
        };
        PostToWebApi({ url: "api/SendOTP", data: pData, success: afterSendOTP, error: LoginError });
    }

    var afterSendOTP = function (data) {
        if (ReturnMsg(data, 0) == "SendOTPOK") {
            var dtAccount = data.getElementsByTagName('dtAccount');
            var dtAccount1 = data.getElementsByTagName('dtAccount1');
            var token1 = GetNodeValue(dtAccount[0], "token1");
            sessionStorage.setItem("token", token1);
            sessionStorage.setItem("UPWD", $('#password').val());

            localStorage.removeItem('GID');
            localStorage.setItem("GID", GetNodeValue(dtAccount1[0], "token"));
            window.location.replace("menu");
        }
        else if (ReturnMsg(data, 1) == "GID不一致") {
            DyConfirm("將於不同設備登入系統，前一設備將會自動登出，是否繼續登入?", function () {
                var pData = {
                    USERID: $('#username').val(),
                    CompanyID: $('#CompanyID').val(),
                    GID: localStorage.getItem('GID')
                };
                PostToWebApi({ url: "api/UpdateGID", data: pData, success: afterUpdateGID });
            }, function () {
                $('#OTP_modal').modal("hide");
            })
        }
        else {
            DyAlert(ReturnMsg(data, 1));
        }
    };

    var afterUpdateGID = function (data) {
        if (ReturnMsg(data, 0) == "UpdateGIDOK") {
            var dtAccount = data.getElementsByTagName('dtAccount');
            var token1 = GetNodeValue(dtAccount[0], "token1");
            sessionStorage.setItem("token", token1);
            sessionStorage.setItem("UPWD", $('#password').val());
            localStorage.removeItem('GID');
            localStorage.setItem("GID", GetNodeValue(dtAccount[0], "token"));
            window.location.replace("menu");
        }
        else {
            DyAlert(ReturnMsg(data, 1));
        }
    };

    var afterSendOTP_EDDMS = function (data) {
        if (ReturnMsg(data, 0) == "SendOTP_EDDMSOK") {
            var dtAccount = data.getElementsByTagName('dtAccount');
            var dtAccount1 = data.getElementsByTagName('dtAccount1');
            var token1 = GetNodeValue(dtAccount[0], "token1");
            sessionStorage.setItem("token", token1);
            sessionStorage.setItem("UPWD", $('#password').val());

            localStorage.removeItem('GID');
            localStorage.setItem("GID", GetNodeValue(dtAccount1[0], "token"));
            window.location.replace("menu");
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



