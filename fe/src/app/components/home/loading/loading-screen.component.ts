import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';

@Component({
  selector: 'app-loading-screen',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './loading-screen.component.html',
  styleUrls: ['./loading-screen.component.scss'],
})
export class LoadingScreenComponent {
  isLoading = true;
  progress = 0;

  ngOnInit(): void {
    const timer = setInterval(() => {
      if (this.progress >= 100) {
        clearInterval(timer);
        setTimeout(() => (this.isLoading = false), 500);
      } else {
        this.progress = Math.min(100, this.progress + Math.random() * 15);
      }
    }, 150);
  }
}
