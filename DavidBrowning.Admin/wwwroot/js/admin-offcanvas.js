// Copyright © 2026 David Browning. All rights reserved.
// Source-available for viewing only. No license granted.

(() => {
   "use strict";

   const offcanvasSelector = "[data-admin-offcanvas]";
   const bodySelector = "[data-admin-offcanvas-body]";
   const autofocusSelector = "[data-admin-autofocus]";
   const focusableSelector =
      "input:not([type='hidden']):not([disabled]), " +
      "select:not([disabled]), " +
      "textarea:not([disabled]), " +
      "button:not([disabled]), " +
      "a[href]";

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

   function scheduleFocusFirstField(offcanvas) {
      window.setTimeout(() => focusFirstField(offcanvas), 0);
   }

   function wireOffcanvas(offcanvas) {
      if (!(offcanvas instanceof HTMLElement)) {
         return;
      }

      if (offcanvas.dataset.adminOffcanvasWired === "true") {
         return;
      }

      offcanvas.dataset.adminOffcanvasWired = "true";

      offcanvas.addEventListener("shown.bs.offcanvas", () => {
         scheduleFocusFirstField(offcanvas);
      });

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

   function focusFirstField(offcanvas) {
      const body = offcanvas.querySelector(bodySelector);

      if (!(body instanceof HTMLElement)) {
         return;
      }

      const preferred = body.querySelector(autofocusSelector);
      const fallback = body.querySelector(focusableSelector);
      const target = preferred ?? fallback;

      if (!(target instanceof HTMLElement)) {
         return;
      }

      window.setTimeout(() => {
         target.focus({ preventScroll: true });

         if (target instanceof HTMLInputElement ||
            target instanceof HTMLTextAreaElement) {
            target.select();
         }
      }, 0);
   }

   document.addEventListener("DOMContentLoaded", () => wireAllOffcanvases());
   document.body.addEventListener("adminOffcanvasClose", event => closeOffcanvas(event.detail.offcanvasId));
   document.body.addEventListener("htmx:afterSwap", event => {
      wireAllOffcanvases(event.detail.target);

      const target = event.detail.target;
      if (!(target instanceof HTMLElement)) {
         return;
      }

      const offcanvas = target.closest(offcanvasSelector);
      if (!(offcanvas instanceof HTMLElement)) {
         return;
      }

      if (offcanvas.classList.contains("show")) {
         scheduleFocusFirstField(offcanvas);
      }
   });
})();