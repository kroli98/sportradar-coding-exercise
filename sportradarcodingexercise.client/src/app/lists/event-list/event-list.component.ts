import { Component } from '@angular/core';
import { EventService } from '../../services/event.service';
import { Event } from '../../models/event';
import { Sport } from '../../models/sport';
import { EventRelatedDataService } from '../../services/event-related-data.service';
import { forkJoin } from 'rxjs';
import { NgbDateStruct } from '@ng-bootstrap/ng-bootstrap'; 


@Component({
  selector: 'app-event-list',
  templateUrl: './event-list.component.html',
  styleUrls: ['./event-list.component.css']
})
export class EventListComponent {

  events: Event[] = [];
  filteredEvents: Event[] = [];
  sports: Sport[] = [];
  loading = true;
  error: string | null = null;
  selectedSportId: number | null = null;
  selectedDate: string = '';
  model: NgbDateStruct | null = null;

  eventDetails: { [key: number]: any } = {}; 
  loadingDetails: { [key: number]: boolean } = {};
  expandedEvents: Set<number> = new Set(); 

  constructor(private readonly eventService: EventService,
              private readonly eventRelatedDataService: EventRelatedDataService
  ) { }

  ngOnInit() {
    this.loadEvents();
  }

  loadEvents() {
    this.loading = true;
    this.error = null;

    forkJoin({
      events: this.eventService.getEvents(),
      sports: this.eventRelatedDataService.getSports()
    }).subscribe({
      next: (data) => {
        this.events = data.events;
        this.filteredEvents = data.events;
        this.sports = data.sports;
        this.loading = false;
      },
      error: (error) => {
        console.error('Error loading events:', error);
        this.error = 'Failed to load sports events. Please try again later.';
        this.loading = false;
      }
    });
  }

  toggleEventDetails(eventId: number) {
    if (this.expandedEvents.has(eventId)) {
      this.expandedEvents.delete(eventId);
      delete this.eventDetails[eventId];
    } else {
      this.expandedEvents.add(eventId);
      this.loadEventDetails(eventId);
    }
  }

  loadEventDetails(eventId: number) {
    this.loadingDetails[eventId] = true;
    this.eventRelatedDataService.getEventDetails(eventId).subscribe({
      next: (details) => {
        this.eventDetails[eventId] = details || []; 
        this.loadingDetails[eventId] = false;
      },
      error: (error) => {
        console.error(`Error loading event details for event ${eventId}:`, error);
        this.eventDetails[eventId] = []; 
        this.loadingDetails[eventId] = false;
      }
    });
  }

  isEventExpanded(eventId: number): boolean {
    return this.expandedEvents.has(eventId);
  }

  applyFilters() {
    this.filteredEvents = this.events.filter(event => {
      const matchesSport = !this.selectedSportId || event.sport.sportId === this.selectedSportId;
      const matchesDate = !this.selectedDate || event.date === this.selectedDate;
      return matchesSport && matchesDate;
    });
  }

  onSportChange(sportId: number) {
    this.selectedSportId = sportId;
    this.applyFilters();
  }

  clearFilters() {
    this.selectedSportId = null;
    this.selectedDate = '';
    this.model = null; 
    this.applyFilters();
  }

  clearSportFilter() {
    this.selectedSportId = null;
    this.applyFilters();
  }

  clearDateFilter() {
    this.selectedDate = '';
    this.model = null; 
    this.applyFilters();
  }

  getSelectedSportName(): string {
    const sport = this.sports.find(s => s.sportId === this.selectedSportId);
    return sport ? sport.name : '';
  }

  onDateChange(date: NgbDateStruct) {
    if (date) {
      this.selectedDate = `${date.year}-${this.padNumber(date.month)}-${this.padNumber(date.day)}`;
      this.applyFilters();
    } else {
      this.clearDateFilter();
    }
  }

  private padNumber(num: number): string {
    return num < 10 ? `0${num}` : num.toString();
  }

  retry() {
    this.loadEvents();
  }

  formatTime(timeUTC: string): string {
    const timeParts = timeUTC.split(':');
    return `${timeParts[0]}:${timeParts[1]}`;
  }
}
