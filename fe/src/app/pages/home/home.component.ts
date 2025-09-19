import {Component, OnDestroy, OnInit} from "@angular/core";
import {CommonModule} from '@angular/common';
import {RouterModule} from '@angular/router';
import {AboutSectionComponent} from '../../components/home/section/about/about-section.component';
import {NavigationComponent} from '../../components/home/navigation/navigation.component';
import {HeroSectionComponent} from '../../components/home/section/hero/hero-section.component';
import {LoadingScreenComponent} from '../../components/home/loading/loading-screen.component';
import {ProjectsSectionComponent} from '../../components/home/section/projects/projects-section.component';
import {SkillsSectionComponent} from '../../components/home/section/skills/skills-section.component';
import {ContactSectionComponent} from '../../components/home/section/contact/contact-section.component';

@Component({
  selector: "app-home",
  standalone: true,
  imports: [CommonModule, RouterModule, AboutSectionComponent, NavigationComponent, HeroSectionComponent, LoadingScreenComponent, ProjectsSectionComponent, SkillsSectionComponent, ContactSectionComponent],
  templateUrl: "./home.component.html",
  styleUrls: ["home.component.scss"],
})
export class HomeComponent implements OnInit, OnDestroy {
  ngOnDestroy(): void {
  }

  ngOnInit(): void {
  }

}
