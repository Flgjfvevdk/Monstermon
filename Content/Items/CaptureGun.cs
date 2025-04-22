using System.Diagnostics;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Audio;
using Microsoft.Xna.Framework;

namespace Monstermon.Content.Items
{
    // This is a basic item template.
    // Please see tModLoader's ExampleMod for every other example:
    // https://github.com/tModLoader/tModLoader/tree/stable/ExampleMod
    public class CaptureGun : ModItem
    {
        // The Display Name and Tooltip of this item can be edited in the 'Localization/en-US_Mods.Monstermon.hjson' file.
        public override void SetDefaults()
        {
            Item.damage = 1;
            Item.DamageType = DamageClass.Ranged;
            Item.width = 60;
            Item.height = 20;
            Item.useTime = 30;
            Item.useAnimation = 30;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.knockBack = 2;
            Item.value = Item.buyPrice(silver: 1);
            Item.rare = ItemRarityID.Cyan;
            Item.UseSound = SoundID.Item11; // Use a different sound for the capture gun
            Item.autoReuse = false;

            Item.noMelee = true;

            // Use our custom capture projectile instead of the default bullet
            Item.shoot = ModContent.ProjectileType<Projectiles.CaptureProjectile>();
            Item.shootSpeed = 12f;
            Item.useAmmo = AmmoID.None;
        }

        public override void AddRecipes()
        {
            Recipe recipe = CreateRecipe();
            recipe.AddIngredient(ItemID.Wood, 2);
            recipe.AddTile(TileID.WorkBenches);
            recipe.Register();
        }
    }
}
