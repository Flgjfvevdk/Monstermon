using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.GameContent.Creative;
using Microsoft.Xna.Framework;
using Terraria.Audio;

namespace Monstermon.Content.Items
{
    public class CapturedMonster : ModItem
    {
        public int CapturedNPCType { get; set; }
        public string CapturedNPCName { get; set; } = "Unknown";

        public override void SetDefaults()
        {
            Item.width = 32;
            Item.height = 32;
            Item.maxStack = 1;
            Item.value = Item.sellPrice(silver: 5);
            Item.rare = ItemRarityID.Blue;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.useTime = 20;
            Item.useAnimation = 20;
            Item.consumable = true;
            Item.UseSound = SoundID.Item4;
        }

        public override void ModifyTooltips(System.Collections.Generic.List<TooltipLine> tooltips)
        {
            // Add the name of the captured monster to the tooltip
            tooltips.Add(new TooltipLine(Mod, "MonsterType", $"Contains: {CapturedNPCName}"));
        }

        public override bool? UseItem(Player player)
        {
            // Spawn the captured NPC when the item is used
            if (CapturedNPCType > 0)
            {
                // Calculate spawn position in front of player
                Vector2 spawnPos = player.Center;
                spawnPos.X += player.direction * 50; // Spawn in front of player
                
                // Spawn the monster
                NPC.NewNPC(Item.GetSource_FromThis(), (int)spawnPos.X, (int)spawnPos.Y, CapturedNPCType);
                
                // Play release effect
                SoundEngine.PlaySound(SoundID.NPCDeath6, player.position);
                for (int i = 0; i < 15; i++)
                {
                    Dust.NewDust(spawnPos, 20, 20, DustID.MagicMirror, 0f, 0f, 150, default, 1.2f);
                }
                
                return true; // Consume the item
            }
            
            return false;
        }

        public override void AddRecipes()
        {
            // No recipes for captured monsters - they can only be obtained by capture
        }
    }
}