using Terraria;
using Terraria.ID;
using Terraria.ModLoader; using CalamityMod.Buffs; using CalamityMod.Items; using CalamityMod.NPCs; using CalamityMod.Projectiles; using CalamityMod.Tiles; using CalamityMod.Walls;

namespace CalamityMod.Items
{
    public class Contagion : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Contagion");
            Tooltip.SetDefault("Fires contagion arrows that leave exploding orbs behind as they travel");
        }

        public override void SetDefaults()
        {
            item.damage = 4000;
            item.ranged = true;
            item.width = 22;
            item.height = 50;
            item.useTime = 20;
            item.useAnimation = 20;
            item.useStyle = 5;
            item.noMelee = true;
            item.noUseGraphic = true;
            item.channel = true;
            item.knockBack = 5f;
            item.value = Item.buyPrice(5, 0, 0, 0);
            item.rare = 10;
            item.autoReuse = true;
            item.shoot = ModContent.ProjectileType<Contagion>();
            item.shootSpeed = 20f;
            item.useAmmo = 40;
            item.Calamity().postMoonLordRarity = 16;
        }

        public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);
            recipe.AddIngredient(null, "ShadowspecBar", 5);
            recipe.AddIngredient(ItemID.Phantasm);
            recipe.AddTile(null, "DraedonsForge");
            recipe.SetResult(this);
            recipe.AddRecipe();
        }

        public override bool Shoot(Player player, ref Microsoft.Xna.Framework.Vector2 position, ref float speedX, ref float speedY, ref int type, ref int damage, ref float knockBack)
        {
            Projectile.NewProjectile(position.X, position.Y, speedX, speedY, ModContent.ProjectileType<Contagion>(), damage, knockBack, player.whoAmI, 0.0f, 0.0f);
            return false;
        }
    }
}
