function initializeDataTable(tableId, options = {}) {
    const defaultConfig = {
        order: [],
        paging: false,
        info: false,
        responsive: {
            details: {
                display: $.fn.dataTable.Responsive.display.childRowImmediate,
                type: 'none',
                target: ''
            }
        },
        columnDefs: [
            { orderable: true, targets: "_all" }
        ],
        dom: 'rt<"dt-info"i><"dt-pagination"p>',
        searching: true
    };

    const config = { ...defaultConfig, ...options };

    const table = $(`#${tableId}`).DataTable(config);

    $('.dt-search input').on('keyup', function() {
        table.search(this.value).draw();
    });

    enhanceTableInteractions(tableId);

    return table;
}

function enhanceTableInteractions(tableId) {
    $('.dt-search input').addClass('focus:ring-2 focus:ring-blue-500 focus:outline-none shadow-sm');

    $('.dt-search').addClass('w-full sm:w-64');

    const tableContainer = $('.table-container');

    function checkScroll() {
        if (tableContainer[0].scrollWidth > tableContainer[0].clientWidth) {
            $('.table-scroll-indicator').addClass('opacity-100');
        } else {
            $('.table-scroll-indicator').removeClass('opacity-100');
        }
    }

    setTimeout(checkScroll, 100);

    $(window).resize(checkScroll);

    const mediaQuery = window.matchMedia('(max-width: 768px)');
    if (mediaQuery.matches) {
        $(`#${tableId}`).addClass('table-mobile-view');
    }

    mediaQuery.addEventListener('change', (e) => {
        if (e.matches) {
            $(`#${tableId}`).addClass('table-mobile-view');
        } else {
            $(`#${tableId}`).removeClass('table-mobile-view');
        }
    });

    tableContainer.on('scroll', function() {
        const maxScroll = this.scrollWidth - this.clientWidth;
        const currentScroll = this.scrollLeft;

        if (currentScroll >= maxScroll - 5) {
            $('.table-scroll-indicator').addClass('opacity-0');
        } else {
            $('.table-scroll-indicator').removeClass('opacity-0');
        }
    });
}