var PageMSSD102 = function (ParentNode) {

    let grdM;
    let grdShopNo_PSNO;
    let grdDate_PSNO;
    let grdShopNo_PSNO_ShopNoDate;
    let grdLookUp_ActivityCode;

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
                    { type: "TextPercent", name: "RePercent"},
                    { type: "TextAmt", name: "ActualDiscount"},
                    { type: "TextAmt", name: "Cash"},
                    { type: "TextAmt", name: "Cnt3" },
                    { type: "TextAmt", name: "SalesPrice" }
                ],
                //rows_per_page: 10,
                method_clickrow: click_PLU,
                afterBind: InitModifyDeleteButton,
                sortable: "Y",
                step:"Y"
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
                    { type: "TextPercent", name: "RePercent" },
                    { type: "TextAmt", name: "ActualDiscount" },
                    { type: "TextAmt", name: "SalesCash1" },
                    { type: "TextAmt", name: "SalesCnt1" },
                    { type: "TextAmt", name: "SalesPrice1" },
                    { type: "TextAmt", name: "SalesCash2" },
                    { type: "TextAmt", name: "SalesCnt2" },
                    { type: "TextAmt", name: "SalesPrice2" }
                ],
                //rows_per_page: 10,
                method_clickrow: click_PLU,
                afterBind: gridclick1,
                sortable: "Y",
                step: "Y"
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
                afterBind: gridclick2,
                sortable: "Y",
                step: "Y"
            }
        );

        grdShopNo_PSNO_ShopNoDate = new DynGrid(
            {
                table_lement: $('#tbShopNo_PSNO_ShopNoDate')[0],
                class_collection: ["tdCol1 text-center", "tdCol2 label-align", "tdCol3 label-align", "tdCol4 label-align", "tdCol5 label-align", "tdCol6 label-align", "tdCol7 label-align", "tdCol8 label-align", "tdCol9 label-align"],
                fields_info: [
                    { type: "Text", name: "ID", style: "" },
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
                method_clickrow: click_PLU,
                sortable: "Y"
            }
        );

        grdLookUp_ActivityCode = new DynGrid(
            {
                table_lement: $('#tbDataLookup_ActivityCode')[0],
                class_collection: ["tdCol1 text-center", "tdCol2", "tdCol3", "tdCol4 text-center", "tdCol5 text-center"],
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

        return;
    };

    let click_PLU = function (tr) {

    };

    let InitModifyDeleteButton = function () {
        $('#tbQuery tbody tr td').click(function () { Step1_click(this) });
    }

    let gridclick1 = function () {
        $('#tbShopNo_PSNO tbody tr td').click(function () { Step2_click(this) });
    }

    let gridclick2 = function () {
        $('#tbDate_PSNO tbody tr td').click(function () { Step2_click(this) });
    }

    let Step1_click = function (bt) {
        //$('#tbQuery td').closest('tr').css('background-color', 'transparent');
        $(bt).closest('tr').click();
        $('.msg-valid').hide();
        var node = $(grdM.ActiveRowTR()).prop('Record');
        //$('#tbQuery td:contains(' + GetNodeValue(node, 'PS_NO') + ')').closest('tr').css('background-color', '#DEEBF7');
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
        //$('#tbShopNo_PSNO thead tr th').css('background-color', '#ffb620')
        //$('#tbDate_PSNO thead tr th').css('background-color', '#ffb620')
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
                    //$(".modal-backdrop").remove();
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
                $('#tbShopNo_PSNO thead td#tdShopNo5_PSNO').html(parseInt(GetNodeValue(dtSumQ[0], "SumSalesCash1")).toLocaleString('en-US'));
                $('#tbShopNo_PSNO thead td#tdShopNo6_PSNO').html(parseInt(GetNodeValue(dtSumQ[0], "SumSalesCnt1")).toLocaleString('en-US'));
                $('#tbShopNo_PSNO thead td#tdShopNo7_PSNO').html(parseInt(GetNodeValue(dtSumQ[0], "SumSalesPrice1")).toLocaleString('en-US'));
                $('#tbShopNo_PSNO thead td#tdShopNo8_PSNO').html(parseInt(GetNodeValue(dtSumQ[0], "SumSalesCash2")).toLocaleString('en-US'));
                $('#tbShopNo_PSNO thead td#tdShopNo9_PSNO').html(parseInt(GetNodeValue(dtSumQ[0], "SumSalesCnt2")).toLocaleString('en-US'));
                $('#tbShopNo_PSNO thead td#tdShopNo10_PSNO').html(parseInt(GetNodeValue(dtSumQ[0], "SumSalesPrice2")).toLocaleString('en-US'));
            }
            else if ($('#rdoDate_PSNO').prop('checked') == true) {
                grdDate_PSNO.BindData(dtE);
                if (dtE.length == 0) {
                    DyAlert("無符合資料!");
                    //$(".modal-backdrop").remove();
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

    let Step2_click = function (bt) {
        $('#lblPSNO_PSNO_ShopNoDate').html($('#lblPSNO_PSNO').html())
        $('#lblActivityCode_PSNO_ShopNoDate').html($('#lblActivityCode_PSNO').html())
        $('#lblEDDate_PSNO_ShopNoDate').html($('#lblEDDate_PSNO').html())
        $('#lblPSName_PSNO_ShopNoDate').html($('#lblPSName_PSNO').html())
        if ($('#rdoShop_PSNO').prop('checked') == true) {
            //$('#tbShopNo_PSNO td').closest('tr').css('background-color', 'transparent');
            $(bt).closest('tr').click();
            $('.msg-valid').hide();
            var node = $(grdShopNo_PSNO.ActiveRowTR()).prop('Record');
            //$('#tbShopNo_PSNO td:contains(' + GetNodeValue(node, 'ID') + ')').closest('tr').css('background-color', '#DEEBF7');
            $('#lblType_PSNO_ShopNoDate').html('店&ensp;&ensp;&ensp;&ensp;別')
            $('#lblTypeID_PSNO_ShopNoDate').html(GetNodeValue(node, 'ID'))
            $('#modal_PSNO_ShopNoDate').modal('show');
            setTimeout(function () {
                QueryPSNO_ShopNoDate($('#lblPSNO_PSNO_ShopNoDate').html(), GetNodeValue(node, 'ID').split('-')[0]);
            }, 500);
        }
        else if ($('#rdoDate_PSNO').prop('checked') == true) {
            //$('#tbDate_PSNO td').closest('tr').css('background-color', 'transparent');
            $(bt).closest('tr').click();
            $('.msg-valid').hide();
            var node = $(grdDate_PSNO.ActiveRowTR()).prop('Record');
            //$('#tbDate_PSNO td:contains(' + GetNodeValue(node, 'ID') + ')').closest('tr').css('background-color', '#DEEBF7');
            $('#lblType_PSNO_ShopNoDate').html('銷售日期')
            $('#lblTypeID_PSNO_ShopNoDate').html(GetNodeValue(node, 'ID'))
            $('#modal_PSNO_ShopNoDate').modal('show');
            setTimeout(function () {
                QueryPSNO_ShopNoDate($('#lblPSNO_PSNO_ShopNoDate').html(), GetNodeValue(node, 'ID'));
            }, 500);
        }
    };

    let QueryPSNO_ShopNoDate = function (PS_NO, ID) {
        //$('#tbShopNo_PSNO_ShopNoDate thead tr th').css('background-color', '#ffb620')
        ShowLoading();
        var Flag = "";

        if ($('#rdoShop_PSNO').prop('checked') == true) {
            Flag = "S";
        }
        else if ($('#rdoDate_PSNO').prop('checked') == true) {
            Flag = "D";
        }

        var OpenDate1 = $('#lblEDDate_PSNO_ShopNoDate').html().split('~')[0]
        var OpenDate2 = $('#lblEDDate_PSNO_ShopNoDate').html().split('~')[1]

        var pData = {
            PS_NO: PS_NO,
            ID: ID,
            OpenDate1: OpenDate1,
            OpenDate2: OpenDate2,
            Flag: Flag
        }
        PostToWebApi({ url: "api/SystemSetup/MSSD102Query_Step2", data: pData, success: afterMSSD102Query_Step2 });
    };

    let afterMSSD102Query_Step2 = function (data) {
        CloseLoading();
        if (ReturnMsg(data, 0) != "MSSD102Query_Step2OK") {
            DyAlert(ReturnMsg(data, 1));
        }
        else {
            var dtE = data.getElementsByTagName('dtE');
            var dtSumQ = data.getElementsByTagName('dtSumQ');
            grdShopNo_PSNO_ShopNoDate.BindData(dtE);
            var heads = $('#tbShopNo_PSNO_ShopNoDate thead tr th#thname_PSNO_ShopNoDate');
            if ($('#rdoShop_PSNO').prop('checked') == true) {
                $(heads).html('銷售日期');
            }
            else if ($('#rdoDate_PSNO').prop('checked') == true) {
                $(heads).html('店別');
            }

            if (dtE.length == 0) {
                DyAlert("無符合資料!");
                //$(".modal-backdrop").remove();
                $('#tbShopNo_PSNO_ShopNoDate thead td#tdShopNo1_PSNO_ShopNoDate').html('');
                $('#tbShopNo_PSNO_ShopNoDate thead td#tdShopNo2_PSNO_ShopNoDate').html('');
                $('#tbShopNo_PSNO_ShopNoDate thead td#tdShopNo3_PSNO_ShopNoDate').html('');
                $('#tbShopNo_PSNO_ShopNoDate thead td#tdShopNo4_PSNO_ShopNoDate').html('');
                $('#tbShopNo_PSNO_ShopNoDate thead td#tdShopNo5_PSNO_ShopNoDate').html('');
                $('#tbShopNo_PSNO_ShopNoDate thead td#tdShopNo6_PSNO_ShopNoDate').html('');
                $('#tbShopNo_PSNO_ShopNoDate thead td#tdShopNo7_PSNO_ShopNoDate').html('');
                $('#tbShopNo_PSNO_ShopNoDate thead td#tdShopNo8_PSNO_ShopNoDate').html('');
                return;
            }
            $('#tbShopNo_PSNO_ShopNoDate thead td#tdShopNo1_PSNO_ShopNoDate').html(parseInt(GetNodeValue(dtSumQ[0], "SumReclaimQty")).toLocaleString('en-US'));
            $('#tbShopNo_PSNO_ShopNoDate thead td#tdShopNo2_PSNO_ShopNoDate').html(parseInt(GetNodeValue(dtSumQ[0], "SumShareAmt")).toLocaleString('en-US'));
            $('#tbShopNo_PSNO_ShopNoDate thead td#tdShopNo3_PSNO_ShopNoDate').html(parseInt(GetNodeValue(dtSumQ[0], "SumReclaimCash")).toLocaleString('en-US'));
            $('#tbShopNo_PSNO_ShopNoDate thead td#tdShopNo4_PSNO_ShopNoDate').html(parseInt(GetNodeValue(dtSumQ[0], "SumReclaimTrans")).toLocaleString('en-US'));
            $('#tbShopNo_PSNO_ShopNoDate thead td#tdShopNo5_PSNO_ShopNoDate').html(parseInt(GetNodeValue(dtSumQ[0], "SumReclaimPrice")).toLocaleString('en-US'));
            $('#tbShopNo_PSNO_ShopNoDate thead td#tdShopNo6_PSNO_ShopNoDate').html(parseInt(GetNodeValue(dtSumQ[0], "SumTotalCash")).toLocaleString('en-US'));
            $('#tbShopNo_PSNO_ShopNoDate thead td#tdShopNo7_PSNO_ShopNoDate').html(parseInt(GetNodeValue(dtSumQ[0], "SumTotalTrans")).toLocaleString('en-US'));
            $('#tbShopNo_PSNO_ShopNoDate thead td#tdShopNo8_PSNO_ShopNoDate').html(parseInt(GetNodeValue(dtSumQ[0], "SumTotalPrice")).toLocaleString('en-US'));
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
        $('#txtActivityCode').val('');
        $('#txtPSName').val('');
        $('#txtEDDate').val('');
    };

    //查詢
    let btQuery_click = function (bt) {
        //Timerset();
        //$('#tbQuery thead tr th').css('background-color', '#ffb620')
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
                //$(".modal-backdrop").remove();
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
            DyAlert(ReturnMsg(data, 1), function () {
                $('#btQLookup_ActivityCode').prop('disabled', false);
            });
        }
        else {
            var dtE = data.getElementsByTagName('dtE');
            if (dtE.length == 0) {
                DyAlert("無符合資料!", function () {
                    $('#btQLookup_ActivityCode').prop('disabled', false);
                });
                //$(".modal-backdrop").remove();
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
            DyAlert("未選取活動代號，請重新確認!", function () {
                $('#btLpOK_ActivityCode').prop('disabled', false);
            });
        }
        else {
            var a = $(obchkedtd[0]).closest('tr');
            var trNode = $(a).prop('Record');
            $('#txtActivityCode').val(GetNodeValue(trNode, "ActivityCode"))
            $('#btLpOK_ActivityCode').prop('disabled', false);
            ClearQuery();
            $('#modal_Lookup_ActivityCode').modal('hide')
        }
    };

    let btLpExit_ActivityCode_click = function (bt) {
        //Timerset();
        $('#modal_Lookup_ActivityCode').modal('hide')
    };

    let btLpClear_ActivityCode_click = function (bt) {
        $("#txtQLookup_ActivityCode").val('');
        $("#tbDataLookup_ActivityCode .checkbox").prop('checked', false);
    };

    let btRe_PSNO_click = function (bt) {
        //Timerset();
        $('#modal_PSNO').modal('hide')
    };

    let btRe_PSNO_ShopNoDate_click = function (bt) {
        //Timerset();
        $('#modal_PSNO_ShopNoDate').modal('hide')
    };

    let ClearQuery = function () {
        grdM.BindData(null)
    }
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
            $('#txtActivityCode,#txtPSName').keydown(function () { ClearQuery() })
            $('#txtEDDate').change(function () { ClearQuery() })

            $('#btActivityCode').click(function () { btActivityCode_click(this) });
            $('#btQLookup_ActivityCode').click(function () { btQLookup_ActivityCode_click(this) });
            $('#btLpOK_ActivityCode').click(function () { btLpOK_ActivityCode_click(this) });
            $('#btLpExit_ActivityCode').click(function () { btLpExit_ActivityCode_click(this) });
            $('#btLpClear_ActivityCode').click(function () { btLpClear_ActivityCode_click(this) });

            $('#btRe_PSNO').click(function () { btRe_PSNO_click(this) });
            $('#rdoShop_PSNO,#rdoDate_PSNO').change(function () { QueryPSNO($('#lblPSNO_PSNO').html()) });

            $('#btRe_PSNO_ShopNoDate').click(function () { btRe_PSNO_ShopNoDate_click(this) });
            btQuery_click();
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