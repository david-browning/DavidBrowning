(() => {
	"use strict";

	const reorderLists = document.querySelectorAll("[data-reorder-list]");

	reorderLists.forEach(reorderList => {
		const form =
			reorderList.querySelector("[data-reorder-form]");

		const itemsContainer =
			reorderList.querySelector("[data-reorder-items]");

		const saveButton =
			reorderList.querySelector("[data-reorder-save]");

		if (!form || !itemsContainer || !saveButton) {
			return;
		}

		const getItems = () =>
			Array.from(
				itemsContainer.querySelectorAll("[data-reorder-item]"));

		const updateIndexes = () => {
			const items = getItems();

			items.forEach((item, index) => {
				const idInput =
					item.querySelector("[data-reorder-id]");

				const sortOrderInput =
					item.querySelector("[data-reorder-sort-order]");

				const moveUpButton =
					item.querySelector("[data-reorder-up]");

				const moveDownButton =
					item.querySelector("[data-reorder-down]");

				if (idInput) {
					idInput.name = `Items[${index}].Id`;
				}

				if (sortOrderInput) {
					sortOrderInput.name =
						`Items[${index}].SortOrder`;

					sortOrderInput.value =
						index.toString();
				}

				if (moveUpButton) {
					moveUpButton.disabled =
						index === 0;
				}

				if (moveDownButton) {
					moveDownButton.disabled =
						index === items.length - 1;
				}
			});
		};

		const markDirty = () => {
			saveButton.disabled = false;
		};

		saveButton.addEventListener("click", () => {
			form.requestSubmit();
		});

		itemsContainer.addEventListener("click", event => {
			const moveUpButton =
				event.target.closest("[data-reorder-up]");

			const moveDownButton =
				event.target.closest("[data-reorder-down]");

			if (!moveUpButton && !moveDownButton) {
				return;
			}

			const item =
				event.target.closest("[data-reorder-item]");

			if (!item) {
				return;
			}

			if (moveUpButton && item.previousElementSibling) {
				itemsContainer.insertBefore(
					item,
					item.previousElementSibling);
			}

			if (moveDownButton && item.nextElementSibling) {
				itemsContainer.insertBefore(
					item.nextElementSibling,
					item);
			}

			updateIndexes();
			markDirty();
		});

		updateIndexes();
	});

	document
		.querySelectorAll("[data-delete-form]")
		.forEach(form => {
			form.addEventListener("submit", event => {
				const displayName =
					form.dataset.deleteName ?? "this item";

				const confirmed = window.confirm(
					`Delete "${displayName}"? ` +
					"This cannot be undone.");

				if (!confirmed) {
					event.preventDefault();
				}
			});
		});
})();