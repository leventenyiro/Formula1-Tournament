import { Driver } from "./driver";
import { Race } from "./race";
import { Team } from "./team";
import { UserSeason } from "./user-season";

export class Season {
  constructor(
    public id: string,
    public name: string,
    public userSeasons: UserSeason[],
    public teams: Team[],
    public drivers: Driver[],
    public races: Race[]
  ) {}
}
