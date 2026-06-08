// Copyright © 2026 David Browning. All rights reserved.
// Source-available for viewing only. No license granted.

(() => {
   "use strict";

   function setSelectedIcon(picker, selectedIconCSS) {
      const input = picker.querySelector("[data-selected-icon-input]");
      const preview = picker.querySelector("[data-selected-icon-preview]");
      const options = picker.querySelectorAll("[data-icon-css]");

      input.value = selectedIconCSS;
      preview.className = selectedIconCSS;

      for (const option of options) {
         const isSelected = option.dataset.iconCss === selectedIconCSS;

         option.classList.toggle("selected", isSelected);
         option.setAttribute("aria-selected", isSelected.toString());
      }
   }

   document.addEventListener("click", event => {
      const option = event.target.closest("[data-icon-css]");

      if (option !== null) {
         const picker = option.closest("[data-fontawesome-icon-picker]");
         if (picker !== null) {
            setSelectedIcon(picker, option.dataset.iconCss);
         }

         return;
      }

      const clearButton = event.target.closest("[data-clear-icon]");
      if (clearButton !== null) {
         const picker = clearButton.closest("[data-fontawesome-icon-picker]");
         if (picker !== null) {
            setSelectedIcon(picker, "");
         }
      }
   });
})();