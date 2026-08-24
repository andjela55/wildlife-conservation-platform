export interface Subspecies {
  id: number;
  speciesId: number;
  name: string;
  description: string;
}

export interface UpsertSubspeciesRequest {
  speciesId: number;
  name: string;
  description: string;
}
