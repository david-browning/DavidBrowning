// Copyright © 2026 David Browning. All rights reserved.
// Source-available for viewing only. No license granted.

(() => {
   "use strict";

   const rootSelector = "[data-post-tags]";
   const selectSelector = "[data-post-tag-select]";
   const addSelector = "[data-post-tag-add]";
   const listSelector = "[data-post-tag-list]";
   const removeSelector = "[data-post-tag-remove]";
   const itemSelector = "[data-post-tag-id]";

   document.addEventListener("DOMContentLoaded", () => {
      initializePostTagEditors(document);
   });

   document.body.addEventListener("htmx:afterSwap", event => {
      initializePostTagEditors(event.detail.target);
   });

   document.addEventListener("click", event => {
      if (!(event.target instanceof Element)) {
         return;
      }

      const addButton = event.target.closest(addSelector);
      if (addButton instanceof HTMLButtonElement) {
         addSelectedTag(addButton);
         return;
      }

      const removeButton = event.target.closest(removeSelector);
      if (removeButton instanceof HTMLButtonElement) {
         removeSelectedTag(removeButton);
      }
   });

   function initializePostTagEditors(root) {
      if (root instanceof Element && root.matches(rootSelector)) {
         initializePostTagEditor(root);
      }

      root.querySelectorAll?.(rootSelector).forEach(initializePostTagEditor);
   }

   function initializePostTagEditor(root) {
      refreshSelectedOptions(root);
      refreshEmptyState(root);
   }

   function addSelectedTag(addButton) {
      const root = addButton.closest(rootSelector);

      if (!(root instanceof HTMLElement)) {
         return;
      }

      const select = root.querySelector(selectSelector);
      const list = root.querySelector(listSelector);

      if (!(select instanceof HTMLSelectElement) ||
         !(list instanceof HTMLUListElement)) {
         return;
      }

      const selectedOption = select.selectedOptions[0];

      if (!(selectedOption instanceof HTMLOptionElement) ||
         selectedOption.value.length === 0) {
         return;
      }

      if (hasSelectedTag(root, selectedOption.value)) {
         select.value = "";
         refreshSelectedOptions(root);
         return;
      }

      const displayName =
         selectedOption.dataset.displayName ?? selectedOption.textContent ?? "";

      const slug = selectedOption.dataset.slug ?? "";

      list.appendChild(createTagListItem(
         selectedOption.value,
         displayName,
         slug));

      select.value = "";
      refreshSelectedOptions(root);
      refreshEmptyState(root);
   }

   function removeSelectedTag(removeButton) {
      const root = removeButton.closest(rootSelector);
      const item = removeButton.closest(itemSelector);

      if (!(root instanceof HTMLElement) ||
         !(item instanceof HTMLElement)) {
         return;
      }

      item.remove();

      refreshSelectedOptions(root);
      refreshEmptyState(root);
   }

   function createTagListItem(tagId, displayName, slug) {
      const item = document.createElement("li");
      item.className =
         "list-group-item d-flex justify-content-between align-items-center";
      item.dataset.postTagId = tagId;

      const textWrapper = document.createElement("div");

      const nameElement = document.createElement("div");
      nameElement.className = "fw-semibold";
      nameElement.textContent = displayName;

      const slugElement = document.createElement("div");
      slugElement.className = "small text-secondary";
      slugElement.textContent = slug;

      textWrapper.append(nameElement, slugElement);

      const hiddenInput = document.createElement("input");
      hiddenInput.type = "hidden";
      hiddenInput.name = "WritingTagIds";
      hiddenInput.value = tagId;

      const removeButton = document.createElement("button");
      removeButton.className = "btn btn-sm btn-outline-danger";
      removeButton.type = "button";
      removeButton.dataset.postTagRemove = "";
      removeButton.setAttribute("aria-label", `Remove ${displayName}`);
      removeButton.innerHTML =
         '<i class="fa-solid fa-xmark" aria-hidden="true"></i>';

      item.append(textWrapper, hiddenInput, removeButton);

      return item;
   }

   function hasSelectedTag(root, tagId) {
      return root.querySelector(
         `[data-post-tag-id="${CSS.escape(tagId)}"]`) !== null;
   }

   function refreshSelectedOptions(root) {
      const select = root.querySelector(selectSelector);

      if (!(select instanceof HTMLSelectElement)) {
         return;
      }

      const selectedIds = new Set(
         [...root.querySelectorAll(itemSelector)]
            .map(item => item.dataset.postTagId)
            .filter(id => id !== undefined));

      for (const option of select.options) {
         if (option.value.length === 0) {
            option.disabled = false;
            continue;
         }

         option.disabled = selectedIds.has(option.value);
      }
   }

   function refreshEmptyState(root) {
      const list = root.querySelector(listSelector);

      if (!(list instanceof HTMLUListElement)) {
         return;
      }

      const hasItems = list.querySelector(itemSelector) !== null;

      if (hasItems) {
         removeEmptyState(list);
         return;
      }

      addEmptyState(list);
   }

   function addEmptyState(list) {
      if (list.querySelector("[data-post-tag-empty]") !== null) {
         return;
      }

      const item = document.createElement("li");
      item.className = "list-group-item text-secondary";
      item.dataset.postTagEmpty = "";
      item.textContent = "No tags selected.";

      list.appendChild(item);
   }

   function removeEmptyState(list) {
      list.querySelector("[data-post-tag-empty]")?.remove();
   }
})();