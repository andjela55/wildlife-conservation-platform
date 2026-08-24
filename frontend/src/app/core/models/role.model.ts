import { PermissionCode } from './permission-code.enum';

export interface Role {
  id: number;
  name: string;
  description: string;
  permissions: ReadonlyArray<PermissionCode>;
}
