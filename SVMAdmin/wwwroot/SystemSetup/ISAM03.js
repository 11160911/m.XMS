var PageISAM03 = function (ParentNode) {

    let grdM;
    let EditMode = "";
    let DelPLU = "";
    let DelPLUQty;
    let ModPLU = "";
    let ModPLUQty;
    let Q1;

    let AssignVar = function () {
        //alert("AssignVar");

        grdM = new DynGrid(
            {
                table_lement: $('#tbISAM03Mod')[0],
                class_collection: ["tdColbt icon_in_td btPLUDelete", "tdCol1", "tdCol2 label-align", "tdColbt icon_in_td btPLUMod"],
                fields_info: [
                    { type: "JQ", name: "fa-trash-o", element: '<i class="fa fa-trash-o"></i>' },
                    { type: "Text", name: "GD_Name", style: "width:46%"},
                    { type: "TextAmt", name: "OutNum", style: "width:18%;text-align:right" },
                    { type: "JQ", name: "fa-file-text-o", element: '<i class="fa fa-file-text-o"></i>' }
                ],
                //rows_per_page: 10,
                method_clickrow: click_PLU,
                afterBind: InitModifyDeleteButton,
                sortable: "N"
            }
        );
        return;
    };

    let click_PLU = function (tr) {

    };

    let InitModifyDeleteButton = function () {
        $('#tbISAM03Mod .fa-file-text-o').click(function () { btPLUMod_click(this) });
        $('#tbISAM03Mod .fa-trash-o').click(function () { btPLUDelete_click(this) });
    }

    let ChkLogOut_1 = function (AfterChkLogOut_1) {
        var LoginDT = sessionStorage.getItem('LoginDT');
        var cData = {
            LoginDT: LoginDT
        }
        PostToWebApi({ url: "api/js/ChkLogOut", data: cData, success: AfterChkLogOut_1 });
    };

    //#region 編查
    //#region 單筆修改
    let AfterSaveISAM03PLUMod = function (data) {
        if (ReturnMsg(data, 0) != "SaveISAM03PLUModOK") {
            DyAlert(ReturnMsg(data, 1));
        }
        else {
            DyAlert("修改完成!");

            $('#modal_ISAM03PLUMod').modal('hide');
            var dtDeliveryMod = data.getElementsByTagName('dtDeliveryMod')[0];
            grdM.RefreshRocord(grdM.ActiveRowTR(), dtDeliveryMod);
        }

    };

    let btModSave_click = function () {
        ChkLogOut_1(btModSave_click_1);
    };

    let btModSave_click_1 = function (data) {
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
        if ($('#txtModQty1').val() == "") {
            DyAlert("請輸入數量!");
            return;
        }
        else {

            if (isNaN($('#txtModQty1').val())) {
                DyAlert("請輸入數字!");
                return;
            }
            if ($('#txtModQty1').val().indexOf(".") > 0) {
                DyAlert("請輸入整數!");
                return;
            }

            if ($('#txtModQty1').val() <= 0) {
                DyAlert("數量需大於0!");
                return;
            }

            if ($('#txtModQty1').val() > 99999) {
                DyAlert("數量不可大於99999!");
                return;
            }
        }
        var cData = {
            WhNoOut: $('#lblWhNoOut').html().split(' ')[0],
            DocDate: $('#lblDate2').html(),
            WhNoIn: $('#lblWhNoIn').html().split(' ')[0],
            PLU: ModPLU,
            Qty: $('#txtModQty1').val()
        }
        PostToWebApi({ url: "api/SystemSetup/SaveISAM03PLUMod", data: cData, success: AfterSaveISAM03PLUMod });
            }
        }
    };

    let btModCancel_click = function () {
        //*
        ChkLogOut(sessionStorage.getItem('isamcomp'));
        Timerset(sessionStorage.getItem('isamcomp'));
        $('#modal_ISAM03PLUMod').modal('hide');
    };

    let AfterGetModGDName = function (data) {
        if (ReturnMsg(data, 0) != "GetGDNameOK") {
            DyAlert(ReturnMsg(data, 1));
        }
        else {
            var dtP = data.getElementsByTagName('dtPLU');
            //alert(dtP.length);
            if (dtP.length > 0) {
                $('#lblModPLU').html(ModPLU);
                $('#lblModPLUName').html(GetNodeValue(dtP[0], 'GD_Name'));
                $('#lblModQty1').html(ModPLUQty);
                $('#txtModQty1').val(ModPLUQty);
                $('#modal_ISAM03PLUMod').modal('show');
            }
            else {
                DyAlert("無符合之商品資料!");
                return;
            }
        }

        $('.msg-valid').hide();
    };

    let btPLUMod_click = function (bt) {
        //*
        ChkLogOut(sessionStorage.getItem('isamcomp'));
        Timerset(sessionStorage.getItem('isamcomp'));
        $(bt).closest('tr').click();
        //alert(GetNodeValue(node, 'AppDate'));


        $('.msg-valid').hide();
        $('#modal_ISAM03PLUMod .modal-title').text('出貨/調撥資料單筆修改');
        //$('#modal_ISAM03Mod .btn-danger').text('刪除');
        var node = $(grdM.ActiveRowTR()).prop('Record');
        ModPLU = GetNodeValue(node, 'PLU');
        ModPLUQty = GetNodeValue(node, 'OutNum');
        var cData = {
            PLU: ModPLU
        }
        PostToWebApi({ url: "api/SystemSetup/GetGDName", data: cData, success: AfterGetModGDName });

    };
    //#endregion

    //#region 單筆刪除
    let AfterDelISAM03PLU = function (data) {
        if (ReturnMsg(data, 0) != "DelISAM03PLUOK") {
            DyAlert(ReturnMsg(data, 1));
        }
        else {
            DyAlert("刪除完成!");

            $('#modal_ISAM03PLUDel').modal('hide');
            //var userxml = data.getElementsByTagName('dtRack')[0];
            grdM.DeleteRow(grdM.ActiveRowTR());
        }

    };

    let btDelSave_click = function () {
        ChkLogOut_1(btDelSave_click_1);
    };

    let btDelSave_click_1 = function (data) {
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
            WhNoOut: $('#lblWhNoOut').html().split(' ')[0],
            DocDate: $('#lblDate2').html(),
            WhNoIn: $('#lblWhNoIn').html().split(' ')[0],
            PLU: DelPLU
        }
        PostToWebApi({ url: "api/SystemSetup/DelISAM03PLU", data: cData, success: AfterDelISAM03PLU });
            }
        }
    };

    let btDelCancel_click = function () {
        //*
        ChkLogOut(sessionStorage.getItem('isamcomp'));
        Timerset(sessionStorage.getItem('isamcomp'));
        $('#modal_ISAM03PLUDel').modal('hide');
    };

    let AfterGetGDName = function (data) {
        if (ReturnMsg(data, 0) != "GetGDNameOK") {
            DyAlert(ReturnMsg(data, 1));
        }
        else {
            var dtP = data.getElementsByTagName('dtPLU');
            //alert(dtP.length);
            if (dtP.length > 0) {
                /*DyConfirm("確定要刪除商品" + GetNodeValue(dtP[0], 'PLU') + GetNodeValue(dtP[0], 'GD_Name') + "？", afterDelPLU(GetNodeValue(dtP[0], 'PLU')), DummyFunction);*/
                $('#lblPLU').html(DelPLU);
                $('#lblPLUName').html(GetNodeValue(dtP[0], 'GD_Name'));
                $('#lblDelQty1').html(DelPLUQty);
                $('#modal_ISAM03PLUDel').modal('show');
            }
            else {
                DyAlert("無符合之商品資料!");
                return;
            }
        }

        $('.msg-valid').hide();
    };

    let btPLUDelete_click = function (bt) {
        //*
        ChkLogOut(sessionStorage.getItem('isamcomp'));
        Timerset(sessionStorage.getItem('isamcomp'));
        $(bt).closest('tr').click();
        $('.msg-valid').hide();
        $('#modal_ISAM03PLUDel .modal-title').text('出貨/調撥資料單筆刪除');
        //$('#modal_ISAM03Mod .btn-danger').text('刪除');
        var node = $(grdM.ActiveRowTR()).prop('Record');
        DelPLU = GetNodeValue(node, 'PLU');
        DelPLUQty = GetNodeValue(node, 'OutNum');
        var cData = {
            PLU: DelPLU
        }
        PostToWebApi({ url: "api/SystemSetup/GetGDName", data: cData, success: AfterGetGDName });

    };
    //#endregion

    let txtBarcode3_ini = function () {
        $('#txtBarcode3').val('');
        $('#txtBarcode3').focus();
    }

    let btBCSave3_click = function () {
        //*
        ChkLogOut(sessionStorage.getItem('isamcomp'));
        Timerset(sessionStorage.getItem('isamcomp'));
        var cData = {
            WhNoOut: $('#lblWhNoOut').html().split(' ')[0],
            DocDate: $('#lblDate2').html(),
            WhNoIn: $('#lblWhNoIn').html().split(' ')[0],
            PLU: $('#txtBarcode3').val()
        }
        PostToWebApi({ url: "api/SystemSetup/GetDeliveryWebMod", data: cData, success: afterGetDeliveryWebMod });
    };

    let afterGetDeliveryWebMod = function (data) {
        if (ReturnMsg(data, 0) != "GetDeliveryWebModOK") {
            DyAlert(ReturnMsg(data, 1), txtBarcode3_ini);
        }
        else {
            var dtDelivery = data.getElementsByTagName('dtDelivery');
            grdM.BindData(dtDelivery);
            if (dtDelivery.length == 0) {
                //alert("No RowData");
                DyAlert("無符合資料!", txtBarcode3_ini);
                return;
            }
            txtBarcode3_ini();
        }

    };

    let btMod_click = function () {
        //*
        ChkLogOut(sessionStorage.getItem('isamcomp'));
        EditMode = "M";
        Timerset(sessionStorage.getItem('isamcomp'));
        var pData = {
            WhNoOut: $('#lblWhNoOut').html().split(' ')[0],
            DocDate: $('#lblDate2').html(),
            WhNoIn: $('#lblWhNoIn').html().split(' ')[0]
        }
        PostToWebApi({ url: "api/SystemSetup/GetDeliveryWebMod", data: pData, success: afterGetDeliveryWebMod });
        $('#pgISAM03Mod').show();
        if ($('#pgISAM03Mod').attr('hidden') == undefined) {
            $('#pgISAM03Mod').show();
        }
        else {
            $('#pgISAM03Mod').removeAttr('hidden');
        }
        if ($('#pgISAM03Add').attr('hidden') == undefined) {
            $('#pgISAM03Add').hide();
        }
        BtnSet("M");

    };
    //#endregion

    //#region 新增
    let txtBarcode1_ini = function () {
        $('#txtBarcode1').val('');
        $('#txtBarcode1').focus();
    }

    let txtQty1_ini = function () {
        $('#txtQty1').val('');
        $('#txtQty1').focus();
    }

    let btQtySave1_click = function () {
        ChkLogOut_1(btQtySave1_click_1);
    };

    let btQtySave1_click_1 = function (data) {
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
                if ($('#txtBarcode1').val() == "" && $('#lblBarcode').html() == "") {
                    DyAlert("請輸入條碼!");
                    $('#txtQty1').val('');
                    //$('#btKeyin1').prop('disabled', false);
                    //$('#btBCSave1').prop('disabled', false);
                    //$('#txtBarcode1').prop('disabled', false);
                    //$('#btQtySave1').prop('disabled', true);
                    //$('#txtQty1').prop('disabled', true);
                    return;
                }
                if ($('#txtQty1').val() == "") {
                    DyAlert("請輸入數量!");
                    return;
                }
                else {

                    if (isNaN($('#txtQty1').val())) {
                        DyAlert("請輸入數字!");
                        return;
                    }
                    if ($('#txtQty1').val().indexOf(".") > 0) {
                        DyAlert("請輸入整數!");
                        return;
                    }

                    if ($('#txtQty1').val() <= 0) {
                        DyAlert("數量需大於0!");
                        return;
                    }
                }
                //$('#btKeyin1').prop('disabled', false);
                //$('#btBCSave1').prop('disabled', false);
                //$('#txtBarcode1').prop('disabled', false);
                //$('#btQtySave1').prop('disabled', true);
                //$('#txtQty1').prop('disabled', true);
                Q1 = $('#txtQty1').val();

                var pData = {
                    WhNoOut: $('#lblWhNoOut').html().split(' ')[0],
                    WhNoIn: $('#lblWhNoIn').html().split(' ')[0],
                    DocDate: $('#lblDate2').html(),
                    Barcode: $('#txtBarcode1').val() == "" ? $('#lblBarcode').html() : $('#txtBarcode1').val(),
                    Qty: Q1
                };
                PostToWebApi({ url: "api/SystemSetup/SaveDeliveryWeb", data: pData, success: afterSaveDeliveryWeb });
            }
        }
    };

    let btKeyin1_click = function () {
        //*
        ChkLogOut(sessionStorage.getItem('isamcomp'));
        Timerset(sessionStorage.getItem('isamcomp'));
        //$('#btKeyin1').prop('disabled', true);
        //$('#btQtySave1').prop('disabled', false);
        //$('#txtQty1').prop('disabled', false);
        //$('#btBCSave1').prop('disabled', true);
        //$('#txtBarcode1').prop('disabled', true);
    };

    let afterSaveDeliveryWeb = function (data) {
        if (ReturnMsg(data, 0) != "SaveDeliveryWebOK") {
            DyAlert(ReturnMsg(data, 1), txtBarcode1_ini);
        }
        else {
           
            var dtSQ = data.getElementsByTagName('dtSQ');   //單品數
            if (dtSQ.length > 0) {
                $('#lblSQty1').html(GetNodeValue(dtSQ[0], "SQ1"));
            }
            else {
                $('#lblSQty1').html('');
            }
            var dtSWQ = data.getElementsByTagName('dtSWQ'); //門市總數
            if (dtSWQ.length > 0) {
                $('#lblSWQty1').html(GetNodeValue(dtSWQ[0], "SWQ1"));
            }
            else {
                $('#lblSWQty1').html('');
            }
            var dtP = data.getElementsByTagName('dtPLU');
            if (dtP.length > 0) {
                $('#lblBarcode').html(GetNodeValue(dtP[0], "PLU"));
                $('#txtBarcode1').val('');
                $('#lblQty1').html($('#txtQty1').val());
                $('#lblPrice').html(parseInt(GetNodeValue(dtP[0], "GD_Retail")));
                $('#lblGDName').html(GetNodeValue(dtP[0], "GD_Name"));
                $('#txtQty1').val('1');
            }
            else {
                if ($('#txtBarcode1').val() == "") {
                }
                else {
                    $('#lblBarcode').html($('#txtBarcode1').val());
                }
                $('#txtBarcode1').val('');
                $('#lblQty1').html($('#txtQty1').val());
                $('#lblPrice').html('');
                $('#lblGDName').html('');
                $('#txtQty1').val('1');
            }
        }
    };


    let btBCSave1_click = function () {
        ChkLogOut_1(btBCSave1_click_1);
    };

    let btBCSave1_click_1 = function (data) {
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
                if ($('#txtBarcode1').val() == "") {
                    DyAlert("請輸入條碼!", txtBarcode1_ini);
                    return;
                }
                if ($('#txtBarcode1').val().length > 16) {
                    DyAlert("條碼限制輸入16個字元!", txtBarcode1_ini);
                    $('#txtBarcode1').val('');
                    return;
                }
                if ($('#txtQty1').val() == "" || $('#txtQty1').val() == "0") {
                    $('#txtQty1').val("1");
                }
                else {
                    if (isNaN($('#txtQty1').val())) {
                        DyAlert("請輸入數字!", txtQty1_ini);
                        return;
                    }
                    if ($('#txtQty1').val().indexOf(".") > 0) {
                        DyAlert("請輸入整數!", txtQty1_ini);
                        return;
                    }
                    //if ($('#txtQty1').val() <= 0) {
                    //    DyAlert("數量需大於0!", txtBarcode1_ini);
                    //    return;
                    //}
                    if ($('#txtQty1').val() > 9999 || $('#txtQty1').val() < -9999) {
                        DyAlert("數量需介於-9999~9999之間!", txtQty1_ini);
                        return;
                    }
                }
                var pData = {
                    WhNoOut: $('#lblWhNoOut').html().split(' ')[0],
                    WhNoIn: $('#lblWhNoIn').html().split(' ')[0],
                    DocDate: $('#lblDate2').html(),
                    Barcode: $('#txtBarcode1').val()
                };
                PostToWebApi({ url: "api/SystemSetup/ChkSaveDeliveryWeb", data: pData, success: afterChkSaveDeliveryWeb });
            }
        }
    };

    let afterChkSaveDeliveryWeb = function (data) {
        if (ReturnMsg(data, 0) != "ChkSaveDeliveryWebOK") {
            DyAlert(ReturnMsg(data, 1));
        }
        else {
            var dtD = data.getElementsByTagName('dtD');
            if (dtD.length == 0) {
                if ($('#txtQty1').val() < 0) {
                    DyAlert("單品數需大於0!", txtQty1_ini);
                    return;
                }
            }
            else {
                if (parseInt(GetNodeValue(dtD[0], "SumQty")) + parseInt($('#txtQty1').val()) > 99999) {
                    DyAlert("單品數不可大於99999!", txtQty1_ini);
                    return;
                }
                if (parseInt(GetNodeValue(dtD[0], "SumQty")) + parseInt($('#txtQty1').val()) < 0) {
                    DyAlert("單品數需大於0!", txtQty1_ini);
                    return;
                }

                var pData = {
                    WhNoOut: $('#lblWhNoOut').html().split(' ')[0],
                    WhNoIn: $('#lblWhNoIn').html().split(' ')[0],
                    DocDate: $('#lblDate2').html(),
                    Barcode: $('#txtBarcode1').val(),
                    Qty: $('#txtQty1').val()
                };
                PostToWebApi({ url: "api/SystemSetup/SaveDeliveryWeb", data: pData, success: afterSaveDeliveryWeb });
            }
        }
    }

    let btAdd_click = function () {
        //*
        ChkLogOut(sessionStorage.getItem('isamcomp'));
        EditMode = "A";
        Timerset(sessionStorage.getItem('isamcomp'));
        $('#pgISAM03Add').show();
        if ($('#pgISAM03Add').attr('hidden') == undefined) {
            $('#pgISAM03Add').show();
        }
        else {
            $('#pgISAM03Add').removeAttr('hidden');
        }
        if ($('#pgISAM03Mod').attr('hidden') == undefined) {
            $('#pgISAM03Mod').hide();
        }
        BtnSet("A");

    };
    //#endregion

    let BtnSet = function (edit) {
        //alert(edit);
       switch (edit) {
            case "A":
                $('#btAdd').prop('disabled', false);
                $('#btMod').prop('disabled', true);
                $('#btToFTP').prop('disabled', true); //true-btn不能使用
               $('#txtBarcode1').val('');
               $('#txtBarcode1').focus();
                $('#txtQty1').val('1');
                $('#lblBarcode').html('');
                $('#lblQty1').html('');
                $('#lblSQty1').html('');
                $('#lblSBQty1').html('');
                $('#lblSWQty1').html('');
                $('#lblPrice').html('');
               $('#lblGDName').html('');

               //$('#btKeyin1').prop('disabled', false);
               //$('#btBCSave1').prop('disabled', false);
               //$('#txtBarcode1').prop('disabled', false);
               //$('#btQtySave1').prop('disabled', true);
               //$('#txtQty1').prop('disabled', true);
                break;
            case "Q":
                $('#btAdd').prop('disabled', false);
                $('#btMod').prop('disabled', false);
                $('#btToFTP').prop('disabled', false); //true-btn不能使用
                break;
            case "M":
                $('#btAdd').prop('disabled', true);
                $('#btMod').prop('disabled', false);
                $('#btToFTP').prop('disabled', true); //true-btn不能使用
                $('#txtBarcode3').val('');
                break;
        }
    };

    //#region 上傳
    let AfterAddISAMToFTPRecWeb = function (data) {
        if (ReturnMsg(data, 0) != "AddISAMToFTPRecWebOK") {
            if (ReturnMsg(data, 1) == "FTP") {
                DyAlert("FTP設定有誤，請重新確認!")
            }
            else if (ReturnMsg(data, 1) == "上傳記錄") {
                DyAlert("待上傳記錄新增失敗，請重新上傳!")
            }
            else if (ReturnMsg(data, 1) == "上傳資料") {
                DyAlert("無上傳資料，請重新確認!")
            }
            else if (ReturnMsg(data, 1) == "上傳檔案") {
                DyAlert("待上傳檔案不存在，請重新確認!")
            }
            else {
                DyAlert(ReturnMsg(data, 1));
            }
        }
        else {
            DyAlert("上傳成功!")
        }
    }

    let CallSendToFTP = function () {
        ChkLogOut_1(CallSendToFTP_1);
    };

    let CallSendToFTP_1 = function (data) {
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
                    Type: "D",
                    Shop: $('#lblWhNoOut').html().split(' ')[0],
                    WhNoIn: $('#lblWhNoIn').html().split(' ')[0],
                    DocDate: $('#lblDate2').html(),
                }
                PostToWebApi({ url: "api/SystemSetup/AddISAMToFTPRecWeb", data: cData, success: AfterAddISAMToFTPRecWeb });
            }
        }
    };


    let btToFTP_click = function () {
        //*
        ChkLogOut(sessionStorage.getItem('isamcomp'));
        Timerset(sessionStorage.getItem('isamcomp'));
        DyConfirm("是否要上傳出貨/調撥資料？", CallSendToFTP, DummyFunction);
    };
    //#endregion

    //#region 返回
    let afterRtnclick = function () {    
       if (EditMode == "A" || EditMode == "M") {
            EditMode = "Q";
            BtnSet(EditMode);
        }
    };

    let btRtn_click = function () {
        //*
        ChkLogOut(sessionStorage.getItem('isamcomp'));
        if (EditMode == "Q") {
            $('#ISAM03btns').hide();
            $('#pgISAM03Init').show();
            $('#pgISAM03Add').hide();
            $('#pgISAM03Mod').hide();
            //$('#pgISAM03UpFtp').hide();
        } else if (EditMode == "A" || EditMode == "M") {
            $('#ISAM03btns').show();
            $('#pgISAM03Init').hide();
            $('#pgISAM03Add').hide();
            $('#pgISAM03Mod').hide();
            //$('#pgISAM03UpFtp').hide();
        }
        Timerset(sessionStorage.getItem('isamcomp'));
    };
    //#endregion

    //#region 輸入日期,門市
    let afterSearchDeliveryWeb = function (data) {
        //EditMode = "Q";
        if (ReturnMsg(data, 0) != "SearchDeliveryWebOK") {
            DyAlert(ReturnMsg(data, 1));
        }
        else {
            var dtDeliveryData = data.getElementsByTagName('dtDeliveryData');
            if (dtDeliveryData.length > 0) {
                if (GetNodeValue(dtDeliveryData[0], "OutUser") != $('#lblManID1').html().split(' ')[0]) {
                    DyConfirm(GetNodeValue(dtDeliveryData[0], "DocDate") + "進貨店" + GetNodeValue(dtDeliveryData[0], "WhNoIn") + "出貨調撥資料已存在\n" + "，是否建立新出貨調撥資料?", CallShowData, DummyFunction);
                    //return;
                }
                else
                {
                    CallShowData();
                }
            }
            else {
                //AssignVar();
                //alert(GetNodeValue(dtISAMShop[0], "STName"));
                CallShowData();
            }
        }
    };

    let CallShowData = function () {
        Timerset(sessionStorage.getItem('isamcomp'));
        EditMode = "Q";
        $('#lblWhNoOut').html($('#lblShop1').html());
        $('#lblWhNoIn').html($('#selST_ID option:selected').text());
        $('#lblDate2').html($('#txtDocDate').val());
        $('#lblSBQty1title').html("單品數：");
        $('#lblSWQty1title').html($('#lblShop1').html().split(' ')[0] + "門市總數：");
        $('#lblSQty1').html('');
        $('#lblSWQty1').html('');
        $('#lblPrice').html('');
        $('#lblGDName').html("品名XXX");
        $('#pgISAM03Init').hide();
        if ($('#ISAM03btns').attr('hidden') == undefined) {
            $('#ISAM03btns').show();
        }
        else {
            $('#ISAM03btns').removeAttr('hidden');
        }
    };

    let btSave_click = function () {
        //*
        ChkLogOut(sessionStorage.getItem('isamcomp'));
        Timerset(sessionStorage.getItem('isamcomp'));
        if ($('#txtDocDate').val() == "" | $('#txtDocDate').val() == null) {
            DyAlert("請輸入出貨/調撥日期!!", function () { $('#txtDocDate').focus() });
            return;
        }
        if ($('#selST_ID').val() == "" | $('#selST_ID').val() == null) {
            DyAlert("請選擇進貨門市!!", function () { $('#selST_ID').focus() });
            return;
        }
        else {
            //alert($('#lblShop1').html().split(' ')[0]);
            //alert($('#selST_ID').val());
            if ($('#lblShop1').html().split(' ')[0] == $('#selST_ID').val()) {
                DyAlert("出貨/調撥店櫃(" + $('#selST_ID').val() + ")不可與進貨店櫃相同!", BlankMode);
                return;
            }
            //if ($('#txtBinNo').val().length>10) {
            //    DyAlert("分區代碼不可超過10個字元!!", function () { $('#txtBinNo').focus() });
            //    return;
            //}
        }
        var pData = {
            WhNoOut: $('#lblShop1').html().split(' ')[0],
            DocDate: $('#txtDocDate').val(),
            WhNoIn: $('#selST_ID').val()
        }
        PostToWebApi({ url: "api/SystemSetup/SearchDeliveryWeb", data: pData, success: afterSearchDeliveryWeb });
    };
    //#endregion

    //#region FormLoad
    let afterGetInitISAM03 = function (data) {
        if (ReturnMsg(data, 0) != "GetInitISAM01OK") {
            DyAlert(ReturnMsg(data, 1));
        }
        else {
            AssignVar();
            EditMode = "Init";
            tbDetail = $('#pgISAM03Mod #tbISAM03Mod tbody');
            var dtISAMShop = data.getElementsByTagName('dtWh');
            var dtShop = data.getElementsByTagName('dtShop');
            InitSelectItem($('#selST_ID')[0], dtShop, "ST_ID", "STName", true, "*請選擇店別");
            //alert(GetNodeValue(dtISAMShop[0], "STName"));
            $('#lblShop1').text(GetNodeValue(dtISAMShop[0], "STName"));
            $('#lblManID1').text(GetNodeValue(dtISAMShop[0], "ManName"));
            SetDateField($('#txtDocDate')[0]);
            $('#pgISAM03Init').removeAttr('hidden');
            //$('#pgISAM03Init').show();
            $('#btSave').click(function () { btSave_click(); });


            $('#btAdd').click(function () { btAdd_click(); });
            $('#btMod').click(function () { btMod_click(); });
            $('#btToFTP').click(function () { btToFTP_click(); });
            $('#btRtn').click(function () { btRtn_click(); afterRtnclick(); });

            //$('#txtQty1').prop('disabled', true);
            //$('#btQtySave1').prop('disabled', true);
            $('#btBCSave1').click(function () { btBCSave1_click(); });
            $('#btKeyin1').click(function () { btKeyin1_click(); });
            $('#btQtySave1').click(function () { btQtySave1_click(); });
            $('#txtBarcode1').keypress(function (e) {
                if (e.which == 13) { btBCSave1_click(); }
            });


            $('#btDelCancel').click(function () { btDelCancel_click(); });
            $('#btDelSave').click(function () { btDelSave_click(); });


            $('#btModCancel').click(function () { btModCancel_click(); });
            $('#btModSave').click(function () { btModSave_click(); });
            $('#btBCSave3').click(function () { btBCSave3_click(); });
            $('#txtBarcode3').keypress(function (e) {
                if (e.which == 13) { btBCSave3_click(); }
            });
        }
    };


    let AfterGetWhName = function (data) {
        if (ReturnMsg(data, 0) != "GetWhNameOK") {
            DyAlert(ReturnMsg(data, 1));
        }
        else {
            var dtWh = data.getElementsByTagName('dtWh');
            //alert(GetNodeValue(dtWh[0], "ST_ID"));
            if (GetNodeValue(dtWh[0], "STName") == "") {
                DyAlert("請確認店櫃(" + GetNodeValue(dtWh[0], "WhNo") + ")是否為允許作業之店櫃!", BlankMode);
                return;
            }
            PostToWebApi({ url: "api/SystemSetup/GetInitISAM03", success: afterGetInitISAM03 });
        }
    };

    let afterGetPageInitBefore = function (data) {
        if (ReturnMsg(data, 0) != "GetPageInitBeforeOK") {
            DyAlert(ReturnMsg(data, 1));
        }
        else {
            var dtISAMWh = data.getElementsByTagName('dtComp');
            if (dtISAMWh.length == 0) {
                DyAlert("無符合資料!", BlankMode);
                return;
            }
            else if (GetNodeValue(dtISAMWh[0], "WhNo") == null || GetNodeValue(dtISAMWh[0], "WhNo") == "") {
                DyAlert("請先至店號設定進行作業店櫃設定!", BlankMode);
                return;
            }
            else if (GetNodeValue(dtISAMWh[0], "WhNo") != "") {
                PostToWebApi({ url: "api/SystemSetup/GetWhName", success: AfterGetWhName });
            }

        }
    };

    let BlankMode = function () {

    };

    let afterLoadPage = function () {
        PostToWebApi({ url: "api/SystemSetup/GetPageInitBefore", success: afterGetPageInitBefore });
    };
    //#endregion


    if ($('#pgISAM03').length == 0) {
        AllPages = new LoadAllPages(ParentNode, "SystemSetup/ISAM03", ["ISAM03btns", "pgISAM03Init", "pgISAM03Add", "pgISAM03Mod"], afterLoadPage);
    };


}