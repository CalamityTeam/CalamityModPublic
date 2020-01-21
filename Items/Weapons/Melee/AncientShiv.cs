using CalamityMod.Projectiles.Melee;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Weapons.Melee
{
    public class AncientShiv : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Ancient Shiv");
            Tooltip.SetDefault("Enemies release a blue aura cloud on death");
        }

        public override void SetDefaults()
        {
            item.useStyle = 3;
            item.useTurn = false;
            item.useAnimation = 12;
            item.useTime = 12;
            item.width = 30;
            item.height = 30;
            item.damage = 35;
            item.melee = true;
            item.knockBack = 6f;
            item.UseSound = SoundID.Item1;
            item.useTurn = true;
            item.autoReuse = true;
            item.value = Item.buyPrice(0, 4, 0, 0);
            item.rare = 3;
        }

        public override void MeleeEffects(Player player, Rectangle hitbox)
        {
            if (Main.rand.NextBool(5))
            {
                int dust = Dust.NewDust(new Vector2(hitbox.X, hitbox.Y), hitbox.Width, hitbox.Height, 15);
            }
        }

        public override void OnHitNPC(Player player, NPC target, int damage, float knockback, bool crit)
        {
            if (target.life <= 0)
            {
                Projectile.NewProjectile(target.Center.X, target.Center.Y, 0f, 0f, ModContent.ProjectileType<BlueAura>(), (int)(item.damage * (player.allDamage + player.meleeDamage - 1f)), knockback, Main.myPlayer);
            }
        }

        public override void OnHitPvp(Player player, Player target, int damage, bool crit)
        {
            if (target.statLife <= 0)
            {
                Projectile.NewProjectile(target.Center.X, target.Center.Y, 0f, 0f, ModContent.ProjectileType<BlueAura>(), (int)(item.damage * (player.allDamage + player.meleeDamage - 1f)), item.knockBack, Main.myPlayer);
            }
        }
    }
}
