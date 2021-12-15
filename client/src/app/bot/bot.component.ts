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
  step: number = 0;

  constructor(private http: HttpClient) { }

  ngOnInit(): void {
    this.addBotMessage('Hello! I\'m CA Location chatbot.');
    this.getIPAddress();
  }

  getIPAddress()
  {
    this.loading = true;
    this.http.get("https://api.ipify.org/?format=json").subscribe((res:any)=>{
      this.loading = false;
      this.ipAddress = res.ip;
      this.addBotMessage('Your IP address is '+ this.ipAddress);
      this.addBotMessage('Checking what the server has to say about it...');
      this.loading = true;
      this.http.get("/api/GetIpAddressInfo?ipAddress=" + this.ipAddress).subscribe((res:any)=>{
        this.loading = false;
        if (res.length > 0 && res[0].city != '' && res[0].county != null) {
          this.addBotMessage('The server says you\'re in '+ res[0].city + ' which is in "' + res[0].county + '" county.');
          this.addBotMessage('Is this right?');
          this.step = 1;
        }
        else {
          this.addBotMessage('I\'m sorry, I don\'t know where you are.');
          this.addBotMessage('Please enter your California county if you know or at least your city.');
          this.step = 2;
        }
      });  
    });
  }

  handleUserMessage(event: any) {
    const text = event.message;
    this.addUserMessage(text);

    switch(this.step) {
      case 0: // still figuring out where you are from IP Address
        break;
      case 1: // confirming yes or no from IP Address
        if (text.toLowerCase() == 'yes') {
          this.addBotMessage('Great! I\'ll transfer you to the live agent.');
          this.step = 3;
          // TODO: 
          this.addBotMessage('I\'m transferring you to \'Batman\'.');
          this.addBotMessage('Please wait while I connect you to \'Batman\'.');
      }
        else {
          this.addBotMessage('I\'m sorry, I don\'t know where you are.');
          this.addBotMessage('Please enter your California county if you know or at least your city.');
          this.step = 2;
        }
        break;
      case 2: // getting county from IP Address
        this.loading = true;
        this.http.get("/api/GetCounty?city=" + text).subscribe((res:any)=>{
          this.loading = false;
          if (res.length > 0) {
            this.loading = true;
            setTimeout(() => {
              this.addBotMessage('The server says you\'re in '+ res[0].city + ' which is in "' + res[0].county + '" county.');
              setTimeout(() => {
                this.addBotMessage('Is this right?');
                this.loading = false;
              }, 1000);
            }, 1000);
            this.step = 1;
          }
          else {
            this.loading = true;
            setTimeout(() => {
              this.addBotMessage('I\'m sorry, I don\'t know where you are.');
              setTimeout(() => {
                this.addBotMessage('Please enter your California county if you know or at least your city.');
                this.loading = false;
              }, 1000);
            }, 1000);
          }
        });
        break;
      case 3: // speak live agent
        // TODO:
        this.loading = true;
        setTimeout(() => {
          this.loading = false;
          this.addBotMessage('Because I\'m Batman.');
        }, 500);
        // this.loading = true;
        //this.http.get("/api/AskBatman?text=" + text).subscribe((res:any)=>{
        //  this.loading = false;
        //  this.addBotMessage(res);
        //});
        break;
      default:
        this.loading = true;
        setTimeout(() => {
          this.loading = false;
          this.addBotMessage('I\'m sorry, I don\'t know what you mean.');
        }, 1000);
        break;
    }  
  }

  addUserMessage(text: any) {
    this.messages.push({
      text,
      sender: 'You',
      reply: true
    });
  }

  addBotMessage(text: any) {
    let isOdd: boolean = Boolean(this.messages.length % 2);
    let avatar = isOdd ? '/assets/bot.jpg' : '/assets/bot-1.jpg';
    this.messages.push({
      text,
      sender: 'Bot',
      avatar: avatar
    });
  }
}
