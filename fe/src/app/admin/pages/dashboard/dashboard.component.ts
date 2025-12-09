import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterModule } from '@angular/router';
import { LucideAngularModule, LayoutDashboard, FileText, FolderGit2, MessageSquare, TrendingUp, Users, Clock, Plus, ArrowRight, MoreHorizontal, Settings } from 'lucide-angular';

interface Activity {
  id: number;
  type: 'blog' | 'project' | 'message';
  title: string;
  description: string;
  time: string;
  icon: string;
}

@Component({
  selector: 'app-dashboard',
  standalone: true,
  imports: [CommonModule, RouterModule, LucideAngularModule],
  templateUrl: './dashboard.component.html',
  styleUrl: './dashboard.component.scss'
})
export class DashboardComponent implements OnInit {
  chartData = [
    { label: 'T2', value: 45 },
    { label: 'T3', value: 72 },
    { label: 'T4', value: 38 },
    { label: 'T5', value: 95 },
    { label: 'T6', value: 60 },
    { label: 'T7', value: 25 },
    { label: 'CN', value: 80 }
  ];

  maxValue = 100;

  recentActivities: Activity[] = [
    {
      id: 1,
      type: 'blog',
      title: 'Bài viết mới',
      description: 'đã được xuất bản: "Angular Tips 2024"',
      time: '2 giờ trước',
      icon: 'FileText'
    },
    {
      id: 2,
      type: 'message',
      title: 'Nguyễn Văn A',
      description: 'đã gửi tin nhắn liên hệ',
      time: '4 giờ trước',
      icon: 'MessageSquare'
    },
    {
      id: 3,
      type: 'project',
      title: 'Dự án Portfolio',
      description: 'đã được cập nhật ảnh bìa',
      time: '1 ngày trước',
      icon: 'FolderGit2'
    },
    {
      id: 4,
      type: 'blog',
      title: 'Bình luận mới',
      description: 'trên bài viết "Clean Architecture"',
      time: '2 ngày trước',
      icon: 'MessageSquare'
    }
  ];

  ngOnInit() {
    // Here we could fetch real data from a service
    // For now using mock data as requested
  }
}
