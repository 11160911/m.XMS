var PageMSSD107 = function (ParentNode) {
    let grdM;
    let grdD;
    let AssignVar = function () {
        grdM = new DynGrid(
            {
                table_lement: $('#tbQuery')[0],
                class_collection: ["tdCol1 text-center", "tdCol2 label-align", "tdCol3 label-align", "tdCol4 label-align", "tdCol5 label-align", "tdCol6 label-align", "tdCol7 label-align", "tdCol8 text-center", "tdCol9 label-align", "tdCol10 label-align", "tdCol11 label-align", "tdCol12 text-center"],
                fields_info: [
                    { type: "Text", name: "ID", style: "" },
                    { type: "TextAmt", name: "Cash"},
                    { type: "TextAmt", name: "RecS"},
                    { type: "TextAmt", name: "Price"},
                    { type: "TextAmt", name: "VIP_Cash"},
                    { type: "TextAmt", name: "VIP_RecS"},
                    { type: "TextAmt", name: "VIPPrice"},
                    { type: "TextPercent", name: "VIPPercent" },
                    { type: "TextAmt", name: "VIPNo_Cash" },
                    { type: "TextAmt", name: "VIPNo_RecS" },
                    { type: "TextAmt", name: "VIPNoPrice" },
                    { type: "TextPercent", name: "VIPNoPercent" }
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
                table_lement: $('#tbQueryD')[0],
                class_collection: ["tdCol1 text-center", "tdCol2 label-align", "tdCol3 label-align", "tdCol4 label-align", "tdCol5 label-align", "tdCol6 label-align", "tdCol7 label-align", "tdCol8 text-center", "tdCol9 label-align", "tdCol10 label-align", "tdCol11 label-align", "tdCol12 text-center"],
                fields_info: [
                    { type: "Text", name: "ID", style: "" },
                    { type: "TextAmt", name: "Cash" },
                    { type: "TextAmt", name: "RecS" },
                    { type: "TextAmt", name: "Price" },
                    { type: "TextAmt", name: "VIP_Cash" },
                    { type: "TextAmt", name: "VIP_RecS" },
                    { type: "TextAmt", name: "VIPPrice" },
                    { type: "TextPercent", name: "VIPPercent" },
                    { type: "TextAmt", name: "VIPNo_Cash" },
                    { type: "TextAmt", name: "VIPNo_RecS" },
                    { type: "TextAmt", name: "VIPNoPrice" },
                    { type: "TextPercent", name: "VIPNoPercent" }
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
        $('#cboYear').val('');
        $('#rdoM').prop('checked','true');
    };

    //查詢
    let btQuery_click = function (bt) {
        //Timerset();
        $('#btQuery').prop('disabled', true)
        if ($('#cboYear').val() == "") {
            DyAlert("請輸入年度!", function () { $('#btQuery').prop('disabled', false); })
            return
        }
        if ($('#rdoM').prop('checked') == false && $('#rdoS').prop('checked') == false) {
            DyAlert("統計條件請至少選擇一個項目!", function () { $('#btQuery').prop('disabled', false); })
            return
        }
        ShowLoading();
        setTimeout(function () {
            var Flag = "";
            if ($('#rdoM').prop('checked') == true) {
                Flag = "M"
            }
            else if ($('#rdoS').prop('checked') == true) {
                Flag = "S"
            }
            var pData = {
                YY: $('#cboYear').val(),
                Flag: Flag
            }
            PostToWebApi({ url: "api/SystemSetup/MSSD107Query", data: pData, success: afterMSSD107Query });
        }, 1000);
    };

    let afterMSSD107Query = function (data) {
        CloseLoading();
        if (ReturnMsg(data, 0) != "MSSD107QueryOK") {
            DyAlert(ReturnMsg(data, 1), function () { $('#btQuery').prop('disabled', false); });
        }
        else {
            $('#btQuery').prop('disabled', false);
            var dtE = data.getElementsByTagName('dtE');
            grdM.BindData(dtE);

            var heads = $('#tbQuery thead tr th#thead0');
            if ($('#rdoM').prop('checked')) {
                $(heads).html('月份');
            }
            else if ($('#rdoS').prop('checked')) {
                $(heads).html('店別');
            }
            if (dtE.length == 0) {
                DyAlert("無符合資料!");
                $(".modal-backdrop").remove();
                var sumtdQD = document.querySelector('.QSum');
                for (i = 0; i < sumtdQD.childElementCount; i++) {
                    if (i == 0) {
                        sumtdQD.children[i].innerHTML = "總數";
                    }
                    else {
                        sumtdQD.children[i].innerHTML = "";
                    }
                }
                return;
            }

            var dtH = data.getElementsByTagName('dtH');
            $('#tbQuery thead td#td1').html(parseInt(GetNodeValue(dtH[0], "SumCash")).toLocaleString('en-US'));
            $('#tbQuery thead td#td2').html(parseInt(GetNodeValue(dtH[0], "SumRecS")).toLocaleString('en-US'));
            $('#tbQuery thead td#td3').html(parseInt(GetNodeValue(dtH[0], "SumPrice")).toLocaleString('en-US'));
            $('#tbQuery thead td#td4').html(parseInt(GetNodeValue(dtH[0], "SumVIP_Cash")).toLocaleString('en-US'));
            $('#tbQuery thead td#td5').html(parseInt(GetNodeValue(dtH[0], "SumVIP_RecS")).toLocaleString('en-US'));
            $('#tbQuery thead td#td6').html(parseInt(GetNodeValue(dtH[0], "SumVIPPrice")).toLocaleString('en-US'));
            $('#tbQuery thead td#td7').html(GetNodeValue(dtH[0], "SumVIPPercent"));
            $('#tbQuery thead td#td8').html(parseInt(GetNodeValue(dtH[0], "SumVIPNo_Cash")).toLocaleString('en-US'));
            $('#tbQuery thead td#td9').html(parseInt(GetNodeValue(dtH[0], "SumVIPNo_RecS")).toLocaleString('en-US'));
            $('#tbQuery thead td#td10').html(parseInt(GetNodeValue(dtH[0], "SumVIPNoPrice")).toLocaleString('en-US'));
            $('#tbQuery thead td#td11').html(GetNodeValue(dtH[0], "SumVIPNoPercent"));
        }
    };

    //第一層
    let Step1_click = function (bt) {
        $(bt).closest('tr').click();
        $('.msg-valid').hide();
        var node = $(grdM.ActiveRowTR()).prop('Record');
        $('#lblYear_D').html(GetNodeValue(node, 'YY') + '年')
        var heads = $('#tbQuery thead tr th#thead0');
        var headsD = $('#tbQueryD thead tr th#theadD0');
        if ($(heads).html() == "月份") {
            $('#lblConText').html('月&ensp;&ensp;&ensp;&ensp;份')
            $(headsD).html('店別')
        }
        else if ($(heads).html() == "店別") {
            $('#lblConText').html('店&ensp;&ensp;&ensp;&ensp;別')
            $(headsD).html('月份')
        }
        $('#lblCon').html(GetNodeValue(node, 'ID'))
        $('#modalD').modal('show');
        setTimeout(function () {
            QueryD();
        }, 500);
    };

    let QueryD = function () {
        ShowLoading();
        var Flag = "";
        var ID = "";
        var heads = $('#tbQuery thead tr th#thead0');
        if ($(heads).html() == "月份") {
            Flag = "M";
            ID = $('#lblCon').html().substr(0, 2);
        }
        else if ($(heads).html() == "店別") {
            Flag = "S";
            ID = $('#lblCon').html().split('-')[0];
        }

        var pData = {
            YY: $('#lblYear_D').html().substr(0, 4),
            ID: ID,
            Flag: Flag
        }
        PostToWebApi({ url: "api/SystemSetup/MSSD107QueryD", data: pData, success: afterMSSD107QueryD });
    };

    let afterMSSD107QueryD = function (data) {
        CloseLoading();
        if (ReturnMsg(data, 0) != "MSSD107QueryDOK") {
            DyAlert(ReturnMsg(data, 1));
        }
        else {
            var dtE = data.getElementsByTagName('dtE');
            grdD.BindData(dtE);
            if (dtE.length == 0) {
                DyAlert("無符合資料!");
                $(".modal-backdrop").remove();
                var sumtdQD = document.querySelector('.QSumD');
                for (i = 0; i < sumtdQD.childElementCount; i++) {
                    if (i == 0) {
                        sumtdQD.children[i].innerHTML = "總數";
                    }
                    else {
                        sumtdQD.children[i].innerHTML = "";
                    }
                }
                return;
            }
            var dtH = data.getElementsByTagName('dtH');
            $('#tbQueryD thead td#tdD1').html(parseInt(GetNodeValue(dtH[0], "SumCash")).toLocaleString('en-US'));
            $('#tbQueryD thead td#tdD2').html(parseInt(GetNodeValue(dtH[0], "SumRecS")).toLocaleString('en-US'));
            $('#tbQueryD thead td#tdD3').html(parseInt(GetNodeValue(dtH[0], "SumPrice")).toLocaleString('en-US'));
            $('#tbQueryD thead td#tdD4').html(parseInt(GetNodeValue(dtH[0], "SumVIP_Cash")).toLocaleString('en-US'));
            $('#tbQueryD thead td#tdD5').html(parseInt(GetNodeValue(dtH[0], "SumVIP_RecS")).toLocaleString('en-US'));
            $('#tbQueryD thead td#tdD6').html(parseInt(GetNodeValue(dtH[0], "SumVIPPrice")).toLocaleString('en-US'));
            $('#tbQueryD thead td#tdD7').html(GetNodeValue(dtH[0], "SumVIPPercent"));
            $('#tbQueryD thead td#tdD8').html(parseInt(GetNodeValue(dtH[0], "SumVIPNo_Cash")).toLocaleString('en-US'));
            $('#tbQueryD thead td#tdD9').html(parseInt(GetNodeValue(dtH[0], "SumVIPNo_RecS")).toLocaleString('en-US'));
            $('#tbQueryD thead td#tdD10').html(parseInt(GetNodeValue(dtH[0], "SumVIPNoPrice")).toLocaleString('en-US'));
            $('#tbQueryD thead td#tdD11').html(GetNodeValue(dtH[0], "SumVIPNoPercent"));
        }
    };

    let ClearQuery = function () {
        grdM.BindData(null)
        var heads = $('#tbQuery thead tr th#thead0');
        if ($('#rdoM').prop('checked')) {
            $(heads).html('月份');
        }
        else if ($('#rdoS').prop('checked')) {
            $(heads).html('店別');
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

    let InitComboItem = function (cboYear) {
        var y2 = new Date().getFullYear();
        for (i = 2020; i <= y2; i++) {
            cboYear.append($('<option>', { value: i, text: i }));
        }
        //for (i = 1; i <= 12; i++) {
        //    cboMonth.append($('<option>', { value: ('0' + i).substr(-2), text: i + '月' }));
        //}
    };

    let btRe_D_click = function (bt) {
        //Timerset();
        $('#modalD').modal('hide')
    };
//#region FormLoad
    let GetInitMSSD107 = function (data) {
        if (ReturnMsg(data, 0) != "GetInitmsDMOK") {
            DyAlert(ReturnMsg(data, 1));
        }
       else {
            var dtE = data.getElementsByTagName('dtE');
            if (dtE.length > 0) {
                $('#lblProgramName').html(GetNodeValue(dtE[0], "ChineseName"));
            }
            AssignVar();
            InitComboItem($('#cboYear'))
            $('#btQuery').click(function () { btQuery_click(this) });
            $('#btClear').click(function () { btClear_click(this) });
            $('#rdoM,#rdoS').change(function () { ClearQuery() });

            $('#btRe_D').click(function () { btRe_D_click(this) });
        }
    };
    
    let afterLoadPage = function () {
        var pData = {
            ProgramID: "MSSD107"
        }
        PostToWebApi({ url: "api/SystemSetup/GetInitmsDM", data: pData, success: GetInitMSSD107 });
    };
//#endregion

    if ($('#pgMSSD107').length == 0) {  
        AllPages = new LoadAllPages(ParentNode, "SystemSetup/MSSD107", ["MSSD107btns", "pgMSSD107Init", "pgMSSD107Add", "pgMSSD107Mod"], afterLoadPage);
    };
}