import { Component } from '@angular/core';
import { RouterLink } from "@angular/router";
import { TranslatePipe } from '@ngx-translate/core';

@Component({
  selector: 'app-vertical-navbar',
  imports: [RouterLink, TranslatePipe],
  templateUrl: './vertical-navbar.component.html',
  styleUrl: './vertical-navbar.component.css'
})
export class VerticalNavbarComponent {

}
