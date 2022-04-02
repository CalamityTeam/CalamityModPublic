using CalamityMod.Items.Materials;
using CalamityMod.Projectiles.Summon;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Weapons.Summon
{
    public class DaedalusGolemStaff : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Daedalus Golem Staff");
            Tooltip.SetDefault("Summons a Daedalus Golem soldier that fires pellets and electricity\n" +
                               "Shining god of greed"); // Funny Hollow Knight reference.
        }

        public override void SetDefaults()
        {
            item.damage = 70;
            item.mana = 10;
            item.width = item.height = 32;
            item.useTime = item.useAnimation = 25;
            item.useStyle = ItemUseStyleID.SwingThrow;
            item.noMelee = true;
            item.knockBack = 3f;
            item.UseSound = SoundID.Item67;
            item.autoReuse = true;
            item.shoot = ModContent.ProjectileType<DaedalusGolem>();
            item.shootSpeed = 10f;
            item.summon = true;

            item.value = CalamityGlobalItem.Rarity5BuyPrice;
            item.rare = ItemRarityID.Pink;
        }
        public override bool Shoot(Player player, ref Vector2 position, ref float speedX, ref float speedY, ref int type, ref int damage, ref float knockBack)
        {
            Point mouseTileCoords = Main.MouseWorld.ToTileCoordinates();
            if (!CalamityUtils.ParanoidTileRetrieval(mouseTileCoords.X, mouseTileCoords.Y).IsTileSolidGround())
                Projectile.NewProjectile(Main.MouseWorld, Vector2.UnitY * 4f, type, damage, knockBack, player.whoAmI, 0f, 0f);
            return false;
        }

        public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);
            recipe.AddIngredient(ModContent.ItemType<VerstaltiteBar>(), 12);
            recipe.AddTile(TileID.MythrilAnvil);
            recipe.SetResult(this);
            recipe.AddRecipe();
        }
    }
}
