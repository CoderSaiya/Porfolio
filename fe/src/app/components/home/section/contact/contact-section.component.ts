import {Component, inject} from '@angular/core';
import { CommonModule } from '@angular/common';
import {ReactiveFormsModule, FormBuilder, Validators, FormGroup} from '@angular/forms';
import { LucideAngularModule } from 'lucide-angular';
import {ContactCreateReq, ContactService} from '../../../../core/services/contact.service';
import {firstValueFrom} from 'rxjs';

interface ContactInfo { icon: string; label: string; value: string; href: string; }
interface SocialLink { icon: string; href: string; label: string; }

@Component({
  selector: 'app-contact-section',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule, LucideAngularModule],
  templateUrl: './contact-section.component.html',
  styleUrls: ['./contact-section.component.scss'],
})
export class ContactSectionComponent {
  private svc = inject(ContactService)
  form: FormGroup;
  loading = false;
  submitted = false;
  successId?: string;
  error?: string;

  constructor(private fb: FormBuilder) {
    this.form = this.fb.group({
      name: ['', Validators.required],
      email: ['', [Validators.required, Validators.email]],
      subject: ['', Validators.required],
      message: ['', Validators.required],
    });
  }

  contactInfo: ContactInfo[] = [
    { icon: 'Mail', label: 'Email', value: 'nhatcuongdev.contacts@gmail.com', href: 'mailto:nhatcuongdev.contacts@gmail.com' },
    { icon: 'Phone', label: 'Điện thoại', value: '+84 935 234 074', href: 'tel:+84935234074' },
    { icon: 'MapPin', label: 'Địa chỉ', value: 'Hồ Chí Minh, Việt Nam', href: '#' },
  ];
  socialLinks: SocialLink[] = [
    { icon: 'Github', href: 'https://github.com/CoderSaiya', label: 'GitHub' },
    { icon: 'Linkedin', href: 'https://www.linkedin.com/in/nhat-cuong', label: 'LinkedIn' },
    { icon: 'Twitter', href: '#', label: 'Twitter' },
  ];

  async submit() {
    this.error = undefined;
    if (this.form.invalid) {
      this.form.markAllAsTouched();
      return;
    }

    this.loading = true;
    try {
      const payload = this.form.value as ContactCreateReq;
      const resp = await firstValueFrom(this.svc.submit(payload));
      this.successId = resp.id;
      this.submitted = true;
      this.form.disable();

      (window as any).gtag?.('event', 'contact_submit_success', { message_id: resp.id });
    } catch (e: any) {
      this.error = e?.error?.title || e?.message || 'Gửi thất bại. Vui lòng thử lại.';
      (window as any).gtag?.('event', 'contact_submit_failed');
    } finally {
      this.loading = false;
    }
  }

  // Cho phép gửi thêm tin nhắn mới nếu muốn
  newMessage() {
    this.submitted = false;
    this.successId = undefined;
    this.error = undefined;
    this.form.enable();
    this.form.reset();
  }
}
