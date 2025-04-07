using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Monstermon.Content.Items
{ 
	// This is a basic item template.
	// Please see tModLoader's ExampleMod for every other example:
	// https://github.com/tModLoader/tModLoader/tree/stable/ExampleMod
	public class TestSword : ModItem
	{
		// The Display Name and Tooltip of this item can be edited in the 'Localization/en-US_Mods.Monstermon.hjson' file.
		public override void SetDefaults()
		{
			Item.damage = 5;
			Item.DamageType = DamageClass.Melee;
			Item.width = 60;
			Item.height = 20;
			Item.useTime = 10;
			Item.useAnimation = 5;
			Item.useStyle = ItemUseStyleID.Thrust;
			Item.knockBack = 20;
			Item.value = Item.buyPrice(silver: 1);
			Item.rare = ItemRarityID.Cyan;
			Item.UseSound = SoundID.Item1;
			Item.autoReuse = true;
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
