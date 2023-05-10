#!/bin/bash

cd ../car-racing-tournament-api;
if [[ $1 = init ]]
then
    echo $1;
    dotnet tool install --global dotnet-ef;
    export PATH="$PATH:/Users/'your user folder'/.dotnet/tools";
    dotnet restore;
    dotnet ef;
fi

dotnet ef database update 0;
rm -rf Migrations
dotnet ef migrations add init;
dotnet ef database update;
cd ..;
