import { Component, OnInit } from '@angular/core';
import { HttpClient } from '@angular/common/http';

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
      this.addBotMessage('Checking what the server has to say about it...');   
      this.http.get("/api/GetIpAddressInfo?ipAddress=" + this.ipAddress).subscribe((res:any)=>{
        debugger;
        if (res.length > 0 && res[0].city != '' && res[0].county != null) {
          this.addBotMessage('The server says you\'re in '+ res[0].city + ' which is in "' + res[0].county + '" county.');
          this.addBotMessage('Is this right?');
        }
        else {
          this.addBotMessage('I\'m sorry, I don\'t know where you are.');
        }
      });  
    });
  }

  handleUserMessage(event: any) {
    console.log(event);
    const text = event.message;
    this.addUserMessage(text);

    this.loading = true;

    // Make the request 
    this.http.post<any>(
      "/api/bot",
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
    let avatar = isOdd ? '/assets/bot.jpg' : '/assets/bot-1.jpg';
    this.messages.push({
      text,
      sender: 'Bot',
      avatar: avatar,
      date: new Date()
    });
  }
}
