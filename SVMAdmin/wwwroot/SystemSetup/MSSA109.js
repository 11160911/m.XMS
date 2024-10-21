var PageMSSA109 = function (ParentNode) {

    let grdQuery;
    let grdQuery_STEP;
    let grdType;
    let grdType_STEP;
    let grdMonth;
    let grdMonth_STEP;

    let AssignVar = function () {

        grdQuery = new DynGrid(
            {
                table_lement: $('#tbQuery')[0],
                class_collection: ["tdCol1 text-center", "tdCol2 text-center", "tdCol3 text-center", "tdCol4 label-align", "tdCol5 label-align"],
                fields_info: [
                    { type: "Text", name: "SeqNo", style: "" },
                    { type: "Text", name: "GD_NO" },
                    { type: "Text", name: "GD_Name" },
                    { type: "TextAmt", name: "Qty" },
                    { type: "TextAmt", name: "Cash" }
                ],
                rows_per_page: 100,
                method_clickrow: click_PLU,
                afterBind: InitDetailButton,
                sortable: "N",
                step: "Y"
            }
        );

        grdQuery_STEP = new DynGrid(
            {
                table_lement: $('#tbQuery_STEP')[0],
                class_collection: ["tdCol1 text-center", "tdCol2 text-center", "tdCol3 text-center", "tdCol4 label-align", "tdCol5 label-align"],
                fields_info: [
                    { type: "Text", name: "SeqNo", style: "" },
                    { type: "Text", name: "GD_NO" },
                    { type: "Text", name: "GD_Name" },
                    { type: "TextAmt", name: "Qty" },
                    { type: "TextAmt", name: "Cash" }
                ],
                rows_per_page: 100,
                method_clickrow: click_PLU,
                afterBind: InitDetailButton,
                sortable: "N",
                step: "Y"
            }
        );

        grdType = new DynGrid(
            {
                table_lement: $('#tbType')[0],
                class_collection: ["tdCol1 text-center", "tdCol2 text-center", "tdCol3 label-align", "tdCol4 label-align"],
                fields_info: [
                    { type: "Text", name: "SeqNo", style: "" },
                    { type: "Text", name: "Name" },
                    { type: "TextAmt", name: "Qty" },
                    { type: "TextAmt", name: "Cash" }
                ],
                rows_per_page: 100,
                method_clickrow: click_PLU,
                afterBind: InitDetailButtonType,
                sortable: "N",
                step: "Y"
            }
        );

        grdType_STEP = new DynGrid(
            {
                table_lement: $('#tbType_STEP')[0],
                class_collection: ["tdCol1 text-center", "tdCol2 text-center", "tdCol3 label-align", "tdCol4 label-align"],
                fields_info: [
                    { type: "Text", name: "SeqNo", style: "" },
                    { type: "Text", name: "Name" },
                    { type: "TextAmt", name: "Qty" },
                    { type: "TextAmt", name: "Cash" }
                ],
                rows_per_page: 100,
                method_clickrow: click_PLU,
                afterBind: InitDetailButtonType,
                sortable: "N",
                step: "Y"
            }
        );

        grdMonth = new DynGrid(
            {
                table_lement: $('#tbMonth')[0],
                class_collection: ["tdCol1 text-center", "tdCol2 label-align", "tdCol3 label-align", "tdCol4"],
                fields_info: [
                    { type: "Text", name: "SeqNo", style: "" },
                    { type: "TextAmt", name: "Qty" },
                    { type: "TextAmt", name: "Cash" },
                    { type: "TextPercent", name: "Per" }
                ],
                rows_per_page: 100,
                method_clickrow: click_PLU,
                afterBind: InitDetailButtonMonth,
                sortable: "N",
                step: "Y"
            }
        );

        grdMonth_STEP = new DynGrid(
            {
                table_lement: $('#tbMonth_STEP')[0],
                class_collection: ["tdCol1 text-center", "tdCol2 label-align", "tdCol3 label-align", "tdCol4"],
                fields_info: [
                    { type: "Text", name: "SeqNo", style: "" },
                    { type: "TextAmt", name: "Qty" },
                    { type: "TextAmt", name: "Cash" },
                    { type: "TextPercent", name: "Per" }
                ],
                rows_per_page: 100,
                method_clickrow: click_PLU,
                afterBind: InitDetailButtonMonth,
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
    let InitDetailButtonType = function () {
        $('#tbType tbody tr td').click(function () { Step1_click(this) });

    };
    let InitDetailButtonMonth = function () {
        $('#tbMonth tbody tr td').click(function () { Step1_click(this) });

    };
    //第一層
    let Step1_click = function (bt) {
        //$('#tbQuery td').closest('tr').css('background-color', 'transparent');
        $(bt).closest('tr').click();
        $('.msg-valid').hide();
        var node;

        grdQuery_STEP.BindData(null)
        grdType_STEP.BindData(null)
        grdMonth_STEP.BindData(null)
        var sumtdQ = document.querySelectorAll('.QSum1');
        ; for (i = 0; i < sumtdQ.length; i++) {
            for (j = 0; j < sumtdQ[i].childElementCount; j++) {
                sumtdQ[i].children[j].innerHTML = "";
            }
        }

        //表頭顯示
        //$('#tbQuery td:contains(' + GetNodeValue(node, 'ID') + ')').closest('tr').css('background-color', '#DEEBF7');
        if ($('#rdoPLUQ').prop('checked') == true | $('#rdoPLUM').prop('checked') == true) {
            node = $(grdQuery.ActiveRowTR()).prop('Record')
            $('#rdoM').show();
            $('#rdoS').show();
            $('#rdoP').hide();
            $('#lblM').show();
            $('#lblS').show();
            $('#lblP').hide();
            $('#lblTypeData').show();
            //if ($('#rdoM').prop('checked') == false && $('#rdoS').prop('checked') == false)
                $('#rdoM').prop('checked', true);
        }
        else if ($('#rdoMONTH').prop('checked') == true) {
            node = $(grdMonth.ActiveRowTR()).prop('Record')
            $('#rdoM').hide();
            $('#rdoS').show();
            $('#rdoP').show();
            $('#lblM').hide();
            $('#lblS').show();
            $('#lblP').show();
            $('#lblTypeData').hide();
            //if ($('#rdoS').prop('checked') == false && $('#rdoP').prop('checked') == false)
                $('#rdoS').prop('checked', true);
        }
        else {
            node = $(grdType.ActiveRowTR()).prop('Record')
            $('#rdoM').show();
            $('#rdoS').hide();
            $('#rdoP').show();
            $('#lblM').show();
            $('#lblS').hide();
            $('#lblP').show();
            $('#lblTypeData').show();
            //if ($('#rdoM').prop('checked') == false && $('#rdoP').prop('checked') == false)
                $('#rdoM').prop('checked', true);
        }

        var strMonth = $('#cboMonth').val();
        if (strMonth != "") {
            $('#lblMonth').html($('#cboMonth').val() + '月');
            $('#rdoGroup1').hide();
            if ($('#rdoPLUQ').prop('checked') == true | $('#rdoPLUM').prop('checked') == true)
                $('#rdoS').prop('checked', true);
            else
                $('#rdoP').prop('checked', true);
        }
        else {
            $('#rdoGroup1').show();
            if ($('#rdoMONTH').prop('checked') == true)
                $('#lblMonth').html(GetNodeValue(node, 'SeqNo') + '月');
            else
                $('#lblMonth').html('');
        }
        $('#tbQuery_STEP').hide();
        $('#tbType_STEP').hide();
        $('#tbMonth_STEP').hide();

        $('#modal_Step1').modal('show');

        //表頭內容
        $('#lblYear').html($('#cboYear').val() + '年');

        $('#lblTypeID').html(GetNodeValue(node, 'Name'));
        if ($('#rdoPLUQ').prop('checked') == true | $('#rdoPLUM').prop('checked') == true) {
            $('#lblTypeName').html('商品：');
            $('#lblTypeID').html(GetNodeValue(node, 'GD_No') + ' ' + GetNodeValue(node, 'GD_Name'));
        }
        else if ($('#rdoShop').prop('checked') == true) {
            $('#lblTypeName').html('店別：');
        }
        else if ($('#rdoDept').prop('checked') == true) {
            $('#lblTypeName').html('部門：');
        }
        else if ($('#rdoBGNO').prop('checked') == true) {
            $('#lblTypeName').html('大類：');
        }
        else if ($('#rdoMDNO').prop('checked') == true) {
            $('#lblTypeName').html('中類：');
        }
        else if ($('#rdoSMNO').prop('checked') == true) {
            $('#lblTypeName').html('小類：');
        }
        else if ($('#rdoBNID').prop('checked') == true) {
            $('#lblTypeName').html('品牌：');
        }
        else if ($('#rdoSERIES').prop('checked') == true) {
            $('#lblTypeName').html('系列：');
        }

        setTimeout(function () {
            Query_Step1_click();
        }, 500);

    };
    let Query_Step1_click = function () {
        ShowLoading();

        var Year = $('#cboYear').val()
        var Month = $('#lblMonth').html()
        var Flag = ""
        var SubFlag = ""
        var SubType = $('#lblTypeID').html()

        //GRID顯示
        if ($('#rdoM').prop('checked') == true) {
            $('#tbQuery_STEP').hide();
            $('#tbType_STEP').hide();
            $('#tbMonth_STEP').show();
            $('#lblTop_STEP').hide();
        }
        else if ($('#rdoS').prop('checked') == true) {
            $('#tbQuery_STEP').hide();
            $('#tbType_STEP').show();
            $('#tbMonth_STEP').hide();
            $('#lblTop_STEP').hide();
        }
        else if ($('#rdoP').prop('checked') == true) {
            $('#tbQuery_STEP').show();
            $('#tbType_STEP').hide();
            $('#tbMonth_STEP').hide();
            $('#lblTop_STEP').show();
        }
                       
        //商品數量
        if ($('#rdoPLUQ').prop('checked') == true) {
            Flag = "PQ";
        }
        //商品金額
        if ($('#rdoPLUM').prop('checked') == true) {
            Flag = "PM";
        }
        //店別
        else if ($('#rdoShop').prop('checked') == true) {
            Flag = "W";
        }
        //部門
        else if ($('#rdoDept').prop('checked') == true) {
            Flag = "D";
        }
        //大類
        if ($('#rdoBGNO').prop('checked') == true) {
            Flag = "L";
        }
        //中類
        else if ($('#rdoMDNO').prop('checked') == true) {
            Flag = "M";
        }
        //小類
        else if ($('#rdoSMNO').prop('checked') == true) {
            Flag = "S";
        }
        //品牌
        if ($('#rdoBNID').prop('checked') == true) {
            Flag = "G";
        }
        //系列
        else if ($('#rdoSERIES').prop('checked') == true) {
            Flag = "E";
        }
        //月份
        else if ($('#rdoMONTH').prop('checked') == true) {
            Flag = "MM";
        }
        //子條件
        //月份
        if ($('#rdoM').prop('checked') == true) {
            SubFlag = $('#rdoM').val();
            SubType = SubType.split(' ')[0];
        }
        //店別
        else if ($('#rdoS').prop('checked') == true) {
            SubFlag = $('#rdoS').val();
            SubType = SubType.split(' ')[0];
        }
        //商品
        else if ($('#rdoP').prop('checked') == true) {
            SubFlag = $('#rdoP').val();
            SubType = SubType.split(' ')[0];
        }

        setTimeout(function () {
            var pData = {
                Year: Year,
                Month: Month,
                Flag: Flag,
                SubFlag: SubFlag,
                SubType: SubType
            }
            PostToWebApi({ url: "api/SystemSetup/MSSA109Query_Step1", data: pData, success: afterQuery_Step1 });
        }, 1000);
    };

    let afterQuery_Step1 = function (data) {
        CloseLoading();
        if (ReturnMsg(data, 0) != "MSSA109Query_Step1OK") {
            DyAlert(ReturnMsg(data, 1));
        }
        else {
            $('#modal_Step1').css("padding-right", "0px");

            var dtH = data.getElementsByTagName('dtH');

            var dtE = data.getElementsByTagName('dtE');
            if ($('#rdoM').prop('checked') == true) {
                grdMonth_STEP.BindData(dtE);
                $('#tbMonth_STEP thead td#td1M').html(parseInt(GetNodeValue(dtH[0], "SumQty")).toLocaleString('en-US'));
                $('#tbMonth_STEP thead td#td2M').html(parseInt(GetNodeValue(dtH[0], "SumCash")).toLocaleString('en-US'));
            }
            else if ($('#rdoP').prop('checked') == true) {
                grdQuery_STEP.BindData(dtE);
                $('#tbQuery_STEP thead td#td1').html(parseInt(GetNodeValue(dtH[0], "SumQty")).toLocaleString('en-US'));
                $('#tbQuery_STEP thead td#td2').html(parseInt(GetNodeValue(dtH[0], "SumCash")).toLocaleString('en-US'));
            }
            else if ($('#rdoS').prop('checked') == true) {
                grdType_STEP.BindData(dtE);
                $('#tbType_STEP thead td#td1S').html(parseInt(GetNodeValue(dtH[0], "SumQty")).toLocaleString('en-US'));
                $('#tbType_STEP thead td#td2S').html(parseInt(GetNodeValue(dtH[0], "SumCash")).toLocaleString('en-US'));
            }
        }
    };

    //查詢
    let btQuery_click = function (bt) {
        //Timerset();
        $('#btQuery').prop('disabled', true)
        ClearQuery();
        //年
        if ($('#cboYear').val() == "") {
            DyAlert("年度需輸入!", function () { $('#btQuery').prop('disabled', false); })
            return
        }

        ShowLoading();

        var Flag = ""
        //商品數量
        if ($('#rdoPLUQ').prop('checked') == true) {
            Flag = "PQ";
        }
        //商品金額
        if ($('#rdoPLUM').prop('checked') == true) {
            Flag = "PM";
        }
        //店別
        else if ($('#rdoShop').prop('checked') == true) {
            Flag = "W";
        }
        //部門
        else if ($('#rdoDept').prop('checked') == true) {
            Flag = "D";
        }
        //大類
        if ($('#rdoBGNO').prop('checked') == true) {
            Flag = "L";
        }
        //中類
        else if ($('#rdoMDNO').prop('checked') == true) {
            Flag = "M";
        }
        //小類
        else if ($('#rdoSMNO').prop('checked') == true) {
            Flag = "S";
        }
        //品牌
        if ($('#rdoBNID').prop('checked') == true) {
            Flag = "G";
        }
        //系列
        else if ($('#rdoSERIES').prop('checked') == true) {
            Flag = "E";
        }
        //月份
        else if ($('#rdoMONTH').prop('checked') == true) {
            Flag = "MM";
        }
        setTimeout(function () {
            var pData = {
                Year: $('#cboYear').val(),
                Month: $('#cboMonth').val(),
                Flag: Flag
            }
            PostToWebApi({ url: "api/SystemSetup/MSSA109Query", data: pData, success: afterMSSA109Query });
        }, 1000);
    };

    let afterMSSA109Query = function (data) {
        CloseLoading();
        if (ReturnMsg(data, 0) != "MSSA109QueryOK") {
            DyAlert(ReturnMsg(data, 1), function () { $('#btQuery').prop('disabled', false); });
        }
        else {
            $('#btQuery').prop('disabled', false);
            var dtE = data.getElementsByTagName('dtE');
            if (dtE.length == 0) {
                DyAlert("無符合資料!");
                //$(".modal-backdrop").remove();
                return;
            }

            var dtH = data.getElementsByTagName('dtH');
            //商品數量,商品金額
            if ($('#rdoPLUQ').prop('checked') == true | $('#rdoPLUM').prop('checked') == true) {
                grdQuery.BindData(dtE);
                $('#tbQuery thead td#td1').html(parseInt(GetNodeValue(dtH[0], "SumQty")).toLocaleString('en-US'));
                $('#tbQuery thead td#td2').html(parseInt(GetNodeValue(dtH[0], "SumCash")).toLocaleString('en-US'));
            }
            //月份
            else if ($('#rdoMONTH').prop('checked') == true) {
                grdMonth.BindData(dtE);
                $('#tbMonth thead td#td1M').html(parseInt(GetNodeValue(dtH[0], "SumQty")).toLocaleString('en-US'));
                $('#tbMonth thead td#td2M').html(parseInt(GetNodeValue(dtH[0], "SumCash")).toLocaleString('en-US'));
            }
            //店,部門,大,中,小類,系列
            else {
                grdType.BindData(dtE);
                $('#tbType thead td#td1S').html(parseInt(GetNodeValue(dtH[0], "SumQty")).toLocaleString('en-US'));
                $('#tbType thead td#td2S').html(parseInt(GetNodeValue(dtH[0], "SumCash")).toLocaleString('en-US'));
            }
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
    let ResetGrid = function () {
        //商品數量,商品金額
        if ($('#rdoPLUQ').prop('checked') == true | $('#rdoPLUM').prop('checked') == true) {
            $('#tbQuery').show();
            $('#tbType').hide();
            $('#tbMonth').hide();
            $('#lblTop').show();
        }
        //月份
        else if ($('#rdoMONTH').prop('checked') == true) {
            $('#tbQuery').hide();
            $('#tbType').hide();
            $('#tbMonth').show();
            $('#lblTop').hide();
        }
        //店,部門,大,中,小類,系列
        else {
            $('#tbQuery').hide();
            $('#tbType').show();
            $('#tbMonth').hide();
            $('#lblTop').hide();
        }

        var heads = $('#tbType thead tr th#thead2S');
        if ($('#rdoShop').prop('checked')) {
            $(heads).html('店別');
        }
        //else if ($('#rdoMONTH').prop('checked')) {
        //    $(heads).html('月份');
        //}
        else if ($('#rdoDept').prop('checked')) {
            $(heads).html('部門');
        }
        else if ($('#rdoBGNO').prop('checked')) {
            $(heads).html('大類');
        }
        else if ($('#rdoMDNO').prop('checked')) {
            $(heads).html('中類');
        }
        else if ($('#rdoSMNO').prop('checked')) {
            $(heads).html('小類');
        }
        else if ($('#rdoBNID').prop('checked')) {
            $(heads).html('品牌');
        }
        else if ($('#rdoSERIES').prop('checked')) {
            $(heads).html('系列');
        }
    }
    let ClearQueryInit = function () {
        $('#cboMonth').val('');
        $('#rdoMONTH').show();
        $('#lblMONTH').show();
        ClearQuery();
    }
    let ClearQuery = function () {
        grdQuery.BindData(null)
        grdType.BindData(null)
        grdMonth.BindData(null)

        ResetGrid()

        var sumtdQ = document.querySelectorAll('.QSum');
        ; for (i = 0; i < sumtdQ.length; i++) {
            for (j = 0; j < sumtdQ[i].childElementCount; j++) {
                sumtdQ[i].children[j].innerHTML = "";
            }
        }
    };
    let HideMonth = function () {
        if ($('#cboMonth').val() == "") {
            $('#rdoMONTH').show();
            $('#lblMONTH').show();
        }
        else {
            $('#rdoMONTH').hide();
            $('#lblMONTH').hide();
            if ($('#rdoMONTH').prop('checked') == true)
                $('#rdoPLUQ').prop('checked', true);
        }
    }

    let InitComboItem = function (cboYear, cboMonth) {
        var y2 = new Date().getFullYear();
        for (i = y2; i >= 2020; i--) {
            cboYear.append($('<option>', { value: i, text: i }));
        }
        $("#cboYear").val(y2);

        for (i = 1; i <= 12; i++) {
            cboMonth.append($('<option>', { value: ('0' + i).substr(-2), text: i + '月' }));
        }

    };

    //#region FormLoad
    let GetInitMSSA109 = function (data) {
        if (ReturnMsg(data, 0) != "GetInitmsDMOK") {
            DyAlert(ReturnMsg(data, 1));
        }
        else {
            var dtE = data.getElementsByTagName('dtE');
            if (dtE.length > 0) {
                $('#lblProgramName').html(GetNodeValue(dtE[0], "ChineseName"));
            }
            InitComboItem($("#cboYear"), $('#cboMonth'));    //下拉選單
            $('#cboMonth').val(GetNodeValue(dtE[0], "SysDate").substr(5, 2))
            HideMonth();

            AssignVar();
            $('#tbQuery').show();
            $('#tbType').hide();
            $('#tbMonth').hide();

            $('#btQuery').click(function () { btQuery_click(this) });
            $('#btClear').click(function () { ClearQueryInit() });
            $('#btExit_Step1').click(function () { btExit_Step1_click(this) });
            $('#btExit_Step2').click(function () { btExit_Step2_click(this) });
            $('#rdoPLUQ,#rdoShop,#rdoDept,#rdoBGNO,#rdoMDNO,#rdoPLUM,#rdoSMNO,#rdoBNID,#rdoSERIES,#rdoMONTH').change(function () { btQuery_click() });
            $('#rdoM,#rdoS,#rdoP').change(function () { Query_Step1_click(this) });
            $('#cboYear').change(function () { ClearQuery() });
            $('#cboMonth').change(function () { HideMonth() });
            $('#cboMonth').change(function () { ClearQuery() });
            btQuery_click();
        }
    };

    let afterLoadPage = function () {
        var pData = {
            ProgramID: "MSSA109"
        }
        PostToWebApi({ url: "api/SystemSetup/GetInitmsDM", data: pData, success: GetInitMSSA109 });
    };


    if ($('#pgMSSA109').length == 0) {
        AllPages = new LoadAllPages(ParentNode, "SystemSetup/MSSA109", ["pgMSSA109Init"], afterLoadPage);
    };
    //#endregion
}