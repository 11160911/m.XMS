var PageMSSD106 = function (ParentNode) {

    let grdM;
    let grdMW;
    let grdLookUp_VIPFaceID;
    let grdLookUp_City;
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
                    { type: "TextPercent", name: "Cnt2p" },
                    { type: "TextAmt", name: "Cnt3" },
                    { type: "TextPercent", name: "Cnt3p" },
                    { type: "TextAmt", name: "Cnt4" },
                    { type: "TextPercent", name: "Cnt4p" },
                    { type: "TextAmt", name: "Cnt5" },
                    { type: "TextPercent", name: "Cnt5p" },
                    { type: "TextAmt", name: "Cnt6" },
                    { type: "TextPercent", name: "Cnt6p" },
                    { type: "TextAmt", name: "Cnt7" },
                    { type: "TextPercent", name: "Cnt7p" },
                    { type: "TextAmt", name: "Cnt8" },
                    { type: "TextPercent", name: "Cnt8p" },
                    { type: "TextAmt", name: "Cnt9" },
                    { type: "TextPercent", name: "Cnt9p" },
                    { type: "TextAmt", name: "Cnt10" },
                    { type: "TextPercent", name: "Cnt10p" }
                ],
                //rows_per_page: 10,
                method_clickrow: click_PLU,
                afterBind: InitModifyDeleteButton,
                sortable: "N",
                step: "Y"
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
                    { type: "TextPercent", name: "Cnt2p" },
                    { type: "TextAmt", name: "Cnt3" },
                    { type: "TextPercent", name: "Cnt3p" },
                    { type: "TextAmt", name: "Cnt4" },
                    { type: "TextPercent", name: "Cnt4p" },
                    { type: "TextAmt", name: "Cnt5" },
                    { type: "TextPercent", name: "Cnt5p" },
                    { type: "TextAmt", name: "Cnt6" },
                    { type: "TextPercent", name: "Cnt6p" },
                    { type: "TextAmt", name: "Cnt7" },
                    { type: "TextPercent", name: "Cnt7p" },
                    { type: "TextAmt", name: "Cnt8" },
                    { type: "TextPercent", name: "Cnt8p" }
                ],
                //rows_per_page: 10,
                method_clickrow: click_PLU,
                afterBind: InitModifyDeleteButton,
                sortable: "N",
                step: "Y"
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
                sortable: "N",
                step: "Y"
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
                sortable: "N",
                step: "Y"
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
                    //$(".modal-backdrop").remove();
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
                    //$(".modal-backdrop").remove();
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
                //$(".modal-backdrop").remove();
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
        ClearQuery();
        if (chkedRow == 0) {
            $('#lblVIPFaceIDCnt').html('');
            $('#lblVIPFaceIDName').html('');
            chkVIPFaceID = "";
            $('#btLpOK_VIPFaceID').prop('disabled', false);
            $('#modal_Lookup_VIPFaceID').modal('hide');
            return
        } else {
            chkVIPFaceID = "";
            var VIPFaceIDName = "";
            for (var i = 0; i < obchkedtd.length; i++) {
                var a = $(obchkedtd[i]).closest('tr');
                var trNode = $(a).prop('Record');
                chkVIPFaceID += "'" + GetNodeValue(trNode, "ST_ID") + "',";  //已勾選的每一筆店倉
                if (i <= 4) {
                    VIPFaceIDName += GetNodeValue(trNode, "ST_SName") + "，";
                }
            }
            chkVIPFaceID = chkVIPFaceID.substr(0, chkVIPFaceID.length - 1)
            if (chkedRow > 5) {
                $('#lblVIPFaceIDCnt').html(chkedRow)
                $('#lblVIPFaceIDName').html(VIPFaceIDName.substr(0, VIPFaceIDName.length - 1) + '...')
            }
            else {
                $('#lblVIPFaceIDCnt').html('');
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
                //$(".modal-backdrop").remove();
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
        ClearQuery();
        if (chkedRow == 0) {
            $('#lblCityCnt').html('');
            $('#lblCityName').html('');
            chkCity = "";
            $('#btLpOK_City').prop('disabled', false);
            $('#modal_Lookup_City').modal('hide');
            return
        } else {
            chkCity = "";
            var CityName = "";
            for (var i = 0; i < obchkedtd.length; i++) {
                var a = $(obchkedtd[i]).closest('tr');
                var trNode = $(a).prop('Record');
                chkCity += "'" + GetNodeValue(trNode, "City") + "',";  //已勾選的每一筆店倉
                if (i <= 4) {
                    CityName += GetNodeValue(trNode, "City") + "，";
                }
            }
            chkCity = chkCity.substr(0, chkCity.length - 1)
            if (chkedRow > 5) {
                $('#lblCityCnt').html(chkedRow)
                $('#lblCityName').html(CityName.substr(0, CityName.length - 1) + '...')
            }
            else {
                $('#lblCityCnt').html('')
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

    let ClearQuery = function () {
        $('#tbQuery thead tr th').css('background-color', '#ffb620')
        $('#tbQueryMW thead tr th').css('background-color', '#ffb620')
        grdM.BindData(null)
        grdMW.BindData(null)

        var sumtdQ = document.querySelector('.QSum');
        for (i = 0; i < sumtdQ.childElementCount; i++) {
            if (i == 0) {
                sumtdQ.children[i].innerHTML = "總數";
            }
            else {
                sumtdQ.children[i].innerHTML = "";
            }
        }
        var sumtdQ_MW = document.querySelector('.QSumMW');
        for (i = 0; i < sumtdQ_MW.childElementCount; i++) {
            if (i == 0) {
                sumtdQ_MW.children[i].innerHTML = "總數";
            }
            else {
                sumtdQ_MW.children[i].innerHTML = "";
            }
        }

        if ($('#rdoShop').prop('checked') == true || $('#rdoCity').prop('checked') == true) {
            $('#tbQuery').show();
            $('#tbQueryMW').hide();

            var heads = $('#tbQuery thead tr th#thtype');
            if ($('#rdoShop').prop('checked')) {
                $(heads).html('店別');
            }
            else if ($('#rdoCity').prop('checked')) {
                $(heads).html('縣市');
            }
        }
        else if ($('#rdoMW').prop('checked') == true) {
            $('#tbQuery').hide();
            if ($('#tbQueryMW').attr('hidden') == undefined) {
                $('#tbQueryMW').show();
            }
            else {
                $('#tbQueryMW').removeAttr('hidden');
                $('#tbQueryMW').show();
            }
        }
    }
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

            $('#rdoDateAll,#rdoDate2M,#rdoDate3M,#rdoDate6M,#rdoDate1Y').change(function () { ClearQuery() });
            $('#rdoShop,#rdoCity,#rdoMW').change(function () { btQuery_click() });
            btQuery_click();
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