var PageMSSA106 = function (ParentNode) {

    let grdM;
    let grdM_Shop;
    let grdLookUp_ShopNo;
    
    let chkShopNo = "";
 
    let AssignVar = function () {

        grdM = new DynGrid(
            {
                table_lement: $('#tbQuery')[0],
                class_collection: ["tdCol1 text-center", "tdCol2 label-align", "tdCol3 label-align", "tdCol4 label-align", "tdCol5 label-align", "tdCol6 label-align", "tdCol7 label-align", "tdCol8 label-align"],
                fields_info: [
                    { type: "Text", name: "ID", style: "" },
                    { type: "TextAmt", name: "D1"},
                    { type: "TextAmt", name: "D2" },
                    { type: "TextAmt", name: "D3" },
                    { type: "TextAmt", name: "D4" },
                    { type: "TextAmt", name: "D5" },
                    { type: "TextAmt", name: "D6" },
                    { type: "TextAmt", name: "D7" }
                ],
                //rows_per_page: 10,
                method_clickrow: click_PLU,
                afterBind: InitModifyDeleteButton,
                sortable: "N"
            }
        );

        grdM_Shop = new DynGrid(
            {
                table_lement: $('#tbDShop1')[0],
                class_collection: ["tdCol1 text-center", "tdCol2 label-align", "tdCol3 label-align", "tdCol4 label-align", "tdCol5 label-align", "tdCol6 label-align", "tdCol7 label-align", "tdCol8 label-align"],
                fields_info: [
                    { type: "Text", name: "ID", style: "" },
                    { type: "TextAmt", name: "D1" },
                    { type: "TextAmt", name: "D2" },
                    { type: "TextAmt", name: "D3" },
                    { type: "TextAmt", name: "D4" },
                    { type: "TextAmt", name: "D5" },
                    { type: "TextAmt", name: "D6" },
                    { type: "TextAmt", name: "D7" }
                ],
                //rows_per_page: 10,
                method_clickrow: click_PLU,
                //afterBind: InitModifyDeleteButton,
                sortable: "N"
            }
        );

        grdLookUp_ShopNo= new DynGrid(
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
        ////$('#tbISAM01Mod .fa-trash-o').click(function () { btPLUDelete_click(this) });
    }

//#region 店別--時段
    let Step1_click = function (bt) {
        var heads = $('#tbQuery thead tr th#th0').html();
        if (heads.toString().indexOf("時段") >= 0) {
            return;
        }

        $('#tbQuery td').closest('tr').css('background-color', 'white');
        $(bt).closest('tr').click();
        $('.msg-valid').hide();
        var node = $(grdM.ActiveRowTR()).prop('Record');
        $('#tbQuery td:contains(' + GetNodeValue(node, 'ID') + ')').closest('tr').css('background-color', '#DEEBF7');
        $('#lblDW_Shop1').html($('#tbQuery thead th#th1').html());
        $('#lblShop1').html(GetNodeValue(node, 'ID'));
        //alert($('#lblShop1').html());
           
        $('#modal_Shop1').modal('show');
        setTimeout(function () {
            QueryShop1();
        }, 500);
    };

    let QueryShop1 = function () {
        ShowLoading();
        var pData = {
            ShopNo: "'" + $('#lblShop1').html().split('-')[0] + "'",
            Flag: "T"
        }
        PostToWebApi({ url: "api/SystemSetup/MSSA106Query", data: pData, success: afterMSSA106Query_Shop1 });
    };

    let afterMSSA106Query_Shop1 = function (data) {
        CloseLoading();
        if (ReturnMsg(data, 0) != "MSSA106QueryOK") {
            DyAlert(ReturnMsg(data, 1));
        }
        else {
            var dtDelt = data.getElementsByTagName('dtDelt');
            grdM_Shop.BindData(dtDelt);
           
            if (dtDelt.length == 0) {
                DyAlert("無符合資料!");
                $(".modal-backdrop").remove();
                $('#tbDShop1 thead td#td1_Shop1').html('');
                $('#tbDShop1 thead td#td2_Shop1').html('');
                $('#tbDShop1 thead td#td3_Shop1').html('');
                $('#tbDShop1 thead td#td4_Shop1').html('');
                return;
            }
            //var dtD = data.getElementsByTagName('dtD');
            $('#tbDShop1 thead td#thead2_Shop1').html($('#tbQuery thead td#thead1').html());
            $('#tbDShop1 thead td#thead3_Shop1').html($('#tbQuery thead td#thead2').html());
            $('#tbDShop1 thead td#thead4_Shop1').html($('#tbQuery thead td#thead3').html());
            $('#tbDShop1 thead td#thead5_Shop1').html($('#tbQuery thead td#thead4').html());
            var dtSum = data.getElementsByTagName('dtSum');
            $('#tbDShop1 thead th#th2_Shop1').html($('#tbQuery thead th#th1').html());
            $('#tbDShop1 thead th#th3_Shop1').html($('#tbQuery thead th#th1').html());
            $('#tbDShop1 thead th#th4_Shop1').html($('#tbQuery thead th#th1').html());
            $('#tbDShop1 thead th#th5_Shop1').html($('#tbQuery thead th#th1').html());
            $('#tbDShop1 thead td#td1_Shop1').html(parseInt(GetNodeValue(dtSum[0], "W1")).toLocaleString('en-US'));
            $('#tbDShop1 thead td#td2_Shop1').html(parseInt(GetNodeValue(dtSum[0], "W2")).toLocaleString('en-US'));
            $('#tbDShop1 thead td#td3_Shop1').html(parseInt(GetNodeValue(dtSum[0], "W3")).toLocaleString('en-US'));
            $('#tbDShop1 thead td#td4_Shop1').html(parseInt(GetNodeValue(dtSum[0], "W4")).toLocaleString('en-US'));
        }
    };

    let btRe_Shop1_click = function (bt) {
        $('#modal_Shop1').modal('hide');
        //setTimeout(function () {
        //    ClearShop1();
        //}, 500);
    };
//#endregion

//#region 清除
    let btClear_click = function (bt) {
        //Timerset();
        $('#rdoOPTime').prop('checked', 'true');
        $('#lblShopCnt').html('');
        $('#lblShopName').html('');
        chkShopNo = "";
        $('#cbDayName').val($('#txtDeltDayNM').val());
    };
//#endregion

//#region 查詢
    let btQuery_click = function (bt) {
        //Timerset();
        $('#btQuery').prop('disabled', true)
     
        //if ($('#cbDayName').val() == "") {
        //    DyAlert("請選擇星期!", function () { $('#btQuery').prop('disabled', false); })
        //    return
        //}
        if ($('#rdoShop').prop('checked') == false && $('#rdoOPTime').prop('checked') == false) {
            DyAlert("統計條件請至少選擇一項!", function () { $('#btQuery').prop('disabled', false); })
            return
        }
        ShowLoading();

        var Flag = ""
        if ($('#rdoShop').prop('checked') == true) {
            Flag = "S";
        }
        else if ($('#rdoOPTime').prop('checked') == true) {
            Flag = "T";
        }

        setTimeout(function () {
            var pData = {
                ShopNo: chkShopNo,
                DayNM: $('#cbDayName').val(),
                Flag: Flag
            }
            PostToWebApi({ url: "api/SystemSetup/MSSA106Query", data: pData, success: afterMSSA106Query });
        }, 1000);
    };

    let afterMSSA106Query = function (data) {
        CloseLoading();
        if (ReturnMsg(data, 0) != "MSSA106QueryOK") {
            DyAlert(ReturnMsg(data, 1), function () { $('#btQuery').prop('disabled', false); });
        }
        else {
            $('#btQuery').prop('disabled', false);
            SetMSSA106Query(data);
        }
    };

    let SetMSSA106Query = function (data) {
        var dtDelt = data.getElementsByTagName('dtDelt');
        grdM.BindData(dtDelt);
        var heads = $('#tbQuery thead tr th#th0');
        if ($('#rdoShop').prop('checked')) {
            $(heads).html('店別');
        }
        else if ($('#rdoOPTime').prop('checked')) {
            $(heads).html('時段');
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
        var dtD = data.getElementsByTagName('dtD');
        $('#tbQuery thead td#thead1').html(GetNodeValue(dtD[0], "RptD1"));
        $('#tbQuery thead td#thead2').html(GetNodeValue(dtD[1], "RptD1"));
        $('#tbQuery thead td#thead3').html(GetNodeValue(dtD[2], "RptD1"));
        $('#tbQuery thead td#thead4').html(GetNodeValue(dtD[3], "RptD1"));
        $('#tbQuery thead td#thead5').html(GetNodeValue(dtD[4], "RptD1"));
        $('#tbQuery thead td#thead6').html(GetNodeValue(dtD[5], "RptD1"));
        $('#tbQuery thead td#thead7').html(GetNodeValue(dtD[6], "RptD1"));
        var dtSum = data.getElementsByTagName('dtSum');
        $('#tbQuery thead th#th1').html('週' + GetNodeValue(dtD[0], "DayWeek"));
        $('#tbQuery thead th#th2').html('週' + GetNodeValue(dtD[1], "DayWeek"));
        $('#tbQuery thead th#th3').html('週' + GetNodeValue(dtD[2], "DayWeek"));
        $('#tbQuery thead th#th4').html('週' + GetNodeValue(dtD[3], "DayWeek"));
        $('#tbQuery thead th#th5').html('週' + GetNodeValue(dtD[4], "DayWeek"));
        $('#tbQuery thead th#th6').html('週' + GetNodeValue(dtD[5], "DayWeek"));
        $('#tbQuery thead th#th7').html('週' + GetNodeValue(dtD[6], "DayWeek"));
        $('#tbQuery thead td#td1').html(parseInt(GetNodeValue(dtSum[0], "D1")).toLocaleString('en-US'));
        $('#tbQuery thead td#td2').html(parseInt(GetNodeValue(dtSum[0], "D2")).toLocaleString('en-US'));
        $('#tbQuery thead td#td3').html(parseInt(GetNodeValue(dtSum[0], "D3")).toLocaleString('en-US'));
        $('#tbQuery thead td#td4').html(parseInt(GetNodeValue(dtSum[0], "D4")).toLocaleString('en-US'));
        $('#tbQuery thead td#td5').html(parseInt(GetNodeValue(dtSum[0], "D5")).toLocaleString('en-US'));
        $('#tbQuery thead td#td6').html(parseInt(GetNodeValue(dtSum[0], "D6")).toLocaleString('en-US'));
        $('#tbQuery thead td#td7').html(parseInt(GetNodeValue(dtSum[0], "D7")).toLocaleString('en-US'));
    };
//#endregion

//#region 店別多選
    let btShopNo_click = function (bt) {
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
            $('#txtLpQ_ShopNo').val('');
            $('#modal_Lookup_ShopNo').modal('show');
            setTimeout(function () {
                grdLookUp_ShopNo.BindData(dtE);
                if (chkShopNo != "") {
                    var VIPFaceID = chkShopNo.split(',');
                    for (var i = 0; i < VIPFaceID.length; i++) {
                        $('#tbLookup_VIPFaceID tbody tr .tdCol2').filter(function () { return $(this).text() == VIPFaceID[i].replaceAll("'", ""); }).closest('tr').find('.tdCol1 input:checkbox').prop('checked', true);
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
                $(".modal-backdrop").remove();
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
            $('#lblShopCnt').html('');
            $('#lblShopName').html('');
            chkShopNo = "";
            $('#btLpOK_ShopNo').prop('disabled', false);
            $('#modal_Lookup_ShopNo').modal('hide');
            return
        } else {
            $('#lblShopCnt').html(chkedRow)
            chkShopNo = "";
            var ShopName = "";
            for (var i = 0; i < obchkedtd.length; i++) {
                var a = $(obchkedtd[i]).closest('tr');
                var trNode = $(a).prop('Record');
                chkShopNo += "'" + GetNodeValue(trNode, "ST_ID") + "',";  //已勾選的每一筆店倉
                if (i <= 4) {
                    ShopName += GetNodeValue(trNode, "ST_SName") + "，";
                }
            }
            chkShopNo = chkShopNo.substr(0, chkShopNo.length - 1)
            if (chkedRow > 5) {
                $('#lblShopName').html(ShopName.substr(0, ShopName.length - 1) + '...')
            }
            else {
                $('#lblShopName').html(ShopName.substr(0, ShopName.length - 1))
            }
            $('#btLpOK_ShopNo').prop('disabled', false);
            $('#modal_Lookup_ShopNo').modal('hide');
        }
    };

    let btLpExit_ShopNo_click = function (bt) {
        //Timerset();
        $('#modal_Lookup_ShopNo').modal('hide');
    };

    let btLpClear_ShopNo_click = function (bt) {
        //Timerset();
        $("#txtLpQ_ShopNo").val('');
        $("#tbLookup_ShopNo .checkbox").prop('checked', false);
    };
//#endregion

//#region FormLoad
    let GetInitMSSA106 = function (data) {
        if (ReturnMsg(data, 0) != "GetInitMSSA106OK") {
            DyAlert(ReturnMsg(data, 1));
        }
        else {
            var dtE = data.getElementsByTagName('dtE');
            var dtD = data.getElementsByTagName('dtD');
            if (dtE.length > 0) {
                $('#lblProgramName').html(GetNodeValue(dtE[0], "ChineseName"));
            }
            if (dtD.length > 0) {

                //$('#cbDayName').val(GetNodeValue(dtD[0], "DayWeek"));
                $('#lblDayRange').html(GetNodeValue(dtD[0], "D1") + ' ~ ' + GetNodeValue(dtD[6], "D1"));
                //alert($('#cbDayName').val());
            }
            else {
                //$('#cbDayName').val('日');
                $('#lblDayRange').html('');
            }
            AssignVar();
            $('#btQuery').click(function () { btQuery_click(this) });
            $('#btClear').click(function () { btClear_click(this) });
            $('#btShopNo').click(function () { btShopNo_click(this) });
            $('#btLpQ_ShopNo').click(function () { btLpQ_ShopNo_click(this) });
            $('#btLpOK_ShopNo').click(function () { btLpOK_ShopNo_click(this) });
            $('#btLpExit_ShopNo').click(function () { btLpExit_ShopNo_click(this) });
            $('#btLpClear_ShopNo').click(function () { btLpClear_ShopNo_click(this) });
            $('#btRe_Shop1').click(function () { btRe_Shop1_click(this) });
            //$('#rdoOPTime').prop('checked', 'true');
            SetMSSA106Query(data);

        }
    };
    
    let afterLoadPage = function () {
        //alert("OPen MSSA106");
        var pData = {
            ProgramID: "MSSA106"
        }
        PostToWebApi({ url: "api/SystemSetup/GetInitMSSA106", data: pData, success: GetInitMSSA106 });
    };
//#endregion
    

    if ($('#pgMSSA106').length == 0) {  
        AllPages = new LoadAllPages(ParentNode, "SystemSetup/MSSA106", ["pgMSSA106Init"], afterLoadPage);  //, "MSSA106btns", "pgMSSA106Add", "pgMSSA106Mod"
    };


}