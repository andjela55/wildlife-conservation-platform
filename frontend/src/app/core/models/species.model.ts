export interface Species {
  id: number;
  name: string;
  description: string;
}

export interface UpsertSpeciesRequest {
  name: string;
  description: string;
}
