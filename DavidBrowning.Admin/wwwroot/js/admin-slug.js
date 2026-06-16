// Copyright © 2026 David Browning. All rights reserved.
// Source-available for viewing only. No license granted.

(() => {
   "use strict";

   const editorSelector = "[data-slug-editor]";
   const sourceSelector = "[data-slug-source]";
   const targetSelector = "[data-slug-target]";

   document.addEventListener("DOMContentLoaded", () => {
      initializeSlugEditors(document);
   });

   document.body.addEventListener("htmx:afterSwap", event => {
      initializeSlugEditors(event.detail.target);
   });

   document.addEventListener("input", event => {
      if (!(event.target instanceof Element)) {
         return;
      }

      if (event.target.matches(sourceSelector)) {
         updateSuggestedSlug(event.target);
         return;
      }

      if (event.target.matches(targetSelector)) {
         event.target.dataset.manuallyEdited = "true";
      }
   });

   document.addEventListener("reset", event => {
      if (!(event.target instanceof HTMLFormElement) ||
         !event.target.matches(editorSelector)) {
         return;
      }

      window.setTimeout(() => {
         initializeSlugEditor(event.target);
      }, 0);
   });

   window.suggestSlug = suggestSlug;

   function suggestSlug(value) {
      return value
         .normalize("NFKD")
         .replace(/[\u0300-\u036f]/g, "")
         .toLowerCase()
         .replace(/['’]/g, "")
         .replace(/&/g, " and ")
         .replace(/[^a-z0-9]+/g, "-")
         .replace(/^-+|-+$/g, "")
         .replace(/-{2,}/g, "-");
   }

   function initializeSlugEditors(root) {
      if (root instanceof Element && root.matches(editorSelector)) {
         initializeSlugEditor(root);
      }

      root.querySelectorAll?.(editorSelector).forEach(initializeSlugEditor);
   }

   function initializeSlugEditor(editor) {
      const source = editor.querySelector(sourceSelector);
      const target = editor.querySelector(targetSelector);

      if (!(source instanceof HTMLInputElement) ||
         !(target instanceof HTMLInputElement)) {
         return;
      }

      const currentSlug = target.value.trim();
      const generatedSlug = suggestSlug(source.value);

      target.dataset.manuallyEdited =
         currentSlug.length > 0 && currentSlug !== generatedSlug
            ? "true"
            : "false";
   }

   function updateSuggestedSlug(source) {
      const editor = source.closest(editorSelector);

      if (!(editor instanceof HTMLElement)) {
         return;
      }

      const target = editor.querySelector(targetSelector);

      if (!(target instanceof HTMLInputElement)) {
         return;
      }

      if (target.dataset.manuallyEdited === "true") {
         return;
      }

      target.value = suggestSlug(source.value);
   }
})();