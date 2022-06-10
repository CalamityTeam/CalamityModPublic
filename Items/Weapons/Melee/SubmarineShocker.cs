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
            SacrificeTotal = 1;
        }

        public override void SetDefaults()
        {
            Item.useStyle = ItemUseStyleID.Thrust;
            Item.useTurn = false;
            Item.useAnimation = 10;
            Item.useTime = 10;
            Item.width = 32;
            Item.height = 32;
            Item.damage = 60;
            Item.DamageType = DamageClass.Melee;
            Item.knockBack = 7f;
            Item.UseSound = SoundID.Item1;
            Item.useTurn = true;
            Item.autoReuse = true;
            Item.value = Item.buyPrice(0, 36, 0, 0);
            Item.rare = ItemRarityID.Pink;
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
            var source = player.GetSource_ItemUse(Item);
            if (crit)
                damage /= 2;

            Projectile.NewProjectile(source, target.Center, Vector2.Zero, ModContent.ProjectileType<Spark>(), (int)(damage * 0.7f), knockback, Main.myPlayer);
        }

        public override void OnHitPvp(Player player, Player target, int damage, bool crit)
        {
            var source = player.GetSource_ItemUse(Item);
            if (crit)
                damage /= 2;

            Projectile.NewProjectile(source, target.Center, Vector2.Zero, ModContent.ProjectileType<Spark>(), (int)(damage * 0.7f), Item.knockBack, Main.myPlayer);
        }
    }
}
