import { Competition } from "./competition"
import { Sport } from "./sport"
import { Stage } from "./stage"
import { Status } from "./status"
import { Team } from "./team"
import { Venue } from "./venue"


export interface Event {
  eventId: number
  date: string
  timeUTC: string
  durationInMinutes: number
  description: string | null
  homeScore: number
  awayScore: number
  winnerTeamId: number | null
  status: Status
  homeTeam: Team
  awayTeam: Team
  venue: Venue
  stage: Stage
  competition: Competition
  sport: Sport
}
