var PageMSSD105 = function (ParentNode) {

    let grdM;
    let grdShop1;
    let grdDate1;
    let SysYM = "";

    let AssignVar = function () {

        grdM = new DynGrid(
            {
                table_lement: $('#tbQuery')[0],
                class_collection: ["tdCol1 text-center", "tdCol2 label-align", "tdCol3 label-align", "tdCol4 label-align", "tdCol5 label-align", "tdCol6 label-align", "tdCol7 label-align", "tdCol8 label-align", "tdCol9 text-center", "tdCol10 label-align", "tdCol11 label-align", "tdCol12 label-align", "tdCol13 text-center"],
                fields_info: [
                    { type: "Text", name: "ID", style: "" },
                    { type: "TextAmt", name: "VIPCnt"},
                    { type: "TextAmt", name: "SalesCash1"},
                    { type: "TextAmt", name: "SalesCnt1"},
                    { type: "TextAmt", name: "SalesPrice1"},
                    { type: "TextAmt", name: "SalesCash2"},
                    { type: "TextAmt", name: "SalesCnt2"},
                    { type: "TextAmt", name: "SalesPrice2" },
                    { type: "TextPercent", name: "SalesPercent2" },
                    { type: "TextAmt", name: "SalesCash3" },
                    { type: "TextAmt", name: "SalesCnt3" },
                    { type: "TextAmt", name: "SalesPrice3" },
                    { type: "TextPercent", name: "SalesPercent3" }
                ],
                //rows_per_page: 10,
                method_clickrow: click_PLU,
                afterBind: InitModifyDeleteButton,
                sortable: "Y",
                step: "Y"
            }
        );

        grdShop1 = new DynGrid(
            {
                table_lement: $('#tbShop1')[0],
                class_collection: ["tdCol1 text-center", "tdCol2 label-align", "tdCol3 label-align", "tdCol4 label-align", "tdCol5 label-align", "tdCol6 label-align", "tdCol7 label-align", "tdCol8 label-align", "tdCol9 text-center", "tdCol10 label-align", "tdCol11 label-align", "tdCol12 label-align", "tdCol13 text-center"],
                fields_info: [
                    { type: "Text", name: "ID", style: "" },
                    { type: "TextAmt", name: "VIPCnt" },
                    { type: "TextAmt", name: "SalesCash1" },
                    { type: "TextAmt", name: "SalesCnt1" },
                    { type: "TextAmt", name: "SalesPrice1" },
                    { type: "TextAmt", name: "SalesCash2" },
                    { type: "TextAmt", name: "SalesCnt2" },
                    { type: "TextAmt", name: "SalesPrice2" },
                    { type: "TextPercent", name: "SalesPercent2" },
                    { type: "TextAmt", name: "SalesCash3" },
                    { type: "TextAmt", name: "SalesCnt3" },
                    { type: "TextAmt", name: "SalesPrice3" },
                    { type: "TextPercent", name: "SalesPercent3" }
                ],
                //rows_per_page: 10,
                method_clickrow: click_PLU,
                sortable: "Y"
            }
        );

        grdDate1 = new DynGrid(
            {
                table_lement: $('#tbDate1')[0],
                class_collection: ["tdCol1 text-center", "tdCol2 label-align", "tdCol3 label-align", "tdCol4 label-align", "tdCol5 label-align", "tdCol6 label-align", "tdCol7 label-align", "tdCol8 label-align", "tdCol9 text-center", "tdCol10 label-align", "tdCol11 label-align", "tdCol12 label-align", "tdCol13 text-center"],
                fields_info: [
                    { type: "Text", name: "ID", style: "" },
                    { type: "TextAmt", name: "VIPCnt" },
                    { type: "TextAmt", name: "SalesCash1" },
                    { type: "TextAmt", name: "SalesCnt1" },
                    { type: "TextAmt", name: "SalesPrice1" },
                    { type: "TextAmt", name: "SalesCash2" },
                    { type: "TextAmt", name: "SalesCnt2" },
                    { type: "TextAmt", name: "SalesPrice2" },
                    { type: "TextPercent", name: "SalesPercent2" },
                    { type: "TextAmt", name: "SalesCash3" },
                    { type: "TextAmt", name: "SalesCnt3" },
                    { type: "TextAmt", name: "SalesPrice3" },
                    { type: "TextPercent", name: "SalesPercent3" }
                ],
                //rows_per_page: 10,
                method_clickrow: click_PLU,
                sortable: "Y"
            }
        );

        return;
    };

    let click_PLU = function (tr) {

    };
    let InitModifyDeleteButton = function () {
        $('#tbQuery tbody tr td').click(function () { Step1_click(this) });
    }
    let ChkLogOut_1 = function (AfterChkLogOut_1) {
        var LoginDT = sessionStorage.getItem('LoginDT');
        var cData = {
            LoginDT: LoginDT
        }
        PostToWebApi({ url: "api/js/ChkLogOut", data: cData, success: AfterChkLogOut_1 });
    };
//#endregion
    //清除
    let btClear_click = function (bt) {
        //Timerset();
        $('#txtYM').val(SysYM);
        $('#rdoShop').prop('checked','true');
    };

    //查詢
    let btQuery_click = function (bt) {
        //Timerset();
        //$('#tbQuery thead tr th').css('background-color', '#ffb620')
        $('#btQuery').prop('disabled', true)
        if ($('#txtYM').val() == "") {
            DyAlert("請輸入年月!", function () { $('#btQuery').prop('disabled', false); })
            return
        }
        if ($('#rdoShop').prop('checked') == false && $('#rdoDate').prop('checked') == false) {
            DyAlert("統計條件請至少選擇一個項目!", function () { $('#btQuery').prop('disabled', false); })
            return
        }

        ShowLoading();

        setTimeout(function () {
            var Flag = "";
            if ($('#rdoShop').prop('checked') == true) {
                Flag = "S"
            }
            else if ($('#rdoDate').prop('checked') == true) {
                Flag = "D"
            }

            var pData = {
                CountYM: $('#txtYM').val().toString().replaceAll('-', '/'),
                Flag: Flag
            }
            PostToWebApi({ url: "api/SystemSetup/MSSD105Query", data: pData, success: afterMSSD105Query });
        }, 1000);
    };

    let afterMSSD105Query = function (data) {
        CloseLoading();
        if (ReturnMsg(data, 0) != "MSSD105QueryOK") {
            DyAlert(ReturnMsg(data, 1), function () { $('#btQuery').prop('disabled', false); });
        }
        else {
            $('#btQuery').prop('disabled', false);
            var dtE = data.getElementsByTagName('dtE');
            grdM.BindData(dtE);

            var heads = $('#tbQuery thead tr th#tdflag');
            if ($('#rdoShop').prop('checked')) {
                $(heads).html('店別');
            }
            else if ($('#rdoDate').prop('checked')) {
                $(heads).html('日期');
            }

            if (dtE.length == 0) {
                DyAlert("無符合資料!");
                $(".modal-backdrop").remove();
                $('#tbQuery thead td#td1').html('');
                $('#tbQuery thead td#td2').html('');
                $('#tbQuery thead td#td3').html('');
                $('#tbQuery thead td#td4').html('');
                $('#tbQuery thead td#td5').html('');
                $('#tbQuery thead td#td6').html('');
                $('#tbQuery thead td#td7').html('');
                $('#tbQuery thead td#td8').html('');
                $('#tbQuery thead td#td9').html('');
                $('#tbQuery thead td#td10').html('');
                $('#tbQuery thead td#td11').html('');
                $('#tbQuery thead td#td12').html('');
                return;
            }

            //$('#tbQuery tbody tr .tdCol9').html($('#tbQuery tbody tr .tdCol9').html() + '%')
            //$('#tbQuery tbody tr .tdCol13').html($('#tbQuery tbody tr .tdCol13').html() + '%')

            var dtSumQ = data.getElementsByTagName('dtSumQ');
            $('#tbQuery thead td#td1').html(parseInt(GetNodeValue(dtSumQ[0], "SumVIPCnt")).toLocaleString('en-US'));
            $('#tbQuery thead td#td2').html(parseInt(GetNodeValue(dtSumQ[0], "SumSalesCash1")).toLocaleString('en-US'));
            $('#tbQuery thead td#td3').html(parseInt(GetNodeValue(dtSumQ[0], "SumSalesCnt1")).toLocaleString('en-US'));
            $('#tbQuery thead td#td4').html(parseInt(GetNodeValue(dtSumQ[0], "SumSalesPrice1")).toLocaleString('en-US'));
            $('#tbQuery thead td#td5').html(parseInt(GetNodeValue(dtSumQ[0], "SumSalesCash2")).toLocaleString('en-US'));
            $('#tbQuery thead td#td6').html(parseInt(GetNodeValue(dtSumQ[0], "SumSalesCnt2")).toLocaleString('en-US'));
            $('#tbQuery thead td#td7').html(parseInt(GetNodeValue(dtSumQ[0], "SumSalesPrice2")).toLocaleString('en-US'));
            $('#tbQuery thead td#td8').html(GetNodeValue(dtSumQ[0], "SumSalesPercent2"));
            $('#tbQuery thead td#td9').html(parseInt(GetNodeValue(dtSumQ[0], "SumSalesCash3")).toLocaleString('en-US'));
            $('#tbQuery thead td#td10').html(parseInt(GetNodeValue(dtSumQ[0], "SumSalesCnt3")).toLocaleString('en-US'));
            $('#tbQuery thead td#td11').html(parseInt(GetNodeValue(dtSumQ[0], "SumSalesPrice3")).toLocaleString('en-US'));
            $('#tbQuery thead td#td12').html(GetNodeValue(dtSumQ[0], "SumSalesPercent3"));
        }
    };

    //第一層
    let Step1_click = function (bt) {
        //$('#tbQuery td').closest('tr').css('background-color', 'transparent');
        $(bt).closest('tr').click();
        $('.msg-valid').hide();
        var node = $(grdM.ActiveRowTR()).prop('Record');
        //$('#tbQuery td:contains(' + GetNodeValue(node, 'ID') + ')').closest('tr').css('background-color', '#DEEBF7');
        var heads = $('#tbQuery thead tr th#tdflag').html();

        if (heads.toString().indexOf("店別") >= 0) {
            $('#lblYM_Shop1').html($('#txtYM').val().toString().replaceAll('-', '/'));
            $('#lblShop1').html(GetNodeValue(node, 'ID'));
            //$('#tbShop1 thead tr th').css('background-color', '#ffb620')
            $('#modal_Shop1').modal('show');
            setTimeout(function () {
                QueryShop1();
            }, 500);
        }
        else if (heads.toString().indexOf("日期") >= 0) {
            $('#lblYM_Date1').html($('#txtYM').val().toString().replaceAll('-', '/'));
            $('#lblDate1').html(GetNodeValue(node, 'ID'));
            //$('#tbDate1 thead tr th').css('background-color', '#ffb620')
            $('#modal_Date1').modal('show');
            setTimeout(function () {
                QueryDate1();
            }, 500);
        }
    };

    let QueryShop1 = function () {
        ShowLoading();
        var pData = {
            CountYM: $('#lblYM_Shop1').html(),
            ShopNo: $('#lblShop1').html().split('-')[0],
            OpenDate: "",
            Flag: "S"
        }
        PostToWebApi({ url: "api/SystemSetup/MSSD105Query_Step1", data: pData, success: afterMSSD105Query_Shop1 });
    };

    let afterMSSD105Query_Shop1 = function (data) {
        CloseLoading();
        if (ReturnMsg(data, 0) != "MSSD105Query_Step1OK") {
            DyAlert(ReturnMsg(data, 1));
        }
        else {
            var dtE = data.getElementsByTagName('dtE');
            grdShop1.BindData(dtE);

            if (dtE.length == 0) {
                DyAlert("無符合資料!");
                $(".modal-backdrop").remove();
                $('#tbShop1 thead td#td1_Shop1').html('');
                $('#tbShop1 thead td#td2_Shop1').html('');
                $('#tbShop1 thead td#td3_Shop1').html('');
                $('#tbShop1 thead td#td4_Shop1').html('');
                $('#tbShop1 thead td#td5_Shop1').html('');
                $('#tbShop1 thead td#td6_Shop1').html('');
                $('#tbShop1 thead td#td7_Shop1').html('');
                $('#tbShop1 thead td#td8_Shop1').html('');
                $('#tbShop1 thead td#td9_Shop1').html('');
                $('#tbShop1 thead td#td10_Shop1').html('');
                $('#tbShop1 thead td#td11_Shop1').html('');
                $('#tbShop1 thead td#td12_Shop1').html('');
                return;
            }
            var dtSumQ = data.getElementsByTagName('dtSumQ');
            $('#tbShop1 thead td#td1_Shop1').html(parseInt(GetNodeValue(dtSumQ[0], "SumVIPCnt")).toLocaleString('en-US'));
            $('#tbShop1 thead td#td2_Shop1').html(parseInt(GetNodeValue(dtSumQ[0], "SumSalesCash1")).toLocaleString('en-US'));
            $('#tbShop1 thead td#td3_Shop1').html(parseInt(GetNodeValue(dtSumQ[0], "SumSalesCnt1")).toLocaleString('en-US'));
            $('#tbShop1 thead td#td4_Shop1').html(parseInt(GetNodeValue(dtSumQ[0], "SumSalesPrice1")).toLocaleString('en-US'));
            $('#tbShop1 thead td#td5_Shop1').html(parseInt(GetNodeValue(dtSumQ[0], "SumSalesCash2")).toLocaleString('en-US'));
            $('#tbShop1 thead td#td6_Shop1').html(parseInt(GetNodeValue(dtSumQ[0], "SumSalesCnt2")).toLocaleString('en-US'));
            $('#tbShop1 thead td#td7_Shop1').html(parseInt(GetNodeValue(dtSumQ[0], "SumSalesPrice2")).toLocaleString('en-US'));
            $('#tbShop1 thead td#td8_Shop1').html(GetNodeValue(dtSumQ[0], "SumSalesPercent2"));
            $('#tbShop1 thead td#td9_Shop1').html(parseInt(GetNodeValue(dtSumQ[0], "SumSalesCash3")).toLocaleString('en-US'));
            $('#tbShop1 thead td#td10_Shop1').html(parseInt(GetNodeValue(dtSumQ[0], "SumSalesCnt3")).toLocaleString('en-US'));
            $('#tbShop1 thead td#td11_Shop1').html(parseInt(GetNodeValue(dtSumQ[0], "SumSalesPrice3")).toLocaleString('en-US'));
            $('#tbShop1 thead td#td12_Shop1').html(GetNodeValue(dtSumQ[0], "SumSalesPercent3"));
        }
    };

    let QueryDate1 = function () {
        ShowLoading();
        var pData = {
            CountYM: $('#lblYM_Date1').html(),
            ShopNo: "",
            OpenDate: $('#lblDate1').html(),
            Flag: "D"
        }
        PostToWebApi({ url: "api/SystemSetup/MSSD105Query_Step1", data: pData, success: afterMSSD105Query_Date1 });
    };

    let afterMSSD105Query_Date1 = function (data) {
        CloseLoading();
        if (ReturnMsg(data, 0) != "MSSD105Query_Step1OK") {
            DyAlert(ReturnMsg(data, 1));
        }
        else {
            var dtE = data.getElementsByTagName('dtE');
            grdDate1.BindData(dtE);

            if (dtE.length == 0) {
                DyAlert("無符合資料!");
                $(".modal-backdrop").remove();
                $('#tbDate1 thead td#td1_Date1').html('');
                $('#tbDate1 thead td#td2_Date1').html('');
                $('#tbDate1 thead td#td3_Date1').html('');
                $('#tbDate1 thead td#td4_Date1').html('');
                $('#tbDate1 thead td#td5_Date1').html('');
                $('#tbDate1 thead td#td6_Date1').html('');
                $('#tbDate1 thead td#td7_Date1').html('');
                $('#tbDate1 thead td#td8_Date1').html('');
                $('#tbDate1 thead td#td9_Date1').html('');
                $('#tbDate1 thead td#td10_Date1').html('');
                $('#tbDate1 thead td#td11_Date1').html('');
                $('#tbDate1 thead td#td12_Date1').html('');
                return;
            }
            var dtSumQ = data.getElementsByTagName('dtSumQ');
            $('#tbDate1 thead td#td1_Date1').html(parseInt(GetNodeValue(dtSumQ[0], "SumVIPCnt")).toLocaleString('en-US'));
            $('#tbDate1 thead td#td2_Date1').html(parseInt(GetNodeValue(dtSumQ[0], "SumSalesCash1")).toLocaleString('en-US'));
            $('#tbDate1 thead td#td3_Date1').html(parseInt(GetNodeValue(dtSumQ[0], "SumSalesCnt1")).toLocaleString('en-US'));
            $('#tbDate1 thead td#td4_Date1').html(parseInt(GetNodeValue(dtSumQ[0], "SumSalesPrice1")).toLocaleString('en-US'));
            $('#tbDate1 thead td#td5_Date1').html(parseInt(GetNodeValue(dtSumQ[0], "SumSalesCash2")).toLocaleString('en-US'));
            $('#tbDate1 thead td#td6_Date1').html(parseInt(GetNodeValue(dtSumQ[0], "SumSalesCnt2")).toLocaleString('en-US'));
            $('#tbDate1 thead td#td7_Date1').html(parseInt(GetNodeValue(dtSumQ[0], "SumSalesPrice2")).toLocaleString('en-US'));
            $('#tbDate1 thead td#td8_Date1').html(GetNodeValue(dtSumQ[0], "SumSalesPercent2"));
            $('#tbDate1 thead td#td9_Date1').html(parseInt(GetNodeValue(dtSumQ[0], "SumSalesCash3")).toLocaleString('en-US'));
            $('#tbDate1 thead td#td10_Date1').html(parseInt(GetNodeValue(dtSumQ[0], "SumSalesCnt3")).toLocaleString('en-US'));
            $('#tbDate1 thead td#td11_Date1').html(parseInt(GetNodeValue(dtSumQ[0], "SumSalesPrice3")).toLocaleString('en-US'));
            $('#tbDate1 thead td#td12_Date1').html(GetNodeValue(dtSumQ[0], "SumSalesPercent3"));
        }
    };

    let btRe_Shop1_click = function (bt) {
        $('#modal_Shop1').modal('hide');
        setTimeout(function () {
            ClearShop1();
        }, 500);
    };

    let ClearShop1 = function () {
        var pData = {
        }
        PostToWebApi({ url: "api/SystemSetup/MSSD105Clear_Step1", data: pData, success: afterMSSD105Clear_Shop1 });
    };

    let afterMSSD105Clear_Shop1 = function (data) {
        if (ReturnMsg(data, 0) != "MSSD105Clear_Step1OK") {
            DyAlert(ReturnMsg(data, 1));
        }
        else {
            var dtE = data.getElementsByTagName('dtE');
            grdShop1.BindData(dtE);

            $(".modal-backdrop").remove();
            $('#tbShop1 thead td#td1_Shop1').html('');
            $('#tbShop1 thead td#td2_Shop1').html('');
            $('#tbShop1 thead td#td3_Shop1').html('');
            $('#tbShop1 thead td#td4_Shop1').html('');
            $('#tbShop1 thead td#td5_Shop1').html('');
            $('#tbShop1 thead td#td6_Shop1').html('');
            $('#tbShop1 thead td#td7_Shop1').html('');
            $('#tbShop1 thead td#td8_Shop1').html('');
            $('#tbShop1 thead td#td9_Shop1').html('');
            $('#tbShop1 thead td#td10_Shop1').html('');
            $('#tbShop1 thead td#td11_Shop1').html('');
            $('#tbShop1 thead td#td12_Shop1').html('');
        }
    };

    let btRe_Date1_click = function (bt) {
        $('#modal_Date1').modal('hide');
        setTimeout(function () {
            ClearDate1();
        }, 500);
    };

    let ClearDate1 = function () {
        var pData = {
        }
        PostToWebApi({ url: "api/SystemSetup/MSSD105Clear_Step1", data: pData, success: afterMSSD105Clear_Date1 });
    };

    let afterMSSD105Clear_Date1 = function (data) {
        if (ReturnMsg(data, 0) != "MSSD105Clear_Step1OK") {
            DyAlert(ReturnMsg(data, 1));
        }
        else {
            var dtE = data.getElementsByTagName('dtE');
            grdDate1.BindData(dtE);

            $(".modal-backdrop").remove();
            $('#tbDate1 thead td#td1_Date1').html('');
            $('#tbDate1 thead td#td2_Date1').html('');
            $('#tbDate1 thead td#td3_Date1').html('');
            $('#tbDate1 thead td#td4_Date1').html('');
            $('#tbDate1 thead td#td5_Date1').html('');
            $('#tbDate1 thead td#td6_Date1').html('');
            $('#tbDate1 thead td#td7_Date1').html('');
            $('#tbDate1 thead td#td8_Date1').html('');
            $('#tbDate1 thead td#td9_Date1').html('');
            $('#tbDate1 thead td#td10_Date1').html('');
            $('#tbDate1 thead td#td11_Date1').html('');
            $('#tbDate1 thead td#td12_Date1').html('');
        }
    };

    let ClearQuery = function () {
        //$('#tbQuery thead tr th').css('background-color', '#ffb620')
        grdM.BindData(null)
        var heads = $('#tbQuery thead tr th#tdflag');
        if ($('#rdoShop').prop('checked')) {
            $(heads).html('店別');
        }
        else if ($('#rdoDate').prop('checked')) {
            $(heads).html('日期');
        }

        var sumtdQ = document.querySelector('.QSum');
        for (i = 0; i < sumtdQ.childElementCount; i++) {
            if (i == 0) {
                sumtdQ.children[i].innerHTML = "總數";
            }
            else {
                sumtdQ.children[i].innerHTML = "";
            }
        }
    }
//#region FormLoad
    let GetInitMSSD105 = function (data) {
        if (ReturnMsg(data, 0) != "GetInitMSSD105OK") {
            DyAlert(ReturnMsg(data, 1));
        }
       else {
            var dtE = data.getElementsByTagName('dtE');
            var dtV = data.getElementsByTagName('dtV');
            if (dtE.length > 0) {
                $('#lblProgramName').html(GetNodeValue(dtE[0], "ChineseName"));
            }
            if (dtV.length > 0) {
                SysYM = GetNodeValue(dtV[0], "EndDate").toString().substring(0, 7).replaceAll('/', '-');
                $('#txtYM').val(SysYM);
                $('#lblEnd').html("統計至 " + GetNodeValue(dtV[0], "EndDate") + " 止");
                $('#lblVIPQty').html('會員總數 : ' + parseInt(GetNodeValue(dtV[0], "VIPCntAll")).toLocaleString('en-US'));
            }
            else {
                $('#txtYM').val('');
                $('#lblEnd').html('');
                $('#lblVIPQty').html('');
            }
            AssignVar();

            $('#btQuery').click(function () { btQuery_click(this) });
            $('#btClear').click(function () { btClear_click(this) });
            $('#btRe_Shop1').click(function () { btRe_Shop1_click(this) });
            $('#btRe_Date1').click(function () { btRe_Date1_click(this) });
            $('#rdoShop,#rdoDate').change(function () { ClearQuery() });
        }
    };
    
    let afterLoadPage = function () {
        var pData = {
            ProgramID: "MSSD105"
        }
        PostToWebApi({ url: "api/SystemSetup/GetInitMSSD105", data: pData, success: GetInitMSSD105 });
    };
//#endregion
    

    if ($('#pgMSSD105').length == 0) {  
        AllPages = new LoadAllPages(ParentNode, "SystemSetup/MSSD105", ["MSSD105btns", "pgMSSD105Init", "pgMSSD105Add", "pgMSSD105Mod"], afterLoadPage);
    };


}