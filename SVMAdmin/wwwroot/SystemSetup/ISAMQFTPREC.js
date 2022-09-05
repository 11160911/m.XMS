var PageISAMQFTPREC = function (ParentNode) {
    
    let tbDetail;
    let grdM;
    let EditMode = "";
    let AssignVar = function () {
        grdM = new DynGrid(
            {
                table_lement: $('#tbISAMQFTPREC')[0],
                class_collection: ["tdColbt icon_in_td", "tdCol1", "tdCol2", "tdCol3", "tdCol4", "tdCol5", "tdCol6"],
                fields_info: [
                    { type: "JQ", name: "fa-search", element: '<i class="fa fa-search"></i>' },
                    { type: "Text", name: "DocTypeDesc", style:"width:20%"  },
                    { type: "Text", name: "CrtDT", style: "width:25%" },
                    { type: "Text", name: "CrtUserName", style: "width:20%"},
                    { type: "Text", name: "UpdateTypeDesc", style: "width:20%"},
                    { type: "Text", name: "CrtUser"},
                    { type: "Text", name: "DocType"}],
                //rows_per_page: 10,
                method_clickrow: click_Machine,
                afterBind: InitModifyDeleteButton
                //sortable: "Y"
            }
        );
        SetDateField($('#txtCrtDateS')[0]);
        SetDateField($('#txtCrtDateE')[0]);

    };

    let click_Machine = function (tr) {

    };

    let InitModifyDeleteButton = function () {
        $('#tbISAMQFTPREC .fa-search').click(function () { btDetail_click(this) });
    }

//#region 明細
let btCancel_click = function () {
        Timerset(sessionStorage.getItem('isamcomp'));
        $('#modal_ISAMFTPRecDetl').modal('hide');
    }

    let AfterSearchISAMFTPRecDetl = function (data) {
        if (ReturnMsg(data, 0) != "SearchISAMFTPRecDetlOK") {
            DyAlert(ReturnMsg(data, 1));
        }
        else {
            var dtD = data.getElementsByTagName('dtRec');
            $('#lblShop').html($('#lblShop2').html());
            $('#lblDocTP').html(GetNodeValue(dtD[0], 'DocTypeDesc'));
            $('#lblCrtDT').html(GetNodeValue(dtD[0], 'CrtDT'));
            $('#lblCrtUser').html(GetNodeValue(dtD[0], 'CrtUserName'));
            $('#lblUpType').html(GetNodeValue(dtD[0], 'UpdateTypeDesc'));
            $('#lblUpDT').html(GetNodeValue(dtD[0], 'FTPDate'));

            var UpComd = GetNodeValue(dtD[0], 'UpLoadComd');
            //alert(UpComd.split(';').length);
            //alert((UpComd.match(/\;/g) || []).length);
            if (UpComd.split(";").length > 2) {
                $('#tmpgrow1').show();
                $('#tmpgrow2').show();
                if (GetNodeValue(dtD[0], 'DocType') == "T") {
                    $('#tmplbl1').html('分區代碼');
                    $('#lblCode1').html(UpComd.split(";")[1]);
                    $('#tmplbl2').html('盤點日期');
                    $('#lblDocDate').html(UpComd.split(";")[2]);
                }
                else {
                    var dtWh = data.getElementsByTagName('dtWh');
                    $('#tmplbl1').html('進貨店櫃');
                    $('#lblCode1').html(GetNodeValue(dtWh[0], 'InSTName'));
                    $('#tmplbl2').html('單據日期');
                    $('#lblDocDate').html(UpComd.split(";")[2]);
                }
            }
            else {
                $('#tmpgrow1').hide();
                $('#tmpgrow2').hide();
            }
            $('#modal_ISAMFTPRecDetl').modal('show');
        }

        $('.msg-valid').hide();
    }
   
        
    let btDetail_click = function (bt) {
        Timerset(sessionStorage.getItem('isamcomp'));
        $(bt).closest('tr').click();
        $('.msg-valid').hide();
        $('#modal_ISAMFTPRecDetl .modal-title').text('上傳記錄明細查詢');
        //$('#modal_ISAM02Mod .btn-danger').text('刪除');
        var node = $(grdM.ActiveRowTR()).prop('Record');
        var cData = {
            CrtUser: GetNodeValue(node, 'CrtUser'),
            CrtDT: GetNodeValue(node, 'CrtDT'),
            DocType: GetNodeValue(node, 'DocType'),
            Shop: $('#lblShop2').html().split(' ')[0]
        }
        PostToWebApi({ url: "api/SystemSetup/SearchISAMFTPRecDetl", data: cData, success: AfterSearchISAMFTPRecDetl });
    }
//#endregion
    
//#region 查詢
    let afterSearchISAMQFTPREC = function (data) {
        if (ReturnMsg(data, 0) != "GetISAMQFTPRECDataOK") {
            DyAlert(ReturnMsg(data, 1));
        }
        else {
            var dtQRec = data.getElementsByTagName('dtQRec');
            grdM.BindData(dtQRec);
        }
    }

    let btQuery_click = function () {
        if (($('#txtCrtDateS').val() != "" && $('#txtCrtDateE').val() == "") || ($('#txtCrtDateS').val() == "" && $('#txtCrtDateE').val() != "")) {
            DyAlert("請輸入建立日期起迄!");
            return;
        }
        EditMode = "Q";
        //alert($('#selDocType').val())
        pData = {
            WhNo: $('#lblShop2').html().split(' ')[0],
            EditMode: EditMode,
            DateS: $('#txtCrtDateS').val(),
            DateE: $('#txtCrtDateE').val(),
            DocType: $('#selDocType').val()
        }
        PostToWebApi({ url: "api/SystemSetup/GetISAMQFTPRECData", data: pData, success: afterSearchISAMQFTPREC });
    }
    //#endregion

//#region FormLoad
    let afterGetInitISAMQFTPREC = function (data) {
        if (ReturnMsg(data, 0) != "GetISAMQFTPRECDataOK") {
            DyAlert(ReturnMsg(data, 1));
        }
        else {
            var dtDoc = data.getElementsByTagName('dtDoc');
            InitSelectItem($('#selDocType')[0], dtDoc, "DocType", "DocTypeDesc", true, "請選擇上傳類型");
            AssignVar();
            //
            tbDetail = $('#pgISAMQFTPREC #tbISAMQFTPREC tbody');
            var dtQDate = data.getElementsByTagName('dtQDate');
            $('#txtCrtDateS').val(GetNodeValue(dtQDate[0], "DTS"));
            $('#txtCrtDateE').val(GetNodeValue(dtQDate[0], "DTE"));

            //alert("dtQDate:"+dtQDate.length);
            var dtQRec = data.getElementsByTagName('dtQRec');
            grdM.BindData(dtQRec);
            $('#btQuery').click(function () { btQuery_click(); })

            $('#btCancel').click(function () { btCancel_click(); })
        }
    };


    let AfterGetWhName = function (data) {
        if (ReturnMsg(data, 0) != "GetWhNameOK") {
            DyAlert(ReturnMsg(data, 1));
        }
        else {
            
            var dtWh = data.getElementsByTagName('dtWh');
            //alert(GetNodeValue(dtWh[0], "ST_ID"));
            if (GetNodeValue(dtWh[0], "STName") == "") {
                DyAlert("請確認店櫃(" + GetNodeValue(dtWh[0], "WhNo") + ")是否為允許作業之店櫃!", DummyFunction);
                return;
            }
            $('#lblShop2').html(GetNodeValue(dtWh[0], "STName"));
            EditMode = "Init";
            pData = {
                WhNo: GetNodeValue(dtWh[0], "WhNo"),
                EditMode: EditMode
            }
            PostToWebApi({ url: "api/SystemSetup/GetISAMQFTPRECData", data: pData, success: afterGetInitISAMQFTPREC });
        }
    };

    let afterGetPageInitBefore = function (data) {
        if (ReturnMsg(data, 0) != "GetPageInitBeforeOK") {
            DyAlert(ReturnMsg(data, 1));
        }
        else {
            var dtISAMWh = data.getElementsByTagName('dtComp');
            //alert(GetNodeValue(dtISAMWh[0], "WhNo") );
            if (dtISAMWh.length == 0) {
                DyAlert("無符合資料!", DummyFunction);
                return;
            }
            else if (GetNodeValue(dtISAMWh[0], "WhNo") == null | GetNodeValue(dtISAMWh[0], "WhNo") == "") {
                DyAlert("請先至店號設定進行作業店櫃設定!", DummyFunction);
                return;
            }
            else if (GetNodeValue(dtISAMWh[0], "WhNo") != "") {
                PostToWebApi({ url: "api/SystemSetup/GetWhName", success: AfterGetWhName });
            }

        }
    };


    let afterLoadPage = function () {
        PostToWebApi({ url: "api/SystemSetup/GetPageInitBefore", success: afterGetPageInitBefore });
    };
//#endregion

    if ($('#pgISAMQFTPREC').length == 0) {
        AllPages = new LoadAllPages(ParentNode, "SystemSetup/ISAMQFTPREC", ["pgISAMQFTPREC"], afterLoadPage);
    };
}