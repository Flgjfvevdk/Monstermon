using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;
using Monstermon.Content.Items;

namespace Monstermon.Content.Projectiles
{
    public class InstantCaptureProjectile : ModProjectile
    {
        public override void SetDefaults()
        {
            Projectile.width = 8;
            Projectile.height = 8;
            Projectile.aiStyle = 1; // Bullet AI
            Projectile.friendly = true;
            Projectile.hostile = false;
            Projectile.DamageType = DamageClass.Ranged;
            Projectile.penetrate = 1;
            Projectile.timeLeft = 600;
            Projectile.alpha = 0;
            Projectile.light = 0.5f;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = true;
            Projectile.extraUpdates = 1;

            AIType = ProjectileID.Bullet;
        }

        public override void AI()
        {
            // Create a trail of dust particles
            if (Main.rand.NextBool(2))
            {
                Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, 
                    DustID.MagicMirror, Projectile.velocity.X * 0.2f, Projectile.velocity.Y * 0.2f, 
                    100, default, 1.2f);
            }

            // Add a glowing effect
            Lighting.AddLight(Projectile.Center, 0.1f, 0.2f, 0.7f);
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            Main.NewText("Projectile hit detected!", Color.Yellow);
            
            try
            {
                // Don't capture bosses, friendly NPCs, or target dummies
                bool canCapture = (!target.boss && !target.friendly && target.lifeMax > 5 && target.type != NPCID.TargetDummy)

                // Do not capture monsters belonging to other players
                canCapture &= !(target.HasBuff<Buffs.Captured>());
                
                if (canCapture)
                {
                    // You can adjust this capture chance as needed
                    bool shouldCapture = true; // Always capture with the gun
                    
                    if (shouldCapture)
                    {
                        Main.NewText($"Capturing {Lang.GetNPCNameValue(target.type)}!", Color.LightBlue);
                        
                        // Create a captured monster item
                        int capturedMonsterItemType = ModContent.ItemType<CapturedMonster>();
                        
                        // Spawn the captured monster item at the target's position
                        int itemIndex = Item.NewItem(target.GetSource_Death(), target.getRect(), capturedMonsterItemType);
                        
                        // Store the monster data in the newly created item
                        if (Main.item[itemIndex].ModItem is CapturedMonster capturedMonster)
                        {
                            capturedMonster.MonsterType = target.type;
                            capturedMonster.MonsterName = Lang.GetNPCNameValue(target.type);
                            Main.NewText($"Caught {capturedMonster.MonsterName}!", Color.LightGreen);
                        }
                        
                        // Play capture effect
                        SoundEngine.PlaySound(SoundID.Item4, target.position);
                        for (int i = 0; i < 30; i++)
                        {
                            Vector2 dustVelocity = Vector2.UnitX.RotatedBy(Main.rand.NextFloat() * MathHelper.TwoPi) * Main.rand.NextFloat(3f);
                            Dust.NewDust(target.position, target.width, target.height, DustID.MagicMirror, 
                                dustVelocity.X, dustVelocity.Y, 150, default, 1.5f);
                        }
                        
                        // Remove the captured NPC
                        target.life = 0;
                        target.HitEffect();
                        target.active = false;
                    }
                }
                else
                {
                    Main.NewText("This creature cannot be caught!", Color.Red);
                }
            }
            catch (System.Exception ex)
            {
                Main.NewText($"Capture projectile error: {ex.Message}", Color.Red);
            }
        }

        public override void Kill(int timeLeft)
        {
            // Create dust effect when the projectile is destroyed
            for (int i = 0; i < 10; i++)
            {
                Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, 
                    DustID.MagicMirror, 0f, 0f, 100, default, 1f);
            }
            
            SoundEngine.PlaySound(SoundID.Item10, Projectile.position);
        }
        
        // Make the projectile look cooler
        public override Color? GetAlpha(Color lightColor)
        {
            return new Color(100, 200, 255, 150);
        }
    }
}
