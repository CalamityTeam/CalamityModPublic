using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;
using Terraria.DataStructures;
using Terraria.ID;

namespace CalamityMod.Items.Weapons.Melee
{
    public class Lucrecia : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Lucrecia");
            Tooltip.SetDefault("Finesse\n" +
                "Striking an enemy makes you immune for a short time\n" +
                "Using this weapon drains your life\n" +
                "Fires a DNA chain");
        }

        public override void SetDefaults()
        {
            item.useStyle = 3;
            item.useTurn = false;
            item.useAnimation = 25;
            item.useTime = 25;
            item.width = 58;
            item.height = 58;
            item.damage = 90;
            item.melee = true;
            item.knockBack = 8.25f;
            item.UseSound = SoundID.Item1;
            item.useTurn = true;
            item.autoReuse = true;
            item.shoot = ModContent.ProjectileType<DNA>();
            item.shootSpeed = 32f;
            item.value = Item.buyPrice(0, 80, 0, 0);
            item.rare = 8;
        }

        public override bool Shoot(Player player, ref Vector2 position, ref float speedX, ref float speedY, ref int type, ref int damage, ref float knockBack)
        {
            player.statLife -= 5;
            if (player.lifeRegen > 0)
            {
                player.lifeRegen = 0;
            }
            player.lifeRegenTime = 0;
            if (Main.myPlayer == player.whoAmI)
            {
                player.HealEffect(-5, true);
            }
            if (player.statLife <= 0)
            {
                player.KillMe(PlayerDeathReason.ByOther(10), 1000.0, 0, false);
            }
            Projectile.NewProjectile(position.X, position.Y, item.shootSpeed * player.direction, 0f, type, damage, knockBack, player.whoAmI, 0f, 0f);
            return false;
        }

        public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);
            recipe.AddIngredient(null, "CoreofCalamity");
            recipe.AddIngredient(null, "BarofLife", 5);
            recipe.AddIngredient(ItemID.SoulofLight, 5);
            recipe.AddIngredient(ItemID.SoulofNight, 5);
            recipe.AddTile(TileID.MythrilAnvil);
            recipe.SetResult(this);
            recipe.AddRecipe();
        }

        public override void MeleeEffects(Player player, Rectangle hitbox)
        {
            if (Main.rand.NextBool(5))
            {
                int dust = Dust.NewDust(new Vector2(hitbox.X, hitbox.Y), hitbox.Width, hitbox.Height, 234);
            }
        }

        public override void OnHitNPC(Player player, NPC target, int damage, float knockback, bool crit)
        {
            if (!player.immune)
            {
                player.immune = true;
                player.immuneTime = 5;
            }
        }
    }
}
