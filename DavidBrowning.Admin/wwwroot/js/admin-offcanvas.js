// Copyright © 2026 David Browning. All rights reserved.
// Source-available for viewing only. No license granted.

(() => {
   "use strict";

   const offcanvasSelector = "[data-admin-offcanvas]";
   const bodySelector = "[data-admin-offcanvas-body]";

   function resetOffcanvas(offcanvas) {
      const body = offcanvas.querySelector(bodySelector);

      if (!(body instanceof HTMLElement)) {
         console.error("Admin offcanvas is missing its body region.", offcanvas);

         return;
      }

      const placeholder = document.createElement("p");
      placeholder.className = "text-secondary mb-0";
      placeholder.textContent =
         offcanvas.dataset.adminOffcanvasPlaceholder ?? "Select an item to edit.";
      body.replaceChildren(placeholder);
   }

   function wireOffcanvas(offcanvas) {
      if (!(offcanvas instanceof HTMLElement)) {
         return;
      }

      if (offcanvas.dataset.adminOffcanvasWired === "true") {
         return;
      }

      offcanvas.dataset.adminOffcanvasWired = "true";
      offcanvas.addEventListener(
         "hidden.bs.offcanvas", () => resetOffcanvas(offcanvas));
   }

   function wireAllOffcanvases(root = document) {
      if (root instanceof Element && root.matches(offcanvasSelector)) {
         wireOffcanvas(root);
      }

      root.querySelectorAll(offcanvasSelector).forEach(wireOffcanvas);
   }

   function closeOffcanvas(offcanvasId) {
      if (typeof offcanvasId !== "string" ||
         offcanvasId.length === 0) {
         console.error("adminOffcanvasClose did not include an offcanvasId.");
         return;
      }

      const offcanvas = document.getElementById(offcanvasId);
      if (!(offcanvas instanceof HTMLElement)) {
         console.error(`Could not find admin offcanvas '${offcanvasId}'.`);

         return;
      }

      const instance = bootstrap.Offcanvas.getOrCreateInstance(offcanvas);
      instance.hide();
   }

   document.addEventListener("DOMContentLoaded", () => wireAllOffcanvases());
   document.body.addEventListener("htmx:afterSwap", event => wireAllOffcanvases(event.detail.target));
   document.body.addEventListener("adminOffcanvasClose", event => closeOffcanvas(event.detail.offcanvasId));
})();