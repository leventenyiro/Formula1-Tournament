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
    public name?: string,
    public teamName?: string,
    public teamColor?: string,
    public position?: number
  ) {}
}

export class PositionStatistics {
  constructor(
    public position?: string,
    public number: number = 0
  ) {}
}
