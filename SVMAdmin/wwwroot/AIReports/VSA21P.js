var PageVSA21P = function (ParentNode) {
    let AllPages;
    let SysDate;
    let QueryDays;
    let grdU;
    let grdUD;
    let dtWarehouseDSV;
    let dtWarehouseDSVBlank;
    
    let AssignVar = function () {
        //let ColClass = [];
        grdU = new DynGrid(
            {
                table_lement: $('#tbVSA21P')[0],
                //class_collection: ColClass,
                class_collection: ["tdColbt icon_in_td", "tdColbt2 label-align", "tdCol3", "tdCol4 label-align", "tdCol5 label-align", "tdCol6 label-align"],
                fields_info: [
                    { type: "JQ", name: "fa-search", element: '<i class="fa fa-search"></i>' },
                    { type: "TextAmt", name: "Seq" },
                    { type: "Text", name: "PLUNAME" },
                    { type: "TextAmt", name: "GD_RETAIL" },
                    { type: "TextAmt", name: "Num" },
                    { type: "TextAmt", name: "Cash" }
                ],
                    rows_per_page: 10,
                //    method_clickrow: click_PLU,
                afterBind: InitModifyDeleteButton,
                    sortable: "Y"
            }
        );
        SetDateField($('#txtOpenDateS')[0]);
        SetDateField($('#txtOpenDateE')[0]);
        $('#selST_ID').change(function () {
            let st_id = $('#selST_ID').val();
            if (st_id == "") {
                InitSelectItem($('#selCkno')[0], dtWarehouseDSV, "SNno", "CkNo", true, "請選擇機號");
            }
            else {
                var pData = {
                    ST_ID: st_id
                };
                PostToWebApi({ url: "api/SystemSetup/GetCkNoByST_ID", data: pData, success: AfterGetCkNoByST_ID });
            }
        });
        AssignVarD();
        $('#btQuery').click(function () { SearchVSA21P(true); });
        //$('#btQty').click(function () { SearchVSA21P(false); });
        //$('#btAmt').click(function () { SearchVSA21P(true); });
        return;
    };

    let AssignVarD = function () {
        grdUD = new DynGrid(
            {
                //2021-04-27
                table_lement: $('#tbDVSA21P')[0],
                class_collection: ["tdColbt label-align", "tdColbt2", "tdCol3", "tdCol4", "tdCol5 label-align", "tdCol6 label-align"],
                fields_info: [
                    { type: "TextAmt", name: "Seq" },
                    { type: "Text", name: "Shopno" },
                    { type: "Text", name: "Ckno" },
                    { type: "Text", name: "WhNAME" },
                    { type: "TextAmt", name: "Num" },
                    { type: "TextAmt", name: "Cash" }
                ],
                    rows_per_page: 10,
                //    method_clickrow: click_PLU,
                //afterBind: SearchDVSA21P(true),
                    sortable: "Y"
            }
        );
        //$('#btDQty').click(function () { SearchDVSA21P(false); });
        //$('#btDAmt').click(function () { SearchDVSA21P(true); });
        return;
    };


    let InitModifyDeleteButton = function () {
        $('#tbVSA21P .fa-search').click(function () { btDisplay_click(this) });
    }


    let btDisplay_click = function (bt) {
        $(bt).closest('tr').click();
        var node = $(grdU.ActiveRowTR()).prop('Record');
        $('#modal_VSA21P .modal-title').text('商品智販機銷售排行');
        $('#lbDateS').html($('#lbDate').html());
        $('#lbPLU').html(GetNodeValue(node, 'PLUNAME'));
        $('#lbGDRetail').html(GetNodeValue(node, 'GD_RETAIL'));
        $('#lbNum').html(GetNodeValue(node, 'Num'));
        $('#lbCash').html(GetNodeValue(node, 'Cash'));
        let GDNo = $('#lbPLU').html().split(" ")[0];
        $('#lbGoodsno').html(GDNo);
        $('#btBack').click(function () { btBack_click(this) });
        
        //alert("btDisplay_click show");
        AfterbtDisPlayClick();
        $('#modal_VSA21P').modal('show');
    };
          


    let AfterbtDisPlayClick = function () {
        setTimeout(function () { SearchDVSA21P(true); }, 300);
    };

    let SearchDVSA21P = function (sort) {
        ShowLoading();
        let SortAmt = "N";
        if (sort)
            SortAmt = "Y";
        setTimeout(function () {
            var pData1 = {
                OpenDateS: $('#txtOpenDateS').val(),
                OpenDateE: $('#txtOpenDateE').val(),
                PLU: $('#lbGoodsno').html(),
                SortAmt: SortAmt
            };
            PostToWebApi({ url: "api/AIReports/SearchDVSA21P", data: pData1, success: AfterSearchDVSA21P });
        }, 100)
    };


    let AfterSearchDVSA21P = function (data) {
        //alert("AfterSearchDVSA21P");
        CloseLoading();
        if (ReturnMsg(data, 0) != "SearchDVSA21POK") {
            DyAlert(ReturnMsg(data, 0));
            return;
        }
        else {
            var dtSalesD = data.getElementsByTagName('dtSalesD');
            if (ReturnMsg(data, 1) == "") {
                grdUD.BindData(dtSalesD);
                //$('#btDQty').prop('disabled', false);
                //$('#btDAmt').prop('disabled', false);
            }
            //alert(dtSalesDSV.length);
            if (dtSalesD.length == 0) {
                //$('#btDQty').prop('disabled', true);
                //$('#btDAmt').prop('disabled', true);
                DyAlert("無符合資料!", DummyFunction);
                return;
            }

        }
        
    };


    let btBack_click = function () {
        $('#modal_VSA21P').modal('hide');
    };
    

    let AfterGetCkNoByST_ID = function (data) {
        let dtWarehouseDSV = data.getElementsByTagName('dtWarehouseDSV');
        InitSelectItem($('#selCkno')[0], dtWarehouseDSV, "CkNo", "CkNoName", true, "請選擇機號");
        $('#selCkno option[value=00]').remove();
    }
  

    let SearchVSA21P = function (sort) {
        if (($('#txtOpenDateS').val() == "" | $('#txtOpenDateS').val() == null)) {
            //CloseLoading();
            DyAlert("銷售日期查詢條件必須輸入資料!!");
            return;
        }
        if (($('#txtOpenDateE').val() == "" | $('#txtOpenDateE').val() == null)) {
            //CloseLoading();
            DyAlert("銷售日期查詢條件必須輸入資料!!");
            return;
        }
        if (DateDiff("d", $('#txtOpenDateS').val(), $('#txtOpenDateE').val()) > parseInt(QueryDays)) {
            DyAlert("銷售日期查詢區間必須小於等於" + QueryDays + "天!!");
            return;
        }
        
        ShowLoading();
        let SortAmt = "N";
        if (sort)
            SortAmt = "Y";
        setTimeout(function () {
            var pData = {
                OpenDateS: $('#txtOpenDateS').val(),
                OpenDateE: $('#txtOpenDateE').val(),
                ST_ID: $('#selST_ID').val(),
                Ckno: $('#selCkno option:selected').val(),
                KeyWord: $('#txtSalesSearch').val(),
                SortAmt: SortAmt
            };
            PostToWebApi({ url: "api/AIReports/SearchVSA21P", data: pData, success: AfterSearchVSA21P });
        }, 100)
    };


    let AfterSearchVSA21P = function (data) {
        $('#lbDate').html($('#txtOpenDateS').val() + " ~ " + $('#txtOpenDateE').val());
        CloseLoading();
        if (ReturnMsg(data, 0) != "SearchVSA21POK") {
            DyAlert(ReturnMsg(data, 0));
            //$('#btQty').prop('disabled', true);
            //$('#btAmt').prop('disabled', true);
            return;
        }
        else {
            var dtSalesDSV = data.getElementsByTagName('dtSalesDSV');
            if (ReturnMsg(data, 1) == "") {
                //alert("OK");
                grdU.BindData(dtSalesDSV);
                //$('#btQty').prop('disabled', false);
                //$('#btAmt').prop('disabled', false);
            }
            //alert(dtSalesDSV.length);
            if (dtSalesDSV.length == 0) {
                //$('#btQty').prop('disabled', true);
                //$('#btAmt').prop('disabled', true);
                DyAlert("無符合資料!", DummyFunction);
                $('#lblSumNum').html("")
                $('#lblSumCash').html("")
                return;
            }
            else {
                var Num = 0;
                var Cash = 0;
                for (var i = 0; i < dtSalesDSV.length; i++) {
                    Num += parseFloat(GetNodeValue(dtSalesDSV[i], 'Num'));
                    Cash += parseFloat(GetNodeValue(dtSalesDSV[i], 'Cash'));
                }
                $('#lblSumNum').html((Num).toLocaleString('en-US'))
                $('#lblSumCash').html((Cash).toLocaleString('en-US'))
            }

        }
    };


    let GetSysDate = function () {

        var cData = {
            DocNo: ""
        }
        PostToWebApi({ url: "api/SystemSetup/GetSysDate", data: cData, success: AfterGetSysDate });

        //return;

    };

    let AfterGetSysDate = function (data) {
        //alert("AfterGetSysDate");
        if (ReturnMsg(data, 0) != "GetSysDateOK") {
            DyAlert(ReturnMsg(data, 0));
            return;
        }
        else {
            var dtSysDate = data.getElementsByTagName('dtSysDate');
            if (dtSysDate.length == 0) {
                //DyAlert("無符合資料!", BlankMode);
                //return;
            }
            else {
                SysDate = GetNodeValue(dtSysDate[0], "SysDate");
            }
        }
    };


    let GetQueryDays = function () {

        var qData = {
            ProgramID: "VSA21P"
        }
        PostToWebApi({ url: "api/SystemSetup/GetQueryDays", data: qData, success: AfterGetQueryDays });

        //return;

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


    let afterGetInitVSA21P = function (data) {
        var dtWarehouse = data.getElementsByTagName('dtWarehouse');
        dtWarehouseDSVBlank = data.getElementsByTagName('dtWarehouseDSV');
        InitSelectItem($('#selST_ID')[0], dtWarehouse, "ST_ID", "ST_Sname", true, "請選擇店號");
        InitSelectItem($('#selCkno')[0], dtWarehouseDSVBlank, "CkNo", "CkNo", true, "請選擇機號");
        $('#txtOpenDateS').val(SysDate);
        $('#txtOpenDateE').val(SysDate);
        $('#lbDate').html("");
        AssignVar();
        //$('#btQty').prop('disabled', true);
        //$('#btAmt').prop('disabled', true);
    };


    let afterLoadPage = function () {
        PostToWebApi({ url: "api/AIReports/GetInitVSA21P", success: afterGetInitVSA21P });
        $('#pgVSA21P').show();
    };

    if ($('#pgVSA21P').length == 0) {
        //2021-04-29 Debug用，按F12後，在主控台內會顯示aaaaaaVMN29
        //console.log("aaaaaaVSA21P");
        GetSysDate();
        GetQueryDays();
        AllPages = new LoadAllPages(ParentNode, "AIReports/VSA21P", ["pgVSA21P"], afterLoadPage);
    };


}