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
                ]
                //rows_per_page: 10,
                //method_clickrow: click_PLU,
                //afterBind: InitModifyDeleteButton,
                //sortable: "Y"
            }
        );
        SetDateField($('#txtOpenDateS')[0]);
        SetDateField($('#txtOpenDateE')[0]);
        $('#selST_ID').change(function () {
            let st_id = $('#selST_ID').val();
            InitSelectItem($('#selLayerNo')[0], dtMachineListSpecBlank, "LayerNo", "LayerNo", true, "請選擇貨艙代號");
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

        $('#pgVSA04P .fa-search').click(function () { SearchVSA04P(false); });
        $('#pgVSA04P #btExpoVSA04P').click(function () { SearchVSA04P(true); });
        return;
    };

    let AfterGetCkNoByST_ID = function (data) {
        let dtWarehouseDSV = data.getElementsByTagName('dtWarehouseDSV');
        InitSelectItem($('#selCkno')[0], dtWarehouseDSV, "SNno", "CkNo", true, "請選擇機號");
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
            grdU.BindData(dtSalesHD);
            if (dtSalesHD.length == 0) {
                DyAlert("無符合資料!", DummyFunction);
                return;
            }

        }
    };




    let afterGetInitVSA04P = function (data) {

        var dtWarehouse = data.getElementsByTagName('dtWarehouse');
        dtWarehouseDSVBlank = data.getElementsByTagName('dtWarehouseDSV');
        InitSelectItem($('#selST_ID')[0], dtWarehouse, "ST_ID", "ST_Sname", true, "請選擇店代號");
        InitSelectItem($('#selCkno')[0], dtWarehouseDSVBlank, "SNno", "CkNo", true, "請選擇機號");
        AssignVar();
        return;
        
        $('#btQueryPLU').click(function () { SearchPLU(); });
        $('#btUPPic1,#btUPPic2').click(function () { UploadPicture(this); });
        //$('#btDelete').click(function () { btDelete_click(); });
        $('#btImportFromiXMS').click(function () { btImportFromiXMS_click(); });
        $('#btSave').click(function () { btSave_click(); });
        $('#btCancel').click(function () { btCancel_click(); });
        //$('.forminput input').change(function () { InputValidation(this) });

        var dtDept = data.getElementsByTagName('dtDept');
        InitSelectItem($('#cbDept')[0], dtDept, "Type_ID", "Type_Name", true);

        var dtBGNo = data.getElementsByTagName('dtBGNo');
        InitSelectItem($('#cbBGNo')[0], dtBGNo, "Type_ID", "Type_Name", true);

        SetPLUAutoComplete("GD_NAME");
        SetPLUAutoComplete("GD_NO");
    };


  
  

  
  


    let InputValidation = function (ip) {
        var str = $(ip).val();
        var msg = "";
        //$('.forminput .msg-valid').text('');
        //$('.forminput .msg-valid').hide();
        $(ip).nextAll('.msg-valid').text(msg);
        $(ip).nextAll('.msg-valid').show();
        if (str == "")
            return;
        if ($(ip).attr('id') == "USR_CODE") {
            var re = /^[\d|a-zA-Z]+$/;
            if (!re.test(str) | str.length < 5 | str.length > 10)
                msg = "必須5~10碼英數字";
        }
        if ($(ip).attr('id') == "USR_PWD") {
            var re = /^[\d|a-zA-Z]+$/;
            if (!re.test(str) | str.length < 6 | str.length > 20)
                msg = "必須6~20碼英數字";
        }
        if ($(ip).attr('id') == "USR_NAME_L") {
            if (str.length > 10)
                msg = "必須10字元以內";
        }
        if ($(ip).attr('id') == "USR_EMPNO") {
            if (str.length > 10)
                msg = "必須10字元以內";
        }
        if ($(ip).attr('id') == "USR_MAIL") {
            var re = /^(([^<>()[\]\\.,;:\s@\"]+(\.[^<>()[\]\\.,;:\s@\"]+)*)|(\".+\"))@((\[[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\])|(([a-zA-Z\-0-9]+\.)+[a-zA-Z]{2,}))$/;
            if (!re.test(str))
                msg = "e-mail格式錯誤!";
        }
        if ($(ip).attr('id') == "USR_MOBILE") {
            var re = /^09\d{8}$/;
            if (!re.test(str))
                msg = "手機格式錯誤!";
        }
        if ($(ip).attr('id') == "USR_NOTE") {
            if (str.length > 50)
                msg = "必須50字元以內";
        }
        if (msg != "") {
            //$(ip).val('');
            $(ip).nextAll('.msg-valid').text(msg);
            $(ip).nextAll('.msg-valid').show();
        }
    }

    let afterLoadPage = function () {
        PostToWebApi({ url: "api/AIReports/GetInitVSA04P", success: afterGetInitVSA04P });
        $('#pgVSA04P').show();
        //$('#pgSysUsersEdit').hide();
    };

    if ($('#pgVSA04P').length == 0) {
        AllPages = new LoadAllPages(ParentNode, "AIReports/VSA04P", ["pgVSA04P"], afterLoadPage);
    };


}