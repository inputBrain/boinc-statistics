function initializeDataTable(tableId, options = {}) {
    const defaultConfig = {
        order: [],
        paging: false,
        info: false,
        responsive: false,
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
    const tableContainer = $('.table-container');
    
    populateDataLabels(tableId);

    $(document).on('click', `#${tableId} tbody tr::after`, function(e) {
        e.stopPropagation();
        const url = $(this).closest('tr').attr('data-url');
        if (url) {
            window.open(url, '_blank');
        }
    });


    function checkScroll() {
        if (tableContainer[0] && tableContainer[0].scrollWidth > tableContainer[0].clientWidth) {
            $('.table-scroll-indicator').addClass('opacity-100');
        } else {
            $('.table-scroll-indicator').removeClass('opacity-100');
        }
    }

    setTimeout(checkScroll, 100);
    $(window).resize(checkScroll);

    const cardModeMedia = window.matchMedia('(max-width: 768px)');

    function handleTableMode(e) {
        if (e.matches) {
            activateCardMode(tableId);
        } else {
            deactivateCardMode(tableId);
        }
    }
    
    handleTableMode(cardModeMedia);
    
    cardModeMedia.addEventListener('change', handleTableMode);

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

function populateDataLabels(tableId) {
    const table = $(`#${tableId}`);
    const headers = table.find('thead th').map(function() {
        return $(this).text().trim();
    }).get();

    table.find('tbody tr').each(function() {
        $(this).find('td').each(function(i) {
            $(this).attr('data-label', headers[i]);
        });


        const url = $(this).attr('onclick');
        if (url) {
            const extractedUrl = url.match(/window\.open\('([^']+)'/);
            if (extractedUrl && extractedUrl[1]) {
                $(this).attr('data-url', extractedUrl[1]).removeAttr('onclick');
            }
        }
    });
}

function activateCardMode(tableId) {
    const table = $(`#${tableId}`);
    table.addClass('card-mode');
}


function deactivateCardMode(tableId) {
    const table = $(`#${tableId}`);
    table.removeClass('card-mode');

    table.find('tbody tr').each(function() {
        const url = $(this).attr('data-url');
        if (url) {
            $(this).attr('onclick', `window.open('${url}', '_blank')`);
        }
    });
}