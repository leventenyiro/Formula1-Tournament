import { Season } from "./season";
import { User } from "./user";
import { UserSeasonPermission } from "./user-season-permission";

export class UserSeason {
  constructor(
    public id: string,
    public user: User,
    public season: Season,
    public userSeasonPermission: UserSeasonPermission
  ) {}
}
