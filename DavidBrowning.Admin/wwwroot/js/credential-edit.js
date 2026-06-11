(() => {
   "use strict";

   document.querySelectorAll("[data-credential-edit-form]")
      .forEach(initializeCredentialDateEditor);

   function initializeCredentialDateEditor(form) {
      const monthInput = form.querySelector("[data-awarded-month]");

      const yearInput = form.querySelector("[data-awarded-year]");

      const displayInput = form.querySelector("[data-awarded-date-display]");

      if (!monthInput || !yearInput || !displayInput) {
         return;
      }

      let isDisplayTextManuallyEdited =
         !isInitiallyAutoManaged(monthInput, yearInput, displayInput);

      monthInput.addEventListener("change",
         updateDisplayText);

      yearInput.addEventListener("input",
         updateDisplayText);

      displayInput.addEventListener("input",
         () => {
            isDisplayTextManuallyEdited = true;
         });

      form.addEventListener("reset",
         () => {
            /*
             * The browser restores the initial input values after the
             * reset event fires. Wait until that has happened before
             * recalculating the state.
             */
            window.setTimeout(
               () => {
                  isDisplayTextManuallyEdited =
                     !isInitiallyAutoManaged(monthInput, yearInput, displayInput);
               },
               0);
         });

      function updateDisplayText() {
         if (isDisplayTextManuallyEdited) {
            return;
         }

         displayInput.value = buildDisplayText(monthInput, yearInput);
      }
   }

   function isInitiallyAutoManaged(monthInput, yearInput, displayInput) {
      const currentDisplayText = displayInput.value.trim();

      if (currentDisplayText.length === 0) {
         return true;
      }

      const generatedDisplayText = buildDisplayText(monthInput, yearInput);

      /*
       * Existing edit forms may already contain a generated display
       * value such as "June 2026". Continue auto-managing that value.
       *
       * Preserve anything custom, such as:
       *    "Expected June 2026"
       *    "Spring 2026"
       *    "In progress"
       */
      return currentDisplayText === generatedDisplayText;
   }

   function buildDisplayText(monthInput, yearInput) {
      const selectedMonth = monthInput.options[monthInput.selectedIndex];
      const month = selectedMonth?.value ? selectedMonth.text : "";
      const year = yearInput.value.trim();
      return [month, year].filter(value => value.length > 0).join(" ");
   }
})();