$(document).ready(function() {
    $('#newsTable').DataTable({
        "order": [],
        "paging": false,
        "info": false,
        "columnDefs": [
            { "orderable": true, "targets": "_all" },
            {
                "type": "html",
                "targets": 0
            },
            {
                "type": "num",
                "targets": [1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11]
            },
            {
                "orderable": false,
                "targets": [13]
            }
        ],
        "language": {
            "search": "Filter:"
        }
    });
});