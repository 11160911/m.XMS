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
                    { type: "TextAmt", name: "Per" }
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

    let InitComboItem = function (cboYear) {
        var y2 = new Date().getFullYear();
        for (i = 2020; i <= y2; i++) {
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