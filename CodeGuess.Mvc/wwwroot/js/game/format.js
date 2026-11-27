// helpers that turn values into text.

// 1 → "1st", 2 → "2nd", 11 → "11th" 
const ordinalRules = new Intl.PluralRules("en", { type: "ordinal" });
const ordinalSuffix = { one: "st", two: "nd", few: "rd", other: "th" };

export function ordinal(n) {
    return n + ordinalSuffix[ordinalRules.select(n)];
}

// 1500 → "1,500"
export function points(n) {
    return n.toLocaleString("en-CA");
}

// (3, 10) → "Q 03 / 10"
export function questionLabel(number, count) {
    const pad = n => String(n).padStart(2, "0");
    return `Q ${pad(number)} / ${pad(count)}`;
}
