var PageMSSD106 = function (ParentNode) {

    let grdM;
    let grdMW;
    let grdLookUp_VIPFaceID;
    let grdLookUp_City;

    let EditMode = "";
    let DelPLU = "";
    let DelPLUQty;
    let ModPLU = "";
    let ModPLUQty;
    let chkVIPFaceID = "";
    let chkCity = "";

    let AssignVar = function () {

        grdM = new DynGrid(
            {
                table_lement: $('#tbQuery')[0],
                class_collection: ["tdCol1 text-center", "tdCol2 label-align", "tdCol3 label-align", "tdCol4 text-center", "tdCol5 label-align", "tdCol6 text-center", "tdCol7 label-align", "tdCol8 text-center", "tdCol9 label-align", "tdCol10 text-center", "tdCol11 label-align", "tdCol12 text-center", "tdCol13 label-align", "tdCol14 text-center", "tdCol15 label-align", "tdCol16 text-center", "tdCol17 label-align", "tdCol18 text-center", "tdCol19 label-align", "tdCol20 text-center"],
                fields_info: [
                    { type: "Text", name: "ID", style: "" },
                    { type: "TextAmt", name: "Cnt1"},
                    { type: "TextAmt", name: "Cnt2" },
                    { type: "TextAmt", name: "Cnt2p" },
                    { type: "TextAmt", name: "Cnt3" },
                    { type: "TextAmt", name: "Cnt3p" },
                    { type: "TextAmt", name: "Cnt4" },
                    { type: "TextAmt", name: "Cnt4p" },
                    { type: "TextAmt", name: "Cnt5" },
                    { type: "TextAmt", name: "Cnt5p" },
                    { type: "TextAmt", name: "Cnt6" },
                    { type: "TextAmt", name: "Cnt6p" },
                    { type: "TextAmt", name: "Cnt7" },
                    { type: "TextAmt", name: "Cnt7p" },
                    { type: "TextAmt", name: "Cnt8" },
                    { type: "TextAmt", name: "Cnt8p" },
                    { type: "TextAmt", name: "Cnt9" },
                    { type: "TextAmt", name: "Cnt9p" },
                    { type: "TextAmt", name: "Cnt10" },
                    { type: "TextAmt", name: "Cnt10p" }
                ],
                //rows_per_page: 10,
                method_clickrow: click_PLU,
                afterBind: InitModifyDeleteButton,
                sortable: "N"
            }
        );

        grdMW = new DynGrid(
            {
                table_lement: $('#tbQueryMW')[0],
                class_collection: ["tdCol1 text-center", "tdCol2 label-align", "tdCol3 label-align", "tdCol4 text-center", "tdCol5 label-align", "tdCol6 text-center", "tdCol7 label-align", "tdCol8 text-center", "tdCol9 label-align", "tdCol10 text-center", "tdCol11 label-align", "tdCol12 text-center", "tdCol13 label-align", "tdCol14 text-center", "tdCol15 label-align", "tdCol16 text-center"],
                fields_info: [
                    { type: "Text", name: "ID", style: "" },
                    { type: "TextAmt", name: "Cnt1" },
                    { type: "TextAmt", name: "Cnt2" },
                    { type: "TextAmt", name: "Cnt2p" },
                    { type: "TextAmt", name: "Cnt3" },
                    { type: "TextAmt", name: "Cnt3p" },
                    { type: "TextAmt", name: "Cnt4" },
                    { type: "TextAmt", name: "Cnt4p" },
                    { type: "TextAmt", name: "Cnt5" },
                    { type: "TextAmt", name: "Cnt5p" },
                    { type: "TextAmt", name: "Cnt6" },
                    { type: "TextAmt", name: "Cnt6p" },
                    { type: "TextAmt", name: "Cnt7" },
                    { type: "TextAmt", name: "Cnt7p" },
                    { type: "TextAmt", name: "Cnt8" },
                    { type: "TextAmt", name: "Cnt8p" }
                ],
                //rows_per_page: 10,
                method_clickrow: click_PLU,
                afterBind: InitModifyDeleteButton,
                sortable: "N"
            }
        );

        grdLookUp_VIPFaceID = new DynGrid(
            {
                table_lement: $('#tbLookup_VIPFaceID')[0],
                class_collection: ["tdCol1 text-center", "tdCol2", "tdCol3"],
                fields_info: [
                    { type: "checkbox", name: "chkset", style: "width:16px;height:16px" },
                    { type: "Text", name: "ST_ID", style: "" },
                    { type: "Text", name: "ST_SName", style: "" }
                ],
                //rows_per_page: 10,
                method_clickrow: click_PLU,
                afterBind: InitModifyDeleteButton,
                sortable: "N"
            }
        );

        grdLookUp_City = new DynGrid(
            {
                table_lement: $('#tbLookup_City')[0],
                class_collection: ["tdCol1 text-center", "tdCol2"],
                fields_info: [
                    { type: "checkbox", name: "chkset", style: "width:16px;height:16px" },
                    { type: "Text", name: "City", style: "" }
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
        //$('#tbISAM01Mod .fa-trash-o').click(function () { btPLUDelete_click(this) });
    }

    let ChkLogOut_1 = function (AfterChkLogOut_1) {
        var LoginDT = sessionStorage.getItem('LoginDT');
        var cData = {
            LoginDT: LoginDT
        }
        PostToWebApi({ url: "api/js/ChkLogOut", data: cData, success: AfterChkLogOut_1 });
    };

//#region 編查
//#region 單筆修改
    let AfterSaveISAM01PLUMod = function (data) {
        if (ReturnMsg(data, 0) != "SaveISAM01PLUModOK") {
            DyAlert(ReturnMsg(data, 1));
        }
        else {
            DyAlert("修改完成!");

            $('#modal_ISAM01PLUMod').modal('hide');
            var dtBINMod = data.getElementsByTagName('dtBINMod')[0];
            grdM.RefreshRocord(grdM.ActiveRowTR(), dtBINMod);
        }

    };

    let btModSave_click = function () {
        ChkLogOut_1(btModSave_click_1);
    };

    let btModSave_click_1 = function (data) {
        if (ReturnMsg(data, 0) != "ChkLogOutOK") {
            DyAlert(ReturnMsg(data, 1));
        }
        else {
            var dtLogin = data.getElementsByTagName('dtLogin');
            if (GetNodeValue(dtLogin[0], "LogOutType") != "") {
                window.location.href = "Login" + sessionStorage.getItem('isamcomp');
            }
            else {
Timerset(sessionStorage.getItem('isamcomp'));
        if ($('#txtModQty1').val() == "") {
            DyAlert("請輸入數量!");
            return;
        }
        else {

            if (isNaN($('#txtModQty1').val())) {
                DyAlert("請輸入數字!");
                return;
            }
            if ($('#txtModQty1').val().indexOf(".") > 0) {
                DyAlert("請輸入整數!");
                return;
            }

            if ($('#txtModQty1').val() <= 0) {
                DyAlert("數量需大於0!");
                return;
            }

            if ($('#txtModQty1').val() > 999999) {
                DyAlert("數量不可大於999999!");
                return;
            }
        }
        var cData = {
            Shop: $('#lblShop2').html().split(' ')[0],
            ISAMDate: $('#lblDate2').html(),
            BinNo: $('#lblBINNo2').html(),
            PLU: ModPLU,
            Qty: $('#txtModQty1').val()
        }
        PostToWebApi({ url: "api/SystemSetup/SaveISAM01PLUMod", data: cData, success: AfterSaveISAM01PLUMod });
            }
        }
    }

    let btModCancel_click = function () {
        //*
        ChkLogOut(sessionStorage.getItem('isamcomp'));
        Timerset(sessionStorage.getItem('isamcomp'));
        $('#modal_ISAM01PLUMod').modal('hide');
    };

    let AfterGetModGDName = function (data) {
        if (ReturnMsg(data, 0) != "GetGDNameOK") {
            DyAlert(ReturnMsg(data, 1));
        }
        else {
            var dtP = data.getElementsByTagName('dtPLU');
            //alert(dtP.length);
            $('#lblModPLU').html(ModPLU);
            $('#lblModQty1').html(ModPLUQty);
            $('#txtModQty1').val(ModPLUQty);
            if (dtP.length > 0) {
                $('#lblModPLUName').html(GetNodeValue(dtP[0], 'GD_Name'));
            }
            else {
                //DyAlert("無符合之商品資料!");
                //return;
                $('#lblModPLUName').html('');
            }
            $('#modal_ISAM01PLUMod').modal('show');
        }

        $('.msg-valid').hide();
    };

    let btPLUMod_click = function (bt) {
        //*
        ChkLogOut(sessionStorage.getItem('isamcomp'));
        Timerset(sessionStorage.getItem('isamcomp'));
        $(bt).closest('tr').click();
        //alert(GetNodeValue(node, 'AppDate'));


        $('.msg-valid').hide();
        $('#modal_ISAM01PLUMod .modal-title').text('盤點資料單筆修改');
        //$('#modal_ISAM01Mod .btn-danger').text('刪除');
        var node = $(grdM.ActiveRowTR()).prop('Record');
        ModPLU = GetNodeValue(node, 'PLU');
        ModPLUQty = GetNodeValue(node, 'Qty1');
        var cData = {
            Shop: $('#lblShop2').html().split(' ')[0],
            ISAMDate: $('#lblDate2').html(),
            BinNo: $('#lblBINNo2').html(),
            PLU: ModPLU
        }
        PostToWebApi({ url: "api/SystemSetup/GetGDName", data: cData, success: AfterGetModGDName });

    };
//#endregion

//#region 單筆刪除
    let AfterDelISAM01PLU = function (data) {
        if (ReturnMsg(data, 0) != "DelISAM01PLUOK") {
            DyAlert(ReturnMsg(data, 1));
        }
        else {
            DyAlert("刪除完成!");

            $('#modal_ISAM01PLUDel').modal('hide');
            //var userxml = data.getElementsByTagName('dtRack')[0];
            grdM.DeleteRow(grdM.ActiveRowTR());
        }

    };

    let btDelSave_click = function () {
        ChkLogOut_1(btDelSave_click_1);
    };

    let btDelSave_click_1 = function (data) {
        if (ReturnMsg(data, 0) != "ChkLogOutOK") {
            DyAlert(ReturnMsg(data, 1));
        }
        else {
            var dtLogin = data.getElementsByTagName('dtLogin');
            if (GetNodeValue(dtLogin[0], "LogOutType") != "") {
                window.location.href = "Login" + sessionStorage.getItem('isamcomp');
            }
            else {
                Timerset(sessionStorage.getItem('isamcomp'));
                var cData = {
                    Shop: $('#lblShop2').html().split(' ')[0],
                    ISAMDate: $('#lblDate2').html(),
                    BinNo: $('#lblBINNo2').html(),
                    PLU: DelPLU
                }
                PostToWebApi({ url: "api/SystemSetup/DelISAM01PLU", data: cData, success: AfterDelISAM01PLU });
            }
        }
    }

    let btDelCancel_click = function () {
        //*
        ChkLogOut(sessionStorage.getItem('isamcomp'))
        Timerset(sessionStorage.getItem('isamcomp'));
        $('#modal_ISAM01PLUDel').modal('hide');
    };

    let AfterGetGDName = function (data) {
        if (ReturnMsg(data, 0) != "GetGDNameOK") {
            DyAlert(ReturnMsg(data, 1));
        }
        else {
            var dtP = data.getElementsByTagName('dtPLU');
            //alert(dtP.length);
            $('#lblPLU').html(DelPLU);
            $('#lblDelQty1').html(DelPLUQty);
            if (dtP.length > 0) {
                /*DyConfirm("確定要刪除商品" + GetNodeValue(dtP[0], 'PLU') + GetNodeValue(dtP[0], 'GD_Name') + "？", afterDelPLU(GetNodeValue(dtP[0], 'PLU')), DummyFunction);*/
                $('#lblPLUName').html(GetNodeValue(dtP[0], 'GD_Name'));
            }
            else {
                $('#lblPLUName').html('');
             }
            $('#modal_ISAM01PLUDel').modal('show');
        }

        $('.msg-valid').hide();
    };

    let btPLUDelete_click = function (bt) {
        //*
        ChkLogOut(sessionStorage.getItem('isamcomp'));
        Timerset(sessionStorage.getItem('isamcomp'));
        $(bt).closest('tr').click();
        $('.msg-valid').hide();
        $('#modal_ISAM01PLUDel .modal-title').text('盤點資料單筆刪除');
        //$('#modal_ISAM01Mod .btn-danger').text('刪除');
        var node = $(grdM.ActiveRowTR()).prop('Record');
        DelPLU = GetNodeValue(node, 'PLU');
        DelPLUQty = GetNodeValue(node, 'Qty1');
        var cData = {
            Shop: $('#lblShop2').html().split(' ')[0],
            ISAMDate: $('#lblDate2').html(),
            BinNo: $('#lblBINNo2').html(),
            PLU: DelPLU
        }
        PostToWebApi({ url: "api/SystemSetup/GetGDName", data: cData, success: AfterGetGDName });

    };
//#endregion

    let txtBarcode3_ini = function () {
        $('#txtBarcode3').val('');
        $('#txtBarcode3').focus();
    }

    let afterGetBINWebMod = function (data) {
        if (ReturnMsg(data, 0) != "GetBINWebModOK") {
            DyAlert(ReturnMsg(data, 1), txtBarcode3_ini);
            
        }
        else {
            var dtBin = data.getElementsByTagName('dtBin');
            grdM.BindData(dtBin);
            if (dtBin.length == 0) {
                //alert("No RowData");
                DyAlert("無符合資料!", txtBarcode3_ini);
                $(".modal-backdrop").remove();
                return;
            }
            txtBarcode3_ini()
        }

    };

//#endregion

//#region 新增
    let btQtySave1_click = function () {
        ChkLogOut_1(btQtySave1_click_1)
    };

    let btQtySave1_click_1 = function (data) {
        if (ReturnMsg(data, 0) != "ChkLogOutOK") {
            DyAlert(ReturnMsg(data, 1));
        }
        else {
            var dtLogin = data.getElementsByTagName('dtLogin');
            if (GetNodeValue(dtLogin[0], "LogOutType") != "") {
                window.location.href = "Login" + sessionStorage.getItem('isamcomp');
            }
            else {
                Timerset(sessionStorage.getItem('isamcomp'));
                if ($('#txtBarcode1').val() == "" && $('#lblBarcode').html() == "") {
                    DyAlert("請輸入條碼!");
                    $('#txtQty1').val('');
                    //$('#btKeyin1').prop('disabled', false);
                    //$('#btBCSave1').prop('disabled', false);
                    //$('#txtBarcode1').prop('disabled', false);
                    //$('#btQtySave1').prop('disabled', true);
                    //$('#txtQty1').prop('disabled', true);
                    return;
                }
                if ($('#txtQty1').val() != "") {
                    if (isNaN($('#txtQty1').val())) {
                        DyAlert("請輸入數字!");
                        return;
                    }
                    if ($('#txtQty1').val().indexOf(".") > 0) {
                        DyAlert("請輸入整數!");
                        return;
                    }

                    if ($('#txtQty1').val() <= 0) {
                        DyAlert("數量需大於0!");
                        return;
                    }
                }
                else {
                    $('#txtQty1').val() == "1"
                }
               
                //$('#btKeyin1').prop('disabled', false);
                //$('#btBCSave1').prop('disabled', false);
                //$('#txtBarcode1').prop('disabled', false);
                //$('#btQtySave1').prop('disabled', true);
                //$('#txtQty1').prop('disabled', true);
                var pData = {
                    Shop: $('#lblShop2').html().split(' ')[0],
                    ISAMDate: $('#lblDate2').html(),
                    BinNo: $('#lblBINNo2').html(),
                    Barcode: $('#txtBarcode1').val() == "" ? $('#lblBarcode').html() : $('#txtBarcode1').val(),
                    Qty: $('#txtQty1').val()
                };
                PostToWebApi({ url: "api/SystemSetup/SaveBINWeb", data: pData, success: afterSaveBINWeb });
            }
        }
    }

    let btKeyin1_click = function () {
        //*
        ChkLogOut(sessionStorage.getItem('isamcomp'))
        Timerset(sessionStorage.getItem('isamcomp'));
        //$('#btKeyin1').prop('disabled', true);
        //$('#btQtySave1').prop('disabled', false);
        //$('#txtQty1').prop('disabled', false);
        //$('#btBCSave1').prop('disabled', true);
        //$('#txtBarcode1').prop('disabled', true);
    };

    let afterSaveBINWeb = function (data) {
        if (ReturnMsg(data, 0) != "SaveBINWebOK") {
            DyAlert(ReturnMsg(data, 1),txtBarcode1_ini);
        }
        else {
            var dtSQ = data.getElementsByTagName('dtSQ');
            if (dtSQ.length > 0) {
                $('#lblSQty1').html(GetNodeValue(dtSQ[0], "SQ1"));
            }
            else {
                $('#lblSQty1').html('');
            }
            var dtSBQ = data.getElementsByTagName('dtSBQ');
            if (dtSBQ.length > 0) {
                $('#lblSBQty1').html(GetNodeValue(dtSBQ[0], "SBQ1"));
            }
            else {
                $('#lblSBQty1').html('');
            }
            var dtSWQ = data.getElementsByTagName('dtSWQ');
            if (dtSWQ.length > 0) {
                $('#lblSWQty1').html(GetNodeValue(dtSWQ[0], "SWQ1"));
            }
            else {
                $('#lblSWQty1').html('');
            }

            var dtP = data.getElementsByTagName('dtPLU');
            if (dtP.length > 0) {
                
                $('#lblBarcode').html(GetNodeValue(dtP[0], "PLU"));
                $('#txtBarcode1').val('');
                $('#lblQty1').html($('#txtQty1').val());
                $('#lblPrice').html(parseInt(GetNodeValue(dtP[0], "GD_Retail")));
                $('#lblGDName').html(GetNodeValue(dtP[0], "GD_Name"));
                $('#txtQty1').val("");
            }
            else {
                
                if ($('#txtBarcode1').val() == "") {
                }
                else {
                    $('#lblBarcode').html($('#txtBarcode1').val());
                }
                $('#txtBarcode1').val('');
                $('#lblQty1').html($('#txtQty1').val());
                $('#lblPrice').html('');
                $('#lblGDName').html('');
                $('#txtQty1').val("");
            }
            //alert(dtBin.length);
            //if (dtBin.length == 0) {
            //    alert("No RowData");
            //    DyAlert("無符合資料!", BlankMode);
            //    return;
            //}
        }
    };

    let txtBarcode1_ini = function () {
        $('#txtBarcode1').val('');
        $('#txtBarcode1').focus();
    }

    let txtQty1_ini = function () {
        $('#txtQty1').val('');
        $('#txtQty1').focus();
    }

    let btBCSave1_click = function () {
        ChkLogOut_1(btBCSave1_click_1)
    };

    let btBCSave1_click_1 = function (data) {

        if (ReturnMsg(data, 0) != "ChkLogOutOK") {
            DyAlert(ReturnMsg(data, 1), txtBarcode1_ini);
        }
        else {
            var dtLogin = data.getElementsByTagName('dtLogin');
            if (GetNodeValue(dtLogin[0], "LogOutType") != "") {
                window.location.href = "Login" + sessionStorage.getItem('isamcomp');
            }
            else {
Timerset(sessionStorage.getItem('isamcomp'));
            if ($('#txtBarcode1').val() == "") {
                DyAlert("請輸入條碼!", txtBarcode1_ini);
                $(".modal-backdrop").remove();
                return;
                }

            if ($('#txtBarcode1').val().length > 16) {
                DyAlert("條碼限制輸入16個字元!", txtBarcode1_ini);
                $(".modal-backdrop").remove();
                return;
                }

            if ($('#txtQty1').val() == "" || $('#txtQty1').val() == "0") {
                $('#txtQty1').val("1");
            }
            else {
                    if (isNaN($('#txtQty1').val())) {
                        DyAlert("請輸入數字!", txtQty1_ini );
                        return;
                    }
                    if ($('#txtQty1').val().indexOf(".") > 0) {
                        DyAlert("請輸入整數!", txtQty1_ini);
                        return;
                    }
                    //if ($('#txtQty1').val() <= 0) {
                    //    DyAlert("數量需大於0!", txtBarcode1_ini);
                    //    return;
                    //}
                    if ($('#txtQty1').val() > 9999 || $('#txtQty1').val() < -9999) {
                        DyAlert("數量需介於-9999~9999之間!", txtQty1_ini);
                        return;
                    }
                }
                var pData = {
                    Shop: $('#lblShop2').html().split(' ')[0],
                    ISAMDate: $('#lblDate2').html(),
                    BinNo: $('#lblBINNo2').html(),
                    Barcode: $('#txtBarcode1').val(),
                    Qty: $('#txtQty1').val()
                };
                PostToWebApi({ url: "api/SystemSetup/ChkSaveBINWeb", data: pData, success: afterChkSaveBINWeb });
            }
        }
    }

    let afterChkSaveBINWeb = function (data) {
        if (ReturnMsg(data, 0) != "ChkSaveBINWebOK") {
            DyAlert(ReturnMsg(data, 1));
        }
        else {
            var dtBIN = data.getElementsByTagName('dtBIN');
            if (dtBIN.length == 0) {
                if ($('#txtQty1').val() < 0) {
                    DyAlert("單品總數需大於0!", txtQty1_ini);
                    return;
                }
            }
            else {
                if (parseInt(GetNodeValue(dtBIN[0], "SumQty")) + parseInt($('#txtQty1').val()) > 999999) {
                    DyAlert("單品總數不可大於999999!", txtQty1_ini);
                    return;
                }
                if (parseInt(GetNodeValue(dtBIN[0], "SumQty")) + parseInt($('#txtQty1').val()) < 0) {
                    DyAlert("單品總數需大於0!", txtQty1_ini);
                    return;
                }
                var pData = {
                    Shop: $('#lblShop2').html().split(' ')[0],
                    ISAMDate: $('#lblDate2').html(),
                    BinNo: $('#lblBINNo2').html(),
                    Barcode: $('#txtBarcode1').val(),
                    Qty: $('#txtQty1').val()
                };
                PostToWebApi({ url: "api/SystemSetup/SaveBINWeb", data: pData, success: afterSaveBINWeb });
            }
        }
        
    }
    
    let btAdd_click = function () {
        //*
        ChkLogOut(sessionStorage.getItem('isamcomp'));
        EditMode = "A";
        Timerset(sessionStorage.getItem('isamcomp'));
        //$('#btKeyin1').prop('disabled', false);
        //$('#btBCSave1').prop('disabled', false);
        //$('#txtBarcode1').prop('disabled', false);
        //$('#btQtySave1').prop('disabled', true);
        //$('#txtQty1').prop('disabled', true);
        $('#pgISAM01Add').show();
        if ($('#pgISAM01Add').attr('hidden') == undefined) {
            $('#pgISAM01Add').show();
        }
        else {
            $('#pgISAM01Add').removeAttr('hidden');
        }
        if ($('#pgISAM01Mod').attr('hidden') == undefined) {
            $('#pgISAM01Mod').hide();
        }
        BtnSet("A");
        
    };
//#endregion

    let BtnSet = function (edit) {
        switch (edit) {
            case "A":
                $('#btAdd').prop('disabled', false);
                document.getElementById("btAdd").style.background = "blue";
                $('#btMod').prop('disabled', true);
                document.getElementById("btMod").style.background = 'gray';
                $('#btToFTP').prop('disabled', true); //true-btn不能使用
                document.getElementById("btToFTP").style.background = 'gray';

                $('#txtBarcode1').val('');
                $('#txtBarcode1').focus();
                $('#txtQty1').val("");
                $('#lblBarcode').html('');
                $('#lblQty1').html('');
                $('#lblSQty1').html('');
                $('#lblSBQty1').html('');
                $('#lblSWQty1').html('');
                $('#lblPrice').html('');
                $('#lblGDName').html('');
                break;
            case "Q":
                $('#btAdd').prop('disabled', false);
                document.getElementById("btAdd").style.background = "blue";
                $('#btMod').prop('disabled', false);
                document.getElementById("btMod").style.background = "Green";
                $('#btToFTP').prop('disabled', false); //true-btn不能使用
                document.getElementById("btToFTP").style.background = "gold";
                break;
            case "M":
                $('#btAdd').prop('disabled', true);
                document.getElementById("btAdd").style.background = 'gray';
                $('#btMod').prop('disabled', false);
                document.getElementById("btMod").style.background = "Green";
                $('#btToFTP').prop('disabled', true); //true-btn不能使用
                document.getElementById("btToFTP").style.background = 'gray';
                $('#txtBarcode3').val('');
                break;
        }
    };

//#region 上傳
    let AfterAddISAMToFTPRecWeb = function (data) {
        if (ReturnMsg(data, 0) != "AddISAMToFTPRecWebOK") {
            if (ReturnMsg(data, 1) == "FTP") {
                DyAlert("FTP設定有誤，請重新確認!")
            }
            else if (ReturnMsg(data, 1) == "上傳記錄") {
                DyAlert("待上傳記錄新增失敗，請重新上傳!")
            }
            else if (ReturnMsg(data, 1) == "上傳資料") {
                DyAlert("無上傳資料，請重新確認!")
            }
            else if (ReturnMsg(data, 1) == "上傳檔案") {
                DyAlert("待上傳檔案不存在，請重新確認!")
            }
            else {
                DyAlert(ReturnMsg(data, 1));
            }
        }
        else {
            DyAlert("上傳成功!")
        }
    }

    let CallSendToFTP = function () {
        ChkLogOut_1(CallSendToFTP_1);
    };

    let CallSendToFTP_1 = function (data) {
        if (ReturnMsg(data, 0) != "ChkLogOutOK") {
            DyAlert(ReturnMsg(data, 1));
        }
        else {
            var dtLogin = data.getElementsByTagName('dtLogin');
            if (GetNodeValue(dtLogin[0], "LogOutType") != "") {
                window.location.href = "Login" + sessionStorage.getItem('isamcomp');
            }
            else {
                Timerset(sessionStorage.getItem('isamcomp'));
                var cData = {
                    Type: "T",
                    Shop: $('#lblShop2').html().split(' ')[0],
                    ISAMDate: $('#lblDate2').html(),
                    BinNo: $('#lblBINNo2').html()
                }
                PostToWebApi({ url: "api/SystemSetup/AddISAMToFTPRecWeb", data: cData, success: AfterAddISAMToFTPRecWeb });
            }
        }
    }


    let btToFTP_click = function () {
        //*
        ChkLogOut(sessionStorage.getItem('isamcomp'));
        Timerset(sessionStorage.getItem('isamcomp'));
        DyConfirm("是否要上傳" + $('#lblBINNo2').text() + "分區盤點資料？", CallSendToFTP, DummyFunction);
    };
//#endregion

//#region 返回
    let afterRtnclick = function () {
        if (EditMode == "A" || EditMode == "M") {
            EditMode = "Q";
            BtnSet(EditMode);
        }
    };

    let btRtn_click = function () {
        ChkLogOut(sessionStorage.getItem('isamcomp'));

        $('#btAdd').prop('disabled', false);
        document.getElementById("btAdd").style.background = "blue";
        $('#btMod').prop('disabled', false);
        document.getElementById("btMod").style.background = "green";
        $('#btToFTP').prop('disabled', false);
        document.getElementById("btToFTP").style.background = "gold";


        if (EditMode == "Q") {
            $('#ISAM01btns').hide();
            $('#pgISAM01Init').show();
            $('#pgISAM01Add').hide();
            $('#pgISAM01Mod').hide();
            //$('#pgISAM01UpFtp').hide();
        } else if (EditMode == "A" || EditMode == "M") {
            $('#ISAM01btns').show();
            $('#pgISAM01Init').hide();
            $('#pgISAM01Add').hide();
            $('#pgISAM01Mod').hide();
            //$('#pgISAM01UpFtp').hide();
        }
        Timerset(sessionStorage.getItem('isamcomp'));
    };
//#endregion

//#region 輸入盤點日期,分區
    let afterSearchBINWeb = function (data) {
        //EditMode = "Q";
        if (ReturnMsg(data, 0) != "SearchBINWebOK") {
            DyAlert(ReturnMsg(data, 1));
        }
        else {
            var dtBINData = data.getElementsByTagName('dtBINData');
            if (dtBINData.length > 0) {
                if (GetNodeValue(dtBINData[0], "BINman") != $('#lblManID1').html().split(' ')[0]) {
                    DyAlert("盤點人員" + GetNodeValue(dtBINData[0], "BINman") + "已建立分區" + $('#txtBinNo').val() + "之分區單!!", DummyFunction);
                    return;
                }
            }

            //AssignVar();
            //alert(GetNodeValue(dtISAMShop[0], "STName"));
            EditMode = "Q";
            $('#lblShop2').html($('#lblShop1').html());
            $('#lblBINNo2').html($('#txtBinNo').val());
            $('#lblDate2').html($('#txtISAMDate').val());
            $('#lblSWQty1title').html($('#lblShop1').html().split(' ')[0] + "門市總數：");
            $('#lblSBQty1title').html($('#txtBinNo').val() + "分區總數：");
            $('#lblSQty1').html('');
            $('#lblSQty1title').html($('#txtBinNo').val() + "分區單品總數：");
            $('#lblSBQty1').html('');
            $('#lblSWQty1').html('');
            $('#lblPrice').html('');
            $('#lblGDName').html("品名XXX");
            $('#pgISAM01Init').hide();
            if ($('#ISAM01btns').attr('hidden') == undefined) {
                $('#ISAM01btns').show();
            }
            else {
                $('#ISAM01btns').removeAttr('hidden');
            }
        }
    };

    let btSave_click = function () {
        //*
        ChkLogOut(sessionStorage.getItem('isamcomp'));
        Timerset(sessionStorage.getItem('isamcomp'));
        if ($('#txtISAMDate').val() == "" | $('#txtISAMDate').val() == null) {
            DyAlert("請輸入盤點日期!!", function () { $('#txtISAMDate').focus() });
            $(".modal-backdrop").remove();
            return;
        }
        if ($('#txtBinNo').val() == "" | $('#txtBinNo').val() == null) {
            DyAlert("請輸入分區代碼!!", function () { $('#txtBinNo').focus() });
            $(".modal-backdrop").remove();
            return;
        }
        else {
            if ($('#txtBinNo').val().length>3) {
                DyAlert("分區代碼不可超過3個字元!!", function () { $('#txtBinNo').focus() });
                $(".modal-backdrop").remove();
                return;
            }
        }
        var pData = {
            Shop: $('#lblShop1').html().split(' ')[0],
            ISAMDate: $('#txtISAMDate').val(),
            BinNo: $('#txtBinNo').val()
        }
        PostToWebApi({ url: "api/SystemSetup/SearchBINWeb", data: pData, success: afterSearchBINWeb });
    };
//#endregion

    //清除
    let btClear_click = function (bt) {
        //Timerset();
        $('#rdoDateAll').prop('checked', 'true');
        $('#rdoShop').prop('checked', 'true');
        $('#lblVIPFaceIDCnt').html('');
        $('#lblVIPFaceIDName').html('');
        chkVIPFaceID = "";
        $('#lblCityCnt').html('');
        $('#lblCityName').html('');
        chkCity = "";
    };

    //查詢
    let btQuery_click = function (bt) {
        //Timerset();
        $('#btQuery').prop('disabled', true)
     
        if ($('#rdoDateAll').prop('checked') == false && $('#rdoDate2M').prop('checked') == false && $('#rdoDate3M').prop('checked') == false && $('#rdoDate6M').prop('checked') == false && $('#rdoDate1Y').prop('checked') == false) {
            DyAlert("入會期間請至少選擇一項!", function () { $('#btQuery').prop('disabled', false); })
            return
        }
        if ($('#rdoShop').prop('checked') == false && $('#rdoCity').prop('checked') == false && $('#rdoMW').prop('checked') == false) {
            DyAlert("統計條件請至少選擇一項!", function () { $('#btQuery').prop('disabled', false); })
            return
        }
        ShowLoading();

        var VIPDate = ""
        if ($('#rdoDateAll').prop('checked') == true) {
            VIPDate = ""
        }
        else if ($('#rdoDate2M').prop('checked') == true) {
            VIPDate = "2M"
        }
        else if ($('#rdoDate3M').prop('checked') == true) {
            VIPDate = "3M"
        }
        else if ($('#rdoDate6M').prop('checked') == true) {
            VIPDate = "6M"
        }
        else if ($('#rdoDate1Y').prop('checked') == true) {
            VIPDate = "1Y"
        }

        var Flag = ""
        if ($('#rdoShop').prop('checked') == true) {
            Flag = "S";
            $('#tbQuery').show();
            $('#tbQueryMW').hide();
        }
        else if ($('#rdoCity').prop('checked') == true) {
            Flag = "C";
            $('#tbQuery').show();
            $('#tbQueryMW').hide();
        }
        else if ($('#rdoMW').prop('checked') == true) {
            Flag = "M";
            $('#tbQuery').hide();
            if ($('#tbQueryMW').attr('hidden') == undefined) {
                $('#tbQueryMW').show();
            }
            else {
                $('#tbQueryMW').removeAttr('hidden');
                $('#tbQueryMW').show();
            }
        }

        setTimeout(function () {
            var pData = {
                VIPFaceID: chkVIPFaceID,
                City: chkCity,
                VIPDate: VIPDate,
                Flag: Flag
            }
            PostToWebApi({ url: "api/SystemSetup/MSSD106Query", data: pData, success: afterMSSD106Query });
        }, 1000);
    };

    let afterMSSD106Query = function (data) {
        CloseLoading();
        if (ReturnMsg(data, 0) != "MSSD106QueryOK") {
            DyAlert(ReturnMsg(data, 1), function () { $('#btQuery').prop('disabled', false); });
        }
        else {
            $('#btQuery').prop('disabled', false);
            var dtE = data.getElementsByTagName('dtE');
            $('#lblBirNo').show();
            if ($('#rdoShop').prop('checked') || $('#rdoCity').prop('checked')) {
                grdM.BindData(dtE);
                var heads = $('#tbQuery thead tr th#thtype');
                if ($('#rdoShop').prop('checked')) {
                    $(heads).html('店別');
                }
                else if ($('#rdoCity').prop('checked')) {
                    $(heads).html('縣市');
                }
                if (dtE.length == 0) {
                    DyAlert("無符合資料!");
                    $('#lblEnd').html('');
                    $('#lblVIPQty').html('');
                    $(".modal-backdrop").remove();
                    $('#tbQuery thead td#td1').html('');
                    $('#tbQuery thead td#td2').html('');
                    $('#tbQuery thead td#td2p').html('');
                    $('#tbQuery thead td#td3').html('');
                    $('#tbQuery thead td#td3p').html('');
                    $('#tbQuery thead td#td4').html('');
                    $('#tbQuery thead td#td4p').html('');
                    $('#tbQuery thead td#td5').html('');
                    $('#tbQuery thead td#td5p').html('');
                    $('#tbQuery thead td#td6').html('');
                    $('#tbQuery thead td#td6p').html('');
                    $('#tbQuery thead td#td7').html('');
                    $('#tbQuery thead td#td7p').html('');
                    $('#tbQuery thead td#td8').html('');
                    $('#tbQuery thead td#td8p').html('');
                    $('#tbQuery thead td#td9').html('');
                    $('#tbQuery thead td#td9p').html('');
                    $('#tbQuery thead td#td10').html('');
                    $('#tbQuery thead td#td10p').html('');
                    return;
                }

                var dtH = data.getElementsByTagName('dtH');
                $('#lblEnd').html('截至' + GetNodeValue(dtH[0], "SysDate") + '止');
                $('#lblVIPQty').html('會員總數 : ' + parseInt(GetNodeValue(dtH[0], "SumCnt1")).toLocaleString('en-US'));
                $('#tbQuery thead td#td1').html(parseInt(GetNodeValue(dtH[0], "SumCnt1")).toLocaleString('en-US'));
                $('#tbQuery thead td#td2').html(parseInt(GetNodeValue(dtH[0], "SumCnt2")).toLocaleString('en-US'));
                $('#tbQuery thead td#td2p').html(GetNodeValue(dtH[0], "SumCnt2p"));
                $('#tbQuery thead td#td3').html(parseInt(GetNodeValue(dtH[0], "SumCnt3")).toLocaleString('en-US'));
                $('#tbQuery thead td#td3p').html(GetNodeValue(dtH[0], "SumCnt3p"));
                $('#tbQuery thead td#td4').html(parseInt(GetNodeValue(dtH[0], "SumCnt4")).toLocaleString('en-US'));
                $('#tbQuery thead td#td4p').html(GetNodeValue(dtH[0], "SumCnt4p"));
                $('#tbQuery thead td#td5').html(parseInt(GetNodeValue(dtH[0], "SumCnt5")).toLocaleString('en-US'));
                $('#tbQuery thead td#td5p').html(GetNodeValue(dtH[0], "SumCnt5p"));
                $('#tbQuery thead td#td6').html(parseInt(GetNodeValue(dtH[0], "SumCnt6")).toLocaleString('en-US'));
                $('#tbQuery thead td#td6p').html(GetNodeValue(dtH[0], "SumCnt6p"));
                $('#tbQuery thead td#td7').html(parseInt(GetNodeValue(dtH[0], "SumCnt7")).toLocaleString('en-US'));
                $('#tbQuery thead td#td7p').html(GetNodeValue(dtH[0], "SumCnt7p"));
                $('#tbQuery thead td#td8').html(parseInt(GetNodeValue(dtH[0], "SumCnt8")).toLocaleString('en-US'));
                $('#tbQuery thead td#td8p').html(GetNodeValue(dtH[0], "SumCnt8p"));
                $('#tbQuery thead td#td9').html(parseInt(GetNodeValue(dtH[0], "SumCnt9")).toLocaleString('en-US'));
                $('#tbQuery thead td#td9p').html(GetNodeValue(dtH[0], "SumCnt9p"));
                $('#tbQuery thead td#td10').html(parseInt(GetNodeValue(dtH[0], "SumCnt10")).toLocaleString('en-US'));
                $('#tbQuery thead td#td10p').html(GetNodeValue(dtH[0], "SumCnt10p"));
            }
            else if ($('#rdoMW').prop('checked')) {
                grdMW.BindData(dtE);
                if (dtE.length == 0) {
                    DyAlert("無符合資料!");
                    $('#lblEnd').html('');
                    $('#lblVIPQty').html('');
                    $(".modal-backdrop").remove();
                    $('#tbQueryMW thead td#td1_MW').html('');
                    $('#tbQueryMW thead td#td2_MW').html('');
                    $('#tbQueryMW thead td#td2p_MW').html('');
                    $('#tbQueryMW thead td#td3_MW').html('');
                    $('#tbQueryMW thead td#td3p_MW').html('');
                    $('#tbQueryMW thead td#td4_MW').html('');
                    $('#tbQueryMW thead td#td4p_MW').html('');
                    $('#tbQueryMW thead td#td5_MW').html('');
                    $('#tbQueryMW thead td#td5p_MW').html('');
                    $('#tbQueryMW thead td#td6_MW').html('');
                    $('#tbQueryMW thead td#td6p_MW').html('');
                    $('#tbQueryMW thead td#td7_MW').html('');
                    $('#tbQueryMW thead td#td7p_MW').html('');
                    $('#tbQueryMW thead td#td8_MW').html('');
                    $('#tbQueryMW thead td#td8p_MW').html('');
                    return;
                }
                var dtH = data.getElementsByTagName('dtH');
                $('#lblEnd').html('截至' + GetNodeValue(dtH[0], "SysDate") + '止');
                $('#lblVIPQty').html('會員總數 : ' + parseInt(GetNodeValue(dtH[0], "SumCnt1")).toLocaleString('en-US'));
                $('#tbQueryMW thead td#td1_MW').html(parseInt(GetNodeValue(dtH[0], "SumCnt1")).toLocaleString('en-US'));
                $('#tbQueryMW thead td#td2_MW').html(parseInt(GetNodeValue(dtH[0], "SumCnt2")).toLocaleString('en-US'));
                $('#tbQueryMW thead td#td2p_MW').html(GetNodeValue(dtH[0], "SumCnt2p"));
                $('#tbQueryMW thead td#td3_MW').html(parseInt(GetNodeValue(dtH[0], "SumCnt3")).toLocaleString('en-US'));
                $('#tbQueryMW thead td#td3p_MW').html(GetNodeValue(dtH[0], "SumCnt3p"));
                $('#tbQueryMW thead td#td4_MW').html(parseInt(GetNodeValue(dtH[0], "SumCnt4")).toLocaleString('en-US'));
                $('#tbQueryMW thead td#td4p_MW').html(GetNodeValue(dtH[0], "SumCnt4p"));
                $('#tbQueryMW thead td#td5_MW').html(parseInt(GetNodeValue(dtH[0], "SumCnt5")).toLocaleString('en-US'));
                $('#tbQueryMW thead td#td5p_MW').html(GetNodeValue(dtH[0], "SumCnt5p"));
                $('#tbQueryMW thead td#td6_MW').html(parseInt(GetNodeValue(dtH[0], "SumCnt6")).toLocaleString('en-US'));
                $('#tbQueryMW thead td#td6p_MW').html(GetNodeValue(dtH[0], "SumCnt6p"));
                $('#tbQueryMW thead td#td7_MW').html(parseInt(GetNodeValue(dtH[0], "SumCnt7")).toLocaleString('en-US'));
                $('#tbQueryMW thead td#td7p_MW').html(GetNodeValue(dtH[0], "SumCnt7p"));
                $('#tbQueryMW thead td#td8_MW').html(parseInt(GetNodeValue(dtH[0], "SumCnt8")).toLocaleString('en-US'));
                $('#tbQueryMW thead td#td8p_MW').html(GetNodeValue(dtH[0], "SumCnt8p"));
            }
        }
    };

    //會籍店櫃多選
    let btVIPFaceID_click = function (bt) {
        //Timerset();
        var pData = {
            ST_ID: ""
        }
        PostToWebApi({ url: "api/SystemSetup/MSVP102_GetVIPFaceID", data: pData, success: afterMSSD106_GetVIPFaceID });
    };

    let afterMSSD106_GetVIPFaceID = function (data) {
        if (ReturnMsg(data, 0) != "MSVP102_GetVIPFaceIDOK") {
            DyAlert(ReturnMsg(data, 1));
        }
        else {
            var dtE = data.getElementsByTagName('dtE');
            $('#txtLpQ_VIPFaceID').val('');
            $('#modal_Lookup_VIPFaceID').modal('show');
            setTimeout(function () {
                grdLookUp_VIPFaceID.BindData(dtE);
                if (chkVIPFaceID != "") {
                    var VIPFaceID = chkVIPFaceID.split(',');
                    for (var i = 0; i < VIPFaceID.length; i++) {
                        $('#tbLookup_VIPFaceID tbody tr .tdCol2').filter(function () { return $(this).text() == VIPFaceID[i].replaceAll("'", ""); }).closest('tr').find('.tdCol1 input:checkbox').prop('checked', true);
                    }
                }
            }, 500);
        }
    };

    let btLpQ_VIPFaceID_click = function (bt) {
        //Timerset();
        $('#btLpQ_VIPFaceID').prop('disabled', true);
        var pData = {
            ST_ID: $('#txtLpQ_VIPFaceID').val()
        }
        PostToWebApi({ url: "api/SystemSetup/MSVP102_GetVIPFaceID", data: pData, success: afterLpQ_VIPFaceID });
    };

    let afterLpQ_VIPFaceID = function (data) {
        if (ReturnMsg(data, 0) != "MSVP102_GetVIPFaceIDOK") {
            DyAlert(ReturnMsg(data, 1), function () {
                $('#btLpQ_VIPFaceID').prop('disabled', false);
            });
        }
        else {
            var dtE = data.getElementsByTagName('dtE');
            if (dtE.length == 0) {
                DyAlert("無符合資料!", function () {
                    $('#btLpQ_VIPFaceID').prop('disabled', false);
                });
                $(".modal-backdrop").remove();
                return;
            }
            grdLookUp_VIPFaceID.BindData(dtE);
            $('#btLpQ_VIPFaceID').prop('disabled', false);
        }
    };

    let btLpOK_VIPFaceID_click = function (bt) {
        //Timerset();
        $('#btLpOK_VIPFaceID').prop('disabled', true);
        var obchkedtd = $('#tbLookup_VIPFaceID .checkbox:checked');
        chkedRow = obchkedtd.length.toString();   //本次已勾選的總筆數
        if (chkedRow == 0) {
            $('#lblVIPFaceIDCnt').html('');
            $('#lblVIPFaceIDName').html('');
            chkVIPFaceID = "";
            $('#btLpOK_VIPFaceID').prop('disabled', false);
            $('#modal_Lookup_VIPFaceID').modal('hide');
            return
        } else {
            $('#lblVIPFaceIDCnt').html(chkedRow)
            chkVIPFaceID = "";
            var VIPFaceIDName = "";
            for (var i = 0; i < obchkedtd.length; i++) {
                var a = $(obchkedtd[i]).closest('tr');
                var trNode = $(a).prop('Record');
                chkVIPFaceID += "'" + GetNodeValue(trNode, "ST_ID") + "',";  //已勾選的每一筆店倉
                if (i <= 9) {
                    VIPFaceIDName += GetNodeValue(trNode, "ST_SName") + "，";
                }
            }
            chkVIPFaceID = chkVIPFaceID.substr(0, chkVIPFaceID.length - 1)
            if (chkedRow > 10) {
                $('#lblVIPFaceIDName').html(VIPFaceIDName.substr(0, VIPFaceIDName.length - 1) + '...')
            }
            else {
                $('#lblVIPFaceIDName').html(VIPFaceIDName.substr(0, VIPFaceIDName.length - 1))
            }
            $('#btLpOK_VIPFaceID').prop('disabled', false);
            $('#modal_Lookup_VIPFaceID').modal('hide');
        }
    };

    let btLpExit_VIPFaceID_click = function (bt) {
        //Timerset();
        $('#modal_Lookup_VIPFaceID').modal('hide');
    };

    let btLpClear_VIPFaceID_click = function (bt) {
        //Timerset();
        $("#txtLpQ_VIPFaceID").val('');
        $("#tbLookup_VIPFaceID .checkbox").prop('checked', false);
    };
    //縣市多選
    let btCity_click = function (bt) {
        //Timerset();
        var pData = {
            City: ""
        }
        PostToWebApi({ url: "api/SystemSetup/GetCity", data: pData, success: afterGetCity });
    };

    let afterGetCity = function (data) {
        if (ReturnMsg(data, 0) != "GetCityOK") {
            DyAlert(ReturnMsg(data, 1));
        }
        else {
            var dtE = data.getElementsByTagName('dtE');
            $('#txtLpQ_City').val('')
            $('#modal_Lookup_City').modal('show');
            setTimeout(function () {
                grdLookUp_City.BindData(dtE);
                if (chkCity != "") {
                    var City = chkCity.split(',');
                    for (var i = 0; i < City.length; i++) {
                        $('#tbLookup_City tbody tr .tdCol2').filter(function () { return $(this).text() == City[i].replaceAll("'", ""); }).closest('tr').find('.tdCol1 input:checkbox').prop('checked', true);
                    }
                }
            }, 500);
        }
    };

    let btLpQ_City_click = function (bt) {
        //Timerset();
        $('#btLpQ_City').prop('disabled', true);
        var pData = {
            City: $('#txtLpQ_City').val()
        }
        PostToWebApi({ url: "api/SystemSetup/GetCity", data: pData, success: afterLpQ_City });
    };

    let afterLpQ_City = function (data) {
        if (ReturnMsg(data, 0) != "GetCityOK") {
            DyAlert(ReturnMsg(data, 1), function () {
                $('#btLpQ_City').prop('disabled', false);
            });
        }
        else {
            var dtE = data.getElementsByTagName('dtE');
            if (dtE.length == 0) {
                DyAlert("無符合資料!", function () {
                    $('#btLpQ_City').prop('disabled', false);
                });
                $(".modal-backdrop").remove();
                return;
            }
            grdLookUp_City.BindData(dtE);
            $('#btLpQ_City').prop('disabled', false);
        }
    };

    let btLpOK_City_click = function (bt) {
        //Timerset();
        $('#btLpOK_City').prop('disabled', true);
        var obchkedtd = $('#tbLookup_City .checkbox:checked');
        chkedRow = obchkedtd.length.toString();   //本次已勾選的總筆數
        if (chkedRow == 0) {
            $('#lblCityCnt').html('');
            $('#lblCityName').html('');
            chkCity = "";
            $('#btLpOK_City').prop('disabled', false);
            $('#modal_Lookup_City').modal('hide');
            return
        } else {
            $('#lblCityCnt').html(chkedRow)
            chkCity = "";
            var CityName = "";
            for (var i = 0; i < obchkedtd.length; i++) {
                var a = $(obchkedtd[i]).closest('tr');
                var trNode = $(a).prop('Record');
                chkCity += "'" + GetNodeValue(trNode, "City") + "',";  //已勾選的每一筆店倉
                if (i <= 1) {
                    CityName += GetNodeValue(trNode, "City") + "，";
                }
            }
            chkCity = chkCity.substr(0, chkCity.length - 1)
            if (chkedRow > 2) {
                $('#lblCityName').html(CityName.substr(0, CityName.length - 1) + '...')
            }
            else {
                $('#lblCityName').html(CityName.substr(0, CityName.length - 1))
            }
            $('#btLpOK_City').prop('disabled', false);
            $('#modal_Lookup_City').modal('hide');
        }
    };

    let btLpExit_City_click = function (bt) {
        //Timerset();
        $('#modal_Lookup_City').modal('hide');
    };

    let btLpClear_City_click = function (bt) {
        //Timerset();
        $("#txtLpQ_City").val('');
        $("#tbLookup_City .checkbox").prop('checked', false);
    };
//#region FormLoad
    let GetInitMSSD106 = function (data) {
        if (ReturnMsg(data, 0) != "GetInitMSSD106OK") {
            DyAlert(ReturnMsg(data, 1));
        }
        else {
            var dtE = data.getElementsByTagName('dtE');
            var dtV = data.getElementsByTagName('dtV');
            $('#lblBirNo').hide();
            if (dtE.length > 0) {
                $('#lblProgramName').html(GetNodeValue(dtE[0], "ChineseName"));
            }
            if (dtV.length > 0) {
                $('#lblEnd').html('截至' + GetNodeValue(dtV[0], "SysDate") + '止');
                $('#lblVIPQty').html('會員總數 : ' + parseInt(GetNodeValue(dtV[0], "VIPCntAll")).toLocaleString('en-US'));
            }
            else {
                $('#lblEnd').html('')
                $('#lblVIPQty').html('');
            }
            AssignVar();
            $('#btQuery').click(function () { btQuery_click(this) });
            $('#btClear').click(function () { btClear_click(this) });
            $('#btVIPFaceID').click(function () { btVIPFaceID_click(this) });
            $('#btLpQ_VIPFaceID').click(function () { btLpQ_VIPFaceID_click(this) });
            $('#btLpOK_VIPFaceID').click(function () { btLpOK_VIPFaceID_click(this) });
            $('#btLpExit_VIPFaceID').click(function () { btLpExit_VIPFaceID_click(this) });
            $('#btLpClear_VIPFaceID').click(function () { btLpClear_VIPFaceID_click(this) });
            $('#btCity').click(function () { btCity_click(this) });
            $('#btLpQ_City').click(function () { btLpQ_City_click(this) });
            $('#btLpOK_City').click(function () { btLpOK_City_click(this) });
            $('#btLpExit_City').click(function () { btLpExit_City_click(this) });
            $('#btLpClear_City').click(function () { btLpClear_City_click(this) });

        }
    };
    
    let afterLoadPage = function () {
        var pData = {
            ProgramID: "MSSD106"
        }
        PostToWebApi({ url: "api/SystemSetup/GetInitMSSD106", data: pData, success: GetInitMSSD106 });
    };
//#endregion
    

    if ($('#pgMSSD106').length == 0) {  
        AllPages = new LoadAllPages(ParentNode, "SystemSetup/MSSD106", ["MSSD106btns", "pgMSSD106Init", "pgMSSD106Add", "pgMSSD106Mod"], afterLoadPage);
    };


}