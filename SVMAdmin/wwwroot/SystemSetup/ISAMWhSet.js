var PageISAMWHSET = function (ParentNode) {
    let dtWh;

    let AssignVar = function () {
        alert("AssignVar");
        //$('#selST_ID').change(function () {
        //    let st_id = $('#selST_ID').val();
        //    alert("st_id");
        //    //InitSelectItem($('#selLayerNo')[0], dtMachineListSpecBlank, "LayerNo", "LayerNo", true, "*請選擇貨倉代號");
        //    if (st_id == "") {
        //    }
        //    else {
        //        var pData = {
        //            ST_ID: st_id
        //        };
        //        alert("PostToWebApi");
        //        PostToWebApi({ url: "api/SystemSetup/GetWh", data: pData, success: AfterGetCkNoByST_ID });
        //    }
        //});

         alert("AfterGetCkNoByST_ID");
       //let AfterGetCkNoByST_ID = function (data) {
       //     let dtWarehouseDSV = data.getElementsByTagName('dtWh');
       // }

        alert("btSaveISAMWH");
    //  $('#btSaveISAMWH').click(function () {
    //        let records = [];
    //        let cbs = $('#ContainerChanSet .CubeChannel-1');
    //        for (let i = 0; i < cbs.length; i++) {
    //            var cb = cbs[i];
    //            if ($(cb).hasClass('CubeChannel-1_mask')) {
    //                continue;
    //            }
    //            let rec = $(cb).prop('Record');
    //            let MachineInfo = $(cb).prop('MachineInfo');
    //            if (rec != null) {
    //                //let record = {
    //                //    WhNO: ST_ID,
    //                //    PLU: GetNodeValue(rec, "GD_NO"),
    //                //    PTNum: 0,
    //                //    SafeNum: 1,
    //                //    In_Date: "",
    //                //    Out_Date: "",
    //                //    StartSalesDate: "",
    //                //    EndSalesDate: "",
    //                //    DisPlayNum: $(cb).find('.ChanQty').val(),
    //                //    CkNo: CkNo,
    //                //    Layer: LayerNo,
    //                //    Sno: GetNodeValue(MachineInfo, "ChannelNo"),
    //                //    EffectiveDate: ""
    //                //}
    //                //records.push(record);
    //            }
    //        }
    //        var pData = {
    //            InventorySV: records
    //        };
    //        PostToWebApi({ url: "api/SystemSetup/SaveNewInventorySV", data: pData, success: AfterSaveNewInventorySV });
    //    });
    };

    let afterGetInitWhSet = function (data) {
        alert("afterGetInitWhSet");
        dtWh = data.getElementsByTagName('dtWh');
        alert("InitSelectItem");
        InitSelectItem($('#selST_ID')[0], dtWh, "ST_ID", "STName", true, "*請選擇店代號");
        AssignVar();
        //$('#btSaveISAMW').click(function () { btSave_click(); });

    };

    let afterLoadPage = function () {
        //alert("afterLoadPage");
        PostToWebApi({ url: "api/SystemSetup/GetWh", success: afterGetInitWhSet });
        $('#pgWhSet').show();
    };

    if ($('#pgWhSet').length == 0) {
        //2021-04-29 Debug用，按F12後，在主控台內會顯示aaaaaa
        //console.log("aaaaaa");
        //alert("VMN01");
        AllPages = new LoadAllPages(ParentNode, "ISAMWhSet", ["pgWhSet"], afterLoadPage);
    };
}
