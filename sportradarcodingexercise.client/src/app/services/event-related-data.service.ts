// services/event-related-data.service.ts
import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { environment } from '../../environments/environment.development';
import { Competition } from '../models/competition';
import { Season } from '../models/season';
import { Sport } from '../models/sport';
import { Stage } from '../models/stage';
import { Status } from '../models/status';
import { Team } from '../models/team';
import { Venue } from '../models/venue';
import { EventDetail } from '../models/event-detail';

@Injectable({
  providedIn: 'root'
})
export class EventRelatedDataService {
  baseUrl = environment.apiUrl;

  constructor(private readonly http: HttpClient) { }

  getSports() {
    return this.http.get<Sport[]>(this.baseUrl + '/sports');
  }

  getCompetitions() {
    return this.http.get<Competition[]>(this.baseUrl + '/competitions');
  }

  getSeasons() {
    return this.http.get<Season[]>(this.baseUrl + '/seasons');
  }

  getStages() {
    return this.http.get<Stage[]>(this.baseUrl + '/stages');
  }

  getStatuses() {
    return this.http.get<Status[]>(this.baseUrl + '/statuses');
  }

  getTeams() {
    return this.http.get<Team[]>(this.baseUrl + '/teams');
  }

  getVenues() {
    return this.http.get<Venue[]>(this.baseUrl + '/venues');
  }

  getEventDetails(eventId: number) {
    return this.http.get<EventDetail[]>(this.baseUrl + '/event-details/' + eventId);
  }
}
