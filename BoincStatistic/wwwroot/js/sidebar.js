document.addEventListener('DOMContentLoaded', function() {
    const sidebar = document.querySelector('.crypto-sidebar');
    const toggle = document.getElementById('sidebarToggle');


    const isSidebarExpanded = localStorage.getItem('sidebarExpanded') === 'true';
    if (isSidebarExpanded) {
        sidebar.classList.add('expanded');
    }
    
    toggle.addEventListener('click', function() {
        sidebar.classList.toggle('expanded');

        localStorage.setItem('sidebarExpanded', sidebar.classList.contains('expanded'));
    });
    
    document.addEventListener('click', function(e) {
        if (window.innerWidth <= 768 &&
            !sidebar.contains(e.target) &&
            !toggle.contains(e.target) &&
            sidebar.classList.contains('expanded')) {
            sidebar.classList.remove('expanded');
            localStorage.setItem('sidebarExpanded', false);
        }
    });
});