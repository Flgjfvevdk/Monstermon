using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;
using Monstermon.Content.Buffs;
using Monstermon.Content.Items;

namespace Monstermon.Content.NPCs
{
    public class MonstermonGlobalNPC : GlobalNPC
    {
        public override bool InstancePerEntity => true;

        // Track if this NPC should be captured instead of killed
        public bool ShouldCapture { get; set; } = false;

        public override void OnKill(NPC npc)
        {
            // Check if the NPC is marked for capture (has the capture-ready buff)
            if (npc.HasBuff(ModContent.BuffType<CaptureReady>()))
            {
                Main.NewText($"Capturing {Lang.GetNPCNameValue(npc.type)}!", Color.LightGreen);

                // Create a captured monster item
                int capturedMonsterItemType = ModContent.ItemType<CapturedMonster>();

                // Spawn the captured monster item at the NPC's position
                int itemIndex = Item.NewItem(npc.GetSource_Death(), npc.getRect(), capturedMonsterItemType);

                // Store the monster data in the newly created item
                if (Main.item[itemIndex].ModItem is CapturedMonster capturedMonster)
                {
                    capturedMonster.MonsterType = npc.type;
                    capturedMonster.MonsterName = Lang.GetNPCNameValue(npc.type);
                    Main.NewText($"Successfully caught {capturedMonster.MonsterName}!", Color.LightGreen);
                }

                // Play capture effect
                SoundEngine.PlaySound(SoundID.Item4, npc.position);
                for (int i = 0; i < 30; i++)
                {
                    Vector2 dustVelocity = Vector2.UnitX.RotatedBy(Main.rand.NextFloat() * MathHelper.TwoPi) * Main.rand.NextFloat(3f);
                    Dust.NewDust(npc.position, npc.width, npc.height, DustID.MagicMirror,
                        dustVelocity.X, dustVelocity.Y, 150, default, 1.5f);
                }
            }
        }

        public override void UpdateLifeRegen(NPC npc, ref int damage)
        {
            // Add visual effect for NPCs marked for capture
            if (npc.HasBuff(ModContent.BuffType<CaptureReady>()))
            {
                if (Main.rand.NextBool(5))
                {
                    Vector2 position = npc.Center + new Vector2(Main.rand.Next(-npc.width / 2, npc.width / 2), Main.rand.Next(-npc.height / 2, npc.height / 2));
                    Dust.NewDust(position, 1, 1, DustID.MagicMirror, 0f, 0f, 0, Color.Cyan, 0.8f);
                }
            }
        }

        public override bool CheckActive(NPC npc)
        {
            if (npc.HasBuff(ModContent.BuffType<Captured>()))
            {
                // Prevent despawning if entity was summoned
                return false;
            }
            return true;
        }

        public override bool CheckDead(NPC npc)
        {
            /* In case the npc is a summoned monster, we do not want it to die, but to despawn
            */
            if (npc.HasBuff(ModContent.BuffType<Captured>()))
            {
                SummoningSystem.RetrieveSummon(npc.whoAmI);

                return false;
            }
            return true;
        }
    }
}
