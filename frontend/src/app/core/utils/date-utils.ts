export function toLocalDateTimeInputValue(date = new Date()): string {
  const offsetMs = date.getTimezoneOffset() * 60_000;
  return new Date(date.getTime() - offsetMs).toISOString().slice(0, 16);
}

export function localDateTimeInputToIso(value: string | null | undefined): string {
  return value ? new Date(value).toISOString() : new Date().toISOString();
}

export function localDateInputToIso(value: string | Date | null | undefined): string | null {
  if (!value) {
    return null;
  }

  return value instanceof Date
    ? value.toISOString()
    : new Date(`${value}T00:00:00`).toISOString();
}

export function localDateBoundaryToIso(value: string | Date, endOfDay: boolean): string {
  const date = value instanceof Date
    ? new Date(value.getFullYear(), value.getMonth(), value.getDate())
    : new Date(`${value}T00:00:00`);

  date.setHours(endOfDay ? 23 : 0, endOfDay ? 59 : 0, endOfDay ? 59 : 0, endOfDay ? 999 : 0);
  return date.toISOString();
}
