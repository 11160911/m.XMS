var PageMSSA101 = function (ParentNode) {

    let grdM;
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
                    { type: "Text", name: "VPer" }
                ],
                //rows_per_page: 10,
                method_clickrow: click_PLU,
                afterBind: InitModifyDeleteButton,
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

        //grdLookUp_City = new DynGrid(
        //    {
        //        table_lement: $('#tbLookup_City')[0],
        //        class_collection: ["tdCol1 text-center", "tdCol2"],
        //        fields_info: [
        //            { type: "checkbox", name: "chkset", style: "width:16px;height:16px" },
        //            { type: "Text", name: "City", style: "" }
        //        ],
        //        //rows_per_page: 10,
        //        method_clickrow: click_PLU,
        //        sortable: "N"
        //    }
        //);

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
                $('#txtOpenDateS1').val(GetNodeValue(dtE[0], "SysDate1").toString().trim().replaceAll('/', '-'));
                $('#txtOpenDateE1').val(GetNodeValue(dtE[0], "SysDate1").toString().trim().replaceAll('/', '-'));
            }
            $('#lblShopNoCnt').html('');
            $('#lblShopNoName').html('');
            chkShopNo = "";
            $('#rdoA').prop('checked', 'true');
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
                $('#tbQuery thead td#td1').html('');
                $('#tbQuery thead td#td2').html('');
                $('#tbQuery thead td#td3').html('');
                $('#tbQuery thead td#td4').html('');
                $('#tbQuery thead td#td5').html('');
                $('#tbQuery thead td#td6').html('');
                $('#tbQuery thead td#td7').html('');
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
            return
        } else {
            $('#lblShopNoCnt').html(chkedRow)
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
                $('#lblShopNoName').html(ShopNoName.substr(0, ShopNoName.length - 1) + '...')
            }
            else {
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
                $('#txtOpenDateS1').val(GetNodeValue(dtE[0], "SysDate1").toString().trim().replaceAll('/', '-'));
                $('#txtOpenDateE1').val(GetNodeValue(dtE[0], "SysDate1").toString().trim().replaceAll('/', '-'));
            }
            AssignVar();

            $('#btQuery').click(function () { btQuery_click(this) });
            $('#btClear').click(function () { btClear_click(this) });

            $('#btShopNo').click(function () { btShopNo_click(this) });
            $('#btLpQ_ShopNo').click(function () { btLpQ_ShopNo_click(this) });
            $('#btLpOK_ShopNo').click(function () { btLpOK_ShopNo_click(this) });
            $('#btLpExit_ShopNo').click(function () { btLpExit_ShopNo_click(this) });
            $('#btLpClear_ShopNo').click(function () { btLpClear_ShopNo_click(this) });
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