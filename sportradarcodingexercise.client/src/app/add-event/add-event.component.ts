import { Component, OnInit } from '@angular/core';
import { AbstractControl, FormBuilder, FormGroup, ValidationErrors, Validators } from '@angular/forms';
import { EventRelatedDataService } from '../services/event-related-data.service';
import { Competition } from '../models/competition';
import { Sport } from '../models/sport';
import { Stage } from '../models/stage';
import { Status } from '../models/status';
import { Team } from '../models/team';
import { Venue } from '../models/venue';
import { forkJoin } from 'rxjs';
import { finalize } from 'rxjs/operators';
import { NgbDateStruct } from '@ng-bootstrap/ng-bootstrap';
import { EventService } from '../services/event.service';
import { CreateEvent } from '../models/create-event';

@Component({
  selector: 'app-add-event',
  templateUrl: './add-event.component.html',
  styleUrls: ['./add-event.component.css']
})
export class AddEventComponent implements OnInit {
  eventForm: FormGroup;
  sports: Sport[] = [];
  competitions: Competition[] = [];
  stages: Stage[] = [];
  statuses: Status[] = [];
  teams: Team[] = [];
  venues: Venue[] = [];
  loading = false;
  error: string | null = null;
  submitSuccess = false;
  formError: string | null = null;
  submitting = false;


  constructor(
    private readonly fb: FormBuilder,
    private readonly eventRelatedDataService: EventRelatedDataService,
    private readonly eventService: EventService
  ) {
    this.eventForm = this.fb.group({
      date: ['', [Validators.required]],
      timeUTC: ['', [Validators.required]],
      durationInMinutes: ['', [Validators.required, Validators.min(1)]],
      description: ['', [Validators.maxLength(1000)]], 
      homeScore: [0, [Validators.required, Validators.min(0)]],
      awayScore: [0, [Validators.required, Validators.min(0)]],
      statusId: ['', [Validators.required]],
      homeTeamId: ['', [Validators.required]],
      awayTeamId: ['', [Validators.required]],
      venueId: ['', [Validators.required]],
      stageId: ['', [Validators.required]],
      competitionId: ['', [Validators.required]],
      sportId: ['', [Validators.required]]
    }, { validators: [this.teamsNotSame] });
  }

  ngOnInit() {
    this.loadRelatedData();
  }

  loadRelatedData() {
    this.loading = true;
    this.error = null;
    forkJoin({
      sports: this.eventRelatedDataService.getSports(),
      competitions: this.eventRelatedDataService.getCompetitions(),
      stages: this.eventRelatedDataService.getStages(),
      statuses: this.eventRelatedDataService.getStatuses(),
      teams: this.eventRelatedDataService.getTeams(),
      venues: this.eventRelatedDataService.getVenues()
    }).pipe(
      finalize(() => {
        this.loading = false;
      })
    ).subscribe({
      next: (data) => {
        this.sports = data.sports;
        this.competitions = data.competitions;
        this.stages = data.stages;
        this.statuses = data.statuses;
        this.teams = data.teams;
        this.venues = data.venues;
      },
      error: (error) => {
        console.error('Error loading data:', error);
        this.error = 'Failed to load required data. Please try again.';
      }
    });
  }

  onSubmit() {
    if (this.eventForm.valid) {
      const formData = this.eventForm.value;

      const transformedData: CreateEvent = {
        date: this.formatDate(formData.date),
        timeUTC: this.formatTime(formData.timeUTC),
        durationInMinutes: Number(formData.durationInMinutes),
        description: formData.description, 
        homeScore: Number(formData.homeScore),
        awayScore: Number(formData.awayScore),
        statusId: Number(formData.statusId),
        homeTeamId: Number(formData.homeTeamId),
        awayTeamId: Number(formData.awayTeamId),
        venueId: Number(formData.venueId),
        stageId: Number(formData.stageId),
        competitionId: Number(formData.competitionId),
        sportId: Number(formData.sportId)
      };

      this.submitting = true;
      this.formError = null;
      console.log(transformedData)
      this.eventService.postEvent(transformedData)
        .pipe(finalize(() => this.submitting = false))
        .subscribe({
          next: (response) => {
            this.submitSuccess = true;
            this.formError = null;
            this.eventForm.reset();
            setTimeout(() => this.submitSuccess = false, 3000);
          },
          error: (error) => {
            this.formError = error.error?.message || 'Failed to add event';
            console.error('Add Event Error:', error);
            window.scrollTo({ top: 0, behavior: 'smooth' });
          }
        });
    } else {
      this.markFormGroupTouched(this.eventForm);
      if (this.eventForm.errors?.['sameTeams']) {
        this.formError = 'Home and Away teams cannot be the same';
      }
    }
  }

  markFormGroupTouched(formGroup: FormGroup) {
    Object.values(formGroup.controls).forEach(control => {
      control.markAsTouched();
      if (control instanceof FormGroup) {
        this.markFormGroupTouched(control);
      }
    });
  }

  onDateSelect(date: NgbDateStruct) {
    this.eventForm.patchValue({
      date: `${date.year}-${date.month.toString().padStart(2, '0')}-${date.day.toString().padStart(2, '0')}`
    });
  }

  private formatDate(date: any): string {
    if (typeof date === 'string') return date;
    return `${date.year}-${String(date.month).padStart(2, '0')}-${String(date.day).padStart(2, '0')}`;
  }

  private formatTime(time: string): string {
    return `${time}:00`;
  }

  teamsNotSame(group: AbstractControl): ValidationErrors | null {
    const homeTeam = group.get('homeTeamId')?.value;
    const awayTeam = group.get('awayTeamId')?.value;
    return homeTeam && awayTeam && homeTeam === awayTeam ? { sameTeams: true } : null;
  }

  getErrorMessage(controlName: string): string {
    const control = this.eventForm.get(controlName);
    if (control?.touched && control?.errors) {
      if (control.errors['required']) {
        return 'This field is required';
      }
      if (control.errors['min']) {
        return `Value must be at least ${control.errors['min'].min}`;
      }
      if (control.errors['sameTeams']) {
        return 'Home and away teams must be different';
      }
    }
    return '';
  }
}
