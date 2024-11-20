import { EventType } from "puppeteer"
import { Team } from "./team"

export interface EventDetail {
  eventDetailId: number
  recordedAtUTC: string
  description: string | null
  team: Team
  eventType: EventType
}
