using Microsoft.Xna.Framework;
using Terraria;
using CalamityMod.Projectiles;
using Terraria.ModLoader;
using Terraria.ID;

namespace CalamityMod.Items.Weapons.Magic
{
    public class AstralStaff : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Astral Staff");
            Tooltip.SetDefault("Summons a large crystal from the sky that has a large area of effect on impact.");
            Item.staff[item.type] = true;
        }

        public override void SetDefaults()
        {
            item.damage = 270;
            item.crit += 15;
            item.magic = true;
            item.mana = 26;
            item.width = 86;
            item.height = 72;
            item.useTime = 35;
            item.useAnimation = 35;
            item.useStyle = 5;
            item.noMelee = true;
            item.knockBack = 5f;
            item.value = Item.buyPrice(0, 95, 0, 0);
            item.rare = 9;
            item.UseSound = SoundID.Item105;
            item.autoReuse = true;
            item.shoot = ModContent.ProjectileType<AstralCrystal>();
            item.shootSpeed = 15f;
        }

        public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);
            recipe.AddIngredient(null, "AstralBar", 6);
            recipe.AddTile(TileID.LunarCraftingStation);
            recipe.SetResult(this);
            recipe.AddRecipe();
        }

        public override bool Shoot(Player player, ref Vector2 position, ref float speedX, ref float speedY, ref int type, ref int damage, ref float knockBack)
        {
            Vector2 spawnPos = new Vector2(player.MountedCenter.X + Main.rand.Next(-200, 201), player.MountedCenter.Y - 600f);
            Vector2 targetPos = Main.MouseWorld + new Vector2(Main.rand.Next(-30, 31), Main.rand.Next(-30, 31));
            Vector2 velocity = targetPos - spawnPos;
            velocity.Normalize();
            velocity *= 13f;

            int p = Projectile.NewProjectile(spawnPos, velocity, type, damage, knockBack, player.whoAmI);
            Main.projectile[p].ai[0] = targetPos.Y - 120;

            return false;
        }
    }
}
