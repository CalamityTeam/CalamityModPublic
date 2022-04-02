using CalamityMod.Projectiles.Melee;
using CalamityMod.Items.Materials;
using CalamityMod.Items.Placeables.Ores;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Weapons.Melee
{
    public class CrescentMoon : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Crescent Moon");
            Tooltip.SetDefault("People wanted the moon, let's bring the moon to them.\n" +
            "Fires a whip sword that summons homing crescent moons");
        }

        public override void SetDefaults()
        {
            item.width = 16;
            item.height = 16;
            item.damage = 300;
            item.noMelee = true;
            item.noUseGraphic = true;
            item.channel = true;
            item.autoReuse = true;
            item.melee = true;
            item.useAnimation = 18;
            item.useTime = 18;
            item.useStyle = ItemUseStyleID.HoldingOut;
            item.knockBack = 4f;
            item.UseSound = SoundID.Item82;
            item.value = Item.buyPrice(1, 40, 0, 0);
            item.rare = ItemRarityID.Red;
            item.Calamity().customRarity = CalamityRarity.PureGreen;
            item.shootSpeed = 24f;
            item.shoot = ModContent.ProjectileType<CrescentMoonFlail>();
        }

        public override bool Shoot(Player player, ref Vector2 position, ref float speedX, ref float speedY, ref int type, ref int damage, ref float knockBack)
        {
            float ai3 = (Main.rand.NextFloat() - 0.5f) * 0.7853982f; //0.5
            Projectile.NewProjectile(position.X, position.Y, speedX, speedY, type, damage, knockBack, player.whoAmI, 0f, ai3);
            return false;
        }

        public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);
            recipe.AddIngredient(ModContent.ItemType<Lumenite>(), 8);
            recipe.AddIngredient(ModContent.ItemType<RuinousSoul>(), 3);
            recipe.AddIngredient(ModContent.ItemType<ExodiumClusterOre>(), 16);
            recipe.AddTile(TileID.LunarCraftingStation);
            recipe.SetResult(this);
            recipe.AddRecipe();
        }
    }
}
