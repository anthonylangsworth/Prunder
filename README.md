# Pruner

A quick and dirty command-line tool to manage members of an in-game **Elite Dangerous** squadron and the squadron's Discord server. It compares the lists of Discord and in-game squadron users, recommending u

## Use

1. Get the output from `/permission-report` from [Cat Herder](https://github.com/anthonylangsworth/CatHerder). Set `DiscordMembersFile` in `appsettings.json` to that file name.
2. Get the output from [Squadron Dump](https://github.com/anthonylangsworth/SquadronDump). Set `SquadronMembersFile` in `appsettings.json` to that file name.
3. (Optional) Update `DiscordToSquadronMemberMap` in `appsettings.json` to map Discord users to Elite commander names. The comparison is case insensitive. This is likely an iterative and ongoing process.
4. Edit `Program.cs` to show the desired report(s).

## License

See [LICENSE](LICENSE) for the licence (GPL v3).