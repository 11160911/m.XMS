var PageMSSD102 = function (ParentNode) {

    let grdM;
    let grdShopNo_PSNO;
    let grdDate_PSNO;
    let grdLookUp_ActivityCode;

    let grdM_Shop;
    let grdM_Area_Step1;
    let grdM_Area_Shop_Step2;
    let grdM_Area_Date_Step2;
    let grdM_Shop_Step1;
    let grdM_Date_Step1;
    let grdM_Date_Area_Step2;

    let EditMode = "";
    let DelPLU = "";
    let DelPLUQty;
    let ModPLU = "";
    let ModPLUQty;


    let AssignVar = function () {

        grdM = new DynGrid(
            {
                table_lement: $('#tbQuery')[0],
                class_collection: ["tdCol1", "tdCol2 text-center", "tdCol3", "tdCol4 text-center", "tdCol5 label-align", "tdCol6 label-align", "tdCol7 text-center", "tdCol8 label-align", "tdCol9 label-align", "tdCol10 label-align", "tdCol11 label-align"],
                fields_info: [
                    { type: "Text", name: "PS_NO", style: "" },
                    { type: "Text", name: "ActivityCode" },
                    { type: "Text", name: "PS_Name" },
                    { type: "Text", name: "EDDate"},
                    { type: "TextAmt", name: "Cnt1"},
                    { type: "TextAmt", name: "Cnt2"},
                    { type: "TextAmt", name: "RePercent"},
                    { type: "TextAmt", name: "ActualDiscount"},
                    { type: "TextAmt", name: "Cnt3"},
                    { type: "TextAmt", name: "Cash" },
                    { type: "TextAmt", name: "SalesPrice" }
                ],
                //rows_per_page: 10,
                method_clickrow: click_PLU,
                afterBind: InitModifyDeleteButton,
                sortable: "Y"
            }
        );

        grdShopNo_PSNO = new DynGrid(
            {
                table_lement: $('#tbShopNo_PSNO')[0],
                class_collection: ["tdCol1 text-center", "tdCol2 label-align", "tdCol3 label-align", "tdCol4 text-center", "tdCol5 label-align", "tdCol6 label-align", "tdCol7 label-align", "tdCol8 label-align", "tdCol9 label-align", "tdCol10 label-align", "tdCol11 label-align"],
                fields_info: [
                    { type: "Text", name: "ID", style: "" },
                    { type: "TextAmt", name: "Cnt1" },
                    { type: "TextAmt", name: "Cnt2" },
                    { type: "TextAmt", name: "RePercent" },
                    { type: "TextAmt", name: "ActualDiscount" },
                    { type: "TextAmt", name: "SalesCnt1" },
                    { type: "TextAmt", name: "SalesCash1" },
                    { type: "TextAmt", name: "SalesPrice1" },
                    { type: "TextAmt", name: "SalesCash2" },
                    { type: "TextAmt", name: "SalesCnt2" },
                    { type: "TextAmt", name: "SalesPrice2" }
                ],
                //rows_per_page: 10,
                method_clickrow: click_PLU,
                afterBind: InitModifyDeleteButton,
                sortable: "Y"
            }
        );

        grdDate_PSNO = new DynGrid(
            {
                table_lement: $('#tbDate_PSNO')[0],
                class_collection: ["tdCol1 text-center", "tdCol2 label-align", "tdCol3 label-align", "tdCol4 label-align", "tdCol5 label-align", "tdCol6 label-align", "tdCol7 label-align", "tdCol8 label-align", "tdCol9 label-align"],
                fields_info: [
                    { type: "Text", name: "ID", style: "" },
                    { type: "TextAmt", name: "Cnt1" },
                    { type: "TextAmt", name: "ActualDiscount" },
                    { type: "TextAmt", name: "SalesCash1" },
                    { type: "TextAmt", name: "SalesCnt1" },
                    { type: "TextAmt", name: "SalesPrice1" },
                    { type: "TextAmt", name: "SalesCash2" },
                    { type: "TextAmt", name: "SalesCnt2" },
                    { type: "TextAmt", name: "SalesPrice2" },
                ],
                //rows_per_page: 10,
                method_clickrow: click_PLU,
                afterBind: InitModifyDeleteButton,
                sortable: "Y"
            }
        );


        grdLookUp_ActivityCode = new DynGrid(
            {
                table_lement: $('#tbDataLookup_ActivityCode')[0],
                class_collection: ["tdCol1", "tdCol2", "tdCol3", "tdCol4", "tdCol5"],
                fields_info: [
                    { type: "radio", name: "chkset", style: "width:16px;height:16px" },
                    { type: "Text", name: "ActivityCode", style: "" },
                    { type: "Text", name: "PS_Name", style: "" },
                    { type: "Text", name: "StartDate", style: "" },
                    { type: "Text", name: "EndDate", style: "" }
                ],
                //rows_per_page: 10,
                method_clickrow: click_PLU,
                afterBind: InitModifyDeleteButton,
                sortable: "N"
            }
        );

        grdM_Shop = new DynGrid(
            {
                table_lement: $('#tbSales_Shop')[0],
                class_collection: ["tdCol1", "tdCol2", "tdCol3 label-align", "tdCol4 label-align", "tdCol5 label-align", "tdCol6 label-align", "tdCol7 label-align", "tdCol8 label-align", "tdCol9"],
                fields_info: [
                    { type: "Text", name: "ID1", style: "display:none" },
                    { type: "Text", name: "Name1" },
                    { type: "TextAmt", name: "Cash1" },
                    { type: "TextAmt", name: "Cnt1" },
                    { type: "TextAmt", name: "CashCnt1" },
                    { type: "TextAmt", name: "Cash2" },
                    { type: "TextAmt", name: "Cnt2" },
                    { type: "TextAmt", name: "CashCnt2" },
                    { type: "Text", name: "VIPPercent" }
                ],
                //rows_per_page: 10,
                method_clickrow: click_PLU,
                afterBind: InitModifyDeleteButton_Shop,
                sortable: "N"
            }
        );

        grdM_Area_Step1 = new DynGrid(
            {
                table_lement: $('#tbSales_Area_Step1')[0],
                class_collection: ["tdCol1", "tdCol2", "tdCol3 label-align", "tdCol4 label-align", "tdCol5 label-align", "tdCol6 label-align", "tdCol7 label-align", "tdCol8 label-align", "tdCol9"],
                fields_info: [
                    { type: "Text", name: "ID1", style: "display:none" },
                    { type: "Text", name: "Name1" },
                    { type: "TextAmt", name: "Cash1" },
                    { type: "TextAmt", name: "Cnt1" },
                    { type: "TextAmt", name: "CashCnt1" },
                    { type: "TextAmt", name: "Cash2" },
                    { type: "TextAmt", name: "Cnt2" },
                    { type: "TextAmt", name: "CashCnt2" },
                    { type: "Text", name: "VIPPercent" }
                ],
                //rows_per_page: 10,
                method_clickrow: click_PLU,
                afterBind: InitModifyDeleteButton_Area_Step1,
                sortable: "N"
            }
        );

        grdM_Area_Shop_Step2 = new DynGrid(
            {
                table_lement: $('#tbSales_Area_Shop_Step2')[0],
                class_collection: ["tdCol1", "tdCol2", "tdCol3 label-align", "tdCol4 label-align", "tdCol5 label-align", "tdCol6 label-align", "tdCol7 label-align", "tdCol8 label-align", "tdCol9"],
                fields_info: [
                    { type: "Text", name: "ID1", style: "display:none" },
                    { type: "Text", name: "Name1" },
                    { type: "TextAmt", name: "Cash1" },
                    { type: "TextAmt", name: "Cnt1" },
                    { type: "TextAmt", name: "CashCnt1" },
                    { type: "TextAmt", name: "Cash2" },
                    { type: "TextAmt", name: "Cnt2" },
                    { type: "TextAmt", name: "CashCnt2" },
                    { type: "Text", name: "VIPPercent" }
                ],
                //rows_per_page: 10,
                method_clickrow: click_PLU,
                afterBind: InitModifyDeleteButton_Area_Shop_Step2,
                sortable: "N"
            }
        );

        grdM_Area_Date_Step2 = new DynGrid(
            {
                table_lement: $('#tbSales_Area_Date_Step2')[0],
                class_collection: ["tdCol1", "tdCol2", "tdCol3 label-align", "tdCol4 label-align", "tdCol5 label-align", "tdCol6 label-align", "tdCol7 label-align", "tdCol8 label-align", "tdCol9"],
                fields_info: [
                    { type: "Text", name: "ID1", style: "display:none" },
                    { type: "Text", name: "Name1" },
                    { type: "TextAmt", name: "Cash1" },
                    { type: "TextAmt", name: "Cnt1" },
                    { type: "TextAmt", name: "CashCnt1" },
                    { type: "TextAmt", name: "Cash2" },
                    { type: "TextAmt", name: "Cnt2" },
                    { type: "TextAmt", name: "CashCnt2" },
                    { type: "Text", name: "VIPPercent" }
                ],
                //rows_per_page: 10,
                method_clickrow: click_PLU,
                afterBind: InitModifyDeleteButton_Area_Date_Step2,
                sortable: "N"
            }
        );

        grdM_Shop_Step1 = new DynGrid(
            {
                table_lement: $('#tbSales_Shop_Step1')[0],
                class_collection: ["tdCol1", "tdCol2", "tdCol3 label-align", "tdCol4 label-align", "tdCol5 label-align", "tdCol6 label-align", "tdCol7 label-align", "tdCol8 label-align", "tdCol9"],
                fields_info: [
                    { type: "Text", name: "ID1", style: "display:none" },
                    { type: "Text", name: "Name1" },
                    { type: "TextAmt", name: "Cash1" },
                    { type: "TextAmt", name: "Cnt1" },
                    { type: "TextAmt", name: "CashCnt1" },
                    { type: "TextAmt", name: "Cash2" },
                    { type: "TextAmt", name: "Cnt2" },
                    { type: "TextAmt", name: "CashCnt2" },
                    { type: "Text", name: "VIPPercent" }
                ],
                //rows_per_page: 10,
                method_clickrow: click_PLU,
                afterBind: InitModifyDeleteButton_Shop_Step1,
                sortable: "N"
            }
        );

        grdM_Date_Step1 = new DynGrid(
            {
                table_lement: $('#tbSales_Date_Step1')[0],
                class_collection: ["tdCol1", "tdCol2", "tdCol3 label-align", "tdCol4 label-align", "tdCol5 label-align", "tdCol6 label-align", "tdCol7 label-align", "tdCol8 label-align", "tdCol9"],
                fields_info: [
                    { type: "Text", name: "ID1", style: "display:none" },
                    { type: "Text", name: "Name1" },
                    { type: "TextAmt", name: "Cash1" },
                    { type: "TextAmt", name: "Cnt1" },
                    { type: "TextAmt", name: "CashCnt1" },
                    { type: "TextAmt", name: "Cash2" },
                    { type: "TextAmt", name: "Cnt2" },
                    { type: "TextAmt", name: "CashCnt2" },
                    { type: "Text", name: "VIPPercent" }
                ],
                //rows_per_page: 10,
                method_clickrow: click_PLU,
                afterBind: InitModifyDeleteButton_Date_Step1,
                sortable: "N"
            }
        );

        grdM_Date_Area_Step2 = new DynGrid(
            {
                table_lement: $('#tbSales_Date_Area_Step2')[0],
                class_collection: ["tdCol1", "tdCol2", "tdCol3 label-align", "tdCol4 label-align", "tdCol5 label-align", "tdCol6 label-align", "tdCol7 label-align", "tdCol8 label-align", "tdCol9"],
                fields_info: [
                    { type: "Text", name: "ID1", style: "display:none" },
                    { type: "Text", name: "Name1" },
                    { type: "TextAmt", name: "Cash1" },
                    { type: "TextAmt", name: "Cnt1" },
                    { type: "TextAmt", name: "CashCnt1" },
                    { type: "TextAmt", name: "Cash2" },
                    { type: "TextAmt", name: "Cnt2" },
                    { type: "TextAmt", name: "CashCnt2" },
                    { type: "Text", name: "VIPPercent" }
                ],
                //rows_per_page: 10,
                method_clickrow: click_PLU,
                afterBind: InitModifyDeleteButton_Date_Area_Step2,
                sortable: "N"
            }
        );

        return;
    };

    let click_PLU = function (tr) {

    };

    let InitModifyDeleteButton = function () {
        $('#tbQuery tbody tr td').click(function () { Step1_click(this) });
        //$('#tbISAM01Mod .fa-trash-o').click(function () { btPLUDelete_click(this) });
    }

    let InitModifyDeleteButton_Shop = function () {
    }

    let InitModifyDeleteButton_Area_Step1 = function () {
        $('#tbSales_Area_Step1 tbody tr td').click(function () { Area_Step1_click(this) });
    }

    let InitModifyDeleteButton_Area_Shop_Step2 = function () {
   
    }

    let InitModifyDeleteButton_Area_Date_Step2 = function () {

    }

    let InitModifyDeleteButton_Shop_Step1 = function () {
    }

    let InitModifyDeleteButton_Date_Step1 = function () {
        $('#tbSales_Date_Step1 tbody tr td').click(function () { Date_Step1_click(this) });
    }

    let InitModifyDeleteButton_Date_Area_Step2 = function () {
    }

    let Step1_click = function (bt) {
        $('#tbQuery td').closest('tr').css('background-color', 'white');
        $(bt).closest('tr').click();
        $('.msg-valid').hide();
        var node = $(grdM.ActiveRowTR()).prop('Record');
        $('#tbQuery td:contains(' + GetNodeValue(node, 'PS_NO') + ')').closest('tr').css('background-color', '#DEEBF7');
        $('#lblPSNO_PSNO').html(GetNodeValue(node, 'PS_NO'))
        $('#lblActivityCode_PSNO').html(GetNodeValue(node, 'ActivityCode'))
        $('#lblEDDate_PSNO').html(GetNodeValue(node, 'EDDate'))
        $('#lblPSName_PSNO').html(GetNodeValue(node, 'PS_Name'))
        $('#rdoShop_PSNO').prop('checked', true);
        $('#modal_PSNO').modal('show');
        setTimeout(function () {
            QueryPSNO(GetNodeValue(node, 'PS_NO'));
        }, 500);
    };

    let QueryPSNO = function (PS_NO) {
        ShowLoading();
        var Flag = "";

        if ($('#rdoShop_PSNO').prop('checked') == true) {
            Flag = "S";
            $('#tbShopNo_PSNO').show();
            $('#tbDate_PSNO').hide();

        }
        else if ($('#rdoDate_PSNO').prop('checked') == true) {
            Flag = "D";
            $('#tbShopNo_PSNO').hide();
            if ($('#tbDate_PSNO').attr('hidden') == undefined) {
                $('#tbDate_PSNO').show();
            }
            else {
                $('#tbDate_PSNO').removeAttr('hidden');
                $('#tbDate_PSNO').show();
            }
        }

        var OpenDate1 = $('#lblEDDate_PSNO').html().split('~')[0]
        var OpenDate2 = $('#lblEDDate_PSNO').html().split('~')[1]

        var pData = {
            PS_NO: PS_NO,
            OpenDate1: OpenDate1,
            OpenDate2: OpenDate2,
            Flag: Flag
        }
        PostToWebApi({ url: "api/SystemSetup/MSSD102Query_Step1", data: pData, success: afterMSSD102Query_Step1 });
    };

    let afterMSSD102Query_Step1 = function (data) {
        CloseLoading();
        if (ReturnMsg(data, 0) != "MSSD102Query_Step1OK") {
            DyAlert(ReturnMsg(data, 1));
        }
        else {
            var dtE = data.getElementsByTagName('dtE');
            var dtSumQ = data.getElementsByTagName('dtSumQ');
            if ($('#rdoShop_PSNO').prop('checked') == true) {
                grdShopNo_PSNO.BindData(dtE);
                if (dtE.length == 0) {
                    DyAlert("無符合資料!");
                    $(".modal-backdrop").remove();
                    $('#tbShopNo_PSNO thead td#tdShopNo1_PSNO').html('');
                    $('#tbShopNo_PSNO thead td#tdShopNo2_PSNO').html('');
                    $('#tbShopNo_PSNO thead td#tdShopNo3_PSNO').html('');
                    $('#tbShopNo_PSNO thead td#tdShopNo4_PSNO').html('');
                    $('#tbShopNo_PSNO thead td#tdShopNo5_PSNO').html('');
                    $('#tbShopNo_PSNO thead td#tdShopNo6_PSNO').html('');
                    $('#tbShopNo_PSNO thead td#tdShopNo7_PSNO').html('');
                    $('#tbShopNo_PSNO thead td#tdShopNo8_PSNO').html('');
                    $('#tbShopNo_PSNO thead td#tdShopNo9_PSNO').html('');
                    $('#tbShopNo_PSNO thead td#tdShopNo10_PSNO').html('');
                    return;
                }
                $('#tbShopNo_PSNO thead td#tdShopNo1_PSNO').html(parseInt(GetNodeValue(dtSumQ[0], "SumCnt1")).toLocaleString('en-US'));
                $('#tbShopNo_PSNO thead td#tdShopNo2_PSNO').html(parseInt(GetNodeValue(dtSumQ[0], "SumCnt2")).toLocaleString('en-US'));
                $('#tbShopNo_PSNO thead td#tdShopNo3_PSNO').html(GetNodeValue(dtSumQ[0], "SumRePercent"));
                $('#tbShopNo_PSNO thead td#tdShopNo4_PSNO').html(parseInt(GetNodeValue(dtSumQ[0], "SumActualDiscount")).toLocaleString('en-US'));
                $('#tbShopNo_PSNO thead td#tdShopNo5_PSNO').html(parseInt(GetNodeValue(dtSumQ[0], "SumSalesCnt1")).toLocaleString('en-US'));
                $('#tbShopNo_PSNO thead td#tdShopNo6_PSNO').html(parseInt(GetNodeValue(dtSumQ[0], "SumSalesCash1")).toLocaleString('en-US'));
                $('#tbShopNo_PSNO thead td#tdShopNo7_PSNO').html(parseInt(GetNodeValue(dtSumQ[0], "SumSalesPrice1")).toLocaleString('en-US'));
                $('#tbShopNo_PSNO thead td#tdShopNo8_PSNO').html(parseInt(GetNodeValue(dtSumQ[0], "SumSalesCash2")).toLocaleString('en-US'));
                $('#tbShopNo_PSNO thead td#tdShopNo9_PSNO').html(parseInt(GetNodeValue(dtSumQ[0], "SumSalesCnt2")).toLocaleString('en-US'));
                $('#tbShopNo_PSNO thead td#tdShopNo10_PSNO').html(parseInt(GetNodeValue(dtSumQ[0], "SumSalesPrice2")).toLocaleString('en-US'));
            }
            else if ($('#rdoDate_PSNO').prop('checked') == true) {
                grdDate_PSNO.BindData(dtE);
                if (dtE.length == 0) {
                    DyAlert("無符合資料!");
                    $(".modal-backdrop").remove();
                    $('#tbDate_PSNO thead td#tdDate1_PSNO').html('');
                    $('#tbDate_PSNO thead td#tdDate2_PSNO').html('');
                    $('#tbDate_PSNO thead td#tdDate3_PSNO').html('');
                    $('#tbDate_PSNO thead td#tdDate4_PSNO').html('');
                    $('#tbDate_PSNO thead td#tdDate5_PSNO').html('');
                    $('#tbDate_PSNO thead td#tdDate6_PSNO').html('');
                    $('#tbDate_PSNO thead td#tdDate7_PSNO').html('');
                    $('#tbDate_PSNO thead td#tdDate8_PSNO').html('');
                    return;
                }
                $('#tbDate_PSNO thead td#tdDate1_PSNO').html(parseInt(GetNodeValue(dtSumQ[0], "SumCnt1")).toLocaleString('en-US'));
                $('#tbDate_PSNO thead td#tdDate2_PSNO').html(parseInt(GetNodeValue(dtSumQ[0], "SumActualDiscount")).toLocaleString('en-US'));
                $('#tbDate_PSNO thead td#tdDate3_PSNO').html(parseInt(GetNodeValue(dtSumQ[0], "SumSalesCash1")).toLocaleString('en-US'));
                $('#tbDate_PSNO thead td#tdDate4_PSNO').html(parseInt(GetNodeValue(dtSumQ[0], "SumSalesCnt1")).toLocaleString('en-US'));
                $('#tbDate_PSNO thead td#tdDate5_PSNO').html(parseInt(GetNodeValue(dtSumQ[0], "SumSalesPrice1")).toLocaleString('en-US'));
                $('#tbDate_PSNO thead td#tdDate6_PSNO').html(parseInt(GetNodeValue(dtSumQ[0], "SumSalesCash2")).toLocaleString('en-US'));
                $('#tbDate_PSNO thead td#tdDate7_PSNO').html(parseInt(GetNodeValue(dtSumQ[0], "SumSalesCnt2")).toLocaleString('en-US'));
                $('#tbDate_PSNO thead td#tdDate8_PSNO').html(parseInt(GetNodeValue(dtSumQ[0], "SumSalesPrice2")).toLocaleString('en-US'));
            }
        }
    };




    let Area_Step1_click = function (bt) {
        $(bt).closest('tr').click();
        $('.msg-valid').hide();
        /*  $('#modal_ISAM01PLUDel .modal-title').text('盤點資料單筆刪除');*/
        var node = $(grdM_Area_Step1.ActiveRowTR()).prop('Record');

        if ($('#rdoShop_Area_Step1').prop('checked')) {
            $('#lblOpenDate_Area_Shop_Step2').html($('#lblOpenDate_Area_Step1').html());
            $('#lblArea_Shop_Step2').html($('#lblArea_Step1').html() + '-' + GetNodeValue(node, 'ID1') + ' ' + GetNodeValue(node, 'Name1'));
            $('#modal_Area_Shop_Step2').modal('show');
            setTimeout(function () {
                Query_Area_Shop_Step2_click();
            }, 500);
        }
        else if ($('#rdoDate_Area_Step1').prop('checked')) {
            $('#lblOpenDate_Area_Date_Step2').html($('#lblOpenDate_Area_Step1').html());
            $('#lblArea_Date_Step2').html($('#lblArea_Step1').html() + '-' + GetNodeValue(node, 'ID1'));
            $('#modal_Area_Date_Step2').modal('show');
            setTimeout(function () {
                Query_Area_Date_Step2_click();
            }, 500);
        }
    };

    let Date_Step1_click = function (bt) {
        $(bt).closest('tr').click();
        $('.msg-valid').hide();
        var node = $(grdM_Date_Step1.ActiveRowTR()).prop('Record');

        if ($('#rdoArea_Date_Step1').prop('checked')) {
            $('#lblOpenDate_Date_Area_Step2').html($('#lblOpenDate_Date_Step1').html());
            $('#lblDate_Area_Step2').html($('#lblDate_Step1').html() + '-' + GetNodeValue(node, 'ID1') + ' ' + GetNodeValue(node, 'Name1'));
            $('#modal_Date_Area_Step2').modal('show');
            setTimeout(function () {
                Query_Date_Area_Step2_click();
            }, 500);
        }
        else if ($('#rdoShop_Date_Step1').prop('checked')) {
        }
    };

    let btExit_Shop_click = function (bt) {
        $('#modal_Shop').modal('hide');
    };

    let ChkLogOut_1 = function (AfterChkLogOut_1) {
        var LoginDT = sessionStorage.getItem('LoginDT');
        var cData = {
            LoginDT: LoginDT
        }
        PostToWebApi({ url: "api/js/ChkLogOut", data: cData, success: AfterChkLogOut_1 });
    };

//#region 編查
//#region 單筆修改
    let AfterSaveISAM01PLUMod = function (data) {
        if (ReturnMsg(data, 0) != "SaveISAM01PLUModOK") {
            DyAlert(ReturnMsg(data, 1));
        }
        else {
            DyAlert("修改完成!");

            $('#modal_ISAM01PLUMod').modal('hide');
            var dtBINMod = data.getElementsByTagName('dtBINMod')[0];
            grdM.RefreshRocord(grdM.ActiveRowTR(), dtBINMod);
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

            if ($('#txtModQty1').val() > 999999) {
                DyAlert("數量不可大於999999!");
                return;
            }
        }
        var cData = {
            Shop: $('#lblShop2').html().split(' ')[0],
            ISAMDate: $('#lblDate2').html(),
            BinNo: $('#lblBINNo2').html(),
            PLU: ModPLU,
            Qty: $('#txtModQty1').val()
        }
        PostToWebApi({ url: "api/SystemSetup/SaveISAM01PLUMod", data: cData, success: AfterSaveISAM01PLUMod });
            }
        }
    }

    let btModCancel_click = function () {
        //*
        ChkLogOut(sessionStorage.getItem('isamcomp'));
        Timerset(sessionStorage.getItem('isamcomp'));
        $('#modal_ISAM01PLUMod').modal('hide');
    };

    let AfterGetModGDName = function (data) {
        if (ReturnMsg(data, 0) != "GetGDNameOK") {
            DyAlert(ReturnMsg(data, 1));
        }
        else {
            var dtP = data.getElementsByTagName('dtPLU');
            //alert(dtP.length);
            $('#lblModPLU').html(ModPLU);
            $('#lblModQty1').html(ModPLUQty);
            $('#txtModQty1').val(ModPLUQty);
            if (dtP.length > 0) {
                $('#lblModPLUName').html(GetNodeValue(dtP[0], 'GD_Name'));
            }
            else {
                //DyAlert("無符合之商品資料!");
                //return;
                $('#lblModPLUName').html('');
            }
            $('#modal_ISAM01PLUMod').modal('show');
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
        $('#modal_ISAM01PLUMod .modal-title').text('盤點資料單筆修改');
        //$('#modal_ISAM01Mod .btn-danger').text('刪除');
        var node = $(grdM.ActiveRowTR()).prop('Record');
        ModPLU = GetNodeValue(node, 'PLU');
        ModPLUQty = GetNodeValue(node, 'Qty1');
        var cData = {
            Shop: $('#lblShop2').html().split(' ')[0],
            ISAMDate: $('#lblDate2').html(),
            BinNo: $('#lblBINNo2').html(),
            PLU: ModPLU
        }
        PostToWebApi({ url: "api/SystemSetup/GetGDName", data: cData, success: AfterGetModGDName });

    };
//#endregion

//#region 單筆刪除
    let AfterDelISAM01PLU = function (data) {
        if (ReturnMsg(data, 0) != "DelISAM01PLUOK") {
            DyAlert(ReturnMsg(data, 1));
        }
        else {
            DyAlert("刪除完成!");

            $('#modal_ISAM01PLUDel').modal('hide');
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
                    Shop: $('#lblShop2').html().split(' ')[0],
                    ISAMDate: $('#lblDate2').html(),
                    BinNo: $('#lblBINNo2').html(),
                    PLU: DelPLU
                }
                PostToWebApi({ url: "api/SystemSetup/DelISAM01PLU", data: cData, success: AfterDelISAM01PLU });
            }
        }
    }

    let btDelCancel_click = function () {
        //*
        ChkLogOut(sessionStorage.getItem('isamcomp'))
        Timerset(sessionStorage.getItem('isamcomp'));
        $('#modal_ISAM01PLUDel').modal('hide');
    };

    let AfterGetGDName = function (data) {
        if (ReturnMsg(data, 0) != "GetGDNameOK") {
            DyAlert(ReturnMsg(data, 1));
        }
        else {
            var dtP = data.getElementsByTagName('dtPLU');
            //alert(dtP.length);
            $('#lblPLU').html(DelPLU);
            $('#lblDelQty1').html(DelPLUQty);
            if (dtP.length > 0) {
                /*DyConfirm("確定要刪除商品" + GetNodeValue(dtP[0], 'PLU') + GetNodeValue(dtP[0], 'GD_Name') + "？", afterDelPLU(GetNodeValue(dtP[0], 'PLU')), DummyFunction);*/
                $('#lblPLUName').html(GetNodeValue(dtP[0], 'GD_Name'));
            }
            else {
                $('#lblPLUName').html('');
             }
            $('#modal_ISAM01PLUDel').modal('show');
        }

        $('.msg-valid').hide();
    };

    let btPLUDelete_click = function (bt) {
        //*
        ChkLogOut(sessionStorage.getItem('isamcomp'));
        Timerset(sessionStorage.getItem('isamcomp'));
        $(bt).closest('tr').click();
        $('.msg-valid').hide();
        $('#modal_ISAM01PLUDel .modal-title').text('盤點資料單筆刪除');
        //$('#modal_ISAM01Mod .btn-danger').text('刪除');
        var node = $(grdM.ActiveRowTR()).prop('Record');
        DelPLU = GetNodeValue(node, 'PLU');
        DelPLUQty = GetNodeValue(node, 'Qty1');
        var cData = {
            Shop: $('#lblShop2').html().split(' ')[0],
            ISAMDate: $('#lblDate2').html(),
            BinNo: $('#lblBINNo2').html(),
            PLU: DelPLU
        }
        PostToWebApi({ url: "api/SystemSetup/GetGDName", data: cData, success: AfterGetGDName });

    };
