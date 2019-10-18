using Microsoft.Xna.Framework;
using Terraria;
using CalamityMod.Projectiles;
using Terraria.ModLoader;
using Terraria.ID;

namespace CalamityMod.Items.Weapons.Melee
{
    public class SubmarineShocker : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Submarine Shocker");
            Tooltip.SetDefault("Enemies release electric sparks on hit");
        }

        public override void SetDefaults()
        {
            item.useStyle = 3;
            item.useTurn = false;
            item.useAnimation = 10;
            item.useTime = 10;
            item.width = 32;
            item.height = 32;
            item.damage = 70;
            item.melee = true;
            item.knockBack = 7f;
            item.UseSound = SoundID.Item1;
            item.useTurn = true;
            item.autoReuse = true;
            item.value = Item.buyPrice(0, 36, 0, 0);
            item.rare = 5;
        }

        public override void MeleeEffects(Player player, Rectangle hitbox)
        {
            if (Main.rand.NextBool(5))
            {
                int dust = Dust.NewDust(new Vector2(hitbox.X, hitbox.Y), hitbox.Width, hitbox.Height, 226);
            }
        }

        public override void OnHitNPC(Player player, NPC target, int damage, float knockback, bool crit)
        {
            Projectile.NewProjectile(target.Center.X, target.Center.Y, 0f, 0f, ModContent.ProjectileType<Spark>(), (int)((float)item.damage * player.meleeDamage), knockback, Main.myPlayer);
        }
    }
}
