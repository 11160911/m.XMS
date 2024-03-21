var PagemsDM101 = function (ParentNode) {

    let grdM;
    let AssignVar = function () {
        grdM = new DynGrid(
            {
                table_lement: $('#tbDM')[0],
                class_collection: ["tdCol1","tdCol2"],
                fields_info: [
                    { type: "Text", name: "Type", style: "" },
                    { type: "Text", name: "DocNo", style: "" }
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
        $('#tbDM tbody tr td').click(function () { DMMod_click(this) });
    }

    let DMMod_click = function (bt) {
        $(bt).closest('tr').click();
        $('.msg-valid').hide();
        var node = $(grdM.ActiveRowTR()).prop('Record');

        if (GetNodeValue(node, 'Type') == "A") {
            for (var i = 0; i < 8; i++) {
                GetGetImage("PLUPic" + String(i + 1) + "_A", "");
            }

            $('#titDMAdd_A').text('版型A修改');
            $('#lblDocNo_DMAdd_A').html(GetNodeValue(node, 'DocNo'))
            $('#modal_DMAdd_A').modal('show');
            setTimeout(function () {
                Print_DMMod_A();
            }, 500);
        }
    };

    let Print_DMMod_A = function () {
        var pData = {
            DocNo: $('#lblDocNo_DMAdd_A').html()
        }
        PostToWebApi({ url: "api/SystemSetup/Print_DMMod_A", data: pData, success: afterPrint_DMMod_A });
    };

    let afterPrint_DMMod_A = function (data) {
        if (ReturnMsg(data, 0) != "Print_DMMod_AOK") {
            DyAlert(ReturnMsg(data, 1), function () {
                $('#modal_DMAdd_A').modal('hide');
            });
        }
        else {
            var dtE = data.getElementsByTagName('dtE');
            for (var i = 0; i < dtE.length; i++) {
                GetGetImage("PLUPic" + String(i + 1) + "_A", GetNodeValue(dtE[i], "SGID"));
            }
        }
    };

//    let InitModifyDeleteButton = function () {
//        $('#tbSales1 tbody tr td').click(function () { Step1_click(this) });
//        //$('#tbISAM01Mod .fa-trash-o').click(function () { btPLUDelete_click(this) });
//    }

//    let Step1_click = function (bt) {

//        $(bt).closest('tr').click();
//        $('.msg-valid').hide();
//        var node = $(grdM.ActiveRowTR()).prop('Record');

//        if ($('#rdoArea').prop('checked')) {
//            $('#lblOpenDate_Area_Step1').html($('#OpenDateS').val() + "~" + $('#OpenDateE').val());
//            $('#lblArea_Step1').html(GetNodeValue(node, 'ID1') + " " + GetNodeValue(node, 'Name1'));
//            $('#rdoShop_Area_Step1').prop('checked', true);
//            $('#modal_Area_Step1').modal('show');
//            setTimeout(function () {
//                Query_Area_Step1_click();
//            }, 500);
//        }
//        else if ($('#rdoShop').prop('checked')) {
//            $('#lblOpenDate_Shop_Step1').html($('#OpenDateS').val() + "~" + $('#OpenDateE').val());
//            $('#lblShop_Step1').html(GetNodeValue(node, 'ID1') + " " + GetNodeValue(node, 'Name1'));
//            $('#modal_Shop_Step1').modal('show');
//            setTimeout(function () {
//                Query_Shop_Step1_click();
//            }, 500);
//        }
//        else if ($('#rdoDate').prop('checked')) {
//            $('#lblOpenDate_Date_Step1').html($('#OpenDateS').val() + "~" + $('#OpenDateE').val());
//            $('#lblDate_Step1').html(GetNodeValue(node, 'ID1'));
//            $('#rdoArea_Date_Step1').prop('checked', true);
//            $('#modal_Date_Step1').modal('show');
//            setTimeout(function () {
//                Query_Date_Step1_click();
//            }, 500);
//        }
//    };

//    let ChkLogOut_1 = function (AfterChkLogOut_1) {
//        var LoginDT = sessionStorage.getItem('LoginDT');
//        var cData = {
//            LoginDT: LoginDT
//        }
//        PostToWebApi({ url: "api/js/ChkLogOut", data: cData, success: AfterChkLogOut_1 });
//    };

////#region 編查
////#region 單筆修改
//    let AfterSaveISAM01PLUMod = function (data) {
//        if (ReturnMsg(data, 0) != "SaveISAM01PLUModOK") {
//            DyAlert(ReturnMsg(data, 1));
//        }
//        else {
//            DyAlert("修改完成!");

//            $('#modal_ISAM01PLUMod').modal('hide');
//            var dtBINMod = data.getElementsByTagName('dtBINMod')[0];
//            grdM.RefreshRocord(grdM.ActiveRowTR(), dtBINMod);
//        }

//    };

//    let btModSave_click = function () {
//        ChkLogOut_1(btModSave_click_1);
//    };

//    let btModSave_click_1 = function (data) {
//        if (ReturnMsg(data, 0) != "ChkLogOutOK") {
//            DyAlert(ReturnMsg(data, 1));
//        }
//        else {
//            var dtLogin = data.getElementsByTagName('dtLogin');
//            if (GetNodeValue(dtLogin[0], "LogOutType") != "") {
//                window.location.href = "Login" + sessionStorage.getItem('isamcomp');
//            }
//            else {
//Timerset(sessionStorage.getItem('isamcomp'));
//        if ($('#txtModQty1').val() == "") {
//            DyAlert("請輸入數量!");
//            return;
//        }
//        else {

//            if (isNaN($('#txtModQty1').val())) {
//                DyAlert("請輸入數字!");
//                return;
//            }
//            if ($('#txtModQty1').val().indexOf(".") > 0) {
//                DyAlert("請輸入整數!");
//                return;
//            }

//            if ($('#txtModQty1').val() <= 0) {
//                DyAlert("數量需大於0!");
//                return;
//            }

//            if ($('#txtModQty1').val() > 999999) {
//                DyAlert("數量不可大於999999!");
//                return;
//            }
//        }
//        var cData = {
//            Shop: $('#lblShop2').html().split(' ')[0],
//            ISAMDate: $('#lblDate2').html(),
//            BinNo: $('#lblBINNo2').html(),
//            PLU: ModPLU,
//            Qty: $('#txtModQty1').val()
//        }
//        PostToWebApi({ url: "api/SystemSetup/SaveISAM01PLUMod", data: cData, success: AfterSaveISAM01PLUMod });
//            }
//        }
//    }

//    let btModCancel_click = function () {
//        //*
//        ChkLogOut(sessionStorage.getItem('isamcomp'));
//        Timerset(sessionStorage.getItem('isamcomp'));
//        $('#modal_ISAM01PLUMod').modal('hide');
//    };

//    let AfterGetModGDName = function (data) {
//        if (ReturnMsg(data, 0) != "GetGDNameOK") {
//            DyAlert(ReturnMsg(data, 1));
//        }
//        else {
//            var dtP = data.getElementsByTagName('dtPLU');
//            //alert(dtP.length);
//            $('#lblModPLU').html(ModPLU);
//            $('#lblModQty1').html(ModPLUQty);
//            $('#txtModQty1').val(ModPLUQty);
//            if (dtP.length > 0) {
//                $('#lblModPLUName').html(GetNodeValue(dtP[0], 'GD_Name'));
//            }
//            else {
//                //DyAlert("無符合之商品資料!");
//                //return;
//                $('#lblModPLUName').html('');
//            }
//            $('#modal_ISAM01PLUMod').modal('show');
//        }

//        $('.msg-valid').hide();
//    };

//    let btPLUMod_click = function (bt) {
//        //*
//        ChkLogOut(sessionStorage.getItem('isamcomp'));
//        Timerset(sessionStorage.getItem('isamcomp'));
//        $(bt).closest('tr').click();
//        //alert(GetNodeValue(node, 'AppDate'));


//        $('.msg-valid').hide();
//        $('#modal_ISAM01PLUMod .modal-title').text('盤點資料單筆修改');
//        //$('#modal_ISAM01Mod .btn-danger').text('刪除');
//        var node = $(grdM.ActiveRowTR()).prop('Record');
//        ModPLU = GetNodeValue(node, 'PLU');
//        ModPLUQty = GetNodeValue(node, 'Qty1');
//        var cData = {
//            Shop: $('#lblShop2').html().split(' ')[0],
//            ISAMDate: $('#lblDate2').html(),
//            BinNo: $('#lblBINNo2').html(),
//            PLU: ModPLU
//        }
//        PostToWebApi({ url: "api/SystemSetup/GetGDName", data: cData, success: AfterGetModGDName });

//    };
////#endregion

////#region 單筆刪除
//    let AfterDelISAM01PLU = function (data) {
//        if (ReturnMsg(data, 0) != "DelISAM01PLUOK") {
//            DyAlert(ReturnMsg(data, 1));
//        }
//        else {
//            DyAlert("刪除完成!");

//            $('#modal_ISAM01PLUDel').modal('hide');
//            //var userxml = data.getElementsByTagName('dtRack')[0];
//            grdM.DeleteRow(grdM.ActiveRowTR());
//        }

//    };

//    let btDelSave_click = function () {
//        ChkLogOut_1(btDelSave_click_1);
//    };

//    let btDelSave_click_1 = function (data) {
//        if (ReturnMsg(data, 0) != "ChkLogOutOK") {
//            DyAlert(ReturnMsg(data, 1));
//        }
//        else {
//            var dtLogin = data.getElementsByTagName('dtLogin');
//            if (GetNodeValue(dtLogin[0], "LogOutType") != "") {
//                window.location.href = "Login" + sessionStorage.getItem('isamcomp');
//            }
//            else {
//                Timerset(sessionStorage.getItem('isamcomp'));
//                var cData = {
//                    Shop: $('#lblShop2').html().split(' ')[0],
//                    ISAMDate: $('#lblDate2').html(),
//                    BinNo: $('#lblBINNo2').html(),
//                    PLU: DelPLU
//                }
//                PostToWebApi({ url: "api/SystemSetup/DelISAM01PLU", data: cData, success: AfterDelISAM01PLU });
//            }
//        }
//    }

//    let btDelCancel_click = function () {
//        //*
//        ChkLogOut(sessionStorage.getItem('isamcomp'))
//        Timerset(sessionStorage.getItem('isamcomp'));
//        $('#modal_ISAM01PLUDel').modal('hide');
//    };

//    let AfterGetGDName = function (data) {
//        if (ReturnMsg(data, 0) != "GetGDNameOK") {
//            DyAlert(ReturnMsg(data, 1));
//        }
//        else {
//            var dtP = data.getElementsByTagName('dtPLU');
//            //alert(dtP.length);
//            $('#lblPLU').html(DelPLU);
//            $('#lblDelQty1').html(DelPLUQty);
//            if (dtP.length > 0) {
//                /*DyConfirm("確定要刪除商品" + GetNodeValue(dtP[0], 'PLU') + GetNodeValue(dtP[0], 'GD_Name') + "？", afterDelPLU(GetNodeValue(dtP[0], 'PLU')), DummyFunction);*/
//                $('#lblPLUName').html(GetNodeValue(dtP[0], 'GD_Name'));
//            }
//            else {
//                $('#lblPLUName').html('');
//             }
//            $('#modal_ISAM01PLUDel').modal('show');
//        }

//        $('.msg-valid').hide();
//    };

//    let btPLUDelete_click = function (bt) {
//        //*
//        ChkLogOut(sessionStorage.getItem('isamcomp'));
//        Timerset(sessionStorage.getItem('isamcomp'));
//        $(bt).closest('tr').click();
//        $('.msg-valid').hide();
//        $('#modal_ISAM01PLUDel .modal-title').text('盤點資料單筆刪除');
//        //$('#modal_ISAM01Mod .btn-danger').text('刪除');
//        var node = $(grdM.ActiveRowTR()).prop('Record');
//        DelPLU = GetNodeValue(node, 'PLU');
//        DelPLUQty = GetNodeValue(node, 'Qty1');
//        var cData = {
//            Shop: $('#lblShop2').html().split(' ')[0],
//            ISAMDate: $('#lblDate2').html(),
//            BinNo: $('#lblBINNo2').html(),
//            PLU: DelPLU
//        }
//        PostToWebApi({ url: "api/SystemSetup/GetGDName", data: cData, success: AfterGetGDName });

//    };
////#endregion

//    let txtBarcode3_ini = function () {
//        $('#txtBarcode3').val('');
//        $('#txtBarcode3').focus();
//    }

//    let afterGetBINWebMod = function (data) {
//        if (ReturnMsg(data, 0) != "GetBINWebModOK") {
//            DyAlert(ReturnMsg(data, 1), txtBarcode3_ini);

//        }
//        else {
//            var dtBin = data.getElementsByTagName('dtBin');
//            grdM.BindData(dtBin);
//            if (dtBin.length == 0) {
//                //alert("No RowData");
//                DyAlert("無符合資料!", txtBarcode3_ini);
//                $(".modal-backdrop").remove();
//                return;
//            }
//            txtBarcode3_ini()
//        }

//    };

////#endregion

////#region 新增
//    let btQtySave1_click = function () {
//        ChkLogOut_1(btQtySave1_click_1)
//    };

//    let btQtySave1_click_1 = function (data) {
//        if (ReturnMsg(data, 0) != "ChkLogOutOK") {
//            DyAlert(ReturnMsg(data, 1));
//        }
//        else {
//            var dtLogin = data.getElementsByTagName('dtLogin');
//            if (GetNodeValue(dtLogin[0], "LogOutType") != "") {
//                window.location.href = "Login" + sessionStorage.getItem('isamcomp');
//            }
//            else {
//                Timerset(sessionStorage.getItem('isamcomp'));
//                if ($('#txtBarcode1').val() == "" && $('#lblBarcode').html() == "") {
//                    DyAlert("請輸入條碼!");
//                    $('#txtQty1').val('');
//                    //$('#btKeyin1').prop('disabled', false);
//                    //$('#btBCSave1').prop('disabled', false);
//                    //$('#txtBarcode1').prop('disabled', false);
//                    //$('#btQtySave1').prop('disabled', true);
//                    //$('#txtQty1').prop('disabled', true);
//                    return;
//                }
//                if ($('#txtQty1').val() != "") {
//                    if (isNaN($('#txtQty1').val())) {
//                        DyAlert("請輸入數字!");
//                        return;
//                    }
//                    if ($('#txtQty1').val().indexOf(".") > 0) {
//                        DyAlert("請輸入整數!");
//                        return;
//                    }

//                    if ($('#txtQty1').val() <= 0) {
//                        DyAlert("數量需大於0!");
//                        return;
//                    }
//                }
//                else {
//                    $('#txtQty1').val() == "1"
//                }

//                //$('#btKeyin1').prop('disabled', false);
//                //$('#btBCSave1').prop('disabled', false);
//                //$('#txtBarcode1').prop('disabled', false);
//                //$('#btQtySave1').prop('disabled', true);
//                //$('#txtQty1').prop('disabled', true);
//                var pData = {
//                    Shop: $('#lblShop2').html().split(' ')[0],
//                    ISAMDate: $('#lblDate2').html(),
//                    BinNo: $('#lblBINNo2').html(),
//                    Barcode: $('#txtBarcode1').val() == "" ? $('#lblBarcode').html() : $('#txtBarcode1').val(),
//                    Qty: $('#txtQty1').val()
//                };
//                PostToWebApi({ url: "api/SystemSetup/SaveBINWeb", data: pData, success: afterSaveBINWeb });
//            }
//        }
//    }

//    let btKeyin1_click = function () {
//        //*
//        ChkLogOut(sessionStorage.getItem('isamcomp'))
//        Timerset(sessionStorage.getItem('isamcomp'));
//        //$('#btKeyin1').prop('disabled', true);
//        //$('#btQtySave1').prop('disabled', false);
//        //$('#txtQty1').prop('disabled', false);
//        //$('#btBCSave1').prop('disabled', true);
//        //$('#txtBarcode1').prop('disabled', true);
//    };

//    let afterSaveBINWeb = function (data) {
//        if (ReturnMsg(data, 0) != "SaveBINWebOK") {
//            DyAlert(ReturnMsg(data, 1),txtBarcode1_ini);
//        }
//        else {
//            var dtSQ = data.getElementsByTagName('dtSQ');
//            if (dtSQ.length > 0) {
//                $('#lblSQty1').html(GetNodeValue(dtSQ[0], "SQ1"));
//            }
//            else {
//                $('#lblSQty1').html('');
//            }
//            var dtSBQ = data.getElementsByTagName('dtSBQ');
//            if (dtSBQ.length > 0) {
//                $('#lblSBQty1').html(GetNodeValue(dtSBQ[0], "SBQ1"));
//            }
//            else {
//                $('#lblSBQty1').html('');
//            }
//            var dtSWQ = data.getElementsByTagName('dtSWQ');
//            if (dtSWQ.length > 0) {
//                $('#lblSWQty1').html(GetNodeValue(dtSWQ[0], "SWQ1"));
//            }
//            else {
//                $('#lblSWQty1').html('');
//            }

//            var dtP = data.getElementsByTagName('dtPLU');
//            if (dtP.length > 0) {

//                $('#lblBarcode').html(GetNodeValue(dtP[0], "PLU"));
//                $('#txtBarcode1').val('');
//                $('#lblQty1').html($('#txtQty1').val());
//                $('#lblPrice').html(parseInt(GetNodeValue(dtP[0], "GD_Retail")));
//                $('#lblGDName').html(GetNodeValue(dtP[0], "GD_Name"));
//                $('#txtQty1').val("");
//            }
//            else {

//                if ($('#txtBarcode1').val() == "") {
//                }
//                else {
//                    $('#lblBarcode').html($('#txtBarcode1').val());
//                }
//                $('#txtBarcode1').val('');
//                $('#lblQty1').html($('#txtQty1').val());
//                $('#lblPrice').html('');
//                $('#lblGDName').html('');
//                $('#txtQty1').val("");
//            }
//            //alert(dtBin.length);
//            //if (dtBin.length == 0) {
//            //    alert("No RowData");
//            //    DyAlert("無符合資料!", BlankMode);
//            //    return;
//            //}
//        }
//    };

//    let txtBarcode1_ini = function () {
//        $('#txtBarcode1').val('');
//        $('#txtBarcode1').focus();
//    }

//    let txtQty1_ini = function () {
//        $('#txtQty1').val('');
//        $('#txtQty1').focus();
//    }

//    let btBCSave1_click = function () {
//        ChkLogOut_1(btBCSave1_click_1)
//    };

//    let btBCSave1_click_1 = function (data) {

//        if (ReturnMsg(data, 0) != "ChkLogOutOK") {
//            DyAlert(ReturnMsg(data, 1), txtBarcode1_ini);
//        }
//        else {
//            var dtLogin = data.getElementsByTagName('dtLogin');
//            if (GetNodeValue(dtLogin[0], "LogOutType") != "") {
//                window.location.href = "Login" + sessionStorage.getItem('isamcomp');
//            }
//            else {
//Timerset(sessionStorage.getItem('isamcomp'));
//            if ($('#txtBarcode1').val() == "") {
//                DyAlert("請輸入條碼!", txtBarcode1_ini);
//                $(".modal-backdrop").remove();
//                return;
//                }

//            if ($('#txtBarcode1').val().length > 16) {
//                DyAlert("條碼限制輸入16個字元!", txtBarcode1_ini);
//                $(".modal-backdrop").remove();
//                return;
//                }

//            if ($('#txtQty1').val() == "" || $('#txtQty1').val() == "0") {
//                $('#txtQty1').val("1");
//            }
//            else {
//                    if (isNaN($('#txtQty1').val())) {
//                        DyAlert("請輸入數字!", txtQty1_ini );
//                        return;
//                    }
//                    if ($('#txtQty1').val().indexOf(".") > 0) {
//                        DyAlert("請輸入整數!", txtQty1_ini);
//                        return;
//                    }
//                    //if ($('#txtQty1').val() <= 0) {
//                    //    DyAlert("數量需大於0!", txtBarcode1_ini);
//                    //    return;
//                    //}
//                    if ($('#txtQty1').val() > 9999 || $('#txtQty1').val() < -9999) {
//                        DyAlert("數量需介於-9999~9999之間!", txtQty1_ini);
//                        return;
//                    }
//                }
//                var pData = {
//                    Shop: $('#lblShop2').html().split(' ')[0],
//                    ISAMDate: $('#lblDate2').html(),
//                    BinNo: $('#lblBINNo2').html(),
//                    Barcode: $('#txtBarcode1').val(),
//                    Qty: $('#txtQty1').val()
//                };
//                PostToWebApi({ url: "api/SystemSetup/ChkSaveBINWeb", data: pData, success: afterChkSaveBINWeb });
//            }
//        }
//    }

//    let afterChkSaveBINWeb = function (data) {
//        if (ReturnMsg(data, 0) != "ChkSaveBINWebOK") {
//            DyAlert(ReturnMsg(data, 1));
//        }
//        else {
//            var dtBIN = data.getElementsByTagName('dtBIN');
//            if (dtBIN.length == 0) {
//                if ($('#txtQty1').val() < 0) {
//                    DyAlert("單品總數需大於0!", txtQty1_ini);
//                    return;
//                }
//            }
//            else {
//                if (parseInt(GetNodeValue(dtBIN[0], "SumQty")) + parseInt($('#txtQty1').val()) > 999999) {
//                    DyAlert("單品總數不可大於999999!", txtQty1_ini);
//                    return;
//                }
//                if (parseInt(GetNodeValue(dtBIN[0], "SumQty")) + parseInt($('#txtQty1').val()) < 0) {
//                    DyAlert("單品總數需大於0!", txtQty1_ini);
//                    return;
//                }
//                var pData = {
//                    Shop: $('#lblShop2').html().split(' ')[0],
//                    ISAMDate: $('#lblDate2').html(),
//                    BinNo: $('#lblBINNo2').html(),
//                    Barcode: $('#txtBarcode1').val(),
//                    Qty: $('#txtQty1').val()
//                };
//                PostToWebApi({ url: "api/SystemSetup/SaveBINWeb", data: pData, success: afterSaveBINWeb });
//            }
//        }

//    }


////#endregion

//    let BtnSet = function (edit) {
//        switch (edit) {
//            case "A":
//                $('#btAdd').prop('disabled', false);
//                document.getElementById("btAdd").style.background = "blue";
//                $('#btMod').prop('disabled', true);
//                document.getElementById("btMod").style.background = 'gray';
//                $('#btToFTP').prop('disabled', true); //true-btn不能使用
//                document.getElementById("btToFTP").style.background = 'gray';

//                $('#txtBarcode1').val('');
//                $('#txtBarcode1').focus();
//                $('#txtQty1').val("");
//                $('#lblBarcode').html('');
//                $('#lblQty1').html('');
//                $('#lblSQty1').html('');
//                $('#lblSBQty1').html('');
//                $('#lblSWQty1').html('');
//                $('#lblPrice').html('');
//                $('#lblGDName').html('');
//                break;
//            case "Q":
//                $('#btAdd').prop('disabled', false);
//                document.getElementById("btAdd").style.background = "blue";
//                $('#btMod').prop('disabled', false);
//                document.getElementById("btMod").style.background = "Green";
//                $('#btToFTP').prop('disabled', false); //true-btn不能使用
//                document.getElementById("btToFTP").style.background = "gold";
//                break;
//            case "M":
//                $('#btAdd').prop('disabled', true);
//                document.getElementById("btAdd").style.background = 'gray';
//                $('#btMod').prop('disabled', false);
//                document.getElementById("btMod").style.background = "Green";
//                $('#btToFTP').prop('disabled', true); //true-btn不能使用
//                document.getElementById("btToFTP").style.background = 'gray';
//                $('#txtBarcode3').val('');
//                break;
//        }
//    };

////#region 上傳
//    let AfterAddISAMToFTPRecWeb = function (data) {
//        if (ReturnMsg(data, 0) != "AddISAMToFTPRecWebOK") {
//            if (ReturnMsg(data, 1) == "FTP") {
//                DyAlert("FTP設定有誤，請重新確認!")
//            }
//            else if (ReturnMsg(data, 1) == "上傳記錄") {
//                DyAlert("待上傳記錄新增失敗，請重新上傳!")
//            }
//            else if (ReturnMsg(data, 1) == "上傳資料") {
//                DyAlert("無上傳資料，請重新確認!")
//            }
//            else if (ReturnMsg(data, 1) == "上傳檔案") {
//                DyAlert("待上傳檔案不存在，請重新確認!")
//            }
//            else {
//                DyAlert(ReturnMsg(data, 1));
//            }
//        }
//        else {
//            DyAlert("上傳成功!")
//        }
//    }

//    let CallSendToFTP = function () {
//        ChkLogOut_1(CallSendToFTP_1);
//    };

//    let CallSendToFTP_1 = function (data) {
//        if (ReturnMsg(data, 0) != "ChkLogOutOK") {
//            DyAlert(ReturnMsg(data, 1));
//        }
//        else {
//            var dtLogin = data.getElementsByTagName('dtLogin');
//            if (GetNodeValue(dtLogin[0], "LogOutType") != "") {
//                window.location.href = "Login" + sessionStorage.getItem('isamcomp');
//            }
//            else {
//                Timerset(sessionStorage.getItem('isamcomp'));
//                var cData = {
//                    Type: "T",
//                    Shop: $('#lblShop2').html().split(' ')[0],
//                    ISAMDate: $('#lblDate2').html(),
//                    BinNo: $('#lblBINNo2').html()
//                }
//                PostToWebApi({ url: "api/SystemSetup/AddISAMToFTPRecWeb", data: cData, success: AfterAddISAMToFTPRecWeb });
//            }
//        }
//    }


//    let btToFTP_click = function () {
//        //*
//        ChkLogOut(sessionStorage.getItem('isamcomp'));
//        Timerset(sessionStorage.getItem('isamcomp'));
//        DyConfirm("是否要上傳" + $('#lblBINNo2').text() + "分區盤點資料？", CallSendToFTP, DummyFunction);
//    };
////#endregion

////#region 返回
//    let afterRtnclick = function () {
//        if (EditMode == "A" || EditMode == "M") {
//            EditMode = "Q";
//            BtnSet(EditMode);
//        }
//    };

//    let btRtn_click = function () {
//        ChkLogOut(sessionStorage.getItem('isamcomp'));

//        $('#btAdd').prop('disabled', false);
//        document.getElementById("btAdd").style.background = "blue";
//        $('#btMod').prop('disabled', false);
//        document.getElementById("btMod").style.background = "green";
//        $('#btToFTP').prop('disabled', false);
//        document.getElementById("btToFTP").style.background = "gold";


//        if (EditMode == "Q") {
//            $('#ISAM01btns').hide();
//            $('#pgISAM01Init').show();
//            $('#pgISAM01Add').hide();
//            $('#pgISAM01Mod').hide();
//            //$('#pgISAM01UpFtp').hide();
//        } else if (EditMode == "A" || EditMode == "M") {
//            $('#ISAM01btns').show();
//            $('#pgISAM01Init').hide();
//            $('#pgISAM01Add').hide();
//            $('#pgISAM01Mod').hide();
//            //$('#pgISAM01UpFtp').hide();
//        }
//        Timerset(sessionStorage.getItem('isamcomp'));
//    };
////#endregion

////#region 輸入盤點日期,分區
//    let afterSearchBINWeb = function (data) {
//        //EditMode = "Q";
//        if (ReturnMsg(data, 0) != "SearchBINWebOK") {
//            DyAlert(ReturnMsg(data, 1));
//        }
//        else {
//            var dtBINData = data.getElementsByTagName('dtBINData');
//            if (dtBINData.length > 0) {
//                if (GetNodeValue(dtBINData[0], "BINman") != $('#lblManID1').html().split(' ')[0]) {
//                    DyAlert("盤點人員" + GetNodeValue(dtBINData[0], "BINman") + "已建立分區" + $('#txtBinNo').val() + "之分區單!!", DummyFunction);
//                    return;
//                }
//            }

//            //AssignVar();
//            //alert(GetNodeValue(dtISAMShop[0], "STName"));
//            EditMode = "Q";
//            $('#lblShop2').html($('#lblShop1').html());
//            $('#lblBINNo2').html($('#txtBinNo').val());
//            $('#lblDate2').html($('#txtISAMDate').val());
//            $('#lblSWQty1title').html($('#lblShop1').html().split(' ')[0] + "門市總數：");
//            $('#lblSBQty1title').html($('#txtBinNo').val() + "分區總數：");
//            $('#lblSQty1').html('');
//            $('#lblSQty1title').html($('#txtBinNo').val() + "分區單品總數：");
//            $('#lblSBQty1').html('');
//            $('#lblSWQty1').html('');
//            $('#lblPrice').html('');
//            $('#lblGDName').html("品名XXX");
//            $('#pgISAM01Init').hide();
//            if ($('#ISAM01btns').attr('hidden') == undefined) {
//                $('#ISAM01btns').show();
//            }
//            else {
//                $('#ISAM01btns').removeAttr('hidden');
//            }
//        }
//    };

//    let btSave_click = function () {
//        //*
//        ChkLogOut(sessionStorage.getItem('isamcomp'));
//        Timerset(sessionStorage.getItem('isamcomp'));
//        if ($('#txtISAMDate').val() == "" | $('#txtISAMDate').val() == null) {
//            DyAlert("請輸入盤點日期!!", function () { $('#txtISAMDate').focus() });
//            $(".modal-backdrop").remove();
//            return;
//        }
//        if ($('#txtBinNo').val() == "" | $('#txtBinNo').val() == null) {
//            DyAlert("請輸入分區代碼!!", function () { $('#txtBinNo').focus() });
//            $(".modal-backdrop").remove();
//            return;
//        }
//        else {
//            if ($('#txtBinNo').val().length>3) {
//                DyAlert("分區代碼不可超過3個字元!!", function () { $('#txtBinNo').focus() });
//                $(".modal-backdrop").remove();
//                return;
//            }
//        }
//        var pData = {
//            Shop: $('#lblShop1').html().split(' ')[0],
//            ISAMDate: $('#txtISAMDate').val(),
//            BinNo: $('#txtBinNo').val()
//        }
//        PostToWebApi({ url: "api/SystemSetup/SearchBINWeb", data: pData, success: afterSearchBINWeb });
//    };
//    //#endregion



/////////////////////////////////////////////////
    let btUPPic1_click = function (bt) {
        InitFileUpload(bt);
        var UploadFileType = "";
        if (bt.id == "btUPPic1_A") {
            UploadFileType = "PLU+Pic1"
        }
        if (bt.id == "btUPPic2_A") {
            UploadFileType = "PLU+Pic2"
        }
        if (bt.id == "btUPPic3_A") {
            UploadFileType = "PLU+Pic3"
        }
        if (bt.id == "btUPPic4_A") {
            UploadFileType = "PLU+Pic4"
        }
        if (bt.id == "btUPPic5_A") {
            UploadFileType = "PLU+Pic5"
        }
        if (bt.id == "btUPPic6_A") {
            UploadFileType = "PLU+Pic6"
        }
        if (bt.id == "btUPPic7_A") {
            UploadFileType = "PLU+Pic7"
        }
        if (bt.id == "btUPPic8_A") {
            UploadFileType = "PLU+Pic8"
        }
        $('#modal-media').prop("UploadFileType", UploadFileType);
        $('#fileURL').val('')
        $('#modal-media').modal('show');

    };

    let InitFileUpload = function (bt) {
        $('#fileupload').fileupload({
            dataType: 'xml',
            url: "api/FileUpload",
            dropZone: $('#dropzone'),
            headers: { "Authorization": "Bearer " + UU }
        });

        $('#fileupload').bind('fileuploadfail',
            function (e, data) {
            }
        );

        $('#fileupload').bind('fileuploadsubmit', function (e, data) {
            var Type = bt.id.split('_')[1]
            var DocNo = ""
            if (Type == "A") {
                DocNo = $('#lblDocNo_DMAdd_A').html();
            }
            data.formData = {
                "UploadFileType": $('#modal-media').prop("UploadFileType"),
                "ImgSGID": $('#' + $('#modal-media').prop("FieldName")).val(),
                "Type": Type,
                "DocNo": DocNo,
                "fileURL": $('#fileURL').val()
            };
        });

        $('#fileupload').bind('fileuploadalways', function (e, data) {
            AfterFileUpoad(data, bt);
        });

    };

    let AfterFileUpoad = function (returndata, bt) {
        var data = returndata.jqXHR.responseXML;
        if (ReturnMsg(data, 0) != "FileUploadOK") {
            DyAlert(ReturnMsg(data, 0));
        }
        else {
            $('#modal-media').modal('hide');
            var UploadFileType = $('#modal-media').prop("UploadFileType");// "PLU+Pic1";
            var Type = bt.id.split('_')[1]

            if (UploadFileType == "PLU+Pic1") {
                if (Type == "A") {
                    GetGetImage("PLUPic1_A", ReturnMsg(data, 1));
                    $('#Photo1_A').val(ReturnMsg(data, 1));
                }
            }
            if (UploadFileType == "PLU+Pic2") {
                if (Type == "A") {
                    GetGetImage("PLUPic2_A", ReturnMsg(data, 1));
                    $('#Photo2_A').val(ReturnMsg(data, 1));
                }
            }
            if (UploadFileType == "PLU+Pic3") {
                if (Type == "A") {
                    GetGetImage("PLUPic3_A", ReturnMsg(data, 1));
                    $('#Photo3_A').val(ReturnMsg(data, 1));
                }
            }
            if (UploadFileType == "PLU+Pic4") {
                if (Type == "A") {
                    GetGetImage("PLUPic4_A", ReturnMsg(data, 1));
                    $('#Photo4_A').val(ReturnMsg(data, 1));
                }
            }
            if (UploadFileType == "PLU+Pic5") {
                if (Type == "A") {
                    GetGetImage("PLUPic5_A", ReturnMsg(data, 1));
                    $('#Photo5_A').val(ReturnMsg(data, 1));
                }
            }
            if (UploadFileType == "PLU+Pic6") {
                if (Type == "A") {
                    GetGetImage("PLUPic6_A", ReturnMsg(data, 1));
                    $('#Photo6_A').val(ReturnMsg(data, 1));
                }
            }
            if (UploadFileType == "PLU+Pic7") {
                if (Type == "A") {
                    GetGetImage("PLUPic7_A", ReturnMsg(data, 1));
                    $('#Photo7_A').val(ReturnMsg(data, 1));
                }
            }
            if (UploadFileType == "PLU+Pic8") {
                if (Type == "A") {
                    GetGetImage("PLUPic8_A", ReturnMsg(data, 1));
                    $('#Photo8_A').val(ReturnMsg(data, 1));
                }
            }
            $('#modal-media').prop("UploadFileType", UploadFileType);
        }

    };

    let GetGetImage = function (elmImg, picSGID) {
        if (picSGID == "") {
            $('#' + elmImg).prop('src', "");
            return;
        }
        var url = "api/GetImage?SGID=" + EncodeSGID(picSGID) + "&UU=" + encodeURIComponent(UU);
        url += "&Ver=" + encodeURIComponent(new Date().toLocaleTimeString());
        $('#' + elmImg).prop('src', url);
    }

    let btDMAdd_click = function (bt) {
        if ($('#cboType').val() == null) {
            DyAlert("請選擇版型!")
            return;
        }
        else if ($('#cboType').val() == "A") {
            for (var i = 0; i < 8; i++) {
                GetGetImage("PLUPic" + String(i + 1) + "_A", "");
            }
            $('#Barcode1_A').val('');

            $('#titDMAdd_A').text('版型A新增');
            $('#modal_DMAdd_A').modal('show');
            setTimeout(function () {
                GetDocNoDM();
            }, 500);
        }
    };

    let btPrint_DMAdd_A_click = function (bt) {
        GetGetImage("PLUPic1_DM_A", "");
        GetGetImage("PLUPic2_DM_A", "");
        GetGetImage("PLUPic3_DM_A", "");
        GetGetImage("PLUPic4_DM_A", "");
        GetGetImage("PLUPic5_DM_A", "");
        GetGetImage("PLUPic6_DM_A", "");
        GetGetImage("PLUPic7_DM_A", "");
        GetGetImage("PLUPic8_DM_A", "");
        GetGetImage("Barcode1_DM_A", "");

        $('#modal_DM_A').modal('show');
        setTimeout(function () {
            Print_DM_A();
        }, 500);
    };

    let Print_DM_A = function () {
        var pData = {
            DocNo: $('#lblDocNo_DMAdd_A').html()
        }
        PostToWebApi({ url: "api/SystemSetup/Print_DM_A", data: pData, success: afterPrint_DM_A });
    };

    let afterPrint_DM_A = function (data) {
        if (ReturnMsg(data, 0) != "Print_DM_AOK") {
            DyAlert(ReturnMsg(data, 1), function () {
                $('#modal_DM_A').modal('hide');
            });
        }
        else {
            var dtE = data.getElementsByTagName('dtE');
            if (dtE.length == 0) {
                DyAlert("請設定DM資料!", function () {
                    $('#modal_DM_A').modal('hide');
                });
                $(".modal-backdrop").remove();
                return;
            }
            for (var i = 0; i < dtE.length; i++) {
                if (GetNodeValue(dtE[i], "DataType") == "PLU+Pic1") {
                    GetGetImage("PLUPic1_DM_A", GetNodeValue(dtE[i], "SGID"));
                }
                else if (GetNodeValue(dtE[i], "DataType") == "PLU+Pic2") {
                    GetGetImage("PLUPic2_DM_A", GetNodeValue(dtE[i], "SGID"));
                }
                else if (GetNodeValue(dtE[i], "DataType") == "PLU+Pic3") {
                    GetGetImage("PLUPic3_DM_A", GetNodeValue(dtE[i], "SGID"));
                }
                else if (GetNodeValue(dtE[i], "DataType") == "PLU+Pic4") {
                    GetGetImage("PLUPic4_DM_A", GetNodeValue(dtE[i], "SGID"));
                }
                else if (GetNodeValue(dtE[i], "DataType") == "PLU+Pic5") {
                    GetGetImage("PLUPic5_DM_A", GetNodeValue(dtE[i], "SGID"));
                }
                else if (GetNodeValue(dtE[i], "DataType") == "PLU+Pic6") {
                    GetGetImage("PLUPic6_DM_A", GetNodeValue(dtE[i], "SGID"));
                }
                else if (GetNodeValue(dtE[i], "DataType") == "PLU+Pic7") {
                    GetGetImage("PLUPic7_DM_A", GetNodeValue(dtE[i], "SGID"));
                }
                else if (GetNodeValue(dtE[i], "DataType") == "PLU+Pic8") {
                    GetGetImage("PLUPic8_DM_A", GetNodeValue(dtE[i], "SGID"));
                }
                else if (GetNodeValue(dtE[i], "DataType") == "Barcode1") {
                    GetGetImage("Barcode1_DM_A", GetNodeValue(dtE[i], "SGID"));
                }
            }
        }
    };

    let btExit_DMAdd_A_click = function (bt) {
        $('#modal_DMAdd_A').modal('hide');
    };

    let btExit_DM_A_click = function (bt) {
        $('#modal_DM_A').modal('hide');
    };

    let DMQuery1_click = function (bt) {
        ShowLoading();
        var pData = {
            DocNo: $('#txtDocNo').val(),
            Type: $('#cboType').val()
        }
        PostToWebApi({ url: "api/SystemSetup/DMQuery1", data: pData, success: afterDMQuery1 });
    };

    let afterDMQuery1 = function (data) {
        CloseLoading();

        if (ReturnMsg(data, 0) != "DMQuery1OK") {
            DyAlert(ReturnMsg(data, 1));
        }
        else {
            var dtE = data.getElementsByTagName('dtE');
            grdM.BindData(dtE);

            if (dtE.length == 0) {
                DyAlert("無符合資料!");
                $(".modal-backdrop").remove();
                return;
            }
        }
    };

    let GetDocNoDM = function () {
        var pData = {
        }
        PostToWebApi({ url: "api/SystemSetup/GetDocNoDM", data: pData, success: afterGetDocNoDM });
    };

    let afterGetDocNoDM = function (data) {
        if (ReturnMsg(data, 0) != "GetDocNoDMOK") {
            DyAlert(ReturnMsg(data, 1), function () {
                $('#modal_DMAdd_A').modal('hide');
            });
        }
        else {
            var dtE = data.getElementsByTagName('dtE');
            $('#lblDocNo_DMAdd_A').html(GetNodeValue(dtE[0], "DocNo"));
        }
    };

    let btBarcode1_A_click = function (bt) {
        var pData = {
            DocNo: $('#lblDocNo_DMAdd_A').html(),
            Barcode1: $('#Barcode1_A').val()
        }
        PostToWebApi({ url: "api/SystemSetup/SetBarcode1_A", data: pData, success: afterSetBarcode1_A });
    };

    let afterSetBarcode1_A = function (data) {
        if (ReturnMsg(data, 0) != "SetBarcode1_AOK") {
            DyAlert(ReturnMsg(data, 1));
        }
        else {
            DyAlert("設定完成!")
        }
    };



    let GetInitmsDM = function (data) {
        if (ReturnMsg(data, 0) != "GetInitmsDMOK") {
            DyAlert(ReturnMsg(data, 1));
        }
        else {
            var dtE = data.getElementsByTagName('dtE');
            AssignVar();

            $('#DMQuery1').click(function () { DMQuery1_click(this) });
            $('#btDMAdd').click(function () { btDMAdd_click(this) });
            $('#btExit_DMAdd_A').click(function () { btExit_DMAdd_A_click(this) });
            $('#btPrint_DMAdd_A').click(function () { btPrint_DMAdd_A_click(this) });
            $('#btExit_DM_A').click(function () { btExit_DM_A_click(this) });

            $('#btUPPic1_A').click(function () { btUPPic1_click(this) });
            $('#btUPPic2_A').click(function () { btUPPic1_click(this) });
            $('#btUPPic3_A').click(function () { btUPPic1_click(this) });
            $('#btUPPic4_A').click(function () { btUPPic1_click(this) });
            $('#btUPPic5_A').click(function () { btUPPic1_click(this) });
            $('#btUPPic6_A').click(function () { btUPPic1_click(this) });
            $('#btUPPic7_A').click(function () { btUPPic1_click(this) });
            $('#btUPPic8_A').click(function () { btUPPic1_click(this) });


            $('#btBarcode1_A').click(function () { btBarcode1_A_click(this) });

        }
    };

    let afterLoadPage = function () {
        PostToWebApi({ url: "api/SystemSetup/GetInitmsDM", success: GetInitmsDM });
    };

    if ($('#pgmsDM101').length == 0) {  
        AllPages = new LoadAllPages(ParentNode, "SystemSetup/msDM101", ["msDM101btns", "pgmsDM101Init", "pgmsDM101Add", "pgmsDM101Mod"], afterLoadPage);
    };


}