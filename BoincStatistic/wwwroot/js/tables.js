$(document).ready(function() {
    const table = $('#projectTable').DataTable({
        "order": [], // Remove default ordering
        "paging": false, // Disable pagination
        "info": false, // Hide "Showing X of Y entries" text
        "searching": true, // Enable search
        "responsive": {
            details: {
                display: $.fn.dataTable.Responsive.display.childRowImmediate,
                type: 'none',
                target: ''
            }
        },
        "columnDefs": [
            { "orderable": true, "targets": "_all" }, // Make all columns sortable
            {
                "type": "html",
                "targets": 0 // Project name column has HTML content
            },
            {
                "type": "num",
                "targets": [1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11] // Numeric columns
            }
        ],
        "language": {
            "search": "",
            "searchPlaceholder": "Search projects..."
        },
        "dom": '<"dt-search"f>rt<"dt-info"i><"dt-pagination"p>',
        "footerCallback": function(row, data, start, end, display) {
            // This ensures the footer remains visible during filtering
            $(this.api().column(0).footer()).html('Total:');
        },
        "initComplete": function() {
            // Style the search box
            $('.dt-search input').addClass('focus:ring-2 focus:ring-blue-500 focus:outline-none shadow-sm');

            // Place search in a better position
            $('.dt-search').addClass('w-full sm:w-64');

            // Check if we need to show the horizontal scroll indicator
            const tableContainer = $('.table-container');
            const table = tableContainer.find('table');

            function checkScroll() {
                if (tableContainer[0].scrollWidth > tableContainer[0].clientWidth) {
                    $('.table-scroll-indicator').addClass('opacity-100');
                } else {
                    $('.table-scroll-indicator').removeClass('opacity-100');
                }
            }

            // Initial check
            setTimeout(checkScroll, 100);

            // Check on window resize
            $(window).resize(checkScroll);

            // Show column info on smaller screens
            const mediaQuery = window.matchMedia('(max-width: 768px)');
            if (mediaQuery.matches) {
                $('#projectTable').addClass('table-mobile-view');
            }

            mediaQuery.addEventListener('change', (e) => {
                if (e.matches) {
                    $('#projectTable').addClass('table-mobile-view');
                } else {
                    $('#projectTable').removeClass('table-mobile-view');
                }
            });
        }
    });

    // Handle horizontal scroll indicators
    const tableContainer = $('.table-container');
    tableContainer.on('scroll', function() {
        const maxScroll = this.scrollWidth - this.clientWidth;
        const currentScroll = this.scrollLeft;

        // If we're at the end of the scroll
        if (currentScroll >= maxScroll - 5) {
            $('.table-scroll-indicator').addClass('opacity-0');
        } else {
            $('.table-scroll-indicator').removeClass('opacity-0');
        }
    });
});