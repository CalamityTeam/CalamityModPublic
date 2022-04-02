using CalamityMod.Projectiles.Melee;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

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
            item.useStyle = ItemUseStyleID.Stabbing;
            item.useTurn = false;
            item.useAnimation = 10;
            item.useTime = 10;
            item.width = 32;
            item.height = 32;
            item.damage = 60;
            item.melee = true;
            item.knockBack = 7f;
            item.UseSound = SoundID.Item1;
            item.useTurn = true;
            item.autoReuse = true;
            item.value = Item.buyPrice(0, 36, 0, 0);
            item.rare = ItemRarityID.Pink;
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
            if (crit)
                damage /= 2;

            Projectile.NewProjectile(target.Center, Vector2.Zero, ModContent.ProjectileType<Spark>(), (int)(damage * 0.7f), knockback, Main.myPlayer);
        }

        public override void OnHitPvp(Player player, Player target, int damage, bool crit)
        {
            if (crit)
                damage /= 2;

            Projectile.NewProjectile(target.Center, Vector2.Zero, ModContent.ProjectileType<Spark>(), (int)(damage * 0.7f), item.knockBack, Main.myPlayer);
        }
    }
}
