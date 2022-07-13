var PRINT = PRINT ? (function () { })() : {

    init: function () {

        $("#printDetail").on('click', function (event) {
            $(document).trigger("open.wb-lbx", [
                [
                    {
                        src: "#print-employee",
                        type: "inline"
                    }
                ],
                true
            ]);

        });
    }
}

wb.doc.on("wb-ready.wb", function (evt) {
    PRINT.init();
});
