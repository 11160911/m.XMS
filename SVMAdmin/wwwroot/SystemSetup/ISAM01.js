var PageISAM01 = function (ParentNode) {

    let tbDetail;
    //let tbView;
    let grdM;
    let grdV;
    //let EditMode = "";
    let ShopNo = "";
    let WhName = "";
    let QtyLeft = 0;
    let INV_YM = ""; let INV_Head = ""; let INV_No = "";

    let AssignVar = function () {


        grdM = new DynGrid(
            {
                table_lement: $('#tbVIV10')[0],
                class_collection: ["tdColbt icon_in_td", "tdColbt icon_in_td", "tdCol2", "tdCol3", "tdCol4", "tdCol5", "tdCol6", "tdCol7", "tdCol8 text-right", "tdCol9", "tdCol10"],
                fields_info: [
                    { type: "JQ", name: "fa-tags", element: '<i class="fa fa-tags"></i>' },
                    { type: "JQ", name: "fa-search", element: '<i class="fa fa-search"></i>' },
                    //{ type: "JQ", name: "fa-file-text-o", element: '<i class="fa fa-file-text-o"></i>' },
                    { type: "Text", name: "INV_YM" },
                    { type: "Text", name: "WhName" },
                    { type: "Text", name: "INV_Head" },

                    { type: "Text", name: "INV_SNo" },
                    { type: "Text", name: "INV_ENo" },
                    { type: "Text", name: "INV_No" },
                    { type: "TextAmt", name: "DiffQty" },
                    { type: "Text", name: "ModDate" },

                    { type: "Text", name: "ModUserName" }
                ],
                rows_per_page: 10,
                method_clickrow: click_Machine,
                afterBind: InitModifyDeleteButton,
                sortable: "Y"
            }
        );


        grdV = new DynGrid(
            {
                table_lement: $('#tbVIV10View')[0],
                class_collection: ["tdCol0", "tdCol1", "tdCol2", "tdCol3", "tdCol4", "tdCol5 label-align", "tdCol6 text-right", "tdCol7"],
                fields_info: [
                    { type: "Text", name: "WhName" },              
                    { type: "Text", name: "CkNo" },

                    { type: "Text", name: "INV_Head" },
                    { type: "Text", name: "INV_SNo" },
                    { type: "Text", name: "INV_ENo" },
                    { type: "TextAmt", name: "DiffQty" },

                    { type: "Text", name: "AppDate" },
                    { type: "Text", name: "AppUserName" }
                ],
                rows_per_page: 10,
                method_clickrow: click_Machine,
                afterBind: InitViewButton,
                sortable: "Y"
            }
        );


    };

    let click_Machine = function (tr) {

    };


    let afterGetInitVIV10 = function (data) {
        AssignVar();
        tbDetail = $('#pgVIV10Detail #tbInvDtl tbody');
        //tbView = $('#pgVIV10View #tbVIV10View tbody');

        var dtYM = data.getElementsByTagName('dtYM');
        InitSelectItem($('#cbYM')[0], dtYM, "Inv_YM", "Inv_YM", true, "*請選擇發票年月");

        var dtWh = data.getElementsByTagName('dtWh');
        InitSelectItem($('#cbWh')[0], dtWh, "ST_ID", "STName", true, "請選擇店代號");

        //var dtRack = data.getElementsByTagName('dtRack');
        //InitSelectItem($('.sel_Rack')[0], dtRack, "Type_ID", "Type_Name", true);

        //$('#btNewMachine').click(function () { btNewMachine_click(); });
        //$('#btNewRack').click(function () { btNewRack_click(); });
        $('#pgVIV10Detail .valid-blank').prev('input').focus(
            function () { $('.valid-blank').text(''); }
        );

        $('#btQuery').click(function () { SearchVIV10(); });
        //$('#btUPPic1,#btUPPic2').click(function () { UploadPicture(this); });
        //$('#btDelete').click(function () { btDelete_click(); });
        $('#btSave').click(function () { btSave_click(); });
        $('#btCancel').click(function () { btCancel_click(); });
        $('#btLeave').click(function () { btLeave_click(); });
        //$('.forminput input').change(function () { InputValidation(this) });
        
        //SetDateField($('#StartDay')[0]);
        //SetDateField($('#StopDay')[0]);
        //$('#StartDay,#StopDay').datepicker();

        //SetPLUAutoComplete("GD_NAME");
        //SetPLUAutoComplete("GD_NO");
    };

    let InitModifyDeleteButton = function () {
        $('#tbVIV10 .fa-tags').click(function () { btModify_click(this) });
        $('#tbVIV10 .fa-search').click(function () { btView_click(this) });
    }

    let InitViewButton = function () {
        //alert("InitViewButton");
        //$('#tbVIV10 .fa-tags').click(function () { btModify_click(this) });
        //$('#tbVIV10 .fa-search').click(function () { btView_click(this) });
    }

    let btView_click = function (bt) {

        $(bt).closest('tr').click();
        //EditMode = "Modify";
        var node = $(grdM.ActiveRowTR()).prop('Record');
        var pData = {
            ShopNo: GetNodeValue(node, 'ShopNo'),
            CkNo: GetNodeValue(node, 'CkNo'),
            YM: GetNodeValue(node, 'INV_YM')
        }
        PostToWebApi({ url: "api/SystemSetup/GetVIV10View", data: pData, success: AfterGetView });

    };


    let AfterGetView = function (data) {
        if (ReturnMsg(data, 0) != "GetVIV10ViewOK") {
            DyAlert(ReturnMsg(data, 0));
            return;
        }
        else {
            var dtDtl = data.getElementsByTagName('dtDtl');
            
            if (dtDtl.length == 0) {
                DyAlert("無符合資料!", BlankMode);
                return;
            }
            else {
                
 
                $('#pgVIV10View .x_title h4').text('發票分配明細');
                $('#pgVIV10').hide();
                $('#pgVIV10Detail').hide();
                $('#pgVIV10View').show();

                setTimeout(
                    function () { grdV.BindData(dtDtl); }
                    , 100
                );
                
            }

        }
    };


    let SearchVIV10 = function () {

        if (($('#cbYM').val() == "" | $('#cbYM').val() == null)) {
            //CloseLoading();
            DyAlert("請選擇使用年月條件!!");
            return;
        }

        var pData = {
            YM: $('#cbYM').val(),
            WhNo: $('#cbWh').val()
        };
        PostToWebApi({ url: "api/SystemSetup/SearchVIV10", data: pData, success: AfterSearchVIV10 });
    }


    let AfterSearchVIV10 = function (data) {
        if (ReturnMsg(data, 0) != "SearchVIV10OK") {
            DyAlert(ReturnMsg(data, 0));
            return;
        }
        else {
            var dtInv = data.getElementsByTagName('dtInv');
            grdM.BindData(dtInv);
            if (dtInv.length == 0) {
                DyAlert("無符合資料!", BlankMode);
                return;
            }

        }
    };


   
    let btModify_click = function (bt) {
        $(bt).closest('tr').click();
        //EditMode = "Modify";
        var node = $(grdM.ActiveRowTR()).prop('Record');
        
        ShopNo = GetNodeValue(node, 'ShopNo');
        QtyLeft = GetNodeValue(node, 'DiffQty');
        WhName = GetNodeValue(node, 'WhName');
        INV_YM = GetNodeValue(node, 'INV_YM');
        INV_Head = GetNodeValue(node, 'INV_Head');
        INV_No = GetNodeValue(node, 'INV_No');

        var pData = {
            //CompanyCode: GetNodeValue(node, 'CompanyCode'),
            ShopNo: GetNodeValue(node, 'ShopNo')
        }
        PostToWebApi({ url: "api/SystemSetup/GetVIV10Detail", data: pData, success: AfterGetDetail });

    };

    let AfterGetDetail = function (data) {
        if (ReturnMsg(data, 0) != "GetVIV10DetailOK") {
            DyAlert(ReturnMsg(data, 0));
        }
        else {
            //var MachineList = data.getElementsByTagName('MachineList')[0];
            var CkList = data.getElementsByTagName('CkList');
            $('#pgVIV10Detail .x_title h4').text('發票分配');
            $('#lblShop').text("智販店：" + WhName);
            $('#lblShopNo').text(ShopNo);
            $('#lblQty').text("可分配張數：" +QtyLeft);
            $('#lblAllQty').text(QtyLeft);
            tbDetail.empty();
            $('#SNno').prop('disabled', true);
            $('#pgVIV10Detail .forminput input:text').val('');

            for (var i = 0; i < CkList.length; i++) {
                var Ck = GetNodeValue(CkList[i], "CkNo");
                var trD = NewOneDetail(Ck);

                tbDetail.append(trD);
            }
            $('#pgVIV10').hide();
            $('#pgVIV10Detail').show();

        }
    }


    let BlankMode = function () {

    }

    let btSavex_click = function () {
        var isCheck = true;
        var nullchk = $('#pgVIV10Detail .valid-blank').prev('input').filter(function () { return this.value == '' });
        if (nullchk.length > 0) {
            $('.valid-blank').text('請填入資料');
            isCheck = false;
        }
        //var nullsel = $('#tbInvDtl .sel_Rack').filter(function () { return $(this).val() == "" });
        //if (nullsel.length > 0) {
        //    nullsel.addClass('ErrorControl');
        //    isCheck = false;
        //}
        var nullQty = $('#tbInvDtl .AssignQty').filter(function () { return $(this).val() == "" });
        if (nullQty.length > 0) {
            nullQty.addClass('ErrorControl');
            isCheck = false;
        }


        if (!isCheck) {
            waitingDialog.hide();
            DyAlert("有資料未填寫，請填妥以後再儲存");
            return;
        }

        var TotalQty = 0;

        var trs = $('#tbInvDtl tbody tr');
        for (var i = 0; i < trs.length; i++) {
            var tr = trs[i];
            //alert($('#SNno').val());
            waitingDialog.hide();
            if ($(tr).find('.AssignQty').val() % 50 != 0) {
                DyAlert($(tr).find('.tdSerNo').text() + '機分配數量不為50之倍數!!');
                return;
            }
            TotalQty = parseInt(TotalQty) + parseInt($(tr).find('.AssignQty').val());
        }

        if (TotalQty == 0) {
            DyAlert('未分配張數!!');
            return;
        }

        if (TotalQty > QtyLeft) {
            DyAlert('合計分配數量超過剩餘張數!!');
            return;
        }

        DyConfirm("儲存後系統將自動分配發票號碼並批核，無法刪除或修改已批核資料。確定要儲存這筆資料嗎?", SaveAssignQty, DummyFunction);

        //setTimeout(
        //    function () { SaveAssignQty(); }
        //    ,100
        //);
        //waitingDialog.show("儲存中.....");
        //setTimeout(
        //    function () { SaveAssignQty(); }
        //    ,100
        //);
    }

    let SaveAssignQty = function () {
        
        var ShopData = [];
        var AssignQty = [];

        var DataH = {
            ShopNo: $('#lblShopNo').text(),
            INV_YM: INV_YM,
            INV_Head: INV_Head,
            INV_No: INV_No
        }
        ShopData.push(DataH);

        var trs = $('#tbInvDtl tbody tr');
        for (var i = 0; i < trs.length; i++) {
            var tr = trs[i];
            //alert($('#SNno').val());
            //alert($(tr).find('.tdSerNo').text());
            var DataD = {
                CkNo: $(tr).find('.tdSerNo').text(),
                AssignQty: $(tr).find('.AssignQty').val()
            }
            AssignQty.push(DataD);
        }
        pData = {
            ShopData: ShopData,
            AssignQty: AssignQty
        }
        PostToWebApi({ url: "api/SystemSetup/SaveVIV10", data: pData, success: AfterSaveVIV10 });
    }


    let AfterSaveVIV10 = function (data) {
        waitingDialog.hide();
        if (ReturnMsg(data, 0) != "SaveVIV10OK") {
            DyAlert(ReturnMsg(data, 0));
        }
        else {
            DyAlert("儲存完成!");
            $('#pgVIV10').show();
            $('#pgVIV10Detail').hide();

            var dtInv = data.getElementsByTagName('dtInv')[0];

            //if (EditMode == "Modify") {
            grdM.RefreshRocord(grdM.ActiveRowTR(), dtInv);
            //}
            //else if (EditMode == "Add") {
            //    grdM.AddNew(dtMachine);
            //}
        }
    }

    //let btNewMachine_click = function () {
    //    $('#SNno').prop('disabled', false);
    //    EditMode = "Add";
    //    $('#pgVIV10Detail .x_title h4').text('新增智販機');
    //    tbDetail.empty();
    //    $('#pgVIV10Detail .forminput input:text').val('');
    //    for (var i = 0; i < 4; i++)
    //        tbDetail.append(NewOneDetail(i));
    //    $('#pgVIV10').hide();
    //    $('#pgVIV10Detail').show();
    //}

    //let btNewRack_click = function () {
    //    var rowCount = tbDetail.find('tr').length;
    //    tbDetail.append(NewOneDetail(rowCount));
    //}

    let NewOneDetail = function (ck) {
        var tr = $('<tr></tr>');

        var td1 = $('<td class="tdSerNo"></td>');
        var td2 = $('<td></td>');
        tr.append(td1);
        tr.append(td2);

        td1.text(ck);
        td2.append($('<input class="AssignQty" type="text" value="" />'));
        $(td2).find('.AssignQty').click(function () { $(this).removeClass('ErrorControl') });

        //var tr = $('<tr></tr>');
        //var td1 = $('<td calss="tdLayer"></td>');
        //var td2 = $('<td class="tdSerNo"></td>');
        //var td3 = $('<td></td>');
        //var td4 = $('<td></td>');
        //tr.append(td1);
        //tr.append(td2);
        //tr.append(td3);
        //tr.append(td4);
        //td1.append($('<span class="fa fa-trash-o"></span>'));
        //td2.text(String.fromCharCode(Rowindex + 65));
        //td3.append($($('.sel_Rack')[0]).clone());
        //td4.append($('<input class="ChanQty" type="text" value="" />'));
        //tr.find('.fa-trash-o').click(function () { deleteDetail(this) });

        //$(td3).find('.sel_Rack').click(function () { $(this).removeClass('ErrorControl') });
        //$(td4).find('.ChanQty').click(function () { $(this).removeClass('ErrorControl') });

        return tr;
    }

    //let deleteDetail = function (img) {
    //    $(img).closest('tr').remove();
    //    var trs = tbDetail.find('tr');
    //    for (var i = 0; i < trs.length; i++) {
    //        var trTmp = trs[i];
    //        $(trTmp).find('.tdSerNo').text(String.fromCharCode(i + 65));
    //    }
    //}

    let btCancel_click = function () {
        $('#pgVIV10').show();
        $('#pgVIV10Detail').hide();
    }

    let btLeave_click = function () {
        $('#pgVIV10').show();
        $('#pgVIV10View').hide();
    }


    let afterSearchBINWeb = function (data) {
        beforeBtnClick();
        if (ReturnMsg(data, 0) != "SearchBINWebOK") {
            DyAlert(ReturnMsg(data, 1));
        }
        else {
            var dtBINData = data.getElementsByTagName('dtBINData');
            if (dtBINData.length>0) {
                if (GetNodeValue(dtBINData[0], "BINman") != $('#lblManID1').val().split(' ')[0]) {
                    DyAlert("盤點人員" + GetNodeValue(dtBINData[0], "BINman") + "已建立分區" + $('#txtBinNo').val() +"之分區單!!", BlankMode);
                    return;
                }
            }

            //AssignVar();
            //alert(GetNodeValue(dtISAMShop[0], "STName"));
            $('#lblShop2').text($('#lblShop1').text());
            $('#lblBINNo2').text($('#txtBinNo').val());
            $('#lblDate2').text($('#txtISAMDate').val());
            $('#pgISAM01Init').hide();
            if ($('#ISAM01btns').attr('hidden') == undefined) {
                $('#ISAM01btns').show();
            }
            else {
                $('#ISAM01btns').removeAttr('hidden');
            }
            

            $('#btAdd').click(function () { btAdd_click(); });
            $('#btMod').click(function () { btMod_click(); });
            $('#btToFTP').click(function () { btToFTP_click(); });
            $('#btRtn').click(function () {
                beforeBtnClick();
                $('#ISAM01btns').hide();
                $('#pgISAM01Init').show();
            });
        }
    }


    let btSave_click = function () {
        if ($('#txtISAMDate').val() == "" | $('#txtISAMDate').val() == null) {
            DyAlert("請輸入盤點日期!!",function () { $('#txtISAMDate').focus() });
            return;
        }
        if ($('#txtBinNo').val() == "" | $('#txtBinNo').val() == null) {
            DyAlert("請輸入分區代碼!!", function () { $('#txtBinNo').focus() });
            return;
        }
        else {
            if ($('#txtBinNo').val().length>10) {
                DyAlert("分區代碼不可超過10個字元!!", function () { $('#txtBinNo').focus() });
                return;
            }
        }
        var pData = {
            Shop: $('#lblShop1').val().split(' ')[0],
            ISAMDate: $('#txtISAMDate').val(),
            BinNo: $('#txtBinNo').val()
        }
        PostToWebApi({ url: "api/SystemSetup/SearchBINWeb", data: pData, success: afterSearchBINWeb });
    };



    var beforeBtnClick = function () {
        TimerReset(sessionStorage.getItem('isamcomp'), "");
        TimerReset(sessionStorage.getItem('isamcomp'));
    };


    let afterGetInitISAM01 = function (data) {
        if (ReturnMsg(data, 0) != "GetInitISAM01OK") {
            DyAlert(ReturnMsg(data, 1));
        }
        else {
            var dtISAMShop = data.getElementsByTagName('dtWh');
            //AssignVar();
            //alert(GetNodeValue(dtISAMShop[0], "STName"));
            $('#lblShop1').text(GetNodeValue(dtISAMShop[0], "STName"));
            $('#lblManID1').text(GetNodeValue(dtISAMShop[0], "ManName"));
            SetDateField($('#txtISAMDate')[0]);
            $('#pgISAM01Init').removeAttr('hidden');
            //$('#pgISAM01Init').show();
            $('#btSave').click(function () { btSave_click(); });
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
                DyAlert("請確認店櫃(" + GetNodeValue(dtWh[0], "WhNo") + ")是否為允許作業之店櫃!", BlankMode);
                return;
            }
            PostToWebApi({ url: "api/SystemSetup/GetInitISAM01", success: afterGetInitISAM01 });
        }
    };

    let afterGetPageInitBefore = function (data) {
        //alert("99908");
        //alert(sessionStorage.getitem("isamcomp"));
        //TimerReset(sessionStorage.getitem("isamcomp"));
        if (ReturnMsg(data, 0) != "GetPageInitBeforeOK") {
            DyAlert(ReturnMsg(data, 1));
        }
        else {
            var dtISAMWh = data.getElementsByTagName('dtComp');
            /*alert(GetNodeValue(dtISAMWh[0], "WhNo") );*/
            if (dtISAMWh.length == 0) {
                DyAlert("無符合資料!", BlankMode);
                return;
            }
            else if (GetNodeValue(dtISAMWh[0], "WhNo") == null | GetNodeValue(dtISAMWh[0], "WhNo") == "") {
                DyAlert("請先至店號設定進行作業店櫃設定!", BlankMode);
                return;
            }
            else if (GetNodeValue(dtISAMWh[0], "WhNo") != "") {
                PostToWebApi({ url: "api/SystemSetup/GetWhName", success: AfterGetWhName });
            }
            
        }
    };


    let afterLoadPage = function () {
        //TimerReset(sessionStorage.getItem('isamcomp'));
        PostToWebApi({ url: "api/SystemSetup/GetPageInitBefore", success: afterGetPageInitBefore });
        //$('#ISAM01btns').hide();
        //$('#pgISAM01Init').hide();
        //$('#pgISAM01Add').hide();
        //$('#pgISAM01Mod').hide();
        //$('#pgISAM01UpFtp').hide();
        
        
    };


    if ($('#pgISAM01').length == 0) {
        AllPages = new LoadAllPages(ParentNode, "SystemSetup/ISAM01", ["ISAM01btns", "pgISAM01Init", "pgISAM01Add", "pgISAM01Mod", "pgISAM01UpFtp"], afterLoadPage);
    };


}