using CalamityMod.Items.Materials;
using CalamityMod.Projectiles.Hybrid;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Weapons.Melee
{
    public class MangroveChakramMelee : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Mangrove Chakram");
        }

        public override void SetDefaults()
        {
            item.width = 38;
            item.damage = 84;
            item.noMelee = true;
            item.noUseGraphic = true;
            item.autoReuse = true;
            item.useAnimation = 14;
            item.useStyle = 1;
            item.useTime = 14;
            item.knockBack = 7.5f;
            item.UseSound = SoundID.Item1;
            item.melee = true;
            item.height = 38;
            item.value = Item.buyPrice(0, 60, 0, 0);
            item.rare = 7;
            item.shoot = ModContent.ProjectileType<MangroveChakramProjectile>();
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
            recipe.AddIngredient(ModContent.ItemType<DraedonBar>(), 7);
            recipe.AddTile(TileID.MythrilAnvil);
            recipe.SetResult(this);
            recipe.AddRecipe();
        }
    }
}
