document.getElementById("userCount").textContent = 12;
document.getElementById("tokenCount").textContent = 5;
document.getElementById("fileCount").textContent = 8;

const toggleBtn = document.getElementById("toggleSidebarBtn");
const sidebar = document.getElementById("sidebar");
const body = document.body;

toggleBtn.addEventListener("click", () => {
    sidebar.classList.toggle("sidebar-visible");
    body.classList.toggle("sidebar-open");
});