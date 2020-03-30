using CalamityMod.Items.Materials;
using CalamityMod.Projectiles.Hybrid;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Weapons.Melee
{
    public class FlameScytheMelee : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Subduction Slicer");
            Tooltip.SetDefault("Throws a scythe that explodes on enemy hits");
        }

        public override void SetDefaults()
        {
            item.width = 50;
            item.damage = 100;
            item.noMelee = true;
            item.noUseGraphic = true;
            item.autoReuse = true;
            item.useAnimation = 19;
            item.useStyle = 1;
            item.useTime = 19;
            item.knockBack = 8.5f;
            item.UseSound = SoundID.Item1;
            item.melee = true;
            item.height = 48;
            item.value = Item.buyPrice(0, 80, 0, 0);
            item.rare = 8;
            item.shoot = ModContent.ProjectileType<FlameScytheProjectile>();
            item.shootSpeed = 16f;
        }

        public override bool Shoot(Player player, ref Vector2 position, ref float speedX, ref float speedY, ref int type, ref int damage, ref float knockBack)
        {
            int proj = Projectile.NewProjectile(position, new Vector2(speedX, speedY), type, damage, knockBack, player.whoAmI);
            Main.projectile[proj].Calamity().forceMelee = true;
            return false;
        }

        public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);
            recipe.AddIngredient(ModContent.ItemType<CruptixBar>(), 9);
            recipe.AddTile(TileID.MythrilAnvil);
            recipe.SetResult(this);
            recipe.AddRecipe();
        }
    }
}
