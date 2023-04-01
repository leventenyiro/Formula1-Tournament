import { Driver } from "./driver";
import { Race } from "./race";
import { ResultType } from "./result-type";
import { Team } from "./team";

export class Result {
  constructor(
    public id: string,
    public type: ResultType,
    public position: number,
    public point: number,
    public driver: Driver,
    public driverId: string,
    public team: Team,
    public teamId: string,
    public race: Race,
    public raceId: string
  ) {}
}