//#endregion

    let txtBarcode3_ini = function () {
        $('#txtBarcode3').val('');
        $('#txtBarcode3').focus();
    }

    let afterGetBINWebMod = function (data) {
        if (ReturnMsg(data, 0) != "GetBINWebModOK") {
            DyAlert(ReturnMsg(data, 1), txtBarcode3_ini);
            
        }
        else {
            var dtBin = data.getElementsByTagName('dtBin');
            grdM.BindData(dtBin);
            if (dtBin.length == 0) {
                //alert("No RowData");
                DyAlert("無符合資料!", txtBarcode3_ini);
                $(".modal-backdrop").remove();
                return;
            }
            txtBarcode3_ini()
        }

    };

//#endregion

//#region 新增
    let btQtySave1_click = function () {
        ChkLogOut_1(btQtySave1_click_1)
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
                if ($('#txtQty1').val() != "") {
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
                else {
                    $('#txtQty1').val() == "1"
                }
               
                //$('#btKeyin1').prop('disabled', false);
                //$('#btBCSave1').prop('disabled', false);
                //$('#txtBarcode1').prop('disabled', false);
                //$('#btQtySave1').prop('disabled', true);
                //$('#txtQty1').prop('disabled', true);
                var pData = {
                    Shop: $('#lblShop2').html().split(' ')[0],
                    ISAMDate: $('#lblDate2').html(),
                    BinNo: $('#lblBINNo2').html(),
                    Barcode: $('#txtBarcode1').val() == "" ? $('#lblBarcode').html() : $('#txtBarcode1').val(),
                    Qty: $('#txtQty1').val()
                };
                PostToWebApi({ url: "api/SystemSetup/SaveBINWeb", data: pData, success: afterSaveBINWeb });
            }
        }
    }

    let btKeyin1_click = function () {
        //*
        ChkLogOut(sessionStorage.getItem('isamcomp'))
        Timerset(sessionStorage.getItem('isamcomp'));
        //$('#btKeyin1').prop('disabled', true);
        //$('#btQtySave1').prop('disabled', false);
        //$('#txtQty1').prop('disabled', false);
        //$('#btBCSave1').prop('disabled', true);
        //$('#txtBarcode1').prop('disabled', true);
    };

    let afterSaveBINWeb = function (data) {
        if (ReturnMsg(data, 0) != "SaveBINWebOK") {
            DyAlert(ReturnMsg(data, 1),txtBarcode1_ini);
        }
        else {
            var dtSQ = data.getElementsByTagName('dtSQ');
            if (dtSQ.length > 0) {
                $('#lblSQty1').html(GetNodeValue(dtSQ[0], "SQ1"));
            }
            else {
                $('#lblSQty1').html('');
            }
            var dtSBQ = data.getElementsByTagName('dtSBQ');
            if (dtSBQ.length > 0) {
                $('#lblSBQty1').html(GetNodeValue(dtSBQ[0], "SBQ1"));
            }
            else {
                $('#lblSBQty1').html('');
            }
            var dtSWQ = data.getElementsByTagName('dtSWQ');
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
                $('#txtQty1').val("");
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
                $('#txtQty1').val("");
            }
            //alert(dtBin.length);
            //if (dtBin.length == 0) {
            //    alert("No RowData");
            //    DyAlert("無符合資料!", BlankMode);
            //    return;
            //}
        }
    };

    let txtBarcode1_ini = function () {
        $('#txtBarcode1').val('');
        $('#txtBarcode1').focus();
    }

    let txtQty1_ini = function () {
        $('#txtQty1').val('');
        $('#txtQty1').focus();
    }

    let btBCSave1_click = function () {
        ChkLogOut_1(btBCSave1_click_1)
    };

    let btBCSave1_click_1 = function (data) {

        if (ReturnMsg(data, 0) != "ChkLogOutOK") {
            DyAlert(ReturnMsg(data, 1), txtBarcode1_ini);
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
                $(".modal-backdrop").remove();
                return;
                }

            if ($('#txtBarcode1').val().length > 16) {
                DyAlert("條碼限制輸入16個字元!", txtBarcode1_ini);
                $(".modal-backdrop").remove();
                return;
                }

            if ($('#txtQty1').val() == "" || $('#txtQty1').val() == "0") {
                $('#txtQty1').val("1");
            }
            else {
                    if (isNaN($('#txtQty1').val())) {
                        DyAlert("請輸入數字!", txtQty1_ini );
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
                    Shop: $('#lblShop2').html().split(' ')[0],
                    ISAMDate: $('#lblDate2').html(),
                    BinNo: $('#lblBINNo2').html(),
                    Barcode: $('#txtBarcode1').val(),
                    Qty: $('#txtQty1').val()
                };
                PostToWebApi({ url: "api/SystemSetup/ChkSaveBINWeb", data: pData, success: afterChkSaveBINWeb });
            }
        }
    }

    let afterChkSaveBINWeb = function (data) {
        if (ReturnMsg(data, 0) != "ChkSaveBINWebOK") {
            DyAlert(ReturnMsg(data, 1));
        }
        else {
            var dtBIN = data.getElementsByTagName('dtBIN');
            if (dtBIN.length == 0) {
                if ($('#txtQty1').val() < 0) {
                    DyAlert("單品總數需大於0!", txtQty1_ini);
                    return;
                }
            }
            else {
                if (parseInt(GetNodeValue(dtBIN[0], "SumQty")) + parseInt($('#txtQty1').val()) > 999999) {
                    DyAlert("單品總數不可大於999999!", txtQty1_ini);
                    return;
                }
                if (parseInt(GetNodeValue(dtBIN[0], "SumQty")) + parseInt($('#txtQty1').val()) < 0) {
                    DyAlert("單品總數需大於0!", txtQty1_ini);
                    return;
                }
                var pData = {
                    Shop: $('#lblShop2').html().split(' ')[0],
                    ISAMDate: $('#lblDate2').html(),
                    BinNo: $('#lblBINNo2').html(),
                    Barcode: $('#txtBarcode1').val(),
                    Qty: $('#txtQty1').val()
                };
                PostToWebApi({ url: "api/SystemSetup/SaveBINWeb", data: pData, success: afterSaveBINWeb });
            }
        }
        
    }
    
    let btAdd_click = function () {
        //*
        ChkLogOut(sessionStorage.getItem('isamcomp'));
        EditMode = "A";
        Timerset(sessionStorage.getItem('isamcomp'));
        //$('#btKeyin1').prop('disabled', false);
        //$('#btBCSave1').prop('disabled', false);
        //$('#txtBarcode1').prop('disabled', false);
        //$('#btQtySave1').prop('disabled', true);
        //$('#txtQty1').prop('disabled', true);
        $('#pgISAM01Add').show();
        if ($('#pgISAM01Add').attr('hidden') == undefined) {
            $('#pgISAM01Add').show();
        }
        else {
            $('#pgISAM01Add').removeAttr('hidden');
        }
        if ($('#pgISAM01Mod').attr('hidden') == undefined) {
            $('#pgISAM01Mod').hide();
        }
        BtnSet("A");
        
    };
