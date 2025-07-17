import { Component } from '@angular/core';
import { ChatWindowComponent } from '../chat-window.component/chat-window.component';
import { ChatSidebarComponent } from '../chat-sidebar.components/chat-sidebar/chat-sidebar.component';
@Component({
  selector: 'app-chat',
  imports: [ChatSidebarComponent , ChatWindowComponent],
  templateUrl: './chat.component.html',
  styleUrl: './chat.component.css'
})
export class ChatComponent {

}
