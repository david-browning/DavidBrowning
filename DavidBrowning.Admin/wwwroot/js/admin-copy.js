// Copyright © 2026 David Browning. All rights reserved.
// Source-available for viewing only. No license granted.
(() => {
   "use strict";

   const copyButtonSelector = "[data-admin-copy-target]";
   const copyLabelSelector = "[data-admin-copy-label]";

   document.addEventListener("click", async event => {
      const button = event.target.closest(copyButtonSelector);

      if (!(button instanceof HTMLButtonElement)) {
         return;
      }

      const targetSelector = button.dataset.adminCopyTarget;

      if (!targetSelector) {
         return;
      }

      const target = document.querySelector(targetSelector);

      if (!(target instanceof HTMLElement)) {
         return;
      }

      const text = target.textContent ?? "";

      if (!text.trim()) {
         return;
      }

      try {
         await copyTextAsync(text);
         showCopyResult(button, "Copied");
      }
      catch {
         showCopyResult(button, "Failed");
      }
   });

   async function copyTextAsync(text) {
      if (navigator.clipboard && window.isSecureContext) {
         await navigator.clipboard.writeText(text);
         return;
      }

      copyTextFallback(text);
   }

   function copyTextFallback(text) {
      const textArea = document.createElement("textarea");
      textArea.value = text;
      textArea.setAttribute("readonly", "");
      textArea.style.position = "fixed";
      textArea.style.top = "-1000px";
      textArea.style.left = "-1000px";

      document.body.appendChild(textArea);
      textArea.select();

      try {
         document.execCommand("copy");
      }
      finally {
         textArea.remove();
      }
   }

   function showCopyResult(button, resultText) {
      const label = button.querySelector(copyLabelSelector);
      if (!(label instanceof HTMLElement)) {
         return;
      }

      const originalText = label.dataset.originalText ?? label.textContent ?? "Copy";
      label.dataset.originalText = originalText;
      label.textContent = resultText;

      window.setTimeout(() => {
         label.textContent = originalText;
      }, 1500);
   }
})();