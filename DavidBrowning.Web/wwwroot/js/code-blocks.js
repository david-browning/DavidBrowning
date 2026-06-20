// Copyright © 2026 David Browning. All rights reserved.
// Source-available for viewing only. No license granted.

(() => {
   "use strict";

   document.addEventListener("DOMContentLoaded", () => {
      enhancePrismCopyButtons(document);
   });

   document.body.addEventListener("htmx:afterSwap", event => {
      enhancePrismCopyButtons(event.target);
   });

   function enhancePrismCopyButtons(root) {
      const buttons = root.querySelectorAll("button.copy-to-clipboard-button");

      buttons.forEach(button => {
         if (button.dataset.wbCodeCopyEnhanced === "true") {
            updateCopyIcon(button);
            return;
         }

         button.dataset.wbCodeCopyEnhanced = "true";
         button.setAttribute("aria-label", "Copy code");

         const label = button.querySelector("span");
         if (label !== null) {
            label.classList.add("wb-code-copy-label");
         }

         const icon = document.createElement("i");
         icon.className = "fa-solid fa-copy wb-code-copy-icon";
         icon.setAttribute("aria-hidden", "true");
         button.prepend(icon);

         observeCopyState(button);
         updateCopyIcon(button);
      });

      if (window.FontAwesome?.dom?.i2svg) {
         window.FontAwesome.dom.i2svg();
      }
   }

   function observeCopyState(button) {
      const observer = new MutationObserver(() => {
         updateCopyIcon(button);
      });

      observer.observe(button, {
         attributes: true,
         attributeFilter: ["data-copy-state"],
      });
   }

   function updateCopyIcon(button) {
      const icon = button.querySelector(".wb-code-copy-icon");
      if (icon === null) {
         return;
      }

      icon.classList.remove(
         "fa-copy",
         "fa-check",
         "fa-triangle-exclamation");

      const state = button.getAttribute("data-copy-state");

      if (state === "copy-success") {
         icon.classList.add("fa-check");
         button.setAttribute("aria-label", "Copied");
      }
      else if (state === "copy-error") {
         icon.classList.add("fa-triangle-exclamation");
         button.setAttribute("aria-label", "Copy failed");
      }
      else {
         icon.classList.add("fa-copy");
         button.setAttribute("aria-label", "Copy code");
      }
   }
})();