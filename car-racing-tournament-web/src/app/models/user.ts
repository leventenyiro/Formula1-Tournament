import { UserSeason } from "./user-season";

export class User {
  constructor(
    public id: string,
    public username: string,
    public userSeasons: UserSeason[]
  ) {}
}
