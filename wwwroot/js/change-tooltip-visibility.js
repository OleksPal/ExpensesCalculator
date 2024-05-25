function changeTooltipVisibility() {
    var leftTooltipstextElems = document.querySelectorAll('.left-tooltiptext');
    var topTooltipstextElems = document.querySelectorAll('.top-tooltiptext');

    if (document.getElementById('tooltipSwitcher').checked) {
        document.getElementById('tooltipSwitcherText').innerHTML = 'Disable tooltips';
        leftTooltipstextElems.forEach(function (el) {
            el.style.display = "block";
        });

        topTooltipstextElems.forEach(function (el) {
            el.style.display = "block";
        });
    }
    else {
        document.getElementById('tooltipSwitcherText').innerHTML = 'Enable tooltips';
        leftTooltipstextElems.forEach(function (el) {
            el.style.display = "none";
        });

        topTooltipstextElems.forEach(function (el) {
            el.style.display = "none";
        });
    }
}