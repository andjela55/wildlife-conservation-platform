export interface Subspecies {
  id: number;
  speciesId: number;
  name: string;
  description: string;
}

export interface CreateSubspeciesRequest {
  speciesId: number;
  name: string;
  description: string;
}
