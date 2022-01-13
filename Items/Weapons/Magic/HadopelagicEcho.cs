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
            Tooltip.SetDefault("Fires a string of bouncing sound waves that become stronger as they travel\n" +
            "Sound waves echo additional sound waves on enemy hits");
        }

        public override void SetDefaults()
        {
            item.damage = 500;
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
            item.shootSpeed = 10f;
            item.shoot = ModContent.ProjectileType<HadopelagicEchoSoundwave>();
            item.Calamity().customRarity = CalamityRarity.Violet;
        }

        public override Vector2? HoldoutOffset()
        {
            return new Vector2(-5, 0);
        }

        public override bool Shoot(Player player, ref Vector2 position, ref float speedX, ref float speedY, ref int type, ref int damage, ref float knockBack)
        {
            Projectile.NewProjectile(position.X, position.Y, speedX, speedY, type, damage, knockBack, player.whoAmI, counter);
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
            recipe.AddIngredient(ModContent.ItemType<AuricBar>(), 5);
            recipe.AddTile(ModContent.TileType<CosmicAnvil>());
            recipe.SetResult(this);
            recipe.AddRecipe();
        }
    }
}
