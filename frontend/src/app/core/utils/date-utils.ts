export function toLocalDateTimeInputValue(date = new Date()): string {
  const offsetMs = date.getTimezoneOffset() * 60_000;
  return new Date(date.getTime() - offsetMs).toISOString().slice(0, 16);
}

export function localDateTimeInputToIso(value: string | null | undefined): string {
  return value ? new Date(value).toISOString() : new Date().toISOString();
}

export function localDateInputToIso(value: string | null | undefined): string | null {
  return value ? new Date(`${value}T00:00:00`).toISOString() : null;
}
