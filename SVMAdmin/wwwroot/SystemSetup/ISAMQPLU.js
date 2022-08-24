PageISAMQPLU = function (ParentNode) {
    let AllPages;
    let grdU;
    let grdS;
    let EditMode;
    let SetSuspend = "";
    let isImport = false;
    let isDelete = false;
    let SysDate;
    //let OldID = "";
    
    let AssignVar = function () {
        grdU = new DynGrid(
            {
                table_lement: $('#tbVIN14_1P')[0],
                class_collection: ["tdColbt icon_in_td", "tdColbt icon_in_td", "tdCol2", "tdCol3", "tdCol4 label-align", "tdColbt icon_in_td", "tdColbt icon_in_td"],
                fields_info: [
                    { type: "JQ", name: "fa-search Q1", element: '<i class="fa fa-search Q1"></i>' },
                    { type: "JQ", name: "fa-search Q2", element: '<i class="fa fa-search Q2"></i>' },
                    { type: "Text", name: "DocNo" },
                    { type: "Text", name: "DocDate" },
                    { type: "TextAmt", name: "Qty" },
                    { type: "JQ", name: "fa-print P1", element: '<i class="fa fa-print P1"></i>' },
                    { type: "JQ", name: "fa-print P2", element: '<i class="fa fa-print P2"></i>' }
                ],
                //rows_per_page: 10,
                method_clickrow: click_PLU,
                afterBind: InitSearchButton,
                sortable: "Y"
            }
        );
        SetDateField($('#txtDocDate')[0]);
        //AssignVar1();
        //AssignVar2();
        //AssignVar3();
    };

    //let AssignVar1 = function () {
    //    grd1 = new DynGrid(
    //        {
    //            table_lement: $('#tbVIN14_1P_1')[0],
    //            class_collection: ["tdCol1", "tdCol2", "tdCol3 label-align"],
    //            fields_info: [
    //                { type: "Text", name: "SeqNo" },
    //                { type: "Text", name: "GD_NO" },
    //                { type: "TextAmt", name: "Qty" }
    //            ],
    //            //rows_per_page: 10,
    //            method_clickrow: click_PLU,
    //            afterBind: InitSearchButton,
    //            sortable: "Y"
    //        }
    //    );

    //};

    //let AssignVar2 = function () {
    //    grd2 = new DynGrid(
    //        {
    //            //2021-04-27
    //            table_lement: $('#tbVIN14_1P_2')[0],
    //            class_collection: ["tdCol1", "tdCol2", "tdCol3", "tdCol4"],
    //            fields_info: [
    //                { type: "Text", name: "WhNo" },
    //                { type: "Text", name: "Ckno" },
    //                { type: "Text", name: "SName" },
    //                { type: "Text", name: "ST_Address" }
    //            ],
    //            //rows_per_page: 10,
    //            method_clickrow: click_PLU,
    //            afterBind: InitSearchButton,
    //            sortable: "Y"
    //        }
    //    );

    //};

    //let AssignVar3 = function () {
    //    grd3 = new DynGrid(
    //        {
    //            table_lement: $('#tbVIN14_1P_3')[0],
    //            class_collection: ["tdCol1", "tdCol2", "tdCol3 label-align"],
    //            fields_info: [
    //                { type: "Text", name: "SeqNo" },
    //                { type: "Text", name: "PLU" },
    //                { type: "TextAmt", name: "Qty" }
    //            ],
    //            //rows_per_page: 10,
    //            method_clickrow: click_PLU,
    //            afterBind: InitSearchButton,
    //            sortable: "Y"
    //        }
    //    );

    //};

    //let InitSearchButton = function () {
    //    $('#tbVIN14_1P .Q1').click(function () { btDisplay1_click(this) });
    //    $('#tbVIN14_1P .Q2').click(function () { btDisplay2_click(this) });
    //    $('#tbVIN14_1P .P1').click(function () { btPrint1_click(this) });
    //    $('#tbVIN14_1P .P2').click(function () { btPrint2_click(this) });
    // }

    //let GetSales = function () {

    //    var pData = {
    //        OpenDate: $('#lblOpenDate').html(),
    //    };
    //    PostToWebApi({ url: "api/SystemSetup/GetSales", data: pData, success: AfterGetSales });
    //};




    let click_PLU = function (tr) {

    };



    //let btAdd_click = function (bt) {
    //    EditMode = "Add"
    //    $(bt).closest('tr').click();
    //    $('.msg-valid').hide();
    //    $('#modal_VMNK1 .modal-title').text('新增貨倉類型1');
    //    $('#modal_VMNK1 .btn-danger').text('儲存');
    //    $('#modal_VMNK1 .btn-primary').text('取消K');

    //    $('#Type_ID,#Type_Name,#DisplayNum,#KK').prop('readonly', false);
    //    $('#Type_ID').val("");
    //    $('#Type_Name').val("");
    //    $('#DisplayNum').val("");
    //    $('#KK').val("");
    //    $('#modal_VMNK1').modal('show');
    //};

    let btCancel_1_click = function () {
        $('#modal_VIN14_1P_1').modal('hide');
    };

    let btCancel_2_click = function () {
        $('#modal_VIN14_1P_2').modal('hide');
    };

    //let setFocus = function () {
    //    $('#Type_ID').focus();
    //}

    //let btSave_click = function () {

    //    if ($('#Type_ID').val() == "" | $('#Type_ID').val() == null | $('#Type_Name').val() == "" | $('#Type_Name').val() == null | $('#DisplayNum').val() == "" | $('#DisplayNum').val() == null) {
    //       DyAlert("所有欄位都必須輸入資料!!", function () { $('#Type_ID').focus() });
    //       //DyAlert("所有欄位都必須輸入資料!!", setFocus);
    //        return;
    //    }
    //    if ($('#Type_ID').val() == "" | $('#Type_ID').val() == null )
    //        {
    //        DyAlert("貨倉代號欄位必須輸入資料!!", function () { $('#Type_ID').focus() } );
    //        return;
    //    }
    //    if ($('#Type_Name').val() == "" | $('#Type_Name').val() == null) {
    //        DyAlert("貨倉名稱欄位必須輸入資料!!", function () { $('#Type_Name').focus() });
    //        return;
    //    }
    //    if ($('#DisplayNum').val() == "" | $('#DisplayNum').val() == null) {
    //        DyAlert("建議滿倉量欄位必須輸入資料!!", function () { $('#DisplayNum').focus() });
    //        return;
    //    }

    //    if (EditMode == "Add") {
    //       var pData = {
    //           Type_ID: $('#Type_ID').val()
    //        }

    //        PostToWebApi({ url: "api/SystemSetup/ChkRackExist", data: pData, success: AfterChkRackUsed });
    //    }
    //    else if (EditMode == "Mod") {
    //        var pData = {
    //            Type_ID: OldID
    //        }
 
    //        PostToWebApi({ url: "api/SystemSetup/ChkRackUsed", data: pData, success: AfterChkRackUsed });
    //    }
    //    else  {
    //        var cData = {
    //            Type_ID: $('#Type_ID').val()
    //        }

    //        PostToWebApi({ url: "api/SystemSetup/ChkRackUsed", data: cData, success: AfterChkRackUsed });
    //    }

    //    return
                  
    //};

 
    let afterGetInitISAMQPLU = function (data) {
        SysDate = getTodayDate();
        //AssignVar();
        //var dtArea = data.getElementsByTagName('dtArea');
        //InitSelectItem($('#cbArea')[0], dtArea, "Type_ID", "Type_Name", true, "請選擇配送區");

        //var dtPick = data.getElementsByTagName('dtPick');
        //InitSelectItem($('#cbPick')[0], dtPick, "DocNo", "DocNo", true, "請選擇撿貨單號");

        var dtShop = data.getElementsByTagName('dtShop');


        if (GetNodeValue(dtShop[0], 'WhNo') == "") {
            
            $('#txtPLU').hide();
            $('#btQuery').hide();
            $('#lblCPrice').hide();
            $('#lblPluName').hide();
            $('#lblCQty').hide();
            $('#lblCPrmPrice').hide();
            $('#lblCPeriod').hide();

            DyAlert("請先至店號設定進行作業店櫃設定!");
            return;
        }
        else if (GetNodeValue(dtShop[0], 'ST_SName') == "") {
            
            $('#txtPLU').hide();
            $('#btQuery').hide();
            $('#lblCPrice').hide();
            $('#lblPluName').hide();
            $('#lblCQty').hide();
            $('#lblCPrmPrice').hide();
            $('#lblCPeriod').hide();

            DyAlert("請確認店櫃(" + GetNodeValue(dtShop[0], 'WhNo') + ")是否為允許作業之店櫃!");
            return;
        }


        $('#lblShopNo').html(GetNodeValue(dtShop[0], 'whno'));
        $('#lblShopNo').hide();
        
        $('#lblShopName').html(GetNodeValue(dtShop[0], 'whno') + GetNodeValue(dtShop[0], 'ST_SName') );

        //$('#txtDocDate').val(SysDate)

        $('#btQuery').click(function () { SearchISAMQPLU(); });
   
        $('.forminput input').change(function () { InputValidation(this) });

        $('#txtPLU').keypress(function (e) {
            //$('#icrpwd').hide();
            if (e.which == 13) {
                SearchISAMQPLU(); e
            }
        });


        $('#txtPLU').focus();

    };

    let SearchISAMQPLU = function () {
        ShowLoading();
        if ($('#txtPLU').val() == "" || $('#txtPLU').val() == null)
            {
            CloseLoading();
            DyAlert("請先輸入查詢條件!!");
            }
            //$('#lblArea').html("")
        else
            //$('#lblArea').html($('#cbArea').val())
            GetISAMQPLU()
    };

    let GetISAMQPLU = function () {
        var pData = {
            //Area: $('#cbArea').val(),
            WhNo: $('#lblShopNo').text(),
            PLU: $('#txtPLU').val(),
        };
        PostToWebApi({ url: "api/SystemSetup/GetISAMQPLU", data: pData, success: AfterGetISAMQPLU });
    };

    let AfterGetISAMQPLU = function (data) {
        CloseLoading();
        //alert("AfterGetVIN14_1P");
        if (ReturnMsg(data, 0) != "GetISAMQPLUOK") {
            DyAlert(ReturnMsg(data, 0));
            return;
        }
        else {
            var dtQPLU = data.getElementsByTagName('dtQPLU');
            //grdU.BindData(dtQPLU);
            //alert("ggg");
            if (dtQPLU.length == 0) {
                //alert("AfterGetISAMQPLU");
                $('#lblPrice').html("");
                $('#lblPluName').html("");
                $('#lblQty').html("");
                $('#lblPrmPrice').html("");
                $('#lblPeriod').html("");
                DyAlert("無符合資料!", BlankMode);
                return;
            }
            else {
                $('#lblPrice').html(parseInt(GetNodeValue(dtQPLU[0], 'GD_Retail')));
                $('#lblPluName').html(GetNodeValue(dtQPLU[0], 'GD_Name'));
                $('#lblQty').html(GetNodeValue(dtQPLU[0], 'PTNum'));

                var dtPrm = data.getElementsByTagName('dtPrm');
                if (dtPrm.length != 0) {
                    //$('#lblPrmPrice').html(GetNodeValue(dtPrm[0], 'Promote'));
                    $('#lblPrmPrice').html(parseInt(GetNodeValue(dtPrm[0], 'Promote')));
                    $('#lblPeriod').html(GetNodeValue(dtPrm[0], 'StartDate') + '~' + GetNodeValue(dtPrm[0], 'EndDate'));
                }
                else {
                    $('#lblPrmPrice').html("-");
                    $('#lblPeriod').html("-");
                }

            }
        }
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

        if ($(ip).attr('id') == "txtPLU") {
            if (str.length > 10)
                msg = "必須10字元以內";
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
        PostToWebApi({ url: "api/SystemSetup/GetInitISAMQPLU", success: afterGetInitISAMQPLU });
        $('#pgISAMQPLU').show();
    };

    if ($('#pgISAMQPLU').length == 0) {
        //2021-04-29 Debug用，按F12後，在主控台內會顯示aaaaaaVMN29
        //alert("LoadPage");
        AllPages = new LoadAllPages(ParentNode, "SystemSetup/ISAMQPLU", ["pgISAMQPLU"], afterLoadPage);
    };


}