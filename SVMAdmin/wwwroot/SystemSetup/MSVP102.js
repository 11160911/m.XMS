var PageMSVP102 = function (ParentNode) {

    let grdM;
    let grdLookUp_VIPFaceID_SendSet;
    let grdSendSet;
    let grdDMSel;

    let EditMode = "";
    let DelPLU = "";
    let DelPLUQty;
    let ModPLU = "";
    let ModPLUQty;

    let chkVIPFaceID = "";

    let AssignVar = function () {
        
        grdM = new DynGrid(
            {
                table_lement: $('#tbQuery')[0],
                class_collection: ["tdCol1 text-center", "tdCol2 text-center", "tdCol3 text-center", "tdCol4 text-center", "tdCol5 text-center", "tdCol6", "tdCol7 text-center"],
                fields_info: [
                    { type: "Text", name: "EVNO", style: "" },
                    { type: "TextAmt", name: "Cnt"},
                    { type: "Text", name: "ApproveDate" },
                    { type: "Text", name: "TOMailDate" },
                    { type: "Text", name: "EDM_DocNo"},
                    { type: "Text", name: "EDMMemo"},
                    { type: "Text", name: "EDDate"}
                ],
                rows_per_page: 50,
                method_clickrow: click_PLU,
                afterBind: InitModifyDeleteButton,
                sortable: "Y"
            }
        );

        grdLookUp_VIPFaceID_SendSet = new DynGrid(
            {
                table_lement: $('#tbDataLookup_VIPFaceID_SendSet')[0],
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

        grdSendSet = new DynGrid(
            {
                table_lement: $('#tbSendSet')[0],
                class_collection: ["tdCol1", "tdCol2", "tdCol3", "tdCol4", "tdCol5", "tdCol6", "tdCol7", "tdCol8", "tdCol9 label-align", "tdCol10"],
                fields_info: [
                    { type: "Text", name: "VIP_ID2", style: "" },
                    { type: "Text", name: "VIP_Name" },
                    { type: "Text", name: "VIP_Tel" },
                    { type: "Text", name: "VIP_Eadd" },
                    { type: "Text", name: "VIP_NM" },
                    { type: "Text", name: "City" },
                    { type: "Text", name: "AreaName" },
                    { type: "Text", name: "VIP_LCDay" },
                    { type: "TextAmt", name: "PointsBalance" },
                    { type: "Text", name: "VIP_Type" },
                ],
                rows_per_page: 200,
                method_clickrow: click_PLU,
                afterBind: InitModifyDeleteButton,
                sortable: "N"
            }
        );

        grdDMSel = new DynGrid(
            {
                table_lement: $('#tbDMSel')[0],
                class_collection: ["tdCol1 text-center", "tdCol2", "tdCol3", "tdCol4", "tdCol5", "tdCol6", "tdCol7", "tdCol8", "tdCol9 label-align"],
                fields_info: [
                    { type: "radio", name: "chkset", style: "width:15px;height:15px" },
                    { type: "Text", name: "DocNo" },
                    { type: "Text", name: "EDMMemo" },
                    { type: "Text", name: "EDDate1" },
                    { type: "Text", name: "ActivityCode" },
                    { type: "Text", name: "PS_Name" },
                    { type: "Text", name: "EDDate2" },
                    { type: "Text", name: "WhNoFlag" },
                    { type: "TextAmt", name: "Cnt2" }
                ],
                rows_per_page: 10,
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
        //$('#tbQuery tbody tr td').click(function () { Step1_click(this) });
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
        $('#txtEVNO').val('');
        $('#txtEDM_DocNo').val('');
        $('#txtStartDate').val('');
        $('#txtEDMMemo').val('');
    };

    //查詢
    let btQuery_click = function (bt) {
        //Timerset();
        $('#btQuery').prop('disabled', true)
        ShowLoading();
        var pData = {
            EVNO: $('#txtEVNO').val(),
            EDM_DocNo: $('#txtEDM_DocNo').val(),
            StartDate: $('#txtStartDate').val().toString().replaceAll('-', '/'),
            EDMMemo: $('#txtEDMMemo').val()
        }
        PostToWebApi({ url: "api/SystemSetup/MSVP102Query", data: pData, success: afterMSVP102Query });
    };

    let afterMSVP102Query = function (data) {
        CloseLoading();
        if (ReturnMsg(data, 0) != "MSVP102QueryOK") {
            DyAlert(ReturnMsg(data, 1), function () { $('#btQuery').prop('disabled', false); });
        }
        else {
            $('#btQuery').prop('disabled', false);
            var dtE = data.getElementsByTagName('dtE');
            if (dtE.length == 0) {
                DyAlert("無符合資料!");
                return;
            }
            grdM.BindData(dtE);
        }
    };

    //發送設定
    let btSendSet_click = function (bt) {
        //Timerset();
        var pData = {
        }
        PostToWebApi({ url: "api/SystemSetup/MSVP102_GetVMEVNO", data: pData, success: afterMSVP102_GetVMEVNO });
    };

    let afterMSVP102_GetVMEVNO = function (data) {
        if (ReturnMsg(data, 0) != "MSVP102_GetVMEVNOOK") {
            DyAlert(ReturnMsg(data, 1));
        }
        else {
            var dtE = data.getElementsByTagName('dtE');
            var dtV = data.getElementsByTagName('dtV');
            $('#lblVMEVNO_SendSet').html(GetNodeValue(dtE[0], "DocNo"))
            $('#lblVIPCnt_SendSet').html('0')
            btClear_SendSet_click();
            grdSendSet.BindData(dtV);
            $('#modal_SendSet').modal('show')
        }
    };

    //會籍店櫃多選(發送設定)
    let btVIPFaceID_SendSet_click = function (bt) {
        //Timerset();
        var pData = {
            ST_ID: ""
        }
        PostToWebApi({ url: "api/SystemSetup/MSVP102_GetVIPFaceID", data: pData, success: afterMSVP102_GetVIPFaceID });
    };

    let afterMSVP102_GetVIPFaceID = function (data) {
        if (ReturnMsg(data, 0) != "MSVP102_GetVIPFaceIDOK") {
            DyAlert(ReturnMsg(data, 1));
        }
        else {
            var dtE = data.getElementsByTagName('dtE');
            $('#modal_Lookup_VIPFaceID_SendSet').modal('show');
            setTimeout(function () {
                grdLookUp_VIPFaceID_SendSet.BindData(dtE);
                if (chkVIPFaceID != "") {
                    var VIPFaceID = chkVIPFaceID.split(',');
                    for (var i = 0; i < VIPFaceID.length; i++) {
                        $('#tbDataLookup_VIPFaceID_SendSet tbody tr .tdCol2').filter(function () { return $(this).text() == VIPFaceID[i].replaceAll("'", ""); }).closest('tr').find('.tdCol1 input:checkbox').prop('checked', true);
                    }
                }
            }, 500);
        }
    };

    let btLpQ_VIPFaceID_SendSet_click = function (bt) {
        //Timerset();
        $('#btLpQ_VIPFaceID_SendSet').prop('disabled', true);
        var pData = {
            ST_ID: $('#txtLpQ_VIPFaceID_SendSet').val()
        }
        PostToWebApi({ url: "api/SystemSetup/MSVP102_GetVIPFaceID", data: pData, success: afterLpQ_VIPFaceID_SendSet });
    };

    let afterLpQ_VIPFaceID_SendSet = function (data) {
        if (ReturnMsg(data, 0) != "MSVP102_GetVIPFaceIDOK") {
            $('#modal_Lookup_VIPFaceID_SendSet').modal('hide');
            DyAlert(ReturnMsg(data, 1), function () {
                $('#btLpQ_VIPFaceID_SendSet').prop('disabled', false);
                $('#modal_Lookup_VIPFaceID_SendSet').modal('show');
            });
        }
        else {
            var dtE = data.getElementsByTagName('dtE');
            if (dtE.length == 0) {
                $('#modal_Lookup_VIPFaceID_SendSet').modal('hide');
                DyAlert("無符合資料!", function () {
                    $('#btLpQ_VIPFaceID_SendSet').prop('disabled', false);
                    $('#modal_Lookup_VIPFaceID_SendSet').modal('show');
                });
                $(".modal-backdrop").remove();
                return;
            }
            grdLookUp_VIPFaceID_SendSet.BindData(dtE);
            $('#btLpQ_VIPFaceID_SendSet').prop('disabled', false);
        }
    };

    let btLpOK_VIPFaceID_SendSet_click = function (bt) {
        //Timerset();
        $('#btLpOK_VIPFaceID_SendSet').prop('disabled', true);
        var obchkedtd = $('#tbDataLookup_VIPFaceID_SendSet .checkbox:checked');
        chkedRow = obchkedtd.length.toString();   //本次已勾選的總筆數
        if (chkedRow == 0) {
            $('#lblVIPFaceIDCnt_SendSet').html('');
            $('#lblVIPFaceIDName_SendSet').html('');
            chkVIPFaceID = "";
            $('#btLpOK_VIPFaceID_SendSet').prop('disabled', false);
            $('#modal_Lookup_VIPFaceID_SendSet').modal('hide');
            return
        } else {
            $('#lblVIPFaceIDCnt_SendSet').html(chkedRow)
            chkVIPFaceID = "";
            var VIPFaceIDName = "";
            for (var i = 0; i < obchkedtd.length; i++) {
                var a = $(obchkedtd[i]).closest('tr');
                var trNode = $(a).prop('Record');
                chkVIPFaceID += "'" + GetNodeValue(trNode, "ST_ID") + "',";  //已勾選的每一筆店倉
                if (i <= 2) {
                    VIPFaceIDName += GetNodeValue(trNode, "ST_SName") + "，";
                }
            }
            chkVIPFaceID = chkVIPFaceID.substr(0, chkVIPFaceID.length - 1)
            if (chkedRow > 3) {
                $('#lblVIPFaceIDName_SendSet').html(VIPFaceIDName.substr(0, VIPFaceIDName.length - 1) + '...')
            }
            else {
                $('#lblVIPFaceIDName_SendSet').html(VIPFaceIDName.substr(0, VIPFaceIDName.length - 1))
            }
            $('#btLpOK_VIPFaceID_SendSet').prop('disabled', false);
            $('#modal_Lookup_VIPFaceID_SendSet').modal('hide');
        }
    };

    let btLpExit_VIPFaceID_SendSet_click = function (bt) {
        //Timerset();
        $('#modal_Lookup_VIPFaceID_SendSet').modal('hide');
    };

    //清除(發送設定)
    let btClear_SendSet_click = function (bt) {
        //Timerset();
        $('#lblDocNo_SendSet').html('');
        $('#lblEDMMemo_SendSet').html('');
        $('#lblEDDate_SendSet').html('');
        $('#lblVIPFaceIDCnt_SendSet').html('');
        $('#lblVIPFaceIDName_SendSet').html('');
        chkVIPFaceID = "";
        $('#chk0_SendSet').prop('checked', true);
        $('#chk1_SendSet').prop('checked', true);
        $('#chk2_SendSet').prop('checked', true);
        $('#chk3_SendSet').prop('checked', false);
    };

    //查詢(發送設定)
    let btQuery_SendSet_click = function (bt) {
        //Timerset();
        $('#btQuery_SendSet').prop('disabled', true)
        if ($('#chk0_SendSet').prop('checked') == false && $('#chk1_SendSet').prop('checked') == false && $('#chk2_SendSet').prop('checked') == false && $('#chk3_SendSet').prop('checked') == false) {
            DyAlert("請選擇會員卡別!", function () {
                $('#btQuery_SendSet').prop('disabled', false)
            })
            return;
        }
        ShowLoading();

        var VIP_Type = "";
        if ($('#chk0_SendSet').prop('checked') == true) {
            VIP_Type += "'0',";
        }
        if ($('#chk1_SendSet').prop('checked') == true) {
            VIP_Type += "'1',";
        }
        if ($('#chk2_SendSet').prop('checked') == true) {
            VIP_Type += "'2',";
        }
        if ($('#chk3_SendSet').prop('checked') == true) {
            VIP_Type += "'3',";
        }
        VIP_Type = VIP_Type.substr(0, VIP_Type.length - 1)

        setTimeout(function () {
            var pData = {
                chkVIPFaceID: chkVIPFaceID,
                VIP_Type: VIP_Type,
                VMEVNO: $('#lblVMEVNO_SendSet').html()
            }
            PostToWebApi({ url: "api/SystemSetup/MSVP102Query_SendSet", data: pData, success: afterMSVP102Query_SendSet });
        }, 1000);
    };

    let afterMSVP102Query_SendSet = function (data) {
        CloseLoading();

        if (ReturnMsg(data, 0) != "MSVP102Query_SendSetOK") {
            DyAlert(ReturnMsg(data, 1), function () { $('#btQuery_SendSet').prop('disabled', false); });
        }
        else {
            var dtE = data.getElementsByTagName('dtE');
            grdSendSet.BindData(dtE);
            if (dtE.length == 0) {
                $('#lblVIPCnt_SendSet').html('0')
                DyAlert("無符合資料!", function () { $('#btQuery_SendSet').prop('disabled', false); });
                $(".modal-backdrop").remove();
                return;
            }
            $('#lblVIPCnt_SendSet').html(dtE.length)
            $('#btQuery_SendSet').prop('disabled', false);
        }
    };

    //上一頁(發送設定)
    let btRe_SendSet_click = function (bt) {
        //Timerset();
        var pData = {
        }
        PostToWebApi({ url: "api/SystemSetup/MSVP102_ReSendSet", data: pData, success: afterMSVP102_ReSendSet });
    };

    let afterMSVP102_ReSendSet = function (data) {
        if (ReturnMsg(data, 0) != "MSVP102_ReSendSetOK") {
            DyAlert(ReturnMsg(data, 1));
        }
        else {
            $('#modal_SendSet').modal('hide');
        }
    };

    //DM選取(發送設定)
    let btDMSel_SendSet_click = function (bt) {
        //Timerset();
        var pData = {
        }
        PostToWebApi({ url: "api/SystemSetup/MSVP102_GetDM", data: pData, success: afterMSVP102_GetDM });
    };

    let afterMSVP102_GetDM = function (data) {
        if (ReturnMsg(data, 0) != "MSVP102_GetDMOK") {
            DyAlert(ReturnMsg(data, 1));
        }
        else {
            var dtE = data.getElementsByTagName('dtE');
            $('#modal_DMSel').modal('show');

            setTimeout(function () {
                grdDMSel.BindData(dtE);
                $('#tbDMSel tbody tr .tdCol2').filter(function () { return $(this).text() == $('#lblDocNo_SendSet').html(); }).closest('tr').find('.tdCol1 input:radio').prop('checked', true);
            }, 500);
        }
    };

    //發送EDM(發送設定)
    let btDMSend_SendSet_click = function (bt) {
        //Timerset();
        $('#btDMSend_SendSet').prop('disabled', true);
        if ($('#lblVIPCnt_SendSet').html() == "0") {
            DyAlert("請先篩選會員資料!", function () { $('#btDMSend_SendSet').prop('disabled', false); })
            return;
        }
        if ($('#lblDocNo_SendSet').html() == "") {
            DyAlert("請選擇欲發送之DM!", function () { $('#btDMSend_SendSet').prop('disabled', false); })
            return;
        }
        DyConfirm("確定發送", function () {
            var pData = {
                VMDocNo: $('#lblVMEVNO_SendSet').html(),
                DMDocNo: $('#lblDocNo_SendSet').html()
            }
            PostToWebApi({ url: "api/SystemSetup/MSVP102_DMSend", data: pData, success: afterMSVP102_DMSend });
        }, function () { $('#btDMSend_SendSet').prop('disabled', false); }, $('#lblDocNo_SendSet').html() + $('#lblEDMMemo_SendSet').html(), "給篩選會員數 " + $('#lblVIPCnt_SendSet').html() + "筆?")

        
    };

    let afterMSVP102_DMSend = function (data) {
        if (ReturnMsg(data, 0) != "MSVP102_DMSendOK") {
            DyAlert(ReturnMsg(data, 1), function () { $('#btDMSend_SendSet').prop('disabled', false); });
        }
        else {
            DyAlert("發送EDM排程", function () {
                $('#btDMSend_SendSet').prop('disabled', false);
                $('#modal_SendSet').modal('hide');
            }, "已成功送出")
        }
    };

    //上一頁(DM選取)
    let btRe_DMSel_click = function (bt) {
        //Timerset();
        $('#modal_DMSel').modal('hide');
    };

    //確定(DM選取)
    let btOK_DMSel_click = function (bt) {
        //Timerset();
        $('#btOK_DMSel').prop('disabled', true);
        var obchkedtd = $('#tbDMSel input[type="radio"]:checked');
        var chkedRow = obchkedtd.length.toString();

        if (chkedRow == 0) {
            $('#modal_DMSel').modal('hide');
            DyAlert("未選取DM單號，請重新確認!", function () {
                $('#btOK_DMSel').prop('disabled', false);
                $('#modal_DMSel').modal('show');
            });
        }
        else {
            var a = $(obchkedtd[0]).closest('tr');
            var trNode = $(a).prop('Record');

            $('#lblDocNo_SendSet').html(GetNodeValue(trNode, "DocNo"))
            $('#lblEDMMemo_SendSet').html(GetNodeValue(trNode, "EDMMemo"))
            $('#lblEDDate_SendSet').html(GetNodeValue(trNode, "EDDate1"))

            $('#btOK_DMSel').prop('disabled', false);
            $('#modal_DMSel').modal('hide')
        }
    };

    //#region FormLoad
    let GetInitMSVP102 = function (data) {
        if (ReturnMsg(data, 0) != "GetInitmsDMOK") {
            DyAlert(ReturnMsg(data, 1));
        }
        else {
            var dtE = data.getElementsByTagName('dtE');
            if (dtE.length > 0) {
                $('#lblProgramName').html(GetNodeValue(dtE[0], "ChineseName"));
            }
            AssignVar();
            $('#btQuery').click(function () { btQuery_click(this) });
            $('#btClear').click(function () { btClear_click(this) });
            $('#btSendSet').click(function () { btSendSet_click(this) });

            $('#btVIPFaceID_SendSet').click(function () { btVIPFaceID_SendSet_click(this) });
            $('#btLpQ_VIPFaceID_SendSet').click(function () { btLpQ_VIPFaceID_SendSet_click(this) });
            $('#btLpOK_VIPFaceID_SendSet').click(function () { btLpOK_VIPFaceID_SendSet_click(this) });
            $('#btLpExit_VIPFaceID_SendSet').click(function () { btLpExit_VIPFaceID_SendSet_click(this) });
            $('#btClear_SendSet').click(function () { btClear_SendSet_click(this) });
            $('#btQuery_SendSet').click(function () { btQuery_SendSet_click(this) });
            $('#btRe_SendSet').click(function () { btRe_SendSet_click(this) });
            $('#btDMSel_SendSet').click(function () { btDMSel_SendSet_click(this) });
            $('#btDMSend_SendSet').click(function () { btDMSend_SendSet_click(this) });
            $('#btOK_DMSel').click(function () { btOK_DMSel_click(this) });
            $('#btRe_DMSel').click(function () { btRe_DMSel_click(this) });

         
        }
    };
    
    let afterLoadPage = function () {
        var pData = {
            ProgramID: "MSVP102"
        }
        PostToWebApi({ url: "api/SystemSetup/GetInitmsDM", data: pData, success: GetInitMSVP102 });
    };
//#endregion
    

    if ($('#pgMSVP102').length == 0) {  
        AllPages = new LoadAllPages(ParentNode, "SystemSetup/MSVP102", ["MSVP102btns", "pgMSVP102Init", "pgMSVP102Add", "pgMSVP102Mod"], afterLoadPage);
    };

    
}