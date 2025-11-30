const testSpan = document.createElement("span");
testSpan.textContent = "MMMMMMM";
testSpan.style.position = "absolute";
testSpan.style.visibility = "hidden";
testSpan.style.fontFamily = "monospace";

window.measureCharacter = () => {
    document.body.appendChild(testSpan);
    const width = testSpan.offsetWidth;
    const height = testSpan.offsetHeight;
    testSpan.remove();
    return { width, height };
};
