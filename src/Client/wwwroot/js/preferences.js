// Apply preferences before styles and Blazor load to avoid a flash of the other theme.
(() => {
    const languageKey = "hts.language", themeKey = "hts.theme";
    const supported = ["en", "pt-BR", "es"];
    const readStorage = key => { try { return localStorage.getItem(key); } catch { return null; } };
    const save = (key, value) => { try { localStorage.setItem(key, value); } catch {} };
    const preferred = navigator.languages?.[0] || navigator.language || "en";
    const detected = preferred.toLowerCase().startsWith("pt") ? "pt-BR" : preferred.toLowerCase().startsWith("es") ? "es" : "en";
    let language = supported.includes(readStorage(languageKey)) ? readStorage(languageKey) : detected;
    let theme = ["dark", "light"].includes(readStorage(themeKey)) ? readStorage(themeKey) : matchMedia("(prefers-color-scheme: dark)").matches ? "dark" : "light";
    const bootCopy = {
        en: ["Loading Mark's Baseball Cards", "Getting the collection ready", "This page needs a fresh start.", "Reload page", "Dismiss error", "Explore Mark’s personal 1991 Topps baseball card collection and live marketplace."],
        "pt-BR": ["Carregando Mark's Baseball Cards", "Preparando a coleção", "Esta página precisa ser recarregada.", "Recarregar página", "Fechar aviso", "Explore a coleção pessoal de cards de beisebol Topps de 1991 do Mark e os anúncios disponíveis."],
        es: ["Cargando Mark's Baseball Cards", "Preparando la colección", "Es necesario recargar esta página.", "Recargar página", "Cerrar aviso", "Explora la colección personal de tarjetas de béisbol Topps de 1991 de Mark y los anuncios disponibles."]
    };
    function updateDocument() {
        document.documentElement.dataset.theme = theme;
        document.documentElement.lang = language;
        document.querySelector('meta[name="theme-color"]')?.setAttribute("content", theme === "dark" ? "#101b23" : "#f5f2eb");
        const copy = bootCopy[language];
        document.querySelector('meta[name="description"]')?.setAttribute("content", copy[5]);
        document.querySelector(".boot-screen")?.setAttribute("aria-label", copy[0]);
        const caption = document.querySelector(".boot-caption"); if (caption) caption.textContent = copy[1];
        const error = document.getElementById("boot-error-message"); if (error) error.textContent = copy[2];
        const reload = document.querySelector("#blazor-error-ui .reload"); if (reload) reload.textContent = copy[3];
        document.querySelector("#blazor-error-ui .dismiss")?.setAttribute("aria-label", copy[4]);
    }
    window.htsPreferences = {
        read: () => ({ language, theme }),
        apply: (nextLanguage, nextTheme) => {
            language = supported.includes(nextLanguage) ? nextLanguage : "en";
            theme = nextTheme === "dark" ? "dark" : "light";
            save(languageKey, language); save(themeKey, theme); updateDocument();
        }
    };
    updateDocument();
    document.addEventListener("DOMContentLoaded", updateDocument, { once: true });
})();
