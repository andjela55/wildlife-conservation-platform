import { Component, forwardRef, Input } from '@angular/core';
import { ControlValueAccessor, NG_VALUE_ACCESSOR, UntypedFormControl } from '@angular/forms';

export interface SearchableSelectOption {
  value: string | number | null;
  label: string;
}

@Component({
  selector: 'app-searchable-select',
  templateUrl: './searchable-select.component.html',
  styleUrls: ['./searchable-select.component.scss'],
  providers: [{
    provide: NG_VALUE_ACCESSOR,
    useExisting: forwardRef(() => SearchableSelectComponent),
    multi: true
  }]
})
export class SearchableSelectComponent implements ControlValueAccessor {
  @Input() label = '';
  @Input() set options(value: Array<SearchableSelectOption>) {
    this.allOptions = value;
    this.updateFilteredOptions();

    const selectedOption = this.allOptions.find((item) => item.value === this.selectedValue);
    if (selectedOption) {
      this.searchControl.setValue(selectedOption.label, { emitEvent: false });
    }
  }

  readonly searchControl = new UntypedFormControl('');
  filteredOptions: Array<SearchableSelectOption> = [];
  selectedValue: string | number | null = null;
  disabled = false;
  isOpen = false;
  filterText = '';
  private allOptions: Array<SearchableSelectOption> = [];
  private onChange: (value: string | number | null) => void = () => {};
  private onTouched: () => void = () => {};

  writeValue(value: string | number | null): void {
    this.selectedValue = value;
    this.filterText = '';
    this.updateFilteredOptions();
    const option = this.allOptions.find((item) => item.value === value) ?? null;
    this.searchControl.setValue(option?.label ?? '', { emitEvent: false });
  }

  registerOnChange(fn: (value: string | number | null) => void): void {
    this.onChange = fn;
  }

  registerOnTouched(fn: () => void): void {
    this.onTouched = fn;
  }

  setDisabledState(disabled: boolean): void {
    this.disabled = disabled;
    if (disabled) {
      this.searchControl.disable({ emitEvent: false });
    } else {
      this.searchControl.enable({ emitEvent: false });
    }
  }

  select(option: SearchableSelectOption): void {
    this.selectedValue = option.value;
    this.filterText = '';
    this.updateFilteredOptions();
    this.searchControl.setValue(option.label, { emitEvent: false });
    this.onChange(option.value);
    this.onTouched();
    this.isOpen = false;
  }

  open(): void {
    if (!this.disabled) {
      this.isOpen = true;
    }
  }

  filter(value: string): void {
    this.filterText = value;
    this.updateFilteredOptions();
    this.selectedValue = null;
    this.onChange(null);
    this.isOpen = true;
  }

  selectOption(event: MouseEvent, option: SearchableSelectOption): void {
    event.preventDefault();
    this.select(option);
  }

  close(): void {
    this.onTouched();
    this.isOpen = false;
  }

  private updateFilteredOptions(): void {
    const query = this.filterText.trim().toLocaleLowerCase();
    this.filteredOptions = query
      ? this.allOptions.filter((option) => option.label.toLocaleLowerCase().includes(query))
      : this.allOptions;
  }
}
