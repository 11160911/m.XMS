var PageMSSD103 = function (ParentNode) {

    let grdM;
    let grdD;
    let grdDD;
    let grdLookUp_ActivityCode;

    let AssignVar = function () {

        grdM = new DynGrid(
            {
                table_lement: $('#tbQuery')[0],
                class_collection: ["tdCol1", "tdCol2 text-center", "tdCol3", "tdCol4 text-center", "tdCol5 label-align", "tdCol6 text-center", "tdCol7 label-align", "tdCol8 text-center", "tdCol9 label-align", "tdCol10 label-align", "tdCol11 label-align", "tdCol12 label-align"],
                fields_info: [
                    { type: "Text", name: "PS_NO", style: "" },
                    { type: "Text", name: "ActivityCode" },
                    { type: "Text", name: "PS_Name" },
                    { type: "Text", name: "PDate"},
                    { type: "TextAmt", name: "Cnt1" },
                    { type: "Text", name: "EDDate" },
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
                step: "Y"
            }
        );

        grdD = new DynGrid(
            {
                table_lement: $('#tbD_PSNO')[0],
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

        grdDD = new DynGrid(
            {
                table_lement: $('#tbDD_PSNO')[0],
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

    //#region 第二層
    let Step1_click = function (bt) {
        //$('#tbQuery td').closest('tr').css('background-color', 'transparent');
        $(bt).closest('tr').click();
        $('.msg-valid').hide();
        var node = $(grdM.ActiveRowTR()).prop('Record');
        //$('#tbQuery td:contains(' + GetNodeValue(node, 'PS_NO') + ')').closest('tr').css('background-color', '#DEEBF7');

        $('#lblD_PSNO').html(GetNodeValue(node, 'PS_NO'))
        $('#lblActivityCode_D').html(GetNodeValue(node, 'ActivityCode'))
        $('#lblPDate_D').html(GetNodeValue(node, 'PDate'))
        $('#lblPSName_D').html(GetNodeValue(node, 'PS_Name'))
        $('#lblEDDate_D').html(GetNodeValue(node, 'EDDate'))
        $('#rdoShop_D').prop('checked', true);
        $('#modal_D').modal('show');
        setTimeout(function () {
            QueryD();
        }, 500);
    };

    let QueryD = function () {
        //$('#tbD_PSNO thead tr th').css('background-color', '#ffb620')
        ShowLoading();
        var Flag = "";

        if ($('#rdoShop_D').prop('checked') == true) {
            Flag = "SA";
            $('#tbD_PSNO thead tr th#Dthead1').html('店別');
        }
        else if ($('#rdoDate_D').prop('checked') == true) {
            Flag = "DA";
            $('#tbD_PSNO thead tr th#Dthead1').html('銷售日期');
        }

        //var SalesDate1 = $('#lblEDDate_PSNO').html().split('~')[0]
        //var SalesDate2 = $('#lblEDDate_PSNO').html().split('~')[1]

        var pData = {
            PS_NO: $('#lblD_PSNO').html(),
            //SalesDate: SalesDate,
            //ShopNo:"",
            Flag: Flag
        }
        PostToWebApi({ url: "api/SystemSetup/MSSD103QueryD", data: pData, success: afterMSSD103QueryD });
    };

    let afterMSSD103QueryD = function (data) {
        CloseLoading();
        if (ReturnMsg(data, 0) != "MSSD103QueryDOK") {
            DyAlert(ReturnMsg(data, 1));
        }
        else {
            var dtE = data.getElementsByTagName('dtE');
            var dtSum = data.getElementsByTagName('dtSum');
            grdD.BindData(dtE);
            if (dtE.length == 0) {
                DyAlert("無符合資料!");
                //$(".modal-backdrop").remove();
                var sumtdQD = document.querySelector('.QDSum');
                for (i = 0; i < sumtdQD.childElementCount; i++) {
                    sumtdQD.children[i].innerHTML = "";
                }
                return;
            }
            if (dtSum.length == 0) {
                return;
            } else {
                $('#tbD_PSNO thead td#Dtd1').html(parseInt(GetNodeValue(dtSum[0], "Cnt1")).toLocaleString('en-US'));
                $('#tbD_PSNO thead td#Dtd2').html(parseInt(GetNodeValue(dtSum[0], "Cnt2")).toLocaleString('en-US'));
                $('#tbD_PSNO thead td#Dtd3').html(GetNodeValue(dtSum[0], "RePercent"));
                $('#tbD_PSNO thead td#Dtd4').html(parseInt(GetNodeValue(dtSum[0], "ActualDiscount")).toLocaleString('en-US'));
                $('#tbD_PSNO thead td#Dtd5').html(parseInt(GetNodeValue(dtSum[0], "SalesCash1")).toLocaleString('en-US'));
                $('#tbD_PSNO thead td#Dtd6').html(parseInt(GetNodeValue(dtSum[0], "SalesCnt1")).toLocaleString('en-US'));
                $('#tbD_PSNO thead td#Dtd7').html(parseInt(GetNodeValue(dtSum[0], "SalesPrice1")).toLocaleString('en-US'));
                $('#tbD_PSNO thead td#Dtd8').html(parseInt(GetNodeValue(dtSum[0], "SalesCash2")).toLocaleString('en-US'));
                $('#tbD_PSNO thead td#Dtd9').html(parseInt(GetNodeValue(dtSum[0], "SalesCnt2")).toLocaleString('en-US'));
                $('#tbD_PSNO thead td#Dtd10').html(parseInt(GetNodeValue(dtSum[0], "SalesPrice2")).toLocaleString('en-US'));
            }
        }
    };

    let btRe_D_click = function (bt) {
        //Timerset();
        $('#modal_D').modal('hide')
        setTimeout(function () {
            grdD.BindData(null)
            var sumtdQD = document.querySelector('.QDSum');
            for (i = 0; i < sumtdQD.childElementCount; i++) {
                sumtdQD.children[i].innerHTML = "";
            }
        }, 500);
    };
    //#endregion

    let gridclick1 = function () {
        $('#tbD_PSNO tbody tr td').click(function () { Step2_click(this) });
    }

    //#region 第三層
    let Step2_click = function (bt) {
        $('#lblDD_PSNO').html($('#lblD_PSNO').html())
        $('#lblActivityCode_DD').html($('#lblActivityCode_D').html())
        $('#lblPDate_DD').html($('#lblPDate_D').html())
        $('#lblPSName_DD').html($('#lblPSName_D').html())
        $('#lblEDDate_DD').html($('#lblEDDate_D').html())
        if ($('#rdoShop_D').prop('checked') == true) {
            $('#lblType_DD').html('店&ensp;&ensp;&ensp;&ensp;別')
        }
        else if ($('#rdoDate_D').prop('checked') == true) {
            $('#lblType_DD').html('銷售日期')
        }

        //$('#tbD_PSNO td').closest('tr').css('background-color', 'transparent');
        $(bt).closest('tr').click();
        $('.msg-valid').hide();
        var node = $(grdD.ActiveRowTR()).prop('Record');
        //$('#tbD_PSNO td:contains(' + GetNodeValue(node, 'ID') + ')').closest('tr').css('background-color', '#DEEBF7');
        $('#lblTypeID_DD').html(GetNodeValue(node, 'ID'))
        //$('#tbDD_PSNO thead tr th').css('background-color', '#ffb620')
        $('#modal_DD').modal('show');
        setTimeout(function () {
            QueryDD(node);
        }, 500);
    };

    let QueryDD = function (rec) {
        ShowLoading();
        var Flag = "";
        var SalesDate;
        var ShopNo;
        if ($('#rdoShop_D').prop('checked') == true) {
            Flag = "S1";
            $('#tbDD_PSNO thead tr th#DDthead1').html('銷售日期');
            ShopNo = $('#lblTypeID_DD').html().split('-')[0];
        }
        else if ($('#rdoDate_D').prop('checked') == true) {
            Flag = "D1";
            $('#tbDD_PSNO thead tr th#DDthead1').html('店別');
            SalesDate = $('#lblTypeID_DD').html();
        }
        setTimeout(function () {
            $('#tbDD_PSNO thead td#DDtd1').html(parseInt(GetNodeValue(rec, "Cnt1")).toLocaleString('en-US'));
            $('#tbDD_PSNO thead td#DDtd2').html(parseInt(GetNodeValue(rec, "Cnt2")).toLocaleString('en-US'));
            $('#tbDD_PSNO thead td#DDtd3').html(GetNodeValue(rec, "RePercent"));
            $('#tbDD_PSNO thead td#DDtd4').html(parseInt(GetNodeValue(rec, "ActualDiscount")).toLocaleString('en-US'));
            $('#tbDD_PSNO thead td#DDtd5').html(parseInt(GetNodeValue(rec, "SalesCash1")).toLocaleString('en-US'));
            $('#tbDD_PSNO thead td#DDtd6').html(parseInt(GetNodeValue(rec, "SalesCnt1")).toLocaleString('en-US'));
            $('#tbDD_PSNO thead td#DDtd7').html(parseInt(GetNodeValue(rec, "SalesPrice1")).toLocaleString('en-US'));
            $('#tbDD_PSNO thead td#DDtd8').html(parseInt(GetNodeValue(rec, "SalesCash2")).toLocaleString('en-US'));
            $('#tbDD_PSNO thead td#DDtd9').html(parseInt(GetNodeValue(rec, "SalesCnt2")).toLocaleString('en-US'));
            $('#tbDD_PSNO thead td#DDtd10').html(parseInt(GetNodeValue(rec, "SalesPrice2")).toLocaleString('en-US'));
        }, 500);
        var pData = {
            PS_NO: $('#lblDD_PSNO').html(),
            SalesDate: SalesDate,
            ShopNo: ShopNo,
            Flag: Flag
        }
        PostToWebApi({ url: "api/SystemSetup/MSSD103QueryD", data: pData, success: afterMSSD103QueryDD });
    };

    let afterMSSD103QueryDD = function (data) {
        CloseLoading();
        if (ReturnMsg(data, 0) != "MSSD103QueryDOK") {
            DyAlert(ReturnMsg(data, 1));
        }
        else {
            var dtE = data.getElementsByTagName('dtE');
            grdDD.BindData(dtE);
            if (dtE.length == 0) {
                DyAlert("無符合資料!");
                //$(".modal-backdrop").remove();
                var sumtdQDD = document.querySelector('.QDDSum');
                for (i = 0; i < sumtdQDD.childElementCount; i++) {
                    sumtdQDD.children[i].innerHTML = "";
                }
                return;
            }
        }
    };

    let btRe_DD_click = function (bt) {
        //Timerset();
        $('#modal_DD').modal('hide')
        setTimeout(function () {
            //$('#tbQueryDD tbody').remove();
            grdDD.BindData(null)
            var sumtdQDD = document.querySelector('.QDDSum');
            for (i = 0; i < sumtdQDD.childElementCount; i++) {
                sumtdQDD.children[i].innerHTML = "";
            }
        }, 500);
    };
    //#endregion

    let ChkLogOut_1 = function (AfterChkLogOut_1) {
        var LoginDT = sessionStorage.getItem('LoginDT');
        var cData = {
            LoginDT: LoginDT
        }
        PostToWebApi({ url: "api/js/ChkLogOut", data: cData, success: AfterChkLogOut_1 });
    };
//#region 清除
    let btClear_click = function (bt) {
        //Timerset();
        $('#txtActivityCode').val('');
        $('#txtPSName').val('');
        $('#txtEDDate').val('');
    };
//#endregion

//#region 查詢
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
        PostToWebApi({ url: "api/SystemSetup/MSSD103Query", data: pData, success: afterMSSD103Query });
    };

    let afterMSSD103Query = function (data) {
        CloseLoading();
        if (ReturnMsg(data, 0) != "MSSD103QueryOK") {
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

//#region 活動代號...
    let SetLookUpData_ActivityCode = {
        GQ_Table: "PromoteSCouponHWeb",
        GQ_Column: "PS_No,ActivityCode,PS_Name,StartDate,EndDate",
        GQ_OrderColumn: "StartDate desc",
        GQ_Condition: "PS_No in (Select distinct PS_No from PrintCouponRecWeb (nolock) where CompanyCode=a.CompanyCode)",
        QueryValue: null
    };
    let btActivityCode_click = function (bt) {
        //Timerset();
        var pData = SetLookUpData_ActivityCode;
        PostToWebApi({ url: "api/SystemSetup/LookUp", data: pData, success: afterMSSD102_LookUpActivityCode });
    };

    let afterMSSD102_LookUpActivityCode = function (data) {
        if (ReturnMsg(data, 0) != "LookUpOK") {
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
        var pData = SetLookUpData_ActivityCode;
        pData.QueryValue = $('#txtQLookup_ActivityCode').val() == null ? null : $('#txtQLookup_ActivityCode').val();
        PostToWebApi({ url: "api/SystemSetup/LookUp", data: pData, success: afterMSSD102_QLookUpActivityCode });
    };

    let afterMSSD102_QLookUpActivityCode = function (data) {
        if (ReturnMsg(data, 0) != "LookUpOK") {
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
//#endregion

let ClearQuery = function () {
    grdM.BindData(null)
}

//#region FormLoad
    let GetInitMSSD103 = function (data) {
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
            $('#txtActivityCode,#txtPSName').keydown(function () { ClearQuery() });
            $('#txtEDDate').change(function () { ClearQuery() });
            $('#btQLookup_ActivityCode').click(function () { btQLookup_ActivityCode_click(this) });
            $('#btLpOK_ActivityCode').click(function () { btLpOK_ActivityCode_click(this) });
            $('#btLpExit_ActivityCode').click(function () { btLpExit_ActivityCode_click(this) });
            $('#btLpClear_ActivityCode').click(function () { btLpClear_ActivityCode_click(this) });

            $('#btRe_D').click(function () { btRe_D_click(this) });
            $('#rdoShop_D,#rdoDate_D').change(function () { QueryD() });

            $('#btRe_DD').click(function () { btRe_DD_click(this) });

            btQuery_click();
        }
    };
    
    let afterLoadPage = function () {
        var pData = {
            ProgramID: "MSSD103"
        }
        PostToWebApi({ url: "api/SystemSetup/GetInitmsDM", data: pData, success: GetInitMSSD103 });
    };
//#endregion
    
    if ($('#pgMSSD103').length == 0) {  
        AllPages = new LoadAllPages(ParentNode, "SystemSetup/MSSD103", ["pgMSSD103Init"], afterLoadPage);
    };


}