import { Race } from "./race";
import { Result } from "./result";
import { Season } from "./season";
import { Team } from "./team";

export class Driver {
  constructor(
    public id: string,
    public name: string,
    public realName: string,
    public number: number,
    public actualTeam: Team,
    public season: Season,
    public races: Race[],
    public results: Result[]
  ) {}
}
