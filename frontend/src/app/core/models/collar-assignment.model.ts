export interface CollarAssignment {
  id: number;
  animalId: number;
  collarId: number;
  assignedAt: string;
  unassignedAt: string | null;
  reason: string | null;
  notes: string | null;
}

export interface CreateCollarAssignmentRequest {
  animalId: number;
  collarId: number;
  assignedAt: string;
  reason: string | null;
  notes: string | null;
}

export interface UnassignCollarRequest {
  unassignedAt: string | null;
  reason: string | null;
  notes: string | null;
}
