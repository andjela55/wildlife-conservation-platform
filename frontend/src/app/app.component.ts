import { Component } from '@angular/core';

@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.scss']
})
export class AppComponent {
  title = 'frontend';
  isSidebarCollapsed = false;
  readonly navigationItems = [
    {
      label: 'Dashboard',
      route: '/',
      icon: 'assets/icons/home.svg',
      exact: true
    },
    {
      label: 'Animals',
      route: '/animals',
      icon: 'assets/icons/paw.svg'
    },
    {
      label: 'Species',
      route: '/species',
      icon: 'assets/icons/category.svg'
    },
    {
      label: 'Reports',
      route: '/reports',
      icon: 'assets/icons/report.svg'
    },
    {
      label: 'Alerts',
      route: '/alerts',
      icon: 'assets/icons/alert.svg'
    },
    {
      label: 'Collars',
      route: '/collars',
      icon: 'assets/icons/collar.svg'
    }
  ];

  toggleSidebar(): void {
    this.isSidebarCollapsed = !this.isSidebarCollapsed;
  }
}
