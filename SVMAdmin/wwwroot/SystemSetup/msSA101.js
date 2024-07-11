var PageMSSA101 = function (ParentNode) {

    let grdM;
    let grdD;
    let grdDD;
    let grdLookUp_ShopNo;

    let QDays = "100";
    let chkShopNo = "";

    let AssignVar = function () {

        grdM = new DynGrid(
            {
                table_lement: $('#tbQuery')[0],
                class_collection: ["tdCol1 text-center", "tdCol2 label-align", "tdCol3 label-align", "tdCol4 label-align", "tdCol5 label-align", "tdCol6 label-align", "tdCol7 label-align", "tdCol8 text-center"],
                fields_info: [
                    { type: "Text", name: "ID", style: "" },
                    { type: "TextAmt", name: "Cash1" },
                    { type: "TextAmt", name: "Cnt1"},
                    { type: "TextAmt", name: "CusCash1" },
                    { type: "TextAmt", name: "VCash" },
                    { type: "TextAmt", name: "VCnt" },
                    { type: "TextAmt", name: "VCusCash" },
                    { type: "TextPercent", name: "VPer" }
                ],
                //rows_per_page: 10,
                method_clickrow: click_PLU,
                afterBind: InitModifyDeleteButton,
                sortable: "Y"
            }
        );
        grdD = new DynGrid(
            {
                table_lement: $('#tbQueryD')[0],
                class_collection: ["tdCol1 text-center", "tdCol2 label-align", "tdCol3 label-align", "tdCol4 label-align", "tdCol5 label-align", "tdCol6 label-align", "tdCol7 label-align", "tdCol8 text-center"],
                fields_info: [
                    { type: "Text", name: "ID", style: "" },
                    { type: "TextAmt", name: "Cash1" },
                    { type: "TextAmt", name: "Cnt1" },
                    { type: "TextAmt", name: "CusCash1" },
                    { type: "TextAmt", name: "VCash" },
                    { type: "TextAmt", name: "VCnt" },
                    { type: "TextAmt", name: "VCusCash" },
                    { type: "TextPercent", name: "VPer" }
                ],
                //rows_per_page: 10,
                method_clickrow: click_PLU,
                afterBind: gridclick1,
                sortable: "Y"
            }
        );
        grdDD = new DynGrid(
            {
                table_lement: $('#tbQueryDD')[0],
                class_collection: ["tdCol1 text-center", "tdCol2 label-align", "tdCol3 label-align", "tdCol4 label-align", "tdCol5 label-align", "tdCol6 label-align", "tdCol7 label-align", "tdCol8 text-center"],
                fields_info: [
                    { type: "Text", name: "ID", style: "" },
                    { type: "TextAmt", name: "Cash1" },
                    { type: "TextAmt", name: "Cnt1" },
                    { type: "TextAmt", name: "CusCash1" },
                    { type: "TextAmt", name: "VCash" },
                    { type: "TextAmt", name: "VCnt" },
                    { type: "TextAmt", name: "VCusCash" },
                    { type: "TextPercent", name: "VPer" }
                ],
                //rows_per_page: 10,
                method_clickrow: click_PLU,
                //afterBind: InitModifyDeleteButton,
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
        //$('#tbISAM01Mod .fa-trash-o').click(function () { btPLUDelete_click(this) });
    }
//#region 下展第二層
    let Step1_click = function (bt) {
        var heads = $('#tbQuery thead tr th#thead1').html();
        if (heads.toString().indexOf("銷售日期") >= 0) {
            $('#lblD_DateTitle').html('日&emsp;&emsp;期');
        } else {
            $('#lblD_DateTitle').html('日期區間');
        }

        $('#tbQuery td').closest('tr').css('background-color', 'white');
        $(bt).closest('tr').click();
        $('.msg-valid').hide();
        var node = $(grdM.ActiveRowTR()).prop('Record');
        $('#tbQuery td:contains(' + GetNodeValue(node, 'ID') + ')').closest('tr').css('background-color', '#DEEBF7');
        if (heads.toString().indexOf("銷售日期") >= 0) {
            $('#lblD_Date').html(GetNodeValue(node, 'ID'));
            $('#D_lblrdo1').html('&ensp;區域');
        }
        else {
            $('#lblD_Date').html($('#txtOpenDateS1').val().toString().replaceAll('-', '/') + ' ~ ' + $('#txtOpenDateE1').val().toString().replaceAll('-', '/'));
            $('#D_lblrdo1').html('&ensp;日期');
        }
        $('#lblD_ShopName').html('');
        $('#lblD_ShopCnt').html('');
        $('#D_rdoShop').prop('checked', 'true');
        if (heads.toString().indexOf("店別") >= 0) {
            $('#lblD_ShopName').html(GetNodeValue(node, 'ID'));
            $('#D_rdogrp').hide();
        }
        else {
            if ($('#lblShopNoName').html() != "") {
                $('#lblD_ShopName').html($('#lblShopNoName').html());
            }
            if ($('#lblShopNoCnt').html() != "") {
                $('#lblD_ShopCnt').html($('#lblShopNoCnt').html());
            }
            if (heads.toString().indexOf("銷售日期") >= 0 && chkShopNo!="") {
                $('#D_rdogrp').hide();
            } else {
                $('#D_rdogrp').show();
            }
        }
        if (heads.toString().indexOf("區域") >= 0) {
            $('#D_grp3').show();
            $('#lblD_Area').html(GetNodeValue(node, 'ID'));
        } else {
            $('#D_grp3').hide();
        }

        $('#modal_D').modal('show');
        setTimeout(function () {
            MSSA101Query_D();
        }, 500);
    };

    let MSSA101Query_D = function () {
        var heads = $('#tbQuery thead tr th#thead1').html();
        var SDate;
        var EDate;
        if ($('#lblD_DateTitle').html().indexOf("日期區間") >= 0) {
            SDate=$('#lblD_Date').html().split('~')[0].toString().trim();
            EDate=$('#lblD_Date').html().split('~')[1].toString().trim();
        } else {
            SDate = $('#lblD_Date').html();
            EDate = $('#lblD_Date').html();
        }
        var Flag_D = "";
        var D_Area;
        var D_Shop = "";
        //店櫃
        if (heads.toString().indexOf("店別") >= 0) {
            $('#tbQueryD thead tr th#Dthead1').html('銷售日期');
            Flag_D = "D";
            D_Shop = "'" + $('#lblD_ShopName').html().split('-')[0] + "'";
        }
        //區域
        else if (heads.toString().indexOf("區域") >= 0) {
            D_Area = $('#lblD_Area').html().split('-')[0];
            if ($('#D_rdoShop').prop('checked') == true) {
                $('#tbQueryD thead tr th#Dthead1').html('店別');
                Flag_D = "S";
            } else {
                $('#tbQueryD thead tr th#Dthead1').html('銷售日期');
                Flag_D = "D";
            }
            if (chkShopNo != "") { D_Shop = chkShopNo; }
        }
        //日期
        else if (heads.toString().indexOf("銷售日期") >= 0) {
            if ($('#D_rdoShop').prop('checked') == true) {
                $('#tbQueryD thead tr th#Dthead1').html('店別');
                Flag_D = "S";
            } else {
                $('#tbQueryD thead tr th#Dthead1').html('區域');
                Flag_D = "A";
            }
            if (chkShopNo != "") { D_Shop = chkShopNo; }
        }

        ShowLoading();
        var pData = {
            OpenDateS1: SDate,
            OpenDateE1: EDate,
            ShopNo: D_Shop,
            Area: D_Area,
            Flag: Flag_D
        }
        PostToWebApi({ url: "api/SystemSetup/MSSA101Query", data: pData, success: afterMSSA101Query_D });
    };

    let afterMSSA101Query_D = function (data) {
        CloseLoading();
        if (ReturnMsg(data, 0) != "MSSA101QueryOK") {
            DyAlert(ReturnMsg(data, 1));
        }
        else {
            var dtDelt = data.getElementsByTagName('dtDelt');
            grdD.BindData(dtDelt);

            if (dtDelt.length == 0) {
                DyAlert("無符合資料!");
                //$(".modal-backdrop").remove();
                var sumtdQD = document.querySelector('.QDSum');
                for (i = 0; i < sumtdQD.childElementCount; i++) {
                    sumtdQD.children[i].innerHTML = "";
                }
                return;
            }
            var dtSum = data.getElementsByTagName('dtSum');
            $('#tbQueryD thead td#Dtd1').html(parseInt(GetNodeValue(dtSum[0], "Cash1")).toLocaleString('en-US'));
            $('#tbQueryD thead td#Dtd2').html(parseInt(GetNodeValue(dtSum[0], "Cnt1")).toLocaleString('en-US'));
            $('#tbQueryD thead td#Dtd3').html(parseInt(GetNodeValue(dtSum[0], "CusCash1")).toLocaleString('en-US'));
            $('#tbQueryD thead td#Dtd4').html(parseInt(GetNodeValue(dtSum[0], "VCash")).toLocaleString('en-US'));
            $('#tbQueryD thead td#Dtd5').html(parseInt(GetNodeValue(dtSum[0], "VCnt")).toLocaleString('en-US'));
            $('#tbQueryD thead td#Dtd6').html(parseInt(GetNodeValue(dtSum[0], "VCusCash")).toLocaleString('en-US'));
            $('#tbQueryD thead td#Dtd7').html(GetNodeValue(dtSum[0], "VPer"));
        }
    };

    let btRe_D_click = function (bt) {
        $('#modal_D').modal('hide');
        setTimeout(function () {
            $('#tbQueryD tbody').find('tr').remove();
            var sumtdQD = document.querySelector('.QDSum');
            for (i = 0; i < sumtdQD.childElementCount; i++) {
                sumtdQD.children[i].innerHTML = "";
            }
        }, 500);
    };
//#endregion

    let gridclick1 = function () {
        $('#tbQueryD tbody tr td').click(function () { Step2_click(this) });
    }

//#region 下展第三層
    let Step2_click = function (bt) {
        var head = $('#tbQuery thead tr th#thead1').html();
        var heads = $('#tbQueryD thead tr th#Dthead1').html();
        if (head.toString().indexOf("店別") >= 0 && heads.toString().indexOf("銷售日期") >= 0) {
            return;
        } else if (head.toString().indexOf("銷售日期") >= 0 && heads.toString().indexOf("店別") >= 0) {
            return;
        }

        $('#tbQueryD td').closest('tr').css('background-color', 'white');
        $(bt).closest('tr').click();
        $('.msg-valid').hide();
        var node = $(grdD.ActiveRowTR()).prop('Record');
        $('#tbQueryD td:contains(' + GetNodeValue(node, 'ID') + ')').closest('tr').css('background-color', '#DEEBF7');

        if (heads.toString().indexOf("銷售日期") >= 0 || head.toString().indexOf("銷售日期") >= 0) {
            $('#lblDD_DateTitle').html('日&emsp;&emsp;期');
        } else {
            $('#lblDD_DateTitle').html('日期區間');
        }
        if (heads.toString().indexOf("銷售日期") >= 0) {
            $('#lblDD_Date').html(GetNodeValue(node, 'ID'));
        }
        else {
            $('#lblDD_Date').html($('#lblD_Date').html());
        }
        $('#lblDD_ShopName').html('');
        $('#lblDD_ShopCnt').html('');
        if (head.toString().indexOf("區域") >= 0 && heads.toString().indexOf("店別") >= 0) {
            $('#lblDD_ShopName').html(GetNodeValue(node, 'ID'));
        }
        else {
            if ($('#lblShopNoName').html() != "") {
                $('#lblDD_ShopName').html($('#lblShopNoName').html());
            }
            if ($('#lblShopNoCnt').html() != "") {
                $('#lblDD_ShopCnt').html($('#lblShopNoCnt').html());
            }
        }
        if (heads.toString().indexOf("區域") >= 0) {
            $('#lblDD_Area').html(GetNodeValue(node, 'ID'));
        } else {
            $('#lblDD_Area').html($('#lblD_Area').html());
        }

        $('#modal_DD').modal('show');
        setTimeout(function () {
            MSSA101Query_DD();
        }, 500);
    };

    let MSSA101Query_DD = function () {
        var SDate;
        var EDate;
        var heads = $('#tbQueryD thead tr th#Dthead1').html();
        if ($('#lblDD_DateTitle').html().indexOf("日期區間") >= 0) {
            SDate = $('#lblDD_Date').html().split('~')[0].toString().trim();
            EDate = $('#lblDD_Date').html().split('~')[1].toString().trim();
        } else {
            SDate = $('#lblDD_Date').html();
            EDate = $('#lblDD_Date').html();
        }
        var Flag_D = "";
        var D_Area;
        var D_Shop = "";
        D_Area = $('#lblDD_Area').html().split('-')[0];
        //店櫃
        if (heads.toString().indexOf("店別") >= 0) {
            $('#tbQueryDD thead tr th#DDthead1').html('銷售日期');
            Flag_D = "D";
            D_Shop = "'" + $('#lblDD_ShopName').html().split('-')[0] + "'";
        }
        //區域
        else if (heads.toString().indexOf("區域") >= 0) {
            $('#tbQueryDD thead tr th#DDthead1').html('店別');
            Flag_D = "S";
            if (chkShopNo != "") { D_Shop = chkShopNo; }
        }
        //日期
        else if (heads.toString().indexOf("銷售日期") >= 0) {
            $('#tbQueryDD thead tr th#DDthead1').html('店別');
            Flag_D = "S";
            if (chkShopNo != "") { D_Shop = chkShopNo; }
        }

        ShowLoading();
        var pData = {
            OpenDateS1: SDate,
            OpenDateE1: EDate,
            ShopNo: D_Shop,
            Area: D_Area,
            Flag: Flag_D
        }
        PostToWebApi({ url: "api/SystemSetup/MSSA101Query", data: pData, success: afterMSSA101Query_DD });
    };

    let afterMSSA101Query_DD = function (data) {
        CloseLoading();
        if (ReturnMsg(data, 0) != "MSSA101QueryOK") {
            DyAlert(ReturnMsg(data, 1));
        }
        else {
            var dtDelt = data.getElementsByTagName('dtDelt');
            grdDD.BindData(dtDelt);

            if (dtDelt.length == 0) {
                DyAlert("無符合資料!");
                //$(".modal-backdrop").remove();
                var sumtdQDD = document.querySelector('.QDDSum');
                for (i = 0; i < sumtdQDD.childElementCount; i++) {
                    sumtdQDD.children[i].innerHTML = "";
                }
                return;
            }
            var dtSum = data.getElementsByTagName('dtSum');
            $('#tbQueryDD thead td#DDtd1').html(parseInt(GetNodeValue(dtSum[0], "Cash1")).toLocaleString('en-US'));
            $('#tbQueryDD thead td#DDtd2').html(parseInt(GetNodeValue(dtSum[0], "Cnt1")).toLocaleString('en-US'));
            $('#tbQueryDD thead td#DDtd3').html(parseInt(GetNodeValue(dtSum[0], "CusCash1")).toLocaleString('en-US'));
            $('#tbQueryDD thead td#DDtd4').html(parseInt(GetNodeValue(dtSum[0], "VCash")).toLocaleString('en-US'));
            $('#tbQueryDD thead td#DDtd5').html(parseInt(GetNodeValue(dtSum[0], "VCnt")).toLocaleString('en-US'));
            $('#tbQueryDD thead td#DDtd6').html(parseInt(GetNodeValue(dtSum[0], "VCusCash")).toLocaleString('en-US'));
            $('#tbQueryDD thead td#DDtd7').html(GetNodeValue(dtSum[0], "VPer"));
        }
    };

    let btRe_DD_click = function (bt) {
        $('#modal_DD').modal('hide');
        setTimeout(function () {
            $('#tbQueryDD tbody').find('tr').remove();
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
        var pData = {
            ProgramID: "MSSA101"
        }
        PostToWebApi({ url: "api/SystemSetup/GetInitMSSA101", data: pData, success: MSSA101Clear });
    };

    let MSSA101Clear = function (data) {
        if (ReturnMsg(data, 0) != "GetInitMSSA101OK") {
            DyAlert(ReturnMsg(data, 1));
        }
        else {
            var dtE = data.getElementsByTagName('dtE');
            if (dtE.length > 0) {
                $('#txtOpenDateS1').val(GetNodeValue(dtE[0], "SysDate").toString().trim().replaceAll('/', '-'));
                $('#txtOpenDateE1').val(GetNodeValue(dtE[0], "SysDate").toString().trim().replaceAll('/', '-'));
            }
            $('#lblShopNoCnt').html('');
            $('#lblShopNoName').html('');
            chkShopNo = "";
            $('#rdoA').prop('checked', 'true');
            ClearQuery();
        }
    };
//#endregion

//#region 查詢
    let btQuery_click = function (bt) {
        //Timerset();
        $('#btQuery').prop('disabled', true)
        
        //期間1
        if ($('#txtOpenDateS1').val() == "" || $('#txtOpenDateE1').val() == "") {
            DyAlert("日期區間兩欄皆需輸入!", function () { $('#btQuery').prop('disabled', false); })
            return;
        }
        else {
            if ($('#txtOpenDateS1').val() > $('#txtOpenDateE1').val()) {
                DyAlert("開始日不可大於結束日!", function () { $('#btQuery').prop('disabled', false); })
                return;
            }
        }

        if (DateDiff("d", $('#txtOpenDateS1').val(), $('#txtOpenDateE1').val()) > parseInt(QDays)) {
            DyAlert("日期區間必須小於等於" + QDays + "天!!");
            return;
        }
        if ($('#rdoA').prop('checked') == true && chkShopNo!="") { $('#rdoS').prop('checked', 'true'); }
        ShowLoading();

        var Flag = ""
        //店櫃
        if ($('#rdoS').prop('checked') == true) {
            Flag = "S";
        }
        //區域
        else if ($('#rdoA').prop('checked') == true) {
            Flag = "A";
        }
        //日期
        else if ($('#rdoD').prop('checked') == true) {
            Flag = "D";
        }

        setTimeout(function () {
            var pData = {
                OpenDateS1: $('#txtOpenDateS1').val().toString().replaceAll('-', '/'),
                OpenDateE1: $('#txtOpenDateE1').val().toString().replaceAll('-', '/'),
                ShopNo: chkShopNo,
                Flag: Flag
            }
            PostToWebApi({ url: "api/SystemSetup/MSSA101Query", data: pData, success: afterMSSA101Query });
        }, 1000);
    };

    let afterMSSA101Query = function (data) {
        CloseLoading();
        if (ReturnMsg(data, 0) != "MSSA101QueryOK") {
            DyAlert(ReturnMsg(data, 1), function () { $('#btQuery').prop('disabled', false); });
        }
        else {
            $('#btQuery').prop('disabled', false);
            var dtDelt = data.getElementsByTagName('dtDelt');
            grdM.BindData(dtDelt);
            var heads = $('#tbQuery thead tr th#thead1');
            if ($('#rdoS').prop('checked')) {
                $(heads).html('店別');
            }
            else if ($('#rdoD').prop('checked')) {
                $(heads).html('銷售日期');
            }
            else if ($('#rdoA').prop('checked')) {
                $(heads).html('區域');
            }

            if (dtDelt.length == 0) {
                DyAlert("無符合資料!");
                $(".modal-backdrop").remove();
                var sumtdQ = document.querySelector('.QSum');
                for (i = 0; i < sumtdQ.childElementCount; i++) {
                    sumtdQ.children[i].innerHTML = "";
                }
                return;
            }

            var dtSum = data.getElementsByTagName('dtSum');
            $('#tbQuery thead td#td1').html(parseInt(GetNodeValue(dtSum[0], "Cash1")).toLocaleString('en-US'));
            $('#tbQuery thead td#td2').html(parseInt(GetNodeValue(dtSum[0], "Cnt1")).toLocaleString('en-US'));
            $('#tbQuery thead td#td3').html(parseInt(GetNodeValue(dtSum[0], "CusCash1")).toLocaleString('en-US'));
            $('#tbQuery thead td#td4').html(parseInt(GetNodeValue(dtSum[0], "VCash")).toLocaleString('en-US'));
            $('#tbQuery thead td#td5').html(parseInt(GetNodeValue(dtSum[0], "VCnt")).toLocaleString('en-US'));
            $('#tbQuery thead td#td6').html(parseInt(GetNodeValue(dtSum[0], "VCusCash")).toLocaleString('en-US'));
            $('#tbQuery thead td#td7').html(GetNodeValue(dtSum[0], "VPer"));
        }
    };

    let ClearQuery = function () {
        $('#tbQuery tbody').find('tr').remove();
        var heads = $('#tbQuery thead tr th#thead1');
        if ($('#rdoS').prop('checked')) {
            $(heads).html('店別');
        }
        else if ($('#rdoD').prop('checked')) {
            $(heads).html('銷售日期');
        }
        else if ($('#rdoA').prop('checked')) {
            $(heads).html('區域');
        }
        var sumtdQ = document.querySelector('.QSum');
        for (i = 0; i < sumtdQ.childElementCount; i++) {
            sumtdQ.children[i].innerHTML = "";
        }
        //$('#tbQuery thead td#td1').html('');
        //$('#tbQuery thead td#td2').html('');
        //$('#tbQuery thead td#td3').html('');
        //$('#tbQuery thead td#td4').html('');
        //$('#tbQuery thead td#td5').html('');
        //$('#tbQuery thead td#td6').html('');
        //$('#tbQuery thead td#td7').html('');
    }
//#endregion

//#region 店櫃多選
    let btShopNo_click = function (bt) {
        //Timerset();
        var pData = {
            ST_ID: ""
        }
        PostToWebApi({ url: "api/SystemSetup/MSVP102_GetVIPFaceID", data: pData, success: afterMSSA101_GetShopNo });
    };

    let afterMSSA101_GetShopNo = function (data) {
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
        if (chkedRow == 0) {
            $('#lblShopNoCnt').html('');
            $('#lblShopNoName').html('');
            chkShopNo = "";
            $('#btLpOK_ShopNo').prop('disabled', false);
            $('#modalLookup_ShopNo').modal('hide');
            ClearQuery();
            return
        } else {
            chkShopNo = "";
            var ShopNoName = "";
            for (var i = 0; i < obchkedtd.length; i++) {
                var a = $(obchkedtd[i]).closest('tr');
                var trNode = $(a).prop('Record');
                chkShopNo += "'" + GetNodeValue(trNode, "ST_ID") + "',";  //已勾選的每一筆店倉
                if (i <= 4) {
                    ShopNoName += GetNodeValue(trNode, "ST_SName") + "，";
                }
            }
            chkShopNo = chkShopNo.substr(0, chkShopNo.length - 1)
            if (chkedRow > 5) {
                $('#lblShopNoName').html(ShopNoName.substr(0, ShopNoName.length - 1) + '...')
                $('#lblShopNoCnt').html(chkedRow)
            }
            else {
                $('#lblShopNoName').html(ShopNoName.substr(0, ShopNoName.length - 1))
                $('#lblShopNoCnt').html('')
            }
            $('#btLpOK_ShopNo').prop('disabled', false);
            $('#modalLookup_ShopNo').modal('hide');
            if ($('#rdoA').prop('checked') == true) { $('#rdoS').prop('checked', 'true'); }
            ClearQuery();
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
//#endregion

//#region FormLoad
    let GetInitMSSA101 = function (data) {
        if (ReturnMsg(data, 0) != "GetInitMSSA101OK") {
            DyAlert(ReturnMsg(data, 1));
        }
        else {
            var dtE = data.getElementsByTagName('dtE');
            if (dtE.length > 0) {
                $('#lblProgramName').html(GetNodeValue(dtE[0], "ChineseName"));
                $('#txtOpenDateS1').val(GetNodeValue(dtE[0], "SysDate").toString().trim().replaceAll('/', '-'));
                $('#txtOpenDateE1').val(GetNodeValue(dtE[0], "SysDate").toString().trim().replaceAll('/', '-'));
            }
            AssignVar();

            $('#btQuery').click(function () { btQuery_click(this) });
            $('#btClear').click(function () { btClear_click(this) });
            $('#rdoS,#rdoA,#rdoD,#txtOpenDateS1,#txtOpenDateE1').change(function () { ClearQuery() });


            $('#btShopNo').click(function () { btShopNo_click(this) });
            $('#btLpQ_ShopNo').click(function () { btLpQ_ShopNo_click(this) });
            $('#btLpOK_ShopNo').click(function () { btLpOK_ShopNo_click(this) });
            $('#btLpExit_ShopNo').click(function () { btLpExit_ShopNo_click(this) });
            $('#btLpClear_ShopNo').click(function () { btLpClear_ShopNo_click(this) });
            $('#btRe_D').click(function () { btRe_D_click(this) });
            $('#D_rdoShop,#D_rdo1').change(function () { MSSA101Query_D() });
            $('#btRe_DD').click(function () { btRe_DD_click(this) });
        }
    };
    
    let afterLoadPage = function () {
        var pData = {
            ProgramID: "MSSA101"
        }
        PostToWebApi({ url: "api/SystemSetup/GetInitMSSA101", data: pData, success: GetInitMSSA101 });
    };
//#endregion
    

    if ($('#pgMSSA101').length == 0) {  
        AllPages = new LoadAllPages(ParentNode, "SystemSetup/MSSA101", ["pgMSSA101Init"], afterLoadPage);
    };


}