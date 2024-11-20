import { Country } from "./country"

export interface Address {
  addressId: number
  streetNumber: string
  streetName: string
  city: string
  country: Country
}
