(function ($) {
    //"use strict";
    var Initdoc = function () {
        $('#CompanyName').val();
        LoginCompany();

        $('#btSignIn').click(function () { LoginSys(); });

        $('#password').keypress(function (e) {
            $('#icrpwd').hide();
            if (e.which == 13) {
                LoginSys();
            }
        });
    };

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
            var token = GetNodeValue(dtE[0], "token");
            var companyid = window.location.search;
            sessionStorage.setItem("token", token);
            sessionStorage.setItem("isamcomp", companyid);
            window.location.replace("menu");
        }
        else if (ReturnMsg(data, 1) == "密碼錯誤") {
            $('#icrpwd').show();
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



