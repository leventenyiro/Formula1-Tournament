import { Season } from "./season";
import { User } from "./user";
import { PermissionType } from "./permission-type";

export class Permission {
  constructor(
    public id: string,
    public user: User,
    public season: Season,
    public type: PermissionType,
    public userId: string,
    public username?: string,
  ) {}
}
