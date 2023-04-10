import { Favorite } from "./favorite";
import { Permission } from "./permission";

export class User {
  constructor(
    public username: string,
    public email: string,
    public id?: string,
    public permissions?: Permission[],
    public favorites?: Favorite[]
  ) {}
}