//#endregion

    let BtnSet = function (edit) {
        switch (edit) {
            case "A":
                $('#btAdd').prop('disabled', false);
                document.getElementById("btAdd").style.background = "blue";
                $('#btMod').prop('disabled', true);
                document.getElementById("btMod").style.background = 'gray';
                $('#btToFTP').prop('disabled', true); //true-btn不能使用
                document.getElementById("btToFTP").style.background = 'gray';

                $('#txtBarcode1').val('');
                $('#txtBarcode1').focus();
                $('#txtQty1').val("");
                $('#lblBarcode').html('');
                $('#lblQty1').html('');
                $('#lblSQty1').html('');
                $('#lblSBQty1').html('');
                $('#lblSWQty1').html('');
                $('#lblPrice').html('');
                $('#lblGDName').html('');
                break;
            case "Q":
                $('#btAdd').prop('disabled', false);
                document.getElementById("btAdd").style.background = "blue";
                $('#btMod').prop('disabled', false);
                document.getElementById("btMod").style.background = "Green";
                $('#btToFTP').prop('disabled', false); //true-btn不能使用
                document.getElementById("btToFTP").style.background = "gold";
                break;
            case "M":
                $('#btAdd').prop('disabled', true);
                document.getElementById("btAdd").style.background = 'gray';
                $('#btMod').prop('disabled', false);
                document.getElementById("btMod").style.background = "Green";
                $('#btToFTP').prop('disabled', true); //true-btn不能使用
                document.getElementById("btToFTP").style.background = 'gray';
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
                    Type: "T",
                    Shop: $('#lblShop2').html().split(' ')[0],
                    ISAMDate: $('#lblDate2').html(),
                    BinNo: $('#lblBINNo2').html()
                }
                PostToWebApi({ url: "api/SystemSetup/AddISAMToFTPRecWeb", data: cData, success: AfterAddISAMToFTPRecWeb });
            }
        }
    }


    let btToFTP_click = function () {
        //*
        ChkLogOut(sessionStorage.getItem('isamcomp'));
        Timerset(sessionStorage.getItem('isamcomp'));
        DyConfirm("是否要上傳" + $('#lblBINNo2').text() + "分區盤點資料？", CallSendToFTP, DummyFunction);
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
        ChkLogOut(sessionStorage.getItem('isamcomp'));

        $('#btAdd').prop('disabled', false);
        document.getElementById("btAdd").style.background = "blue";
        $('#btMod').prop('disabled', false);
        document.getElementById("btMod").style.background = "green";
        $('#btToFTP').prop('disabled', false);
        document.getElementById("btToFTP").style.background = "gold";


        if (EditMode == "Q") {
            $('#ISAM01btns').hide();
            $('#pgISAM01Init').show();
            $('#pgISAM01Add').hide();
            $('#pgISAM01Mod').hide();
            //$('#pgISAM01UpFtp').hide();
        } else if (EditMode == "A" || EditMode == "M") {
            $('#ISAM01btns').show();
            $('#pgISAM01Init').hide();
            $('#pgISAM01Add').hide();
            $('#pgISAM01Mod').hide();
            //$('#pgISAM01UpFtp').hide();
        }
        Timerset(sessionStorage.getItem('isamcomp'));
    };
