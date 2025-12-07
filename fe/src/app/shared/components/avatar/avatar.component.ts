import { Component, Input, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { getInitials, getAvatarColor } from '../../../core/utils/avatar.util';

@Component({
    selector: 'app-avatar',
    standalone: true,
    imports: [CommonModule],
    template: `
        <div class="avatar" [class.avatar--small]="size === 'sm'" [class.avatar--large]="size === 'lg'">
            <img *ngIf="avatarUrl" [src]="avatarUrl" [alt]="name" class="avatar__img">
            <div *ngIf="!avatarUrl" class="avatar__initials" [style.background-color]="bgColor">
                {{ initials }}
            </div>
        </div>
    `,
    styles: [`
        .avatar {
            width: 40px;
            height: 40px;
            border-radius: 50%;
            overflow: hidden;
            flex-shrink: 0;
            
            &--small {
                width: 32px;
                height: 32px;
            }
            
            &--large {
                width: 64px;
                height: 64px;
            }
            
            &__img {
                width: 100%;
                height: 100%;
                object-fit: cover;
            }
            
            &__initials {
                width: 100%;
                height: 100%;
                display: flex;
                align-items: center;
                justify-content: center;
                font-weight: 600;
                color: white;
                font-size: 16px;
                user-select: none;
            }
            
            &--small &__initials {
                font-size: 14px;
            }
            
            &--large &__initials {
                font-size: 24px;
            }
        }
    `]
})
export class AvatarComponent implements OnInit {
    @Input() avatarUrl?: string | null;
    @Input() name?: string | null;
    @Input() size: 'sm' | 'md' | 'lg' = 'md';

    initials: string = '';
    bgColor: string = '';

    ngOnInit() {
        this.initials = getInitials(this.name);
        this.bgColor = getAvatarColor(this.name);
    }
}
