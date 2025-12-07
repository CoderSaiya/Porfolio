import { Component, EventEmitter, Input, Output } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { PickerComponent } from '@ctrl/ngx-emoji-mart';
import { LucideAngularModule } from 'lucide-angular';

@Component({
    selector: 'app-comment-input',
    standalone: true,
    imports: [CommonModule, FormsModule, PickerComponent, LucideAngularModule],
    templateUrl: './comment-input.component.html',
    styleUrls: ['./comment-input.component.scss']
})
export class CommentInputComponent {
    @Input() placeholder: string = 'Viết một bình luận...';
    @Input() buttonLabel: string = 'Đăng tải';
    @Input() isReply: boolean = false;
    @Output() submitComment = new EventEmitter<string>();
    @Output() cancel = new EventEmitter<void>();

    content: string = '';
    showEmojiPicker: boolean = false;

    addEmoji(event: any) {
        this.content += event.emoji.native;
        // Keep focus or picker open? usually close after one or keep open. Let's keep open.
    }

    toggleEmojiPicker() {
        this.showEmojiPicker = !this.showEmojiPicker;
    }

    onSubmit() {
        if (this.content.trim()) {
            this.submitComment.emit(this.content);
            this.content = '';
            this.showEmojiPicker = false;
        }
    }

    onCancel() {
        this.cancel.emit();
    }
}
