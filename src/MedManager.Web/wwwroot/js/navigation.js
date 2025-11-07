// Active link highlighting for sidebar
document.addEventListener("DOMContentLoaded", function () {
  const currentPath = window.location.pathname;
  const sidebarLinks = document.querySelectorAll(".sidebar-link");

  sidebarLinks.forEach((link) => {
    const linkPath = new URL(link.href).pathname;
    if (currentPath === linkPath) {
      link.classList.add("active");
    }
  });
});
