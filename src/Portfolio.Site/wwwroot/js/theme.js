// Light / dark theme. Loaded in <head> (not deferred) so the right theme is applied before first paint.
// Default follows the visitor's system setting; a click on the toggle saves an explicit choice.
(() => {
  const root = document.documentElement;
  const media = window.matchMedia("(prefers-color-scheme: dark)");
  const KEY = "theme";

  const stored = () => {
    try { return localStorage.getItem(KEY); } catch { return null; }
  };
  const save = (value) => {
    try { localStorage.setItem(KEY, value); } catch { /* private mode: the choice lasts for this page only */ }
  };
  const resolve = () => stored() ?? (media.matches ? "dark" : "light");

  const apply = (theme) => {
    root.dataset.theme = theme;
    const button = document.querySelector("[data-theme-toggle]");
    if (button) {
      const next = theme === "dark" ? "light" : "dark";
      button.setAttribute("aria-label", `Switch to ${next} mode`);
      button.setAttribute("title", `Switch to ${next} mode`);
    }
  };

  apply(resolve());

  // Follow system changes until the visitor picks a theme themselves.
  media.addEventListener("change", () => {
    if (!stored()) apply(resolve());
  });

  document.addEventListener("DOMContentLoaded", () => {
    const button = document.querySelector("[data-theme-toggle]");
    if (!button) return;
    button.hidden = false;
    apply(root.dataset.theme);
    button.addEventListener("click", () => {
      const next = root.dataset.theme === "dark" ? "light" : "dark";
      save(next);
      apply(next);
    });
  });
})();
