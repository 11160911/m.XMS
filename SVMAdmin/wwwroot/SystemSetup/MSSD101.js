var PageMSSD101 = function (ParentNode) {

    let grdM;
    let grdLookUp_ActivityCode;
    let grdM_PS_Step1_Shop;
    let grdM_PS_Step1_Date;
    let grdM_PS_Shop_Step2;
    let grdM_PS_Date_Step2;

    let AssignVar = function () {

        grdM = new DynGrid(
            {
                table_lement: $('#tbQuery')[0],
                class_collection: ["tdCol1 label-align:center", "tdCol2", "tdCol3", "tdCol4 label-align", "tdCol5 label-align", "tdCol6 label-align", "tdCol7 label-align", "tdCol8 label-align", "tdCol9 label-align", "tdCol10 label-align", "tdCol11H"],
                fields_info: [
                    { type: "Text", name: "ActivityCode", style: "text-align:center" },
                    { type: "Text", name: "PS_Name" },
                    { type: "Text", name: "PSDate", style: "text-align:center" },
                    { type: "TextAmt", name: "SendCnt" },
                    { type: "TextAmt", name: "BackCnt" },
                    { type: "TextPercent", name: "BackPer" },
                    { type: "TextAmt", name: "Discount" },
                    { type: "TextAmt", name: "Cash" },
                    { type: "TextAmt", name: "SalesCnt" },
                    { type: "TextAmt", name: "Balance" },
                    { type: "Text", name: "PS_No", style: "display:none" }
                ],
                //rows_per_page: 10,
                method_clickrow: click_Row,
                afterBind: Init_Shop_PS_Step1,
                sortable: "Y",
                step: "Y"
            }
        );

        grdLookUp_ActivityCode = new DynGrid(
            {
                table_lement: $('#tbDataLookup_ActivityCode')[0],
                class_collection: ["tdCol1 text-center", "tdCol2", "tdCol3", "tdCol4", "tdCol5"],
                fields_info: [
                    { type: "radio", name: "chkset", style: "width:16px;height:16px" },
                    { type: "Text", name: "ActivityCode", style: "" },
                    { type: "Text", name: "PS_Name", style: "" },
                    { type: "Text", name: "StartDate", style: "" },
                    { type: "Text", name: "EndDate", style: "" }
                ],
                //rows_per_page: 10,
                method_clickrow: click_Row,
                sortable: "N",
                step: "Y"
            }
        );

        grdLookUp_DocNO = new DynGrid(
            {
                table_lement: $('#tbDataLookup_DocNO')[0],
                class_collection: ["tdCol1", "tdCol2", "tdCol3", "tdCol4", "tdCol5"],
                fields_info: [
                    { type: "radio", name: "chkset", style: "width:16px;height:16px" },
                    { type: "Text", name: "DocNO", style: "" },
                    { type: "Text", name: "EDMMemo", style: "" },
                    { type: "Text", name: "StartDate", style: "" },
                    { type: "Text", name: "EndDate", style: "" }
                ],
                //rows_per_page: 10,
                method_clickrow: click_Row,
                sortable: "N",
                step: "Y"
            }
        );

        grdM_PS_Step1_Shop = new DynGrid(
            {
                table_lement: $('#tbShop_PS_Step1')[0],
                class_collection: ["tdCol1", "tdCol2 label-align", "tdCol3 label-align", "tdCol4", "tdCol5 label-align", "tdCol6 label-align", "tdCol7 label-align", "tdCol8 label-align", "tdCol9 label-align", "tdCol10 label-align", "tdCol11 label-align"],
                fields_info: [
                    { type: "Text", name: "ShopNO", style: "text-align:center" },
                    { type: "TextAmt", name: "SendCnt" },
                    { type: "TextAmt", name: "BackCnt" },
                    { type: "TextPercent", name: "BackPer" },
                    { type: "TextAmt", name: "Discount" },
                    { type: "TextAmt", name: "Cash" },
                    { type: "TextAmt", name: "VIPCNT" },
                    { type: "TextAmt", name: "VIPPer" },
                    { type: "TextAmt", name: "SalesCash" },
                    { type: "TextAmt", name: "SalesCNT" },
                    { type: "TextAmt", name: "SalesPer" }
                ],
                //rows_per_page: 10,
                method_clickrow: click_Row,
                afterBind: Init_Shop_Step1,
                sortable: "Y",
                step: "Y"
            }
        );
        grdM_PS_Step1_Date = new DynGrid(
            {
                table_lement: $('#tbShop_PS_Step1_2')[0],
                class_collection: ["tdCol1", "tdCol2 label-align", "tdCol3 label-align", "tdCol4 label-align", "tdCol5 label-align", "tdCol6 label-align", "tdCol7 label-align", "tdCol8 label-align", "tdCol9 label-align"],
                fields_info: [
                    { type: "Text", name: "Salesdate", style: "text-align:center" },
                    { type: "TextAmt", name: "BackCnt" },
                    { type: "TextAmt", name: "Discount" },
                    { type: "TextAmt", name: "Cash" },
                    { type: "TextAmt", name: "VIPCNT" },
                    { type: "TextAmt", name: "VIPPer" },
                    { type: "TextAmt", name: "SalesCash" },
                    { type: "TextAmt", name: "SalesCNT" },
                    { type: "TextAmt", name: "SalesPer" }
                ],
                //rows_per_page: 10,
                method_clickrow: click_Row,
                afterBind: Init_Date_Step1,
                sortable: "Y",
                step: "Y"
            }
        );

        grdM_PS_Shop_Step2 = new DynGrid(
            {
                table_lement: $('#tbSales_Shop_Step2')[0],
                class_collection: ["tdCol1", "tdCol2 label-align", "tdCol3 label-align", "tdCol4 label-align", "tdCol5 label-align", "tdCol6 label-align", "tdCol7 label-align", "tdCol8 label-align", "tdCol9 label-align"],
                fields_info: [
                    { type: "Text", name: "SalesDate", style: "text-align:center" },
                    { type: "TextAmt", name: "ReclaimQty" },
                    { type: "TextAmt", name: "ShareAmt" },
                    { type: "TextAmt", name: "ReclaimCash" },
                    { type: "TextAmt", name: "ReclaimTrans" },
                    { type: "TextAmt", name: "ReclaimPrice" },
                    { type: "TextAmt", name: "TotalCash" },
                    { type: "TextAmt", name: "TotalTrans" },
                    { type: "TextAmt", name: "TotalPrice" },

                ],
                //rows_per_page: 10,
                method_clickrow: click_Row,
                sortable: "Y",
                step: "Y"
            }
        );

        grdM_PS_Date_Step2 = new DynGrid(
            {
                table_lement: $('#tbSales_Date_Step2')[0],
                class_collection: ["tdCol1", "tdCol2 label-align", "tdCol3 label-align", "tdCol4 label-align", "tdCol5 label-align", "tdCol6 label-align", "tdCol7 label-align", "tdCol8 label-align", "tdCol9"],
                fields_info: [
                    { type: "Text", name: "Shop", style: "text-align:center" },
                    { type: "TextAmt", name: "ReclaimQty" },
                    { type: "TextAmt", name: "ShareAmt" },
                    { type: "TextAmt", name: "ReclaimCash" },
                    { type: "TextAmt", name: "ReclaimTrans" },
                    { type: "TextAmt", name: "ReclaimPrice" },
                    { type: "TextAmt", name: "TotalCash" },
                    { type: "TextAmt", name: "TotalTrans" },
                    { type: "TextAmt", name: "TotalPrice" },
                ],
                //rows_per_page: 10,
                method_clickrow: click_Row,
                sortable: "Y",
                step: "Y"
            }
        );

        return;
    };

    //點ROW時的事件
    let click_Row = function (tr) {

    };
    let Init_Shop_PS_Step1 = function () {
        $('#tbQuery tbody tr td').click(function () { Step1_click(this) });
    }
    let Init_Shop_Step1 = function () {
        $('#tbShop_PS_Step1 tbody tr td').click(function () { Shop_Step1_click(this) });
    }
    let Init_Date_Step1 = function () {
        $('#tbShop_PS_Step1_2 tbody tr td').click(function () { Date_Step1_click(this) });
    }

    let Step1_click = function (bt) {
        //$('#tbQuery td').closest('tr').css('background-color', 'transparent');
        $(bt).closest('tr').click();
        $('.msg-valid').hide();
        var node = $(grdM.ActiveRowTR()).prop('Record');
        //$('#tbQuery td:contains(' + GetNodeValue(node, 'PS_NO') + ')').closest('tr').css('background-color', '#DEEBF7');
        //var rdoAB = $('input[name="TypeCode"]:checked').val();
        //if (rdoAB == "DA") {

        //$('#pgMSSD101Init').hide();
        //if ($('#pgMSSD101_PS_STEP1').attr('hidden') == undefined) {
        //    $('#pgMSSD101_PS_STEP1').show();
        //}
        //else {
        //    $('#pgMSSD101_PS_STEP1').removeAttr('hidden');
        //}
        //$('#tbShop_PS_Step1 thead tr th').css('background-color', '#ffb620')
        //$('#tbShop_PS_Step1_2 thead tr th').css('background-color', '#ffb620')
        $('#modal_PS_Step1').modal('show');
        $('#lblActivityCode_Step1').html(GetNodeValue(node, 'ActivityCode'));
        $('#lblPSDate_Step1').html(GetNodeValue(node, 'PSDate'));
        $('#lblPSName_Step1').html(GetNodeValue(node, 'PS_Name'));
        $('#lblPSNO_Step1').html(GetNodeValue(node, 'PS_No'));
        $('#rdoShop_PS_Step1').prop('checked', true);
        setTimeout(function () {
            Query_PS_Step1_click();
        }, 500);
        //}
        //else if (rdoAB == "DB") {
        //    $('#lblOpenDate_Shop_Step1').html($('#OpenDateS').val() + "~" + $('#OpenDateE').val());
        //    $('#lblShop_Step1').html(GetNodeValue(node, 'ID1') + " " + GetNodeValue(node, 'Name1'));
        //    $('#modal_Shop_Step1').modal('show');
        //    setTimeout(function () {
        //        Query_DM_Step1_click();
        //    }, 500);
        //}

    };

    let Shop_Step1_click = function (bt) {
        $(bt).closest('tr').click();
        $('.msg-valid').hide();

        var node = $(grdM_PS_Step1_Shop.ActiveRowTR()).prop('Record');

        if ($('#rdoShop_PS_Step1').prop('checked') == true) {
            $('#lblShop_ActivityCode_Step2').html($('#lblActivityCode_Step1').html());
            $('#lblShop_PSName_Step2').html($('#lblPSName_Step1').html());
            $('#lblShop_Date_Step2').html($('#lblPSDate_Step1').html());
            $('#lblShop_Step2').html(GetNodeValue(node, 'ShopNO'));
            //$('#tbSales_Shop_Step2 thead tr th').css('background-color', '#ffb620')
            $('#modal_Shop_Step2').modal('show');

            setTimeout(function () {
                Query_Shop_Step2_click($('#lblPSNO_Step1').html(), GetNodeValue(node, 'ShopNO').split('-')[0]);
            }, 500);
        }
    };

    let Date_Step1_click = function (bt) {
        $(bt).closest('tr').click();
        $('.msg-valid').hide();

        var node = $(grdM_PS_Step1_Date.ActiveRowTR()).prop('Record');

        if ($('#rdoDate_PS_Step1').prop('checked')) {
            $('#lblDate_ActivityCode_Step2').html($('#lblActivityCode_Step1').html());
            $('#lblDate_PSName_Step2').html($('#lblPSName_Step1').html());
            $('#lblDate_Date_Step2').html($('#lblPSDate_Step1').html());
            $('#lblOpenDate_Date_Step2').html(GetNodeValue(node, 'Salesdate'));
            //$('#tbSales_Date_Step2 thead tr th').css('background-color', '#ffb620')
            $('#modal_Date_Step2').modal('show');
            setTimeout(function () {
                Query_Date_Step2_click($('#lblPSNO_Step1').html(), GetNodeValue(node, 'Salesdate'));
            }, 500);
        }

    };

    //各畫面的回上頁
    let btExit_PS_Step1_click = function (bt) {
        $('#modal_PS_Step1').modal('hide');
        //$('#pgMSSD101Init').show();
        //$('#pgMSSD101_PS_STEP1').hide();
    };

    let btExit_Shop_Step2_click = function (bt) {
        $('#modal_Shop_Step2').modal('hide');
    };

    let btExit_Date_Step2_click = function (bt) {
        $('#modal_Date_Step2').modal('hide');
    };

    //未使用
    let ChkLogOut_1 = function (AfterChkLogOut_1) {
        var LoginDT = sessionStorage.getItem('LoginDT');
        var cData = {
            LoginDT: LoginDT
        }
        PostToWebApi({ url: "api/js/ChkLogOut", data: cData, success: AfterChkLogOut_1 });
    };

    let Query_PS_Step1_click = function () {
        //$('#tbShop_PS_Step1 thead tr th').css('background-color', '#ffb620')
        //$('#tbShop_PS_Step1_2 thead tr th').css('background-color', '#ffb620')
        ShowLoading();
        var Type_Step1 = "";
        if ($('#rdoShop_PS_Step1').prop('checked')) {
            Type_Step1 = "S";
            $('#tbShop_PS_Step1').show();
            $('#tbShop_PS_Step1_2').hide();
        }
        else if ($('#rdoDate_PS_Step1').prop('checked')) {
            Type_Step1 = "D";
            if ($('#tbShop_PS_Step1_2').attr('hidden') == undefined) {
                $('#tbShop_PS_Step1_2').show();
            }
            else {
                $('#tbShop_PS_Step1_2').removeAttr('hidden');
                $('#tbShop_PS_Step1_2').show();
            }
            $('#tbShop_PS_Step1').hide();
        }
        var PSDateS = $('#lblPSDate_Step1').html().split('~')[0]
        var PSDateE = $('#lblPSDate_Step1').html().split('~')[1]

        var pData = {
            PS_No: $('#lblPSNO_Step1').html(),
            ActivityCode: $('#lblActivityCode_Step1').html(),
            PSDateS: PSDateS,
            PSDateE: PSDateE,
            Type_Step1: Type_Step1
        }
        PostToWebApi({ url: "api/SystemSetup/MSSD101_Query_PS_Step1", data: pData, success: afterQuery_PS_Step1 });
    };

    let afterQuery_PS_Step1 = function (data) {
        CloseLoading();
        if (ReturnMsg(data, 0) != "Query_PS_Step1OK") {
            DyAlert(ReturnMsg(data, 1));
        }
        else {

            var dtGridData = data.getElementsByTagName('dtGridData');
            if ($('#rdoShop_PS_Step1').prop('checked')) {
                grdM_PS_Step1_Shop.BindData(dtGridData);
            }
            else {
                grdM_PS_Step1_Date.BindData(dtGridData);
            }

            if (dtGridData.length == 0) {
                DyAlert("無符合資料!");
                //$(".modal-backdrop").remove();
                return;
            }

            if ($('#rdoShop_PS_Step1').prop('checked')) {
                var dtHeadSum = data.getElementsByTagName('dtHeadSum');
                $('#tbShop_PS_Step1 thead td#td1S').html(parseInt(GetNodeValue(dtHeadSum[0], "SendCnt")).toLocaleString('en-US'));
                $('#tbShop_PS_Step1 thead td#td2S').html(parseInt(GetNodeValue(dtHeadSum[0], "BackCnt")).toLocaleString('en-US'));
                $('#tbShop_PS_Step1 thead td#td3S').html(GetNodeValue(dtHeadSum[0], "BackPer").toLocaleString('en-US'));
                $('#tbShop_PS_Step1 thead td#td4S').html(parseInt(GetNodeValue(dtHeadSum[0], "Discount")).toLocaleString('en-US'));
                $('#tbShop_PS_Step1 thead td#td5S').html(parseInt(GetNodeValue(dtHeadSum[0], "Cash")).toLocaleString('en-US'));
                $('#tbShop_PS_Step1 thead td#td6S').html(parseInt(GetNodeValue(dtHeadSum[0], "Cnt")).toLocaleString('en-US'));
                $('#tbShop_PS_Step1 thead td#td7S').html(parseInt(GetNodeValue(dtHeadSum[0], "VIPPer")).toLocaleString('en-US'));

                $('#tbShop_PS_Step1 thead td#td8S').html(parseInt(GetNodeValue(dtHeadSum[0], "SalesCash")).toLocaleString('en-US'));
                $('#tbShop_PS_Step1 thead td#td9S').html(parseInt(GetNodeValue(dtHeadSum[0], "SalesCNT")).toLocaleString('en-US'));
                $('#tbShop_PS_Step1 thead td#td10S').html(parseInt(GetNodeValue(dtHeadSum[0], "SalesPer")).toLocaleString('en-US'));

            }
            else {

                var dtHeadSum = data.getElementsByTagName('dtHeadSum');
                $('#tbShop_PS_Step1_2 thead td#td1D').html(parseInt(GetNodeValue(dtHeadSum[0], "BackCnt")).toLocaleString('en-US'));
                $('#tbShop_PS_Step1_2 thead td#td2D').html(parseInt(GetNodeValue(dtHeadSum[0], "Discount")).toLocaleString('en-US'));
                $('#tbShop_PS_Step1_2 thead td#td3D').html(parseInt(GetNodeValue(dtHeadSum[0], "Cash")).toLocaleString('en-US'));
                $('#tbShop_PS_Step1_2 thead td#td4D').html(parseInt(GetNodeValue(dtHeadSum[0], "Cnt")).toLocaleString('en-US'));
                $('#tbShop_PS_Step1_2 thead td#td5D').html(parseInt(GetNodeValue(dtHeadSum[0], "VIPPer")).toLocaleString('en-US'));

                $('#tbShop_PS_Step1_2 thead td#td6D').html(parseInt(GetNodeValue(dtHeadSum[0], "SalesCash")).toLocaleString('en-US'));
                $('#tbShop_PS_Step1_2 thead td#td7D').html(parseInt(GetNodeValue(dtHeadSum[0], "SalesCNT")).toLocaleString('en-US'));
                $('#tbShop_PS_Step1_2 thead td#td8D').html(parseInt(GetNodeValue(dtHeadSum[0], "SalesPer")).toLocaleString('en-US'));

            }
            // //動態變動欄位
            // var heads = $('#tbShop_PS_Step1 thead tr td#Shop');
            // var coupon = $('#tbShop_PS_Step1 thead tr td#Coupon');
            // var th1 = $('#tbShop_PS_Step1 thead th#td1H');
            // var td1 = $('#tbShop_PS_Step1 thead td#td1');
            // //var tdS = $('#tbShop_PS_Step1 tbody td#Col2');
            // //alert(tds.html());
            //if ($('#rdoShop_PS_Step1').prop('checked')) {
            //     $(heads).html('店別');
            //     $(th1).show();
            //     $(td1).show();
            //    $(coupon).attr('colspan', 7);
            //    //$('tr td:nth-child(1)').show();
            //}
            // else if ($('#rdoDate_PS_Step1').prop('checked')) {
            //     $(heads).html('日期');
            //     $(th1).hide();
            //     $(td1).hide();
            //     $(coupon).attr('colspan',6);
            //   // $('tr td:nth-child(1)').hide();
            // }
        }
    };

    let Query_Shop_Step2_click = function (PS_NO, ID) {
        ShowLoading();
        var Flag = "S";

        var SDate = $('#lblPSDate_Step1').html().split('~')[0]
        var EDate = $('#lblPSDate_Step1').html().split('~')[1]

        var pData = {
            PS_NO: PS_NO,
            ID: ID,
            OpenDate1: SDate,
            OpenDate2: EDate,
            Flag: Flag
        }
        PostToWebApi({ url: "api/SystemSetup/MSSD101_Query_Step2", data: pData, success: afterQuery_Shop_Step2 });
    };

    let afterQuery_Shop_Step2 = function (data) {
        CloseLoading();
        if (ReturnMsg(data, 0) != "Query_Step2OK") {
            DyAlert(ReturnMsg(data, 1));
        }
        else {

            var dtE = data.getElementsByTagName('dtE');
            grdM_PS_Shop_Step2.BindData(dtE);

            if (dtE.length == 0) {
                DyAlert("無符合資料!");
                //$(".modal-backdrop").remove();
                return;
            }

            var dtSumQ = data.getElementsByTagName('dtSumQ');
            $('#tbSales_Shop_Step2 thead td#tdShopNo1').html(parseInt(GetNodeValue(dtSumQ[0], "SumReclaimQty")).toLocaleString('en-US'));
            $('#tbSales_Shop_Step2 thead td#tdShopNo2').html(parseInt(GetNodeValue(dtSumQ[0], "SumShareAmt")).toLocaleString('en-US'));
            $('#tbSales_Shop_Step2 thead td#tdShopNo3').html(parseInt(GetNodeValue(dtSumQ[0], "SumReclaimCash")).toLocaleString('en-US'));
            $('#tbSales_Shop_Step2 thead td#tdShopNo4').html(parseInt(GetNodeValue(dtSumQ[0], "SumReclaimTrans")).toLocaleString('en-US'));
            $('#tbSales_Shop_Step2 thead td#tdShopNo5').html(parseInt(GetNodeValue(dtSumQ[0], "SumReclaimPrice")).toLocaleString('en-US'));
            $('#tbSales_Shop_Step2 thead td#tdShopNo6').html(parseInt(GetNodeValue(dtSumQ[0], "SumTotalCash")).toLocaleString('en-US'));
            $('#tbSales_Shop_Step2 thead td#tdShopNo7').html(parseInt(GetNodeValue(dtSumQ[0], "SumTotalTrans")).toLocaleString('en-US'));
            $('#tbSales_Shop_Step2 thead td#tdShopNo8').html(parseInt(GetNodeValue(dtSumQ[0], "SumTotalPrice")).toLocaleString('en-US'));
        }
    };

    let Query_Date_Step2_click = function (PS_NO, ID) {
        ShowLoading();
        var Flag = "D";

        var SDate = $('#lblPSDate_Step1').html().split('~')[0]
        var EDate = $('#lblPSDate_Step1').html().split('~')[1]

        var pData = {
            PS_NO: PS_NO,
            ID: ID,
            OpenDate1: SDate,
            OpenDate2: EDate,
            Flag: Flag
        }
        PostToWebApi({ url: "api/SystemSetup/MSSD101_Query_Step2", data: pData, success: afterQuery_Date_Step2 });
    };

    let afterQuery_Date_Step2 = function (data) {
        CloseLoading();
        if (ReturnMsg(data, 0) != "Query_Step2OK") {
            DyAlert(ReturnMsg(data, 1));
        }
        else {

            var dtE = data.getElementsByTagName('dtE');
            grdM_PS_Date_Step2.BindData(dtE);

            if (dtE.length == 0) {
                DyAlert("無符合資料!");
                //$(".modal-backdrop").remove();
                return;
            }

            var dtSumQ = data.getElementsByTagName('dtSumQ');
            $('#tbSales_Date_Step2 thead td#tdDate1').html(parseInt(GetNodeValue(dtSumQ[0], "SumReclaimQty")).toLocaleString('en-US'));
            $('#tbSales_Date_Step2 thead td#tdDate2').html(parseInt(GetNodeValue(dtSumQ[0], "SumShareAmt")).toLocaleString('en-US'));
            $('#tbSales_Date_Step2 thead td#tdDate3').html(parseInt(GetNodeValue(dtSumQ[0], "SumReclaimCash")).toLocaleString('en-US'));
            $('#tbSales_Date_Step2 thead td#tdDate4').html(parseInt(GetNodeValue(dtSumQ[0], "SumReclaimTrans")).toLocaleString('en-US'));
            $('#tbSales_Date_Step2 thead td#tdDate5').html(parseInt(GetNodeValue(dtSumQ[0], "SumReclaimPrice")).toLocaleString('en-US'));
            $('#tbSales_Date_Step2 thead td#tdDate6').html(parseInt(GetNodeValue(dtSumQ[0], "SumTotalCash")).toLocaleString('en-US'));
            $('#tbSales_Date_Step2 thead td#tdDate7').html(parseInt(GetNodeValue(dtSumQ[0], "SumTotalTrans")).toLocaleString('en-US'));
            $('#tbSales_Date_Step2 thead td#tdDate8').html(parseInt(GetNodeValue(dtSumQ[0], "SumTotalPrice")).toLocaleString('en-US'));
        }
    };

    //清除
    let btClear_click = function (bt) {
        //Timerset();
        $('#txtActivityCode').val('');
        $('#txtPSName').val('');
        $('#txtPSDate').val('');
        $('#txtDocNO').val('');
        $('#txtEDMMemo').val('');
        $('#txtEDDate').val('');
    };

    //#region 查詢
    let btQuery_click = function (bt) {
        //Timerset();
        //$('#tbQuery thead tr th').css('background-color', '#ffb620')
        $('#btQuery').prop('disabled', true)
        ShowLoading();
        var pData = {
            ActivityCode: $('#txtActivityCode').val(),
            PSName: $('#txtPSName').val(),
            PSDate: $('#txtPSDate').val().toString().replaceAll('-', '/')
        }
        //    DocNO: $('#txtDocNO').val(),
        //    EDMMemo: $('#txtEDMMemo').val(),
        //    EDDate: $('#txtEDDate').val().toString().replaceAll('-', '/'),
        //    OptAB: $('input[name="TypeCode"]:checked').val()   //群組rdo
        //var rdoAB = $('input[name="TypeCode"]:checked').val();
        //if (rdoAB == "DB") {
        //$('table tr th:contains("活動代號 ")').text('DM單號 ');   //空一格是避免抓到相同名稱
        //$('table tr th:contains("活動名稱 ")').text('DM名稱 ');
        //$('table tr th:contains("活動期間 ")').text('DM期間 ');
        //}
        //else if (rdoAB == "DA") {
        //    $('table tr th:contains("DM單號 ")').text('活動代號 ');
        //    $('table tr th:contains("DM名稱 ")').text('活動名稱 ');
        //    $('table tr th:contains("DM期間 ")').text('活動期間 ');
        //}

        PostToWebApi({ url: "api/SystemSetup/MSSD101Query", data: pData, success: afterMSSD101Query });
    };

    let afterMSSD101Query = function (data) {
        CloseLoading();
        if (ReturnMsg(data, 0) != "MSSD101QueryOK") {
            DyAlert(ReturnMsg(data, 1), function () { $('#btQuery').prop('disabled', false); });
        }
        else {
            $('#btQuery').prop('disabled', false); 
            var dtE = data.getElementsByTagName('dtE');
            grdM.BindData(dtE);
            if (dtE.length == 0) {
                DyAlert("無符合資料!");
                //$(".modal-backdrop").remove();
                return;
            }
        }
    };
    //#endregion

    //活動代號btn
    //#region 活動代號
    let btActivityCode_click = function (bt) {
        //Timerset();
        var pData = {
            ActivityCode: $('#txtActivityCode').val()
        }
        PostToWebApi({ url: "api/SystemSetup/MSSD101_LookUpActivityCode", data: pData, success: afterMSSD101_LookUpActivityCode });
    };

    let afterMSSD101_LookUpActivityCode = function (data) {
        if (ReturnMsg(data, 0) != "MSSD101_LookUpActivityCodeOK") {
            DyAlert(ReturnMsg(data, 1));
        }
        else {
            var dtE = data.getElementsByTagName('dtE');
            $('#txtQLookup_ActivityCode').val($('#txtActivityCode').val());
            $('#modal_Lookup_ActivityCode').modal('show');
            setTimeout(function () {
                grdLookUp_ActivityCode.BindData(dtE);
            }, 500);
        }
    };

    let btQLookup_ActivityCode_click = function (bt) {
        //Timerset();
        $('#btQLookup_ActivityCode').prop('disabled', true)
        var pData = {
            ActivityCode: $('#txtQLookup_ActivityCode').val()
        }
        PostToWebApi({ url: "api/SystemSetup/MSSD101_LookUpActivityCode", data: pData, success: afterMSSD101_QLookUpActivityCode });
    };

    let afterMSSD101_QLookUpActivityCode = function (data) {
        if (ReturnMsg(data, 0) != "MSSD101_LookUpActivityCodeOK") {
            DyAlert(ReturnMsg(data, 1), function () { $('#btQLookup_ActivityCode').prop('disabled', false) });
        }
        else {
            var dtE = data.getElementsByTagName('dtE');
            if (dtE.length == 0) {
                DyAlert("無符合資料!", function () { $('#btQLookup_ActivityCode').prop('disabled', false) });
                //$(".modal-backdrop").remove();
                return;
            }
            grdLookUp_ActivityCode.BindData(dtE);
            $('#btQLookup_ActivityCode').prop('disabled', false);
        }
    };

    let btLpOK_ActivityCode_click = function (bt) {
        //Timerset();
        var obchkedtd = $('#tbDataLookup_ActivityCode input[type="radio"]:checked');
        var chkedRow = obchkedtd.length.toString();

        if (chkedRow == 0) {
            DyAlert("未選取活動代號，請重新確認!");
        }
        else {
            var a = $(obchkedtd[0]).closest('tr');
            var trNode = $(a).prop('Record');
            $('#txtActivityCode').val(GetNodeValue(trNode, "ActivityCode"))
            ClearQuery();
            $('#modal_Lookup_ActivityCode').modal('hide')
        }
    };

    let btLpClear_ActivityCode_click = function (bt) {
        $("#txtQLookup_ActivityCode").val('');
        $("#tbDataLookup_ActivityCode .checkbox").prop('checked', false);
    };

    let btLpExit_ActivityCode_click = function (bt) {
        //Timerset();
        $('#modal_Lookup_ActivityCode').modal('hide')
    };
    //#endregion

    let ClearQuery = function () {
        grdM.BindData(null)
    }
    //FormLoad
    //#region FormLoad
    let GetInitMSSD101 = function (data) {
        if (ReturnMsg(data, 0) != "GetInitmsDMOK") {
            DyAlert(ReturnMsg(data, 1));
        }
        else {
            var dtE = data.getElementsByTagName('dtE');
            $('#lblProgramName').html(GetNodeValue(dtE[0], "ChineseName"));
            AssignVar();
            //查詢
            $('#btQuery').click(function () { btQuery_click(this) });
            $('#btClear').click(function () { btClear_click(this) });
            $('#txtActivityCode,#txtPSName').keydown(function () { ClearQuery() })
            $('#txtPSDate').change(function () { ClearQuery() })

            //開啟活動代碼介面
            $('#btActivityCode').click(function () { btActivityCode_click(this) });
            //活動代碼介面查詢鍵
            $('#btQLookup_ActivityCode').click(function () { btQLookup_ActivityCode_click(this) });
            //活動代碼介面確認鍵
            $('#btLpOK_ActivityCode').click(function () { btLpOK_ActivityCode_click(this) });
            //活動代碼介面離開鍵
            $('#btLpExit_ActivityCode').click(function () { btLpExit_ActivityCode_click(this) });
            $('#btLpClear_ActivityCode').click(function () { btLpClear_ActivityCode_click(this) });

            ////開啟DM代碼介面
            //$('#btDocNO').click(function () { btDocNO_click(this) });
            ////DM代碼介面查詢鍵
            //$('#btQLookup_DocNO').click(function () { btQLookup_DocNO_click(this) });
            ////DM代碼介面確認鍵
            //$('#btLpOK_DocNO').click(function () { btLpOK_DocNO_click(this) });
            ////DM代碼介面離開鍵
            //$('#btLpExit_DocNO').click(function () { btLpExit_DocNO_click(this) });

            //活動代碼by店/日期
            $('#btExit_PS_Step1').click(function () { btExit_PS_Step1_click(this) });
            $('#rdoShop_PS_Step1').change(function () { Query_PS_Step1_click(this) });
            $('#rdoDate_PS_Step1').change(function () { Query_PS_Step1_click(this) });

            $('#btExit_Shop_Step2').click(function () { btExit_Shop_Step2_click(this) });
            $('#btExit_Date_Step2').click(function () { btExit_Date_Step2_click(this) });

            btQuery_click();
            return;
        }
    };

    let afterLoadPage = function () {
        var pData = {
            ProgramID: "MSSD101"
        }
        PostToWebApi({ url: "api/SystemSetup/GetInitmsDM", data: pData, success: GetInitMSSD101 });
    };
    // #endregion


    if ($('#pgMSSD101').length == 0) {
        AllPages = new LoadAllPages(ParentNode, "SystemSetup/MSSD101", ["pgMSSD101Init", "pgMSSD101_PS_STEP1", "pgMSSD101Mod"], afterLoadPage);
    };


}