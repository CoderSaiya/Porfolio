import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ActivatedRoute, Router, RouterModule } from '@angular/router';
import { MessageAdminService, ContactMessage, MessageStatus } from '../../../services/message-admin.service';
import { LucideAngularModule, ArrowLeft, Trash2, Mail, MailOpen, Reply } from 'lucide-angular';

@Component({
    selector: 'app-message-detail',
    standalone: true,
    imports: [CommonModule, RouterModule, LucideAngularModule],
    templateUrl: './message-detail.component.html',
    styleUrls: ['./message-detail.component.scss']
})
export class MessageDetailComponent implements OnInit {
    message: ContactMessage | null = null;
    loading = false;

    // Icons
    readonly ArrowLeftIcon = ArrowLeft;
    readonly TrashIcon = Trash2;
    readonly MailIcon = Mail;
    readonly MailOpenIcon = MailOpen;
    readonly ReplyIcon = Reply;

    constructor(
        private route: ActivatedRoute,
        private router: Router,
        private messageService: MessageAdminService
    ) { }

    ngOnInit(): void {
        const id = this.route.snapshot.paramMap.get('id');
        if (id) {
            this.loadMessage(id);
        }
    }

    loadMessage(id: string) {
        this.loading = true;
        this.messageService.getMessage(id).subscribe({
            next: (msg) => {
                this.message = msg;
                this.loading = false;
                // Auto mark as read if unread
                if (msg.status === MessageStatus.Unread) {
                    this.markAsRead(true);
                }
            },
            error: (err) => {
                console.error('Failed to load message', err);
                this.loading = false;
                this.router.navigate(['/admin/messages']);
            }
        });
    }

    markAsRead(read: boolean) {
        if (!this.message) return;
        const status = read ? MessageStatus.Read : MessageStatus.Unread;
        this.messageService.updateStatus(this.message.id, status).subscribe(() => {
            if (this.message) {
                this.message.status = status;
                this.message.readAt = read ? new Date().toISOString() : undefined;
            }
        });
    }

    deleteMessage() {
        if (!this.message) return;
        if (confirm('Bạn có chắc chắn muốn xóa tin nhắn này?')) {
            this.messageService.deleteMessage(this.message.id).subscribe(() => {
                this.router.navigate(['/admin/messages']);
            });
        }
    }
}
