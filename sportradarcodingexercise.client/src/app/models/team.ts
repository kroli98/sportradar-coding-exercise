import { Country } from "./country"
import { Sport } from "./sport"

export interface Team {
  teamId: number
  name: string
  officialName: string
  slug: string
  abbreviation: string
  country: Country
  sport: Sport
}
