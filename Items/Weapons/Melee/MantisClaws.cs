using CalamityMod.Projectiles.Typeless;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Weapons.Melee
{
    public class MantisClaws : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Mantis Claws");
            Tooltip.SetDefault("Explodes on enemy hits");
        }

        public override void SetDefaults()
        {
            item.width = 26;
            item.damage = 88;
            item.melee = true;
            item.useAnimation = 6;
            item.useStyle = ItemUseStyleID.SwingThrow;
            item.useTime = 6;
            item.useTurn = true;
            item.knockBack = 7f;
            item.UseSound = SoundID.Item1;
            item.autoReuse = true;
            item.height = 20;
            item.value = Item.buyPrice(0, 60, 0, 0);
            item.rare = ItemRarityID.Lime;
        }

        public override void MeleeEffects(Player player, Rectangle hitbox)
        {
            if (Main.rand.NextBool(4))
            {
                int dust = Dust.NewDust(new Vector2(hitbox.X, hitbox.Y), hitbox.Width, hitbox.Height, 33);
            }
        }

        public override void OnHitNPC(Player player, NPC target, int damage, float knockback, bool crit)
        {
            //does no damage. Explosion is visual
            Projectile.NewProjectile(target.Center.X, target.Center.Y, 0f, 0f, ModContent.ProjectileType<FuckYou>(), 0, 0f, player.whoAmI, 0f, 0.85f + Main.rand.NextFloat() * 1.15f);
        }

        public override void OnHitPvp(Player player, Player target, int damage, bool crit)
        {
            //does no damage. Explosion is visual
            int boom = Projectile.NewProjectile(target.Center.X, target.Center.Y, 0f, 0f, ModContent.ProjectileType<FuckYou>(), 0, 0f, player.whoAmI, 0f, 0.85f + Main.rand.NextFloat() * 1.15f);
        }
    }
}
