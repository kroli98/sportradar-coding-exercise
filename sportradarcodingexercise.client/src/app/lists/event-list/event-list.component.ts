import { Component } from '@angular/core';
import { EventService } from '../../services/event.service';
import { Event } from '../../models/event';


@Component({
  selector: 'app-event-list',
  templateUrl: './event-list.component.html',
  styleUrls: ['./event-list.component.css']
})
export class EventListComponent {

  events: Event[] = []
  loading = true;
  error: string | null = null;

  constructor(private readonly eventService: EventService) { }

  ngOnInit() {
    this.loadEvents();
  }

  loadEvents() {
    this.loading = true;
    this.error = null;

    this.eventService.getEvents().subscribe({
      next: (events) => {
        this.events = events;
        this.loading = false;
      },
      error: (error) => {
        console.error('Error loading events:', error);
        this.error = 'Failed to load sports events. Please try again later.';
        this.loading = false;
      }
    });
  }

  retry() {
    this.loadEvents();
  }

  formatTime(timeUTC: string): string {
    const timeParts = timeUTC.split(':');
    return `${timeParts[0]}:${timeParts[1]}`;
  }
}
