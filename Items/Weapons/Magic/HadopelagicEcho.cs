using CalamityMod.Projectiles.Magic;
using CalamityMod.Items.Materials;
using CalamityMod.Tiles.Furniture.CraftingStations;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;
using Terraria.ID;

namespace CalamityMod.Items.Weapons.Magic
{
    public class HadopelagicEcho : ModItem
    {
		private int counter = 0;

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Hadopelagic Echo");
            Tooltip.SetDefault("Fires a string of bouncing sound waves\n" +
			"Sound waves fired later in the chain deal more damage\n" +
			"Sound waves echo additional sound waves on enemy hits\n" +
			"Sound waves deal more damage the more they pierce");
        }

        public override void SetDefaults()
        {
            item.damage = 769;
            item.magic = true;
            item.mana = 15;
            item.width = 60;
            item.height = 60;
            item.useTime = 8;
            item.reuseDelay = 20;
            item.useAnimation = 30;
            item.useStyle = ItemUseStyleID.HoldingOut;
            item.noMelee = true;
            item.knockBack = 1.5f;
            item.value = Item.buyPrice(2, 50, 0, 0);
            item.rare = ItemRarityID.Red;
            item.autoReuse = true;
            item.shootSpeed = 20f;
            item.shoot = ModContent.ProjectileType<HadopelagicEchoSoundwave>();
            item.Calamity().customRarity = CalamityRarity.Violet;
        }

        public override Vector2? HoldoutOffset()
        {
            return new Vector2(-5, 0);
        }

        public override bool Shoot(Player player, ref Vector2 position, ref float speedX, ref float speedY, ref int type, ref int damage, ref float knockBack)
        {
			float damageMult = 1f;
			if (counter == 1)
				damageMult = 1.1f;
			if (counter == 2)
				damageMult = 1.2f;
			if (counter == 3)
				damageMult = 1.35f;
			if (counter == 4)
				damageMult = 1.5f;
            Projectile.NewProjectile(position.X, position.Y, speedX, speedY, type, (int)(damage * damageMult), knockBack, player.whoAmI, counter, 0f);
			counter++;
			if (counter >= 5)
                counter = 0;
            return false;
        }

        public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);
            recipe.AddIngredient(ModContent.ItemType<EidolicWail>());
            recipe.AddIngredient(ModContent.ItemType<ReaperTooth>(), 20);
            recipe.AddIngredient(ModContent.ItemType<DepthCells>(), 20);
            recipe.AddIngredient(ModContent.ItemType<Lumenite>(), 20);
			recipe.AddIngredient(ModContent.ItemType<AuricBar>(), 4);
			recipe.AddTile(ModContent.TileType<DraedonsForge>());
            recipe.SetResult(this);
            recipe.AddRecipe();
        }
    }
}
