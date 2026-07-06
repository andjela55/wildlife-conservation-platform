import { Injectable, OnDestroy } from '@angular/core';
import { HubConnection, HubConnectionBuilder, HubConnectionState, LogLevel } from '@microsoft/signalr';
import { BehaviorSubject, defer, from, map, Observable, of, Subject, tap } from 'rxjs';
import { environment } from 'src/environments/environment';
import { LocationPointReceived } from '../models';

@Injectable({ providedIn: 'root' })
export class AnimalTrackingSignalRService implements OnDestroy {
  private connection?: HubConnection;
  private readonly locationPointReceivedSubject = new Subject<LocationPointReceived>();
  private readonly isConnectedSubject = new BehaviorSubject<boolean>(false);

  readonly locationPointReceived$ = this.locationPointReceivedSubject.asObservable();
  readonly isConnected$ = this.isConnectedSubject.asObservable();

  start(): Observable<void> {
    return defer(() => {
      const connection = this.getConnection();

      if (connection.state === HubConnectionState.Connected) {
        this.isConnectedSubject.next(true);
        return of(void 0);
      }

      return from(connection.start()).pipe(
        tap(() => this.isConnectedSubject.next(true)),
        map(() => void 0)
      );
    });
  }

  stop(): Observable<void> {
    return defer(() => {
      if (!this.connection || this.connection.state === HubConnectionState.Disconnected) {
        this.isConnectedSubject.next(false);
        return of(void 0);
      }

      return from(this.connection.stop()).pipe(
        tap(() => this.isConnectedSubject.next(false)),
        map(() => void 0)
      );
    });
  }

  ngOnDestroy(): void {
    this.stop().subscribe();
    this.locationPointReceivedSubject.complete();
    this.isConnectedSubject.complete();
  }

  private getConnection(): HubConnection {
    if (this.connection) {
      return this.connection;
    }

    this.connection = new HubConnectionBuilder()
      .withUrl(`${environment.apiUrl}/animal-tracking-hub`, { withCredentials: false })
      .withAutomaticReconnect()
      .configureLogging(LogLevel.Information)
      .build();

    this.connection.onreconnecting(() => this.isConnectedSubject.next(false));
    this.connection.onreconnected(() => this.isConnectedSubject.next(true));
    this.connection.onclose(() => this.isConnectedSubject.next(false));
    this.connection.on('LocationPointReceived', (locationPoint: LocationPointReceived) => {
      this.locationPointReceivedSubject.next(locationPoint);
    });

    return this.connection;
  }
}
