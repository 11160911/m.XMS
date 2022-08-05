var PageISAMWHSET = function (ParentNode) {
    let WhSt
    let AssignVar = function () {
        //alert("進入AssignVar");
        //$('#selST_ID').change(function () {
        //    alert("st_id");
        //    let st_id = $('#selST_ID').val();
        //    //InitSelectItem($('#selLayerNo')[0], dtMachineListSpecBlank, "LayerNo", "LayerNo", true, "*請選擇貨倉代號");
        //    if (st_id == "") {
        //    }
        //    else {
        //        var pData = {
        //            ST_ID: st_id
        //        };
        //        //alert("PostToWebApi");
        //        //PostToWebApi({ url: "api/SystemSetup/GetWh", data: pData, success: AfterGetCkNoByST_ID });
        //    }
        //});

       //let AfterGetCkNoByST_ID = function (data) {
       //    let dtWarehouseDSV = data.getElementsByTagName('dtWh');
       // }

      $('#btSaveISAMWH').click(function () {
          if ($('#selST_ID').val() != "") {
              WhSt = $('#selST_ID option:selected').text()
              var pData = {
                  WHSetSV: $('#selST_ID').val()
              };
              PostToWebApi({ url: "api/SystemSetup/SaveST_ID", data: pData, success: AfterSaveNewInventorySV });
         }
        });
    };

    let AfterSaveNewInventorySV = function (data) {
        if (ReturnMsg(data, 0) != "SaveWhSetOK") {
            DyAlert(ReturnMsg(data, 0));
        }
        else {
            DyAlert("完成!");
            //alert(WhSt);
            //$('#defWh').text(ReturnMsg(data, 1));
            $('#defWh').text(WhSt);
        }
    }

    let afterGetInitWhSet = function (data) {
        let dtWh;
        let dtUserWh;
       //alert("afterGetInitWhSet");
        dtUserWh = data.getElementsByTagName('dtUserWh');
        dtWh = data.getElementsByTagName('dtWh');
        $('#defWh').text(GetNodeValue(dtUserWh[0], "WhName"));
        
        //alert("InitSelectItem");
        InitSelectItem($('#selST_ID')[0], dtWh, "ST_ID", "STName", true, "*請選擇店別" );
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
        AllPages = new LoadAllPages(ParentNode, "SystemSetup/ISAMWhSet", ["pgWhSet"], afterLoadPage);
    };
}
