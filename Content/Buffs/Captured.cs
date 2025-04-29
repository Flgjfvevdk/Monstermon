using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;


namespace Monstermon.Content.Buffs
{
    public class Captured : ModBuff
    {
        public override void SetStaticDefaults()
        {
            // Display name and tooltip shown when hovering over buff icon
            // Localization is now handled through language files

            // This buff is not saved when the world is exited
            Main.buffNoSave[Type] = true;

            // Do not display remaining time
            Main.buffNoTimeDisplay[Type] = true;

            // This buff is bad for NPCs (marks them for capture)
            Main.debuff[Type] = true;

            // Don't allow this buff to be blocked by NPCs
            BuffID.Sets.NurseCannotRemoveDebuff[Type] = true;
        }

        public override void Update(NPC npc, ref int buffIndex)
        {
            // Make the NPC friendly to players (won't damage them)
            npc.friendly = true;

            // Visual effect to show the NPC is marked for capture
            if (Main.rand.NextBool(5))
            {
                Vector2 position = npc.Center + new Vector2(Main.rand.Next(-npc.width / 2, npc.width / 2), Main.rand.Next(-npc.height / 2, npc.height / 2));
                Dust.NewDust(position, 1, 1, DustID.MagicMirror, 0f, 0f, 0, Color.Cyan, 0.8f);
            }

            // Increase duration if buff was to expire
            if (npc.buffTime[buffIndex] < 60)
                npc.buffTime[buffIndex] += 600;
        }

        public override void ModifyBuffText(ref string buffName, ref string tip, ref int rare)
        {
            buffName = "Captured!";
            tip = "This mob has been captured by a player.";
            rare = ItemRarityID.Quest;
        }
    }
}
