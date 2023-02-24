import { Driver } from "./driver";
import { Race } from "./race";
import { Team } from "./team";
import { Permission } from "./permission";

export class Season {
  constructor(
    public id: string,
    public name: string,
    public description: string,
    public isArchived: string,
    public createdAt: Date,
    public permissions: Permission[],
    public teams: Team[],
    public drivers: Driver[],
    public races: Race[]
  ) {}
}
