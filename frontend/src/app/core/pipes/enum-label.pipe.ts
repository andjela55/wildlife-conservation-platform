import { Pipe, PipeTransform } from '@angular/core';
import { enumLabel, EnumName } from '../utils/enum-utils';

@Pipe({ name: 'enumLabel' })
export class EnumLabelPipe implements PipeTransform {
  transform(value: unknown, enumName?: EnumName): string {
    return enumLabel(value, enumName);
  }
}
