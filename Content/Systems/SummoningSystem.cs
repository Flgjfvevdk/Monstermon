using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;
using Microsoft.Xna.Framework;
using Terraria.Audio;
using Monstermon.Content.Buffs;

namespace Monstermon.Content.Items
{
    public class SummoningSystem : ModSystem
    {
        public static int?[] monster_to_player;
        public static int?[] player_to_monster;

        public override void Load()
        {
            monster_to_player = new int?[Main.maxPlayers];
            player_to_monster = new int?[Main.maxNPCs];
        }

        public static bool has_summon(Player player)
        {
            return player_to_monster[player.whoAmI] is int;
        }

        public static bool? summon_monster(Player player, CapturedMonster item)
        {
            if (Main.netMode == NetmodeID.MultiplayerClient) return null;

            // Spawn the captured NPC when the item is used
            if (item.MonsterType > 0)
            {
                // Calculate spawn position in front of player
                Vector2 spawnPos = player.Center;
                spawnPos.X += player.direction * 50; // Spawn in front of player

                // Spawn the monster
                NPC monster = NPC.NewNPCDirect(item.Item.GetSource_FromThis(), (int)spawnPos.X, (int)spawnPos.Y, item.MonsterType);

                // Play release effect
                SoundEngine.PlaySound(SoundID.NPCDeath6, player.position);
                for (int i = 0; i < 15; i++)
                {
                    Dust.NewDust(spawnPos, 20, 20, DustID.MagicMirror, 0f, 0f, 150, default, 1.2f);
                }

                // Give the monster the "Captured" status effect.
                monster.AddBuff(ModContent.BuffType<Buffs.Captured>(), 300);


                // Save the summoned monster, to retrieve him
                player_to_monster[player.whoAmI] = monster.whoAmI;
                monster_to_player[monster.whoAmI] = player.whoAmI;

                return true;
            }

            return false;
        }

        public static bool? retrieve_summon(Player player)
        {
            // Only execute this code on the server or in solo play.
            if (Main.netMode == NetmodeID.MultiplayerClient) return null;

            if (player_to_monster[player.whoAmI] is int index && Main.npc[index] is NPC monster && monster.active)
            {
                // Remove the buff because it should prevent the monster from despawning
                int buffIndex = monster.FindBuffIndex(ModContent.BuffType<Captured>());
                if (buffIndex >= 0)
                    monster.DelBuff(buffIndex);

                monster.active = false;
                monster_to_player[monster.whoAmI] = null;
                player_to_monster[player.whoAmI] = null;
                return true;
            }

            return false;
        }

        public static bool? retrieve_summon(int monster_id)
        {
            // Only execute this code on the server or in solo play.
            if (Main.netMode != NetmodeID.MultiplayerClient) return null;

            if (monster_to_player[monster_id] is int index && Main.player[index] is Player player && player.active)
            {
                return retrieve_summon(player);
            }

            return false;
        }
    }
}
