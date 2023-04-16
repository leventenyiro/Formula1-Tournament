export class Statistics {
  constructor(
    public name?: string,
    public numberOfRace?: number,
    public numberOfWin?: number,
    public numberOfPodium?: number,
    public numberOfChampion?: number,
    public sumPoint?: number,
    public seasonStatistics?: SeasonStatistics[],
    public positionStatistics?: PositionStatistics[]
  ) {}
}

export class SeasonStatistics {
  constructor(
    public id?: string,
    public name?: string,
    public color?: string,
    public position?: number
  ) {}
}

export class PositionStatistics {
  constructor(
    public position?: string,
    public number: number = 0
  ) {}
}
