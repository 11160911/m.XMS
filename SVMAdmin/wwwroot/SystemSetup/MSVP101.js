var PageMSVP101 = function (ParentNode) {

    let grdM;
    let grdSendSet;
    let grdLookUp_VIPFaceID_SendSet;
    let grdLookUp_City_SendSet;
    let grdLookUp_Dept_SendSet;
    let grdLookUp_Bgno_SendSet;
    let grdEDMHistoryQuery;
    let grdDMSel;

    let chkVIPFaceID = "";
    let chkVIPFaceIDName = "";
    let chkCity = "";
    let chkDept = "";
    let chkDeptName = "";
    let chkBgno = "";
    let chkBgnoName = "";
    let DMDocNo = "";


    let AssignVar = function () {
        
        grdM = new DynGrid(
            {
                table_lement: $('#tbQuery')[0],
                class_collection: ["tdCol1 text-center", "tdCol2 text-center", "tdCol3 text-center", "tdCol4 text-center", "tdColbt icon_in_td btShowEDM", "tdCol5 text-center", "tdCol6", "tdCol7 text-center"],
                fields_info: [
                    { type: "Text", name: "EVNO", style: "" },
                    { type: "TextAmt", name: "Cnt"},
                    { type: "Text", name: "ApproveDate" },
                    { type: "Text", name: "TOMailDate" },
                    { type: "JQ", name: "fa-binoculars", element: '<i class="fa fa-binoculars" style="font-size:24px;color:#348000"></i>' },
                    { type: "Text", name: "EDM_DocNo"},
                    { type: "Text", name: "EDMMemo"},
                    { type: "Text", name: "EDDate" }
                ],
                rows_per_page: 50,
                method_clickrow: click_PLU,
                afterBind: InitModifyDeleteButton,
                sortable: "Y",
                step: "Y"
            }
        );

        grdSendSet = new DynGrid(
            {
                table_lement: $('#tbSendSet')[0],
                class_collection: ["tdColbt icon_in_td btDelete_SendSet", "tdCol8", "tdCol9", "tdCol10", "tdCol11", "tdCol12", "tdCol13", "tdCol14", "tdCol15", "tdCol15 label-align", "tdCol16"],
                fields_info: [
                    { type: "JQ", name: "fa-trash-o", element: '<i class="fa fa-trash-o" style="font-size:24px"></i>' },
                    { type: "Text", name: "VIP_ID2", style: "" },
                    { type: "Text", name: "VIP_Name" },
                    { type: "Text", name: "VIP_Tel" },
                    { type: "Text", name: "VIP_Eadd" },
                    { type: "Text", name: "VIP_NM" },
                    { type: "Text", name: "City" },
                    { type: "Text", name: "AreaName" },
                    { type: "Text", name: "VIP_LCDay" },
                    { type: "TextAmt", name: "PointsBalance" },
                    { type: "Text", name: "VIP_Type" },
                ],
                rows_per_page: 100,
                method_clickrow: click_PLU,
                afterBind: gridclick_SendSet,
                sortable: "N"
            }
        );

        grdEDMHistoryQuery = new DynGrid(
            {
                table_lement: $('#tbEDMHistoryQuery')[0],
                class_collection: ["tdCol8 text-center", "tdCol9 text-center", "tdCol10 text-center", "tdCol11 text-center", "tdCol12 text-center", "tdCol13 text-center", "tdCol14 text-center", "tdCol15 text-center"],
                fields_info: [
                    { type: "Text", name: "VIP_ID2", style: "" },
                    { type: "Text", name: "VIP_Name" },
                    { type: "Text", name: "VIP_Tel" },
                    { type: "Text", name: "VIP_Eadd" },
                    { type: "Text", name: "VIP_MW" },
                    { type: "Text", name: "City" },
                    { type: "Text", name: "AreaName" },
                    { type: "Text", name: "VIP_Type" }
                ],
                rows_per_page: 100,
                method_clickrow: click_PLU,
                sortable: "N"
            }
        );

        grdLookUp_VIPFaceID_SendSet = new DynGrid(
            {
                table_lement: $('#tbDataLookup_VIPFaceID_SendSet')[0],
                class_collection: ["tdCol8 text-center", "tdCol9", "tdCol10"],
                fields_info: [
                    { type: "checkbox", name: "chkset", style: "width:16px;height:16px" },
                    { type: "Text", name: "ST_ID", style: "" },
                    { type: "Text", name: "ST_SName", style: "" }
                ],
                //rows_per_page: 10,
                method_clickrow: click_PLU,
                sortable: "N"
            }
        );

        grdLookUp_City_SendSet = new DynGrid(
            {
                table_lement: $('#tbLookup_City_SendSet')[0],
                class_collection: ["tdCol8 text-center", "tdCol9"],
                fields_info: [
                    { type: "checkbox", name: "chkset", style: "width:16px;height:16px" },
                    { type: "Text", name: "City", style: "" }
                ],
                //rows_per_page: 10,
                method_clickrow: click_PLU,
                sortable: "N"
            }
        );

        grdLookUp_Dept_SendSet = new DynGrid(
            {
                table_lement: $('#tbLookup_Dept_SendSet')[0],
                class_collection: ["tdCol8 text-center", "tdCol9", "tdCol10"],
                fields_info: [
                    { type: "checkbox", name: "chkset", style: "width:16px;height:16px" },
                    { type: "Text", name: "Type_ID", style: "" },
                    { type: "Text", name: "Type_Name", style: "" }
                ],
                //rows_per_page: 10,
                method_clickrow: click_PLU,
                sortable: "N"
            }
        );

        grdLookUp_Bgno_SendSet = new DynGrid(
            {
                table_lement: $('#tbLookup_Bgno_SendSet')[0],
                class_collection: ["tdCol8 text-center", "tdCol9", "tdCol10"],
                fields_info: [
                    { type: "checkbox", name: "chkset", style: "width:16px;height:16px" },
                    { type: "Text", name: "Type_ID", style: "" },
                    { type: "Text", name: "Type_Name", style: "" }
                ],
                //rows_per_page: 10,
                method_clickrow: click_PLU,
                sortable: "N"
            }
        );

        grdDMSel = new DynGrid(
            {
                table_lement: $('#tbDMSel')[0],
                class_collection: ["tdCol8 text-center", "tdCol9", "tdCol10", "tdCol11","tdColbt icon_in_td btShowEDM_DMSel", "tdCol12", "tdCol13", "tdCol14", "tdCol15", "tdCol16 label-align"],
                fields_info: [
                    { type: "radio", name: "chkset", style: "width:15px;height:15px" },
                    { type: "Text", name: "DocNo" },
                    { type: "Text", name: "EDMMemo" },
                    { type: "Text", name: "EDDate1" },
                    { type: "JQ", name: "fa-binoculars", element: '<i class="fa fa-binoculars" style="font-size:24px;color:#348000"></i>' },
                    { type: "Text", name: "ActivityCode" },
                    { type: "Text", name: "PS_Name" },
                    { type: "Text", name: "EDDate2" },
                    { type: "Text", name: "WhNoFlag" },
                    { type: "TextAmt", name: "Cnt2" }
                ],
                rows_per_page: 10,
                method_clickrow: click_PLU,
                afterBind: gridclick_DMSel,
                sortable: "N"
            }
        );
        return;
    };

    let click_PLU = function (tr) {

    };

    let InitModifyDeleteButton = function () {
        $('#tbQuery tbody tr .tdCol1,#tbQuery tbody tr .tdCol2,#tbQuery tbody tr .tdCol3,#tbQuery tbody tr .tdCol4,#tbQuery tbody tr .tdCol5,#tbQuery tbody tr .tdCol6,#tbQuery tbody tr .tdCol7').click(function () { EDMHistoryQuery_click(this) });
        $('#tbQuery tbody tr .btShowEDM').click(function () { btShowEDM_click(this) });
    }

    let gridclick_SendSet = function () {
        $('#tbSendSet tbody tr .btDelete_SendSet').click(function () { btDelete_SendSet_click(this) });
    }

    let gridclick_DMSel = function () {
        $('#tbDMSel tbody tr .btShowEDM_DMSel').click(function () { btShowEDM_DMSel_click(this) });
    }

    let EDMHistoryQuery_click = function (bt) {
        
        //$('#tbQuery td').closest('tr').css('background-color', 'transparent');
        $(bt).closest('tr').click();
        $('.msg-valid').hide();
        var node = $(grdM.ActiveRowTR()).prop('Record');
        //$('#tbQuery td:contains(' + GetNodeValue(node, 'EVNO') + ')').closest('tr').css('background-color', '#DEEBF7');
        $('#lblEVNO_EDMHistoryQuery').html(GetNodeValue(node, 'EVNO'))
        $('#lblEDMDocNo_EDMHistoryQuery').html(GetNodeValue(node, 'EDM_DocNo'))
        $('#lblEDDate_EDMHistoryQuery').html(GetNodeValue(node, 'EDDate'))
        $('#lblStartDate_EDMHistoryQuery').html(GetNodeValue(node, 'ApproveDate'))
        $('#lblEDMMemo_EDMHistoryQuery').html(GetNodeValue(node, 'EDMMemo'))
        $('#modal_EDMHistoryQuery').modal('show');
        setTimeout(function () {
            QueryEDMHistoryQuery(GetNodeValue(node, 'EVNO'));
        }, 500);
    };

    let QueryEDMHistoryQuery = function (EVNO) {
        ShowLoading();
        var pData = {
            EVNO: EVNO
        }
        PostToWebApi({ url: "api/SystemSetup/MSVP101EDMHistoryQuery", data: pData, success: afterMSVP101EDMHistoryQuery });
    };

    let afterMSVP101EDMHistoryQuery = function (data) {
        CloseLoading();
        if (ReturnMsg(data, 0) != "MSVP101EDMHistoryQueryOK") {
            DyAlert(ReturnMsg(data, 1));
        }
        else {
            var dtE = data.getElementsByTagName('dtE');
            var dtV = data.getElementsByTagName('dtV');
            var dtC = data.getElementsByTagName('dtC');
            grdEDMHistoryQuery.BindData(dtE);
            if (dtE.length == 0) {
                DyAlert("無符合資料!");
                $("#divVIPCon_EDMHistoryQuery").empty();
                $("#lblVIPCnt_EDMHistoryQuery").html('0 筆')
                //$(".modal-backdrop").remove();
                return;
            }
            $("#lblVIPCnt_EDMHistoryQuery").html(parseInt(GetNodeValue(dtC[0], "Cnt")).toLocaleString('en-US') + ' 筆')

            var VIPCon = document.getElementById("divVIPCon_EDMHistoryQuery");
            $("#divVIPCon_EDMHistoryQuery").empty();
            for (var i = 0; i < dtV.length; i++) {
                var p1 = document.createElement('label')
                p1.innerHTML = GetNodeValue(dtV[i], "ColTitle");
                p1.style.cssText = 'margin-top: 3px;margin-left: 3px;margin-bottom: 0px;font-size: 16px;color: #1e62d0;font-weight: bold;';

                var p2 = document.createElement('label')
                p2.innerHTML = GetNodeValue(dtV[i], "ColData");
                p2.style.cssText = 'margin-left: 15px; margin-top: 3px;font-size: 16px;color: black;font-weight: bold; ';

                var p3 = document.createElement('p')
                p3.style.cssText = 'margin-bottom: 0px';
                VIPCon.appendChild(p1);
                VIPCon.appendChild(p2);
                VIPCon.appendChild(p3);
            }
        }
    };

    //DM預覽(主畫面)
    let btShowEDM_click = function (bt) {
        //Timerset();
        $(bt).closest('tr').click();
        $('.msg-valid').hide();
        var node = $(grdM.ActiveRowTR()).prop('Record');
        DMDocNo = GetNodeValue(node, 'EDM_DocNo')

        var pData = {
        }
        PostToWebApi({ url: "api/SystemSetup/GetCompanyShowEDM", data: pData, success: afterShowEDM });
    };

    let afterShowEDM = function (data) {
        if (ReturnMsg(data, 0) != "GetCompanyShowEDMOK") {
            DyAlert(ReturnMsg(data, 1));
        }
        else {
            var dtE = data.getElementsByTagName('dtE');
            var hostname = location.hostname;
            //測試環境
            if (hostname.indexOf("94") >= 0 || hostname.indexOf("localhost") >= 0) {
                window.open("http://192.168.1.94/ShowEDMWEB/ShowEDMWEB?company=" + GetNodeValue(dtE[0], "CompanyID") + ";" + DMDocNo + "");
            }
            //正式環境
            else {
                window.open("https://www.portal.e-dynasty.com.tw/ShowEDMWEB/ShowEDMWEB?company=" + GetNodeValue(dtE[0], "CompanyID") + ";" + DMDocNo + "");
            }
        }
    };

    //刪除(發送設定)
    let btDelete_SendSet_click = function (bt) {
        $(bt).closest('tr').click();
        $('.msg-valid').hide();
        var node = $(grdSendSet.ActiveRowTR()).prop('Record');

        DyConfirm("請確認是否刪除會員(" + GetNodeValue(node, 'VIP_ID2') + ")？", function () {
            var pData = {
                VMEVNO: $('#lblVMEVNO_SendSet').html(),
                VIP_ID2: GetNodeValue(node, 'VIP_ID2')
            }
            PostToWebApi({ url: "api/SystemSetup/MSVP101Delete_SendSet", data: pData, success: afterMSVP101Delete_SendSet });
        }, function () { DummyFunction(); })
    };

    let afterMSVP101Delete_SendSet = function (data) {
        if (ReturnMsg(data, 0) != "MSVP101Delete_SendSetOK") {
            DyAlert(ReturnMsg(data, 1));
        }
        else {
            DyAlert("刪除完成!")
            var dtE = data.getElementsByTagName('dtE');
            $('#lblVIPCnt_SendSet').html(GetNodeValue(dtE[0], 'Cnt') + ' 筆')
            if (GetNodeValue(dtE[0], 'Cnt') == "0") {
                $('#btDMSend_SendSet').prop('disabled', true);
                $('#btDMSend_SendSet').css('background-color', 'gray');
            }
            grdSendSet.DeleteRow(grdSendSet.ActiveRowTR());
        }
    };

    //DM預覽(DM選取)
    let btShowEDM_DMSel_click = function (bt) {
        //Timerset();
        $(bt).closest('tr').click();
        $('.msg-valid').hide();
        var node = $(grdDMSel.ActiveRowTR()).prop('Record');
        DMDocNo = GetNodeValue(node, 'DocNo')

        var pData = {
        }
        PostToWebApi({ url: "api/SystemSetup/GetCompanyShowEDM", data: pData, success: afterShowEDM_DMSel });
    };

    let afterShowEDM_DMSel = function (data) {
        if (ReturnMsg(data, 0) != "GetCompanyShowEDMOK") {
            DyAlert(ReturnMsg(data, 1));
        }
        else {
            var dtE = data.getElementsByTagName('dtE');
            var hostname = location.hostname;
            //測試環境
            if (hostname.indexOf("94") >= 0 || hostname.indexOf("localhost") >= 0) {
                window.open("http://192.168.1.94/ShowEDMWEB/ShowEDMWEB?company=" + GetNodeValue(dtE[0], "CompanyID") + ";" + DMDocNo + "");
            }
            //正式環境
            else {
                window.open("https://www.portal.e-dynasty.com.tw/ShowEDMWEB/ShowEDMWEB?company=" + GetNodeValue(dtE[0], "CompanyID") + ";" + DMDocNo + "");
            }
        }
    };

    let ChkLogOut_1 = function (AfterChkLogOut_1) {
        var LoginDT = sessionStorage.getItem('LoginDT');
        var cData = {
            LoginDT: LoginDT
        }
        PostToWebApi({ url: "api/js/ChkLogOut", data: cData, success: AfterChkLogOut_1 });
    };

    //清除
    let btClear_click = function (bt) {
        //Timerset();
        $('#txtEVNO').val('');
        $('#txtEDM_DocNo').val('');
        $('#txtStartDate').val('');
        $('#txtEDMMemo').val('');
    };

    //查詢
    let btQuery_click = function (bt) {
        //Timerset();
        //$('#tbQuery thead tr th').css('background-color', '#ffb620')
        $('#btQuery').prop('disabled', true)
        ShowLoading();
        var pData = {
            EVNO: $('#txtEVNO').val(),
            EDM_DocNo: $('#txtEDM_DocNo').val(),
            StartDate: $('#txtStartDate').val().toString().replaceAll('-', '/'),
            EDMMemo: $('#txtEDMMemo').val()
        }
        PostToWebApi({ url: "api/SystemSetup/MSVP101Query", data: pData, success: afterMSVP101Query });
    };

    let afterMSVP101Query = function (data) {
        CloseLoading();
        if (ReturnMsg(data, 0) != "MSVP101QueryOK") {
            DyAlert(ReturnMsg(data, 1), function () { $('#btQuery').prop('disabled', false); });
        }
        else {
            $('#btQuery').prop('disabled', false);
            var dtE = data.getElementsByTagName('dtE');
            if (dtE.length == 0) {
                DyAlert("無符合資料!");
                return;
            }
            grdM.BindData(dtE);
        }
    };

    //發送設定
    let btSendSet_click = function (bt) {
        //Timerset();
        var pData = {
        }
        PostToWebApi({ url: "api/SystemSetup/MSVP102_GetVMEVNO", data: pData, success: afterMSVP101_GetVMEVNO });
    };

    let afterMSVP101_GetVMEVNO = function (data) {
        if (ReturnMsg(data, 0) != "MSVP102_GetVMEVNOOK") {
            DyAlert(ReturnMsg(data, 1));
        }
        else {
            var dtE = data.getElementsByTagName('dtE');
            var dtV = data.getElementsByTagName('dtV');
            $('#lblVMEVNO_SendSet').html(GetNodeValue(dtE[0], "DocNo"))
            $('#lblVIPCnt_SendSet').html('0 筆')
            btClear_SendSet_click();
            grdSendSet.BindData(dtV);
            $('#modal_SendSet').modal('show')
        }
    };

    //會籍店櫃多選(發送設定)
    let btVIPFaceID_SendSet_click = function (bt) {
        //Timerset();
        var pData = {
            ST_ID: ""
        }
        PostToWebApi({ url: "api/SystemSetup/MSVP102_GetVIPFaceID", data: pData, success: afterMSVP101_GetVIPFaceID });
    };

    let afterMSVP101_GetVIPFaceID = function (data) {
        if (ReturnMsg(data, 0) != "MSVP102_GetVIPFaceIDOK") {
            DyAlert(ReturnMsg(data, 1));
        }
        else {
            var dtE = data.getElementsByTagName('dtE');
            $('#txtLpQ_VIPFaceID_SendSet').val('');
            $('#modal_Lookup_VIPFaceID_SendSet').modal('show');
            setTimeout(function () {
                grdLookUp_VIPFaceID_SendSet.BindData(dtE);
                if (chkVIPFaceID != "") {
                    var VIPFaceID = chkVIPFaceID.split(',');
                    for (var i = 0; i < VIPFaceID.length; i++) {
                        $('#tbDataLookup_VIPFaceID_SendSet tbody tr .tdCol9').filter(function () { return $(this).text() == VIPFaceID[i].replaceAll("'", ""); }).closest('tr').find('.tdCol8 input:checkbox').prop('checked', true);
                    }
                }
            }, 500);
        }
    };

    let btLpQ_VIPFaceID_SendSet_click = function (bt) {
        //Timerset();
        $('#btLpQ_VIPFaceID_SendSet').prop('disabled', true);
        var pData = {
            ST_ID: $('#txtLpQ_VIPFaceID_SendSet').val()
        }
        PostToWebApi({ url: "api/SystemSetup/MSVP102_GetVIPFaceID", data: pData, success: afterLpQ_VIPFaceID_SendSet });
    };

    let afterLpQ_VIPFaceID_SendSet = function (data) {
        if (ReturnMsg(data, 0) != "MSVP102_GetVIPFaceIDOK") {
            DyAlert(ReturnMsg(data, 1), function () {
                $('#btLpQ_VIPFaceID_SendSet').prop('disabled', false);
            });
        }
        else {
            var dtE = data.getElementsByTagName('dtE');
            if (dtE.length == 0) {
                DyAlert("無符合資料!", function () {
                    $('#btLpQ_VIPFaceID_SendSet').prop('disabled', false);
                });
                //$(".modal-backdrop").remove();
                return;
            }
            grdLookUp_VIPFaceID_SendSet.BindData(dtE);
            $('#btLpQ_VIPFaceID_SendSet').prop('disabled', false);
        }
    };

    let btLpOK_VIPFaceID_SendSet_click = function (bt) {
        //Timerset();
        $('#btLpOK_VIPFaceID_SendSet').prop('disabled', true);
        var obchkedtd = $('#tbDataLookup_VIPFaceID_SendSet .checkbox:checked');
        chkedRow = obchkedtd.length.toString();   //本次已勾選的總筆數
        if (chkedRow == 0) {
            $('#lblVIPFaceIDCnt_SendSet').html('');
            $('#lblVIPFaceIDName_SendSet').html('');
            chkVIPFaceID = "";
            chkVIPFaceIDName = "";
            $('#btLpOK_VIPFaceID_SendSet').prop('disabled', false);
            $('#modal_Lookup_VIPFaceID_SendSet').modal('hide');
            UpdateVIPCnt();
            return
        } else {
            chkVIPFaceID = "";
            chkVIPFaceIDName = "";
            var VIPFaceIDName = "";
            for (var i = 0; i < obchkedtd.length; i++) {
                var a = $(obchkedtd[i]).closest('tr');
                var trNode = $(a).prop('Record');
                chkVIPFaceID += "'" + GetNodeValue(trNode, "ST_ID") + "',";  //已勾選的每一筆店倉
                chkVIPFaceIDName += "'" + GetNodeValue(trNode, "ST_SName") + "',";
                if (i <= 4) {
                    VIPFaceIDName += GetNodeValue(trNode, "ST_SName") + "，";
                }
            }
            chkVIPFaceID = chkVIPFaceID.substr(0, chkVIPFaceID.length - 1)
            chkVIPFaceIDName = chkVIPFaceIDName.substr(0, chkVIPFaceIDName.length - 1)
            if (chkedRow > 5) {
                $('#lblVIPFaceIDCnt_SendSet').html(chkedRow)
                $('#lblVIPFaceIDName_SendSet').html(VIPFaceIDName.substr(0, VIPFaceIDName.length - 1) + '...')
            }
            else {
                $('#lblVIPFaceIDCnt_SendSet').html('')
                $('#lblVIPFaceIDName_SendSet').html(VIPFaceIDName.substr(0, VIPFaceIDName.length - 1))
            }
            $('#btLpOK_VIPFaceID_SendSet').prop('disabled', false);
            $('#modal_Lookup_VIPFaceID_SendSet').modal('hide');
            UpdateVIPCnt();
        }
    };

    let btLpExit_VIPFaceID_SendSet_click = function (bt) {
        //Timerset();
        $('#modal_Lookup_VIPFaceID_SendSet').modal('hide');
    };

    let btLpClear_VIPFaceID_SendSet_click = function (bt) {
        //Timerset();
        $("#txtLpQ_VIPFaceID_SendSet").val('');
        $("#tbDataLookup_VIPFaceID_SendSet .checkbox").prop('checked', false);
    };

    //縣市多選(發送設定)
    let btCity_SendSet_click = function (bt) {
        //Timerset();
        var pData = {
            City: ""
        }
        PostToWebApi({ url: "api/SystemSetup/GetCity", data: pData, success: afterGetCity });
    };

    let afterGetCity = function (data) {
        if (ReturnMsg(data, 0) != "GetCityOK") {
            DyAlert(ReturnMsg(data, 1));
        }
        else {
            var dtE = data.getElementsByTagName('dtE');
            $('#txtLpQ_City_SendSet').val('');
            $('#modal_Lookup_City_SendSet').modal('show');
            setTimeout(function () {
                grdLookUp_City_SendSet.BindData(dtE);
                if (chkCity != "") {
                    var City = chkCity.split(',');
                    for (var i = 0; i < City.length; i++) {
                        $('#tbLookup_City_SendSet tbody tr .tdCol9').filter(function () { return $(this).text() == City[i].replaceAll("'", ""); }).closest('tr').find('.tdCol8 input:checkbox').prop('checked', true);
                    }
                }
            }, 500);
        }
    };

    let btLpQ_City_SendSet_click = function (bt) {
        //Timerset();
        $('#btLpQ_City_SendSet').prop('disabled', true);
        var pData = {
            City: $('#txtLpQ_City_SendSet').val()
        }
        PostToWebApi({ url: "api/SystemSetup/GetCity", data: pData, success: afterLpQ_City });
    };

    let afterLpQ_City = function (data) {
        if (ReturnMsg(data, 0) != "GetCityOK") {
            DyAlert(ReturnMsg(data, 1), function () {
                $('#btLpQ_City_SendSet').prop('disabled', false);
            });
        }
        else {
            var dtE = data.getElementsByTagName('dtE');
            if (dtE.length == 0) {
                DyAlert("無符合資料!", function () {
                    $('#btLpQ_City_SendSet').prop('disabled', false);
                });
                //$(".modal-backdrop").remove();
                return;
            }
            grdLookUp_City_SendSet.BindData(dtE);
            $('#btLpQ_City_SendSet').prop('disabled', false);
        }
    };

    let btLpOK_City_SendSet_click = function (bt) {
        //Timerset();
        $('#btLpOK_City_SendSet').prop('disabled', true);
        var obchkedtd = $('#tbLookup_City_SendSet .checkbox:checked');
        chkedRow = obchkedtd.length.toString();   //本次已勾選的總筆數
        if (chkedRow == 0) {
            $('#lblCityCnt_SendSet').html('');
            $('#lblCityName_SendSet').html('');
            chkCity = "";
            $('#btLpOK_City_SendSet').prop('disabled', false);
            $('#modal_Lookup_City_SendSet').modal('hide');
            UpdateVIPCnt();
            return
        } else {
            chkCity = "";
            var CityName = "";
            for (var i = 0; i < obchkedtd.length; i++) {
                var a = $(obchkedtd[i]).closest('tr');
                var trNode = $(a).prop('Record');
                chkCity += "'" + GetNodeValue(trNode, "City") + "',";  //已勾選的每一筆縣市
                if (i <= 4) {
                    CityName += GetNodeValue(trNode, "City") + "，";
                }
            }
            chkCity = chkCity.substr(0, chkCity.length - 1)
            if (chkedRow > 5) {
                $('#lblCityCnt_SendSet').html(chkedRow)
                $('#lblCityName_SendSet').html(CityName.substr(0, CityName.length - 1) + '...')
            }
            else {
                $('#lblCityCnt_SendSet').html('')
                $('#lblCityName_SendSet').html(CityName.substr(0, CityName.length - 1))
            }
            $('#btLpOK_City_SendSet').prop('disabled', false);
            $('#modal_Lookup_City_SendSet').modal('hide');
            UpdateVIPCnt();
        }
    };

    let btLpExit_City_SendSet_click = function (bt) {
        //Timerset();
        $('#modal_Lookup_City_SendSet').modal('hide');
    };

    let btLpClear_City_SendSet_click = function (bt) {
        //Timerset();
        $("#txtLpQ_City_SendSet").val('');
        $("#tbLookup_City_SendSet .checkbox").prop('checked', false);
    };

    //消費部門多選(發送設定)
    let btDept_SendSet_click = function (bt) {
        //Timerset();
        var pData = {
            Type_Code: "G",
            Type_ID: ""
        }
        PostToWebApi({ url: "api/SystemSetup/GetTypeDataWeb", data: pData, success: afterGetDept });
    };

    let afterGetDept = function (data) {
        if (ReturnMsg(data, 0) != "GetTypeDataWebOK") {
            DyAlert(ReturnMsg(data, 1));
        }
        else {
            var dtE = data.getElementsByTagName('dtE');
            $('#txtLpQ_Dept_SendSet').val('');
            $('#modal_Lookup_Dept_SendSet').modal('show');
            setTimeout(function () {
                grdLookUp_Dept_SendSet.BindData(dtE);
                if (chkDept != "") {
                    var Dept = chkDept.split(',');
                    for (var i = 0; i < Dept.length; i++) {
                        $('#tbLookup_Dept_SendSet tbody tr .tdCol9').filter(function () { return $(this).text() == Dept[i].replaceAll("'", ""); }).closest('tr').find('.tdCol8 input:checkbox').prop('checked', true);
                    }
                }
            }, 500);
        }
    };

    let btLpQ_Dept_SendSet_click = function (bt) {
        //Timerset();
        $('#btLpQ_Dept_SendSet').prop('disabled', true);
        var pData = {
            Type_Code: "G",
            Type_ID: $('#txtLpQ_Dept_SendSet').val()
        }
        PostToWebApi({ url: "api/SystemSetup/GetTypeDataWeb", data: pData, success: afterLpQ_Dept });
    };

    let afterLpQ_Dept = function (data) {
        if (ReturnMsg(data, 0) != "GetTypeDataWebOK") {
            DyAlert(ReturnMsg(data, 1), function () {
                $('#btLpQ_Dept_SendSet').prop('disabled', false);
            });
        }
        else {
            var dtE = data.getElementsByTagName('dtE');
            if (dtE.length == 0) {
                DyAlert("無符合資料!", function () {
                    $('#btLpQ_Dept_SendSet').prop('disabled', false);
                });
                //$(".modal-backdrop").remove();
                return;
            }
            grdLookUp_Dept_SendSet.BindData(dtE);
            $('#btLpQ_Dept_SendSet').prop('disabled', false);
        }
    };

    let btLpOK_Dept_SendSet_click = function (bt) {
        //Timerset();
        $('#btLpOK_Dept_SendSet').prop('disabled', true);
        var obchkedtd = $('#tbLookup_Dept_SendSet .checkbox:checked');
        chkedRow = obchkedtd.length.toString();   //本次已勾選的總筆數
        if (chkedRow == 0) {
            $('#lblDeptCnt_SendSet').html('');
            $('#lblDeptName_SendSet').html('');
            chkDept = "";
            chkDeptName = "";
            $('#btLpOK_Dept_SendSet').prop('disabled', false);
            $('#modal_Lookup_Dept_SendSet').modal('hide');
            UpdateVIPCnt();
            return
        } else {
            chkDept = "";
            chkDeptName = "";
            var DeptName = "";
            for (var i = 0; i < obchkedtd.length; i++) {
                var a = $(obchkedtd[i]).closest('tr');
                var trNode = $(a).prop('Record');
                chkDept += "'" + GetNodeValue(trNode, "Type_ID") + "',";  //已勾選的每一筆部門
                chkDeptName += "'" + GetNodeValue(trNode, "Type_Name") + "',";
                if (i <= 4) {
                    DeptName += GetNodeValue(trNode, "Type_Name") + "，";
                }
            }
            chkDept = chkDept.substr(0, chkDept.length - 1)
            chkDeptName = chkDeptName.substr(0, chkDeptName.length - 1)
            if (chkedRow > 5) {
                $('#lblDeptCnt_SendSet').html(chkedRow)
                $('#lblDeptName_SendSet').html(DeptName.substr(0, DeptName.length - 1) + '...')
            }
            else {
                $('#lblDeptCnt_SendSet').html('')
                $('#lblDeptName_SendSet').html(DeptName.substr(0, DeptName.length - 1))
            }
            $('#btLpOK_Dept_SendSet').prop('disabled', false);
            $('#modal_Lookup_Dept_SendSet').modal('hide');
            UpdateVIPCnt();
        }
    };

    let btLpExit_Dept_SendSet_click = function (bt) {
        //Timerset();
        $('#modal_Lookup_Dept_SendSet').modal('hide');
    };

    let btLpClear_Dept_SendSet_click = function (bt) {
        //Timerset();
        $("#txtLpQ_Dept_SendSet").val('');
        $("#tbLookup_Dept_SendSet .checkbox").prop('checked', false);
    };

    //消費大類多選(發送設定)
    let btBgno_SendSet_click = function (bt) {
        //Timerset();
        var pData = {
            Type_Code: "L",
            Type_ID: ""
        }
        PostToWebApi({ url: "api/SystemSetup/GetTypeDataWeb", data: pData, success: afterGetBgno });
    };

    let afterGetBgno = function (data) {
        if (ReturnMsg(data, 0) != "GetTypeDataWebOK") {
            DyAlert(ReturnMsg(data, 1));
        }
        else {
            var dtE = data.getElementsByTagName('dtE');
            $('#txtLpQ_Bgno_SendSet').val('');
            $('#modal_Lookup_Bgno_SendSet').modal('show');
            setTimeout(function () {
                grdLookUp_Bgno_SendSet.BindData(dtE);
                if (chkBgno != "") {
                    var Bgno = chkBgno.split(',');
                    for (var i = 0; i < Bgno.length; i++) {
                        $('#tbLookup_Bgno_SendSet tbody tr .tdCol9').filter(function () { return $(this).text() == Bgno[i].replaceAll("'", ""); }).closest('tr').find('.tdCol8 input:checkbox').prop('checked', true);
                    }
                }
            }, 500);
        }
    };

    let btLpQ_Bgno_SendSet_click = function (bt) {
        //Timerset();
        $('#btLpQ_Bgno_SendSet').prop('disabled', true);
        var pData = {
            Type_Code: "B",
            Type_ID: $('#txtLpQ_Bgno_SendSet').val()
        }
        PostToWebApi({ url: "api/SystemSetup/GetTypeDataWeb", data: pData, success: afterLpQ_Bgno });
    };

    let afterLpQ_Bgno = function (data) {
        if (ReturnMsg(data, 0) != "GetTypeDataWebOK") {
            DyAlert(ReturnMsg(data, 1), function () {
                $('#btLpQ_Bgno_SendSet').prop('disabled', false);
            });
        }
        else {
            var dtE = data.getElementsByTagName('dtE');
            if (dtE.length == 0) {
                DyAlert("無符合資料!", function () {
                    $('#btLpQ_Bgno_SendSet').prop('disabled', false);
                });
                //$(".modal-backdrop").remove();
                return;
            }
            grdLookUp_Bgno_SendSet.BindData(dtE);
            $('#btLpQ_Bgno_SendSet').prop('disabled', false);
        }
    };

    let btLpOK_Bgno_SendSet_click = function (bt) {
        //Timerset();
        $('#btLpOK_Bgno_SendSet').prop('disabled', true);
        var obchkedtd = $('#tbLookup_Bgno_SendSet .checkbox:checked');
        chkedRow = obchkedtd.length.toString();   //本次已勾選的總筆數
        if (chkedRow == 0) {
            $('#lblBgnoCnt_SendSet').html('');
            $('#lblBgnoName_SendSet').html('');
            chkBgno = "";
            chkBgnoName = "";
            $('#btLpOK_Bgno_SendSet').prop('disabled', false);
            $('#modal_Lookup_Bgno_SendSet').modal('hide');
            UpdateVIPCnt();
            return
        } else {
            chkBgno = "";
            chkBgnoName = "";
            var BgnoName = "";
            for (var i = 0; i < obchkedtd.length; i++) {
                var a = $(obchkedtd[i]).closest('tr');
                var trNode = $(a).prop('Record');
                chkBgno += "'" + GetNodeValue(trNode, "Type_ID") + "',";  //已勾選的每一筆部門
                chkBgnoName += "'" + GetNodeValue(trNode, "Type_Name") + "',";  //已勾選的每一筆部門
                if (i <= 4) {
                    BgnoName += GetNodeValue(trNode, "Type_Name") + "，";
                }
            }
            chkBgno = chkBgno.substr(0, chkBgno.length - 1)
            chkBgnoName = chkBgnoName.substr(0, chkBgnoName.length - 1)
            if (chkedRow > 5) {
                $('#lblBgnoCnt_SendSet').html(chkedRow)
                $('#lblBgnoName_SendSet').html(BgnoName.substr(0, BgnoName.length - 1) + '...')
            }
            else {
                $('#lblBgnoCnt_SendSet').html('')
                $('#lblBgnoName_SendSet').html(BgnoName.substr(0, BgnoName.length - 1))
            }
            $('#btLpOK_Bgno_SendSet').prop('disabled', false);
            $('#modal_Lookup_Bgno_SendSet').modal('hide');
            UpdateVIPCnt();
        }
    };

    let btLpExit_Bgno_SendSet_click = function (bt) {
        //Timerset();
        $('#modal_Lookup_Bgno_SendSet').modal('hide');
    };

    let btLpClear_Bgno_SendSet_click = function (bt) {
        //Timerset();
        $("#txtLpQ_Bgno_SendSet").val('');
        $("#tbLookup_Bgno_SendSet .checkbox").prop('checked', false);
    };
    //清除(發送設定)
    let btClear_SendSet_click = function (bt) {
        //Timerset();
        $('#lblDocNo_SendSet').html('');
        $('#lblEDMMemo_SendSet').html('');
        $('#lblEDDate_SendSet').html('');
        $('#lblVIPFaceIDCnt_SendSet').html('');
        $('#lblVIPFaceIDName_SendSet').html('');
        chkVIPFaceID = "";
        chkVIPFaceIDName = "";
        $('#lblCityCnt_SendSet').html('');
        $('#lblCityName_SendSet').html('');
        chkCity = "";
        $('#chk0_SendSet,#chk1_SendSet,#chk2_SendSet,#chk3_SendSet').prop('checked', true);
        $('#rdoMWAll_SendSet').prop('checked', true);
        $('#rdoQDayAll_SendSet').prop('checked', true);
        $('#txtQDayS_SendSet').val('');
        $('#txtQDayE_SendSet').val('');
        $('#rdoLCDayY_SendSet').prop('checked', true);
        $('#rdoLCDayAll_SendSet').prop('checked', true);
        $('#rdoSDateAll_SendSet').prop('checked', true);
        $('#lblDeptCnt_SendSet').html('');
        $('#lblDeptName_SendSet').html('');
        chkDept = "";
        chkDeptName = "";
        $('#lblBgnoCnt_SendSet').html('');
        $('#lblBgnoName_SendSet').html('');
        chkBgno = "";
        chkBgnoName = "";
        UpdateVIPCnt();
    };

    //顯示會員清單(發送設定)
    let btQuery_SendSet_click = function (bt) {
        //Timerset();
        $('#btQuery_SendSet').prop('disabled', true)
        if ($('#chk0_SendSet').prop('checked') == false && $('#chk1_SendSet').prop('checked') == false && $('#chk2_SendSet').prop('checked') == false && $('#chk3_SendSet').prop('checked') == false) {
            DyAlert("請選擇會員卡別!", function () {
                $('#btQuery_SendSet').prop('disabled', false)
            })
            return;
        }
        if ($('#txtQDayS_SendSet').val() == "" && $('#txtQDayE_SendSet').val() != "") {
            DyAlert("入會日期區間兩欄皆需輸入!", function () { $('#btQuery_SendSet').prop('disabled', false); })
            return;
        }
        else if ($('#txtQDayS_SendSet').val() != "" && $('#txtQDayE_SendSet').val() == "") {
            DyAlert("入會日期區間兩欄皆需輸入!", function () { $('#btQuery_SendSet').prop('disabled', false); })
            return;
        }
        else if ($('#txtQDayS_SendSet').val() != "" && $('#txtQDayE_SendSet').val() != "") {
            if ($('#txtQDayS_SendSet').val() > $('#txtQDayE_SendSet').val()) {
                DyAlert("入會日期開始日不可大於結束日!", function () { $('#btQuery_SendSet').prop('disabled', false); })
                return;
            }
        }
        
        ShowLoading();

        //會員卡別
        var VIP_Type = "";
        var VIP_TypeName = "";
        if ($('#chk0_SendSet').prop('checked') == true) {
            VIP_Type += "'0',";
            VIP_TypeName += "'一般',";
        }
        if ($('#chk1_SendSet').prop('checked') == true) {
            VIP_Type += "'1',";
            VIP_TypeName += "'會員',";
        }
        if ($('#chk2_SendSet').prop('checked') == true) {
            VIP_Type += "'2',";
            VIP_TypeName += "'貴賓',";
        }
        if ($('#chk3_SendSet').prop('checked') == true) {
            VIP_Type += "'3',";
            VIP_TypeName += "'白金卡',";
        }
        VIP_Type = VIP_Type.substr(0, VIP_Type.length - 1)
        VIP_TypeName = VIP_TypeName.substr(0, VIP_TypeName.length - 1)

        //會員性別
        var VIP_MW = "";
        if ($('#rdoMWAll_SendSet').prop('checked') == true) {
            VIP_MW = "";
        }
        else if ($('#rdoMW0_SendSet').prop('checked') == true) {
            VIP_MW = "0";
        }
        else if ($('#rdoMW1_SendSet').prop('checked') == true) {
            VIP_MW = "1";
        }

        //入會期間
        var QDay = ""
        if ($('#rdoQDayAll_SendSet').prop('checked') == true) {
            QDay = ""
        }
        else if ($('#rdoQDay2M_SendSet').prop('checked') == true) {
            QDay = "2M"
        }
        else if ($('#rdoQDay3M_SendSet').prop('checked') == true) {
            QDay = "3M"
        }
        else if ($('#rdoQDay6M_SendSet').prop('checked') == true) {
            QDay = "6M"
        }
        else if ($('#rdoQDay1Y_SendSet').prop('checked') == true) {
            QDay = "1Y"
        }


        var LCDayFlag = ""
        //最近有來店
        if ($('#rdoLCDayY_SendSet').prop('checked') == true) {
            LCDayFlag = "Y"
        }
        //最近沒來店
        else if ($('#rdoLCDayN_SendSet').prop('checked') == true) {
            LCDayFlag = "N"
        }
        var LCDay = ""
        if ($('#rdoLCDayAll_SendSet').prop('checked') == true) {
            LCDay = ""
        }
        else if ($('#rdoLCDay2W_SendSet').prop('checked') == true) {
            LCDay = "2W"
        }
        else if ($('#rdoLCDay1M_SendSet').prop('checked') == true) {
            LCDay = "1M"
        }
        else if ($('#rdoLCDay2M_SendSet').prop('checked') == true) {
            LCDay = "2M"
        }
        else if ($('#rdoLCDay3M_SendSet').prop('checked') == true) {
            LCDay = "3M"
        }
        else if ($('#rdoLCDay6M_SendSet').prop('checked') == true) {
            LCDay = "6M"
        }
        else if ($('#rdoLCDay1Y_SendSet').prop('checked') == true) {
            LCDay = "1Y"
        }
        else if ($('#rdoLCDay2Y_SendSet').prop('checked') == true) {
            LCDay = "2Y"
        }

        //消費月份
        var SDate = ""
        if ($('#rdoSDateAll_SendSet').prop('checked') == true) {
            SDate = ""
        }
        else if ($('#rdoSDate2M_SendSet').prop('checked') == true) {
            SDate = "2M"
        }
        else if ($('#rdoSDate3M_SendSet').prop('checked') == true) {
            SDate = "3M"
        }
        else if ($('#rdoSDate6M_SendSet').prop('checked') == true) {
            SDate = "6M"
        }
        else if ($('#rdoSDate1Y_SendSet').prop('checked') == true) {
            SDate = "1Y"
        }

        setTimeout(function () {
            var pData = {
                chkVIPFaceID: chkVIPFaceID,
                chkVIPFaceIDName: chkVIPFaceIDName,
                chkCity: chkCity,
                VIP_Type: VIP_Type,
                VIP_TypeName: VIP_TypeName,
                VIP_MW: VIP_MW,
                QDay: QDay,
                QDayS: $('#txtQDayS_SendSet').val().toString().replaceAll('-', '/'),
                QDayE: $('#txtQDayE_SendSet').val().toString().replaceAll('-', '/'),
                LCDayFlag: LCDayFlag,
                LCDay: LCDay,
                SDate: SDate,
                chkDept: chkDept,
                chkDeptName: chkDeptName,
                chkBgno: chkBgno,
                chkBgnoName: chkBgnoName,
                VMEVNO: $('#lblVMEVNO_SendSet').html(),
                Flag:"Q"
            }
            PostToWebApi({ url: "api/SystemSetup/MSVP101Query_SendSet", data: pData, success: afterMSVP101Query_SendSet });
        }, 1000);
    };

    let afterMSVP101Query_SendSet = function (data) {
        CloseLoading();

        if (ReturnMsg(data, 0) != "MSVP101Query_SendSetOK") {
            DyAlert(ReturnMsg(data, 1), function () { $('#btQuery_SendSet').prop('disabled', false); });
        }
        else {
            var dtE = data.getElementsByTagName('dtE');
            grdSendSet.BindData(dtE);
            if (dtE.length == 0) {
                $('#lblVIPCnt_SendSet').html('0 筆')
                $('#btDMSend_SendSet').prop('disabled', true);
                $('#btDMSend_SendSet').css('background-color', 'gray');
                DyAlert("無符合資料!", function () { $('#btQuery_SendSet').prop('disabled', false); });
                //$(".modal-backdrop").remove();
                return;
            }
            if ($('#lblDocNo_SendSet').html() == "") {
                $('#btDMSend_SendSet').prop('disabled', true);
                $('#btDMSend_SendSet').css('background-color', 'gray');
            }
            else {
                $('#btDMSend_SendSet').prop('disabled', false);
                $('#btDMSend_SendSet').css('background-color', '#3d94f6');
            }
            $('#lblVIPCnt_SendSet').html(dtE.length + ' 筆')
            $('#btQuery_SendSet').prop('disabled', false);
        }
    };

    //異動條件則更新會員筆數(發送設定)
    let UpdateVIPCnt = function () {
        //Timerset();
        if ($('#chk0_SendSet').prop('checked') == false && $('#chk1_SendSet').prop('checked') == false && $('#chk2_SendSet').prop('checked') == false && $('#chk3_SendSet').prop('checked') == false) {
            DyAlert("請選擇會員卡別!")
            return;
        }
        if ($('#txtQDayS_SendSet').val() != "" && $('#txtQDayE_SendSet').val() != "") {
            if ($('#txtQDayS_SendSet').val() > $('#txtQDayE_SendSet').val()) {
                DyAlert("入會日期開始日不可大於結束日!")
                return;
            }
        }
        //會員卡別
        var VIP_Type = "";
        var VIP_TypeName = "";
        if ($('#chk0_SendSet').prop('checked') == true) {
            VIP_Type += "'0',";
            VIP_TypeName += "'一般',"
        }
        if ($('#chk1_SendSet').prop('checked') == true) {
            VIP_Type += "'1',";
            VIP_TypeName += "'會員',"
        }
        if ($('#chk2_SendSet').prop('checked') == true) {
            VIP_Type += "'2',";
            VIP_TypeName += "'貴賓',"
        }
        if ($('#chk3_SendSet').prop('checked') == true) {
            VIP_Type += "'3',";
            VIP_TypeName += "'白金卡',"
        }
        VIP_Type = VIP_Type.substr(0, VIP_Type.length - 1)
        VIP_TypeName = VIP_TypeName.substr(0, VIP_TypeName.length - 1)

        //會員性別
        var VIP_MW = "";
        if ($('#rdoMWAll_SendSet').prop('checked') == true) {
            VIP_MW = "";
        }
        else if ($('#rdoMW0_SendSet').prop('checked') == true) {
            VIP_MW = "0";
        }
        else if ($('#rdoMW1_SendSet').prop('checked') == true) {
            VIP_MW = "1";
        }

        //入會期間
        var QDay = ""
        if ($('#rdoQDayAll_SendSet').prop('checked') == true) {
            QDay = ""
        }
        else if ($('#rdoQDay2M_SendSet').prop('checked') == true) {
            QDay = "2M"
        }
        else if ($('#rdoQDay3M_SendSet').prop('checked') == true) {
            QDay = "3M"
        }
        else if ($('#rdoQDay6M_SendSet').prop('checked') == true) {
            QDay = "6M"
        }
        else if ($('#rdoQDay1Y_SendSet').prop('checked') == true) {
            QDay = "1Y"
        }
        //入會日期
        var QDayS = ""
        var QDayE = ""
        if ($('#txtQDayS_SendSet').val() != "" && $('#txtQDayE_SendSet').val() != "") {
            QDayS = $('#txtQDayS_SendSet').val().toString().replaceAll('-', '/');
            QDayE = $('#txtQDayE_SendSet').val().toString().replaceAll('-', '/');
        }
        var LCDayFlag = ""
        //最近有來店
        if ($('#rdoLCDayY_SendSet').prop('checked') == true) {
            LCDayFlag = "Y"
        }
        //最近沒來店
        else if ($('#rdoLCDayN_SendSet').prop('checked') == true) {
            LCDayFlag = "N"
        }
        var LCDay = ""
        if ($('#rdoLCDayAll_SendSet').prop('checked') == true) {
            LCDay = ""
        }
        else if ($('#rdoLCDay2W_SendSet').prop('checked') == true) {
            LCDay = "2W"
        }
        else if ($('#rdoLCDay1M_SendSet').prop('checked') == true) {
            LCDay = "1M"
        }
        else if ($('#rdoLCDay2M_SendSet').prop('checked') == true) {
            LCDay = "2M"
        }
        else if ($('#rdoLCDay3M_SendSet').prop('checked') == true) {
            LCDay = "3M"
        }
        else if ($('#rdoLCDay6M_SendSet').prop('checked') == true) {
            LCDay = "6M"
        }
        else if ($('#rdoLCDay1Y_SendSet').prop('checked') == true) {
            LCDay = "1Y"
        }
        else if ($('#rdoLCDay2Y_SendSet').prop('checked') == true) {
            LCDay = "2Y"
        }

        //消費月份
        var SDate = ""
        if ($('#rdoSDateAll_SendSet').prop('checked') == true) {
            SDate = ""
        }
        else if ($('#rdoSDate2M_SendSet').prop('checked') == true) {
            SDate = "2M"
        }
        else if ($('#rdoSDate3M_SendSet').prop('checked') == true) {
            SDate = "3M"
        }
        else if ($('#rdoSDate6M_SendSet').prop('checked') == true) {
            SDate = "6M"
        }
        else if ($('#rdoSDate1Y_SendSet').prop('checked') == true) {
            SDate = "1Y"
        }

        setTimeout(function () {
            var pData = {
                chkVIPFaceID: chkVIPFaceID,
                chkVIPFaceIDName: chkVIPFaceIDName,
                chkCity: chkCity,
                VIP_Type: VIP_Type,
                VIP_TypeName: VIP_TypeName,
                VIP_MW: VIP_MW,
                QDay: QDay,
                QDayS: QDayS,
                QDayE: QDayE,
                LCDayFlag: LCDayFlag,
                LCDay: LCDay,
                SDate: SDate,
                chkDept: chkDept,
                chkDeptName: chkDeptName,
                chkBgno: chkBgno,
                chkBgnoName: chkBgnoName,
                VMEVNO: $('#lblVMEVNO_SendSet').html(),
                Flag: "C"
            }
            PostToWebApi({ url: "api/SystemSetup/MSVP101Query_SendSet", data: pData, success: afterUpdateVIPCnt });
        }, 500);
    };

    let afterUpdateVIPCnt = function (data) {
        if (ReturnMsg(data, 0) != "MSVP101Query_SendSetOK") {
            DyAlert(ReturnMsg(data, 1));
        }
        else {
            var dtE = data.getElementsByTagName('dtE');
            var dtC = data.getElementsByTagName('dtC');

            $('#btDMSend_SendSet').prop('disabled', true);
            $('#btDMSend_SendSet').css('background-color', 'gray');

            grdSendSet.BindData(dtE);
            if (dtC.length == 0) {
                $('#lblVIPCnt_SendSet').html('0 筆')
                return;
            }
            $('#lblVIPCnt_SendSet').html(parseInt(GetNodeValue(dtC[0], "VIPCnt")).toLocaleString('en-US') + ' 筆')
        }
    };

    //上一頁(發送設定)
    let btRe_SendSet_click = function (bt) {
        //Timerset();
        var pData = {
        }
        PostToWebApi({ url: "api/SystemSetup/MSVP102_ReSendSet", data: pData, success: afterMSVP101_ReSendSet });
    };

    let afterMSVP101_ReSendSet = function (data) {
        if (ReturnMsg(data, 0) != "MSVP102_ReSendSetOK") {
            DyAlert(ReturnMsg(data, 1));
        }
        else {
            $('#modal_SendSet').modal('hide');
        }
    };

    //DM選取(發送設定)
    let btDMSel_SendSet_click = function (bt) {
        //Timerset();
        var pData = {
        }
        PostToWebApi({ url: "api/SystemSetup/MSVP102_GetDM", data: pData, success: afterMSVP101_GetDM });
    };

    let afterMSVP101_GetDM = function (data) {
        if (ReturnMsg(data, 0) != "MSVP102_GetDMOK") {
            DyAlert(ReturnMsg(data, 1));
        }
        else {
            var dtE = data.getElementsByTagName('dtE');
            $('#modal_DMSel').modal('show');

            setTimeout(function () {
                grdDMSel.BindData(dtE);
                $('#tbDMSel tbody tr .tdCol9').filter(function () { return $(this).text() == $('#lblDocNo_SendSet').html(); }).closest('tr').find('.tdCol8 input:radio').prop('checked', true);
            }, 500);
        }
    };

    //發送EDM(發送設定)
    let btDMSend_SendSet_click = function (bt) {
        //Timerset();
        $('#btDMSend_SendSet').prop('disabled', true);
        if ($('#lblVIPCnt_SendSet').html() == "0 筆") {
            DyAlert("請先篩選會員資料!", function () { $('#btDMSend_SendSet').prop('disabled', false); })
            return;
        }
        if ($('#lblDocNo_SendSet').html() == "") {
            DyAlert("請選擇欲發送之DM!", function () { $('#btDMSend_SendSet').prop('disabled', false); })
            return;
        }
        DyConfirm("確定發送", function () {
            var pData = {
                VMDocNo: $('#lblVMEVNO_SendSet').html(),
                DMDocNo: $('#lblDocNo_SendSet').html(),
                EV_Model: "VP101"
            }
            PostToWebApi({ url: "api/SystemSetup/MSVP102_DMSend", data: pData, success: afterMSVP101_DMSend });
        }, function () { $('#btDMSend_SendSet').prop('disabled', false); }, $('#lblDocNo_SendSet').html() + $('#lblEDMMemo_SendSet').html(), "會員數 " + $('#lblVIPCnt_SendSet').html() + "?")
    };

    let afterMSVP101_DMSend = function (data) {
        if (ReturnMsg(data, 0) != "MSVP102_DMSendOK") {
            DyAlert(ReturnMsg(data, 1), function () { $('#btDMSend_SendSet').prop('disabled', false); });
        }
        else {
            DyAlert("發送EDM排程", function () {
                $('#btDMSend_SendSet').prop('disabled', false);
                $('#modal_SendSet').modal('hide');
            }, "已成功送出")
        }
    };

    //DM預覽(發送設定)
    let btShowEDM_SendSet_click = function (bt) {
        //Timerset();
        if ($('#lblDocNo_SendSet').html() == "") {
            DyAlert("請選取DM!")
            return;
        }
        var pData = {
        }
        PostToWebApi({ url: "api/SystemSetup/GetCompanyShowEDM", data: pData, success: afterShowEDM_SendSet });
    };

    let afterShowEDM_SendSet = function (data) {
        if (ReturnMsg(data, 0) != "GetCompanyShowEDMOK") {
            DyAlert(ReturnMsg(data, 1));
        }
        else {
            var dtE = data.getElementsByTagName('dtE');
            var hostname = location.hostname;
            //測試環境
            if (hostname.indexOf("94") >= 0 || hostname.indexOf("localhost") >= 0) {
                window.open("http://192.168.1.94/ShowEDMWEB/ShowEDMWEB?company=" + GetNodeValue(dtE[0], "CompanyID") + ";" + $('#lblDocNo_SendSet').html() + "");
            }
            //正式環境
            else {
                window.open("https://www.portal.e-dynasty.com.tw/ShowEDMWEB/ShowEDMWEB?company=" + GetNodeValue(dtE[0], "CompanyID") + ";" + $('#lblDocNo_SendSet').html() + "");
            }
        }
    };


    //上一頁(DM選取)
    let btRe_DMSel_click = function (bt) {
        //Timerset();
        $('#modal_DMSel').modal('hide');
    };

    //確定(DM選取)
    let btOK_DMSel_click = function (bt) {
        //Timerset();
        $('#btOK_DMSel').prop('disabled', true);
        var obchkedtd = $('#tbDMSel input[type="radio"]:checked');
        var chkedRow = obchkedtd.length.toString();

        if (chkedRow == 0) {
            DyAlert("未選取DM單號，請重新確認!", function () {
                $('#btOK_DMSel').prop('disabled', false);
            });
        }
        else {
            var pData = {
                VMDocNo: $('#lblVMEVNO_SendSet').html()
            }
            PostToWebApi({ url: "api/SystemSetup/MSVP101ChkDMSend", data: pData, success: afterMSVP101ChkDMSend });

        }
    };

    let afterMSVP101ChkDMSend = function (data) {
        if (ReturnMsg(data, 0) != "MSVP101ChkDMSendOK") {
            DyAlert(ReturnMsg(data, 1), function () {
                $('#btOK_DMSel').prop('disabled', false);
            });
        }
        else {
            var dtE = data.getElementsByTagName('dtE');
            if (dtE.length == 0) {
                $('#btDMSend_SendSet').prop('disabled', true);
                $('#btDMSend_SendSet').css('background-color', 'gray');
            }
            else {
                $('#btDMSend_SendSet').prop('disabled', false);
                $('#btDMSend_SendSet').css('background-color', '#3d94f6');
            }

            var obchkedtd = $('#tbDMSel input[type="radio"]:checked');
            var a = $(obchkedtd[0]).closest('tr');
            var trNode = $(a).prop('Record');
            $('#lblDocNo_SendSet').html(GetNodeValue(trNode, "DocNo"))
            $('#lblEDMMemo_SendSet').html(GetNodeValue(trNode, "EDMMemo"))
            $('#lblEDDate_SendSet').html(GetNodeValue(trNode, "EDDate1"))
            $('#btOK_DMSel').prop('disabled', false);
            $('#modal_DMSel').modal('hide')
        }
    };

    //上一頁(歷史查詢)
    let btRe_EDMHistoryQuery_click = function (bt) {
        //Timerset();
       
        $('#modal_EDMHistoryQuery').modal('hide');
    };

    //DM預覽(歷史查詢)
    let btShowEDM_EDMHistoryQuery_click = function (bt) {
        //Timerset();
        var pData = {
        }
        PostToWebApi({ url: "api/SystemSetup/GetCompanyShowEDM", data: pData, success: afterShowEDM_EDMHistoryQuery });
    };

    let afterShowEDM_EDMHistoryQuery = function (data) {
        if (ReturnMsg(data, 0) != "GetCompanyShowEDMOK") {
            DyAlert(ReturnMsg(data, 1));
        }
        else {
            var dtE = data.getElementsByTagName('dtE');
            var hostname = location.hostname;
            //測試環境
            if (hostname.indexOf("94") >= 0 || hostname.indexOf("localhost") >= 0) {
                window.open("http://192.168.1.94/ShowEDMWEB/ShowEDMWEB?company=" + GetNodeValue(dtE[0], "CompanyID") + ";" + $('#lblEDMDocNo_EDMHistoryQuery').html() + "");
            }
            //正式環境
            else {
                window.open("https://www.portal.e-dynasty.com.tw/ShowEDMWEB/ShowEDMWEB?company=" + GetNodeValue(dtE[0], "CompanyID") + ";" + $('#lblEDMDocNo_EDMHistoryQuery').html() + "");
            }
        }
    };

    let ClearQuery = function () {
        grdM.BindData(null)
    }
    //#region FormLoad
    let GetInitMSVP101 = function (data) {
        if (ReturnMsg(data, 0) != "GetInitmsDMOK") {
            DyAlert(ReturnMsg(data, 1));
        }
        else {
            var dtE = data.getElementsByTagName('dtE');
            if (dtE.length > 0) {
                $('#lblProgramName').html(GetNodeValue(dtE[0], "ChineseName"));
            }
            AssignVar();
            $('#btQuery').click(function () { btQuery_click(this) });
            $('#btClear').click(function () { btClear_click(this) });
            $('#btSendSet').click(function () { btSendSet_click(this) });
            $('#txtEVNO,#txtEDM_DocNo,#txtEDMMemo').keydown(function () { ClearQuery() })
            $('#txtStartDate').change(function () { ClearQuery() })

            $('#btRe_EDMHistoryQuery').click(function () { btRe_EDMHistoryQuery_click(this) });
            $('#btShowEDM_EDMHistoryQuery').click(function () { btShowEDM_EDMHistoryQuery_click(this) });

            $('#btClear_SendSet').click(function () { btClear_SendSet_click(this) });
            $('#btRe_SendSet').click(function () { btRe_SendSet_click(this) });
            $('#btShowEDM_SendSet').click(function () { btShowEDM_SendSet_click(this) });
            $('#btQuery_SendSet').click(function () { btQuery_SendSet_click(this) });
            $('#btDMSel_SendSet').click(function () { btDMSel_SendSet_click(this) });
            $('#btDMSend_SendSet').click(function () { btDMSend_SendSet_click(this) });
            $('#chk0_SendSet,#chk1_SendSet,#chk2_SendSet,#chk3_SendSet').change(function () { UpdateVIPCnt(); });
            $('#rdoMWAll_SendSet,#rdoMW0_SendSet,#rdoMW1_SendSet').change(function () { UpdateVIPCnt(); });
            $('#rdoQDayAll_SendSet,#rdoQDay2M_SendSet,#rdoQDay3M_SendSet,#rdoQDay6M_SendSet,#rdoQDay1Y_SendSet').change(function () { UpdateVIPCnt(); });
            $('#txtQDayS_SendSet,#txtQDayE_SendSet').change(function () { UpdateVIPCnt(); });

            $('#rdoLCDayY_SendSet,#rdoLCDayN_SendSet,#rdoLCDayAll_SendSet,#rdoLCDay2W_SendSet,#rdoLCDay1M_SendSet,#rdoLCDay2M_SendSet,#rdoLCDay3M_SendSet,#rdoLCDay6M_SendSet,#rdoLCDay1Y_SendSet,#rdoLCDay2Y_SendSet').change(function () { UpdateVIPCnt(); });

            $('#rdoSDateAll_SendSet,#rdoSDate2M_SendSet,#rdoSDate3M_SendSet,#rdoSDate6M_SendSet,#rdoSDate1Y_SendSet').change(function () { UpdateVIPCnt(); });

            $('#btVIPFaceID_SendSet').click(function () { btVIPFaceID_SendSet_click(this) });
            $('#btLpQ_VIPFaceID_SendSet').click(function () { btLpQ_VIPFaceID_SendSet_click(this) });
            $('#btLpOK_VIPFaceID_SendSet').click(function () { btLpOK_VIPFaceID_SendSet_click(this) });
            $('#btLpExit_VIPFaceID_SendSet').click(function () { btLpExit_VIPFaceID_SendSet_click(this) });
            $('#btLpClear_VIPFaceID_SendSet').click(function () { btLpClear_VIPFaceID_SendSet_click(this) });
           
            $('#btCity_SendSet').click(function () { btCity_SendSet_click(this) });
            $('#btLpQ_City_SendSet').click(function () { btLpQ_City_SendSet_click(this) });
            $('#btLpOK_City_SendSet').click(function () { btLpOK_City_SendSet_click(this) });
            $('#btLpExit_City_SendSet').click(function () { btLpExit_City_SendSet_click(this) });
            $('#btLpClear_City_SendSet').click(function () { btLpClear_City_SendSet_click(this) });

            $('#btDept_SendSet').click(function () { btDept_SendSet_click(this) });
            $('#btLpQ_Dept_SendSet').click(function () { btLpQ_Dept_SendSet_click(this) });
            $('#btLpOK_Dept_SendSet').click(function () { btLpOK_Dept_SendSet_click(this) });
            $('#btLpExit_Dept_SendSet').click(function () { btLpExit_Dept_SendSet_click(this) });
            $('#btLpClear_Dept_SendSet').click(function () { btLpClear_Dept_SendSet_click(this) });

            $('#btBgno_SendSet').click(function () { btBgno_SendSet_click(this) });
            $('#btLpQ_Bgno_SendSet').click(function () { btLpQ_Bgno_SendSet_click(this) });
            $('#btLpOK_Bgno_SendSet').click(function () { btLpOK_Bgno_SendSet_click(this) });
            $('#btLpExit_Bgno_SendSet').click(function () { btLpExit_Bgno_SendSet_click(this) });
            $('#btLpClear_Bgno_SendSet').click(function () { btLpClear_Bgno_SendSet_click(this) });

            $('#btOK_DMSel').click(function () { btOK_DMSel_click(this) });
            $('#btRe_DMSel').click(function () { btRe_DMSel_click(this) });
            btQuery_click();
        }
    };
    
    let afterLoadPage = function () {
        var pData = {
            ProgramID: "MSVP101"
        }
        PostToWebApi({ url: "api/SystemSetup/GetInitmsDM", data: pData, success: GetInitMSVP101 });
    };
//#endregion
    

    if ($('#pgMSVP101').length == 0) {  
        AllPages = new LoadAllPages(ParentNode, "SystemSetup/MSVP101", ["MSVP101btns", "pgMSVP101Init", "pgMSVP101Add", "pgMSVP101Mod"], afterLoadPage);
    };

    
}