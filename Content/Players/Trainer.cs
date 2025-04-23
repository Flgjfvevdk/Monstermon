using Monstermon.Content.Items;
using Monstermon.Content.Types;
using Monstermon.Content.UI.TeamManager;
using Terraria;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;

public class Trainer : ModPlayer
{
    public MonsterTeam team;

    public override void Initialize()
    {
        team = new MonsterTeam();
    }

    public override void LoadData(TagCompound tag)
    {
        team = tag.Get<MonsterTeam>("team");
        Mod.Logger.Info($"MONSTERMON : Loaded team : {team}");
    }

    public override void SaveData(TagCompound tag)
    {
        tag.Add("team", team);
    }

    public override void OnEnterWorld()
    {
        ModContent.GetInstance<TeamManagerUISystem>().InitializeUI();
    }

}