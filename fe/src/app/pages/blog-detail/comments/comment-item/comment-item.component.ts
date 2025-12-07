import { Component, EventEmitter, Input, Output } from '@angular/core';
import { CommonModule } from '@angular/common';
import { CommentResponse } from '../../../../core/services/comment.service';
import { AuthUser } from '../../../../core/models/auth.model';
import { LucideAngularModule } from 'lucide-angular';
import { CommentInputComponent } from '../comment-input/comment-input.component';

@Component({
    selector: 'app-comment-item',
    standalone: true,
    imports: [CommonModule, LucideAngularModule, CommentInputComponent],
    templateUrl: './comment-item.component.html',
    styleUrls: ['./comment-item.component.scss']
})
export class CommentItemComponent {
    @Input() comment!: CommentResponse;
    @Input() currentUser: AuthUser | null = null;
    @Output() reply = new EventEmitter<{ parentId: string, content: string }>();
    @Output() delete = new EventEmitter<string>();

    isReplying: boolean = false;

    onReplyClick() {
        this.isReplying = !this.isReplying;
    }

    onSubmitReply(content: string) {
        this.reply.emit({ parentId: this.comment.id, content });
        this.isReplying = false;
    }

    // Recursive event bubbling
    onNestedReply(event: { parentId: string, content: string }) {
        this.reply.emit(event);
    }

    onNestedDelete(commentId: string) {
        this.delete.emit(commentId);
    }

    onDeleteClick() {
        if (confirm('Are you sure you want to delete this comment?')) {
            this.delete.emit(this.comment.id);
        }
    }
}
