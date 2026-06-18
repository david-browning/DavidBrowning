// Copyright © 2026 David Browning. All rights reserved.
// Source-available for viewing only. No license granted.

(() => {
   "use strict";

   const editorSelector = "[data-project-links-editor]";
   const itemSelector = "[data-project-link-item]";

   document.addEventListener("click", event => {
      const addButton = event.target.closest("[data-project-link-add]");
      if (addButton !== null) {
         const editor = addButton.closest(editorSelector);
         if (editor !== null) {
            addProjectLink(editor);
         }

         return;
      }

      const removeButton = event.target.closest("[data-project-link-remove]");
      if (removeButton !== null) {
         const editor = removeButton.closest(editorSelector);
         const item = removeButton.closest(itemSelector);

         if (editor !== null && item !== null) {
            item.remove();
            renumberProjectLinks(editor);
            updateEmptyState(editor);
         }
      }
   });

   function addProjectLink(editor) {
      const typeInput = editor.querySelector("[data-project-link-type-input]");
      const labelInput = editor.querySelector("[data-project-link-label-input]");
      const urlInput = editor.querySelector("[data-project-link-url-input]");

      if (!(typeInput instanceof HTMLSelectElement) ||
         !(labelInput instanceof HTMLInputElement) ||
         !(urlInput instanceof HTMLInputElement)) {
         showError(editor, "The project link editor is malformed.");
         return;
      }

      const typeId = typeInput.value.trim();
      const label = labelInput.value.trim();
      const url = urlInput.value.trim();

      if (!typeId) {
         showError(editor, "Select a link type.");
         return;
      }

      if (!label) {
         showError(editor, "Enter a link label.");
         return;
      }

      if (!isHttpUrl(url)) {
         showError(editor, "Enter a valid HTTP or HTTPS URL.");
         return;
      }

      const selectedOption = typeInput.selectedOptions[0];
      const typeName = selectedOption?.textContent?.trim() ?? "Unknown";
      const list = editor.querySelector("[data-project-link-list]");

      if (!(list instanceof HTMLElement)) {
         showError(editor, "The project link list is missing.");
         return;
      }

      const index = list.querySelectorAll(itemSelector).length;

      list.appendChild(createProjectLinkItem(
         index,
         typeId,
         typeName,
         label,
         url));

      typeInput.value = "";
      labelInput.value = "";
      urlInput.value = "";

      clearError(editor);
      renumberProjectLinks(editor);
      updateEmptyState(editor);
   }

   function createProjectLinkItem(
      index,
      typeId,
      typeName,
      label,
      url) {
      const item = document.createElement("div");
      item.className = "list-group-item";
      item.dataset.projectLinkItem = "";

      item.appendChild(createHiddenInput(index, "ProjectLinkTypeId", typeId));
      item.appendChild(createHiddenInput(index, "Label", label));
      item.appendChild(createHiddenInput(index, "Url", url));
      item.appendChild(createHiddenInput(index, "SortOrder", index.toString()));

      const layout = document.createElement("div");
      layout.className = "d-flex flex-column flex-md-row justify-content-between gap-3";

      const body = document.createElement("div");
      body.className = "min-width-0";

      const header = document.createElement("div");
      header.className = "d-flex flex-wrap align-items-center gap-2";

      const labelElement = document.createElement("strong");
      labelElement.dataset.projectLinkDisplayLabel = "";
      labelElement.textContent = label;

      const typeElement = document.createElement("span");
      typeElement.className = "badge text-bg-secondary";
      typeElement.dataset.projectLinkDisplayType = "";
      typeElement.textContent = typeName;

      header.appendChild(labelElement);
      header.appendChild(typeElement);

      const urlWrapper = document.createElement("div");
      urlWrapper.className = "small text-break mt-1";

      const urlElement = document.createElement("a");
      urlElement.dataset.projectLinkDisplayUrl = "";
      urlElement.href = url;
      urlElement.target = "_blank";
      urlElement.rel = "noopener noreferrer";
      urlElement.textContent = url;

      urlWrapper.appendChild(urlElement);

      body.appendChild(header);
      body.appendChild(urlWrapper);

      const actions = document.createElement("div");
      actions.className = "flex-shrink-0";

      const removeButton = document.createElement("button");
      removeButton.className = "btn btn-sm btn-outline-danger";
      removeButton.type = "button";
      removeButton.dataset.projectLinkRemove = "";
      removeButton.textContent = "Remove";

      actions.appendChild(removeButton);

      layout.appendChild(body);
      layout.appendChild(actions);

      item.appendChild(layout);

      return item;
   }

   function createHiddenInput(index, fieldName, value) {
      const input = document.createElement("input");
      input.type = "hidden";
      input.name = `Links[${index}].${fieldName}`;
      input.value = value;
      input.dataset.projectLinkField = fieldName;

      return input;
   }

   function renumberProjectLinks(editor) {
      const items = editor.querySelectorAll(itemSelector);

      items.forEach((item, index) => {
         const inputs = item.querySelectorAll("[data-project-link-field]");

         inputs.forEach(input => {
            if (!(input instanceof HTMLInputElement)) {
               return;
            }

            const fieldName = input.dataset.projectLinkField;
            if (!fieldName) {
               return;
            }

            input.name = `Links[${index}].${fieldName}`;

            if (fieldName === "SortOrder") {
               input.value = index.toString();
            }
         });
      });
   }

   function updateEmptyState(editor) {
      const emptyMessage = editor.querySelector("[data-project-link-empty]");
      const itemCount = editor.querySelectorAll(itemSelector).length;

      if (emptyMessage instanceof HTMLElement) {
         emptyMessage.classList.toggle("d-none", itemCount !== 0);
      }
   }

   function showError(editor, message) {
      const error = editor.querySelector("[data-project-link-error]");

      if (error instanceof HTMLElement) {
         error.textContent = message;
         error.classList.remove("d-none");
      }
   }

   function clearError(editor) {
      const error = editor.querySelector("[data-project-link-error]");

      if (error instanceof HTMLElement) {
         error.textContent = "";
         error.classList.add("d-none");
      }
   }

   function isHttpUrl(value) {
      try {
         const url = new URL(value);
         return url.protocol === "http:" || url.protocol === "https:";
      }
      catch {
         return false;
      }
   }
})();