﻿var PageVSA76P = function (ParentNode) {
    let AllPages;
    let grdU;
    let grdS;
    let EditMode;
    let SetSuspend = "";
    let isImport = false;
    let isDelete = false;
    //let actMode = "";
    let OldID = "";
    let QueryDays;
    let SysDate;
    
    let AssignVar = function () {
        grdU = new DynGrid(
            {
                //2021-04-27
                table_lement: $('#tbVSA76P')[0],
                class_collection: ["tdColbt icon_in_td", "tdCol3 label-align", "tdCol4", "tdCol5", "tdCol6", "tdCol7 label-align", "tdCol8 label-align"],
                fields_info: [
                    { type: "JQ", name: "fa-search", element: '<i class="fa fa-search"></i>' },
                    { type: "TextAmt", name: "SeqNo" },
                    { type: "Text", name: "ShopNo" },
                    { type: "Text", name: "CkNo" },
                    { type: "Text", name: "ST_SName" },
                    { type: "TextAmt", name: "Num" },
                    { type: "TextAmt", name: "Cash" }
                ],
                //rows_per_page: 10,
                method_clickrow: click_PLU,
                afterBind: InitSearchButton,
                sortable: "Y"
            }
        );
        SetDateField($('#txtOpenDateS')[0]);
        SetDateField($('#txtOpenDateE')[0]);
        AssignVarS(); 
    };

    let AssignVarS = function () {
        grdS = new DynGrid(
            {
                //2021-04-27
                table_lement: $('#tbVSA76P_Search')[0],
                class_collection: ["tdCol1", "tdCol2", "tdCol3", "tdCol4", "tdCol5  label-align", "tdCol6 label-align", "tdCol7"],
                fields_info: [
                    { type: "Text", name: "chrno" },
                    { type: "Text", name: "opendate" },
                    { type: "Text", name: "layer" },
                    { type: "Text", name: "goodsno" },
                    { type: "TextAmt", name: "num" },
                    { type: "TextAmt", name: "cash" },
                    { type: "Text", name: "pay_type" }
                ],
                /*rows_per_page: 5,*/
                method_clickrow: click_PLU,
                afterBind: InitSearchButton,
                sortable: "Y"
            }
        );

    };

    let InitSearchButton = function () {
        $('#tbVSA76P .fa-search').click(function () { btDisplay_click(this) });
        //$('#tbVSA76_1P .fa-trash-o').click(function () { btDelete_click(this) });
    }

    let GetVSA76P = function () {

        var pData = {
            OpenDateS: $('#txtOpenDateS').val(),
            OpenDateE: $('#txtOpenDateE').val(),
        };
        PostToWebApi({ url: "api/AIReports/GetVSA76P", data: pData, success: AfterGetVSA76P });
    };

    let GetVSA76PSearch = function () {
        var nodeS = $(grdU.ActiveRowTR()).prop('Record');
        var pData = {
            ShopNo: GetNodeValue(nodeS, 'ShopNo'),
            OpenDateS: $('#lblOpenDate').html().substr(0, 10),
            OpenDateE: $('#lblOpenDate').html().substr(11),
            CkNo: GetNodeValue(nodeS, 'CkNo'),
        };
        PostToWebApi({ url: "api/AIReports/GetVSA76PSearch", data: pData, success: AfterGetVSA76PSearch });
    };

    let click_PLU = function (tr) {

    };

    let AfterGetVSA76P = function (data) {

        if (ReturnMsg(data, 0) != "GetVSA76POK") {
            DyAlert(ReturnMsg(data, 0));
            return;
        }
        else {

            var dtVSA76P = data.getElementsByTagName('dtVSA76P');
            grdU.BindData(dtVSA76P);

            if (dtVSA76P.length == 0) {
                //DyAlert("無符合資料!", BlankMode);
                $('#lblSumNum').html("")
                $('#lblSumCash').html("")
                return;
            }
            else {
                var Num = 0;
                var Cash = 0;
                for (var i = 0; i < dtVSA76P.length; i++) {
                    Num += parseFloat(GetNodeValue(dtVSA76P[i], 'Num'));
                    Cash += parseFloat(GetNodeValue(dtVSA76P[i], 'Cash'));
                }
                $('#lblSumNum').html((Num).toLocaleString('en-US'))
                $('#lblSumCash').html((Cash).toLocaleString('en-US'))
            }

        }
    };

    let AfterGetVSA76PSearch = function (data) {

        if (ReturnMsg(data, 0) != "GetVSA76PSearchOK") {
            DyAlert(ReturnMsg(data, 0));
            return;
        }
        else {

            var dtVSA76PSearch = data.getElementsByTagName('dtVSA76PSearch');
            grdS.BindData(dtVSA76PSearch);

            if (dtVSA76PSearch.length == 0) {
                //DyAlert("無符合資料!", BlankMode);
                return;
            }
            
        }
    };
        
    let btDisplay_click = function (bt) {

        $(bt).closest('tr').click();
        $('.msg-valid').hide();
        $('#modal_VSA76P .modal-title').text('商品銷售明細查詢');

        var node = $(grdU.ActiveRowTR()).prop('Record');
        var NumD = 0;
        var CashD = 0;
        $('#ShopNo').html(GetNodeValue(node, 'ShopNo') + '店 ' + GetNodeValue(node, 'CkNo') + '機 ' + GetNodeValue(node, 'ST_SName'));
        $('#OpenDate').html($('#lblOpenDate').html());

        NumD = parseFloat(GetNodeValue(node, 'Num'))
        CashD = parseFloat(GetNodeValue(node, 'Cash'))
        $('#Num').html((NumD).toLocaleString('en-US'));
        $('#Cash').html((CashD).toLocaleString('en-US'));

        $('#modal_VSA76P').modal('show');
        setTimeout(function () { GetVSA76PSearch(); }, 500);
    };

    let btQuery_click = function () {
        if ($('#txtOpenDateS').val() == "") {
            $('#txtOpenDateS').val() == ""
            DyAlert("請輸入銷售日期!!", function () { $('#txtOpenDateS').focus() });
            return;
        }
        else if ($('#txtOpenDateE').val() == "") {
            $('#txtOpenDateE').val() == ""
            DyAlert("請輸入銷售日期!!", function () { $('#txtOpenDateE').focus() });
            return;
        }
        else {
            if (DateDiff("d", $('#txtOpenDateS').val(), $('#txtOpenDateE').val()) > parseInt(QueryDays)) {
                DyAlert("銷售日期查詢區間必須小於等於" + QueryDays + "天!!");
                return;
            }
            $('#lblOpenDate').html($('#txtOpenDateS').val() + "~" + $('#txtOpenDateE').val())
            GetVSA76P()
        }
    };

    let btAdd_click = function (bt) {
        EditMode = "Add"
        $(bt).closest('tr').click();
        $('.msg-valid').hide();
        $('#modal_VMNK1 .modal-title').text('新增貨倉類型1');
        $('#modal_VMNK1 .btn-danger').text('儲存');
        $('#modal_VMNK1 .btn-primary').text('取消K');

        $('#Type_ID,#Type_Name,#DisplayNum,#KK').prop('readonly', false);
        $('#Type_ID').val("");
        $('#Type_Name').val("");
        $('#DisplayNum').val("");
        $('#KK').val("");
        $('#modal_VMNK1').modal('show');
    };

    let btCancel_click = function () {
        //2021-04-27
        $('#modal_VSA76P').modal('hide');
    };

    let setFocus = function () {
        $('#Type_ID').focus();
    }

    let btSave_click = function () {

        if ($('#Type_ID').val() == "" | $('#Type_ID').val() == null | $('#Type_Name').val() == "" | $('#Type_Name').val() == null | $('#DisplayNum').val() == "" | $('#DisplayNum').val() == null) {
           DyAlert("所有欄位都必須輸入資料!!", function () { $('#Type_ID').focus() });
           //DyAlert("所有欄位都必須輸入資料!!", setFocus);
            return;
        }
        if ($('#Type_ID').val() == "" | $('#Type_ID').val() == null )
            {
            DyAlert("貨倉代號欄位必須輸入資料!!", function () { $('#Type_ID').focus() } );
            return;
        }
        if ($('#Type_Name').val() == "" | $('#Type_Name').val() == null) {
            DyAlert("貨倉名稱欄位必須輸入資料!!", function () { $('#Type_Name').focus() });
            return;
        }
        if ($('#DisplayNum').val() == "" | $('#DisplayNum').val() == null) {
            DyAlert("建議滿倉量欄位必須輸入資料!!", function () { $('#DisplayNum').focus() });
            return;
        }

        if (EditMode == "Add") {
           var pData = {
               Type_ID: $('#Type_ID').val()
            }

            PostToWebApi({ url: "api/SystemSetup/ChkRackExist", data: pData, success: AfterChkRackUsed });
        }
        else if (EditMode == "Mod") {
            var pData = {
                Type_ID: OldID
            }
 
            PostToWebApi({ url: "api/SystemSetup/ChkRackUsed", data: pData, success: AfterChkRackUsed });
        }
        else  {
            var cData = {
                Type_ID: $('#Type_ID').val()
            }

            PostToWebApi({ url: "api/SystemSetup/ChkRackUsed", data: cData, success: AfterChkRackUsed });
        }

        return
                  
    };

    let AfterChkRackUsed = function (data) {
        if (ReturnMsg(data, 0) != "ChkRackUsedOK") {
            DyAlert(ReturnMsg(data, 1));
        }
        else {
            var dtRack = data.getElementsByTagName("dtRack");
            //alert("Rack Rows:" + dtRack.length);

            if (EditMode == "Mod") {
                //alert("Mod OldID:" + OldID);
                if (OldID != $('#Type_ID').val()) {
                    if (dtRack.length > 0) {
                        DyAlert("貨倉代號已被引用，無法修改!!")
                        return;
                    }
                }

  
                var pData = {
                    Rack: [
                        {
                            OldType_ID: OldID,
                            Type_ID: $('#Type_ID').val(),
                            Type_Name: $('#Type_Name').val(),
                            DisplayNum: $('#DisplayNum').val()
                        }
                    ]
                };
                PostToWebApi({ url: "api/SystemSetup/UpdateRack", data: pData, success: AfterUpdateRack });
            }
            else if (EditMode == "Add") {
                if (dtRack.length > 0) {
                    DyAlert("貨倉代號已存在，無法新增!!")
                    return;
                }
                var pData = {
                    Rack: [
                        {
                            Type_ID: $('#Type_ID').val(),
                            Type_Name: $('#Type_Name').val(),
                            DisplayNum: $('#DisplayNum').val(),
                            companycode: $('#KK').val()
                        }
                    ]
                };
                PostToWebApi({ url: "api/SystemSetup/AddRack", data: pData, success: AfterAddRack });
            }
            else if (EditMode == "Del") {
                if (dtRack.length > 0) {
                    DyAlert("貨倉代號已被引用，無法刪除!!")
                    return;
                }

                var pData = {
                    Rack: [
                        {
                            Type_ID: $('#Type_ID').val()
                        }
                    ]
                };
                
                PostToWebApi({ url: "api/SystemSetup/DelRack", data: pData, success: AfterDelRack });
            }
                
            //DyAlert("匯入完成!");
            //$('#modal_VMN29').modal('hide');
        }
    };

    //2021-04-27
    let afterGetInitVSA76P = function (data) {
        SysDate = getTodayDate();
        $('#txtOpenDateS').val(SysDate);
        $('#txtOpenDateE').val(SysDate)
        $('#lblOpenDate').html(SysDate + "~" + SysDate)
        /*$('#txtOpenDateS').prop('readonly', true);*/

        AssignVar();
        GetQueryDays();
        $('#btQuery').click(function () { btQuery_click() });
        $('#btSave').click(function () { btSave_click(); });
        $('#btCancel').click(function () { btCancel_click(); });
        $('#btAddRack').click(function () { btAdd_click(); });
        $('.forminput input').change(function () { InputValidation(this) });

    };




    let SetPLUAutoComplete = function (inputID, apiPath) {
        var divmenu = $("<div></div>");
        divmenu.prop('id', 'EMAC' + inputID);
        divmenu.css("display", "block");
        divmenu.css("position", "relative");
        $('#' + inputID).after(divmenu);
        if (apiPath == null)
            apiPath = "";
        $('#' + inputID).autocomplete({
            position: { my: "left top", at: "left bottom" },
            appendTo: "#" + divmenu.prop('id'),
            source: function (request, response) {
                $.ajax({
                    url: apiPath + "api/SystemSetup/GetPLUFromIXms",
                    method: "POST",
                    contentType: "application/x-www-form-urlencoded; charset=UTF-8",
                    headers: { "Authorization": "Bearer " + UU },
                    dataType: "json",
                    data: {
                        "term": request.term
                    },
                    success: function (data) {
                        response($.map(data.list, function (item) {
                            return {
                                label: item.value + ' ' + item.label,
                                value: item.value,
                                display: item.label
                            }
                        }));
                    }
                });

            },
            select: function (event, ui) {
                $("#GD_NO").val(ui.item.value);
                $("#GD_NAME").val(ui.item.display);
                return false;
            }
        });
    }

    let InputValidation = function (ip) {
        var str = $(ip).val();
        var msg = "";
        $(ip).nextAll('.msg-valid').text(msg);
        $(ip).nextAll('.msg-valid').show();
        if (str == "")
            return;
        if ($(ip).attr('id') == "Type_ID") {
            $(ip).val($(ip).val().toUpperCase());
            var re = /^[\d|a-zA-Z]+$/;
            if (!re.test(str) | str.length > 2 )
                msg = "必須2碼內英數字";
        }
        if ($(ip).attr('id') == "Type_Name") {
            if (str.length > 20)
                msg = "必須20字元以內";
        }
        if ($(ip).attr('id') == "DisplayNum") {
            var re = /^[\d]+$/;
            if (!re.test(str) | str.length > 2 | str < 0 )
                msg = "必須2位內正整數";
        }
            if (msg != "") {
            $(ip).nextAll('.msg-valid').text(msg);
            $(ip).nextAll('.msg-valid').show();
        }
    }

    function getTodayDate() {
        var fullDate = new Date();
        var yyyy = fullDate.getFullYear();
        var MM = (fullDate.getMonth() + 1) >= 10 ? (fullDate.getMonth() + 1) : ("0" + (fullDate.getMonth() + 1));
        var dd = fullDate.getDate() < 10 ? ("0" + fullDate.getDate()) : fullDate.getDate();
        var today = yyyy + "/" + MM + "/" + dd;
        return today;
    }

    let afterLoadPage = function () {
        //2021-04-27
        //alert("afterLoadPage");
        PostToWebApi({ url: "api/AIReports/GetInitVSA76P", success: afterGetInitVSA76P });
        $('#pgVSA76P').show();
    };

    if ($('#pgVSA76P').length == 0) {
        //2021-04-29 Debug用，按F12後，在主控台內會顯示aaaaaaVMN29
        //console.log("aaaaaaVMN29");
        AllPages = new LoadAllPages(ParentNode, "AIReports/VSA76P", ["pgVSA76P"], afterLoadPage);
    };

    let GetQueryDays = function () {
        var qData = {
            ProgramID: "VSA76P",
        }
        PostToWebApi({ url: "api/SystemSetup/GetQueryDays", data: qData, success: AfterGetQueryDays });
    };

    let AfterGetQueryDays = function (data) {
        if (ReturnMsg(data, 0) != "GetQueryDaysOK") {
            DyAlert(ReturnMsg(data, 0));
            return;
        }
        else {
            var dtQueryDays = data.getElementsByTagName('dtQueryDays');
            if (dtQueryDays.length == 0) {
                QueryDays = 90;
                //DyAlert("無符合資料!", BlankMode);
                //return;
            }
            else {
                if (GetNodeValue(dtQueryDays[0], "QueryDays") != 0)
                    QueryDays = GetNodeValue(dtQueryDays[0], "QueryDays");
                else 
                    QueryDays = 90;
            }
        }
    };
}