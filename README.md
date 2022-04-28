# BardMidiApi
A project built for an upcoming interview, showcasing the creation of a REST API from scratch, including database design and creation.  

Includes: 
- Dependency Injection
- Unit Test Dependency Injection
- LINQ Statements (Entity Framework Core)
- Clustered and Non-clustered indexing
- Performant SQL database
- Deployed to Linux server

## Context
In the game Mordhau, you can play instruments.  I developed https://github.com/Dimencia/LuteBot3 in order to read MIDI files and play them on Mordhau instruments

Many players have modified MIDIs to play better in the game, and submitted them to our Discord.  Previously, I had created a Discord bot to store data about these files in json, but there were thousands of entries and it wasn't scalable

Upon hearing about the requirements for an upcoming interview, I decided to make a mock API out of this data, and move it to an SQL database.  

This will likely be updated until the interview process is complete - afterward, some of the implementation will likely be swapped for an existing API that another Discord user already provides, with even more data.  

At current state 4/27, it is missing many tests and a lot of functionality, design could be improved (move API functionality to a Service), and I would like to also add a web frontend, authentication, and benchmark/improve DB performance
