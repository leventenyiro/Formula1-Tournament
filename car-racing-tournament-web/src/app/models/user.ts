import { Permission } from "./permission";

export class User {
  constructor(
    public id: string,
    public username: string,
    public email: string,
    public permissions: Permission[]
  ) {}
}
