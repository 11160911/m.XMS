﻿var PageMSSA105 = function (ParentNode) {

    let grdM;

    let AssignVar = function () {

        grdM = new DynGrid(
            {
                table_lement: $('#tbQuery')[0],
                class_collection: ["tdCol1 text-center", "tdCol2 label-align", "tdCol3 label-align", "tdCol4"],
                fields_info: [
                    { type: "Text", name: "ID", style: "" },
                    { type: "TextAmt", name: "Cash1" },
                    { type: "TextAmt", name: "Cash2" },
                    { type: "TextPercent", name: "Per" }
                ],
                //rows_per_page: 10,
                method_clickrow: click_PLU,
                afterBind: InitDetailButton,
                sortable: "N",
                step: "Y"
            }
        );

        grdSTEP1 = new DynGrid(
            {
                table_lement: $('#tbQuerySTEP1')[0],
                class_collection: ["tdCol1 text-center", "tdCol2 label-align", "tdCol3 label-align", "tdCol4"],
                fields_info: [
                    { type: "Text", name: "ID", style: "" },
                    { type: "TextAmt", name: "Cash1" },
                    { type: "TextAmt", name: "Cash2" },
                    { type: "TextPercent", name: "Per" }
                ],
                //rows_per_page: 10,
                method_clickrow: click_PLU,
                afterBind: InitDetail2Button,
                sortable: "N",
                step: "Y"
            }
        );

        grdSTEP2 = new DynGrid(
            {
                table_lement: $('#tbQuerySTEP2')[0],
                class_collection: ["tdCol1 text-center", "tdCol2 label-align", "tdCol3 label-align", "tdCol4"],
                fields_info: [
                    { type: "Text", name: "ID", style: "" },
                    { type: "TextAmt", name: "Cash1" },
                    { type: "TextAmt", name: "Cash2" },
                    { type: "TextPercent", name: "Per" }
                ],
                //rows_per_page: 10,
                sortable: "N",
                step: "Y"
            }
        );

        return;
    };

    let click_PLU = function (tr) {

    };

    let InitDetailButton = function () {
        $('#tbQuery tbody tr td').click(function () { Step1_click(this) });

    };
    let InitDetail2Button = function () {
        if ($('#rdoB').prop('checked') == true) //區課
            $('#tbQuerySTEP1 tbody tr td').click(function () { Step2_click(this) });

    };
    //第二層
    let Step2_click = function (bt) {
        //$('#tbQuerySTEP1 td').closest('tr').css('background-color', 'transparent');
        $(bt).closest('tr').click();
        $('.msg-valid').hide();
        var node = $(grdSTEP1.ActiveRowTR()).prop('Record');
        //$('#tbQuerySTEP1 td:contains(' + GetNodeValue(node, 'ID') + ')').closest('tr').css('background-color', '#DEEBF7');
        $('#modal_Step2').modal('show');
        $('#lblYear2').html($('#cboYear').val() + '年');
        $('#lblType').html($('#lblMonth').html());
        $('#lblShop').html(GetNodeValue(node, 'ID'));
        setTimeout(function () {
            Query_Step2_click();
        }, 500);

    };
    let Query_Step2_click = function () {
        ShowLoading();

        var Year = $('#lblYear2').html()
        var Type = $('#lblType').html()
        var Shop = $('#lblShop').html()
        var Flag = ""
        if ($('#rdoB').prop('checked') == true) {
            Flag = "B";
            Type = Type.split('-')[0];
            Shop = Shop.split('-')[0];
        }

        setTimeout(function () {
            var pData = {
                Year: Year,
                Shop: Shop,
                Type: Type
            }
            PostToWebApi({ url: "api/SystemSetup/MSSA105Query_Step2", data: pData, success: afterQuery_Step2 });
        }, 1000);
    };
    let afterQuery_Step2 = function (data) {
        CloseLoading();
        if (ReturnMsg(data, 0) != "MSSA105Query_Step2OK") {
            DyAlert(ReturnMsg(data, 1));
        }
        else {
            $('#modal_Step2').css("padding-right", "0px");
            var Year = $('#cboYear').val();
            var YearBef = Year - 1;
            $('#tbQuerySTEP2 #thead2').html(YearBef + '年度業績');
            $('#tbQuerySTEP2 #thead3').html(Year + '年度業績');

            var dtH = data.getElementsByTagName('dtH');
            $('#tbQuerySTEP2 thead td#td1').html(parseInt(GetNodeValue(dtH[0], "SumCash1")).toLocaleString('en-US'));
            $('#tbQuerySTEP2 thead td#td2').html(parseInt(GetNodeValue(dtH[0], "SumCash2")).toLocaleString('en-US'));
            $('#tbQuerySTEP2 thead td#td3').html(GetNodeValue(dtH[0], "SumPer"));

            var dtE = data.getElementsByTagName('dtE');
            grdSTEP2.BindData(dtE);

        }
    };
    //第一層
    let Step1_click = function (bt) {
        //$('#tbQuery td').closest('tr').css('background-color', 'transparent');
        $(bt).closest('tr').click();
        $('.msg-valid').hide();
        var node = $(grdM.ActiveRowTR()).prop('Record');
        //$('#tbQuery td:contains(' + GetNodeValue(node, 'ID') + ')').closest('tr').css('background-color', '#DEEBF7');
        $('#modal_Step1').modal('show');
        $('#lblYear').html($('#cboYear').val() + '年');
        $('#lblMonth').html(GetNodeValue(node, 'ID'));
        setTimeout(function () {
            Query_Step1_click();
        }, 500);

    };
    let Query_Step1_click = function () {
        ShowLoading();

        var Year = $('#lblYear').html()
        var Month = $('#lblMonth').html()
        var Flag = ""
        //月份
        if ($('#rdoS').prop('checked') == true) {
            Flag = "S";
            $('#lblTypeName').html('月份：');
            $('#tbQuerySTEP1 thead tr th#thead1').html('店別');
            $('#tbQuerySTEP1 thead td#td0').html('店總業績');
        }
        //店別
        else if ($('#rdoD').prop('checked') == true) {
            Flag = "D";
            Month = Month.split('-')[0];
            $('#lblTypeName').html('店別：');
            $('#tbQuerySTEP1 thead tr th#thead1').html('月份');
            $('#tbQuerySTEP1 thead td#td0').html('月總業績');
        }
        //區課
        else if ($('#rdoB').prop('checked') == true) {
            Flag = "B";
            Month = Month.split('-')[0];
            $('#lblTypeName').html('區課：');
            $('#tbQuerySTEP1 thead tr th#thead1').html('店別');
            $('#tbQuerySTEP1 thead td#td0').html('店總業績');
        }

        setTimeout(function () {
            var pData = {
                Year: Year,
                Month: Month,
                Flag: Flag
            }
            PostToWebApi({ url: "api/SystemSetup/MSSA105Query_Step1", data: pData, success: afterQuery_Step1 });
        }, 1000);
    };

    let afterQuery_Step1 = function (data) {
        CloseLoading();
        if (ReturnMsg(data, 0) != "MSSA105Query_Step1OK") {
            DyAlert(ReturnMsg(data, 1));
        }
        else {
            $('#modal_Step1').css("padding-right", "0px");
            var Year = $('#cboYear').val();
            var YearBef = Year - 1;
            $('#tbQuerySTEP1 #thead2').html(YearBef + '年度業績');
            $('#tbQuerySTEP1 #thead3').html(Year + '年度業績');

            var dtH = data.getElementsByTagName('dtH');
            $('#tbQuerySTEP1 thead td#td1').html(parseInt(GetNodeValue(dtH[0], "SumCash1")).toLocaleString('en-US'));
            $('#tbQuerySTEP1 thead td#td2').html(parseInt(GetNodeValue(dtH[0], "SumCash2")).toLocaleString('en-US'));
            $('#tbQuerySTEP1 thead td#td3').html(GetNodeValue(dtH[0], "SumPer"));

            var dtE = data.getElementsByTagName('dtE');
            grdSTEP1.BindData(dtE);


        }
    };

    //查詢
    let btQuery_click = function (bt) {
        //Timerset();
        $('#btQuery').prop('disabled', true)

        //年
        if ($('#cboYear').val() == "") {
            DyAlert("年度需輸入!", function () { $('#btQuery').prop('disabled', false); })
            return
        }

        ShowLoading();

        var Flag = ""
        //月份
        if ($('#rdoS').prop('checked') == true) {
            Flag = "S";
        }
        //店別
        else if ($('#rdoD').prop('checked') == true) {
            Flag = "D";
        }
        //區課
        else if ($('#rdoB').prop('checked') == true) {
            Flag = "B";
        }

        setTimeout(function () {
            var pData = {
                Year: $('#cboYear').val(),
                Flag: Flag
            }
            PostToWebApi({ url: "api/SystemSetup/MSSA105Query", data: pData, success: afterMSSA105Query });
        }, 1000);
    };

    let afterMSSA105Query = function (data) {
        CloseLoading();
        if (ReturnMsg(data, 0) != "MSSA105QueryOK") {
            DyAlert(ReturnMsg(data, 1), function () { $('#btQuery').prop('disabled', false); });
        }
        else {
            $('#btQuery').prop('disabled', false);
            var dtE = data.getElementsByTagName('dtE');
            grdM.BindData(dtE);

            var heads = $('#tbQuery thead tr th#thead1');
            if ($('#rdoS').prop('checked')) {
                $(heads).html('月份');
            }
            else if ($('#rdoD').prop('checked')) {
                $(heads).html('店別');
            }
            else if ($('#rdoB').prop('checked')) {
                $(heads).html('區課');
            }

            if (dtE.length == 0) {
                DyAlert("無符合資料!");
                //$(".modal-backdrop").remove();
                $('#tbQuery thead td#td1').html('');
                $('#tbQuery thead td#td2').html('');
                $('#tbQuery thead td#td3').html('');
                return;
            }
            var Year = $('#cboYear').val();
            var YearBef = Year - 1;
            $('#tbQuery #thead2').html(YearBef + '年度業績');
            $('#tbQuery #thead3').html(Year + '年度業績');

            var dtH = data.getElementsByTagName('dtH');
            $('#tbQuery thead td#td1').html(parseInt(GetNodeValue(dtH[0], "SumCash1")).toLocaleString('en-US'));
            $('#tbQuery thead td#td2').html(parseInt(GetNodeValue(dtH[0], "SumCash2")).toLocaleString('en-US'));
            $('#tbQuery thead td#td3').html(GetNodeValue(dtH[0], "SumPer"));
        }
    };
    let btExit_Step1_click = function (bt) {
        $('#modal_Step1').modal('hide');
        $('#tbQuerySTEP1 tbody').find('tr').remove();
        $('#tbQuerySTEP1 thead td#td1').html('');
        $('#tbQuerySTEP1 thead td#td2').html('');
        $('#tbQuerySTEP1 thead td#td3').html('');
        //$('#pgMSSD101Init').show();
        //$('#pgMSSD101_PS_STEP1').hide();
    };
    let btExit_Step2_click = function (bt) {
        $('#modal_Step2').modal('hide');
        $('#tbQuerySTEP2 tbody').find('tr').remove();
        $('#tbQuerySTEP2 thead td#td1').html('');
        $('#tbQuerySTEP2 thead td#td2').html('');
        $('#tbQuerySTEP2 thead td#td3').html('');

        //$('#pgMSSD101Init').show();
        //$('#pgMSSD101_PS_STEP1').hide();
    };

    let ClearQuery = function () {
        grdM.BindData(null)
        var heads = $('#tbQuery thead tr th#thead1');
        if ($('#rdoS').prop('checked')) {
            $(heads).html('月份');
        }
        else if ($('#rdoD').prop('checked')) {
            $(heads).html('店別');
        }
        else if ($('#rdoB').prop('checked')) {
            $(heads).html('區課');
        }
        var sumtdQ = document.querySelector('.QSum');
        for (i = 0; i < sumtdQ.childElementCount; i++) {
            if (i == 0) {
                sumtdQ.children[i].innerHTML = "總業績";
            }
            else {
                sumtdQ.children[i].innerHTML = "";
            }
        }
    };

    let InitComboItem = function (cboYear) {
        var y2 = new Date().getFullYear();
        for (i = y2; i >= 2020; i--) {
            cboYear.append($('<option>', { value: i, text: i }));
        }
        $("#cboYear").val(y2);
    };

    //#region FormLoad
    let GetInitMSSA105 = function (data) {
        if (ReturnMsg(data, 0) != "GetInitmsDMOK") {
            DyAlert(ReturnMsg(data, 1));
        }
        else {
            var dtE = data.getElementsByTagName('dtE');
            if (dtE.length > 0) {
                $('#lblProgramName').html(GetNodeValue(dtE[0], "ChineseName"));
            }
            InitComboItem($("#cboYear"));    //下拉選單

            AssignVar();

            $('#btQuery').click(function () { btQuery_click(this) });
            $('#btExit_Step1').click(function () { btExit_Step1_click(this) });
            $('#btExit_Step2').click(function () { btExit_Step2_click(this) });
            $('#rdoS,#rdoD,#rdoB').change(function () { btQuery_click(this) });
            $('#cboYear').change(function () { ClearQuery() });
            btQuery_click();
        }
    };

    let afterLoadPage = function () {
        var pData = {
            ProgramID: "MSSA105"
        }
        PostToWebApi({ url: "api/SystemSetup/GetInitmsDM", data: pData, success: GetInitMSSA105 });
    };


    if ($('#pgMSSA105').length == 0) {
        AllPages = new LoadAllPages(ParentNode, "SystemSetup/MSSA105", ["pgMSSA105Init"], afterLoadPage);
    };
    //#endregion
}