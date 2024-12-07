function changeTooltipVisibility() {
    var tooltipstextElems = document.querySelectorAll('.tooltiptext');

    if (document.getElementById('tooltipSwitcher').checked) {
        document.getElementById('tooltipSwitcherText').innerHTML = 'Disable tooltips';
        tooltipstextElems.forEach(function (el) {
            el.style.display = "block";
        });
    }
    else {
        document.getElementById('tooltipSwitcherText').innerHTML = 'Enable tooltips';
        tooltipstextElems.forEach(function (el) {
            el.style.display = "none";
        });
    }
}