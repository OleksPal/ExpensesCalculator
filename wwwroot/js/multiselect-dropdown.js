$(document).ready(function () {
    const dropdownButton = document.getElementById('multiSelectDropdown');
    const dropdownMenu = document.querySelector('.dropdown-menu');

     function handleMultiSelectList(event) {
         const checkbox = event.target;
         if (checkbox.checked) {
             mySelectedItems.push(checkbox.value);
         } else {
             mySelectedItems =
                 mySelectedItems.filter((item) => item !== checkbox.value);
         }

         dropdownButton.innerText = mySelectedItems.length > 0
             ? mySelectedItems.join(', ') : 'Select users';
     }

    dropdownMenu.addEventListener('change', handleMultiSelectList);
});