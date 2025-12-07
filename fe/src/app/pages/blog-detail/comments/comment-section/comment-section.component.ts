import { Component, Input, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { Router, RouterModule } from '@angular/router'; // Added RouterModule for routerLink
import { CommentService, CommentResponse } from '../../../../core/services/comment.service';
import { AuthService } from '../../../../core/services/auth.service';
import { AuthUser } from '../../../../core/models/auth.model';
import { CommentInputComponent } from '../comment-input/comment-input.component';
import { CommentItemComponent } from '../comment-item/comment-item.component';
import { LucideAngularModule } from 'lucide-angular';
import { firstValueFrom } from 'rxjs';

@Component({
    selector: 'app-comment-section',
    standalone: true,
    imports: [CommonModule, RouterModule, CommentInputComponent, CommentItemComponent, LucideAngularModule],
    templateUrl: './comment-section.component.html',
    styleUrls: ['./comment-section.component.scss']
})
export class CommentSectionComponent implements OnInit {
    @Input() blogId!: string;

    comments: CommentResponse[] = [];
    currentUser: AuthUser | null = null;
    loading: boolean = true;

    constructor(
        private commentService: CommentService,
        private authService: AuthService
    ) { }

    ngOnInit() {
        this.authService.currentUser$.subscribe(user => this.currentUser = user);
        if (this.blogId) {
            this.loadComments();
        }
    }

    async loadComments() {
        this.loading = true;
        try {
            this.comments = await firstValueFrom(this.commentService.getComments(this.blogId));
        } catch (error) {
            console.error('Failed to load comments', error);
        } finally {
            this.loading = false;
        }
    }

    async onAddComment(content: string) {
        if (!this.currentUser) return;

        try {
            const newComment = await firstValueFrom(this.commentService.addComment({
                blogPostId: this.blogId,
                content
            }));
            // Add to top of list
            this.comments.unshift(newComment);
        } catch (error) {
            console.error('Failed to add comment', error);
        }
    }

    async onReply(event: { parentId: string, content: string }) {
        try {
            const newReply = await firstValueFrom(this.commentService.addComment({
                blogPostId: this.blogId,
                content: event.content,
                parentId: event.parentId
            }));

            this.addReplyToTree(this.comments, newReply);
        } catch (error) {
            console.error('Failed to reply', error);
        }
    }

    async onDelete(commentId: string) {
        try {
            await firstValueFrom(this.commentService.deleteComment(commentId));
            this.removeCommentFromTree(this.comments, commentId);
        } catch (error) {
            console.error('Failed to delete comment', error);
        }
    }

    // Recursive helper to find parent and add reply
    private addReplyToTree(comments: CommentResponse[], reply: CommentResponse): boolean {
        for (let comment of comments) {
            if (comment.id === reply.parentId) {
                if (!comment.replies) comment.replies = [];
                comment.replies.push(reply);
                return true;
            }
            if (comment.replies && comment.replies.length) {
                if (this.addReplyToTree(comment.replies, reply)) return true;
            }
        }
        return false;
    }

    // Recursive helper to remove comment
    private removeCommentFromTree(comments: CommentResponse[], id: string): boolean {
        const index = comments.findIndex(c => c.id === id);
        if (index !== -1) {
            comments.splice(index, 1);
            return true;
        }

        for (let comment of comments) {
            if (comment.replies && comment.replies.length) {
                if (this.removeCommentFromTree(comment.replies, id)) return true;
            }
        }
        return false;
    }
}
