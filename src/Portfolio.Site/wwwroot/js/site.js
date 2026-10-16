// Progressive enhancement: the project list works without JavaScript; this adds the filter chips.
(() => {
  const filters = document.querySelector("[data-filters]");
  const projects = document.querySelectorAll("[data-projects] > li");
  const empty = document.querySelector("[data-empty]");
  if (!filters || projects.length === 0) return;

  filters.hidden = false;

  filters.addEventListener("click", (event) => {
    const button = event.target.closest("[data-filter]");
    if (!button) return;

    const id = button.dataset.filter;
    filters.querySelectorAll("[data-filter]").forEach((b) => b.setAttribute("aria-pressed", String(b === button)));

    let shown = 0;
    projects.forEach((item) => {
      const match = id === "all" || item.dataset.categories.split(" ").includes(id);
      item.hidden = !match;
      if (match) shown++;
    });
    if (empty) empty.hidden = shown > 0;
  });
})();
