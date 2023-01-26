import { Driver } from "./driver";
import { Result } from "./result";
import { Season } from "./season";

export class Team {
  constructor(
    public id: string,
    public name: string,
    public color: string,
    public season: Season,
    public drivers: Driver[],
    public results: Result[]
  ) {}
}
