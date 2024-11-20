import { Season } from "./season"

export interface Competition {
  competitionId: number
  name: string
  competitionSlug: string
  season: Season
}
