using Monstermon.Content.Items;
using Monstermon.Content.Types;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;

public class Trainer : ModPlayer
{
    public MonsterTeam team;

    public override void LoadData(TagCompound tag)
    {
        team = tag.Get<MonsterTeam>("team");
    }

    public override void SaveData(TagCompound tag)
    {
        tag.Add("team", team);
    }

}