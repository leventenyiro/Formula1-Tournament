import { Season } from "./season";
import { User } from "./user";

export class Favorite {
  constructor(
    public id: string,
    public user: User,
    public season: Season,
    public seasonId: string,
    public userId: string,
  ) {}
}
