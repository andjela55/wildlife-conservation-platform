import { Pipe, PipeTransform } from '@angular/core';

export type BelgradeDateFormat = 'short' | 'shortTime' | 'dayMonthTime';

@Pipe({ name: 'belgradeDate' })
export class BelgradeDatePipe implements PipeTransform {
  transform(value: string | Date | null | undefined, format: BelgradeDateFormat = 'short'): string {
    if (!value) {
      return '';
    }

    const date = value instanceof Date ? value : new Date(value);
    if (Number.isNaN(date.getTime())) {
      return '';
    }

    return new Intl.DateTimeFormat('en-GB', {
      timeZone: 'Europe/Belgrade',
      ...this.getFormatOptions(format)
    }).format(date);
  }

  private getFormatOptions(format: BelgradeDateFormat): Intl.DateTimeFormatOptions {
    if (format === 'shortTime') {
      return { hour: '2-digit', minute: '2-digit', hourCycle: 'h23' };
    }

    if (format === 'dayMonthTime') {
      return { month: 'short', day: 'numeric', hour: '2-digit', minute: '2-digit', hourCycle: 'h23' };
    }

    return {
      year: 'numeric',
      month: '2-digit',
      day: '2-digit',
      hour: '2-digit',
      minute: '2-digit',
      hourCycle: 'h23'
    };
  }
}
