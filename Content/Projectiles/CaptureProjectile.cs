using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;
using Monstermon.Content.Items;
using Monstermon.Content.Buffs;

namespace Monstermon.Content.Projectiles
{
    public class CaptureProjectile : ModProjectile
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
            // Do not capture bosses, friends or target dummies.
            bool canCapture = !target.boss && !target.friendly && target.lifeMax > 5 && target.type != NPCID.TargetDummy;            // Don't mark bosses, friendly NPCs, or target dummies

            // Do not capture summoned monsters
            canCapture &= !(target.HasBuff<Buffs.Captured>());
            
            if (canCapture)
            {
                // Apply capture-ready buff
                int buffType = ModContent.BuffType<CaptureReady>();
                target.AddBuff(buffType, 60 * 30); // 30 seconds duration
                
                Main.NewText($"{Lang.GetNPCNameValue(target.type)} is now ready to be captured!", Color.LightBlue);
                
                // Visual and sound effects
                SoundEngine.PlaySound(SoundID.Item4, target.position);
                for (int i = 0; i < 15; i++)
                {
                    Vector2 dustVelocity = Vector2.UnitX.RotatedBy(Main.rand.NextFloat() * MathHelper.TwoPi) * Main.rand.NextFloat(3f);
                    Dust.NewDust(target.position, target.width, target.height, DustID.MagicMirror, 
                        dustVelocity.X, dustVelocity.Y, 100, default, 1.2f);
                }
            }
            else
            {
                Main.NewText("This creature cannot be captured!", Color.Red);
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
