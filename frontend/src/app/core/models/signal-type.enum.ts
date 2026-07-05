import { ApiEnum } from './api-enum.type';

export type SignalTypeKey = 'Cellular' | 'Satellite' | 'LoRaWAN' | 'Manual' | 'Simulator';
export type SignalType = ApiEnum<SignalTypeKey>;
