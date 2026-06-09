(() => {
   "use strict";

   document.body.addEventListener("interestUpdated", () => {
      const sidebar = document.getElementById("interest-edit-sidebar");

      if (sidebar === null) {
         return;
      }

      const offcanvas = bootstrap.Offcanvas.getOrCreateInstance(sidebar);
      offcanvas.hide();
   });
})();

(() => {
   "use strict";

   const sidebar = document.getElementById("interest-edit-sidebar");

   if (sidebar === null) {
      return;
   }

   sidebar.addEventListener("hidden.bs.offcanvas", () => {
      const body = document.getElementById("interest-edit-sidebar-body");

      if (body !== null) {
         body.innerHTML = `
            <p class="text-secondary">
               Select an interest to edit.
            </p>`;
      }
   });

   document.body.addEventListener("interestUpdated", () => {
      const offcanvas = bootstrap.Offcanvas.getOrCreateInstance(sidebar);
      offcanvas.hide();
   });
})();