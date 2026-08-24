import { Directive, ElementRef, EventEmitter, OnDestroy, Output } from '@angular/core';
import { fromEvent, Subject } from 'rxjs';
import { debounceTime, distinctUntilChanged, map, takeUntil } from 'rxjs/operators';

@Directive({
  selector: '[valueAfterTyping]'
})
export class TypingTimeDirective implements OnDestroy {
  @Output() typed = new EventEmitter<string>();
  private readonly destroy$ = new Subject<void>();

  constructor(element: ElementRef<HTMLInputElement>) {
    fromEvent<InputEvent>(element.nativeElement, 'input').pipe(
      map((event) => (event.target as HTMLInputElement).value),
      debounceTime(300),
      distinctUntilChanged(),
      takeUntil(this.destroy$)
    ).subscribe((value) => this.typed.emit(value));
  }

  ngOnDestroy(): void {
    this.destroy$.next();
    this.destroy$.complete();
  }
}
