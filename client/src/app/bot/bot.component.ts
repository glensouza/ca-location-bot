import { Component, OnInit } from '@angular/core';
import { HttpClient } from '@angular/common/http';

const dialogflowURL = 'http://localhost:3000/getBotResponse';

@Component({
  selector: 'app-bot',
  templateUrl: './bot.component.html',
  styleUrls: ['./bot.component.css']
})
export class BotComponent implements OnInit {
  ipAddress:string = '';
  messages: any[] = [];
  loading: boolean = false;
  // Random ID to maintain session with server
  sessionId = Math.random().toString(36).slice(-5);

  constructor(private http: HttpClient) { }

  ngOnInit(): void {
    this.addBotMessage('Hello! I\'m CA Location chatbot.');
    this.getIPAddress();
  }

  getIPAddress()
  {
    this.http.get("https://api.ipify.org/?format=json").subscribe((res:any)=>{
      this.ipAddress = res.ip;
      this.addBotMessage('Your IP address is '+ this.ipAddress);
    });
  }

  handleUserMessage(event: any) {
    console.log(event);
    const text = event.message;
    this.addUserMessage(text);

    this.loading = true;

    // Make the request 
    this.http.post<any>(
      dialogflowURL,
      {
        sessionId: this.sessionId,
        queryInput: {
          text: {
            text,
            languageCode: 'en-US'
          }
        }
      }
    )
    .subscribe(res => {
      const { fulfillmentText } = res;
      this.addBotMessage(fulfillmentText);
      this.loading = false;
    });
  }

  addUserMessage(text: any) {
    this.messages.push({
      text,
      sender: 'You',
      reply: true,
      date: new Date()
    });
  }

  addBotMessage(text: any) {
    let isOdd: boolean = Boolean(this.messages.length % 2);
    let avatar = isOdd ? 'assets/images/bot.png' : 'assets/images/bot-1.png';
    this.messages.push({
      text,
      sender: 'Bot',
      avatar: avatar,
      date: new Date()
    });
  }
}
