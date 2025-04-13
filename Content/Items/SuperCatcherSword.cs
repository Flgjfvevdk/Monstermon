using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;
using Terraria.Audio;
using System;

namespace Monstermon.Content.Items
{ 
	public class SuperCatcherSword : ModItem
	{
        // Simple toggle for the capture mode
        public static bool InstantCaptureMode = false;
        
        public override void SetStaticDefaults()
        {
            // This is now primarily for display purposes
            ItemID.Sets.CatchingTool[Item.type] = true;
        }

		public override void SetDefaults()
		{
			Item.width = 40;
			Item.height = 40;
            
            // Set to 1 damage instead of 0 to ensure hit detection works
            Item.damage = 1; // Tiny damage to ensure OnHitNPC fires
            Item.DamageType = DamageClass.Melee;
			Item.useTime = 25;
			Item.useAnimation = 25;
			Item.useStyle = ItemUseStyleID.Swing;
			Item.knockBack = 0;
			Item.value = Item.buyPrice(gold: 1);
			Item.rare = ItemRarityID.Green;
			Item.UseSound = SoundID.Item1;
			Item.autoReuse = false;
            Item.noMelee = false; // We need melee hits to trigger
		}

		public override void AddRecipes()
		{
			Recipe recipe = CreateRecipe();
			recipe.AddIngredient(ItemID.Wood, 1);
			recipe.AddTile(TileID.WorkBenches);
			recipe.Register();
		}
        
        // Allow right-clicking to toggle mode
        public override bool AltFunctionUse(Player player)
        {
            return true;
        }

        public override bool CanUseItem(Player player)
        {
            // Toggle mode on right-click
            if (player.altFunctionUse == 2)
            {
                InstantCaptureMode = !InstantCaptureMode;
                string message = InstantCaptureMode ? 
                    "Instant-Catch Mode: ON" : 
                    "Instant-Catch Mode: OFF";
                Main.NewText(message, InstantCaptureMode ? Color.LightGreen : Color.Orange);
                return false; // Don't use the item on toggle
            }
            return true;
        }
        
        // Prevent actual damage from being dealt
        public override void ModifyHitNPC(Player player, NPC target, ref NPC.HitModifiers modifiers)
        {
            // Cancel all damage - we're just capturing, not hurting
            modifiers.FinalDamage *= 0; 
            
            // Debug: Log when hit calculation happens
            Main.NewText("Hit calculation triggered", Color.Yellow);
        }
        
        public override void OnHitNPC(Player player, NPC target, NPC.HitInfo hit, int damageDone)
        {
			Main.NewText("Capture attempt triggered", Color.Orange);
            
            try
            {
                // Don't capture bosses, friendly NPCs, or target dummies
                if (!target.boss && !target.friendly && target.lifeMax > 5 && target.type != NPCID.TargetDummy)
                {
                    bool shouldCapture = InstantCaptureMode || Main.rand.NextBool(3); // 100% in instant mode, 33% otherwise
                    
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
                        
                        // Remove the captured NPC without triggering death effects
                        target.life = 0;
                        target.HitEffect();
                        target.active = false;
                    }
                    else
                    {
                        Main.NewText("The monster escaped!", Color.Orange);
                    }
                }
                else
                {
                    Main.NewText("This creature cannot be caught!", Color.Red);
                }
            }
            catch (Exception ex)
            {
                // Error handling for troubleshooting
                Main.NewText($"Error: {ex.Message}", Color.Red);
            }
        }
        
        // This helps show which NPCs can be captured
        public override bool? CanCatchNPC(NPC target, Player player)
        {
            return !target.boss && !target.friendly && target.lifeMax > 5 && target.type != NPCID.TargetDummy;
        }
	}
}
