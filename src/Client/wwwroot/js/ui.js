/*
 * HTS / Mark's Baseball Cards.
 * OriginKit Stagger Text Rise and Skiper16 / StickyCard_001 adaptations.
 * Attribution and source hashes: docs/UI_REFERENCES.md.
 */
let observer, mutations, frame, motionPreference, pointerPreference;
let initialized = false, reduced = false;
let seen = new WeakSet(), stacks = new Set(), cleanupTilt = [];

function reveal(el) {
    if (reduced || !el.animate) return;
    el.animate([{ opacity: .15, transform: "translateY(18px)" }, { opacity: 1, transform: "translateY(0)" }], { duration: 620, easing: "cubic-bezier(.22,1,.36,1)" });
}
function rise(el) {
    if (reduced || !el.animate) return;
    // OriginKit's per-character y + opacity interpolation and 30ms stagger.
    // A smaller offset and capped total delay suit the collection headings.
    const chars = [...el.querySelectorAll(".rise-char")];
    const delay = Math.min(30, 220 / Math.max(chars.length - 1, 1));
    chars.forEach((char, i) => char.animate(
        [{ transform: "translateY(18px)", opacity: .1 }, { transform: "translateY(0)", opacity: 1 }],
        { duration: 580, delay: i * delay, easing: "cubic-bezier(.22,1,.36,1)", fill: "backwards" }
    ));
}
function observeContent() {
    document.querySelectorAll("[data-reveal],[data-rise]").forEach(el => {
        if (seen.has(el)) return;
        seen.add(el);
        if (!reduced) observer.observe(el);
    });
    document.querySelectorAll("[data-card-stack]").forEach(el => stacks.add(el));
    stacks.forEach(el => { if (!el.isConnected) stacks.delete(el); });
    document.querySelectorAll("[data-tilt]").forEach(el => {
        if (seen.has(el)) return;
        seen.add(el);
        if (reduced || !pointerPreference.matches) return;
        const move = e => {
            const rect = el.getBoundingClientRect();
            el.style.setProperty("--tilt-x", (((e.clientX - rect.left) / rect.width - .5) * 7).toFixed(2) + "deg");
            el.style.setProperty("--tilt-y", (((e.clientY - rect.top) / rect.height - .5) * -5).toFixed(2) + "deg");
        };
        const reset = () => { el.style.setProperty("--tilt-x", "0deg"); el.style.setProperty("--tilt-y", "0deg"); };
        el.addEventListener("pointermove", move, { passive: true });
        el.addEventListener("pointerleave", reset);
        cleanupTilt.push(() => { el.removeEventListener("pointermove", move); el.removeEventListener("pointerleave", reset); reset(); });
    });
    schedule();
}
function updateScroll() {
    frame = null;
    document.querySelector(".site-header")?.classList.toggle("scrolled", window.scrollY > 12);
    if (reduced) return;
    // Skiper16's useTransform(progress, range, [1, targetScale]), with native scroll.
    stacks.forEach(stack => {
        if (!stack.isConnected) { stacks.delete(stack); return; }
        const rect = stack.getBoundingClientRect();
        if (rect.bottom < 0 || rect.top > window.innerHeight) return;
        const progress = Math.max(0, Math.min(1, (150 - rect.top) / Math.max(rect.height - 260, 1)));
        const cards = [...stack.querySelectorAll("[data-stack-card]")];
        cards.forEach((card, i) => {
            const start = i * .25, target = Math.max(.92, 1 - (cards.length - i - 1) * .035);
            const local = Math.max(0, Math.min(1, (progress - start) / (1 - start)));
            card.style.setProperty("--stack-scale", (1 + (target - 1) * local).toFixed(4));
        });
    });
}
function schedule() { if (frame == null) frame = requestAnimationFrame(updateScroll); }
function motionChanged() {
    reduced = motionPreference.matches;
    document.getAnimations().forEach(a => a.cancel());
    cleanupTilt.forEach(fn => fn()); cleanupTilt = [];
    observer.disconnect();
    stacks.forEach(s => s.querySelectorAll("[data-stack-card]").forEach(c => c.style.removeProperty("--stack-scale")));
    seen = new WeakSet(); observeContent();
}
export function initialize() {
    if (initialized) return;
    initialized = true;
    motionPreference = matchMedia("(prefers-reduced-motion: reduce)");
    pointerPreference = matchMedia("(hover: hover) and (pointer: fine)");
    reduced = motionPreference.matches;
    observer = new IntersectionObserver(entries => {
        entries.forEach(entry => {
            if (!entry.isIntersecting) return;
            observer.unobserve(entry.target);
            if (entry.target.hasAttribute("data-rise")) rise(entry.target); else reveal(entry.target);
        });
    }, { threshold: .12 });
    mutations = new MutationObserver(observeContent);
    mutations.observe(document.getElementById("main-content"), { childList: true, subtree: true });
    window.addEventListener("scroll", schedule, { passive: true });
    window.addEventListener("resize", schedule, { passive: true });
    motionPreference.addEventListener("change", motionChanged);
    observeContent();
}
export function onNavigate() {
    observer?.disconnect();
    cleanupTilt.forEach(fn => fn()); cleanupTilt = [];
    seen = new WeakSet();
    document.querySelectorAll("dialog[open]").forEach(dialog => dialog.close());
    document.documentElement.style.removeProperty("overflow");
    requestAnimationFrame(() => {
        let target = null;
        if (location.hash) { try { target = document.getElementById(decodeURIComponent(location.hash.slice(1))); } catch {} }
        if (target) target.scrollIntoView({ behavior: "auto" });
        else window.scrollTo({ top: 0, behavior: "instant" });
        observeContent();
    });
}
const dialogFocus = new WeakMap();
export function openDialog(dialog) {
    if (!dialog || dialog.open) return;
    dialogFocus.set(dialog, document.activeElement);
    dialog.showModal();
    document.documentElement.style.overflow = "hidden";
    dialog.querySelector("button")?.focus({ preventScroll: true });
}
export function closeDialog(dialog) {
    if (!dialog) return;
    dialog.close();
    document.documentElement.style.removeProperty("overflow");
    const focus = dialogFocus.get(dialog);
    if (focus?.isConnected) focus.focus({ preventScroll: true });
    dialogFocus.delete(dialog);
}
export function dispose() {
    observer?.disconnect(); mutations?.disconnect();
    cleanupTilt.forEach(fn => fn()); cleanupTilt = [];
    window.removeEventListener("scroll", schedule);
    window.removeEventListener("resize", schedule);
    motionPreference?.removeEventListener("change", motionChanged);
    if (frame != null) cancelAnimationFrame(frame);
    frame = null; stacks.clear(); seen = new WeakSet();
    document.documentElement.style.removeProperty("overflow");
    initialized = false;
}
