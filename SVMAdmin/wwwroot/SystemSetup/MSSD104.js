var PageMSSD104 = function (ParentNode) {

    let grdM;
    let grdD_S;
    let grdD_D;
    let grdDD;
    let grdLookUp_ActivityCode;

    let AssignVar = function () {

        grdM = new DynGrid(
            {
                table_lement: $('#tbQuery')[0],
                class_collection: ["tdCol1 text-center", "tdCol2 text-center", "tdCol3 text-center", "tdCol4 text-center", "tdCol5 text-center", "tdCol6 label-align", "tdCol7 label-align", "tdCol8 text-center", "tdCol9 label-align", "tdCol10 label-align", "tdCol11 label-align", "tdCol12 label-align"],
                fields_info: [
                    { type: "Text", name: "ActivityCode" },
                    { type: "Text", name: "PS_Name" },
                    { type: "Text", name: "EDDate" },
                    { type: "Text", name: "BIR_Year"},
                    { type: "Text", name: "BIR_Month"},
                    { type: "TextAmt", name: "issueQty"},
                    { type: "TextAmt", name: "ReclaimQty"},
                    { type: "TextPercent", name: "RePercent"},
                    { type: "TextAmt", name: "ShareAmt"},
                    { type: "TextAmt", name: "ReclaimCash" },
                    { type: "TextAmt", name: "ReclaimTrans" },
                    { type: "TextAmt", name: "Price" }
                ],
                //rows_per_page: 10,
                method_clickrow: click_PLU,
                afterBind: InitModifyDeleteButton,
                sortable: "Y",
                step: "Y"
            }
        );

        grdD_S = new DynGrid(
            {
                table_lement: $('#tbQueryD_S')[0],
                class_collection: ["tdCol1 text-center", "tdCol2 label-align", "tdCol3 label-align", "tdCol4 text-center", "tdCol5 label-align", "tdCol6 label-align", "tdCol7 label-align", "tdCol8 label-align", "tdCol9 label-align", "tdCol10 label-align", "tdCol11 label-align"],
                fields_info: [
                    { type: "Text", name: "ID" },
                    { type: "TextAmt", name: "issueQty" },
                    { type: "TextAmt", name: "ReclaimQty" },
                    { type: "TextPercent", name: "RePercent" },
                    { type: "TextAmt", name: "ShareAmt" },
                    { type: "TextAmt", name: "ReclaimCash" },
                    { type: "TextAmt", name: "ReclaimTrans" },
                    { type: "TextAmt", name: "Price" },
                    { type: "TextAmt", name: "TotalCash" },
                    { type: "TextAmt", name: "TotalTrans" },
                    { type: "TextAmt", name: "PriceAll" }
                ],
                //rows_per_page: 10,
                method_clickrow: click_PLU,
                afterBind: gridclick1,
                sortable: "Y",
                step: "Y"
            }
        );

        grdD_D = new DynGrid(
            {
                table_lement: $('#tbQueryD_D')[0],
                class_collection: ["tdCol1 text-center", "tdCol2 label-align", "tdCol3 label-align", "tdCol4 label-align", "tdCol5 label-align", "tdCol6 label-align", "tdCol7 label-align", "tdCol8 label-align", "tdCol9 label-align"],
                fields_info: [
                    { type: "Text", name: "ID" },
                    { type: "TextAmt", name: "ReclaimQty" },
                    { type: "TextAmt", name: "ShareAmt" },
                    { type: "TextAmt", name: "ReclaimCash" },
                    { type: "TextAmt", name: "ReclaimTrans" },
                    { type: "TextAmt", name: "Price" },
                    { type: "TextAmt", name: "TotalCash" },
                    { type: "TextAmt", name: "TotalTrans" },
                    { type: "TextAmt", name: "PriceAll" }
                ],
                //rows_per_page: 10,
                method_clickrow: click_PLU,
                afterBind: gridclick2,
                sortable: "Y",
                step: "Y"
            }
        );

        grdDD = new DynGrid(
            {
                table_lement: $('#tbQueryDD')[0],
                class_collection: ["tdCol1 text-center", "tdCol2 label-align", "tdCol3 label-align", "tdCol4 label-align", "tdCol5 label-align", "tdCol6 label-align", "tdCol7 label-align", "tdCol8 label-align", "tdCol9 label-align"],
                fields_info: [
                    { type: "Text", name: "ID" },
                    { type: "TextAmt", name: "ReclaimQty" },
                    { type: "TextAmt", name: "ShareAmt" },
                    { type: "TextAmt", name: "ReclaimCash" },
                    { type: "TextAmt", name: "ReclaimTrans" },
                    { type: "TextAmt", name: "Price" },
                    { type: "TextAmt", name: "TotalCash" },
                    { type: "TextAmt", name: "TotalTrans" },
                    { type: "TextAmt", name: "PriceAll" }
                ],
                //rows_per_page: 10,
                method_clickrow: click_PLU,
                sortable: "Y",
                step: "Y"
            }
        );

        grdLookUp_ActivityCode = new DynGrid(
            {
                table_lement: $('#tbLookup_ActivityCode')[0],
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
                sortable: "N",
                step: "Y"
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
        $('#tbQueryD_S tbody tr td').click(function () { Step2_click(this) });
    }

    let gridclick2 = function () {
        $('#tbQueryD_D tbody tr td').click(function () { Step2_click(this) });
    }

    let Step1_click = function (bt) {
        $(bt).closest('tr').click();
        $('.msg-valid').hide();
        var node = $(grdM.ActiveRowTR()).prop('Record');
        $('#lblPSNO_D').html(GetNodeValue(node, 'PS_NO'))
        $('#lblActivityCode_D').html(GetNodeValue(node, 'ActivityCode'))
        $('#lblBIRYear_D').html(GetNodeValue(node, 'BIR_Year'))
        $('#lblPSName_D').html(GetNodeValue(node, 'PS_Name'))
        $('#lblBIRMonth_D').html(GetNodeValue(node, 'BIR_Month') + '月份')
        $('#lblEDDate_D').html(GetNodeValue(node, 'EDDate'))
        $('#rdoS_D').prop('checked', true);
        $('#modalD').modal('show');
        setTimeout(function () {
            QueryD(GetNodeValue(node, 'PS_NO'));
        }, 500);
    };

    let QueryD = function (PS_NO) {
        ShowLoading();
        var Flag = "";
        if ($('#rdoS_D').prop('checked') == true) {
            Flag = "S";
            $('#tbQueryD_S').show();
            $('#tbQueryD_D').hide();
        }
        else if ($('#rdoD_D').prop('checked') == true) {
            Flag = "D";
            $('#tbQueryD_S').hide();
            if ($('#tbQueryD_D').attr('hidden') == undefined) {
                $('#tbQueryD_D').show();
            }
            else {
                $('#tbQueryD_D').removeAttr('hidden');
                $('#tbQueryD_D').show();
            }
        }
        var pData = {
            PS_NO: PS_NO,
            Flag: Flag
        }
        PostToWebApi({ url: "api/SystemSetup/MSSD104QueryD", data: pData, success: afterMSSD104QueryD });
    };

    let afterMSSD104QueryD = function (data) {
        CloseLoading();
        if (ReturnMsg(data, 0) != "MSSD104QueryDOK") {
            DyAlert(ReturnMsg(data, 1));
        }
        else {
            var dtE = data.getElementsByTagName('dtE');
            if ($('#rdoS_D').prop('checked') == true) {
                grdD_S.BindData(dtE);
                if (dtE.length == 0) {
                    DyAlert("無符合資料!");
                    var sumtdQD = document.querySelector('.QSumD_S');
                    for (i = 0; i < sumtdQD.childElementCount; i++) {
                        if (i == 0) {
                            sumtdQD.children[i].innerHTML = "總計";
                        }
                        else {
                            sumtdQD.children[i].innerHTML = "";
                        }
                    }
                    return;
                }
                var dtH = data.getElementsByTagName('dtH');
                $('#tbQueryD_S thead td#tdD1_S').html(parseInt(GetNodeValue(dtH[0], "SumissueQty")).toLocaleString('en-US'));
                $('#tbQueryD_S thead td#tdD2_S').html(parseInt(GetNodeValue(dtH[0], "SumReclaimQty")).toLocaleString('en-US'));
                $('#tbQueryD_S thead td#tdD3_S').html(GetNodeValue(dtH[0], "SumRePercent"));
                $('#tbQueryD_S thead td#tdD4_S').html(parseInt(GetNodeValue(dtH[0], "SumShareAmt")).toLocaleString('en-US'));
                $('#tbQueryD_S thead td#tdD5_S').html(parseInt(GetNodeValue(dtH[0], "SumReclaimCash")).toLocaleString('en-US'));
                $('#tbQueryD_S thead td#tdD6_S').html(parseInt(GetNodeValue(dtH[0], "SumReclaimTrans")).toLocaleString('en-US'));
                $('#tbQueryD_S thead td#tdD7_S').html(parseInt(GetNodeValue(dtH[0], "SumPrice")).toLocaleString('en-US'));
                $('#tbQueryD_S thead td#tdD8_S').html(parseInt(GetNodeValue(dtH[0], "SumTotalCash")).toLocaleString('en-US'));
                $('#tbQueryD_S thead td#tdD9_S').html(parseInt(GetNodeValue(dtH[0], "SumTotalTrans")).toLocaleString('en-US'));
                $('#tbQueryD_S thead td#tdD10_S').html(parseInt(GetNodeValue(dtH[0], "SumPriceAll")).toLocaleString('en-US'));
            }
            else if ($('#rdoD_D').prop('checked') == true) {
                grdD_D.BindData(dtE);
                if (dtE.length == 0) {
                    DyAlert("無符合資料!");
                    var sumtdQD = document.querySelector('.QSumD_D');
                    for (i = 0; i < sumtdQD.childElementCount; i++) {
                        if (i == 0) {
                            sumtdQD.children[i].innerHTML = "總計";
                        }
                        else {
                            sumtdQD.children[i].innerHTML = "";
                        }
                    }
                    return;
                }
                var dtH = data.getElementsByTagName('dtH');
                $('#tbQueryD_D thead td#tdD1_D').html(parseInt(GetNodeValue(dtH[0], "SumReclaimQty")).toLocaleString('en-US'));
                $('#tbQueryD_D thead td#tdD2_D').html(parseInt(GetNodeValue(dtH[0], "SumShareAmt")).toLocaleString('en-US'));
                $('#tbQueryD_D thead td#tdD3_D').html(parseInt(GetNodeValue(dtH[0], "SumReclaimCash")).toLocaleString('en-US'));
                $('#tbQueryD_D thead td#tdD4_D').html(parseInt(GetNodeValue(dtH[0], "SumReclaimTrans")).toLocaleString('en-US'));
                $('#tbQueryD_D thead td#tdD5_D').html(parseInt(GetNodeValue(dtH[0], "SumPrice")).toLocaleString('en-US'));
                $('#tbQueryD_D thead td#tdD6_D').html(parseInt(GetNodeValue(dtH[0], "SumTotalCash")).toLocaleString('en-US'));
                $('#tbQueryD_D thead td#tdD7_D').html(parseInt(GetNodeValue(dtH[0], "SumTotalTrans")).toLocaleString('en-US'));
                $('#tbQueryD_D thead td#tdD8_D').html(parseInt(GetNodeValue(dtH[0], "SumPriceAll")).toLocaleString('en-US'));
            }
        }
    };

    let Step2_click = function (bt) {
        $(bt).closest('tr').click();
        $('.msg-valid').hide();
        $('#lblPSNO_DD').html($('#lblPSNO_D').html())
        $('#lblActivityCode_DD').html($('#lblActivityCode_D').html())
        $('#lblBIRYear_DD').html($('#lblBIRYear_D').html())
        $('#lblPSName_DD').html($('#lblPSName_D').html())
        $('#lblBIRMonth_DD').html($('#lblBIRMonth_D').html())
        $('#lblEDDate_DD').html($('#lblEDDate_D').html())
        if ($('#rdoS_D').prop('checked') == true) {
            var node = $(grdD_S.ActiveRowTR()).prop('Record');
            $('#lblConText_DD').html('店&ensp;&ensp;&ensp;&ensp;別')
            $('#lblCon_DD').html(GetNodeValue(node, 'ID'))
            $('#modalDD').modal('show');
            setTimeout(function () {
                QueryDD($('#lblPSNO_DD').html(), $('#lblCon_DD').html().split('-')[0]);
            }, 500);
        }
        else if ($('#rdoD_D').prop('checked') == true) {
            var node = $(grdD_D.ActiveRowTR()).prop('Record');
            $('#lblConText_DD').html('銷售日期')
            $('#lblCon_DD').html(GetNodeValue(node, 'ID'))
            $('#modalDD').modal('show');
            setTimeout(function () {
                QueryDD($('#lblPSNO_DD').html(), $('#lblCon_DD').html());
            }, 500);
        }
    };

    let QueryDD = function (PS_NO,ID) {
        ShowLoading();
        var Flag = "";
        if ($('#rdoS_D').prop('checked') == true) {
            Flag = "S";
        }
        else if ($('#rdoD_D').prop('checked') == true) {
            Flag = "D";
        }
        var pData = {
            PS_NO: PS_NO,
            ID: ID,
            Flag: Flag
        }
        PostToWebApi({ url: "api/SystemSetup/MSSD104QueryDD", data: pData, success: afterMSSD104QueryDD });
    };

    let afterMSSD104QueryDD = function (data) {
        CloseLoading();
        if (ReturnMsg(data, 0) != "MSSD104QueryDDOK") {
            DyAlert(ReturnMsg(data, 1));
        }
        else {
            var dtE = data.getElementsByTagName('dtE');
            var heads = $('#tbQueryDD thead tr th#theadDD1');
            if ($('#rdoS_D').prop('checked') == true) {
                $(heads).html('銷售日期');
            }
            else if ($('#rdoD_D').prop('checked') == true) {
                $(heads).html('店別');
            }
            grdDD.BindData(dtE);
            if (dtE.length == 0) {
                DyAlert("無符合資料!");
                var sumtdQD = document.querySelector('.QSumDD');
                for (i = 0; i < sumtdQD.childElementCount; i++) {
                    if (i == 0) {
                        sumtdQD.children[i].innerHTML = "總計";
                    }
                    else {
                        sumtdQD.children[i].innerHTML = "";
                    }
                }
                return;
            }
            var dtH = data.getElementsByTagName('dtH');
            $('#tbQueryDD thead td#tdDD1').html(parseInt(GetNodeValue(dtH[0], "SumReclaimQty")).toLocaleString('en-US'));
            $('#tbQueryDD thead td#tdDD2').html(parseInt(GetNodeValue(dtH[0], "SumShareAmt")).toLocaleString('en-US'));
            $('#tbQueryDD thead td#tdDD3').html(parseInt(GetNodeValue(dtH[0], "SumReclaimCash")).toLocaleString('en-US'));
            $('#tbQueryDD thead td#tdDD4').html(parseInt(GetNodeValue(dtH[0], "SumReclaimTrans")).toLocaleString('en-US'));
            $('#tbQueryDD thead td#tdDD5').html(parseInt(GetNodeValue(dtH[0], "SumPrice")).toLocaleString('en-US'));
            $('#tbQueryDD thead td#tdDD6').html(parseInt(GetNodeValue(dtH[0], "SumTotalCash")).toLocaleString('en-US'));
            $('#tbQueryDD thead td#tdDD7').html(parseInt(GetNodeValue(dtH[0], "SumTotalTrans")).toLocaleString('en-US'));
            $('#tbQueryDD thead td#tdDD8').html(parseInt(GetNodeValue(dtH[0], "SumPriceAll")).toLocaleString('en-US'));
        }
    };
    //清除
    let btClear_click = function (bt) {
        //Timerset();
        $('#txtActivityCode').val('');
        $('#txtPSName').val('');
        $('#cboBIRYear').val('');
        $('#cboBIRMonth').val('');
    };

    //查詢
    let btQuery_click = function (bt) {
        //Timerset();
        $('#btQuery').prop('disabled', true)
        ShowLoading();
        var pData = {
            ActivityCode: $('#txtActivityCode').val(),
            BirYear: $('#cboBIRYear').val(),
            PSName: $('#txtPSName').val(),
            BirMonth: $('#cboBIRMonth').val()
        }
        PostToWebApi({ url: "api/SystemSetup/MSSD104Query", data: pData, success: afterMSSD104Query });
    };

    let afterMSSD104Query = function (data) {
        CloseLoading();
        if (ReturnMsg(data, 0) != "MSSD104QueryOK") {
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

    //活動代號[...]
    let btActivityCode_click = function (bt) {
        //Timerset();
        var pData = {
            ActivityCode: $('#txtActivityCode').val()
        }
        PostToWebApi({ url: "api/SystemSetup/MSSD104_LookUpActivityCode", data: pData, success: afterMSDMLookUpActivityCode });
    };

    let afterMSDMLookUpActivityCode = function (data) {
        if (ReturnMsg(data, 0) != "MSSD104_LookUpActivityCodeOK") {
            DyAlert(ReturnMsg(data, 1));
        }
        else {
            var dtE = data.getElementsByTagName('dtE');
            //if (dtE.length == 0) {
            //    DyAlert("無符合資料!");
            //    $(".modal-backdrop").remove();
            //    return;
            //}
            $('#txtLpQ_ActivityCode').val($('#txtActivityCode').val());
            $('#modalLookup_ActivityCode').modal('show');
            setTimeout(function () {
                grdLookUp_ActivityCode.BindData(dtE);
                $('#tbLookup_ActivityCode tbody tr .tdCol2').filter(function () { return $(this).text() == $('#txtActivityCode').val(); }).closest('tr').find('.tdCol1 input:radio').prop('checked', true);
            }, 500);
        }
    };

    let btLpQ_ActivityCode_click = function (bt) {
        //Timerset();
        $('#btLpQ_ActivityCode').prop('disabled', true)
        var pData = {
            ActivityCode: $('#txtLpQ_ActivityCode').val()
        }
        PostToWebApi({ url: "api/SystemSetup/MSSD104_LookUpActivityCode", data: pData, success: afterQMSDMLookUpActivityCode });
    };

    let afterQMSDMLookUpActivityCode = function (data) {
        if (ReturnMsg(data, 0) != "MSSD104_LookUpActivityCodeOK") {
            DyAlert(ReturnMsg(data, 1), function () {
                $('#btLpQ_ActivityCode').prop('disabled', false)
            });
        }
        else {
            var dtE = data.getElementsByTagName('dtE');
            if (dtE.length == 0) {
                DyAlert("無符合資料!", function () {
                    $('#btLpQ_ActivityCode').prop('disabled', false)
                });
                return;
            }
            grdLookUp_ActivityCode.BindData(dtE);
            $('#btLpQ_ActivityCode').prop('disabled', false)
        }
    };

    let btLpOK_ActivityCode_click = function (bt) {
        //Timerset();
        $('#btLpOK_ActivityCode').prop('disabled', true)
        var obchkedtd = $('#tbLookup_ActivityCode input[type="radio"]:checked');
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
            $('#modalLookup_ActivityCode').modal('hide')
        }
    };

    let btLpExit_ActivityCode_click = function (bt) {
        //Timerset();
        $('#modalLookup_ActivityCode').modal('hide')
    };

    let btLpClear_ActivityCode_click = function (bt) {
        //Timerset();
        $("#txtLpQ_ActivityCode").val('');
        $("#tbLookup_ActivityCode .checkbox").prop('checked', false);
    };

    let btRe_D_click = function (bt) {
        //Timerset();
        $('#modalD').modal('hide')
    };

    let btRe_DD_click = function (bt) {
        //Timerset();
        $('#modalDD').modal('hide')
    };

    let InitComboItem = function (cboYear, cboMonth) {
        var y2 = new Date().getFullYear();
        for (i = 2024; i <= y2 + 1; i++) {
            cboYear.append($('<option>', { value: i, text: i }));
        }

        for (i = 1; i <= 12; i++) {
            cboMonth.append($('<option>', { value: ('0' + i).substr(-2), text: i + '月' }));
        }
    };

    let ClearQuery = function () {
        grdM.BindData(null)
    }
//#region FormLoad
    let GetInitMSSD104 = function (data) {
        if (ReturnMsg(data, 0) != "GetInitmsDMOK") {
            DyAlert(ReturnMsg(data, 1));
        }
       else {
           var dtE = data.getElementsByTagName('dtE');
            $('#lblProgramName').html(GetNodeValue(dtE[0], "ChineseName"));
            InitComboItem($("#cboBIRYear"), $('#cboBIRMonth'));    //下拉選單
            AssignVar();
            $('#btQuery').click(function () { btQuery_click(this) });
            $('#btClear').click(function () { btClear_click(this) });
            $('#txtActivityCode,#txtPSName').keydown(function () { ClearQuery() })
            $('#cboBIRYear,#cboBIRMonth').change(function () { ClearQuery() })

            $('#btActivityCode').click(function () { btActivityCode_click(this) });
            $('#btLpQ_ActivityCode').click(function () { btLpQ_ActivityCode_click(this) });
            $('#btLpOK_ActivityCode').click(function () { btLpOK_ActivityCode_click(this) });
            $('#btLpExit_ActivityCode').click(function () { btLpExit_ActivityCode_click(this) });
            $('#btLpClear_ActivityCode').click(function () { btLpClear_ActivityCode_click(this) });

            $('#rdoS_D,#rdoD_D').change(function () { QueryD($('#lblPSNO_D').html()) });
            $('#btRe_D').click(function () { btRe_D_click(this) });
   
            $('#btRe_DD').click(function () { btRe_DD_click(this) });

            btQuery_click();
        }
    };
    
    let afterLoadPage = function () {
        var pData = {
            ProgramID: "MSSD104"
        }
        PostToWebApi({ url: "api/SystemSetup/GetInitmsDM", data: pData, success: GetInitMSSD104 });
    };
//#endregion
    

    if ($('#pgMSSD104').length == 0) {  
        AllPages = new LoadAllPages(ParentNode, "SystemSetup/MSSD104", ["MSSD104btns", "pgMSSD104Init", "pgMSSD104Add", "pgMSSD104Mod"], afterLoadPage);
    };


}