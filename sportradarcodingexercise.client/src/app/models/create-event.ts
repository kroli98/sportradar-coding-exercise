export interface CreateEvent {
  date: string;
  timeUTC: string;
  durationInMinutes: number;
  description?: string;
  homeScore: number;
  awayScore: number;
  statusId: number;
  homeTeamId: number;
  awayTeamId: number;
  venueId: number;
  stageId: number;
  competitionId: number;
  sportId: number;
}
