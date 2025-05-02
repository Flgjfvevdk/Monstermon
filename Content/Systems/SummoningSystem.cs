using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;
using Microsoft.Xna.Framework;
using Terraria.Audio;
using Monstermon.Content.Buffs;
using Monstermon.Content.Items;

namespace Monstermon.Content.Systems
{
    public class SummoningSystem : ModSystem
    {
        static int?[] monsterToPlayer;
        static int?[] playerToMonster;

        public override void Load()
        {
            monsterToPlayer = new int?[Main.maxPlayers];
            playerToMonster = new int?[Main.maxNPCs];
        }

        public static bool HasSummon(Player player)
        {
            return playerToMonster[player.whoAmI] is int;
        }

        public static bool? SummonMonster(Player player, CapturedMonster item)
        {
            if (Main.netMode == NetmodeID.MultiplayerClient) return null;

            // Spawn the captured NPC when the item is used
            if (item.MonsterType != NPCID.None)
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
                playerToMonster[player.whoAmI] = monster.whoAmI;
                monsterToPlayer[monster.whoAmI] = player.whoAmI;

                return true;
            }

            return false;
        }

        public static bool? RetrieveSummon(Player player)
        {
            // Only execute this code on the server or in solo play.
            if (Main.netMode == NetmodeID.MultiplayerClient) return null;

            if (playerToMonster[player.whoAmI] is int index && Main.npc[index] is NPC monster)
            {
                // Remove the buff because it should prevent the monster from despawning
                int buffIndex = monster.FindBuffIndex(ModContent.BuffType<Captured>());
                if (buffIndex >= 0)
                    monster.DelBuff(buffIndex);

                monster.active = false;
                monsterToPlayer[monster.whoAmI] = null;
                playerToMonster[player.whoAmI] = null;
                return true;
            }

            return false;
        }

        public static bool? RetrieveSummon(int monsterIdx)
        {
            // Only execute this code on the server or in solo play.
            if (Main.netMode == NetmodeID.MultiplayerClient) return null;

            if (monsterToPlayer[monsterIdx] is int index && Main.player[index] is Player player)
            {
                return RetrieveSummon(player);
            }

            return false;
        }
    }
}
