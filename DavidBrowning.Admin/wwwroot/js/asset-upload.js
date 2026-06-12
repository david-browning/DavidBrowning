// Copyright © 2026 David Browning. All rights reserved.
// Source-available for viewing only. No license granted.

(() => {
   "use strict";

   function formatBytes(sizeBytes) {
      if (sizeBytes < 1024) {
         return `${sizeBytes} bytes`;
      }

      const sizeKilobytes = sizeBytes / 1024;

      if (sizeKilobytes < 1024) {
         return `${sizeKilobytes.toFixed(1)} KB`;
      }

      return `${(sizeKilobytes / 1024).toFixed(1)} MB`;
   }

   function setValue(root, selector, value) {
      const element = root.querySelector(selector);

      if (element instanceof HTMLInputElement ||
         element instanceof HTMLSelectElement) {
         element.value = value ?? "";
      }
   }

   function populateImageDimensions(root, file) {
      setValue(root, "[data-image-width-preview]", "");
      setValue(root, "[data-image-height-preview]", "");

      const preview = root.querySelector("[data-image-preview]");

      if (!(preview instanceof HTMLImageElement)) {
         return;
      }

      preview.classList.add("d-none");
      preview.removeAttribute("src");

      if (!file.type.startsWith("image/")) {
         return;
      }

      const objectUrl = URL.createObjectURL(file);
      const image = new Image();

      image.addEventListener("load", () => {
         setValue(
            root, "[data-image-width-preview]", image.naturalWidth.toString());

         setValue(
            root, "[data-image-height-preview]", image.naturalHeight.toString());

         preview.src = objectUrl;
         preview.classList.remove("d-none");
      });

      image.addEventListener("error", () => {
         URL.revokeObjectURL(objectUrl);
      });

      preview.addEventListener(
         "load",
         () => URL.revokeObjectURL(objectUrl),
         {
            once: true,
         });

      image.src = objectUrl;
   }

   function populateSelectedFile(root, file, replaceKey) {
      const summary = root.querySelector("[data-asset-file-summary]");

      if (summary instanceof HTMLElement) {
         summary.textContent = `${file.name} (${formatBytes(file.size)})`;
      }

      setValue(root, "[data-original-file-name-preview]", file.name);
      setValue(root, "[data-file-type]", file.type);
      setValue(root, "[data-file-size-preview]", file.size.toString());
      const contentTypeInput = root.querySelector("[name='ContentType']");

      if (contentTypeInput instanceof HTMLSelectElement &&
         file.type.length > 0) {
         const supportedType = Array
            .from(contentTypeInput.options)
            .some(option => option.value === file.type);

         if (supportedType) {
            contentTypeInput.value = file.type;
         }
      }

      if (replaceKey) {
         const assetKeyInput = root.querySelector("[data-asset-key-input]");
         if (assetKeyInput instanceof HTMLInputElement) {
            //&& assetKeyInput.value.trim().length === 0) {
            const suggestedKey = suggestAssetKey(file);
            assetKeyInput.value = suggestedKey;
         }
      }

      populateImageDimensions(root, file);
   }

   function suggestAssetKey(file) {
      const directory = getAssetDirectory(file.type);
      const fileName = normalizeAssetFileName(file.name);
      return `${directory}/${fileName}`;
   }

   function getAssetDirectory(contentType) {
      if (contentType.startsWith("image/")) {
         return "images";
      }

      if (contentType.startsWith("audio/")) {
         return "audio";
      }

      if (contentType.startsWith("video/")) {
         return "videos";
      }

      if (contentType === "application/pdf" ||
         contentType.startsWith("text/")) {
         return "documents";
      }

      return "assets";
   }

   function normalizeAssetFileName(fileName) {
      return fileName
         .normalize("NFKD")
         .replace(/[\u0300-\u036f]/g, "")
         .trim()
         .toLowerCase()
         .replace(/\s+/g, "-")
         .replace(/[^a-z0-9._-]/g, "")
         .replace(/-+/g, "-")
         .replace(/^[._-]+|[._-]+$/g, "");
   }

   document.addEventListener("change", event => {
      const fileInput = event.target.closest("[data-asset-file-input]");

      if (!(fileInput instanceof HTMLInputElement)) {
         return;
      }

      const root = fileInput.form;
      if (!(root instanceof HTMLFormElement)) {
         console.error(
            "Asset file input is not associated with a form.", fileInput);

         return;
      }

      const file = fileInput.files?.[0];
      if (file === undefined) {
         return;
      }

      const replaceKey = root.dataset.assetEditMode === "false";
      populateSelectedFile(root, file, replaceKey);
   });
})();