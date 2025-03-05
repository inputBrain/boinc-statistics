/**
 * DataTables initialization for BOINC Statistics tables
 */
function initializeDataTable(tableId, options = {}) {
    // Default configuration
    const defaultConfig = {
        order: [],
        paging: false,
        info: false,
        searching: true,
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
        language: {
            search: "",
            searchPlaceholder: "Search projects..."
        },
        dom: '<"dt-search"f>rt<"dt-info"i><"dt-pagination"p>'
    };

    // Merge the provided options with defaults
    const config = { ...defaultConfig, ...options };

    // Initialize DataTable
    const table = $(`#${tableId}`).DataTable(config);

    // Apply common enhancements
    enhanceTableInteractions(tableId);

    return table;
}

/**
 * Apply common enhancements to table interactions
 */
function enhanceTableInteractions(tableId) {
    // Style the search box
    $('.dt-search input').addClass('focus:ring-2 focus:ring-blue-500 focus:outline-none shadow-sm');

    // Place search in a better position
    $('.dt-search').addClass('w-full sm:w-64');

    // Check if we need to show the horizontal scroll indicator
    const tableContainer = $('.table-container');

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
        $(`#${tableId}`).addClass('table-mobile-view');
    }

    mediaQuery.addEventListener('change', (e) => {
        if (e.matches) {
            $(`#${tableId}`).addClass('table-mobile-view');
        } else {
            $(`#${tableId}`).removeClass('table-mobile-view');
        }
    });

    // Handle horizontal scroll indicators
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
}