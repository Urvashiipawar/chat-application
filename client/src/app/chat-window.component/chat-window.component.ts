import { Component, ElementRef, ViewChild } from '@angular/core';
import { inject } from '@angular/core';
import { ChatService } from '../services/chat.service';
import { TitleCasePipe } from '@angular/common';
import { MatIconModule } from '@angular/material/icon';
import { FormsModule } from '@angular/forms';
import { ChatBoxComponent } from "../components/chat-box-component/chat-box.component";
import { VideoChatService } from '../services/video-chat.service';
import { MatDialog } from '@angular/material/dialog';
import { VideoChatComponent } from '../video-chat/video-chat.component';

@Component({
  selector: 'app-chat-window',
  imports: [TitleCasePipe, MatIconModule, FormsModule, ChatBoxComponent],
  templateUrl: './chat-window.component.html',
  styles: ``
})
export class ChatWindowComponent {
  @ViewChild('chatBoxComponent') chatContainer?: ElementRef;
  dialog = inject(MatDialog);
  chatService = inject(ChatService);
  signalRService = inject(VideoChatService);
  message: string ='';

  sendMessage() {
    if(!this.message) return;
    this.chatService.sendMessage(this.message);
    this.message='';
    this.scrollToBottom();
   
  }

  displayDialog(receiverId:string){
    this.signalRService.remoteUserId = receiverId;

    this.dialog.open(VideoChatComponent,{
      width:"400px",
      height:'600px',
      disableClose:true,
      autoFocus:false
    })

  }

  private scrollToBottom(){
    if(this.chatContainer){
      this.chatContainer.nativeElement.scrollTop =
        this.chatContainer.nativeElement.scrollHeight;
    }

  }



}
