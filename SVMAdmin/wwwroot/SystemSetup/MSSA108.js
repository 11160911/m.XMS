var PageMSSA108 = function (ParentNode) {

    let grdM;
    let grdTop20;
    let AssignVar = function () {

        grdM = new DynGrid(
            {
                table_lement: $('#tbQuery')[0],
                class_collection: ["tdCol1 text-center", "tdCol2 label-align", "tdCol3 label-align", "tdCol4 label-align", "tdCol5 label-align"],
                fields_info: [
                    { type: "Text", name: "ID", style: "" },
                    { type: "TextAmt", name: "RecCount"},
                    { type: "TextAmt", name: "Qty" },
                    { type: "TextAmt", name: "Cash" },
                    { type: "TextAmt", name: "Price" }
                ],
                //rows_per_page: 10,
                method_clickrow: click_PLU,
                sortable: "Y"
            }
        );
        grdTop20 = new DynGrid(
            {
                table_lement: $('#tbTop20')[0],
                class_collection: ["tdCol1 text-center", "tdCol2 text-center", "tdCol3 label-align", "tdCol4 label-align"],
                fields_info: [
                    { type: "Text", name: "ID", style: "" },
                    { type: "Text", name: "Name" },
                    { type: "TextAmt", name: "Qty" },
                    { type: "TextAmt", name: "Cash" }
                ],
                //rows_per_page: 10,
                method_clickrow: click_PLU,
                sortable: "Y"
            }
        );
        return;
    };

    let click_PLU = function (tr) {

    };

    let ChkLogOut_1 = function (AfterChkLogOut_1) {
        var LoginDT = sessionStorage.getItem('LoginDT');
        var cData = {
            LoginDT: LoginDT
        }
        PostToWebApi({ url: "api/js/ChkLogOut", data: cData, success: AfterChkLogOut_1 });
    };

//#region FormLoad
    let MSSA108Query = function () {
        ShowLoading();
        //$('#tbQuery thead tr th').css('background-color', '#ffb620')
        //$('#tbTop20 thead tr th').css('background-color', '#ffb620')
        //區域
        if ($('#rdoA').prop('checked') == true) {
            Flag = "A";
            $('#tbQuery').show();
            $('#tbTop20').hide();
        }
        //店別
        else if ($('#rdoS').prop('checked') == true) {
            Flag = "S";
            $('#tbQuery').show();
            $('#tbTop20').hide();
        }
        //前20名商品(金額)
        else if ($('#rdo20C').prop('checked') == true) {
            Flag = "20C";
            $('#tbQuery').hide();
            if ($('#tbTop20').attr('hidden') == undefined) {
                $('#tbTop20').show();
            }
            else {
                $('#tbTop20').removeAttr('hidden');
                $('#tbTop20').show();
            }
        }
        //前20名商品(數量)
        else if ($('#rdo20N').prop('checked') == true) {
            Flag = "20N";
            $('#tbQuery').hide();
            if ($('#tbTop20').attr('hidden') == undefined) {
                $('#tbTop20').show();
            }
            else {
                $('#tbTop20').removeAttr('hidden');
                $('#tbTop20').show();
            }
        }

        var pData = {
            ProgramID: "MSSA108",
            Flag: Flag
        }
        PostToWebApi({ url: "api/SystemSetup/MSSA108Query", data: pData, success: afterMSSA108Query });
    };

    let afterMSSA108Query = function (data) {
        CloseLoading();
        if (ReturnMsg(data, 0) != "MSSA108QueryOK") {
            DyAlert(ReturnMsg(data, 1));
        }
        else {
            var dtE = data.getElementsByTagName('dtE');
            var dtD = data.getElementsByTagName('dtD');
            if (dtE.length > 0) {
                $('#lblProgramName').html(GetNodeValue(dtE[0], "ChineseName"));
                $('#lblToday').html(GetNodeValue(dtE[0], "SysDate"));
            }
            if ($('#rdoA').prop('checked') || $('#rdoS').prop('checked')) {
                grdM.BindData(dtD);
                var heads = $('#tbQuery thead tr th#thead1');
                if ($('#rdoA').prop('checked')) {
                    $(heads).html('區域');
                }
                else if ($('#rdoS').prop('checked')) {
                    $(heads).html('店別');
                }
                if (dtD.length == 0) {
                    DyAlert("無符合資料!");
                    $(".modal-backdrop").remove();
                    $('#tbQuery thead td#td1').html('');
                    $('#tbQuery thead td#td2').html('');
                    $('#tbQuery thead td#td3').html('');
                    $('#tbQuery thead td#td4').html('');
                    return;
                }
                var dtH = data.getElementsByTagName('dtH');
                $('#tbQuery thead td#td1').html(parseInt(GetNodeValue(dtH[0], "SumRecCount")).toLocaleString('en-US'));
                $('#tbQuery thead td#td2').html(parseInt(GetNodeValue(dtH[0], "SumQty")).toLocaleString('en-US'));
                $('#tbQuery thead td#td3').html(parseInt(GetNodeValue(dtH[0], "SumCash")).toLocaleString('en-US'));
                $('#tbQuery thead td#td4').html(parseInt(GetNodeValue(dtH[0], "SumPrice")).toLocaleString('en-US'));
            }
            else {
                grdTop20.BindData(dtD);
                if (dtD.length == 0) {
                    DyAlert("無符合資料!");
                    $(".modal-backdrop").remove();
                    $('#tbTop20 thead td#tdTop20_1').html('');
                    $('#tbTop20 thead td#tdTop20_2').html('');
                    return;
                }
                var dtH = data.getElementsByTagName('dtH');
                $('#tbTop20 thead td#tdTop20_1').html(parseInt(GetNodeValue(dtH[0], "SumQty")).toLocaleString('en-US'));
                $('#tbTop20 thead td#tdTop20_2').html(parseInt(GetNodeValue(dtH[0], "SumCash")).toLocaleString('en-US'));
            }
        }
    };
    
    let afterLoadPage = function () {
        AssignVar();
        $('#rdoA,#rdoS,#rdo20C,#rdo20N').change(function () { MSSA108Query() });
        MSSA108Query();
    };
//#endregion
    

    if ($('#pgMSSA108').length == 0) {  
        AllPages = new LoadAllPages(ParentNode, "SystemSetup/MSSA108", ["MSSA108btns", "pgMSSA108Init", "pgMSSA108Add", "pgMSSA108Mod"], afterLoadPage);
    };


}