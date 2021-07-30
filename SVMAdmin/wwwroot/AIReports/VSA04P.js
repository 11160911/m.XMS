var PageVSA04P = function (ParentNode) {
    let AllPages;
    let grdU;
    let dtMachineListSpecBlank;
    let dtWarehouseDSV;

    let AssignVar = function () {
        let ColClass = [];
        for (let i = 0; i < 15; i++)
            ColClass.push("tdColAuto");
        grdU = new DynGrid(
            {
                table_lement: $('#tbVSA04P')[0],
                class_collection: ColClass,
                fields_info: [
                    { type: "TextAmt", name: "SerNo" },
                    { type: "Text", name: "ChrNo" },
                    { type: "Text", name: "OpenDT" },
                    { type: "Text", name: "ShopNo" },
                    { type: "Text", name: "ST_Sname" },
                    { type: "Text", name: "CKNo" },
                    { type: "Text", name: "Channel" },
                    { type: "Text", name: "InvNo" },
                    { type: "Text", name: "GoodsNo" },
                    { type: "Text", name: "GD_Sname" },
                    { type: "TextAmt", name: "GD_RETAIL" },
                    { type: "TextAmt", name: "Num" },
                    { type: "TextAmt", name: "Cash" },
                    { type: "Text", name: "Pay_Type" },
                    { type: "Text", name: "TranCompleted" }
                ],
                //rows_per_page: 10,
                //method_clickrow: click_PLU,
                //afterBind: InitModifyDeleteButton,
                sortable: "Y"
            }
        );
        SetDateField($('#txtOpenDateS')[0]);
        SetDateField($('#txtOpenDateE')[0]);
        $('#selST_ID').change(function () {
            let st_id = $('#selST_ID').val();
            InitSelectItem($('#selLayerNo')[0], dtMachineListSpecBlank, "LayerNo", "LayerNo", true, "請選擇貨艙代號");
            if (st_id == "") {
                InitSelectItem($('#selCkno')[0], dtWarehouseDSV, "CkNo", "CkNo", true, "請選擇機號");
            }
            else {
                var pData = {
                    ST_ID: st_id
                };
                PostToWebApi({ url: "api/SystemSetup/GetCkNoByST_ID", data: pData, success: AfterGetCkNoByST_ID });
            }
        });

        $('#btQuery').click(function () { SearchVSA04P(false); });
        $('#pgVSA04P #btExpoVSA04P').click(function () { SearchVSA04P(true); });
        return;
    };

    let AfterGetCkNoByST_ID = function (data) {
        let dtWarehouseDSV = data.getElementsByTagName('dtWarehouseDSV');
        InitSelectItem($('#selCkno')[0], dtWarehouseDSV, "CkNo", "CkNoName", true, "請選擇機號");
    }

    let SearchVSA04P = function (ExpXls) {
        ShowLoading();
        let ToExcel = "N";
        if (ExpXls)
            ToExcel = "Y";
        setTimeout(function () {
            var pData = {
                OpenDateS: $('#txtOpenDateS').val(),
                OpenDateE: $('#txtOpenDateE').val(),
                ST_ID: $('#selST_ID').val(),
                Ckno: $('#selCkno').val(),
                KeyWord: $('#txtSalesSearch').val(),
                ToExcel: ToExcel
            };
            PostToWebApi({ url: "api/AIReports/SearchVSA04P", data: pData, success: AfterSearchVSA04P });
        },100)
    };

    let AfterSearchVSA04P = function (data) {
        CloseLoading();
        if (ReturnMsg(data, 0) != "SearchVSA04POK") {
            DyAlert(ReturnMsg(data, 0));
            return;
        }
        else {
            var dtSalesHD = data.getElementsByTagName('dtSalesHD');
            if (ReturnMsg(data, 1) == "") {
                grdU.BindData(dtSalesHD);
            }
            else //Excel
            {
                var url = "api/FileDownload?ID=" + EncodeSGID(ReturnMsg(data, 1));
                url += "&CID=" + EncodeSGID(ReturnMsg(data, 2));
                url += "&UID=" + EncodeSGID(ReturnMsg(data, 3));
                $('#iframe_for_download').prop('src', url);
            }
            if (dtSalesHD.length == 0) {
                DyAlert("無符合資料!", DummyFunction);
                return;
            }

        }
    };




    let afterGetInitVSA04P = function (data) {
        $('#txtOpenDateS').val(getTodayDate());
        $('#txtOpenDateE').val(getTodayDate());

        var dtWarehouse = data.getElementsByTagName('dtWarehouse');
        dtWarehouseDSVBlank = data.getElementsByTagName('dtWarehouseDSV');
        InitSelectItem($('#selST_ID')[0], dtWarehouse, "ST_ID", "ST_Sname", true, "請選擇店代號");
        InitSelectItem($('#selCkno')[0], dtWarehouseDSVBlank, "CkNo", "CkNo", true, "請選擇機號");
        AssignVar();
        return;
    };


    let afterLoadPage = function () {
        PostToWebApi({ url: "api/AIReports/GetInitVSA04P", success: afterGetInitVSA04P });
        $('#pgVSA04P').show();
        //$('#pgSysUsersEdit').hide();
    };

    if ($('#pgVSA04P').length == 0) {
        AllPages = new LoadAllPages(ParentNode, "AIReports/VSA04P", ["pgVSA04P"], afterLoadPage);
    };

    function getTodayDate() {
        var fullDate = new Date();
        var yyyy = fullDate.getFullYear();
        var MM = (fullDate.getMonth() + 1) >= 10 ? (fullDate.getMonth() + 1) : ("0" + (fullDate.getMonth() + 1));
        var dd = fullDate.getDate() < 10 ? ("0" + fullDate.getDate()) : fullDate.getDate();
        var today = yyyy + "/" + MM + "/" + dd;
        return today;
    }
}