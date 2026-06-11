// Copyright © 2026 David Browning. All rights reserved.
// Source-available for viewing only. No license granted.

(() => {
   "use strict";

   const formSelector =
      "[data-credential-edit-form]";

   const monthSelector =
      "[data-awarded-month]";

   const yearSelector =
      "[data-awarded-year]";

   const displaySelector =
      "[data-awarded-date-display]";

   document.addEventListener(
      "DOMContentLoaded",
      () => initializeCredentialEditors(document));

   document.body.addEventListener(
      "htmx:afterSwap",
      event => initializeCredentialEditors(
         event.detail.target));

   document.addEventListener(
      "change",
      event => {
         if (event.target instanceof Element &&
            event.target.matches(monthSelector)) {
            updateDisplayText(event.target);
         }
      });

   document.addEventListener(
      "input",
      event => {
         if (!(event.target instanceof Element)) {
            return;
         }

         if (event.target.matches(yearSelector)) {
            updateDisplayText(event.target);
            return;
         }

         if (event.target.matches(displaySelector)) {
            event.target.dataset.manuallyEdited =
               "true";
         }
      });

   document.addEventListener(
      "reset",
      event => {
         if (!(event.target instanceof HTMLFormElement) ||
            !event.target.matches(formSelector)) {
            return;
         }

         window.setTimeout(
            () => initializeCredentialEditor(
               event.target),
            0);
      });

   function initializeCredentialEditors(root) {
      if (root instanceof Element &&
         root.matches(formSelector)) {
         initializeCredentialEditor(root);
      }

      root.querySelectorAll(formSelector)
         .forEach(initializeCredentialEditor);
   }

   function initializeCredentialEditor(form) {
      if (!(form instanceof HTMLFormElement)) {
         return;
      }

      const monthInput =
         form.querySelector(monthSelector);

      const yearInput =
         form.querySelector(yearSelector);

      const displayInput =
         form.querySelector(displaySelector);

      if (!(monthInput instanceof HTMLSelectElement) ||
         !(yearInput instanceof HTMLInputElement) ||
         !(displayInput instanceof HTMLInputElement)) {
         return;
      }

      const currentText =
         displayInput.value.trim();

      const generatedText =
         buildDisplayText(
            monthInput,
            yearInput);

      displayInput.dataset.manuallyEdited =
         currentText.length > 0 &&
            currentText !== generatedText
            ? "true"
            : "false";
   }

   function updateDisplayText(changedInput) {
      const form =
         changedInput.closest(formSelector);

      if (!(form instanceof HTMLFormElement)) {
         return;
      }

      const monthInput =
         form.querySelector(monthSelector);

      const yearInput =
         form.querySelector(yearSelector);

      const displayInput =
         form.querySelector(displaySelector);

      if (!(monthInput instanceof HTMLSelectElement) ||
         !(yearInput instanceof HTMLInputElement) ||
         !(displayInput instanceof HTMLInputElement)) {
         return;
      }

      if (displayInput.dataset.manuallyEdited === "true") {
         return;
      }

      displayInput.value =
         buildDisplayText(
            monthInput,
            yearInput);
   }

   function buildDisplayText(
      monthInput,
      yearInput) {
      const selectedMonth =
         monthInput.options[
         monthInput.selectedIndex];

      const month =
         selectedMonth?.value
            ? selectedMonth.text
            : "";

      const year =
         yearInput.value.trim();

      return [month, year]
         .filter(value => value.length > 0)
         .join(" ");
   }
})();