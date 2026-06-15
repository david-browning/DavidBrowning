// Copyright © 2026 David Browning. All rights reserved.
// Source-available for viewing only. No license granted.

(() => {
   "use strict";

   const chooserSelector = "[data-asset-chooser]";
   const chooserItemSelector = "[data-asset-chooser-item]";
   const emptySelector = "[data-asset-chooser-empty]";
   const itemSelector = "[data-asset-chooser-item]";
   const searchSelector = "[data-asset-chooser-search]";
   const insertButtonSelector = "[data-asset-chooser-insert]";
   const insertableSelector = "[data-asset-insertable]";
   const assetLinkInputsSelector = "[data-asset-link-inputs]";
   const assetLinkGroupSelector = "[data-asset-link-reference-key]";

   const assetTokenRegex =
      /^\{\{asset:([a-z0-9][a-z0-9-]*)\}\}\r?$/gim;

   let lastInsertable = null;

   document.addEventListener("focusin", event => {
      if (event.target instanceof HTMLElement &&
         event.target.matches(insertableSelector)) {
         lastInsertable = event.target;
      }
   });

   document.addEventListener("input", event => {
      if (!(event.target instanceof HTMLInputElement) ||
         !event.target.matches(searchSelector)) {
         return;
      }

      filterChooser(event.target);
   });

   document.addEventListener("click", event => {
      if (!(event.target instanceof Element)) {
         return;
      }

      const button = event.target.closest(insertButtonSelector);

      if (!(button instanceof HTMLButtonElement)) {
         return;
      }

      const item = button.closest(chooserItemSelector);

      if (item instanceof HTMLElement) {
         insertAsset(item);
      }
   });

   document.addEventListener("dblclick", event => {
      if (!(event.target instanceof Element)) {
         return;
      }

      const item = event.target.closest(chooserItemSelector);

      if (item instanceof HTMLElement) {
         insertAsset(item);
      }
   });

   document.addEventListener("submit", event => {
      if (!(event.target instanceof HTMLFormElement)) {
         return;
      }

      syncAssetLinks(event.target);
   });

   function insertAsset(item) {
      const target = getInsertTarget();

      if (!(target instanceof HTMLTextAreaElement) &&
         !(target instanceof HTMLInputElement)) {
         return;
      }

      const form = target.closest("form");

      if (!(form instanceof HTMLFormElement)) {
         return;
      }

      const insertText = item.dataset.assetInsertText;
      const referenceKey = item.dataset.assetReferenceKey;
      const siteAssetId = item.dataset.assetSiteAssetId;

      if (!insertText || !referenceKey || !siteAssetId) {
         return;
      }

      insertAssetTokenAtCursor(target, insertText);
      ensureAssetLinkInputs(form, siteAssetId, referenceKey);
   }

   function getInsertTarget() {
      if (document.activeElement instanceof HTMLElement &&
         document.activeElement.matches(insertableSelector)) {
         return document.activeElement;
      }

      if (lastInsertable instanceof HTMLElement &&
         document.contains(lastInsertable)) {
         return lastInsertable;
      }

      return document.querySelector(insertableSelector);
   }

   function insertAssetTokenAtCursor(target, token) {
      const start = target.selectionStart ?? target.value.length;
      const end = target.selectionEnd ?? target.value.length;

      const insertion = buildBlockInsertion(target.value, start, end, token);

      target.setRangeText(insertion, start, end, "end");
      target.focus();

      target.dispatchEvent(new Event("input", {
         bubbles: true,
      }));
   }

   function buildBlockInsertion(value, start, end, token) {
      const before = value.slice(0, start);
      const after = value.slice(end);

      const prefix = before.length === 0 || before.endsWith("\n\n")
         ? ""
         : before.endsWith("\n")
            ? "\n"
            : "\n\n";

      const suffix = after.length === 0 || after.startsWith("\n\n")
         ? ""
         : after.startsWith("\n")
            ? "\n"
            : "\n\n";

      return `${prefix}${token}${suffix}`;
   }

   function ensureAssetLinkInputs(form, siteAssetId, referenceKey) {
      const container = form.querySelector(assetLinkInputsSelector);

      if (!(container instanceof HTMLElement)) {
         return;
      }

      const existing = container.querySelector(
         `[data-asset-link-reference-key="${CSS.escape(referenceKey)}"]`);

      if (existing !== null) {
         return;
      }

      const group = document.createElement("div");
      group.dataset.assetLinkReferenceKey = referenceKey;

      group.append(
         createHiddenInput("AssetLinks.Index", referenceKey),
         createHiddenInput(
            `AssetLinks[${referenceKey}].SiteAssetId`,
            siteAssetId),
         createHiddenInput(
            `AssetLinks[${referenceKey}].ReferenceKey`,
            referenceKey));

      container.appendChild(group);
   }

   function createHiddenInput(name, value) {
      const input = document.createElement("input");
      input.type = "hidden";
      input.name = name;
      input.value = value;

      return input;
   }

   function syncAssetLinks(form) {
      const container = form.querySelector(assetLinkInputsSelector);

      if (!(container instanceof HTMLElement)) {
         return;
      }

      const usedReferenceKeys = getUsedReferenceKeys(form);

      for (const group of container.querySelectorAll(assetLinkGroupSelector)) {
         if (!(group instanceof HTMLElement)) {
            continue;
         }

         const referenceKey = group.dataset.assetLinkReferenceKey;

         if (!referenceKey || !usedReferenceKeys.has(referenceKey)) {
            group.remove();
         }
      }

      for (const referenceKey of usedReferenceKeys) {
         const existing = container.querySelector(
            `[data-asset-link-reference-key="${CSS.escape(referenceKey)}"]`);

         if (existing !== null) {
            continue;
         }

         const chooserItem = document.querySelector(
            `[data-asset-reference-key="${CSS.escape(referenceKey)}"]`);

         if (!(chooserItem instanceof HTMLElement)) {
            continue;
         }

         const siteAssetId = chooserItem.dataset.assetSiteAssetId;

         if (!siteAssetId) {
            continue;
         }

         ensureAssetLinkInputs(form, siteAssetId, referenceKey);
      }
   }

   function getUsedReferenceKeys(form) {
      const referenceKeys = new Set();

      for (const input of form.querySelectorAll(insertableSelector)) {
         if (!(input instanceof HTMLTextAreaElement) &&
            !(input instanceof HTMLInputElement)) {
            continue;
         }

         for (const match of input.value.matchAll(assetTokenRegex)) {
            referenceKeys.add(match[1].toLowerCase());
         }
      }

      return referenceKeys;
   }

   function filterChooser(searchInput) {
      const chooser = searchInput.closest(chooserSelector);

      if (!(chooser instanceof HTMLElement)) {
         return;
      }

      const query = searchInput.value.trim().toLowerCase();
      const items = chooser.querySelectorAll(itemSelector);

      let visibleCount = 0;

      for (const item of items) {
         if (!(item instanceof HTMLElement)) {
            continue;
         }

         const searchText =
            item.dataset.assetSearchText?.toLowerCase() ?? "";

         const isVisible =
            query.length === 0 || searchText.includes(query);

         item.classList.toggle("d-none", !isVisible);

         if (isVisible) {
            visibleCount++;
         }
      }

      const emptyMessage = chooser.querySelector(emptySelector);

      if (emptyMessage instanceof HTMLElement) {
         emptyMessage.classList.toggle("d-none", visibleCount > 0);
      }
   }
})();