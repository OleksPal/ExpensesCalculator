import { Component } from '@angular/core';
import { ScrollTopComponent } from '../../shared/scroll-top/scroll-top.component';
import { TranslatePipe } from '@ngx-translate/core';

@Component({
  selector: 'app-home',
  imports: [ScrollTopComponent, TranslatePipe],
  templateUrl: './home.component.html',
  styleUrl: './home.component.css'
})
export class HomeComponent {

}