//#endregion

//#region 輸入盤點日期,分區
    let afterSearchBINWeb = function (data) {
        //EditMode = "Q";
        if (ReturnMsg(data, 0) != "SearchBINWebOK") {
            DyAlert(ReturnMsg(data, 1));
        }
        else {
            var dtBINData = data.getElementsByTagName('dtBINData');
            if (dtBINData.length > 0) {
                if (GetNodeValue(dtBINData[0], "BINman") != $('#lblManID1').html().split(' ')[0]) {
                    DyAlert("盤點人員" + GetNodeValue(dtBINData[0], "BINman") + "已建立分區" + $('#txtBinNo').val() + "之分區單!!", DummyFunction);
                    return;
                }
            }

            //AssignVar();
            //alert(GetNodeValue(dtISAMShop[0], "STName"));
            EditMode = "Q";
            $('#lblShop2').html($('#lblShop1').html());
            $('#lblBINNo2').html($('#txtBinNo').val());
            $('#lblDate2').html($('#txtISAMDate').val());
            $('#lblSWQty1title').html($('#lblShop1').html().split(' ')[0] + "門市總數：");
            $('#lblSBQty1title').html($('#txtBinNo').val() + "分區總數：");
            $('#lblSQty1').html('');
            $('#lblSQty1title').html($('#txtBinNo').val() + "分區單品總數：");
            $('#lblSBQty1').html('');
            $('#lblSWQty1').html('');
            $('#lblPrice').html('');
            $('#lblGDName').html("品名XXX");
            $('#pgISAM01Init').hide();
            if ($('#ISAM01btns').attr('hidden') == undefined) {
                $('#ISAM01btns').show();
            }
            else {
                $('#ISAM01btns').removeAttr('hidden');
            }
        }
    };

    let btSave_click = function () {
        //*
        ChkLogOut(sessionStorage.getItem('isamcomp'));
        Timerset(sessionStorage.getItem('isamcomp'));
        if ($('#txtISAMDate').val() == "" | $('#txtISAMDate').val() == null) {
            DyAlert("請輸入盤點日期!!", function () { $('#txtISAMDate').focus() });
            $(".modal-backdrop").remove();
            return;
        }
        if ($('#txtBinNo').val() == "" | $('#txtBinNo').val() == null) {
            DyAlert("請輸入分區代碼!!", function () { $('#txtBinNo').focus() });
            $(".modal-backdrop").remove();
            return;
        }
        else {
            if ($('#txtBinNo').val().length>3) {
                DyAlert("分區代碼不可超過3個字元!!", function () { $('#txtBinNo').focus() });
                $(".modal-backdrop").remove();
                return;
            }
        }
        var pData = {
            Shop: $('#lblShop1').html().split(' ')[0],
            ISAMDate: $('#txtISAMDate').val(),
            BinNo: $('#txtBinNo').val()
        }
        PostToWebApi({ url: "api/SystemSetup/SearchBINWeb", data: pData, success: afterSearchBINWeb });
    };
