// site.js

// Function to toggle the sidebar visibility
function toggleSidebar() {
    const sidebar = document.querySelector('.sidebar');
    sidebar.classList.toggle('sidebar-hidden');
}

// Event listener for a button or link that toggles the sidebar
const toggleSidebarButton = document.querySelector('#toggle-sidebar-button');
toggleSidebarButton.addEventListener('click', toggleSidebar);
