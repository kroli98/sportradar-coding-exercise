import { Injectable } from '@angular/core';
import { environment } from '../../environments/environment.development';
import { HttpClient } from '@angular/common/http';
import { Event } from '../models/event';
import {CreateEvent} from '../models/create-event';

@Injectable({
  providedIn: 'root'
})
export class EventService {

  baseUrl = environment.apiUrl;

  constructor(private http: HttpClient) { }

  getEvents() {
    return this.http.get<Event[]>(this.baseUrl + '/sport-events');
  }

  getEvent(eventId: number) {
    return this.http.get<Event>(this.baseUrl + '/sport-events/id/' + eventId);
  }

  getEventByDate(date: string) {
    return this.http.get<Event>(this.baseUrl + '/sport-events/date/' + date);
  }

  getEventBySportId(sportId: number) {
    return this.http.get<Event>(this.baseUrl + '/sport-events/sport/' + sportId);
  }

  postEvent(event: CreateEvent) {
    return this.http.post(this.baseUrl + '/sport-events', event);
  }
}