//#endregion

    let Query1_click = function () {
        if ($('#OpenDateS').val() == "" | $('#OpenDateS').val() == null) {
            DyAlert("請輸入日期!!", function () { $('#OpenDateS').focus() });
            $(".modal-backdrop").remove();
            return;
        }
        if ($('#OpenDateE').val() == "" | $('#OpenDateE').val() == null) {
            DyAlert("請輸入日期!!", function () { $('#OpenDateE').focus() });
            $(".modal-backdrop").remove();
            return;
        }

        ShowLoading();

        if ($('#txtShop1').val() == "") {
            var Type = "";
            if ($('#rdoArea').prop('checked')) {
                Type = "A";
            }
            else if ($('#rdoShop').prop('checked')) {
                Type = "S";
            }
            else if ($('#rdoDate').prop('checked')) {
                Type = "D";
            }

            var pData = {
                OpenDateS: $('#OpenDateS').val(),
                OpenDateE: $('#OpenDateE').val(),
                Type: Type
            }
            PostToWebApi({ url: "api/Query1", data: pData, success: afterQuery1 });
        }
        else {
            var pData = {
                Shop: $('#txtShop1').val()
            }
            PostToWebApi({ url: "api/ChkQuery1_Shop", data: pData, success: afterChkQuery1_Shop });
        }
    };

    let afterChkQuery1_Shop = function (data) {
        if (ReturnMsg(data, 0) != "ChkQuery1_ShopOK") {
            CloseLoading();
            DyAlert(ReturnMsg(data, 1));
        }
        else {
            var dtW = data.getElementsByTagName('dtW');
            if (dtW.length == 0) {
                DyAlert("無符合店櫃資料，請重新確認!");
                $(".modal-backdrop").remove();
                return;
            }

            $('#lblOpenDate_Shop').html($('#OpenDateS').val() + "~" + $('#OpenDateE').val());
            $('#lblShop').html(GetNodeValue(dtW[0], "st_placeid") + ' ' + GetNodeValue(dtW[0], "type_name") + '-' + GetNodeValue(dtW[0], "st_id") + ' ' + GetNodeValue(dtW[0], "st_sname"));
            $('#modal_Shop').modal('show');
            setTimeout(function () {
                Query1_Shop_click();
            }, 500);
        }
    };

    let Query1_Shop_click = function () {
        var pData = {
            OpenDateS: $('#OpenDateS').val(),
            OpenDateE: $('#OpenDateE').val(),
            Shop: $('#txtShop1').val()
        }
        PostToWebApi({ url: "api/Query1_Shop", data: pData, success: afterQuery1_Shop });
    };

    let afterQuery1_Shop = function (data) {
        CloseLoading();
        if (ReturnMsg(data, 0) != "Query1_ShopOK") {
            DyAlert(ReturnMsg(data, 1));
        }
        else {
            var dtQ = data.getElementsByTagName('dtQ');
            grdM_Shop.BindData(dtQ);
            
            if (dtQ.length == 0) {
                DyAlert("無符合資料!");
                $(".modal-backdrop").remove();
                return;
            }
            var dtSumQ = data.getElementsByTagName('dtSumQ');
            $('#tbSales_Shop thead td#td1').html(parseInt(GetNodeValue(dtSumQ[0], "SumCash1")).toLocaleString('en-US'));
            $('#tbSales_Shop thead td#td2').html(parseInt(GetNodeValue(dtSumQ[0], "SumCnt1")).toLocaleString('en-US'));
            $('#tbSales_Shop thead td#td3').html(parseInt(GetNodeValue(dtSumQ[0], "SumCashCnt1")).toLocaleString('en-US'));
            $('#tbSales_Shop thead td#td4').html(parseInt(GetNodeValue(dtSumQ[0], "SumCash2")).toLocaleString('en-US'));
            $('#tbSales_Shop thead td#td5').html(parseInt(GetNodeValue(dtSumQ[0], "SumCnt2")).toLocaleString('en-US'));
            $('#tbSales_Shop thead td#td6').html(parseInt(GetNodeValue(dtSumQ[0], "SumCashCnt2")).toLocaleString('en-US'));
            $('#tbSales_Shop thead td#td7').html(parseInt(GetNodeValue(dtSumQ[0], "SumVIPPercent")).toLocaleString('en-US') + '%');
        }
    };

    let afterQuery1 = function (data) {
        CloseLoading();

        if (ReturnMsg(data, 0) != "Query1OK") {
            DyAlert(ReturnMsg(data, 1));
        }
        else {
           
            var dtQ = data.getElementsByTagName('dtQ');
            grdM.BindData(dtQ);

            var heads = $('#tbSales1 thead tr th#thtype');
            if ($('#rdoArea').prop('checked')) {
                $(heads).html('區');
            }
            else if ($('#rdoShop').prop('checked')) {
                $(heads).html('店');
            }
            else if ($('#rdoDate').prop('checked')) {
                $(heads).html('日');
            }
            
            if (dtQ.length == 0) {
                DyAlert("無符合資料!");
                $(".modal-backdrop").remove();
                return;
            }
            var dtSumQ = data.getElementsByTagName('dtSumQ');
            $('#tbSales1 thead td#td1').html(parseInt(GetNodeValue(dtSumQ[0], "SumCash1")).toLocaleString('en-US'));
            $('#tbSales1 thead td#td2').html(parseInt(GetNodeValue(dtSumQ[0], "SumCnt1")).toLocaleString('en-US'));
            $('#tbSales1 thead td#td3').html(parseInt(GetNodeValue(dtSumQ[0], "SumCashCnt1")).toLocaleString('en-US'));
            $('#tbSales1 thead td#td4').html(parseInt(GetNodeValue(dtSumQ[0], "SumCash2")).toLocaleString('en-US'));
            $('#tbSales1 thead td#td5').html(parseInt(GetNodeValue(dtSumQ[0], "SumCnt2")).toLocaleString('en-US'));
            $('#tbSales1 thead td#td6').html(parseInt(GetNodeValue(dtSumQ[0], "SumCashCnt2")).toLocaleString('en-US'));
            $('#tbSales1 thead td#td7').html(parseInt(GetNodeValue(dtSumQ[0], "SumVIPPercent")).toLocaleString('en-US') + '%');
        }
    };

    let Query_Area_Step1_click = function () {
        ShowLoading();

        var Type_Step1 = "";
        if ($('#rdoShop_Area_Step1').prop('checked')) {
            Type_Step1 = "S";
        }
        else if ($('#rdoDate_Area_Step1').prop('checked')) {
            Type_Step1 = "D";
        }

        var pData = {
            OpenDateS: $('#lblOpenDate_Area_Step1').html().split('~')[0],
            OpenDateE: $('#lblOpenDate_Area_Step1').html().split('~')[1],
            Area: $('#lblArea_Step1').html().split(' ')[0],
            Type_Step1: Type_Step1
        }
        PostToWebApi({ url: "api/Query_Area_Step1", data: pData, success: afterQuery_Area_Step1 });
    };

    let afterQuery_Area_Step1 = function (data) {
        CloseLoading();
        if (ReturnMsg(data, 0) != "Query_Area_Step1OK") {
            DyAlert(ReturnMsg(data, 1));
        }
        else {
            
            var dtQ = data.getElementsByTagName('dtQ');
            grdM_Area_Step1.BindData(dtQ);

            var heads = $('#tbSales_Area_Step1 thead tr th#thtype');
            if ($('#rdoShop_Area_Step1').prop('checked')) {
                $(heads).text('店');
            }
            else if ($('#rdoDate_Area_Step1').prop('checked')) {
                $(heads).text('日');
            }

            if (dtQ.length == 0) {
                DyAlert("無符合資料!");
                $(".modal-backdrop").remove();
                return;
            }

            var dtSumQ = data.getElementsByTagName('dtSumQ');
            $('#tbSales_Area_Step1 thead td#td1').html(parseInt(GetNodeValue(dtSumQ[0], "SumCash1")).toLocaleString('en-US'));
            $('#tbSales_Area_Step1 thead td#td2').html(parseInt(GetNodeValue(dtSumQ[0], "SumCnt1")).toLocaleString('en-US'));
            $('#tbSales_Area_Step1 thead td#td3').html(parseInt(GetNodeValue(dtSumQ[0], "SumCashCnt1")).toLocaleString('en-US'));
            $('#tbSales_Area_Step1 thead td#td4').html(parseInt(GetNodeValue(dtSumQ[0], "SumCash2")).toLocaleString('en-US'));
            $('#tbSales_Area_Step1 thead td#td5').html(parseInt(GetNodeValue(dtSumQ[0], "SumCnt2")).toLocaleString('en-US'));
            $('#tbSales_Area_Step1 thead td#td6').html(parseInt(GetNodeValue(dtSumQ[0], "SumCashCnt2")).toLocaleString('en-US'));
            $('#tbSales_Area_Step1 thead td#td7').html(parseInt(GetNodeValue(dtSumQ[0], "SumVIPPercent")).toLocaleString('en-US') + '%');

        }
    };

    let Query_Area_Shop_Step2_click = function () {
        ShowLoading();
        var Area = $('#lblArea_Shop_Step2').html().split('-')[0].split(' ')[0];
        var Shop = $('#lblArea_Shop_Step2').html().split('-')[1].split(' ')[0];

        var pData = {
            OpenDateS: $('#lblOpenDate_Area_Shop_Step2').html().split('~')[0],
            OpenDateE: $('#lblOpenDate_Area_Shop_Step2').html().split('~')[1],
            Area: Area,
            Shop: Shop
        }
        PostToWebApi({ url: "api/Query_Area_Shop_Step2", data: pData, success: afterQuery_Area_Shop_Step2 });
    };

    let afterQuery_Area_Shop_Step2 = function (data) {
        CloseLoading();
        if (ReturnMsg(data, 0) != "Query_Area_Shop_Step2OK") {
            DyAlert(ReturnMsg(data, 1));
        }
        else {

            var dtQ = data.getElementsByTagName('dtQ');
            grdM_Area_Shop_Step2.BindData(dtQ);

            if (dtQ.length == 0) {
                DyAlert("無符合資料!");
                $(".modal-backdrop").remove();
                return;
            }

            var dtSumQ = data.getElementsByTagName('dtSumQ');
            $('#tbSales_Area_Shop_Step2 thead td#td1').html(parseInt(GetNodeValue(dtSumQ[0], "SumCash1")).toLocaleString('en-US'));
            $('#tbSales_Area_Shop_Step2 thead td#td2').html(parseInt(GetNodeValue(dtSumQ[0], "SumCnt1")).toLocaleString('en-US'));
            $('#tbSales_Area_Shop_Step2 thead td#td3').html(parseInt(GetNodeValue(dtSumQ[0], "SumCashCnt1")).toLocaleString('en-US'));
            $('#tbSales_Area_Shop_Step2 thead td#td4').html(parseInt(GetNodeValue(dtSumQ[0], "SumCash2")).toLocaleString('en-US'));
            $('#tbSales_Area_Shop_Step2 thead td#td5').html(parseInt(GetNodeValue(dtSumQ[0], "SumCnt2")).toLocaleString('en-US'));
            $('#tbSales_Area_Shop_Step2 thead td#td6').html(parseInt(GetNodeValue(dtSumQ[0], "SumCashCnt2")).toLocaleString('en-US'));
            $('#tbSales_Area_Shop_Step2 thead td#td7').html(parseInt(GetNodeValue(dtSumQ[0], "SumVIPPercent")).toLocaleString('en-US') + '%');
        }
    };

    let Query_Area_Date_Step2_click = function () {
        ShowLoading();
        var Area = $('#lblArea_Date_Step2').html().split('-')[0].split(' ')[0];
        var Date = $('#lblArea_Date_Step2').html().split('-')[1];

        var pData = {
            OpenDate: Date,
            Area: Area
        }
        PostToWebApi({ url: "api/Query_Area_Date_Step2", data: pData, success: afterQuery_Area_Date_Step2 });
    };

    let afterQuery_Area_Date_Step2 = function (data) {
        CloseLoading();
        if (ReturnMsg(data, 0) != "Query_Area_Date_Step2OK") {
            DyAlert(ReturnMsg(data, 1));
        }
        else {

            var dtQ = data.getElementsByTagName('dtQ');
            grdM_Area_Date_Step2.BindData(dtQ);

            if (dtQ.length == 0) {
                DyAlert("無符合資料!");
                $(".modal-backdrop").remove();
                return;
            }

            var dtSumQ = data.getElementsByTagName('dtSumQ');
            $('#tbSales_Area_Date_Step2 thead td#td1').html(parseInt(GetNodeValue(dtSumQ[0], "SumCash1")).toLocaleString('en-US'));
            $('#tbSales_Area_Date_Step2 thead td#td2').html(parseInt(GetNodeValue(dtSumQ[0], "SumCnt1")).toLocaleString('en-US'));
            $('#tbSales_Area_Date_Step2 thead td#td3').html(parseInt(GetNodeValue(dtSumQ[0], "SumCashCnt1")).toLocaleString('en-US'));
            $('#tbSales_Area_Date_Step2 thead td#td4').html(parseInt(GetNodeValue(dtSumQ[0], "SumCash2")).toLocaleString('en-US'));
            $('#tbSales_Area_Date_Step2 thead td#td5').html(parseInt(GetNodeValue(dtSumQ[0], "SumCnt2")).toLocaleString('en-US'));
            $('#tbSales_Area_Date_Step2 thead td#td6').html(parseInt(GetNodeValue(dtSumQ[0], "SumCashCnt2")).toLocaleString('en-US'));
            $('#tbSales_Area_Date_Step2 thead td#td7').html(parseInt(GetNodeValue(dtSumQ[0], "SumVIPPercent")).toLocaleString('en-US') + '%');
        }
    };

    let Query_Shop_Step1_click = function () {
        ShowLoading();
        var pData = {
            OpenDateS: $('#lblOpenDate_Shop_Step1').html().split('~')[0],
            OpenDateE: $('#lblOpenDate_Shop_Step1').html().split('~')[1],
            Shop: $('#lblShop_Step1').html().split(' ')[0]
        }
        PostToWebApi({ url: "api/Query_Shop_Step1", data: pData, success: afterQuery_Shop_Step1 });
    };

    let afterQuery_Shop_Step1 = function (data) {
        CloseLoading();
        if (ReturnMsg(data, 0) != "Query_Shop_Step1OK") {
            DyAlert(ReturnMsg(data, 1));
        }
        else {
            var dtQ = data.getElementsByTagName('dtQ');
            grdM_Shop_Step1.BindData(dtQ);

            if (dtQ.length == 0) {
                DyAlert("無符合資料!");
                $(".modal-backdrop").remove();
                return;
            }
            var dtSumQ = data.getElementsByTagName('dtSumQ');
            $('#tbSales_Shop_Step1 thead td#td1').html(parseInt(GetNodeValue(dtSumQ[0], "SumCash1")).toLocaleString('en-US'));
            $('#tbSales_Shop_Step1 thead td#td2').html(parseInt(GetNodeValue(dtSumQ[0], "SumCnt1")).toLocaleString('en-US'));
            $('#tbSales_Shop_Step1 thead td#td3').html(parseInt(GetNodeValue(dtSumQ[0], "SumCashCnt1")).toLocaleString('en-US'));
            $('#tbSales_Shop_Step1 thead td#td4').html(parseInt(GetNodeValue(dtSumQ[0], "SumCash2")).toLocaleString('en-US'));
            $('#tbSales_Shop_Step1 thead td#td5').html(parseInt(GetNodeValue(dtSumQ[0], "SumCnt2")).toLocaleString('en-US'));
            $('#tbSales_Shop_Step1 thead td#td6').html(parseInt(GetNodeValue(dtSumQ[0], "SumCashCnt2")).toLocaleString('en-US'));
            $('#tbSales_Shop_Step1 thead td#td7').html(parseInt(GetNodeValue(dtSumQ[0], "SumVIPPercent")).toLocaleString('en-US') + '%');
        }
    };

    let Query_Date_Step1_click = function () {
        ShowLoading();
        var Type_Step1 = "";
        if ($('#rdoArea_Date_Step1').prop('checked')) {
            Type_Step1 = "A";
        }
        else if ($('#rdoShop_Date_Step1').prop('checked')) {
            Type_Step1 = "S";
        }

        var pData = {
            OpenDateS: $('#lblOpenDate_Date_Step1').html().split('~')[0],
            OpenDateE: $('#lblOpenDate_Date_Step1').html().split('~')[1],
            Date: $('#lblDate_Step1').html(),
            Type_Step1: Type_Step1
        }
        PostToWebApi({ url: "api/Query_Date_Step1", data: pData, success: afterQuery_Date_Step1 });
    };

    let afterQuery_Date_Step1 = function (data) {
        CloseLoading();
        if (ReturnMsg(data, 0) != "Query_Date_Step1OK") {
            DyAlert(ReturnMsg(data, 1));
        }
        else {

            var dtQ = data.getElementsByTagName('dtQ');
            grdM_Date_Step1.BindData(dtQ);

            var heads = $('#tbSales_Date_Step1 thead tr th#thtype');
            
            if ($('#rdoArea_Date_Step1').prop('checked')) {
                $(heads).text('區');
            }
            else if ($('#rdoShop_Date_Step1').prop('checked')) {
                $(heads).text('店');
            }

            if (dtQ.length == 0) {
                DyAlert("無符合資料!");
                $(".modal-backdrop").remove();
                return;
            }

            var dtSumQ = data.getElementsByTagName('dtSumQ');
            $('#tbSales_Date_Step1 thead td#td1').html(parseInt(GetNodeValue(dtSumQ[0], "SumCash1")).toLocaleString('en-US'));
            $('#tbSales_Date_Step1 thead td#td2').html(parseInt(GetNodeValue(dtSumQ[0], "SumCnt1")).toLocaleString('en-US'));
            $('#tbSales_Date_Step1 thead td#td3').html(parseInt(GetNodeValue(dtSumQ[0], "SumCashCnt1")).toLocaleString('en-US'));
            $('#tbSales_Date_Step1 thead td#td4').html(parseInt(GetNodeValue(dtSumQ[0], "SumCash2")).toLocaleString('en-US'));
            $('#tbSales_Date_Step1 thead td#td5').html(parseInt(GetNodeValue(dtSumQ[0], "SumCnt2")).toLocaleString('en-US'));
            $('#tbSales_Date_Step1 thead td#td6').html(parseInt(GetNodeValue(dtSumQ[0], "SumCashCnt2")).toLocaleString('en-US'));
            $('#tbSales_Date_Step1 thead td#td7').html(parseInt(GetNodeValue(dtSumQ[0], "SumVIPPercent")).toLocaleString('en-US') + '%');
        }
    };

    let Query_Date_Area_Step2_click = function () {
        ShowLoading();
        var pData = {
            OpenDateS: $('#lblOpenDate_Date_Area_Step2').html().split('~')[0],
            OpenDateE: $('#lblOpenDate_Date_Area_Step2').html().split('~')[1],
            Date: $('#lblDate_Area_Step2').html().split('-')[0],
            Area: $('#lblDate_Area_Step2').html().split('-')[1].split(' ')[0]
        }
        PostToWebApi({ url: "api/Query_Date_Area_Step2", data: pData, success: afterQuery_Date_Area_Step2 });
    };

    let afterQuery_Date_Area_Step2 = function (data) {
        CloseLoading();
        if (ReturnMsg(data, 0) != "Query_Date_Area_Step2OK") {
            DyAlert(ReturnMsg(data, 1));
        }
        else {
            var dtQ = data.getElementsByTagName('dtQ');
            grdM_Date_Area_Step2.BindData(dtQ);
            if (dtQ.length == 0) {
                DyAlert("無符合資料!");
                $(".modal-backdrop").remove();
                return;
            }

            var dtSumQ = data.getElementsByTagName('dtSumQ');
            $('#tbSales_Date_Area_Step2 thead td#td1').html(parseInt(GetNodeValue(dtSumQ[0], "SumCash1")).toLocaleString('en-US'));
            $('#tbSales_Date_Area_Step2 thead td#td2').html(parseInt(GetNodeValue(dtSumQ[0], "SumCnt1")).toLocaleString('en-US'));
            $('#tbSales_Date_Area_Step2 thead td#td3').html(parseInt(GetNodeValue(dtSumQ[0], "SumCashCnt1")).toLocaleString('en-US'));
            $('#tbSales_Date_Area_Step2 thead td#td4').html(parseInt(GetNodeValue(dtSumQ[0], "SumCash2")).toLocaleString('en-US'));
            $('#tbSales_Date_Area_Step2 thead td#td5').html(parseInt(GetNodeValue(dtSumQ[0], "SumCnt2")).toLocaleString('en-US'));
            $('#tbSales_Date_Area_Step2 thead td#td6').html(parseInt(GetNodeValue(dtSumQ[0], "SumCashCnt2")).toLocaleString('en-US'));
            $('#tbSales_Date_Area_Step2 thead td#td7').html(parseInt(GetNodeValue(dtSumQ[0], "SumVIPPercent")).toLocaleString('en-US') + '%');
        }
    };

    //清除
    let btClear_click = function (bt) {
        //Timerset();
        $('#txtActivityCode').val('');
        $('#txtPSName').val('');
        $('#txtEDDate').val('');
    };

    //查詢
    let btQuery_click = function (bt) {
        //Timerset();
        $('#btQuery').prop('disabled', true)
        ShowLoading();
        var pData = {
            ActivityCode: $('#txtActivityCode').val(),
            PSName: $('#txtPSName').val(),
            EDDate: $('#txtEDDate').val().toString().replaceAll('-', '/')
        }
        PostToWebApi({ url: "api/SystemSetup/MSSD102Query", data: pData, success: afterMSSD102Query });
    };

    let afterMSSD102Query = function (data) {
        CloseLoading();
        if (ReturnMsg(data, 0) != "MSSD102QueryOK") {
            DyAlert(ReturnMsg(data, 1), function () { $('#btQuery').prop('disabled', false); });
        }
        else {
            $('#btQuery').prop('disabled', false);
            var dtE = data.getElementsByTagName('dtE');
            grdM.BindData(dtE);

            if (dtE.length == 0) {
                DyAlert("無符合資料!");
                $(".modal-backdrop").remove();
                return;
            }
        }
    };

    //活動代號...
    let btActivityCode_click = function (bt) {
        //Timerset();
        var pData = {
            ActivityCode: $('#txtActivityCode').val()
        }
        PostToWebApi({ url: "api/SystemSetup/MSSD102_LookUpActivityCode", data: pData, success: afterMSSD102_LookUpActivityCode });
    };

    let afterMSSD102_LookUpActivityCode = function (data) {
        if (ReturnMsg(data, 0) != "MSSD102_LookUpActivityCodeOK") {
            DyAlert(ReturnMsg(data, 1));
        }
        else {
            var dtE = data.getElementsByTagName('dtE');
            $('#txtQLookup_ActivityCode').val($('#txtActivityCode').val());
            $('#modal_Lookup_ActivityCode').modal('show');
            setTimeout(function () {
                grdLookUp_ActivityCode.BindData(dtE);
                $('#tbDataLookup_ActivityCode tbody tr .tdCol2').filter(function () { return $(this).text() == $('#txtActivityCode').val(); }).closest('tr').find('.tdCol1 input:radio').prop('checked', true);
            }, 500);
        }
    };

    let btQLookup_ActivityCode_click = function (bt) {
        //Timerset();
        $('#btQLookup_ActivityCode').prop('disabled', true)
        var pData = {
            ActivityCode: $('#txtQLookup_ActivityCode').val()
        }
        PostToWebApi({ url: "api/SystemSetup/MSSD102_LookUpActivityCode", data: pData, success: afterMSSD102_QLookUpActivityCode });
    };

    let afterMSSD102_QLookUpActivityCode = function (data) {
        if (ReturnMsg(data, 0) != "MSSD102_LookUpActivityCodeOK") {
            $('#modal_Lookup_ActivityCode').modal('hide');
            DyAlert(ReturnMsg(data, 1), function () {
                $('#btQLookup_ActivityCode').prop('disabled', false);
                $('#modal_Lookup_ActivityCode').modal('show');
            });
        }
        else {
            var dtE = data.getElementsByTagName('dtE');
            if (dtE.length == 0) {
                $('#modal_Lookup_ActivityCode').modal('hide');
                DyAlert("無符合資料!", function () {
                    $('#btQLookup_ActivityCode').prop('disabled', false);
                    $('#modal_Lookup_ActivityCode').modal('show');
                });
                $(".modal-backdrop").remove();
                return;
            }
            setTimeout(function () {
                grdLookUp_ActivityCode.BindData(dtE);
                $('#btQLookup_ActivityCode').prop('disabled', false);
            }, 500);
        }
    };

    let btLpOK_ActivityCode_click = function (bt) {
        //Timerset();
        $('#btLpOK_ActivityCode').prop('disabled', true);
        var obchkedtd = $('#tbDataLookup_ActivityCode input[type="radio"]:checked');
        var chkedRow = obchkedtd.length.toString();

        if (chkedRow == 0) {
            $('#modal_Lookup_ActivityCode').modal('hide');
            DyAlert("未選取活動代號，請重新確認!", function () {
                $('#btLpOK_ActivityCode').prop('disabled', false);
                $('#modal_Lookup_ActivityCode').modal('show');
            });
        }
        else {
            var a = $(obchkedtd[0]).closest('tr');
            var trNode = $(a).prop('Record');
            $('#txtActivityCode').val(GetNodeValue(trNode, "ActivityCode"))
            $('#btLpOK_ActivityCode').prop('disabled', false);
            $('#modal_Lookup_ActivityCode').modal('hide')
        }
    };

    let btLpExit_ActivityCode_click = function (bt) {
        //Timerset();
        $('#modal_Lookup_ActivityCode').modal('hide')
    };

    let btRe_PSNO_click = function (bt) {
        //Timerset();
        $('#modal_PSNO').modal('hide')
    };
