import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterModule } from '@angular/router';
import { FormsModule } from '@angular/forms';
import { MessageAdminService, ContactMessage, MessageStatus, MessageFilterDTO } from '../../services/message-admin.service';
import { LucideAngularModule, Search, Trash2, Eye, Mail, MailOpen } from 'lucide-angular';

@Component({
    selector: 'app-messages',
    standalone: true,
    imports: [CommonModule, RouterModule, FormsModule, LucideAngularModule],
    templateUrl: './messages.component.html',
    styleUrls: ['./messages.component.scss']
})
export class MessagesComponent implements OnInit {
    messages: ContactMessage[] = [];
    total = 0;
    loading = false;

    filter: MessageFilterDTO = {
        page: 1,
        pageSize: 10,
        searchTerm: '',
        status: undefined
    };

    selectedIds: Set<string> = new Set();

    // Expose Math for template
    readonly Math = Math;

    // Icons
    readonly SearchIcon = Search;
    readonly TrashIcon = Trash2;
    readonly EyeIcon = Eye;
    readonly MailIcon = Mail;
    readonly MailOpenIcon = MailOpen;

    constructor(private messageService: MessageAdminService) { }

    ngOnInit(): void {
        this.loadMessages();
    }

    loadMessages() {
        this.loading = true;
        this.messageService.getMessages(this.filter).subscribe({
            next: (res) => {
                this.messages = res.data;
                this.total = res.total;
                this.loading = false;
            },
            error: (err) => {
                console.error('Failed to load messages', err);
                this.loading = false;
            }
        });
    }

    onSearch() {
        this.filter.page = 1;
        this.loadMessages();
    }

    onStatusChange(status: string) {
        this.filter.status = status === 'all' ? undefined : parseInt(status);
        this.filter.page = 1;
        this.loadMessages();
    }

    onPageChange(page: number) {
        this.filter.page = page;
        this.loadMessages();
    }

    toggleSelection(id: string) {
        if (this.selectedIds.has(id)) {
            this.selectedIds.delete(id);
        } else {
            this.selectedIds.add(id);
        }
    }

    toggleAll(event: any) {
        if (event.target.checked) {
            this.messages.forEach(m => this.selectedIds.add(m.id));
        } else {
            this.selectedIds.clear();
        }
    }

    deleteMessage(id: string) {
        if (confirm('Bạn có chắc chắn muốn xóa tin nhắn này?')) {
            this.messageService.deleteMessage(id).subscribe(() => {
                this.loadMessages();
                this.selectedIds.delete(id);
            });
        }
    }

    bulkDelete() {
        if (this.selectedIds.size === 0) return;

        if (confirm(`Bạn có chắc chắn muốn xóa ${this.selectedIds.size} tin nhắn?`)) {
            this.messageService.bulkDelete(Array.from(this.selectedIds)).subscribe(() => {
                this.loadMessages();
                this.selectedIds.clear();
            });
        }
    }

    markAsRead(id: string, read: boolean) {
        const status = read ? MessageStatus.Read : MessageStatus.Unread;
        this.messageService.updateStatus(id, status).subscribe(() => {
            this.loadMessages();
        });
    }

    get totalPages(): number {
        return Math.ceil(this.total / this.filter.pageSize);
    }

    get pages(): number[] {
        return Array.from({ length: this.totalPages }, (_, i) => i + 1);
    }
}
