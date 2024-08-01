var PageMSPV101 = function (ParentNode) {
    var ChkCode0;
    var ChkCode1;
    var ChkCode2;
    var ChkCode3;

    var createCode = function () {
        var codeLength = 4;//驗證碼的長度	
        var random = new Array(0, 1, 2, 3, 4, 5, 6, 7, 8, 9);//隨機數 
        for (var i = 0; i < codeLength; i++) {
            var index = Math.floor(Math.random() * 10);//取得隨機數的索引

            if (i == 0) {
                ChkCode0 = random[index]
                $('#lblcode1').html(random[index])
            }
            else if (i == 1) {
                ChkCode1 = random[index]
                $('#lblcode2').html(random[index])
            }
            else if (i == 2) {
                ChkCode2 = random[index]
                $('#lblcode3').html(random[index])
            }
            else if (i == 3) {
                ChkCode3 = random[index]
                $('#lblcode4').html(random[index])
            }
        }
    }

    let btOK_click = function (bt) {
        $('#btOK').prop('disabled', true)
        //帳號
        if ($('#lblUID').html() == "") {
            DyAlert("無此帳號請重新確認!", function () { $('#btOK').prop('disabled', false); })
            return;
        }
        //舊密碼
        if ($('#txtOldUPWD').val() == "") {
            DyAlert("請輸入舊密碼!", function () { $('#btOK').prop('disabled', false); })
            return;
        }
        //新密碼
        if ($('#txtNewUPWD').val() == "") {
            DyAlert("請輸入新密碼!", function () { $('#btOK').prop('disabled', false); })
            return;
        }
        else {
            var chkCh = new RegExp("[\u4E00-\u9FA5]");
            var chkReg = new RegExp("[#*?!%@$^&()_+/]");
            var chkEn = new RegExp("[A-Za-z]");
            var chkNum = new RegExp("[0-9]");

            if (chkReg.test($('#txtNewUPWD').val()) == true) {
                DyAlert("新密碼不可含特殊符號!", function () { $('#btOK').prop('disabled', false); })
                return;
            }

            if (chkCh.test($('#txtNewUPWD').val()) == true) {
                DyAlert("新密碼不可含中文!", function () { $('#btOK').prop('disabled', false); })
                return;
            }

            if (chkEn.test($('#txtNewUPWD').val()) == true || chkNum.test($('#txtNewUPWD').val()) == true) {
            }
            else {
                DyAlert("新密碼需為英文字或數字!", function () { $('#btOK').prop('disabled', false); })
                return;
            }

            if ($('#txtNewUPWD').val().length < 6 | $('#txtNewUPWD').val().length > 10) {
                DyAlert("新密碼長度需介於6~10碼!", function () { $('#btOK').prop('disabled', false); })
                return;
            }
        }
        //確認新密碼
        if ($('#txtChkUPWD').val() == "") {
            DyAlert("請輸入確認新密碼!", function () { $('#btOK').prop('disabled', false); })
            return;
        }
        else {
            if ($('#txtChkUPWD').val().toLowerCase() != $('#txtNewUPWD').val().toLowerCase()) {
                DyAlert("確認新密碼需與新密碼一致!", function () { $('#btOK').prop('disabled', false); })
                return;
            }
        }

        //驗證碼
        if ($('#txtCode').val() == "") {
            DyAlert("請輸入驗證碼!", function () { $('#btOK').prop('disabled', false); })
            return;
        }
        else {
            var ChkCode = String(ChkCode0) + String(ChkCode1) + String(ChkCode2) + String(ChkCode3)
            var txtCode = $('#txtCode').val()
            if (ChkCode != txtCode) {
                DyAlert("驗證碼不符，請重新確認!", function () { $('#btOK').prop('disabled', false); })
                return;
            }
        }

        setTimeout(function () {
            var pData = {
                UID: $('#lblUID').html(),
                OldUPWD: $('#txtOldUPWD').val(),
                NewUPWD: $('#txtNewUPWD').val()
            }
            PostToWebApi({ url: "api/SystemSetup/MSPV101Save", data: pData, success: afterMSPV101Save });
        }, 1000);
    };

    let afterMSPV101Save = function (data) {
        if (ReturnMsg(data, 0) != "MSPV101SaveOK") {
            DyAlert(ReturnMsg(data, 1), function () { $('#btOK').prop('disabled', false); });
        }
        else {
            DyAlert("變更密碼成功!", function () {
                $('#btOK').prop('disabled', false);
                $('#txtOldUPWD').val('');
                $('#txtNewUPWD').val('');
                $('#txtChkUPWD').val('');
                $('#txtCode').val('')
                //更新驗證碼
                createCode();
            }, "下次登入請使用新密碼")
        };
    }
    let GetInitMSPV101 = function (data) {
        if (ReturnMsg(data, 0) != "GetInitmsDMOK") {
            DyAlert(ReturnMsg(data, 1));
        }
        else {
            var dtE = data.getElementsByTagName('dtE');
            if (dtE.length > 0) {
                $('#lblProgramName').html(GetNodeValue(dtE[0], "ChineseName"));
                $('#lblUID').html(GetNodeValue(dtE[0], "SysUser"));
            }
            $('#btOK').click(function () { btOK_click(this) })
            //更新驗證碼
            createCode();
            var recode = document.getElementById('recode');
            recode.addEventListener("click", function (e) {
                createCode();
                document.getElementById("txtCode").value = "";//清空文字框
                e.preventDefault();
            });
        }
    };

    let afterLoadPage = function () {
        var pData = {
            ProgramID: "MSPV101"
        }
        PostToWebApi({ url: "api/SystemSetup/GetInitmsDM", data: pData, success: GetInitMSPV101 });
    };

    if ($('#pgMSPV101').length == 0) {  
        AllPages = new LoadAllPages(ParentNode, "SystemSetup/MSPV101", ["MSPV101btns", "pgMSPV101Init", "pgMSPV101Add", "pgMSPV101Mod"], afterLoadPage);
    };
}