//#region FormLoad
    let GetInitMSSD102 = function (data) {
        if (ReturnMsg(data, 0) != "GetInitmsDMOK") {
            DyAlert(ReturnMsg(data, 1));
        }
       else {
           var dtE = data.getElementsByTagName('dtE');
            $('#lblProgramName').html(GetNodeValue(dtE[0], "ChineseName"));
            AssignVar();

            $('#btQuery').click(function () { btQuery_click(this) });
            $('#btClear').click(function () { btClear_click(this) });
            $('#btActivityCode').click(function () { btActivityCode_click(this) });
            $('#btQLookup_ActivityCode').click(function () { btQLookup_ActivityCode_click(this) });
            $('#btLpOK_ActivityCode').click(function () { btLpOK_ActivityCode_click(this) });
            $('#btLpExit_ActivityCode').click(function () { btLpExit_ActivityCode_click(this) });

            $('#btRe_PSNO').click(function () { btRe_PSNO_click(this) });
            $('#rdoShop_PSNO,#rdoDate_PSNO').change(function () { QueryPSNO($('#lblPSNO_PSNO').html()) });
        }
    };
    
    let afterLoadPage = function () {
        var pData = {
            ProgramID: "MSSD102"
        }
        PostToWebApi({ url: "api/SystemSetup/GetInitmsDM", data: pData, success: GetInitMSSD102 });
    };
//#endregion
    

    if ($('#pgMSSD102').length == 0) {  
        AllPages = new LoadAllPages(ParentNode, "SystemSetup/MSSD102", ["MSSD102btns", "pgMSSD102Init", "pgMSSD102Add", "pgMSSD102Mod"], afterLoadPage);
    };


}