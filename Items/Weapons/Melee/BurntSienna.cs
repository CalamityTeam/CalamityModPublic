using CalamityMod.Projectiles.Healing;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Weapons.Melee
{
    public class BurntSienna : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Burnt Sienna");
            Tooltip.SetDefault("Causes enemies to erupt into healing projectiles on death");
        }

        public override void SetDefaults()
        {
            item.width = 42;
            item.damage = 28;
            item.melee = true;
            item.useAnimation = item.useTime = 21;
            item.useTurn = true;
            item.useStyle = ItemUseStyleID.SwingThrow;
            item.knockBack = 5.5f;
            item.UseSound = SoundID.Item1;
            item.autoReuse = true;
            item.height = 54;
            item.value = CalamityGlobalItem.Rarity1BuyPrice;
            item.rare = ItemRarityID.Blue;
            item.shootSpeed = 5f;
        }

        public override void OnHitNPC(Player player, NPC target, int damage, float knockback, bool crit)
        {
            if (target.life <= 0 && !player.moonLeech)
            {
                float randomSpeedX = Main.rand.Next(3);
                float randomSpeedY = Main.rand.Next(3, 5);
                Projectile.NewProjectile(target.Center.X, target.Center.Y, -randomSpeedX, -randomSpeedY, ModContent.ProjectileType<BurntSiennaProj>(), 0, 0f, player.whoAmI, player.whoAmI);
                Projectile.NewProjectile(target.Center.X, target.Center.Y, randomSpeedX, -randomSpeedY, ModContent.ProjectileType<BurntSiennaProj>(), 0, 0f, player.whoAmI, player.whoAmI);
                Projectile.NewProjectile(target.Center.X, target.Center.Y, 0f, -randomSpeedY, ModContent.ProjectileType<BurntSiennaProj>(), 0, 0f, player.whoAmI, player.whoAmI);
            }
        }

        public override void OnHitPvp(Player player, Player target, int damage, bool crit)
        {
            if (target.statLife <= 0 && !player.moonLeech)
            {
                float randomSpeedX = Main.rand.Next(3);
                float randomSpeedY = Main.rand.Next(3, 5);
                Projectile.NewProjectile(target.Center.X, target.Center.Y, -randomSpeedX, -randomSpeedY, ModContent.ProjectileType<BurntSiennaProj>(), 0, 0f, player.whoAmI, player.whoAmI);
                Projectile.NewProjectile(target.Center.X, target.Center.Y, randomSpeedX, -randomSpeedY, ModContent.ProjectileType<BurntSiennaProj>(), 0, 0f, player.whoAmI, player.whoAmI);
                Projectile.NewProjectile(target.Center.X, target.Center.Y, 0f, -randomSpeedY, ModContent.ProjectileType<BurntSiennaProj>(), 0, 0f, player.whoAmI, player.whoAmI);
            }
        }

        public override void MeleeEffects(Player player, Rectangle hitbox)
        {
            if (Main.rand.NextBool(5))
            {
                int dust = Dust.NewDust(new Vector2(hitbox.X, hitbox.Y), hitbox.Width, hitbox.Height, 246);
            }
        }
    }
}
