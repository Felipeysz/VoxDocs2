    // injeta a sidebar
    fetch('/Components/sidebar-dashboard.html')
      .then(r => r.text())
      .then(html => {
        document.getElementById('sidebar-container').outerHTML = html;
      })
      .then(() => {
        // ativa o toggle
        const sidebar = document.getElementById('sidebar');
        const toggle  = document.getElementById('sidebarToggle');
        const icon    = toggle.querySelector('.material-symbols-outlined');
        toggle.addEventListener('click', () => {
          const collapsed = sidebar.classList.toggle('collapsed');
          icon.textContent = collapsed ? 'menu' : 'menu_open';
        });
      });