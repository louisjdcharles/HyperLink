function addZeroes(n) {
    if (n.length < 2) return "0" + n;

    return n;
}

window.onload = () => {
    const promise = fetch("/time");
    promise
        .then(res => res.json())
        .then(res => {
            document.getElementById("hours").innerHTML = addZeroes(res.hours.toString());
            document.getElementById("miniutes").innerHTML = addZeroes(res.miniutes.toString());
            document.getElementById("seconds").innerHTML = addZeroes(res.seconds.toString());
        });
}