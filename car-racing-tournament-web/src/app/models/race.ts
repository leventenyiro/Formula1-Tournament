import { Result } from "./result";
import { Season } from "./season";

export class Race {
  constructor(
    public id: string,
    public name: string,
    public dateTime: Date,
    public season: Season,
    public results: Result[]
  ) {}
}
