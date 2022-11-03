var PageISAMWHSET = function (ParentNode) {
    let WhSt

    let ChkLogOut_1 = function (AfterChkLogOut_1) {
        var LoginDT = sessionStorage.getItem('LoginDT');
        var cData = {
            LoginDT: LoginDT
        }
        PostToWebApi({ url: "api/js/ChkLogOut", data: cData, success: AfterChkLogOut_1 });
    };

    let AssignVar = function () {
        $('#btSaveISAMWH').click(function () {
            ChkLogOut_1(btSaveISAMWH_click);
        });
    };

    let btSaveISAMWH_click = function (data) {
        if (ReturnMsg(data, 0) != "ChkLogOutOK") {
            DyAlert(ReturnMsg(data, 1));
        }
        else {
            var dtLogin = data.getElementsByTagName('dtLogin');
            if (GetNodeValue(dtLogin[0], "LogOutType") != "") {
                window.location.href = "Login" + sessionStorage.getItem('isamcomp');
            }
            else {
if ($('#selST_ID').val() != "") {
            WhSt = $('#selST_ID option:selected').text()
            var pData = {
                WHSetSV: $('#selST_ID').val()
            };
            PostToWebApi({ url: "api/SystemSetup/SaveST_ID", data: pData, success: AfterSaveNewInventorySV });
        }
            }
        }
    }

    let AfterSaveNewInventorySV = function (data) {
        if (ReturnMsg(data, 0) != "SaveWhSetOK") {
            DyAlert(ReturnMsg(data, 0));
        }
        else {
            DyAlert("完成!");
            //alert(WhSt);
            //$('#defWh').text(ReturnMsg(data, 1));
            $('#defWh').text(WhSt);
        }
    }

    let afterGetInitWhSet = function (data) {
        let dtWh;
        let dtUserWh;
       //alert("afterGetInitWhSet");
        dtUserWh = data.getElementsByTagName('dtUserWh');
        dtWh = data.getElementsByTagName('dtWh');
        $('#defWh').text(GetNodeValue(dtUserWh[0], "WhName"));
        
        //alert("InitSelectItem");
        InitSelectItem($('#selST_ID')[0], dtWh, "ST_ID", "STName", true, "*請選擇店別" );
        AssignVar();
        //$('#btSaveISAMW').click(function () { btSave_click(); });

    };

    let afterLoadPage = function () {
        //alert("afterLoadPage");
        PostToWebApi({ url: "api/SystemSetup/GetWh", success: afterGetInitWhSet });
        $('#pgWhSet').show();
    };

    if ($('#pgWhSet').length == 0) {
        //2021-04-29 Debug用，按F12後，在主控台內會顯示aaaaaa
        //console.log("aaaaaa");
        //alert("VMN01");
        AllPages = new LoadAllPages(ParentNode, "SystemSetup/ISAMWhSet", ["pgWhSet"], afterLoadPage);
    };
}
