export interface Species {
  id: number;
  name: string;
  description: string;
}

export interface CreateSpeciesRequest {
  name: string;
  description: string;
}
