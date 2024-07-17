var PageMSSD102 = function (ParentNode) {

    let grdM;
    let grdShopNo_PSNO;
    let grdDate_PSNO;
    let grdShopNo_PSNO_ShopNoDate;
    let grdLookUp_ActivityCode;

    let EditMode = "";
    let DelPLU = "";
    let DelPLUQty;
    let ModPLU = "";
    let ModPLUQty;


    let AssignVar = function () {

        grdM = new DynGrid(
            {
                table_lement: $('#tbQuery')[0],
                class_collection: ["tdCol1", "tdCol2 text-center", "tdCol3", "tdCol4 text-center", "tdCol5 label-align", "tdCol6 label-align", "tdCol7 text-center", "tdCol8 label-align", "tdCol9 label-align", "tdCol10 label-align", "tdCol11 label-align"],
                fields_info: [
                    { type: "Text", name: "PS_NO", style: "" },
                    { type: "Text", name: "ActivityCode" },
                    { type: "Text", name: "PS_Name" },
                    { type: "Text", name: "EDDate"},
                    { type: "TextAmt", name: "Cnt1"},
                    { type: "TextAmt", name: "Cnt2"},
                    { type: "TextPercent", name: "RePercent"},
                    { type: "TextAmt", name: "ActualDiscount"},
                    { type: "TextAmt", name: "Cash"},
                    { type: "TextAmt", name: "Cnt3" },
                    { type: "TextAmt", name: "SalesPrice" }
                ],
                //rows_per_page: 10,
                method_clickrow: click_PLU,
                afterBind: InitModifyDeleteButton,
                sortable: "Y"
            }
        );

        grdShopNo_PSNO = new DynGrid(
            {
                table_lement: $('#tbShopNo_PSNO')[0],
                class_collection: ["tdCol1 text-center", "tdCol2 label-align", "tdCol3 label-align", "tdCol4 text-center", "tdCol5 label-align", "tdCol6 label-align", "tdCol7 label-align", "tdCol8 label-align", "tdCol9 label-align", "tdCol10 label-align", "tdCol11 label-align"],
                fields_info: [
                    { type: "Text", name: "ID", style: "" },
                    { type: "TextAmt", name: "Cnt1" },
                    { type: "TextAmt", name: "Cnt2" },
                    { type: "TextPercent", name: "RePercent" },
                    { type: "TextAmt", name: "ActualDiscount" },
                    { type: "TextAmt", name: "SalesCash1" },
                    { type: "TextAmt", name: "SalesCnt1" },
                    { type: "TextAmt", name: "SalesPrice1" },
                    { type: "TextAmt", name: "SalesCash2" },
                    { type: "TextAmt", name: "SalesCnt2" },
                    { type: "TextAmt", name: "SalesPrice2" }
                ],
                //rows_per_page: 10,
                method_clickrow: click_PLU,
                afterBind: gridclick1,
                sortable: "Y"
            }
        );

        grdDate_PSNO = new DynGrid(
            {
                table_lement: $('#tbDate_PSNO')[0],
                class_collection: ["tdCol1 text-center", "tdCol2 label-align", "tdCol3 label-align", "tdCol4 label-align", "tdCol5 label-align", "tdCol6 label-align", "tdCol7 label-align", "tdCol8 label-align", "tdCol9 label-align"],
                fields_info: [
                    { type: "Text", name: "ID", style: "" },
                    { type: "TextAmt", name: "Cnt1" },
                    { type: "TextAmt", name: "ActualDiscount" },
                    { type: "TextAmt", name: "SalesCash1" },
                    { type: "TextAmt", name: "SalesCnt1" },
                    { type: "TextAmt", name: "SalesPrice1" },
                    { type: "TextAmt", name: "SalesCash2" },
                    { type: "TextAmt", name: "SalesCnt2" },
                    { type: "TextAmt", name: "SalesPrice2" },
                ],
                //rows_per_page: 10,
                method_clickrow: click_PLU,
                afterBind: gridclick2,
                sortable: "Y"
            }
        );

        grdShopNo_PSNO_ShopNoDate = new DynGrid(
            {
                table_lement: $('#tbShopNo_PSNO_ShopNoDate')[0],
                class_collection: ["tdCol1 text-center", "tdCol2 label-align", "tdCol3 label-align", "tdCol4 label-align", "tdCol5 label-align", "tdCol6 label-align", "tdCol7 label-align", "tdCol8 label-align", "tdCol9 label-align"],
                fields_info: [
                    { type: "Text", name: "ID", style: "" },
                    { type: "TextAmt", name: "ReclaimQty" },
                    { type: "TextAmt", name: "ShareAmt" },
                    { type: "TextAmt", name: "ReclaimCash" },
                    { type: "TextAmt", name: "ReclaimTrans" },
                    { type: "TextAmt", name: "ReclaimPrice" },
                    { type: "TextAmt", name: "TotalCash" },
                    { type: "TextAmt", name: "TotalTrans" },
                    { type: "TextAmt", name: "TotalPrice" },
                ],
                //rows_per_page: 10,
                method_clickrow: click_PLU,
                afterBind: gridclick3,
                sortable: "Y"
            }
        );

        grdLookUp_ActivityCode = new DynGrid(
            {
                table_lement: $('#tbDataLookup_ActivityCode')[0],
                class_collection: ["tdCol1 text-center", "tdCol2", "tdCol3", "tdCol4 text-center", "tdCol5 text-center"],
                fields_info: [
                    { type: "radio", name: "chkset", style: "width:16px;height:16px" },
                    { type: "Text", name: "ActivityCode", style: "" },
                    { type: "Text", name: "PS_Name", style: "" },
                    { type: "Text", name: "StartDate", style: "" },
                    { type: "Text", name: "EndDate", style: "" }
                ],
                //rows_per_page: 10,
                method_clickrow: click_PLU,
                afterBind: InitModifyDeleteButton,
                sortable: "N"
            }
        );

        return;
    };

    let click_PLU = function (tr) {

    };

    let InitModifyDeleteButton = function () {
        $('#tbQuery tbody tr td').click(function () { Step1_click(this) });

        $('#tbQuery thead tr th').mouseenter(function () {
            var fdinfo = $(this).prop('fdinfo');
            if (fdinfo.name == "ActivityCode") {
                var rgb = $('#tbQuery thead tr th#thead2').css('background-color')
                var hexcolor = SetRgbTo16(rgb);
                if (hexcolor != '#ffeaa7') {
                    $('#tbQuery thead tr th#thead2').css('background-color', '#ffff00')
                }
            }
            else if (fdinfo.name == "PS_Name") {
                var rgb = $('#tbQuery thead tr th#thead3').css('background-color')
                var hexcolor = SetRgbTo16(rgb);
                if (hexcolor != '#ffeaa7') {
                    $('#tbQuery thead tr th#thead3').css('background-color', '#ffff00')
                }
            }
            else if (fdinfo.name == "EDDate") {
                var rgb = $('#tbQuery thead tr th#thead4').css('background-color')
                var hexcolor = SetRgbTo16(rgb);
                if (hexcolor != '#ffeaa7') {
                    $('#tbQuery thead tr th#thead4').css('background-color', '#ffff00')
                }
            }
            else if (fdinfo.name == "Cnt1") {
                var rgb = $('#tbQuery thead tr th#thead5').css('background-color')
                var hexcolor = SetRgbTo16(rgb);
                if (hexcolor != '#ffeaa7') {
                    $('#tbQuery thead tr th#thead5').css('background-color', '#ffff00')
                }
            }
            else if (fdinfo.name == "Cnt2") {
                var rgb = $('#tbQuery thead tr th#thead6').css('background-color')
                var hexcolor = SetRgbTo16(rgb);
                if (hexcolor != '#ffeaa7') {
                    $('#tbQuery thead tr th#thead6').css('background-color', '#ffff00')
                }
            }
            else if (fdinfo.name == "RePercent") {
                var rgb = $('#tbQuery thead tr th#thead7').css('background-color')
                var hexcolor = SetRgbTo16(rgb);
                if (hexcolor != '#ffeaa7') {
                    $('#tbQuery thead tr th#thead7').css('background-color', '#ffff00')
                }
            }
            else if (fdinfo.name == "ActualDiscount") {
                var rgb = $('#tbQuery thead tr th#thead8').css('background-color')
                var hexcolor = SetRgbTo16(rgb);
                if (hexcolor != '#ffeaa7') {
                    $('#tbQuery thead tr th#thead8').css('background-color', '#ffff00')
                }
            }
            else if (fdinfo.name == "Cash") {
                var rgb = $('#tbQuery thead tr th#thead9').css('background-color')
                var hexcolor = SetRgbTo16(rgb);
                if (hexcolor != '#ffeaa7') {
                    $('#tbQuery thead tr th#thead9').css('background-color', '#ffff00')
                }
            }
            else if (fdinfo.name == "Cnt3") {
                var rgb = $('#tbQuery thead tr th#thead10').css('background-color')
                var hexcolor = SetRgbTo16(rgb);
                if (hexcolor != '#ffeaa7') {
                    $('#tbQuery thead tr th#thead10').css('background-color', '#ffff00')
                }
            }
            else if (fdinfo.name == "SalesPrice") {
                var rgb = $('#tbQuery thead tr th#thead11').css('background-color')
                var hexcolor = SetRgbTo16(rgb);
                if (hexcolor != '#ffeaa7') {
                    $('#tbQuery thead tr th#thead11').css('background-color', '#ffff00')
                }
            }
        });
        $('#tbQuery thead tr th').mouseleave(function () {
            var fdinfo = $(this).prop('fdinfo');
            if (fdinfo.name == "ActivityCode") {
                var rgb = $('#tbQuery thead tr th#thead2').css('background-color')
                var hexcolor = SetRgbTo16(rgb);
                if (hexcolor != '#ffeaa7') {
                    $('#tbQuery thead tr th#thead2').css('background-color', '#ffb620')
                }
            }
            else if (fdinfo.name == "PS_Name") {
                var rgb = $('#tbQuery thead tr th#thead3').css('background-color')
                var hexcolor = SetRgbTo16(rgb);
                if (hexcolor != '#ffeaa7') {
                    $('#tbQuery thead tr th#thead3').css('background-color', '#ffb620')
                }
            }
            else if (fdinfo.name == "EDDate") {
                var rgb = $('#tbQuery thead tr th#thead4').css('background-color')
                var hexcolor = SetRgbTo16(rgb);
                if (hexcolor != '#ffeaa7') {
                    $('#tbQuery thead tr th#thead4').css('background-color', '#ffb620')
                }
            }
            else if (fdinfo.name == "Cnt1") {
                var rgb = $('#tbQuery thead tr th#thead5').css('background-color')
                var hexcolor = SetRgbTo16(rgb);
                if (hexcolor != '#ffeaa7') {
                    $('#tbQuery thead tr th#thead5').css('background-color', '#ffb620')
                }
            }
            else if (fdinfo.name == "Cnt2") {
                var rgb = $('#tbQuery thead tr th#thead6').css('background-color')
                var hexcolor = SetRgbTo16(rgb);
                if (hexcolor != '#ffeaa7') {
                    $('#tbQuery thead tr th#thead6').css('background-color', '#ffb620')
                }
            }
            else if (fdinfo.name == "RePercent") {
                var rgb = $('#tbQuery thead tr th#thead7').css('background-color')
                var hexcolor = SetRgbTo16(rgb);
                if (hexcolor != '#ffeaa7') {
                    $('#tbQuery thead tr th#thead7').css('background-color', '#ffb620')
                }
            }
            else if (fdinfo.name == "ActualDiscount") {
                var rgb = $('#tbQuery thead tr th#thead8').css('background-color')
                var hexcolor = SetRgbTo16(rgb);
                if (hexcolor != '#ffeaa7') {
                    $('#tbQuery thead tr th#thead8').css('background-color', '#ffb620')
                }
            }
            else if (fdinfo.name == "Cash") {
                var rgb = $('#tbQuery thead tr th#thead9').css('background-color')
                var hexcolor = SetRgbTo16(rgb);
                if (hexcolor != '#ffeaa7') {
                    $('#tbQuery thead tr th#thead9').css('background-color', '#ffb620')
                }
            }
            else if (fdinfo.name == "Cnt3") {
                var rgb = $('#tbQuery thead tr th#thead10').css('background-color')
                var hexcolor = SetRgbTo16(rgb);
                if (hexcolor != '#ffeaa7') {
                    $('#tbQuery thead tr th#thead10').css('background-color', '#ffb620')
                }
            }
            else if (fdinfo.name == "SalesPrice") {
                var rgb = $('#tbQuery thead tr th#thead11').css('background-color')
                var hexcolor = SetRgbTo16(rgb);
                if (hexcolor != '#ffeaa7') {
                    $('#tbQuery thead tr th#thead11').css('background-color', '#ffb620')
                }
            }
        });
        $('#tbQuery thead tr th').click(function () {
            $('#tbQuery thead tr th').css('background-color', '#ffb620')
            var fdinfo = $(this).prop('fdinfo');
            if (fdinfo.name == "ActivityCode") {
                $('#tbQuery thead tr th#thead2').css('background-color', '#ffeaa7')
            }
            else if (fdinfo.name == "PS_Name") {
                $('#tbQuery thead tr th#thead3').css('background-color', '#ffeaa7')
            }
            else if (fdinfo.name == "EDDate") {
                $('#tbQuery thead tr th#thead4').css('background-color', '#ffeaa7')
            }
            else if (fdinfo.name == "Cnt1") {
                $('#tbQuery thead tr th#thead5').css('background-color', '#ffeaa7')
            }
            else if (fdinfo.name == "Cnt2") {
                $('#tbQuery thead tr th#thead6').css('background-color', '#ffeaa7')
            }
            else if (fdinfo.name == "RePercent") {
                $('#tbQuery thead tr th#thead7').css('background-color', '#ffeaa7')
            }
            else if (fdinfo.name == "ActualDiscount") {
                $('#tbQuery thead tr th#thead8').css('background-color', '#ffeaa7')
            }
            else if (fdinfo.name == "Cash") {
                $('#tbQuery thead tr th#thead9').css('background-color', '#ffeaa7')
            }
            else if (fdinfo.name == "Cnt3") {
                $('#tbQuery thead tr th#thead10').css('background-color', '#ffeaa7')
            }
            else if (fdinfo.name == "SalesPrice") {
                $('#tbQuery thead tr th#thead11').css('background-color', '#ffeaa7')
            }
        });
    }

    let gridclick1 = function () {
        $('#tbShopNo_PSNO tbody tr td').click(function () { Step2_click(this) });

        $('#tbShopNo_PSNO thead tr th').mouseenter(function () {
            var fdinfo = $(this).prop('fdinfo');
            if (fdinfo.name == "ID") {
                var rgb = $('#tbShopNo_PSNO thead tr th#theadShopNo1').css('background-color')
                var hexcolor = SetRgbTo16(rgb);
                if (hexcolor != '#ffeaa7') {
                    $('#tbShopNo_PSNO thead tr th#theadShopNo1').css('background-color', '#ffff00')
                }
            }
            else if (fdinfo.name == "Cnt1") {
                var rgb = $('#tbShopNo_PSNO thead tr th#thShopNo1').css('background-color')
                var hexcolor = SetRgbTo16(rgb);
                if (hexcolor != '#ffeaa7') {
                    $('#tbShopNo_PSNO thead tr th#thShopNo1').css('background-color', '#ffff00')
                }
            }
            else if (fdinfo.name == "Cnt2") {
                var rgb = $('#tbShopNo_PSNO thead tr th#thShopNo2').css('background-color')
                var hexcolor = SetRgbTo16(rgb);
                if (hexcolor != '#ffeaa7') {
                    $('#tbShopNo_PSNO thead tr th#thShopNo2').css('background-color', '#ffff00')
                }
            }
            else if (fdinfo.name == "RePercent") {
                var rgb = $('#tbShopNo_PSNO thead tr th#thShopNo3').css('background-color')
                var hexcolor = SetRgbTo16(rgb);
                if (hexcolor != '#ffeaa7') {
                    $('#tbShopNo_PSNO thead tr th#thShopNo3').css('background-color', '#ffff00')
                }
            }
            else if (fdinfo.name == "ActualDiscount") {
                var rgb = $('#tbShopNo_PSNO thead tr th#thShopNo4').css('background-color')
                var hexcolor = SetRgbTo16(rgb);
                if (hexcolor != '#ffeaa7') {
                    $('#tbShopNo_PSNO thead tr th#thShopNo4').css('background-color', '#ffff00')
                }
            }
            else if (fdinfo.name == "SalesCash1") {
                var rgb = $('#tbShopNo_PSNO thead tr th#thShopNo5').css('background-color')
                var hexcolor = SetRgbTo16(rgb);
                if (hexcolor != '#ffeaa7') {
                    $('#tbShopNo_PSNO thead tr th#thShopNo5').css('background-color', '#ffff00')
                }
            }
            else if (fdinfo.name == "SalesCnt1") {
                var rgb = $('#tbShopNo_PSNO thead tr th#thShopNo6').css('background-color')
                var hexcolor = SetRgbTo16(rgb);
                if (hexcolor != '#ffeaa7') {
                    $('#tbShopNo_PSNO thead tr th#thShopNo6').css('background-color', '#ffff00')
                }
            }
            else if (fdinfo.name == "SalesPrice1") {
                var rgb = $('#tbShopNo_PSNO thead tr th#thShopNo7').css('background-color')
                var hexcolor = SetRgbTo16(rgb);
                if (hexcolor != '#ffeaa7') {
                    $('#tbShopNo_PSNO thead tr th#thShopNo7').css('background-color', '#ffff00')
                }
            }
            else if (fdinfo.name == "SalesCash2") {
                var rgb = $('#tbShopNo_PSNO thead tr th#thShopNo8').css('background-color')
                var hexcolor = SetRgbTo16(rgb);
                if (hexcolor != '#ffeaa7') {
                    $('#tbShopNo_PSNO thead tr th#thShopNo8').css('background-color', '#ffff00')
                }
            }
            else if (fdinfo.name == "SalesCnt2") {
                var rgb = $('#tbShopNo_PSNO thead tr th#thShopNo9').css('background-color')
                var hexcolor = SetRgbTo16(rgb);
                if (hexcolor != '#ffeaa7') {
                    $('#tbShopNo_PSNO thead tr th#thShopNo9').css('background-color', '#ffff00')
                }
            }
            else if (fdinfo.name == "SalesPrice2") {
                var rgb = $('#tbShopNo_PSNO thead tr th#thShopNo10').css('background-color')
                var hexcolor = SetRgbTo16(rgb);
                if (hexcolor != '#ffeaa7') {
                    $('#tbShopNo_PSNO thead tr th#thShopNo10').css('background-color', '#ffff00')
                }
            }
        });
        $('#tbShopNo_PSNO thead tr th').mouseleave(function () {
            var fdinfo = $(this).prop('fdinfo');
            if (fdinfo.name == "ID") {
                var rgb = $('#tbShopNo_PSNO thead tr th#theadShopNo1').css('background-color')
                var hexcolor = SetRgbTo16(rgb);
                if (hexcolor != '#ffeaa7') {
                    $('#tbShopNo_PSNO thead tr th#theadShopNo1').css('background-color', '#ffb620')
                }
            }
            else if (fdinfo.name == "Cnt1") {
                var rgb = $('#tbShopNo_PSNO thead tr th#thShopNo1').css('background-color')
                var hexcolor = SetRgbTo16(rgb);
                if (hexcolor != '#ffeaa7') {
                    $('#tbShopNo_PSNO thead tr th#thShopNo1').css('background-color', '#ffb620')
                }
            }
            else if (fdinfo.name == "Cnt2") {
                var rgb = $('#tbShopNo_PSNO thead tr th#thShopNo2').css('background-color')
                var hexcolor = SetRgbTo16(rgb);
                if (hexcolor != '#ffeaa7') {
                    $('#tbShopNo_PSNO thead tr th#thShopNo2').css('background-color', '#ffb620')
                }
            }
            else if (fdinfo.name == "RePercent") {
                var rgb = $('#tbShopNo_PSNO thead tr th#thShopNo3').css('background-color')
                var hexcolor = SetRgbTo16(rgb);
                if (hexcolor != '#ffeaa7') {
                    $('#tbShopNo_PSNO thead tr th#thShopNo3').css('background-color', '#ffb620')
                }
            }
            else if (fdinfo.name == "ActualDiscount") {
                var rgb = $('#tbShopNo_PSNO thead tr th#thShopNo4').css('background-color')
                var hexcolor = SetRgbTo16(rgb);
                if (hexcolor != '#ffeaa7') {
                    $('#tbShopNo_PSNO thead tr th#thShopNo4').css('background-color', '#ffb620')
                }
            }
            else if (fdinfo.name == "SalesCash1") {
                var rgb = $('#tbShopNo_PSNO thead tr th#thShopNo5').css('background-color')
                var hexcolor = SetRgbTo16(rgb);
                if (hexcolor != '#ffeaa7') {
                    $('#tbShopNo_PSNO thead tr th#thShopNo5').css('background-color', '#ffb620')
                }
            }
            else if (fdinfo.name == "SalesCnt1") {
                var rgb = $('#tbShopNo_PSNO thead tr th#thShopNo6').css('background-color')
                var hexcolor = SetRgbTo16(rgb);
                if (hexcolor != '#ffeaa7') {
                    $('#tbShopNo_PSNO thead tr th#thShopNo6').css('background-color', '#ffb620')
                }
            }
            else if (fdinfo.name == "SalesPrice1") {
                var rgb = $('#tbShopNo_PSNO thead tr th#thShopNo7').css('background-color')
                var hexcolor = SetRgbTo16(rgb);
                if (hexcolor != '#ffeaa7') {
                    $('#tbShopNo_PSNO thead tr th#thShopNo7').css('background-color', '#ffb620')
                }
            }
            else if (fdinfo.name == "SalesCash2") {
                var rgb = $('#tbShopNo_PSNO thead tr th#thShopNo8').css('background-color')
                var hexcolor = SetRgbTo16(rgb);
                if (hexcolor != '#ffeaa7') {
                    $('#tbShopNo_PSNO thead tr th#thShopNo8').css('background-color', '#ffb620')
                }
            }
            else if (fdinfo.name == "SalesCnt2") {
                var rgb = $('#tbShopNo_PSNO thead tr th#thShopNo9').css('background-color')
                var hexcolor = SetRgbTo16(rgb);
                if (hexcolor != '#ffeaa7') {
                    $('#tbShopNo_PSNO thead tr th#thShopNo9').css('background-color', '#ffb620')
                }
            }
            else if (fdinfo.name == "SalesPrice2") {
                var rgb = $('#tbShopNo_PSNO thead tr th#thead10').css('background-color')
                var hexcolor = SetRgbTo16(rgb);
                if (hexcolor != '#ffeaa7') {
                    $('#tbShopNo_PSNO thead tr th#thead10').css('background-color', '#ffb620')
                }
            }
        });
        $('#tbShopNo_PSNO thead tr th').click(function () {
            $('#tbShopNo_PSNO thead tr th').css('background-color', '#ffb620')
            var fdinfo = $(this).prop('fdinfo');
            if (fdinfo.name == "ID") {
                $('#tbShopNo_PSNO thead tr th#theadShopNo1').css('background-color', '#ffeaa7')
            }
            else if (fdinfo.name == "Cnt1") {
                $('#tbShopNo_PSNO thead tr th#thShopNo1').css('background-color', '#ffeaa7')
            }
            else if (fdinfo.name == "Cnt2") {
                $('#tbShopNo_PSNO thead tr th#thShopNo2').css('background-color', '#ffeaa7')
            }
            else if (fdinfo.name == "RePercent") {
                $('#tbShopNo_PSNO thead tr th#thShopNo3').css('background-color', '#ffeaa7')
            }
            else if (fdinfo.name == "ActualDiscount") {
                $('#tbShopNo_PSNO thead tr th#thShopNo4').css('background-color', '#ffeaa7')
            }
            else if (fdinfo.name == "SalesCash1") {
                $('#tbShopNo_PSNO thead tr th#thShopNo5').css('background-color', '#ffeaa7')
            }
            else if (fdinfo.name == "SalesCnt1") {
                $('#tbShopNo_PSNO thead tr th#thShopNo6').css('background-color', '#ffeaa7')
            }
            else if (fdinfo.name == "SalesPrice1") {
                $('#tbShopNo_PSNO thead tr th#thShopNo7').css('background-color', '#ffeaa7')
            }
            else if (fdinfo.name == "SalesCash2") {
                $('#tbShopNo_PSNO thead tr th#thShopNo8').css('background-color', '#ffeaa7')
            }
            else if (fdinfo.name == "SalesCnt2") {
                $('#tbShopNo_PSNO thead tr th#thShopNo9').css('background-color', '#ffeaa7')
            }
            else if (fdinfo.name == "SalesPrice2") {
                $('#tbShopNo_PSNO thead tr th#thShopNo10').css('background-color', '#ffeaa7')
            }
        });
    }

    let gridclick2 = function () {
        $('#tbDate_PSNO tbody tr td').click(function () { Step2_click(this) });

        $('#tbDate_PSNO thead tr th').mouseenter(function () {
            var fdinfo = $(this).prop('fdinfo');
            if (fdinfo.name == "ID") {
                var rgb = $('#tbDate_PSNO thead tr th#theadDate1').css('background-color')
                var hexcolor = SetRgbTo16(rgb);
                if (hexcolor != '#ffeaa7') {
                    $('#tbDate_PSNO thead tr th#theadDate1').css('background-color', '#ffff00')
                }
            }
            else if (fdinfo.name == "Cnt1") {
                var rgb = $('#tbDate_PSNO thead tr th#thDate1').css('background-color')
                var hexcolor = SetRgbTo16(rgb);
                if (hexcolor != '#ffeaa7') {
                    $('#tbDate_PSNO thead tr th#thDate1').css('background-color', '#ffff00')
                }
            }
            else if (fdinfo.name == "ActualDiscount") {
                var rgb = $('#tbDate_PSNO thead tr th#thDate2').css('background-color')
                var hexcolor = SetRgbTo16(rgb);
                if (hexcolor != '#ffeaa7') {
                    $('#tbDate_PSNO thead tr th#thDate2').css('background-color', '#ffff00')
                }
            }
            else if (fdinfo.name == "SalesCash1") {
                var rgb = $('#tbDate_PSNO thead tr th#thDate3').css('background-color')
                var hexcolor = SetRgbTo16(rgb);
                if (hexcolor != '#ffeaa7') {
                    $('#tbDate_PSNO thead tr th#thDate3').css('background-color', '#ffff00')
                }
            }
            else if (fdinfo.name == "SalesCnt1") {
                var rgb = $('#tbDate_PSNO thead tr th#thDate4').css('background-color')
                var hexcolor = SetRgbTo16(rgb);
                if (hexcolor != '#ffeaa7') {
                    $('#tbDate_PSNO thead tr th#thDate4').css('background-color', '#ffff00')
                }
            }
            else if (fdinfo.name == "SalesPrice1") {
                var rgb = $('#tbDate_PSNO thead tr th#thDate5').css('background-color')
                var hexcolor = SetRgbTo16(rgb);
                if (hexcolor != '#ffeaa7') {
                    $('#tbDate_PSNO thead tr th#thDate5').css('background-color', '#ffff00')
                }
            }
            else if (fdinfo.name == "SalesCash2") {
                var rgb = $('#tbDate_PSNO thead tr th#thDate6').css('background-color')
                var hexcolor = SetRgbTo16(rgb);
                if (hexcolor != '#ffeaa7') {
                    $('#tbDate_PSNO thead tr th#thDate6').css('background-color', '#ffff00')
                }
            }
            else if (fdinfo.name == "SalesCnt2") {
                var rgb = $('#tbDate_PSNO thead tr th#thDate7').css('background-color')
                var hexcolor = SetRgbTo16(rgb);
                if (hexcolor != '#ffeaa7') {
                    $('#tbDate_PSNO thead tr th#thDate7').css('background-color', '#ffff00')
                }
            }
            else if (fdinfo.name == "SalesPrice2") {
                var rgb = $('#tbDate_PSNO thead tr th#thDate8').css('background-color')
                var hexcolor = SetRgbTo16(rgb);
                if (hexcolor != '#ffeaa7') {
                    $('#tbDate_PSNO thead tr th#thDate8').css('background-color', '#ffff00')
                }
            }
        });
        $('#tbDate_PSNO thead tr th').mouseleave(function () {
            var fdinfo = $(this).prop('fdinfo');
            if (fdinfo.name == "ID") {
                var rgb = $('#tbDate_PSNO thead tr th#theadDate1').css('background-color')
                var hexcolor = SetRgbTo16(rgb);
                if (hexcolor != '#ffeaa7') {
                    $('#tbDate_PSNO thead tr th#theadDate1').css('background-color', '#ffb620')
                }
            }
            else if (fdinfo.name == "Cnt1") {
                var rgb = $('#tbDate_PSNO thead tr th#thDate1').css('background-color')
                var hexcolor = SetRgbTo16(rgb);
                if (hexcolor != '#ffeaa7') {
                    $('#tbDate_PSNO thead tr th#thDate1').css('background-color', '#ffb620')
                }
            }
            else if (fdinfo.name == "ActualDiscount") {
                var rgb = $('#tbDate_PSNO thead tr th#thDate2').css('background-color')
                var hexcolor = SetRgbTo16(rgb);
                if (hexcolor != '#ffeaa7') {
                    $('#tbDate_PSNO thead tr th#thDate2').css('background-color', '#ffb620')
                }
            }
            else if (fdinfo.name == "SalesCash1") {
                var rgb = $('#tbDate_PSNO thead tr th#thDate3').css('background-color')
                var hexcolor = SetRgbTo16(rgb);
                if (hexcolor != '#ffeaa7') {
                    $('#tbDate_PSNO thead tr th#thDate3').css('background-color', '#ffb620')
                }
            }
            else if (fdinfo.name == "SalesCnt1") {
                var rgb = $('#tbDate_PSNO thead tr th#thDate4').css('background-color')
                var hexcolor = SetRgbTo16(rgb);
                if (hexcolor != '#ffeaa7') {
                    $('#tbDate_PSNO thead tr th#thDate4').css('background-color', '#ffb620')
                }
            }
            else if (fdinfo.name == "SalesPrice1") {
                var rgb = $('#tbDate_PSNO thead tr th#thDate5').css('background-color')
                var hexcolor = SetRgbTo16(rgb);
                if (hexcolor != '#ffeaa7') {
                    $('#tbDate_PSNO thead tr th#thDate5').css('background-color', '#ffb620')
                }
            }
            else if (fdinfo.name == "SalesCash2") {
                var rgb = $('#tbDate_PSNO thead tr th#thDate6').css('background-color')
                var hexcolor = SetRgbTo16(rgb);
                if (hexcolor != '#ffeaa7') {
                    $('#tbDate_PSNO thead tr th#thDate6').css('background-color', '#ffb620')
                }
            }
            else if (fdinfo.name == "SalesCnt2") {
                var rgb = $('#tbDate_PSNO thead tr th#thDate7').css('background-color')
                var hexcolor = SetRgbTo16(rgb);
                if (hexcolor != '#ffeaa7') {
                    $('#tbDate_PSNO thead tr th#thDate7').css('background-color', '#ffb620')
                }
            }
            else if (fdinfo.name == "SalesPrice2") {
                var rgb = $('#tbDate_PSNO thead tr th#thDate8').css('background-color')
                var hexcolor = SetRgbTo16(rgb);
                if (hexcolor != '#ffeaa7') {
                    $('#tbDate_PSNO thead tr th#thDate8').css('background-color', '#ffb620')
                }
            }
        });
        $('#tbDate_PSNO thead tr th').click(function () {
            $('#tbDate_PSNO thead tr th').css('background-color', '#ffb620')
            var fdinfo = $(this).prop('fdinfo');
            if (fdinfo.name == "ID") {
                $('#tbDate_PSNO thead tr th#theadDate1').css('background-color', '#ffeaa7')
            }
            else if (fdinfo.name == "Cnt1") {
                $('#tbDate_PSNO thead tr th#thDate1').css('background-color', '#ffeaa7')
            }
            else if (fdinfo.name == "ActualDiscount") {
                $('#tbDate_PSNO thead tr th#thDate2').css('background-color', '#ffeaa7')
            }
            else if (fdinfo.name == "SalesCash1") {
                $('#tbDate_PSNO thead tr th#thDate3').css('background-color', '#ffeaa7')
            }
            else if (fdinfo.name == "SalesCnt1") {
                $('#tbDate_PSNO thead tr th#thDate4').css('background-color', '#ffeaa7')
            }
            else if (fdinfo.name == "SalesPrice1") {
                $('#tbDate_PSNO thead tr th#thDate5').css('background-color', '#ffeaa7')
            }
            else if (fdinfo.name == "SalesCash2") {
                $('#tbDate_PSNO thead tr th#thDate6').css('background-color', '#ffeaa7')
            }
            else if (fdinfo.name == "SalesCnt2") {
                $('#tbDate_PSNO thead tr th#thDate7').css('background-color', '#ffeaa7')
            }
            else if (fdinfo.name == "SalesPrice2") {
                $('#tbDate_PSNO thead tr th#thDate8').css('background-color', '#ffeaa7')
            }
        });
    }

    let gridclick3 = function () {
        $('#tbShopNo_PSNO_ShopNoDate thead tr th').mouseenter(function () {
            var fdinfo = $(this).prop('fdinfo');
            if (fdinfo.name == "ID") {
                var rgb = $('#tbShopNo_PSNO_ShopNoDate thead tr th#thname_PSNO_ShopNoDate').css('background-color')
                var hexcolor = SetRgbTo16(rgb);
                if (hexcolor != '#ffeaa7') {
                    $('#tbShopNo_PSNO_ShopNoDate thead tr th#thname_PSNO_ShopNoDate').css('background-color', '#ffff00')
                }
            }
            else if (fdinfo.name == "ReclaimQty") {
                var rgb = $('#tbShopNo_PSNO_ShopNoDate thead tr th#th1_PSNO_ShopNoDate').css('background-color')
                var hexcolor = SetRgbTo16(rgb);
                if (hexcolor != '#ffeaa7') {
                    $('#tbShopNo_PSNO_ShopNoDate thead tr th#th1_PSNO_ShopNoDate').css('background-color', '#ffff00')
                }
            }
            else if (fdinfo.name == "ShareAmt") {
                var rgb = $('#tbShopNo_PSNO_ShopNoDate thead tr th#th2_PSNO_ShopNoDate').css('background-color')
                var hexcolor = SetRgbTo16(rgb);
                if (hexcolor != '#ffeaa7') {
                    $('#tbShopNo_PSNO_ShopNoDate thead tr th#th2_PSNO_ShopNoDate').css('background-color', '#ffff00')
                }
            }
            else if (fdinfo.name == "ReclaimCash") {
                var rgb = $('#tbShopNo_PSNO_ShopNoDate thead tr th#th3_PSNO_ShopNoDate').css('background-color')
                var hexcolor = SetRgbTo16(rgb);
                if (hexcolor != '#ffeaa7') {
                    $('#tbShopNo_PSNO_ShopNoDate thead tr th#th3_PSNO_ShopNoDate').css('background-color', '#ffff00')
                }
            }
            else if (fdinfo.name == "ReclaimTrans") {
                var rgb = $('#tbShopNo_PSNO_ShopNoDate thead tr th#th4_PSNO_ShopNoDate').css('background-color')
                var hexcolor = SetRgbTo16(rgb);
                if (hexcolor != '#ffeaa7') {
                    $('#tbShopNo_PSNO_ShopNoDate thead tr th#th4_PSNO_ShopNoDate').css('background-color', '#ffff00')
                }
            }
            else if (fdinfo.name == "ReclaimPrice") {
                var rgb = $('#tbShopNo_PSNO_ShopNoDate thead tr th#th5_PSNO_ShopNoDate').css('background-color')
                var hexcolor = SetRgbTo16(rgb);
                if (hexcolor != '#ffeaa7') {
                    $('#tbShopNo_PSNO_ShopNoDate thead tr th#th5_PSNO_ShopNoDate').css('background-color', '#ffff00')
                }
            }
            else if (fdinfo.name == "TotalCash") {
                var rgb = $('#tbShopNo_PSNO_ShopNoDate thead tr th#th6_PSNO_ShopNoDate').css('background-color')
                var hexcolor = SetRgbTo16(rgb);
                if (hexcolor != '#ffeaa7') {
                    $('#tbShopNo_PSNO_ShopNoDate thead tr th#th6_PSNO_ShopNoDate').css('background-color', '#ffff00')
                }
            }
            else if (fdinfo.name == "TotalTrans") {
                var rgb = $('#tbShopNo_PSNO_ShopNoDate thead tr th#th7_PSNO_ShopNoDate').css('background-color')
                var hexcolor = SetRgbTo16(rgb);
                if (hexcolor != '#ffeaa7') {
                    $('#tbShopNo_PSNO_ShopNoDate thead tr th#th7_PSNO_ShopNoDate').css('background-color', '#ffff00')
                }
            }
            else if (fdinfo.name == "TotalPrice") {
                var rgb = $('#tbShopNo_PSNO_ShopNoDate thead tr th#th8_PSNO_ShopNoDate').css('background-color')
                var hexcolor = SetRgbTo16(rgb);
                if (hexcolor != '#ffeaa7') {
                    $('#tbShopNo_PSNO_ShopNoDate thead tr th#th8_PSNO_ShopNoDate').css('background-color', '#ffff00')
                }
            }
        });
        $('#tbShopNo_PSNO_ShopNoDate thead tr th').mouseleave(function () {
            var fdinfo = $(this).prop('fdinfo');
            if (fdinfo.name == "ID") {
                var rgb = $('#tbShopNo_PSNO_ShopNoDate thead tr th#thname_PSNO_ShopNoDate').css('background-color')
                var hexcolor = SetRgbTo16(rgb);
                if (hexcolor != '#ffeaa7') {
                    $('#tbShopNo_PSNO_ShopNoDate thead tr th#thname_PSNO_ShopNoDate').css('background-color', '#ffb620')
                }
            }
            else if (fdinfo.name == "ReclaimQty") {
                var rgb = $('#tbShopNo_PSNO_ShopNoDate thead tr th#th1_PSNO_ShopNoDate').css('background-color')
                var hexcolor = SetRgbTo16(rgb);
                if (hexcolor != '#ffeaa7') {
                    $('#tbShopNo_PSNO_ShopNoDate thead tr th#th1_PSNO_ShopNoDate').css('background-color', '#ffb620')
                }
            }
            else if (fdinfo.name == "ShareAmt") {
                var rgb = $('#tbShopNo_PSNO_ShopNoDate thead tr th#th2_PSNO_ShopNoDate').css('background-color')
                var hexcolor = SetRgbTo16(rgb);
                if (hexcolor != '#ffeaa7') {
                    $('#tbShopNo_PSNO_ShopNoDate thead tr th#th2_PSNO_ShopNoDate').css('background-color', '#ffb620')
                }
            }
            else if (fdinfo.name == "ReclaimCash") {
                var rgb = $('#tbShopNo_PSNO_ShopNoDate thead tr th#th3_PSNO_ShopNoDate').css('background-color')
                var hexcolor = SetRgbTo16(rgb);
                if (hexcolor != '#ffeaa7') {
                    $('#tbShopNo_PSNO_ShopNoDate thead tr th#th3_PSNO_ShopNoDate').css('background-color', '#ffb620')
                }
            }
            else if (fdinfo.name == "ReclaimTrans") {
                var rgb = $('#tbShopNo_PSNO_ShopNoDate thead tr th#th4_PSNO_ShopNoDate').css('background-color')
                var hexcolor = SetRgbTo16(rgb);
                if (hexcolor != '#ffeaa7') {
                    $('#tbShopNo_PSNO_ShopNoDate thead tr th#th4_PSNO_ShopNoDate').css('background-color', '#ffb620')
                }
            }
            else if (fdinfo.name == "ReclaimPrice") {
                var rgb = $('#tbShopNo_PSNO_ShopNoDate thead tr th#th5_PSNO_ShopNoDate').css('background-color')
                var hexcolor = SetRgbTo16(rgb);
                if (hexcolor != '#ffeaa7') {
                    $('#tbShopNo_PSNO_ShopNoDate thead tr th#th5_PSNO_ShopNoDate').css('background-color', '#ffb620')
                }
            }
            else if (fdinfo.name == "TotalCash") {
                var rgb = $('#tbShopNo_PSNO_ShopNoDate thead tr th#th6_PSNO_ShopNoDate').css('background-color')
                var hexcolor = SetRgbTo16(rgb);
                if (hexcolor != '#ffeaa7') {
                    $('#tbShopNo_PSNO_ShopNoDate thead tr th#th6_PSNO_ShopNoDate').css('background-color', '#ffb620')
                }
            }
            else if (fdinfo.name == "TotalTrans") {
                var rgb = $('#tbShopNo_PSNO_ShopNoDate thead tr th#th7_PSNO_ShopNoDate').css('background-color')
                var hexcolor = SetRgbTo16(rgb);
                if (hexcolor != '#ffeaa7') {
                    $('#tbShopNo_PSNO_ShopNoDate thead tr th#th7_PSNO_ShopNoDate').css('background-color', '#ffb620')
                }
            }
            else if (fdinfo.name == "TotalPrice") {
                var rgb = $('#tbShopNo_PSNO_ShopNoDate thead tr th#th8_PSNO_ShopNoDate').css('background-color')
                var hexcolor = SetRgbTo16(rgb);
                if (hexcolor != '#ffeaa7') {
                    $('#tbShopNo_PSNO_ShopNoDate thead tr th#th8_PSNO_ShopNoDate').css('background-color', '#ffb620')
                }
            }
        });
        $('#tbShopNo_PSNO_ShopNoDate thead tr th').click(function () {
            $('#tbShopNo_PSNO_ShopNoDate thead tr th').css('background-color', '#ffb620')
            var fdinfo = $(this).prop('fdinfo');
            if (fdinfo.name == "ID") {
                $('#tbShopNo_PSNO_ShopNoDate thead tr th#thname_PSNO_ShopNoDate').css('background-color', '#ffeaa7')
            }
            else if (fdinfo.name == "ReclaimQty") {
                $('#tbShopNo_PSNO_ShopNoDate thead tr th#th1_PSNO_ShopNoDate').css('background-color', '#ffeaa7')
            }
            else if (fdinfo.name == "ShareAmt") {
                $('#tbShopNo_PSNO_ShopNoDate thead tr th#th2_PSNO_ShopNoDate').css('background-color', '#ffeaa7')
            }
            else if (fdinfo.name == "ReclaimCash") {
                $('#tbShopNo_PSNO_ShopNoDate thead tr th#th3_PSNO_ShopNoDate').css('background-color', '#ffeaa7')
            }
            else if (fdinfo.name == "ReclaimTrans") {
                $('#tbShopNo_PSNO_ShopNoDate thead tr th#th4_PSNO_ShopNoDate').css('background-color', '#ffeaa7')
            }
            else if (fdinfo.name == "ReclaimPrice") {
                $('#tbShopNo_PSNO_ShopNoDate thead tr th#th5_PSNO_ShopNoDate').css('background-color', '#ffeaa7')
            }
            else if (fdinfo.name == "TotalCash") {
                $('#tbShopNo_PSNO_ShopNoDate thead tr th#th6_PSNO_ShopNoDate').css('background-color', '#ffeaa7')
            }
            else if (fdinfo.name == "TotalTrans") {
                $('#tbShopNo_PSNO_ShopNoDate thead tr th#th7_PSNO_ShopNoDate').css('background-color', '#ffeaa7')
            }
            else if (fdinfo.name == "TotalPrice") {
                $('#tbShopNo_PSNO_ShopNoDate thead tr th#th8_PSNO_ShopNoDate').css('background-color', '#ffeaa7')
            }
        });
    }

    let Step1_click = function (bt) {
        $('#tbQuery td').closest('tr').css('background-color', 'white');
        $(bt).closest('tr').click();
        $('.msg-valid').hide();
        var node = $(grdM.ActiveRowTR()).prop('Record');
        $('#tbQuery td:contains(' + GetNodeValue(node, 'PS_NO') + ')').closest('tr').css('background-color', '#DEEBF7');
        $('#lblPSNO_PSNO').html(GetNodeValue(node, 'PS_NO'))
        $('#lblActivityCode_PSNO').html(GetNodeValue(node, 'ActivityCode'))
        $('#lblEDDate_PSNO').html(GetNodeValue(node, 'EDDate'))
        $('#lblPSName_PSNO').html(GetNodeValue(node, 'PS_Name'))
        $('#rdoShop_PSNO').prop('checked', true);
        $('#modal_PSNO').modal('show');
        setTimeout(function () {
            QueryPSNO(GetNodeValue(node, 'PS_NO'));
        }, 500);
    };

    let QueryPSNO = function (PS_NO) {
        $('#tbShopNo_PSNO thead tr th').css('background-color', '#ffb620')
        $('#tbDate_PSNO thead tr th').css('background-color', '#ffb620')
        ShowLoading();
        var Flag = "";

        if ($('#rdoShop_PSNO').prop('checked') == true) {
            Flag = "S";
            $('#tbShopNo_PSNO').show();
            $('#tbDate_PSNO').hide();

        }
        else if ($('#rdoDate_PSNO').prop('checked') == true) {
            Flag = "D";
            $('#tbShopNo_PSNO').hide();
            if ($('#tbDate_PSNO').attr('hidden') == undefined) {
                $('#tbDate_PSNO').show();
            }
            else {
                $('#tbDate_PSNO').removeAttr('hidden');
                $('#tbDate_PSNO').show();
            }
        }

        var OpenDate1 = $('#lblEDDate_PSNO').html().split('~')[0]
        var OpenDate2 = $('#lblEDDate_PSNO').html().split('~')[1]

        var pData = {
            PS_NO: PS_NO,
            OpenDate1: OpenDate1,
            OpenDate2: OpenDate2,
            Flag: Flag
        }
        PostToWebApi({ url: "api/SystemSetup/MSSD102Query_Step1", data: pData, success: afterMSSD102Query_Step1 });
    };

    let afterMSSD102Query_Step1 = function (data) {
        CloseLoading();
        if (ReturnMsg(data, 0) != "MSSD102Query_Step1OK") {
            DyAlert(ReturnMsg(data, 1));
        }
        else {
            var dtE = data.getElementsByTagName('dtE');
            var dtSumQ = data.getElementsByTagName('dtSumQ');
            if ($('#rdoShop_PSNO').prop('checked') == true) {
                grdShopNo_PSNO.BindData(dtE);
                if (dtE.length == 0) {
                    DyAlert("無符合資料!");
                    //$(".modal-backdrop").remove();
                    $('#tbShopNo_PSNO thead td#tdShopNo1_PSNO').html('');
                    $('#tbShopNo_PSNO thead td#tdShopNo2_PSNO').html('');
                    $('#tbShopNo_PSNO thead td#tdShopNo3_PSNO').html('');
                    $('#tbShopNo_PSNO thead td#tdShopNo4_PSNO').html('');
                    $('#tbShopNo_PSNO thead td#tdShopNo5_PSNO').html('');
                    $('#tbShopNo_PSNO thead td#tdShopNo6_PSNO').html('');
                    $('#tbShopNo_PSNO thead td#tdShopNo7_PSNO').html('');
                    $('#tbShopNo_PSNO thead td#tdShopNo8_PSNO').html('');
                    $('#tbShopNo_PSNO thead td#tdShopNo9_PSNO').html('');
                    $('#tbShopNo_PSNO thead td#tdShopNo10_PSNO').html('');
                    return;
                }
                $('#tbShopNo_PSNO thead td#tdShopNo1_PSNO').html(parseInt(GetNodeValue(dtSumQ[0], "SumCnt1")).toLocaleString('en-US'));
                $('#tbShopNo_PSNO thead td#tdShopNo2_PSNO').html(parseInt(GetNodeValue(dtSumQ[0], "SumCnt2")).toLocaleString('en-US'));
                $('#tbShopNo_PSNO thead td#tdShopNo3_PSNO').html(GetNodeValue(dtSumQ[0], "SumRePercent"));
                $('#tbShopNo_PSNO thead td#tdShopNo4_PSNO').html(parseInt(GetNodeValue(dtSumQ[0], "SumActualDiscount")).toLocaleString('en-US'));
                $('#tbShopNo_PSNO thead td#tdShopNo5_PSNO').html(parseInt(GetNodeValue(dtSumQ[0], "SumSalesCash1")).toLocaleString('en-US'));
                $('#tbShopNo_PSNO thead td#tdShopNo6_PSNO').html(parseInt(GetNodeValue(dtSumQ[0], "SumSalesCnt1")).toLocaleString('en-US'));
                $('#tbShopNo_PSNO thead td#tdShopNo7_PSNO').html(parseInt(GetNodeValue(dtSumQ[0], "SumSalesPrice1")).toLocaleString('en-US'));
                $('#tbShopNo_PSNO thead td#tdShopNo8_PSNO').html(parseInt(GetNodeValue(dtSumQ[0], "SumSalesCash2")).toLocaleString('en-US'));
                $('#tbShopNo_PSNO thead td#tdShopNo9_PSNO').html(parseInt(GetNodeValue(dtSumQ[0], "SumSalesCnt2")).toLocaleString('en-US'));
                $('#tbShopNo_PSNO thead td#tdShopNo10_PSNO').html(parseInt(GetNodeValue(dtSumQ[0], "SumSalesPrice2")).toLocaleString('en-US'));
            }
            else if ($('#rdoDate_PSNO').prop('checked') == true) {
                grdDate_PSNO.BindData(dtE);
                if (dtE.length == 0) {
                    DyAlert("無符合資料!");
                    $(".modal-backdrop").remove();
                    $('#tbDate_PSNO thead td#tdDate1_PSNO').html('');
                    $('#tbDate_PSNO thead td#tdDate2_PSNO').html('');
                    $('#tbDate_PSNO thead td#tdDate3_PSNO').html('');
                    $('#tbDate_PSNO thead td#tdDate4_PSNO').html('');
                    $('#tbDate_PSNO thead td#tdDate5_PSNO').html('');
                    $('#tbDate_PSNO thead td#tdDate6_PSNO').html('');
                    $('#tbDate_PSNO thead td#tdDate7_PSNO').html('');
                    $('#tbDate_PSNO thead td#tdDate8_PSNO').html('');
                    return;
                }
                $('#tbDate_PSNO thead td#tdDate1_PSNO').html(parseInt(GetNodeValue(dtSumQ[0], "SumCnt1")).toLocaleString('en-US'));
                $('#tbDate_PSNO thead td#tdDate2_PSNO').html(parseInt(GetNodeValue(dtSumQ[0], "SumActualDiscount")).toLocaleString('en-US'));
                $('#tbDate_PSNO thead td#tdDate3_PSNO').html(parseInt(GetNodeValue(dtSumQ[0], "SumSalesCash1")).toLocaleString('en-US'));
                $('#tbDate_PSNO thead td#tdDate4_PSNO').html(parseInt(GetNodeValue(dtSumQ[0], "SumSalesCnt1")).toLocaleString('en-US'));
                $('#tbDate_PSNO thead td#tdDate5_PSNO').html(parseInt(GetNodeValue(dtSumQ[0], "SumSalesPrice1")).toLocaleString('en-US'));
                $('#tbDate_PSNO thead td#tdDate6_PSNO').html(parseInt(GetNodeValue(dtSumQ[0], "SumSalesCash2")).toLocaleString('en-US'));
                $('#tbDate_PSNO thead td#tdDate7_PSNO').html(parseInt(GetNodeValue(dtSumQ[0], "SumSalesCnt2")).toLocaleString('en-US'));
                $('#tbDate_PSNO thead td#tdDate8_PSNO').html(parseInt(GetNodeValue(dtSumQ[0], "SumSalesPrice2")).toLocaleString('en-US'));
            }
        }
    };

    let Step2_click = function (bt) {
        $('#lblPSNO_PSNO_ShopNoDate').html($('#lblPSNO_PSNO').html())
        $('#lblActivityCode_PSNO_ShopNoDate').html($('#lblActivityCode_PSNO').html())
        $('#lblEDDate_PSNO_ShopNoDate').html($('#lblEDDate_PSNO').html())
        $('#lblPSName_PSNO_ShopNoDate').html($('#lblPSName_PSNO').html())
        if ($('#rdoShop_PSNO').prop('checked') == true) {
            $('#tbShopNo_PSNO td').closest('tr').css('background-color', 'white');
            $(bt).closest('tr').click();
            $('.msg-valid').hide();
            var node = $(grdShopNo_PSNO.ActiveRowTR()).prop('Record');
            $('#tbShopNo_PSNO td:contains(' + GetNodeValue(node, 'ID') + ')').closest('tr').css('background-color', '#DEEBF7');
            $('#lblType_PSNO_ShopNoDate').html('店&ensp;&ensp;&ensp;&ensp;別')
            $('#lblTypeID_PSNO_ShopNoDate').html(GetNodeValue(node, 'ID'))
            $('#modal_PSNO_ShopNoDate').modal('show');
            setTimeout(function () {
                QueryPSNO_ShopNoDate($('#lblPSNO_PSNO_ShopNoDate').html(), GetNodeValue(node, 'ID').split('-')[0]);
            }, 500);
        }
        else if ($('#rdoDate_PSNO').prop('checked') == true) {
            $('#tbDate_PSNO td').closest('tr').css('background-color', 'white');
            $(bt).closest('tr').click();
            $('.msg-valid').hide();
            var node = $(grdDate_PSNO.ActiveRowTR()).prop('Record');
            $('#tbDate_PSNO td:contains(' + GetNodeValue(node, 'ID') + ')').closest('tr').css('background-color', '#DEEBF7');
            $('#lblType_PSNO_ShopNoDate').html('銷售日期')
            $('#lblTypeID_PSNO_ShopNoDate').html(GetNodeValue(node, 'ID'))
            $('#modal_PSNO_ShopNoDate').modal('show');
            setTimeout(function () {
                QueryPSNO_ShopNoDate($('#lblPSNO_PSNO_ShopNoDate').html(), GetNodeValue(node, 'ID'));
            }, 500);
        }
    };

    let QueryPSNO_ShopNoDate = function (PS_NO, ID) {
        $('#tbShopNo_PSNO_ShopNoDate thead tr th').css('background-color', '#ffb620')
        ShowLoading();
        var Flag = "";

        if ($('#rdoShop_PSNO').prop('checked') == true) {
            Flag = "S";
        }
        else if ($('#rdoDate_PSNO').prop('checked') == true) {
            Flag = "D";
        }

        var OpenDate1 = $('#lblEDDate_PSNO_ShopNoDate').html().split('~')[0]
        var OpenDate2 = $('#lblEDDate_PSNO_ShopNoDate').html().split('~')[1]

        var pData = {
            PS_NO: PS_NO,
            ID: ID,
            OpenDate1: OpenDate1,
            OpenDate2: OpenDate2,
            Flag: Flag
        }
        PostToWebApi({ url: "api/SystemSetup/MSSD102Query_Step2", data: pData, success: afterMSSD102Query_Step2 });
    };

    let afterMSSD102Query_Step2 = function (data) {
        CloseLoading();
        if (ReturnMsg(data, 0) != "MSSD102Query_Step2OK") {
            DyAlert(ReturnMsg(data, 1));
        }
        else {
            var dtE = data.getElementsByTagName('dtE');
            var dtSumQ = data.getElementsByTagName('dtSumQ');
            grdShopNo_PSNO_ShopNoDate.BindData(dtE);
            var heads = $('#tbShopNo_PSNO_ShopNoDate thead tr th#thname_PSNO_ShopNoDate');
            if ($('#rdoShop_PSNO').prop('checked') == true) {
                $(heads).html('銷售日期');
            }
            else if ($('#rdoDate_PSNO').prop('checked') == true) {
                $(heads).html('店別');
            }

            if (dtE.length == 0) {
                DyAlert("無符合資料!");
                //$(".modal-backdrop").remove();
                $('#tbShopNo_PSNO_ShopNoDate thead td#tdShopNo1_PSNO_ShopNoDate').html('');
                $('#tbShopNo_PSNO_ShopNoDate thead td#tdShopNo2_PSNO_ShopNoDate').html('');
                $('#tbShopNo_PSNO_ShopNoDate thead td#tdShopNo3_PSNO_ShopNoDate').html('');
                $('#tbShopNo_PSNO_ShopNoDate thead td#tdShopNo4_PSNO_ShopNoDate').html('');
                $('#tbShopNo_PSNO_ShopNoDate thead td#tdShopNo5_PSNO_ShopNoDate').html('');
                $('#tbShopNo_PSNO_ShopNoDate thead td#tdShopNo6_PSNO_ShopNoDate').html('');
                $('#tbShopNo_PSNO_ShopNoDate thead td#tdShopNo7_PSNO_ShopNoDate').html('');
                $('#tbShopNo_PSNO_ShopNoDate thead td#tdShopNo8_PSNO_ShopNoDate').html('');
                return;
            }
            $('#tbShopNo_PSNO_ShopNoDate thead td#tdShopNo1_PSNO_ShopNoDate').html(parseInt(GetNodeValue(dtSumQ[0], "SumReclaimQty")).toLocaleString('en-US'));
            $('#tbShopNo_PSNO_ShopNoDate thead td#tdShopNo2_PSNO_ShopNoDate').html(parseInt(GetNodeValue(dtSumQ[0], "SumShareAmt")).toLocaleString('en-US'));
            $('#tbShopNo_PSNO_ShopNoDate thead td#tdShopNo3_PSNO_ShopNoDate').html(parseInt(GetNodeValue(dtSumQ[0], "SumReclaimCash")).toLocaleString('en-US'));
            $('#tbShopNo_PSNO_ShopNoDate thead td#tdShopNo4_PSNO_ShopNoDate').html(parseInt(GetNodeValue(dtSumQ[0], "SumReclaimTrans")).toLocaleString('en-US'));
            $('#tbShopNo_PSNO_ShopNoDate thead td#tdShopNo5_PSNO_ShopNoDate').html(parseInt(GetNodeValue(dtSumQ[0], "SumReclaimPrice")).toLocaleString('en-US'));
            $('#tbShopNo_PSNO_ShopNoDate thead td#tdShopNo6_PSNO_ShopNoDate').html(parseInt(GetNodeValue(dtSumQ[0], "SumTotalCash")).toLocaleString('en-US'));
            $('#tbShopNo_PSNO_ShopNoDate thead td#tdShopNo7_PSNO_ShopNoDate').html(parseInt(GetNodeValue(dtSumQ[0], "SumTotalTrans")).toLocaleString('en-US'));
            $('#tbShopNo_PSNO_ShopNoDate thead td#tdShopNo8_PSNO_ShopNoDate').html(parseInt(GetNodeValue(dtSumQ[0], "SumTotalPrice")).toLocaleString('en-US'));
        }
    };

    let ChkLogOut_1 = function (AfterChkLogOut_1) {
        var LoginDT = sessionStorage.getItem('LoginDT');
        var cData = {
            LoginDT: LoginDT
        }
        PostToWebApi({ url: "api/js/ChkLogOut", data: cData, success: AfterChkLogOut_1 });
    };

//#region 編查
//#region 單筆修改
    let AfterSaveISAM01PLUMod = function (data) {
        if (ReturnMsg(data, 0) != "SaveISAM01PLUModOK") {
            DyAlert(ReturnMsg(data, 1));
        }
        else {
            DyAlert("修改完成!");

            $('#modal_ISAM01PLUMod').modal('hide');
            var dtBINMod = data.getElementsByTagName('dtBINMod')[0];
            grdM.RefreshRocord(grdM.ActiveRowTR(), dtBINMod);
        }

    };

    let btModSave_click = function () {
        ChkLogOut_1(btModSave_click_1);
    };

    let btModSave_click_1 = function (data) {
        if (ReturnMsg(data, 0) != "ChkLogOutOK") {
            DyAlert(ReturnMsg(data, 1));
        }
        else {
            var dtLogin = data.getElementsByTagName('dtLogin');
            if (GetNodeValue(dtLogin[0], "LogOutType") != "") {
                window.location.href = "Login" + sessionStorage.getItem('isamcomp');
            }
            else {
Timerset(sessionStorage.getItem('isamcomp'));
        if ($('#txtModQty1').val() == "") {
            DyAlert("請輸入數量!");
            return;
        }
        else {

            if (isNaN($('#txtModQty1').val())) {
                DyAlert("請輸入數字!");
                return;
            }
            if ($('#txtModQty1').val().indexOf(".") > 0) {
                DyAlert("請輸入整數!");
                return;
            }

            if ($('#txtModQty1').val() <= 0) {
                DyAlert("數量需大於0!");
                return;
            }

            if ($('#txtModQty1').val() > 999999) {
                DyAlert("數量不可大於999999!");
                return;
            }
        }
        var cData = {
            Shop: $('#lblShop2').html().split(' ')[0],
            ISAMDate: $('#lblDate2').html(),
            BinNo: $('#lblBINNo2').html(),
            PLU: ModPLU,
            Qty: $('#txtModQty1').val()
        }
        PostToWebApi({ url: "api/SystemSetup/SaveISAM01PLUMod", data: cData, success: AfterSaveISAM01PLUMod });
            }
        }
    }

    let btModCancel_click = function () {
        //*
        ChkLogOut(sessionStorage.getItem('isamcomp'));
        Timerset(sessionStorage.getItem('isamcomp'));
        $('#modal_ISAM01PLUMod').modal('hide');
    };

    let AfterGetModGDName = function (data) {
        if (ReturnMsg(data, 0) != "GetGDNameOK") {
            DyAlert(ReturnMsg(data, 1));
        }
        else {
            var dtP = data.getElementsByTagName('dtPLU');
            //alert(dtP.length);
            $('#lblModPLU').html(ModPLU);
            $('#lblModQty1').html(ModPLUQty);
            $('#txtModQty1').val(ModPLUQty);
            if (dtP.length > 0) {
                $('#lblModPLUName').html(GetNodeValue(dtP[0], 'GD_Name'));
            }
            else {
                //DyAlert("無符合之商品資料!");
                //return;
                $('#lblModPLUName').html('');
            }
            $('#modal_ISAM01PLUMod').modal('show');
        }

        $('.msg-valid').hide();
    };

    let btPLUMod_click = function (bt) {
        //*
        ChkLogOut(sessionStorage.getItem('isamcomp'));
        Timerset(sessionStorage.getItem('isamcomp'));
        $(bt).closest('tr').click();
        //alert(GetNodeValue(node, 'AppDate'));


        $('.msg-valid').hide();
        $('#modal_ISAM01PLUMod .modal-title').text('盤點資料單筆修改');
        //$('#modal_ISAM01Mod .btn-danger').text('刪除');
        var node = $(grdM.ActiveRowTR()).prop('Record');
        ModPLU = GetNodeValue(node, 'PLU');
        ModPLUQty = GetNodeValue(node, 'Qty1');
        var cData = {
            Shop: $('#lblShop2').html().split(' ')[0],
            ISAMDate: $('#lblDate2').html(),
            BinNo: $('#lblBINNo2').html(),
            PLU: ModPLU
        }
        PostToWebApi({ url: "api/SystemSetup/GetGDName", data: cData, success: AfterGetModGDName });

    };
//#endregion

//#region 單筆刪除
    let AfterDelISAM01PLU = function (data) {
        if (ReturnMsg(data, 0) != "DelISAM01PLUOK") {
            DyAlert(ReturnMsg(data, 1));
        }
        else {
            DyAlert("刪除完成!");

            $('#modal_ISAM01PLUDel').modal('hide');
            //var userxml = data.getElementsByTagName('dtRack')[0];
            grdM.DeleteRow(grdM.ActiveRowTR());
        }

    };

    let btDelSave_click = function () {
        ChkLogOut_1(btDelSave_click_1);
    };

    let btDelSave_click_1 = function (data) {
        if (ReturnMsg(data, 0) != "ChkLogOutOK") {
            DyAlert(ReturnMsg(data, 1));
        }
        else {
            var dtLogin = data.getElementsByTagName('dtLogin');
            if (GetNodeValue(dtLogin[0], "LogOutType") != "") {
                window.location.href = "Login" + sessionStorage.getItem('isamcomp');
            }
            else {
                Timerset(sessionStorage.getItem('isamcomp'));
                var cData = {
                    Shop: $('#lblShop2').html().split(' ')[0],
                    ISAMDate: $('#lblDate2').html(),
                    BinNo: $('#lblBINNo2').html(),
                    PLU: DelPLU
                }
                PostToWebApi({ url: "api/SystemSetup/DelISAM01PLU", data: cData, success: AfterDelISAM01PLU });
            }
        }
    }

    let btDelCancel_click = function () {
        //*
        ChkLogOut(sessionStorage.getItem('isamcomp'))
        Timerset(sessionStorage.getItem('isamcomp'));
        $('#modal_ISAM01PLUDel').modal('hide');
    };

    let AfterGetGDName = function (data) {
        if (ReturnMsg(data, 0) != "GetGDNameOK") {
            DyAlert(ReturnMsg(data, 1));
        }
        else {
            var dtP = data.getElementsByTagName('dtPLU');
            //alert(dtP.length);
            $('#lblPLU').html(DelPLU);
            $('#lblDelQty1').html(DelPLUQty);
            if (dtP.length > 0) {
                /*DyConfirm("確定要刪除商品" + GetNodeValue(dtP[0], 'PLU') + GetNodeValue(dtP[0], 'GD_Name') + "？", afterDelPLU(GetNodeValue(dtP[0], 'PLU')), DummyFunction);*/
                $('#lblPLUName').html(GetNodeValue(dtP[0], 'GD_Name'));
            }
            else {
                $('#lblPLUName').html('');
             }
            $('#modal_ISAM01PLUDel').modal('show');
        }

        $('.msg-valid').hide();
    };

    let btPLUDelete_click = function (bt) {
        //*
        ChkLogOut(sessionStorage.getItem('isamcomp'));
        Timerset(sessionStorage.getItem('isamcomp'));
        $(bt).closest('tr').click();
        $('.msg-valid').hide();
        $('#modal_ISAM01PLUDel .modal-title').text('盤點資料單筆刪除');
        //$('#modal_ISAM01Mod .btn-danger').text('刪除');
        var node = $(grdM.ActiveRowTR()).prop('Record');
        DelPLU = GetNodeValue(node, 'PLU');
        DelPLUQty = GetNodeValue(node, 'Qty1');
        var cData = {
            Shop: $('#lblShop2').html().split(' ')[0],
            ISAMDate: $('#lblDate2').html(),
            BinNo: $('#lblBINNo2').html(),
            PLU: DelPLU
        }
        PostToWebApi({ url: "api/SystemSetup/GetGDName", data: cData, success: AfterGetGDName });

    };
//#endregion

    let txtBarcode3_ini = function () {
        $('#txtBarcode3').val('');
        $('#txtBarcode3').focus();
    }

    let afterGetBINWebMod = function (data) {
        if (ReturnMsg(data, 0) != "GetBINWebModOK") {
            DyAlert(ReturnMsg(data, 1), txtBarcode3_ini);
            
        }
        else {
            var dtBin = data.getElementsByTagName('dtBin');
            grdM.BindData(dtBin);
            if (dtBin.length == 0) {
                //alert("No RowData");
                DyAlert("無符合資料!", txtBarcode3_ini);
                $(".modal-backdrop").remove();
                return;
            }
            txtBarcode3_ini()
        }

    };

//#endregion

//#region 新增
    let btQtySave1_click = function () {
        ChkLogOut_1(btQtySave1_click_1)
    };

    let btQtySave1_click_1 = function (data) {
        if (ReturnMsg(data, 0) != "ChkLogOutOK") {
            DyAlert(ReturnMsg(data, 1));
        }
        else {
            var dtLogin = data.getElementsByTagName('dtLogin');
            if (GetNodeValue(dtLogin[0], "LogOutType") != "") {
                window.location.href = "Login" + sessionStorage.getItem('isamcomp');
            }
            else {
                Timerset(sessionStorage.getItem('isamcomp'));
                if ($('#txtBarcode1').val() == "" && $('#lblBarcode').html() == "") {
                    DyAlert("請輸入條碼!");
                    $('#txtQty1').val('');
                    //$('#btKeyin1').prop('disabled', false);
                    //$('#btBCSave1').prop('disabled', false);
                    //$('#txtBarcode1').prop('disabled', false);
                    //$('#btQtySave1').prop('disabled', true);
                    //$('#txtQty1').prop('disabled', true);
                    return;
                }
                if ($('#txtQty1').val() != "") {
                    if (isNaN($('#txtQty1').val())) {
                        DyAlert("請輸入數字!");
                        return;
                    }
                    if ($('#txtQty1').val().indexOf(".") > 0) {
                        DyAlert("請輸入整數!");
                        return;
                    }

                    if ($('#txtQty1').val() <= 0) {
                        DyAlert("數量需大於0!");
                        return;
                    }
                }
                else {
                    $('#txtQty1').val() == "1"
                }
               
                //$('#btKeyin1').prop('disabled', false);
                //$('#btBCSave1').prop('disabled', false);
                //$('#txtBarcode1').prop('disabled', false);
                //$('#btQtySave1').prop('disabled', true);
                //$('#txtQty1').prop('disabled', true);
                var pData = {
                    Shop: $('#lblShop2').html().split(' ')[0],
                    ISAMDate: $('#lblDate2').html(),
                    BinNo: $('#lblBINNo2').html(),
                    Barcode: $('#txtBarcode1').val() == "" ? $('#lblBarcode').html() : $('#txtBarcode1').val(),
                    Qty: $('#txtQty1').val()
                };
                PostToWebApi({ url: "api/SystemSetup/SaveBINWeb", data: pData, success: afterSaveBINWeb });
            }
        }
    }

    let btKeyin1_click = function () {
        //*
        ChkLogOut(sessionStorage.getItem('isamcomp'))
        Timerset(sessionStorage.getItem('isamcomp'));
        //$('#btKeyin1').prop('disabled', true);
        //$('#btQtySave1').prop('disabled', false);
        //$('#txtQty1').prop('disabled', false);
        //$('#btBCSave1').prop('disabled', true);
        //$('#txtBarcode1').prop('disabled', true);
    };

    let afterSaveBINWeb = function (data) {
        if (ReturnMsg(data, 0) != "SaveBINWebOK") {
            DyAlert(ReturnMsg(data, 1),txtBarcode1_ini);
        }
        else {
            var dtSQ = data.getElementsByTagName('dtSQ');
            if (dtSQ.length > 0) {
                $('#lblSQty1').html(GetNodeValue(dtSQ[0], "SQ1"));
            }
            else {
                $('#lblSQty1').html('');
            }
            var dtSBQ = data.getElementsByTagName('dtSBQ');
            if (dtSBQ.length > 0) {
                $('#lblSBQty1').html(GetNodeValue(dtSBQ[0], "SBQ1"));
            }
            else {
                $('#lblSBQty1').html('');
            }
            var dtSWQ = data.getElementsByTagName('dtSWQ');
            if (dtSWQ.length > 0) {
                $('#lblSWQty1').html(GetNodeValue(dtSWQ[0], "SWQ1"));
            }
            else {
                $('#lblSWQty1').html('');
            }

            var dtP = data.getElementsByTagName('dtPLU');
            if (dtP.length > 0) {
                
                $('#lblBarcode').html(GetNodeValue(dtP[0], "PLU"));
                $('#txtBarcode1').val('');
                $('#lblQty1').html($('#txtQty1').val());
                $('#lblPrice').html(parseInt(GetNodeValue(dtP[0], "GD_Retail")));
                $('#lblGDName').html(GetNodeValue(dtP[0], "GD_Name"));
                $('#txtQty1').val("");
            }
            else {
                
                if ($('#txtBarcode1').val() == "") {
                }
                else {
                    $('#lblBarcode').html($('#txtBarcode1').val());
                }
                $('#txtBarcode1').val('');
                $('#lblQty1').html($('#txtQty1').val());
                $('#lblPrice').html('');
                $('#lblGDName').html('');
                $('#txtQty1').val("");
            }
            //alert(dtBin.length);
            //if (dtBin.length == 0) {
            //    alert("No RowData");
            //    DyAlert("無符合資料!", BlankMode);
            //    return;
            //}
        }
    };

    let txtBarcode1_ini = function () {
        $('#txtBarcode1').val('');
        $('#txtBarcode1').focus();
    }

    let txtQty1_ini = function () {
        $('#txtQty1').val('');
        $('#txtQty1').focus();
    }

    let btBCSave1_click = function () {
        ChkLogOut_1(btBCSave1_click_1)
    };

    let btBCSave1_click_1 = function (data) {

        if (ReturnMsg(data, 0) != "ChkLogOutOK") {
            DyAlert(ReturnMsg(data, 1), txtBarcode1_ini);
        }
        else {
            var dtLogin = data.getElementsByTagName('dtLogin');
            if (GetNodeValue(dtLogin[0], "LogOutType") != "") {
                window.location.href = "Login" + sessionStorage.getItem('isamcomp');
            }
            else {
Timerset(sessionStorage.getItem('isamcomp'));
            if ($('#txtBarcode1').val() == "") {
                DyAlert("請輸入條碼!", txtBarcode1_ini);
                $(".modal-backdrop").remove();
                return;
                }

            if ($('#txtBarcode1').val().length > 16) {
                DyAlert("條碼限制輸入16個字元!", txtBarcode1_ini);
                $(".modal-backdrop").remove();
                return;
                }

            if ($('#txtQty1').val() == "" || $('#txtQty1').val() == "0") {
                $('#txtQty1').val("1");
            }
            else {
                    if (isNaN($('#txtQty1').val())) {
                        DyAlert("請輸入數字!", txtQty1_ini );
                        return;
                    }
                    if ($('#txtQty1').val().indexOf(".") > 0) {
                        DyAlert("請輸入整數!", txtQty1_ini);
                        return;
                    }
                    //if ($('#txtQty1').val() <= 0) {
                    //    DyAlert("數量需大於0!", txtBarcode1_ini);
                    //    return;
                    //}
                    if ($('#txtQty1').val() > 9999 || $('#txtQty1').val() < -9999) {
                        DyAlert("數量需介於-9999~9999之間!", txtQty1_ini);
                        return;
                    }
                }
                var pData = {
                    Shop: $('#lblShop2').html().split(' ')[0],
                    ISAMDate: $('#lblDate2').html(),
                    BinNo: $('#lblBINNo2').html(),
                    Barcode: $('#txtBarcode1').val(),
                    Qty: $('#txtQty1').val()
                };
                PostToWebApi({ url: "api/SystemSetup/ChkSaveBINWeb", data: pData, success: afterChkSaveBINWeb });
            }
        }
    }

    let afterChkSaveBINWeb = function (data) {
        if (ReturnMsg(data, 0) != "ChkSaveBINWebOK") {
            DyAlert(ReturnMsg(data, 1));
        }
        else {
            var dtBIN = data.getElementsByTagName('dtBIN');
            if (dtBIN.length == 0) {
                if ($('#txtQty1').val() < 0) {
                    DyAlert("單品總數需大於0!", txtQty1_ini);
                    return;
                }
            }
            else {
                if (parseInt(GetNodeValue(dtBIN[0], "SumQty")) + parseInt($('#txtQty1').val()) > 999999) {
                    DyAlert("單品總數不可大於999999!", txtQty1_ini);
                    return;
                }
                if (parseInt(GetNodeValue(dtBIN[0], "SumQty")) + parseInt($('#txtQty1').val()) < 0) {
                    DyAlert("單品總數需大於0!", txtQty1_ini);
                    return;
                }
                var pData = {
                    Shop: $('#lblShop2').html().split(' ')[0],
                    ISAMDate: $('#lblDate2').html(),
                    BinNo: $('#lblBINNo2').html(),
                    Barcode: $('#txtBarcode1').val(),
                    Qty: $('#txtQty1').val()
                };
                PostToWebApi({ url: "api/SystemSetup/SaveBINWeb", data: pData, success: afterSaveBINWeb });
            }
        }
        
    }
    
    let btAdd_click = function () {
        //*
        ChkLogOut(sessionStorage.getItem('isamcomp'));
        EditMode = "A";
        Timerset(sessionStorage.getItem('isamcomp'));
        //$('#btKeyin1').prop('disabled', false);
        //$('#btBCSave1').prop('disabled', false);
        //$('#txtBarcode1').prop('disabled', false);
        //$('#btQtySave1').prop('disabled', true);
        //$('#txtQty1').prop('disabled', true);
        $('#pgISAM01Add').show();
        if ($('#pgISAM01Add').attr('hidden') == undefined) {
            $('#pgISAM01Add').show();
        }
        else {
            $('#pgISAM01Add').removeAttr('hidden');
        }
        if ($('#pgISAM01Mod').attr('hidden') == undefined) {
            $('#pgISAM01Mod').hide();
        }
        BtnSet("A");
        
    };
//#endregion

    let BtnSet = function (edit) {
        switch (edit) {
            case "A":
                $('#btAdd').prop('disabled', false);
                document.getElementById("btAdd").style.background = "blue";
                $('#btMod').prop('disabled', true);
                document.getElementById("btMod").style.background = 'gray';
                $('#btToFTP').prop('disabled', true); //true-btn不能使用
                document.getElementById("btToFTP").style.background = 'gray';

                $('#txtBarcode1').val('');
                $('#txtBarcode1').focus();
                $('#txtQty1').val("");
                $('#lblBarcode').html('');
                $('#lblQty1').html('');
                $('#lblSQty1').html('');
                $('#lblSBQty1').html('');
                $('#lblSWQty1').html('');
                $('#lblPrice').html('');
                $('#lblGDName').html('');
                break;
            case "Q":
                $('#btAdd').prop('disabled', false);
                document.getElementById("btAdd").style.background = "blue";
                $('#btMod').prop('disabled', false);
                document.getElementById("btMod").style.background = "Green";
                $('#btToFTP').prop('disabled', false); //true-btn不能使用
                document.getElementById("btToFTP").style.background = "gold";
                break;
            case "M":
                $('#btAdd').prop('disabled', true);
                document.getElementById("btAdd").style.background = 'gray';
                $('#btMod').prop('disabled', false);
                document.getElementById("btMod").style.background = "Green";
                $('#btToFTP').prop('disabled', true); //true-btn不能使用
                document.getElementById("btToFTP").style.background = 'gray';
                $('#txtBarcode3').val('');
                break;
        }
    };

//#region 上傳
    let AfterAddISAMToFTPRecWeb = function (data) {
        if (ReturnMsg(data, 0) != "AddISAMToFTPRecWebOK") {
            if (ReturnMsg(data, 1) == "FTP") {
                DyAlert("FTP設定有誤，請重新確認!")
            }
            else if (ReturnMsg(data, 1) == "上傳記錄") {
                DyAlert("待上傳記錄新增失敗，請重新上傳!")
            }
            else if (ReturnMsg(data, 1) == "上傳資料") {
                DyAlert("無上傳資料，請重新確認!")
            }
            else if (ReturnMsg(data, 1) == "上傳檔案") {
                DyAlert("待上傳檔案不存在，請重新確認!")
            }
            else {
                DyAlert(ReturnMsg(data, 1));
            }
        }
        else {
            DyAlert("上傳成功!")
        }
    }

    let CallSendToFTP = function () {
        ChkLogOut_1(CallSendToFTP_1);
    };

    let CallSendToFTP_1 = function (data) {
        if (ReturnMsg(data, 0) != "ChkLogOutOK") {
            DyAlert(ReturnMsg(data, 1));
        }
        else {
            var dtLogin = data.getElementsByTagName('dtLogin');
            if (GetNodeValue(dtLogin[0], "LogOutType") != "") {
                window.location.href = "Login" + sessionStorage.getItem('isamcomp');
            }
            else {
                Timerset(sessionStorage.getItem('isamcomp'));
                var cData = {
                    Type: "T",
                    Shop: $('#lblShop2').html().split(' ')[0],
                    ISAMDate: $('#lblDate2').html(),
                    BinNo: $('#lblBINNo2').html()
                }
                PostToWebApi({ url: "api/SystemSetup/AddISAMToFTPRecWeb", data: cData, success: AfterAddISAMToFTPRecWeb });
            }
        }
    }


    let btToFTP_click = function () {
        //*
        ChkLogOut(sessionStorage.getItem('isamcomp'));
        Timerset(sessionStorage.getItem('isamcomp'));
        DyConfirm("是否要上傳" + $('#lblBINNo2').text() + "分區盤點資料？", CallSendToFTP, DummyFunction);
    };
//#endregion

//#region 返回
    let afterRtnclick = function () {
        if (EditMode == "A" || EditMode == "M") {
            EditMode = "Q";
            BtnSet(EditMode);
        }
    };

    let btRtn_click = function () {
        ChkLogOut(sessionStorage.getItem('isamcomp'));

        $('#btAdd').prop('disabled', false);
        document.getElementById("btAdd").style.background = "blue";
        $('#btMod').prop('disabled', false);
        document.getElementById("btMod").style.background = "green";
        $('#btToFTP').prop('disabled', false);
        document.getElementById("btToFTP").style.background = "gold";


        if (EditMode == "Q") {
            $('#ISAM01btns').hide();
            $('#pgISAM01Init').show();
            $('#pgISAM01Add').hide();
            $('#pgISAM01Mod').hide();
            //$('#pgISAM01UpFtp').hide();
        } else if (EditMode == "A" || EditMode == "M") {
            $('#ISAM01btns').show();
            $('#pgISAM01Init').hide();
            $('#pgISAM01Add').hide();
            $('#pgISAM01Mod').hide();
            //$('#pgISAM01UpFtp').hide();
        }
        Timerset(sessionStorage.getItem('isamcomp'));
    };
//#endregion

//#region 輸入盤點日期,分區
    let afterSearchBINWeb = function (data) {
        //EditMode = "Q";
        if (ReturnMsg(data, 0) != "SearchBINWebOK") {
            DyAlert(ReturnMsg(data, 1));
        }
        else {
            var dtBINData = data.getElementsByTagName('dtBINData');
            if (dtBINData.length > 0) {
                if (GetNodeValue(dtBINData[0], "BINman") != $('#lblManID1').html().split(' ')[0]) {
                    DyAlert("盤點人員" + GetNodeValue(dtBINData[0], "BINman") + "已建立分區" + $('#txtBinNo').val() + "之分區單!!", DummyFunction);
                    return;
                }
            }

            //AssignVar();
            //alert(GetNodeValue(dtISAMShop[0], "STName"));
            EditMode = "Q";
            $('#lblShop2').html($('#lblShop1').html());
            $('#lblBINNo2').html($('#txtBinNo').val());
            $('#lblDate2').html($('#txtISAMDate').val());
            $('#lblSWQty1title').html($('#lblShop1').html().split(' ')[0] + "門市總數：");
            $('#lblSBQty1title').html($('#txtBinNo').val() + "分區總數：");
            $('#lblSQty1').html('');
            $('#lblSQty1title').html($('#txtBinNo').val() + "分區單品總數：");
            $('#lblSBQty1').html('');
            $('#lblSWQty1').html('');
            $('#lblPrice').html('');
            $('#lblGDName').html("品名XXX");
            $('#pgISAM01Init').hide();
            if ($('#ISAM01btns').attr('hidden') == undefined) {
                $('#ISAM01btns').show();
            }
            else {
                $('#ISAM01btns').removeAttr('hidden');
            }
        }
    };

    let btSave_click = function () {
        //*
        ChkLogOut(sessionStorage.getItem('isamcomp'));
        Timerset(sessionStorage.getItem('isamcomp'));
        if ($('#txtISAMDate').val() == "" | $('#txtISAMDate').val() == null) {
            DyAlert("請輸入盤點日期!!", function () { $('#txtISAMDate').focus() });
            $(".modal-backdrop").remove();
            return;
        }
        if ($('#txtBinNo').val() == "" | $('#txtBinNo').val() == null) {
            DyAlert("請輸入分區代碼!!", function () { $('#txtBinNo').focus() });
            $(".modal-backdrop").remove();
            return;
        }
        else {
            if ($('#txtBinNo').val().length>3) {
                DyAlert("分區代碼不可超過3個字元!!", function () { $('#txtBinNo').focus() });
                $(".modal-backdrop").remove();
                return;
            }
        }
        var pData = {
            Shop: $('#lblShop1').html().split(' ')[0],
            ISAMDate: $('#txtISAMDate').val(),
            BinNo: $('#txtBinNo').val()
        }
        PostToWebApi({ url: "api/SystemSetup/SearchBINWeb", data: pData, success: afterSearchBINWeb });
    };
//#endregion

    //清除
    let btClear_click = function (bt) {
        //Timerset();
        $('#txtActivityCode').val('');
        $('#txtPSName').val('');
        $('#txtEDDate').val('');
    };

    //查詢
    let btQuery_click = function (bt) {
        //Timerset();
        $('#tbQuery thead tr th').css('background-color', '#ffb620')
        $('#btQuery').prop('disabled', true)
        ShowLoading();
        var pData = {
            ActivityCode: $('#txtActivityCode').val(),
            PSName: $('#txtPSName').val(),
            EDDate: $('#txtEDDate').val().toString().replaceAll('-', '/')
        }
        PostToWebApi({ url: "api/SystemSetup/MSSD102Query", data: pData, success: afterMSSD102Query });
    };

    let afterMSSD102Query = function (data) {
        CloseLoading();
        if (ReturnMsg(data, 0) != "MSSD102QueryOK") {
            DyAlert(ReturnMsg(data, 1), function () { $('#btQuery').prop('disabled', false); });
        }
        else {
            $('#btQuery').prop('disabled', false);
            var dtE = data.getElementsByTagName('dtE');
            grdM.BindData(dtE);

            if (dtE.length == 0) {
                DyAlert("無符合資料!");
                $(".modal-backdrop").remove();
                return;
            }
        }
    };

    //活動代號...
    let btActivityCode_click = function (bt) {
        //Timerset();
        var pData = {
            ActivityCode: $('#txtActivityCode').val()
        }
        PostToWebApi({ url: "api/SystemSetup/MSSD102_LookUpActivityCode", data: pData, success: afterMSSD102_LookUpActivityCode });
    };

    let afterMSSD102_LookUpActivityCode = function (data) {
        if (ReturnMsg(data, 0) != "MSSD102_LookUpActivityCodeOK") {
            DyAlert(ReturnMsg(data, 1));
        }
        else {
            var dtE = data.getElementsByTagName('dtE');
            $('#txtQLookup_ActivityCode').val($('#txtActivityCode').val());
            $('#modal_Lookup_ActivityCode').modal('show');
            setTimeout(function () {
                grdLookUp_ActivityCode.BindData(dtE);
                $('#tbDataLookup_ActivityCode tbody tr .tdCol2').filter(function () { return $(this).text() == $('#txtActivityCode').val(); }).closest('tr').find('.tdCol1 input:radio').prop('checked', true);
            }, 500);
        }
    };

    let btQLookup_ActivityCode_click = function (bt) {
        //Timerset();
        $('#btQLookup_ActivityCode').prop('disabled', true)
        var pData = {
            ActivityCode: $('#txtQLookup_ActivityCode').val()
        }
        PostToWebApi({ url: "api/SystemSetup/MSSD102_LookUpActivityCode", data: pData, success: afterMSSD102_QLookUpActivityCode });
    };

    let afterMSSD102_QLookUpActivityCode = function (data) {
        if (ReturnMsg(data, 0) != "MSSD102_LookUpActivityCodeOK") {
            DyAlert(ReturnMsg(data, 1), function () {
                $('#btQLookup_ActivityCode').prop('disabled', false);
            });
        }
        else {
            var dtE = data.getElementsByTagName('dtE');
            if (dtE.length == 0) {
                DyAlert("無符合資料!", function () {
                    $('#btQLookup_ActivityCode').prop('disabled', false);
                });
                //$(".modal-backdrop").remove();
                return;
            }
            setTimeout(function () {
                grdLookUp_ActivityCode.BindData(dtE);
                $('#btQLookup_ActivityCode').prop('disabled', false);
            }, 500);
        }
    };

    let btLpOK_ActivityCode_click = function (bt) {
        //Timerset();
        $('#btLpOK_ActivityCode').prop('disabled', true);
        var obchkedtd = $('#tbDataLookup_ActivityCode input[type="radio"]:checked');
        var chkedRow = obchkedtd.length.toString();

        if (chkedRow == 0) {
            DyAlert("未選取活動代號，請重新確認!", function () {
                $('#btLpOK_ActivityCode').prop('disabled', false);
            });
        }
        else {
            var a = $(obchkedtd[0]).closest('tr');
            var trNode = $(a).prop('Record');
            $('#txtActivityCode').val(GetNodeValue(trNode, "ActivityCode"))
            $('#btLpOK_ActivityCode').prop('disabled', false);
            $('#modal_Lookup_ActivityCode').modal('hide')
        }
    };

    let btLpExit_ActivityCode_click = function (bt) {
        //Timerset();
        $('#modal_Lookup_ActivityCode').modal('hide')
    };

    let btLpClear_ActivityCode_click = function (bt) {
        $("#txtQLookup_ActivityCode").val('');
        $("#tbDataLookup_ActivityCode .checkbox").prop('checked', false);
    };

    let btRe_PSNO_click = function (bt) {
        //Timerset();
        $('#modal_PSNO').modal('hide')
    };

    let btRe_PSNO_ShopNoDate_click = function (bt) {
        //Timerset();
        $('#modal_PSNO_ShopNoDate').modal('hide')
    };
//#region FormLoad
    let GetInitMSSD102 = function (data) {
        if (ReturnMsg(data, 0) != "GetInitmsDMOK") {
            DyAlert(ReturnMsg(data, 1));
        }
       else {
           var dtE = data.getElementsByTagName('dtE');
            $('#lblProgramName').html(GetNodeValue(dtE[0], "ChineseName"));
            AssignVar();

            $('#btQuery').click(function () { btQuery_click(this) });
            $('#btClear').click(function () { btClear_click(this) });
            $('#btActivityCode').click(function () { btActivityCode_click(this) });
            $('#btQLookup_ActivityCode').click(function () { btQLookup_ActivityCode_click(this) });
            $('#btLpOK_ActivityCode').click(function () { btLpOK_ActivityCode_click(this) });
            $('#btLpExit_ActivityCode').click(function () { btLpExit_ActivityCode_click(this) });
            $('#btLpClear_ActivityCode').click(function () { btLpClear_ActivityCode_click(this) });

            $('#btRe_PSNO').click(function () { btRe_PSNO_click(this) });
            $('#rdoShop_PSNO,#rdoDate_PSNO').change(function () { QueryPSNO($('#lblPSNO_PSNO').html()) });

            $('#btRe_PSNO_ShopNoDate').click(function () { btRe_PSNO_ShopNoDate_click(this) });
        }
    };
    
    let afterLoadPage = function () {
        var pData = {
            ProgramID: "MSSD102"
        }
        PostToWebApi({ url: "api/SystemSetup/GetInitmsDM", data: pData, success: GetInitMSSD102 });
    };
//#endregion
    

    if ($('#pgMSSD102').length == 0) {  
        AllPages = new LoadAllPages(ParentNode, "SystemSetup/MSSD102", ["MSSD102btns", "pgMSSD102Init", "pgMSSD102Add", "pgMSSD102Mod"], afterLoadPage);
    };


}