PageVIN14_1P = function (ParentNode) {
    let AllPages;
    let grdU;
    let grdS;
    let EditMode;
    let SetSuspend = "";
    let isImport = false;
    let isDelete = false;
    let SysDate;
    let OldID = "";
    
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
        AssignVar1();
        AssignVar2();
        AssignVar3();
    };

    let AssignVar1 = function () {
        grd1 = new DynGrid(
            {
                table_lement: $('#tbVIN14_1P_1')[0],
                class_collection: ["tdCol1", "tdCol2", "tdCol3 label-align"],
                fields_info: [
                    { type: "Text", name: "SeqNo" },
                    { type: "Text", name: "GD_NO" },
                    { type: "TextAmt", name: "Qty" }
                ],
                //rows_per_page: 10,
                method_clickrow: click_PLU,
                afterBind: InitSearchButton,
                sortable: "Y"
            }
        );

    };

    let AssignVar2 = function () {
        grd2 = new DynGrid(
            {
                //2021-04-27
                table_lement: $('#tbVIN14_1P_2')[0],
                class_collection: ["tdCol1", "tdCol2", "tdCol3", "tdCol4"],
                fields_info: [
                    { type: "Text", name: "WhNo" },
                    { type: "Text", name: "Ckno" },
                    { type: "Text", name: "SName" },
                    { type: "Text", name: "ST_Address" }
                ],
                //rows_per_page: 10,
                method_clickrow: click_PLU,
                afterBind: InitSearchButton,
                sortable: "Y"
            }
        );

    };

    let AssignVar3 = function () {
        grd3 = new DynGrid(
            {
                table_lement: $('#tbVIN14_1P_3')[0],
                class_collection: ["tdCol1", "tdCol2", "tdCol3 label-align"],
                fields_info: [
                    { type: "Text", name: "SeqNo" },
                    { type: "Text", name: "PLU" },
                    { type: "TextAmt", name: "Qty" }
                ],
                //rows_per_page: 10,
                method_clickrow: click_PLU,
                afterBind: InitSearchButton,
                sortable: "Y"
            }
        );

    };

    let InitSearchButton = function () {
        $('#tbVIN14_1P .Q1').click(function () { btDisplay1_click(this) });
        $('#tbVIN14_1P .Q2').click(function () { btDisplay2_click(this) });
        $('#tbVIN14_1P .P1').click(function () { btPrint1_click(this) });
        $('#tbVIN14_1P .P2').click(function () { btPrint2_click(this) });
     }

    let GetSales = function () {

        var pData = {
            OpenDate: $('#lblOpenDate').html(),
        };
        PostToWebApi({ url: "api/SystemSetup/GetSales", data: pData, success: AfterGetSales });
    };

    let GetVIN14_1P_1 = function () {

        var node1 = $(grdU.ActiveRowTR()).prop('Record');
        var pData = {
            DocNo: GetNodeValue(node1, 'DocNo'),
        };
        PostToWebApi({ url: "api/SystemSetup/GetVIN14_1P_1", data: pData, success: AfterGetVIN14_1P_1 });
    };

    let GetVIN14_1P_2 = function () {

        var node2 = $(grdU.ActiveRowTR()).prop('Record');
        var pData = {
            DocNo: GetNodeValue(node2, 'DocNo'),
        };
        PostToWebApi({ url: "api/SystemSetup/GetVIN14_1P_2", data: pData, success: AfterGetVIN14_1P_2 });
    };

    let GetVIN14_1P_3 = function () {

        var node3 = $(grdU.ActiveRowTR()).prop('Record');
        var pData = {
            DocNo: GetNodeValue(node3, 'DocNo'),
        };
        PostToWebApi({ url: "api/SystemSetup/GetVIN14_1P_3", data: pData, success: AfterGetVIN14_1P_3 });
    };

    let click_PLU = function (tr) {

    };

    let AfterGetSales = function (data) {

        if (ReturnMsg(data, 0) != "GetSalesOK") {
            DyAlert(ReturnMsg(data, 0));
            return;
        }
        else {

            var dtSales = data.getElementsByTagName('dtSales');
            grdU.BindData(dtSales);

            if (dtSales.length == 0) {
                //DyAlert("無符合資料!", BlankMode);
                return;
            }

        }
    };

    let AfterGetVIN14_1P_1 = function (data) {

        if (ReturnMsg(data, 0) != "GetVIN14_1P_1OK") {
            DyAlert(ReturnMsg(data, 0));
            return;
        }
        else {
            var dtVIN14_1P_1 = data.getElementsByTagName('dtVIN14_1P_1');
            grd1.BindData(dtVIN14_1P_1);

            if (dtVIN14_1P_1.length == 0) {
                //DyAlert("無符合資料!", BlankMode);
                return;
            }
        }
    };

    let AfterGetVIN14_1P_2 = function (data) {

        if (ReturnMsg(data, 0) != "GetVIN14_1P_2OK") {
            DyAlert(ReturnMsg(data, 0));
            return;
        }
        else {
            var dtVIN14_1P_2 = data.getElementsByTagName('dtVIN14_1P_2');
            grd2.BindData(dtVIN14_1P_2);
            $('#lblArea_1').html(GetNodeValue(dtVIN14_1P_2[0], 'ST_DeliArea'))

            if (dtVIN14_1P_2.length == 0) {
                //DyAlert("無符合資料!", BlankMode);
                return;
            }
        }
    };

    let AfterGetVIN14_1P_3 = function (data) {

        if (ReturnMsg(data, 0) != "GetVIN14_1P_3OK") {
            DyAlert(ReturnMsg(data, 0));
            return;
        }
        else {
            var dtVIN14_1P_3 = data.getElementsByTagName('dtVIN14_1P_3');
            grd3.BindData(dtVIN14_1P_3);

            if (dtVIN14_1P_3.length == 0) {
                //DyAlert("無符合資料!", BlankMode);
                return;
            }
            else {
                var Qty = 0;
                $('#lblSName_2').html(GetNodeValue(dtVIN14_1P_3[0], 'Name'));
                for (var i = 0; i < dtVIN14_1P_3.length; i++)
                {
                    Qty += parseFloat(GetNodeValue(dtVIN14_1P_3[i], 'Qty'));
                }
                $('#lblQty_2').html(Qty)
            }

        }
    };

    let btDisplay1_click = function (bt) {
        
        $(bt).closest('tr').click();
        $('.msg-valid').hide();
        $('#modal_VIN14_1P_1 .modal-title').text('智販機撿貨單查詢');

        var node = $(grdU.ActiveRowTR()).prop('Record');
        /*$('#ShopNo,#OpenDate,#Cash').prop('readonly', true);*/
        $('#lblPick_1').html(GetNodeValue(node, 'DocNo'));
        $('#lblQty_1').html(GetNodeValue(node, 'Qty'));
        $('#modal_VIN14_1P_1').modal('show');
        setTimeout(function () { GetVIN14_1P_1(); }, 500);
        setTimeout(function () { GetVIN14_1P_2(); }, 500);
    };

    let btDisplay2_click = function (bt) {
        
        $(bt).closest('tr').click();
        $('.msg-valid').hide();
        $('#modal_VIN14_1P_2 .modal-title').text('智販機撿貨單查詢');

        var node = $(grdU.ActiveRowTR()).prop('Record');
        /*$('#ShopNo,#OpenDate,#Cash').prop('readonly', true);*/
        $('#lblPick_2').html(GetNodeValue(node, 'DocNo'));

        $('#modal_VIN14_1P_2').modal('show');
        setTimeout(function () { GetVIN14_1P_3(); }, 500);
    };

    let btPrint1_click = function (bt) {
        $(bt).closest('tr').click();
        $('.msg-valid').hide();
        var nodP1 = $(grdU.ActiveRowTR()).prop('Record');
        alert("商品彙總撿貨單 列印日期:" + SysDate + " 補貨店倉:" + $('#lblShopNo').html() + " 撿貨單號:" + GetNodeValue(nodP1, 'DocNo'))
    };

    let btPrint2_click = function (bt) {
        $(bt).closest('tr').click();
        $('.msg-valid').hide();
        var nodP2 = $(grdU.ActiveRowTR()).prop('Record');
        alert("單機商品撿貨單 列印日期:" + SysDate + " 補貨店倉:" + $('#lblShopNo').html() + " 撿貨單號:" + GetNodeValue(nodP2, 'DocNo'))
    };

    let btDelete_click = function (bt) {
        EditMode = "Del"
        $(bt).closest('tr').click();
        $('.msg-valid').hide();
        $('#modal_VMNK1 .modal-title').text('刪除貨倉類型KK');
        $('#modal_VMNK1 .btn-danger').text('刪除KK');

        var node = $(grdU.ActiveRowTR()).prop('Record');
        $('#Type_ID,#Type_Name,#DisplayNum,#KK').prop('readonly', true);
        $('#Type_ID').val(GetNodeValue(node, 'Type_ID'));
        $('#Type_Name').val(GetNodeValue(node, 'Type_Name'));
        $('#DisplayNum').val(GetNodeValue(node, 'DisplayNum'));
        $('#KK').val(GetNodeValue(node, 'companycode'));
 
        $('#modal_VMNK1').modal('show');
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

    let btCancel_1_click = function () {
        $('#modal_VIN14_1P_1').modal('hide');
    };

    let btCancel_2_click = function () {
        $('#modal_VIN14_1P_2').modal('hide');
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


    let AfterUpdateRack = function (data) {
        if (ReturnMsg(data, 0) != "UpdateRackOK") {
            DyAlert(ReturnMsg(data, 1));
        }
        else {
            DyAlert("儲存完成!");
            
            $('#modal_VMNK1').modal('hide');
            var userxml = data.getElementsByTagName('dtRack')[0];
            grdU.RefreshRocord(grdU.ActiveRowTR(), userxml);
        }
    };

    let AfterDelRack = function (data) {
        if (ReturnMsg(data, 0) != "DelRackOK") {
            DyAlert(ReturnMsg(data, 1));
        }
        else {
            DyAlert("刪除完成!");

            $('#modal_VMNK1').modal('hide');
            //var userxml = data.getElementsByTagName('dtRack')[0];
            grdU.DeleteRow(grdU.ActiveRowTR());
        }
    };

    let AfterAddRack = function (data) {
        if (ReturnMsg(data, 0) != "AddRackOK") {
            DyAlert(ReturnMsg(data, 1));
        }
        else {
            DyAlert("新增完成!");

            $('#modal_VMNK1').modal('hide');
            var userxml = data.getElementsByTagName('dtRack')[0];
            grdU.AddNew( userxml);
        }
    };

    //2021-04-27
    let afterGetInitVIN14_1P = function (data) {
        SysDate = getTodayDate();
        AssignVar();
        var dtArea = data.getElementsByTagName('dtArea');
        InitSelectItem($('#cbArea')[0], dtArea, "Type_ID", "Type_Name", true, "請選擇配送區");

        var dtPick = data.getElementsByTagName('dtPick');
        InitSelectItem($('#cbPick')[0], dtPick, "DocNo", "DocNo", true, "請選擇撿貨單號");

        var dtShop = data.getElementsByTagName('dtShop');
        $('#lblShopNo').html(GetNodeValue(dtShop[0], 'whno') + "店");

        $('#txtDocDate').val(SysDate)


        $('#btQuery').click(function () { SearchVIN14_1P(); });
        $('#btCancel_1').click(function () { btCancel_1_click(); });
        $('#btCancel_2').click(function () { btCancel_2_click(); });
        /*$('#btSave').click(function () { btSave_click(); });*/
        /*$('#btAddRack').click(function () { btAdd_click(); });*/
        $('.forminput input').change(function () { InputValidation(this) });
    };

    let SearchVIN14_1P = function () {
        ShowLoading();
        if ($('#cbArea').val() == "" || $('#cbArea').val() == null)
            $('#lblArea').html("")
        else
            $('#lblArea').html($('#cbArea').val())
        GetVIN14_1P()
    };

    let GetVIN14_1P = function () {
        var pData = {
            Area: $('#cbArea').val(),
            Pick: $('#cbPick').val(),
            DocDate: $('#txtDocDate').val(),
        };
        PostToWebApi({ url: "api/SystemSetup/GetVIN14_1P", data: pData, success: AfterGetVIN14_1P });
    };

    let AfterGetVIN14_1P = function (data) {
        CloseLoading();
        if (ReturnMsg(data, 0) != "GetVIN14_1POK") {
            DyAlert(ReturnMsg(data, 0));
            return;
        }
        else {
            var dtVIN14_1P = data.getElementsByTagName('dtVIN14_1P');
            grdU.BindData(dtVIN14_1P);
            if (VIN14_1P.length == 0) {
                //DyAlert("無符合資料!", BlankMode);
                return;
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
        PostToWebApi({ url: "api/SystemSetup/GetInitVIN14_1P", success: afterGetInitVIN14_1P });
        $('#pgVIN14_1P').show();
    };

    if ($('#pgVIN14_1P').length == 0) {
        //2021-04-29 Debug用，按F12後，在主控台內會顯示aaaaaaVMN29
        //console.log("aaaaaaVMN29");
        AllPages = new LoadAllPages(ParentNode, "VIN14_1P", ["pgVIN14_1P"], afterLoadPage);
    };


}