export class Statistics {
  constructor(
    public numberOfRace?: number,
    public numberOfWin?: number,
    public numberOfPodium?: number,
    public numberOfChampion?: number,
    public sumPoint?: number,
    public seasonStatistics?: SeasonStatistics[],
  ) {}
}

export class SeasonStatistics {
  constructor(
    public id?: string,
    public name?: string,
    public position?: number
  ) {}
}

export class PositionStatistics {
  constructor(
    public position?: string,
    public number?: number
  ) {}
}
