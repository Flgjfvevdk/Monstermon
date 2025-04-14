using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;
using Microsoft.Xna.Framework;
using Terraria.Audio;
// using Monstermon.Content.Buffs.Captured;
using Monstermon.Content.Buffs;

namespace Monstermon.Content.Items
{
    public class CapturedMonster : ModItem
    {
        public int MonsterType { get; set; }
        public string MonsterName { get; set; } = "Unknown";
        public int level { get; set; } = 1;
        public int deployedMonster { get; set; }
        //public int cooldown { get; set; } = 0;

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
            tooltips.Add(new TooltipLine(Mod, "MonsterType", $"Contains: {MonsterName} - LVL: {level}"));
        }

        public override bool? UseItem(Player player)
        {
            // // Call back the monster if it has already been summoned
            // if (deployedMonster is not null)
            // {
                
            // }
            // Spawn the captured NPC when the item is used
            if (MonsterType > 0)
            {
                // Calculate spawn position in front of player
                Vector2 spawnPos = player.Center;
                spawnPos.X += player.direction * 50; // Spawn in front of player
                
                // Spawn the monster
                NPC monster = NPC.NewNPCDirect(Item.GetSource_FromThis(), (int)spawnPos.X, (int)spawnPos.Y, MonsterType);
                
                // Play release effect
                SoundEngine.PlaySound(SoundID.NPCDeath6, player.position);
                for (int i = 0; i < 15; i++)
                {
                    Dust.NewDust(spawnPos, 20, 20, DustID.MagicMirror, 0f, 0f, 150, default, 1.2f);
                }

                // Give the monster the "Captured" status effect.
                monster.AddBuff(ModContent.BuffType<Buffs.Captured>(),300);
                // Save The summoned monster, to call it back into his ball
                deployedMonster = monster.whoAmI;
                
                return false; // Does not consume the item
            }
            
            return false;
        }

        // Save data when the item is saved
        public override void SaveData(TagCompound tag)
        {
            // Save the monster type as an integer
            tag["MonsterType"] = MonsterType;
            
            // Save the monster name as a string
            tag["MonsterName"] = MonsterName;
            
            // Save the monster level as an integer
            tag["Level"] = level;
        }

        // Load data when the item is loaded
        public override void LoadData(TagCompound tag)
        {
            // Load the monster type, default to 0 if not found
            MonsterType = tag.GetInt("MonsterType");
            
            // Load the monster name, default to "Unknown" if not found
            MonsterName = tag.GetString("MonsterName");
            if (string.IsNullOrEmpty(MonsterName)){
                // If name is missing but we have a valid type, try to get the name from the type
                if (MonsterType > 0){
                    MonsterName = Lang.GetNPCNameValue(MonsterType);
                }
                else{
                    MonsterName = "Unknown";
                }
            }
            
            // Load the monster level, default to 1 if not found
            level = tag.GetInt("Level");
            if (level < 1) level = 1; // Ensure level is at least 1
        }

        // Override Clone to correctly copy our custom properties
        public override ModItem Clone(Item item)
        {
            CapturedMonster clone = (CapturedMonster)base.Clone(item);
            clone.MonsterType = this.MonsterType;
            clone.MonsterName = this.MonsterName;
            clone.level = this.level;
            return clone;
        }

        public override void AddRecipes()
        {
            // No recipes for captured monsters - they can only be obtained by capture
        }
    }
}
