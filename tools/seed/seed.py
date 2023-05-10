import json

writeFile = open("results.cs", "a")

# Load the JSON file
with open('f12021.json', 'r') as f:
    data = json.load(f)

# Extract results from each race
for race in data['races']:
    for result in race['results']:
        writeFile.write(f"new Result\n{{\n")
        writeFile.write(f"\tId = new Guid(\"{result['id']}\")")
        writeFile.write(f",\n\tType = ResultType.{result['type']}")
        writeFile.write(f",\n\tPosition = {result['position']}")
        writeFile.write(f",\n\tPoint = {result['point']}")
        writeFile.write(f",\n\tDriverId = new Guid(\"{result['driverId']}\")")
        writeFile.write(f",\n\tTeamId = new Guid(\"{result['teamId']}\")")
        writeFile.write(f",\n\tRaceId = new Guid(\"{race['id']}\")")
        writeFile.write("\n},\n")

writeFile.close()

writeFile = open("drivers.cs", "a")

# Extract drivers
for driver in data['drivers']:
    writeFile.write(f"new Driver\n{{\n")
    writeFile.write(f"\tId = new Guid(\"{driver['id']}\")")
    writeFile.write(f",\n\tName = \"{driver['name'].split(' ')[1]}\"")
    writeFile.write(f",\n\tRealName = \"{driver['name']}\"")
    writeFile.write(f",\n\tNumber = {driver['number']}")
    writeFile.write(f",\n\tActualTeamId = new Guid(\"{driver['actualTeamId']}\")")
    writeFile.write(f",\n\tSeasonId = new Guid(\"{data['id']}\")")
    writeFile.write("\n},\n")

writeFile.close()

writeFile = open("teams.cs", "a")

# Extract teams
for team in data['teams']:
    writeFile.write(f"new Team\n{{\n")
    writeFile.write(f"\tId = new Guid(\"{team['id']}\")")
    writeFile.write(f",\n\tName = \"{team['name']}\"")
    writeFile.write(f",\n\tColor = \"{team['color']}\"")
    writeFile.write(f",\n\tSeasonId = new Guid(\"{data['id']}\")")
    writeFile.write("\n},\n")

writeFile.close()

writeFile = open("races.cs", "a")

# Extract races
for race in data['races']:
    writeFile.write(f"new Race\n{{\n")
    writeFile.write(f"\tId = new Guid(\"{race['id']}\")")
    writeFile.write(f",\n\tName = \"{race['name']}\"")
    writeFile.write(f",\n\tDateTime = DateTime.Parse(\"{race['dateTime']}\")")
    writeFile.write(f",\n\tSeasonId = new Guid(\"{data['id']}\")")
    writeFile.write("\n},\n")

writeFile.close()
