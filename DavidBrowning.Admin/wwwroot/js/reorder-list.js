// Copyright © 2026 David Browning. All rights reserved.
//
// Source-available for viewing only. No license granted.

(() => {
   "use strict";

   const reorderListSelector = "[data-reorder-list]";
   const deleteFormSelector = "[data-delete-form]";

   function wireReorderList(reorderList) {
      if (!(reorderList instanceof HTMLElement)) {
         return;
      }

      if (reorderList.dataset.reorderListWired === "true") {
         return;
      }

      const form = reorderList.querySelector("[data-reorder-form]");
      const itemsContainer = reorderList.querySelector("[data-reorder-items]");
      const saveButton = reorderList.querySelector("[data-reorder-save]");

      if (!(form instanceof HTMLFormElement)) {
         return;
      }

      if (!(itemsContainer instanceof HTMLElement) ||
         !(saveButton instanceof HTMLButtonElement)) {
         console.error("Malformed reorder list.", reorderList);
         return;
      }

      reorderList.dataset.reorderListWired = "true";

      function updateSortOrder() {
         const items = itemsContainer.querySelectorAll("[data-reorder-item]");
         items.forEach((item, index) => {
            const input = item.querySelector("[data-sort-order-input]");
            if (input instanceof HTMLInputElement) {
               input.value = index.toString();
            }
         });

         saveButton.disabled = false;
      }

      reorderList.addEventListener("click", event => {
         const button = event.target.closest("[data-reorder-direction]");
         if (!(button instanceof HTMLButtonElement)) {
            return;
         }

         const item = button.closest("[data-reorder-item]");
         if (!(item instanceof HTMLElement)) {
            return;
         }

         const direction = button.dataset.reorderDirection;
         if (direction === "up") {
            const previous = item.previousElementSibling;
            if (previous !== null) {
               itemsContainer.insertBefore(item, previous);
               updateSortOrder();
            }
         }
         else if (direction === "down") {
            const next = item.nextElementSibling;
            if (next !== null) {
               itemsContainer.insertBefore(next, item);
               updateSortOrder();
            }
         }
      });
   }

   function wireDeleteForm(form) {
      if (!(form instanceof HTMLFormElement)) {
         return;
      }

      if (form.dataset.deleteFormWired === "true") {
         return;
      }

      form.dataset.deleteFormWired = "true";
      form.addEventListener("submit", event => {
         const displayName = form.dataset.deleteName ?? "this item";
         if (!window.confirm(`Delete ${displayName}?`)) {
            event.preventDefault();
         }
      });
   }

   function wireAll(root = document) {
      if (root instanceof Element && root.matches(reorderListSelector)) {
         wireReorderList(root);
      }

      root.querySelectorAll(reorderListSelector).forEach(wireReorderList);
      if (root instanceof Element && root.matches(deleteFormSelector)) {
         wireDeleteForm(root);
      }

      root.querySelectorAll(deleteFormSelector).forEach(wireDeleteForm);
   }

   document.addEventListener("DOMContentLoaded", () => wireAll());
   document.body.addEventListener(
      "htmx:afterSwap", event => wireAll(event.detail.target));
})();