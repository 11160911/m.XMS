var PageMSSA105 = function (ParentNode) {

    let grdM;

    let AssignVar = function () {

        grdM = new DynGrid(
            {
                table_lement: $('#tbQuery')[0],
                class_collection: ["tdCol1 text-center", "tdCol2 label-align", "tdCol3 label-align", "tdCol4 text-center"],
                fields_info: [
                    { type: "Text", name: "ID", style: "" },
                    { type: "TextAmt", name: "Cash1" },
                    { type: "TextAmt", name: "Cash2" },
                    { type: "TextPercent", name: "Per" }
                ],
                //rows_per_page: 10,
                method_clickrow: click_PLU,
                afterBind: InitDetailButton,
                sortable: "N"
            }
        );

        grdMoonth = new DynGrid(
            {
                table_lement: $('#tbQueryMonth')[0],
                class_collection: ["tdCol1 text-center", "tdCol2 label-align", "tdCol3 label-align", "tdCol4 text-center"],
                fields_info: [
                    { type: "Text", name: "ID", style: "" },
                    { type: "TextAmt", name: "Cash1" },
                    { type: "TextAmt", name: "Cash2" },
                    { type: "TextPercent", name: "Per" }
                ],
                //rows_per_page: 10,
                method_clickrow: click_PLU,
                afterBind: InitDetailButton,
                sortable: "N"
            }
        );
        return;
    };

    let click_PLU = function (tr) {

    };

    let InitDetailButton = function () {
        $('#tbQuery tbody tr td').click(function () { Step1_click(this) });

    };

    let Step1_click = function (bt) {
        $('#tbQuery td').closest('tr').css('background-color', 'white');

        $(bt).closest('tr').click();
        $('.msg-valid').hide();
        var node = $(grdM.ActiveRowTR()).prop('Record');
        $('#tbQuery td:contains(' + GetNodeValue(node, 'ID') + ')').closest('tr').css('background-color', '#DEEBF7');

        $('#modal_Step1').modal('show');

        $('#lblYear').html($('#cboYear').val() + '年');
        $('#lblMonth').html(GetNodeValue(node, 'ID'));
        setTimeout(function () {
            Query_Step1_click();
        }, 500);

    };
    let Query_Step1_click = function () {
        ShowLoading();

        var Year = $('#lblYear').html()
        var Month = $('#lblMonth').html()
        var Flag = ""
        //月份
        if ($('#rdoS').prop('checked') == true) {
            Flag = "S";
            $('#lblType').html('月份：');
            $('#tbQueryMonth thead tr th#thead1').html('店別');
            $('#tbQueryMonth thead td#td0').html('店總業績');
        }
        //店別
        else if ($('#rdoD').prop('checked') == true) {
            Flag = "D";
            Month = Month.split('-')[0];
            $('#lblType').html('店別：');
            $('#tbQueryMonth thead tr th#thead1').html('月份');
            $('#tbQueryMonth thead td#td0').html('月總業績');
        }
        //區課
        else if ($('#rdoB').prop('checked') == true) {
            Flag = "B";
            Month = Month.split('-')[0];
            $('#lblType').html('區課：');
            $('#tbQueryMonth thead tr th#thead1').html('店別');
            $('#tbQueryMonth thead td#td0').html('店總業績');
        }

        setTimeout(function () {
            var pData = {
                Year: Year,
                Month: Month,
                Flag: Flag
            }
            PostToWebApi({ url: "api/SystemSetup/MSSA105Query_Step1", data: pData, success: afterQuery_Step1 });
        }, 1000);
    };

    let afterQuery_Step1 = function (data) {
        CloseLoading();
        if (ReturnMsg(data, 0) != "MSSA105Query_Step1OK") {
            DyAlert(ReturnMsg(data, 1));
        }
        else {
            $('#modal_Step1').css("padding-right", "0px");
            var Year = $('#cboYear').val();
            var YearBef = Year - 1;
            $('#tbQueryMonth #thead2').html(YearBef + '年度業績');
            $('#tbQueryMonth #thead3').html(Year + '年度業績');

            var dtH = data.getElementsByTagName('dtH');
            $('#tbQueryMonth thead td#td1').html(parseInt(GetNodeValue(dtH[0], "SumCash1")).toLocaleString('en-US'));
            $('#tbQueryMonth thead td#td2').html(parseInt(GetNodeValue(dtH[0], "SumCash2")).toLocaleString('en-US'));
            $('#tbQueryMonth thead td#td3').html(GetNodeValue(dtH[0], "SumPer"));

            var dtE = data.getElementsByTagName('dtE');
            grdMoonth.BindData(dtE);


        }
    };

    //查詢
    let btQuery_click = function (bt) {
        //Timerset();
        $('#btQuery').prop('disabled', true)

        //年
        if ($('#cboYear').val() == "") {
            DyAlert("年度需輸入!", function () { $('#btQuery').prop('disabled', false); })
            return
        }

        ShowLoading();

        var Flag = ""
        //月份
        if ($('#rdoS').prop('checked') == true) {
            Flag = "S";
        }
        //店別
        else if ($('#rdoD').prop('checked') == true) {
            Flag = "D";
        }
        //區課
        else if ($('#rdoB').prop('checked') == true) {
            Flag = "B";
        }

        setTimeout(function () {
            var pData = {
                Year: $('#cboYear').val(),
                Flag: Flag
            }
            PostToWebApi({ url: "api/SystemSetup/MSSA105Query", data: pData, success: afterMSSA105Query });
        }, 1000);
    };

    let afterMSSA105Query = function (data) {
        CloseLoading();
        if (ReturnMsg(data, 0) != "MSSA105QueryOK") {
            DyAlert(ReturnMsg(data, 1), function () { $('#btQuery').prop('disabled', false); });
        }
        else {
            $('#btQuery').prop('disabled', false);
            var dtE = data.getElementsByTagName('dtE');
            grdM.BindData(dtE);

            var heads = $('#tbQuery thead tr th#thead1');
            if ($('#rdoS').prop('checked')) {
                $(heads).html('月份');
            }
            else if ($('#rdoD').prop('checked')) {
                $(heads).html('店別');
            }
            else if ($('#rdoB').prop('checked')) {
                $(heads).html('區課');
            }

            if (dtE.length == 0) {
                DyAlert("無符合資料!");
                $(".modal-backdrop").remove();
                $('#tbQuery thead td#td1').html('');
                $('#tbQuery thead td#td2').html('');
                $('#tbQuery thead td#td3').html('');
                return;
            }
            var Year = $('#cboYear').val();
            var YearBef = Year - 1;
            $('#tbQuery #thead2').html(YearBef + '年度業績');
            $('#tbQuery #thead3').html(Year + '年度業績');

            var dtH = data.getElementsByTagName('dtH');
            $('#tbQuery thead td#td1').html(parseInt(GetNodeValue(dtH[0], "SumCash1")).toLocaleString('en-US'));
            $('#tbQuery thead td#td2').html(parseInt(GetNodeValue(dtH[0], "SumCash2")).toLocaleString('en-US'));
            $('#tbQuery thead td#td3').html(GetNodeValue(dtH[0], "SumPer"));
        }
    };
    let btExit_Step1_click = function (bt) {
        $('#modal_Step1').modal('hide');
        //$('#pgMSSD101Init').show();
        //$('#pgMSSD101_PS_STEP1').hide();
    };
    let InitComboItem = function (cboYear) {
        var y2 = new Date().getFullYear();
        for (i = y2; i >= 2020; i--) {
            cboYear.append($('<option>', { value: i, text: i }));
        }
    };

    //#region FormLoad
    let GetInitMSSA105 = function (data) {
        if (ReturnMsg(data, 0) != "GetInitmsDMOK") {
            DyAlert(ReturnMsg(data, 1));
        }
        else {
            var dtE = data.getElementsByTagName('dtE');
            if (dtE.length > 0) {
                $('#lblProgramName').html(GetNodeValue(dtE[0], "ChineseName"));
            }
            InitComboItem($("#cboYear"));    //下拉選單

            AssignVar();

            $('#btQuery').click(function () { btQuery_click(this) });
            $('#btExit_Step1').click(function () { btExit_Step1_click(this) });

        }
    };

    let afterLoadPage = function () {
        var pData = {
            ProgramID: "MSSA105"
        }
        PostToWebApi({ url: "api/SystemSetup/GetInitmsDM", data: pData, success: GetInitMSSA105 });
    };


    if ($('#pgMSSA105').length == 0) {
        AllPages = new LoadAllPages(ParentNode, "SystemSetup/MSSA105", ["pgMSSA105Init"], afterLoadPage);
    };
    //#endregion


}