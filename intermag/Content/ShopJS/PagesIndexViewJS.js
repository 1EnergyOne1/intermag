$(function () {
    //Подтверждение удаления страницы
    $("a.delete").click(function () {
        if (!confirm("Удалить страницу?")) return false;
    });

    //-------------------------------------------------------------------------------

    // Sorting script

    $("table#pages tbody").sortable(
        {
            items: "tr:not(.home)",
            placeholder: "ui-state-hilight",
            update: function () {
                var ids = $("table#pages tbody").sortable("serialize");
                var url = "/Admin/Pages/ReorderPages";

                $.post(url, ids, function (data) {
                });
            }
        });
});