const testSpan = document.createElement("span");
testSpan.textContent = "MMMMMMMMMM"; // dont worry about it
testSpan.style.position = "absolute";
testSpan.style.visibility = "hidden";

window.measureCharacter = () => {
    document.body.appendChild(testSpan);
    const width = testSpan.offsetWidth;
    const height = testSpan.offsetHeight;
    testSpan.remove();
    return { width, height };
};
