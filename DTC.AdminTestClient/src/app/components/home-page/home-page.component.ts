import { CommonModule } from '@angular/common';
import { Component } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { Project } from '../../core/models/Project';

@Component({
  selector: 'app-home-page',
  standalone: true,
  imports: [FormsModule,CommonModule],
  templateUrl: './home-page.component.html',
  styleUrl: './home-page.component.css'
})
export class HomePageComponent {
  features = [
    {
      icon: '⚡',
      title: 'Быстрая разработка',
      description: 'Современные инструменты для эффективной работы'
    },
    {
      icon: '🎨',
      title: 'Красивый дизайн',
      description: 'Современный и адаптивный интерфейс'
    },
    {
      icon: '🔧',
      title: 'Простая настройка',
      description: 'Легко кастомизировать под ваши нужды'
    }
  ];

  stats = [
    { number: '100+', label: 'Проектов' },
    { number: '50+', label: 'Клиентов' },
    { number: '99%', label: 'Удовлетворения' },
    { number: '24/7', label: 'Поддержка' }
  ];
  navigateToContact() {
    console.log('Переход к контактам');
  }


//   mockProjects: Project[] = [
//   {
//     name: 'Корпоративный портал управления',
//     version: '2.1.0',
//     versionDate: '2024-03-15',
//     description: 'Комплексная система для управления внутренними процессами компании с модулями HR, финансов и аналитики в реальном времени. Включает дашборды для руководства и мобильное приложение для сотрудников.',
//     isOpenSource: false,
//     photoUrl: 'https://images.unsplash.com/photo-1552664730-d307ca884978?ixlib=rb-4.0.3&auto=format&fit=crop&w=1000&q=80',
//     authorGroupId: 1,
//     status: {id:3,name: "In develop"},
//     projectTypeId: 1, // Веб-приложение
//     beginAge: 18,
//     endAge: 65,
//     files: []
//   },
//   {
//     name: 'Мобильное приложение доставки еды',
//     version: '1.4.2',
//     versionDate: '2024-02-28',
//     description: 'Кроссплатформенное приложение для службы доставки еды с функцией реального времени отслеживания заказа, интегрированными платежами и системой рекомендаций на основе AI.',
//     isOpenSource: true,
//     photoUrl: 'https://images.unsplash.com/photo-1556742049-0cfed4f6a45d?ixlib=rb-4.0.3&auto=format&fit=crop&w=1000&q=80',
//     authorGroupId: 2,
//     status: {id:3,name: "Approved"}, // Завершен
//     projectTypeId: 3, // Мобильное приложение
//     beginAge: 16,
//     endAge: 99,
//     files: []
//   },
//   {
//     name: 'Образовательная платформа для детей',
//     version: '3.0.1',
//     versionDate: '2024-01-10',
//     description: 'Интерактивная обучающая платформа для детей с игровыми элементами, системой достижений и родительским контролем. Поддерживает множество образовательных курсов и адаптируется под уровень ребенка.',
//     isOpenSource: false,
//     photoUrl: 'https://images.unsplash.com/photo-1588072432836-e10032774350?ixlib=rb-4.0.3&auto=format&fit=crop&w=1000&q=80',
//     authorGroupId: 3,
//     status: {id:1,name: "Registered"}, // Завершен
//     projectTypeId: 2, // Образовательная система
//     beginAge: 6,
//     endAge: 14,
//     files: []
//   }
// ];

}
