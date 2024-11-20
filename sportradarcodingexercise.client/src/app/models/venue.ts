import { Address } from "./address"

export interface Venue {
  venueId: number
  name: string
  capacity: number
  address: Address
}
