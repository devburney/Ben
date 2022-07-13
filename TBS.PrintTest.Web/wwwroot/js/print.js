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
    },

    PrintDetail: function () {
        var print_div = document.getElementById("print-contents");
        var print_area = window.open();
        print_area.document.write(print_div.innerHTML);
        print_area.document.close();
        print_area.focus();
        print_area.print();
        print_area.close();
    }
}

wb.doc.on("wb-ready.wb", function (evt) {
    PRINT.init();
});
