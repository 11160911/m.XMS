var PageMSSA103 = function (ParentNode) {

    let grdM;
    let grdD1;
    let grdDD1;
    let grdD2;
    let grdLookUp_ShopNo;

    let chkShopNo = "";


    let AssignVar = function () {
        grdM = new DynGrid(
            {
                table_lement: $('#tbQuery')[0],
                class_collection: ["tdCol1 text-center", "tdCol2 label-align", "tdCol3 label-align", "tdCol4 label-align", "tdCol5 label-align", "tdCol6 text-center"],
                fields_info: [
                    { type: "Text", name: "ID", style: "" },
                    { type: "TextAmt", name: "Qty1"},
                    { type: "TextAmt", name: "Cash1" },
                    { type: "TextAmt", name: "Qty2" },
                    { type: "TextAmt", name: "Cash2" },
                    { type: "TextPercent", name: "Per" }
                ],
                //rows_per_page: 10,
                method_clickrow: click_PLU,
                afterBind: InitModifyDeleteButton,
                sortable: "Y",
                step: "Y"
            }
        );
        grdD1 = new DynGrid(
            {
                table_lement: $('#tbQueryD1')[0],
                class_collection: ["tdCol1 text-center", "tdCol2 label-align", "tdCol3 label-align", "tdCol4 label-align", "tdCol5 label-align", "tdCol6 text-center"],
                fields_info: [
                    { type: "Text", name: "ID", style: "" },
                    { type: "TextAmt", name: "Qty1" },
                    { type: "TextAmt", name: "Cash1" },
                    { type: "TextAmt", name: "Qty2" },
                    { type: "TextAmt", name: "Cash2" },
                    { type: "TextPercent", name: "Per" }
                ],
                //rows_per_page: 10,
                method_clickrow: click_PLU,
                afterBind: gridclickD1,
                sortable: "Y",
                step: "Y"
            }
        );
        grdDD1 = new DynGrid(
            {
                table_lement: $('#tbQueryDD1')[0],
                class_collection: ["tdCol1 text-center", "tdCol2 label-align", "tdCol3 label-align", "tdCol4 label-align", "tdCol5 label-align", "tdCol6 text-center"],
                fields_info: [
                    { type: "Text", name: "ID", style: "" },
                    { type: "TextAmt", name: "Qty1" },
                    { type: "TextAmt", name: "Cash1" },
                    { type: "TextAmt", name: "Qty2" },
                    { type: "TextAmt", name: "Cash2" },
                    { type: "TextPercent", name: "Per" }
                ],
                //rows_per_page: 10,
                method_clickrow: click_PLU,
                sortable: "Y"
            }
        );
        grdD2 = new DynGrid(
            {
                table_lement: $('#tbQueryD2')[0],
                class_collection: ["tdCol1 text-center", "tdCol2 label-align", "tdCol3 label-align", "tdCol4 label-align", "tdCol5 label-align", "tdCol6 text-center"],
                fields_info: [
                    { type: "Text", name: "ID", style: "" },
                    { type: "TextAmt", name: "Qty1" },
                    { type: "TextAmt", name: "Cash1" },
                    { type: "TextAmt", name: "Qty2" },
                    { type: "TextAmt", name: "Cash2" },
                    { type: "TextPercent", name: "Per" }
                ],
                //rows_per_page: 10,
                method_clickrow: click_PLU,
                afterBind: gridclickD1,
                sortable: "Y"
            }
        );

        grdLookUp_ShopNo = new DynGrid(
            {
                table_lement: $('#tbLookup_ShopNo')[0],
                class_collection: ["tdCol1 text-center", "tdCol2", "tdCol3"],
                fields_info: [
                    { type: "checkbox", name: "chkset", style: "width:16px;height:16px" },
                    { type: "Text", name: "ST_ID", style: "" },
                    { type: "Text", name: "ST_SName", style: "" }
                ],
                //rows_per_page: 10,
                method_clickrow: click_PLU,
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

    let gridclickD1 = function () {
        var heads = $('#tbQueryD1 thead tr th#thead1_D1');
        if ($(heads).html() == "部門") {
            $('#tbQueryD1 tbody tr td').click(function () { StepD1_click(this) });
        }
    }

    let Step1_click = function (bt) {
        $(bt).closest('tr').click();
        $('.msg-valid').hide();
        var node = $(grdM.ActiveRowTR()).prop('Record');
        var heads = $('#tbQuery thead tr th#thead1');
        if ($(heads).html() == "店櫃") {
            $('#lblOpenDate2_D1').html(GetNodeValue(node, 'OpenDateS2') + '~' + GetNodeValue(node, 'OpenDateE2'))
            $('#lblOpenDate1_D1').html(GetNodeValue(node, 'OpenDateS1') + '~' + GetNodeValue(node, 'OpenDateE1'))
            $('#lblShopNo_D1').html(GetNodeValue(node, 'id'))
            $('#rdoD_D1').prop('checked', true);
            $('#modalD1').modal('show');
            setTimeout(function () {
                QueryD1();
            }, 500);
        }
        else if ($(heads).html() == "部門") {
            $('#lblOpenDate2_D2').html(GetNodeValue(node, 'OpenDateS2') + '~' + GetNodeValue(node, 'OpenDateE2'))
            $('#lblOpenDate1_D2').html(GetNodeValue(node, 'OpenDateS1') + '~' + GetNodeValue(node, 'OpenDateE1'))
            $('#lblShopNo_D2').html($('#lblShopNoName').html() + ' ' + $('#lblShopNoCnt').html())
            $('#lblDept_D2').html(GetNodeValue(node, 'id'))
            $('#modalD2').modal('show');
            setTimeout(function () {
                QueryD2();
            }, 500);
        }
    };

    let QueryD1 = function () {
        ShowLoading();
        var Flag = "";
        var ID = $('#lblShopNo_D1').html().split('-')[0];
        var OpenDateS1 = $('#lblOpenDate1_D1').html().split('~')[0];
        var OpenDateE1 = $('#lblOpenDate1_D1').html().split('~')[1];
        var OpenDateS2 = $('#lblOpenDate2_D1').html().split('~')[0];
        var OpenDateE2 = $('#lblOpenDate2_D1').html().split('~')[1];
        var heads = $('#tbQueryD1 thead tr th#thead1_D1');

        if ($('#rdoD_D1').prop('checked') == true) {
            $(heads).html('部門');
            Flag = "D";
        }
        else if ($('#rdoB_D1').prop('checked') == true) {
            $(heads).html('大類');
            Flag = "B";
        }

        var pData = {
            OpenDateS1: OpenDateS1,
            OpenDateE1: OpenDateE1,
            OpenDateS2: OpenDateS2,
            OpenDateE2: OpenDateE2,
            ID: ID,
            Flag: Flag
        }
        PostToWebApi({ url: "api/SystemSetup/MSSA103QueryD1", data: pData, success: afterMSSA103QueryD1 });
    };

    let afterMSSA103QueryD1 = function (data) {
        CloseLoading();
        if (ReturnMsg(data, 0) != "MSSA103QueryD1OK") {
            DyAlert(ReturnMsg(data, 1));
        }
        else {
            var dtE = data.getElementsByTagName('dtE');
            grdD1.BindData(dtE);

            if (dtE.length == 0) {
                DyAlert("無符合資料!");
                //$(".modal-backdrop").remove();
                var sumtdQD = document.querySelector('.QSumD_D1');
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
            $('#tbQueryD1 thead td#td1_D1').html(parseInt(GetNodeValue(dtH[0], "SumQty1")).toLocaleString('en-US'));
            $('#tbQueryD1 thead td#td2_D1').html(parseInt(GetNodeValue(dtH[0], "SumCash1")).toLocaleString('en-US'));
            $('#tbQueryD1 thead td#td3_D1').html(parseInt(GetNodeValue(dtH[0], "SumQty2")).toLocaleString('en-US'));
            $('#tbQueryD1 thead td#td4_D1').html(parseInt(GetNodeValue(dtH[0], "SumCash2")).toLocaleString('en-US'));
            $('#tbQueryD1 thead td#td5_D1').html(GetNodeValue(dtH[0], "SumPer"));
        }
    };

    let StepD1_click = function (bt) {
        $(bt).closest('tr').click();
        $('.msg-valid').hide();
        var node = $(grdD1.ActiveRowTR()).prop('Record');
        $('#lblOpenDate2_DD1').html($('#lblOpenDate2_D1').html())
        $('#lblOpenDate1_DD1').html($('#lblOpenDate1_D1').html())
        $('#lblShopNo_DD1').html($('#lblShopNo_D1').html())
        $('#lblDept_DD1').html(GetNodeValue(node, 'ID'))
        $('#modalDD1').modal('show');
        setTimeout(function () {
            QueryDD1();
        }, 500);
    };

    let QueryDD1 = function () {
        ShowLoading();
        var ShopNo = $('#lblShopNo_DD1').html().split('-')[0];
        var Dept = $('#lblDept_DD1').html().split('-')[0];
        var OpenDateS1 = $('#lblOpenDate1_DD1').html().split('~')[0];
        var OpenDateE1 = $('#lblOpenDate1_DD1').html().split('~')[1];
        var OpenDateS2 = $('#lblOpenDate2_DD1').html().split('~')[0];
        var OpenDateE2 = $('#lblOpenDate2_DD1').html().split('~')[1];

        var pData = {
            OpenDateS1: OpenDateS1,
            OpenDateE1: OpenDateE1,
            OpenDateS2: OpenDateS2,
            OpenDateE2: OpenDateE2,
            ShopNo: ShopNo,
            Dept: Dept
        }
        PostToWebApi({ url: "api/SystemSetup/MSSA103QueryDD1", data: pData, success: afterMSSA103QueryDD1 });
    };

    let afterMSSA103QueryDD1 = function (data) {
        CloseLoading();
        if (ReturnMsg(data, 0) != "MSSA103QueryDD1OK") {
            DyAlert(ReturnMsg(data, 1));
        }
        else {
            var dtE = data.getElementsByTagName('dtE');
            grdDD1.BindData(dtE);

            if (dtE.length == 0) {
                DyAlert("無符合資料!");
                //$(".modal-backdrop").remove();
                var sumtdQD = document.querySelector('.QSumD_DD1');
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
            $('#tbQueryDD1 thead td#td1_DD1').html(parseInt(GetNodeValue(dtH[0], "SumQty1")).toLocaleString('en-US'));
            $('#tbQueryDD1 thead td#td2_DD1').html(parseInt(GetNodeValue(dtH[0], "SumCash1")).toLocaleString('en-US'));
            $('#tbQueryDD1 thead td#td3_DD1').html(parseInt(GetNodeValue(dtH[0], "SumQty2")).toLocaleString('en-US'));
            $('#tbQueryDD1 thead td#td4_DD1').html(parseInt(GetNodeValue(dtH[0], "SumCash2")).toLocaleString('en-US'));
            $('#tbQueryDD1 thead td#td5_DD1').html(GetNodeValue(dtH[0], "SumPer"));
        }
    };

    let QueryD2 = function () {
        ShowLoading();
        var ID = $('#lblDept_D2').html().split('-')[0];
        var OpenDateS1 = $('#lblOpenDate1_D2').html().split('~')[0];
        var OpenDateE1 = $('#lblOpenDate1_D2').html().split('~')[1];
        var OpenDateS2 = $('#lblOpenDate2_D2').html().split('~')[0];
        var OpenDateE2 = $('#lblOpenDate2_D2').html().split('~')[1];

        var pData = {
            OpenDateS1: OpenDateS1,
            OpenDateE1: OpenDateE1,
            OpenDateS2: OpenDateS2,
            OpenDateE2: OpenDateE2,
            ShopNo: chkShopNo,
            ID: ID
        }
        PostToWebApi({ url: "api/SystemSetup/MSSA103QueryD2", data: pData, success: afterMSSA103QueryD2 });
    };

    let afterMSSA103QueryD2 = function (data) {
        CloseLoading();
        if (ReturnMsg(data, 0) != "MSSA103QueryD2OK") {
            DyAlert(ReturnMsg(data, 1));
        }
        else {
            var dtE = data.getElementsByTagName('dtE');
            grdD2.BindData(dtE);
            if (dtE.length == 0) {
                DyAlert("無符合資料!");
                //$(".modal-backdrop").remove();
                var sumtdQD = document.querySelector('.QSumD_D2');
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
            $('#tbQueryD2 thead td#td1_D2').html(parseInt(GetNodeValue(dtH[0], "SumQty1")).toLocaleString('en-US'));
            $('#tbQueryD2 thead td#td2_D2').html(parseInt(GetNodeValue(dtH[0], "SumCash1")).toLocaleString('en-US'));
            $('#tbQueryD2 thead td#td3_D2').html(parseInt(GetNodeValue(dtH[0], "SumQty2")).toLocaleString('en-US'));
            $('#tbQueryD2 thead td#td4_D2').html(parseInt(GetNodeValue(dtH[0], "SumCash2")).toLocaleString('en-US'));
            $('#tbQueryD2 thead td#td5_D2').html(GetNodeValue(dtH[0], "SumPer"));
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
        var pData = {
            ProgramID: "MSSA103"
        }
        PostToWebApi({ url: "api/SystemSetup/GetInitMSSA103", data: pData, success: MSSA103Clear });
    };

    let MSSA103Clear = function (data) {
        if (ReturnMsg(data, 0) != "GetInitMSSA103OK") {
            DyAlert(ReturnMsg(data, 1));
        }
        else {
            var dtE = data.getElementsByTagName('dtE');
            if (dtE.length > 0) {
                $('#txtOpenDateS1').val(GetNodeValue(dtE[0], "SysDate1").toString().trim().replaceAll('/', '-'));
                $('#txtOpenDateE1').val(GetNodeValue(dtE[0], "SysDate1").toString().trim().replaceAll('/', '-'));
                $('#txtOpenDateS2').val(GetNodeValue(dtE[0], "SysDate2").toString().trim().replaceAll('/', '-'));
                $('#txtOpenDateE2').val(GetNodeValue(dtE[0], "SysDate2").toString().trim().replaceAll('/', '-'));
            }
            $('#lblShopNoCnt').html('');
            $('#lblShopNoName').html('');
            chkShopNo = "";
            $('#rdoS').prop('checked', 'true');
        }
    };

    //查詢
    let btQuery_click = function (bt) {
        //Timerset();
        $('#btQuery').prop('disabled', true)
        var QDays = "100";
        //本期
        if ($('#txtOpenDateS2').val() == "" || $('#txtOpenDateE2').val() == "") {
            DyAlert("本期兩欄皆需輸入!", function () { $('#btQuery').prop('disabled', false); })
            return
        }
        else {
            if ($('#txtOpenDateS2').val() > $('#txtOpenDateE2').val()) {
                DyAlert("本期開始日不可大於結束日!", function () { $('#btQuery').prop('disabled', false); })
                return;
            }
            if (DateDiff("d", $('#txtOpenDateS2').val(), $('#txtOpenDateE2').val()) > parseInt(QDays)) {
                DyAlert("本期必須小於等於" + QDays + "天!!");
                return;
            }
        }

        //前期
        if ($('#txtOpenDateS1').val() == "" || $('#txtOpenDateE1').val() == "") {
            DyAlert("前期兩欄皆需輸入!", function () { $('#btQuery').prop('disabled', false); })
            return;
        }
        else {
            if ($('#txtOpenDateS1').val() > $('#txtOpenDateE1').val()) {
                DyAlert("前期開始日不可大於結束日!", function () { $('#btQuery').prop('disabled', false); })
                return;
            }
            if (DateDiff("d", $('#txtOpenDateS1').val(), $('#txtOpenDateE1').val()) > parseInt(QDays)) {
                DyAlert("前期必須小於等於" + QDays + "天!!");
                return;
            }
        }
        
        ShowLoading();
        var Flag = ""
        //店櫃
        if ($('#rdoS').prop('checked') == true) {
            Flag = "S";
        }
        //部門
        else if ($('#rdoD').prop('checked') == true) {
            Flag = "D";
        }
        //大類
        else if ($('#rdoB').prop('checked') == true) {
            Flag = "B";
        }
        setTimeout(function () {
            var pData = {
                OpenDateS1: $('#txtOpenDateS1').val().toString().replaceAll('-', '/'),
                OpenDateE1: $('#txtOpenDateE1').val().toString().replaceAll('-', '/'),
                OpenDateS2: $('#txtOpenDateS2').val().toString().replaceAll('-', '/'),
                OpenDateE2: $('#txtOpenDateE2').val().toString().replaceAll('-', '/'),
                ShopNo: chkShopNo,
                Flag: Flag
            }
            PostToWebApi({ url: "api/SystemSetup/MSSA103Query", data: pData, success: afterMSSA103Query });
        }, 1000);
    };

    let afterMSSA103Query = function (data) {
        CloseLoading();
        if (ReturnMsg(data, 0) != "MSSA103QueryOK") {
            DyAlert(ReturnMsg(data, 1), function () { $('#btQuery').prop('disabled', false); });
        }
        else {
            $('#btQuery').prop('disabled', false);
            var dtE = data.getElementsByTagName('dtE');
            grdM.BindData(dtE);
            var heads = $('#tbQuery thead tr th#thead1');
            if ($('#rdoS').prop('checked')) {
                $(heads).html('店櫃');
            }
            else if ($('#rdoD').prop('checked')) {
                $(heads).html('部門');
            }
            else if ($('#rdoB').prop('checked')) {
                $(heads).html('大類');
            }

            if (dtE.length == 0) {
                DyAlert("無符合資料!");
                //$(".modal-backdrop").remove();
                $('#tbQuery thead td#td1').html('');
                $('#tbQuery thead td#td2').html('');
                $('#tbQuery thead td#td3').html('');
                $('#tbQuery thead td#td4').html('');
                $('#tbQuery thead td#td5').html('');
                return;
            }

            var dtH = data.getElementsByTagName('dtH');
            $('#tbQuery thead td#td1').html(parseInt(GetNodeValue(dtH[0], "SumQty1")).toLocaleString('en-US'));
            $('#tbQuery thead td#td2').html(parseInt(GetNodeValue(dtH[0], "SumCash1")).toLocaleString('en-US'));
            $('#tbQuery thead td#td3').html(parseInt(GetNodeValue(dtH[0], "SumQty2")).toLocaleString('en-US'));
            $('#tbQuery thead td#td4').html(parseInt(GetNodeValue(dtH[0], "SumCash2")).toLocaleString('en-US'));
            $('#tbQuery thead td#td5').html(GetNodeValue(dtH[0], "SumPer"));
        }
    };

    //店櫃多選
    let btShopNo_click = function (bt) {
        //Timerset();
        var pData = {
            ST_ID: ""
        }
        PostToWebApi({ url: "api/SystemSetup/MSVP102_GetVIPFaceID", data: pData, success: afterMSSA103_GetShopNo });
    };

    let afterMSSA103_GetShopNo = function (data) {
        if (ReturnMsg(data, 0) != "MSVP102_GetVIPFaceIDOK") {
            DyAlert(ReturnMsg(data, 1));
        }
        else {
            var dtE = data.getElementsByTagName('dtE');
            $('#txtLpQ_ShopNo').val('');
            $('#modalLookup_ShopNo').modal('show');
            setTimeout(function () {
                grdLookUp_ShopNo.BindData(dtE);
                if (chkShopNo != "") {
                    var ShopNo = chkShopNo.split(',');
                    for (var i = 0; i < ShopNo.length; i++) {
                        $('#tbLookup_ShopNo tbody tr .tdCol2').filter(function () { return $(this).text() == ShopNo[i].replaceAll("'", ""); }).closest('tr').find('.tdCol1 input:checkbox').prop('checked', true);
                    }
                }
            }, 500);
        }
    };

    let btLpQ_ShopNo_click = function (bt) {
        //Timerset();
        $('#btLpQ_ShopNo').prop('disabled', true);
        var pData = {
            ST_ID: $('#txtLpQ_ShopNo').val()
        }
        PostToWebApi({ url: "api/SystemSetup/MSVP102_GetVIPFaceID", data: pData, success: afterLpQ_ShopNo });
    };

    let afterLpQ_ShopNo = function (data) {
        if (ReturnMsg(data, 0) != "MSVP102_GetVIPFaceIDOK") {
            DyAlert(ReturnMsg(data, 1), function () {
                $('#btLpQ_ShopNo').prop('disabled', false);
            });
        }
        else {
            var dtE = data.getElementsByTagName('dtE');
            if (dtE.length == 0) {
                DyAlert("無符合資料!", function () {
                    $('#btLpQ_ShopNo').prop('disabled', false);
                });
                //$(".modal-backdrop").remove();
                return;
            }
            grdLookUp_ShopNo.BindData(dtE);
            $('#btLpQ_ShopNo').prop('disabled', false);
        }
    };

    let btLpOK_ShopNo_click = function (bt) {
        //Timerset();
        $('#btLpOK_ShopNo').prop('disabled', true);
        var obchkedtd = $('#tbLookup_ShopNo .checkbox:checked');
        chkedRow = obchkedtd.length.toString();   //本次已勾選的總筆數
        ClearQuery();
        if (chkedRow == 0) {
            $('#lblShopNoCnt').html('');
            $('#lblShopNoName').html('');
            chkShopNo = "";
            $('#btLpOK_ShopNo').prop('disabled', false);
            $('#modalLookup_ShopNo').modal('hide');
            return
        } else {
            chkShopNo = "";
            var ShopNoName = "";
            for (var i = 0; i < obchkedtd.length; i++) {
                var a = $(obchkedtd[i]).closest('tr');
                var trNode = $(a).prop('Record');
                chkShopNo += "'" + GetNodeValue(trNode, "ST_ID") + "',";  //已勾選的每一筆店倉
                if (i <= 1) {
                    ShopNoName += GetNodeValue(trNode, "ST_SName") + "，";
                }
            }
            chkShopNo = chkShopNo.substr(0, chkShopNo.length - 1)
            if (chkedRow > 2) {
                $('#lblShopNoCnt').html(chkedRow)
                $('#lblShopNoName').html(ShopNoName.substr(0, ShopNoName.length - 1) + '...')
            }
            else {
                $('#lblShopNoCnt').html('')
                $('#lblShopNoName').html(ShopNoName.substr(0, ShopNoName.length - 1))
            }
            $('#btLpOK_ShopNo').prop('disabled', false);
            $('#modalLookup_ShopNo').modal('hide');
        }
    };

    let btLpExit_ShopNo_click = function (bt) {
        //Timerset();
        $('#modalLookup_ShopNo').modal('hide');
    };

    let btLpClear_ShopNo_click = function (bt) {
        //Timerset();
        $("#txtLpQ_ShopNo").val('');
        $("#tbLookup_ShopNo .checkbox").prop('checked', false);
    };

    let ClearQuery = function () {
        //$('#tbQuery thead tr th').css('background-color', '#ffb620')
        grdM.BindData(null)
        var heads = $('#tbQuery thead tr th#thead1');
        if ($('#rdoS').prop('checked')) {
            $(heads).html('店櫃');
        }
        else if ($('#rdoD').prop('checked')) {
            $(heads).html('部門');
        }
        else if ($('#rdoB').prop('checked')) {
            $(heads).html('大類');
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

    let btRe_D1_click = function (bt) {
        //Timerset();
        $('#modalD1').modal('hide')
    };

    let btRe_DD1_click = function (bt) {
        //Timerset();
        $('#modalDD1').modal('hide')
    };

    let btRe_D2_click = function (bt) {
        //Timerset();
        $('#modalD2').modal('hide')
    };
//#region FormLoad
    let GetInitMSSA103 = function (data) {
        if (ReturnMsg(data, 0) != "GetInitMSSA103OK") {
            DyAlert(ReturnMsg(data, 1));
        }
        else {
            var dtE = data.getElementsByTagName('dtE');
            if (dtE.length > 0) {
                $('#lblProgramName').html(GetNodeValue(dtE[0], "ChineseName"));
                $('#txtOpenDateS1').val(GetNodeValue(dtE[0], "SysDate1").toString().trim().replaceAll('/', '-'));
                $('#txtOpenDateE1').val(GetNodeValue(dtE[0], "SysDate1").toString().trim().replaceAll('/', '-'));
                $('#txtOpenDateS2').val(GetNodeValue(dtE[0], "SysDate2").toString().trim().replaceAll('/', '-'));
                $('#txtOpenDateE2').val(GetNodeValue(dtE[0], "SysDate2").toString().trim().replaceAll('/', '-'));
            }
            AssignVar();

            $('#btQuery').click(function () { btQuery_click(this) });
            $('#btClear').click(function () { btClear_click(this) });
            $('#txtOpenDateS1,#txtOpenDateE1,#txtOpenDateS2,#txtOpenDateE2').change(function () { ClearQuery() });
            $('#rdoS,#rdoD,#rdoB').change(function () { btQuery_click() });
            $('#btShopNo').click(function () { btShopNo_click(this) });

            $('#btLpQ_ShopNo').click(function () { btLpQ_ShopNo_click(this) });
            $('#btLpOK_ShopNo').click(function () { btLpOK_ShopNo_click(this) });
            $('#btLpExit_ShopNo').click(function () { btLpExit_ShopNo_click(this) });
            $('#btLpClear_ShopNo').click(function () { btLpClear_ShopNo_click(this) });

            $('#btRe_D1').click(function () { btRe_D1_click(this) });
            $('#rdoD_D1,#rdoB_D1').change(function () { QueryD1(); });

            $('#btRe_DD1').click(function () { btRe_DD1_click(this) });

            $('#btRe_D2').click(function () { btRe_D2_click(this) });

            btQuery_click();
        }
    };
    
    let afterLoadPage = function () {
        var pData = {
            ProgramID: "MSSA103"
        }
        PostToWebApi({ url: "api/SystemSetup/GetInitMSSA103", data: pData, success: GetInitMSSA103 });
    };
//#endregion
    

    if ($('#pgMSSA103').length == 0) {  
        AllPages = new LoadAllPages(ParentNode, "SystemSetup/MSSA103", ["MSSA103btns", "pgMSSA103Init", "pgMSSA103Add", "pgMSSA103Mod"], afterLoadPage);
    };


}