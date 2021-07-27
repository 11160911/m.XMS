var PageVIN14_5 = function (ParentNode) {
    let AllPages;
    let grdU;
    let EditMode;
    let SetSuspend = "";
    let isImport = false;
    let SysDate ;
    let gDocNo = "";
    let gWhNo = "";
    let ST_StopDay = '';


    let AssignVar = function () {

        console.log("AssignVar");

        grdU = new DynGrid(
            {
                //2021-04-27
                table_lement: $('#tbVIN14_5')[0],
                class_collection: ["tdColbt icon_in_td btEdit", "tdCol2", "tdCol3", "tdCol4", "tdCol5", "tdCol6", "tdCol7", "tdCol8", "tdCol9", "tdCol10"],
                //class_collection: ["tdColbt icon_in_td", "tdColbt icon_in_td btsuspend", "tdCol3", "tdCol4", "tdCol5", "tdCol6", "tdCol7",  "tdCol8"],
                fields_info: [
                    { type: "JQ", name: "fa-tags", element: '<i class="fa fa-tags"></i>' },
                    { type: "Text", name: "WhName" },
                    { type: "Text", name: "CkNo" },
                    { type: "Text", name: "LayerSno" },
                    { type: "Text", name: "OldName" },

                    { type: "Text", name: "PtNum" },
                    { type: "Text", name: "NewName" },
                    { type: "Text", name: "Num" },
                    { type: "Text", name: "ExchangeDate" },
                    { type: "Text", name: "FinStatus" }
                ],
                rows_per_page: 10,
                method_clickrow: click_PLU,
                afterBind: InitModifyDeleteButton,
                //sortable: "Y"
            }
        );

    };


    let InitModifyDeleteButton = function () {
        $('#tbVIN14_5 .fa-tags').click(function () { btMod_Click(this) });
     }


    let btMod_Click = function (bt) {
        EditMode = "Mod";
        $(bt).closest('tr').click();
        $('.msg-valid').hide();
        $('#modal_VIN14_5 .modal-title').text('智販機換貨作業');
        var node = $(grdU.ActiveRowTR()).prop('Record');

        if (GetNodeValue(node, 'FinishDate') != '') {
            DyAlert("此單據已完成，無法操作!");
            return
        }
        gWhNo = GetNodeValue(node, 'WhNo');


        gDocNo = GetNodeValue(node, 'DocNo');

        //$('#WhNo').val(GetNodeValue(node, 'WhNo'));
        //$('#WhNo').closest('.col-3').hide();
        $('#lblWhNo').text(GetNodeValue(node, 'WhNo'));
        $('#lblWhNo').closest('.col-3').hide();
        $('#WhName').text(GetNodeValue(node, 'WhName'));

        //$('#CkNo').closest('.col-2').hide();
        //$('#CkNo').val(GetNodeValue(node, 'CkNo'));
        $('#lblCkNo').text(GetNodeValue(node, 'CkNo'));
        $('#lblCkNo').closest('.col-3').show();

        $('#Layer').closest('.col-2').hide();
        $('#lblLayer').text(GetNodeValue(node, 'Layer'));
        $('#lblLayer').closest('.col-3').show();

        $('#Sno').closest('.col-2').hide();
        $('#lblSno').text(GetNodeValue(node, 'Sno'));
        $('#lblSno').closest('.col-3').show();

        //$('#OldPLU').text(GetNodeValue(node, 'PLUOld') + ' ' + GetNodeValue(node, 'OldName'));
        $('#lblOldPLU').text(GetNodeValue(node, 'PLUOld'));
        //$('#lblOldPLU').closest('.col-3').hide();
        $('#OldPLUName').text(GetNodeValue(node, 'OldName'));

        //$('#NewPLU').closest('.col-4').show();
        //$('#NewPLU').val(GetNodeValue(node, 'PLUNew'));
        //$('#lblNewPLU').closest('.col-2').show();
        $('#lblNewPLU').text(GetNodeValue(node, 'PLUNew'));
        $('#NewPLUName').text(GetNodeValue(node, 'NewName'));

        //$('#Num').closest('.col-3').show();
        $('#Num').val(GetNodeValue(node, 'Num'));
        //$('#lblNum').closest('.col-2').show();
        $('#lblNum').text(GetNodeValue(node, 'Num'));

        //$('#PtNum').closest('.col-3').show();
        $('#PtNum').val(GetNodeValue(node, 'PtNum'));
        //$('#lblPtNum').closest('.col-2').hide();
        $('#lblPtNum').text(GetNodeValue(node, 'PtNum'));

        //$('#ExchangeDate').val(GetNodeValue(node, 'ExchangeDate'));
        //$('#ExchangeDate').closest('.col-4').show();
        $('#lblExDate').text(GetNodeValue(node, 'ExchangeDate'));
        //$('#lblExDate').closest('.col-3').hide();
        
        var FlagUse = "";
        FlagUse = GetNodeValue(node, 'FlagUse');

        if (FlagUse == 'U' | FlagUse == 'M') {
  
        }
        else {
            DyAlert("此智販機不為使用中狀態，無法操作!");
            return
        }

        ST_StopDay = GetNodeValue(node, 'ST_StopDay');

        GetSysDate();

    };



    //let ShowModal = function (bt) {
    //    EditMode = "Mod";
    //    $(bt).closest('tr').click();
    //    $('.msg-valid').hide();
    //    $('#modal_VIN14_5 .modal-title').text('智販機換貨作業');
    //    var node = $(grdU.ActiveRowTR()).prop('Record');

    //    if (GetNodeValue(node, 'FinishDate') != '') {
    //        DyAlert("此單據已完成，無法操作!");
    //        return
    //    }

    //    if (GetNodeValue(node, 'GD_Flag1') != '1') {
    //        DyAlert("新商品狀態不為啟用，無法操作!");
    //        return
    //    }

    //    gWhNo = GetNodeValue(node, 'WhNo');

    //    //GetSysDate();

    //    //if (GetNodeValue(node, 'DefeasanceDate') != '') {
    //    //    DyAlert("此單據已作廢，無法操作!");
    //    //    return
    //    //}
    //    //if (GetNodeValue(node, 'AppDate') != '') {
    //    //    DyAlert("此單據已批核，無法操作!");
    //    //    return
    //    //}

    //    gDocNo = GetNodeValue(node, 'DocNo');

    //    //$('#WhNo').val(GetNodeValue(node, 'WhNo'));
    //    //$('#WhNo').closest('.col-3').hide();
    //    $('#lblWhNo').text(GetNodeValue(node, 'WhName'));
    //    //$('#lblWhNo').closest('.col-3').show();
    //    //$('#WhName').text(GetNodeValue(node, 'WhName'));

    //    $('#CkNo').closest('.col-2').hide();
    //    $('#CkNo').val(GetNodeValue(node, 'CkNo'));
    //    $('#lblCkNo').text(GetNodeValue(node, 'CkNo'));
    //    $('#lblCkNo').closest('.col-3').show();

    //    $('#Layer').closest('.col-2').hide();
    //    $('#lblLayer').text(GetNodeValue(node, 'Layer'));
    //    $('#lblLayer').closest('.col-3').show();

    //    $('#Sno').closest('.col-2').hide();
    //    $('#lblSno').text(GetNodeValue(node, 'Sno'));
    //    $('#lblSno').closest('.col-3').show();

    //    //$('#OldPLU').text(GetNodeValue(node, 'PLUOld') + ' ' + GetNodeValue(node, 'OldName'));
    //    $('#lblOldPLU').text(GetNodeValue(node, 'PLUOld'));
    //    //$('#lblOldPLU').closest('.col-3').hide();
    //    $('#OldPLUName').text(GetNodeValue(node, 'OldName'));

    //    //$('#NewPLU').closest('.col-4').show();
    //    //$('#NewPLU').val(GetNodeValue(node, 'PLUNew'));
    //    //$('#lblNewPLU').closest('.col-2').show();
    //    $('#lblNewPLU').text(GetNodeValue(node, 'PLUNew'));
    //    $('#NewPLUName').text(GetNodeValue(node, 'NewName'));

    //    //$('#Num').closest('.col-3').show();
    //    $('#Num').val(GetNodeValue(node, 'Num'));
    //    //$('#lblNum').closest('.col-2').show();
    //    $('#lblNum').text(GetNodeValue(node, 'Num'));

    //    //$('#PtNum').closest('.col-3').show();
    //    $('#PtNum').val(GetNodeValue(node, 'PtNum'));
    //    //$('#lblPtNum').closest('.col-2').hide();
    //    $('#lblPtNum').text(GetNodeValue(node, 'PtNum'));

    //    //$('#ExchangeDate').val(GetNodeValue(node, 'ExchangeDate'));
    //    //$('#ExchangeDate').closest('.col-4').show();
    //    $('#lblExDate').text(GetNodeValue(node, 'ExchangeDate'));
    //    //$('#lblExDate').closest('.col-3').hide();
    //    //GetSysDate();


    //    ST_StopDay = GetNodeValue(node, 'ST_StopDay');
    //    //DyAlert(SysDate);

    //    //if (ST_StopDay > SysDate | ST_StopDay == '') {

    //    //}
    //    //else {
    //    //    DyAlert("該店該機已停用!!");
    //    //}

    //    CheckData();
    //    //DyConfirm("確定要執行換店作業嗎?", CheckData, DummyFunction);

    //    //$('#modal_VIN14_5').modal('show');
    //};


    let CheckData = function () {
        //alert(gWhNo);
        var pData = {
            WhNo: gWhNo
        }
        PostToWebApi({ url: "api/SystemSetup/ChkChangeShop", data: pData, success: AfterCheckData });
    }


    let AfterCheckData = function (data) {
        //alert("AfterCheckData");
        if (ReturnMsg(data, 0) != "ChkChangeShopOK") {
            DyAlert(ReturnMsg(data, 0));
            return;
        }
        else {

            var ChkRes = "";

            var dtChkTF = data.getElementsByTagName('dtChkTF');
            var dtChkUseless = data.getElementsByTagName('dtChkUseless');
            var dtChkAdj = data.getElementsByTagName('dtChkAdj');

            if (dtChkTF.length != 0) {
                ChkRes += "本單該店有調撥單尚未過帳!! ";
            }

            if (dtChkUseless.length != 0) {
                ChkRes += "本單該店有報廢單尚未過帳!! ";
            }
            //alert(dtChkAdj.length);
            if (dtChkAdj.length != 0) {
                ChkRes += "本單該店有調整單尚未過帳!! ";
            }

            if (ChkRes != "") {
                DyAlert(ChkRes);
                return
            }
            else {
                $('#modal_VIN14_5').modal('show');
            }

        }
    };



    let SearchVIN14_5 = function () {

        console.log("SearchVIN14_5");

        var pData = {
            WhNo: $('#cbWh').val(),
            CkNo: $('#cbCK').val(),
            Layer: $('#cbLayer').val(),
            exDate: $('#exDate').val()
        };
        PostToWebApi({ url: "api/SystemSetup/SearchVIN14_5", data: pData, success: AfterSearchVIN14_5 });
    };


    let click_PLU = function (tr) {

    };


    let AfterSearchVIN14_5 = function (data) {
        if (ReturnMsg(data, 0) != "SearchVIN14_5OK") {
            DyAlert(ReturnMsg(data, 0));
            return;
        }
        else {
            var dtInv = data.getElementsByTagName('dtInv');
            //alert(dtInv.length);
            grdU.BindData(dtInv);
            if (dtInv.length == 0) {
                DyAlert("無符合資料!", BlankMode);
                return;
            }
        }
    };


    let BlankMode = function () {
        
    };


    let btCancel_click = function () {
        $('#modal_VIN14_5').modal('hide');
    };

 
    //2021-05-07
    let cbCK_click = function () {

        if ($('#cbWh').val() == "") {
            $('#cbCK').val() == ""
            DyAlert("請先選擇店查詢條件!!", function () { $('#cbWh').focus() });
            //DyAlert("請先選擇店查詢條件!!");
            return;
        }
    };


    let cbLayer_click = function () {

        if ($('#cbCK').val() == "") {
            $('#cbLayer').val() == ""
            DyAlert("請先選擇機號查詢條件!!", function () { $('#cbCK').focus() });
            //DyAlert("請先選擇店查詢條件!!");
            return;
        }
    };


    let GetLayerNo = function () {

        if ($('#cbCK').val() == "") {
            $('#cbLayer').empty();
            return;
        }

        var pData = {
            WhNo: $('#cbWh').val(),
            CkNo: $('#cbCK').val()
        };
        PostToWebApi({ url: "api/SystemSetup/GetWhCkLayer", data: pData, success: AfterGetLayerNo });
    };


    let AfterGetLayerNo = function (data) {
        //alert("AfterGetLayerNo");
        if (ReturnMsg(data, 0) != "GetWhCkLayerOK") {
            DyAlert(ReturnMsg(data, 0));
            return;
        }
        else {
            var dtCK = data.getElementsByTagName('dtCK');
            InitSelectItem($('#cbLayer')[0], dtCK, "LayerNo", "LayerNo", true);
        }
    };


    let GetWhDSVCkNo = function () {

        console.log("GetWhDSVCkNo");

        if ($('#cbWh').val() == "") {
            $('#cbCK').empty();
            return;
        }

        var pData = {
            WhNo: $('#cbWh').val(),
            StopDay: 'Y',
            CheckUse: 'Y'
        };
        PostToWebApi({ url: "api/SystemSetup/GetWhDSVCkNoWithCond", data: pData, success: AfterGetWhDSVCkNo });
    };


    let AfterGetWhDSVCkNo = function (data) {
        //alert("AfterGetWhDSVCkNo");
        if (ReturnMsg(data, 0) != "GetWhDSVCkNoWithCondOK") {
            DyAlert(ReturnMsg(data, 0));
            return;
        }
        else {
            var dtCK = data.getElementsByTagName('dtCK');
            InitSelectItem($('#cbCK')[0], dtCK, "CKNo", "CKNo", true);
        }
    };


 
    let GetSysDate = function () {

        var cData = {
            DocNo: $('#DocNo').val()
        }
        PostToWebApi({ url: "api/SystemSetup/GetSysDate", data: cData, success: AfterGetSysDate });
   
        //return;
    };


    let AfterGetSysDate = function (data) {
        //alert("AfterGetSysDate");
        if (ReturnMsg(data, 0) != "GetSysDateOK") {
            DyAlert(ReturnMsg(data, 0));
            return;
        }
        else {
            var dtSysDate = data.getElementsByTagName('dtSysDate');
            
            if (dtSysDate.length == 0) {

            }
            else {
                SysDate = GetNodeValue(dtSysDate[0], "SysDate");

                if (ST_StopDay > SysDate | ST_StopDay == '') {
                    
                }
                else {
                    alert("該店該機已停用!!");
                }

                CheckData();

            }
        }
    };



    let SaveData = function () {
 
            var pData = {
                ChangePLUSV: [
                    {
                        DocNo: gDocNo,
                        WhNo: $('#lblWhNo').text(),
                        CkNo: $('#lblCkNo').text(),
                        Layer: $('#lblLayer').text(),
                        Sno: $('#lblSno').text(),
                        OldPLU: $('#lblOldPLU').text(),
                        PtNum: $('#PtNum').val(),
                        NewPLU: $('#lblNewPLU').text(),
                        Num: $('#Num').val(),
                        ExpDate: $('#ExpDate').val()
                    }
                ]
            }
  
        PostToWebApi({ url: "api/SystemSetup/SaveVIN14_5", data: pData, success: AfterUpdateChgPLU });

    }


    let btSave_click = function () {

        //alert("SysDate " + SysDate);
        
        //if ($('#WhNo').val() == "" | $('#WhNo').val() == null | $('#CkNo').val() == "" | $('#CkNo').val() == null | $('#WhNoIn').val() == "" | $('#WhNoIn').val() == null | $('#CkNoIn').val() == "" | $('#CkNoIn').val() == null | $('#ExchangeDate').val() == "" | $('#ExchangeDate').val() == null) {
        //    DyAlert("所有欄位都必須輸入資料!!", function () { $('#WhNo').focus() });
        //    //DyAlert("所有欄位都必須輸入資料!!", setFocus);
        //    return;
        //}
        //if ($('#WhNo').val() == "" | $('#WhNo').val() == null) {
        //    DyAlert("智販店代號欄位必須輸入資料!!", function () { $('#WhNo').focus() });
        //    return;
        //}

        //if (EditMode == "Add") {
        //    if ($('#CkNo').val() == "" | $('#CkNo').val() == null) {
        //        DyAlert("機號欄位必須輸入資料!!", function () { $('#CkNo').focus() });
        //        return;
        //    }
        //    if ($('#Layer').val() == "" | $('#Layer').val() == null) {
        //        DyAlert("貨倉欄位必須輸入資料!!", function () { $('#Layer').focus() });
        //        return;
        //    }
        //    if ($('#Sno').val() == "" | $('#Sno').val() == null) {
        //        DyAlert("貨道欄位必須輸入資料!!", function () { $('#Sno').focus() });
        //        return;
        //    }
        //}


        //if ($('#NewPLU').val() == "" | $('#NewPLU').val() == null) {
        //    DyAlert("新品號欄位必須輸入資料!!", function () { $('#NewPLU').focus() });
        //    return;
        //}

        if ($('#PtNum').val() == "" | $('#PtNum').val() == null) {
            DyAlert("實際退貨量欄位必須輸入資料!!", function () { $('#PtNum').focus() });
            return;
        }
        else {
            if ($('#PtNum').val() == 0) {
                DyAlert("實際退貨量必須大於0!!", function () { $('#PtNum').focus() });
                return;
            }
            if ($('#PtNum').val() != $('#lblPtNum').text() ) {
                DyAlert("實際退貨量必須等於原商品庫存量!!", function () { $('#PtNum').focus() });
                return;
            }
        }

        if ($('#Num').val() == "" | $('#Num').val() == null) {
            DyAlert("實際補貨量欄位必須輸入資料!!", function () { $('#Num').focus() });
            return;
        }
        else {
            if ($('#Num').val() == 0) {
                DyAlert("實際補貨量必須大於0!!", function () { $('#Num').focus() });
                return;
            }
            if ($('#Num').val() > $('#lblNum').text()) {
                DyAlert("實際補貨量不可大於新補貨量!!", function () { $('#Num').focus() });
                return;
            }
        }

        if ($('#ExpDate').val() == "" | $('#ExpDate').val() == null) {
            DyAlert("新最近有效日欄位必須輸入資料!!", function () { $('#ExpDate').focus() });
            return;
        }

       // //檢查換貨日期必須大於系統日
       //if ($('#ExchangeDate').val() < SysDate ) {
       //    DyAlert("換貨日期必須大於等於系統日期!!", function () { $('#ExchangeDate').focus() });
       //     return;
       // }

        

        //alert("EditMode:" + EditMode);
        if (EditMode == "App") {
            DyConfirm("確定要批核這筆資料嗎?", SaveData, DummyFunction);
        }
        else if (EditMode == "Def") {
            DyConfirm("確定要作廢這筆資料嗎?", SaveData, DummyFunction);
        }
        else if (EditMode == "Del") {
            DyConfirm("確定要刪除這筆資料嗎?", SaveData, DummyFunction);
        }
        else {
            //alert("EditMode:" + EditMode);
            SaveData();
        }

    };


 



    //let AfterAddChgPLU = function (data) {
    //    if (ReturnMsg(data, 0) != "AddChgPLUOK") {
    //        DyAlert(ReturnMsg(data, 1));
    //    }
    //    else {
    //        DyAlert("新增完成!");

    //        $('#modal_VIN14_5').modal('hide');
    //        var userxml = data.getElementsByTagName('dtChgPLU')[0];
    //        grdU.AddNew(userxml);
    //    }
    //};


    let AfterUpdateChgPLU = function (data) {
        //alert("AfterUpdateChgPLU:" + EditMode);
        //alert(ReturnMsg(data, 0));
        if (ReturnMsg(data, 0) != "SaveVIN14_5OK") {
            DyAlert(ReturnMsg(data, 1));
        }
        else {

            if (EditMode == "App") {
                DyAlert("批核完成!");
            }
            else if (EditMode == "Def") {
                DyAlert("作廢完成!");
            }
            else {
                DyAlert("儲存完成!");
            }


            $('#modal_VIN14_5').modal('hide');
            var userxml = data.getElementsByTagName('dtRes')[0];
            grdU.RefreshRocord(grdU.ActiveRowTR(), userxml);
        }
    };

 

    //2021-05-18
    let afterGetInitVIN14_5 = function (data) {

        //alert("afterGetInitVIN14_5");
        AssignVar();
        $('#btQuery').click(function () { SearchVIN14_5(); });

        var dtWh = data.getElementsByTagName('dtWh');
        InitSelectItem($('#cbWh')[0], dtWh, "ST_ID", "STName", true);

        $('#cbWh').change(function () { GetWhDSVCkNo(); });
        $('#cbCK').click(function () { cbCK_click(); });
        $('#cbCK').change(function () { GetLayerNo(); });
        $('#cbLayer').click(function () { cbLayer_click(); });


        SetDateField($('#ExpDate')[0]);
        $('#ExpDate').datepicker();

        SetDateField($('#exDate')[0]);
        $('#exDate').datepicker();

        //$('#btAdd').click(function () { btAdd_click(); });
        $('#btSave').click(function () { btSave_click(); });
        $('#btCancel').click(function () { btCancel_click(); });
        $('.forminput input').change(function () { InputValidation(this) });


        //SetPLUAutoComplete("GD_NAME");
        //SetPLUAutoComplete("GD_NO");
    };



    let InputValidation = function (ip) {
        var str = $(ip).val();
        var msg = "";
        //$('.forminput .msg-valid').text('');
        //$('.forminput .msg-valid').hide();
        $(ip).nextAll('.msg-valid').text(msg);
        $(ip).nextAll('.msg-valid').show();
        if (str == "")
            return;
        if ($(ip).attr('id') == "WhNo") {
            var re = /^[\d|a-zA-Z]+$/;
            if (!re.test(str) | str.length < 5 | str.length > 10)
                msg = "必須5~10碼英數字";
        }
        if ($(ip).attr('id') == "Num") {
            var re = /^[\d]+$/;
            if (!re.test(str) | str.length > 2 )
                msg = "必須為個位或十位數字";
            if (Number(str) > $('#lblNum').text())
                msg = "數量超過新補貨量(滿倉量)";
        }
        if ($(ip).attr('id') == "PtNum") {
            var re = /^[\d]+$/;
            if (!re.test(str) | str.length > 2)
                msg = "必須為個位或十位數字";
            if (Number(str) > $('#lblPtNum').text())
                msg = "數量超過原商品庫存量";
        }
        if ($(ip).attr('id') == "USR_PWD") {
            var re = /^[\d|a-zA-Z]+$/;
            if (!re.test(str) | str.length < 6 | str.length > 20)
                msg = "必須6~20碼英數字";
        }
        if ($(ip).attr('id') == "USR_NAME_L") {
            if (str.length > 10)
                msg = "必須10字元以內";
        }
        if ($(ip).attr('id') == "USR_EMPNO") {
            if (str.length > 10)
                msg = "必須10字元以內";
        }
        if ($(ip).attr('id') == "USR_MAIL") {
            var re = /^(([^<>()[\]\\.,;:\s@\"]+(\.[^<>()[\]\\.,;:\s@\"]+)*)|(\".+\"))@((\[[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\])|(([a-zA-Z\-0-9]+\.)+[a-zA-Z]{2,}))$/;
            if (!re.test(str))
                msg = "e-mail格式錯誤!";
        }
        if ($(ip).attr('id') == "USR_MOBILE") {
            var re = /^09\d{8}$/;
            if (!re.test(str))
                msg = "手機格式錯誤!";
        }
        if ($(ip).attr('id') == "USR_NOTE") {
            if (str.length > 50)
                msg = "必須50字元以內";
        }
        if (msg != "") {
            //$(ip).val('');
            $(ip).nextAll('.msg-valid').text(msg);
            $(ip).nextAll('.msg-valid').show();
        }
    }

    let afterLoadPage = function () {
        PostToWebApi({ url: "api/SystemSetup/GetWh", success: afterGetInitVIN14_5 });
        $('#pgVIN14_5').show();
    };

    if ($('#pgVIN14_5').length == 0) {
        AllPages = new LoadAllPages(ParentNode, "VIN14_5", ["pgVIN14_5"], afterLoadPage);
    };


}