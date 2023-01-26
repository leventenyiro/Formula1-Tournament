import { Driver } from "./driver";
import { Race } from "./race";
import { Team } from "./team";

export class Result {
  constructor(
    public id: string,
    public position: number,
    public points: number,
    public driver: Driver,
    public team: Team,
    public race: Race
  ) {}
}
