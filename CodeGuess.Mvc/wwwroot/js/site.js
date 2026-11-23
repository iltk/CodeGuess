// Please see documentation at https://learn.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.


// Scramble the CodeGuess title with binary digits on a loop on join page
function animateTitle() {
    const code = document.querySelector(".cg-hero-code");
    const guess = document.querySelector(".cg-hero-guess");
    const reduceMotion = window.matchMedia("(prefers-reduced-motion: reduce)").matches;

    // window.gsap is missing if the CDN is unreachable
    if (!code || !guess || !window.gsap || reduceMotion) return;

    const FRAME_SECONDS = 0.06;     // one frame every 60ms
    const FRAMES_PER_LETTER = 3;    // a new letter locks every 180ms
    const LOOP_DELAY_SECONDS = 3;

    // Put each letter in its own span so it can have its own color
    const letters = [code, guess].flatMap(word => {
        const text = word.textContent.trim();
        word.textContent = "";

        return [...text].map(char => {
            const span = document.createElement("span");
            word.appendChild(span);
            return { span, char };
        });
    });

    // Locked letters show their real character, all the others get a new digit
    function renderFrame(frame) {
        const lockedCount = Math.floor(frame / FRAMES_PER_LETTER);

        letters.forEach((letter, index) => {
            const isLocked = index < lockedCount;
            letter.span.textContent = isLocked ? letter.char : (Math.random() < 0.5 ? "0" : "1");
            letter.span.classList.toggle("cg-hero-scramble", !isLocked);
        });
    }

    const totalFrames = letters.length * FRAMES_PER_LETTER;
    const clock = { frame: 0 };
    let lastFrame = 0;

    renderFrame(0);

    gsap.to(clock, {
        frame: totalFrames,
        duration: totalFrames * FRAME_SECONDS,
        ease: "none",
        repeat: -1,
        repeatDelay: LOOP_DELAY_SECONDS,
        onUpdate() {
            const frame = Math.floor(clock.frame);
            if (frame === lastFrame) return; // only draw when the 60ms clock ticks

            lastFrame = frame;
            renderFrame(frame);
        },
    });
}

animateTitle();
