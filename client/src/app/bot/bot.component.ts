import { Component, OnDestroy, OnInit } from '@angular/core';
import { HttpClient } from '@angular/common/http';

@Component({
  selector: 'app-bot',
  templateUrl: './bot.component.html',
  styleUrls: ['./bot.component.css']
})
export class BotComponent implements OnInit, OnDestroy {
  private interval: any;
  private futureMessages: string[] = [];
  ipAddress:string = '';
  messages: any[] = [];
  loading: boolean = false;
  step: number = 0;
  county: string = '';

  constructor(private http: HttpClient) { }
  ngOnInit(): void {
    this.addBotMessage('Hello! I\'m CA Location chatbot.');
    this.interval = setInterval(() => {
      if (this.futureMessages.length > 0) {
        this.loading = true;
        setTimeout(() => {
          this.addBotMessage(this.futureMessages.shift());
          this.loading = false;
        }, 1000);
      }
    }, 2000);
    this.getIPAddress();
  }

  ngOnDestroy(): void {
    clearInterval(this.interval);
  }

  getIPAddress()
  {
    this.loading = true;
    this.http.get("https://api.ipify.org/?format=json").subscribe((res:any)=>{
      this.ipAddress = res.ip;
      this.futureMessages.push('Your IP address is '+ this.ipAddress);
      this.futureMessages.push('Checking what the server has to say about it...');
      this.loading = true;
      this.http.get("/api/GetIpAddressInfo?ipAddress=" + this.ipAddress).subscribe((res:any)=>{
        if (res.length > 0 && res[0].city != '' && res[0].county != null) {
          this.futureMessages.push('The server says you\'re in '+ res[0].city + ' which is in "' + res[0].county + '" county.');
          this.futureMessages.push('Is this right?');
          this.county = res[0].county;
          this.step = 1;
        }
        else {
          this.futureMessages.push('I\'m sorry, I don\'t know where you are.');
          this.futureMessages.push('Please enter your California county if you know or at least your city.');
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
          this.futureMessages.push('Great! I\'ll transfer you to the live agent.');
          this.futureMessages.push('I\'m transferring you to \'Batman\'.');
          this.futureMessages.push('Please wait while I connect you to \'Batman\'.');
          this.futureMessages.push('You are now connected to \'Batman\'.');
          setTimeout(() => {
            this.step = 3;
            this.futureMessages.push('I\'m Batman, I see you\'re from "' + this.county + '" county.');
            this.futureMessages.push('Ask me anything.');
          }, 10000);
        }
        else {
          this.futureMessages.push('I\'m sorry, I don\'t know where you are.');
          this.futureMessages.push('Please enter your California county if you know or at least your city.');
          this.step = 2;
        }
        break;
      case 2: // getting county from IP Address
        this.loading = true;
        this.http.get("/api/GetCountyFromText?text=" + text).subscribe((res:any)=>{
          if (res == null) {
            this.futureMessages.push('I\'m sorry, I don\'t know where you are.');
            this.futureMessages.push('Please try again by entering your California county if you know or at least your city.');
          }
          else {
            this.futureMessages.push('Did you mean the city of '+ res.city + ' which is in "' + res.county + '" county?');
            this.county = res.county;
            this.step = 1;
          }
        });
        break;
      case 3: // speak live agent
        this.loading = true;
        this.http.get("/api/TalkToBatman?question=" + text).subscribe((res:any)=>{
          // this.futureMessages.push('Because I\'m Batman.');
          debugger;
          this.futureMessages.push(res);
        });
        break;
      default:
        this.futureMessages.push('I\'m sorry, I don\'t know what you mean.');
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
    let avatar = this.step == 3 
      ? '/assets/batman.jpg'
      : isOdd ? '/assets/bot.jpg' : '/assets/bot-1.jpg';
    this.messages.push({
      text,
      sender: 'Bot',
      avatar: avatar
    });
  }
}
