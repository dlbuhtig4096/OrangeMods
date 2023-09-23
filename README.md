
# Building #
 - Install the dotnet 6 SDK.
 - Install and run the [BepInEx fork](https://www.nexusmods.com/megamanxdiveoffline/mods/1).
 - Build the project with `dotnet publish /p:Configuration=Release;Platform=AnyCPU;Orange="(Root directory to the game)".`
 - The output binary will be available at `bin/Release/net6.0/publish/OrangeMods.dll`.

# Credits #
 - **CAPCOM TAIWAN** for creating this game.
 - The **BepInEx team** who created the modding framework.
 - **[SutandoTsukai181](https://github.com/SutandoTsukai181)** for his works on making the modding framework and workarounding il2cpp for the game.
 - **[Jrprogrammer](https://github.com/Jrprogrammer)** for his [corelib](https://github.com/Jrprogrammer/CoreLib/), we are taking some code from the repository.
