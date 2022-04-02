using CalamityMod.Projectiles.Melee;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Weapons.Melee
{
    public class StormSaber : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Storm Saber");
            Tooltip.SetDefault("Fires two homing storm beams\n" +
            "One from blade and one from the sky");
        }

        public override void SetDefaults()
        {
            item.width = 58;
            item.damage = 66;
            item.melee = true;
            item.useAnimation = 23;
            item.useTime = 23;
            item.useTurn = true;
            item.useStyle = ItemUseStyleID.SwingThrow;
            item.knockBack = 6f;
            item.UseSound = SoundID.Item1;
            item.autoReuse = true;
            item.height = 64;
            item.value = Item.buyPrice(0, 36, 0, 0);
            item.rare = ItemRarityID.Pink;
            item.shoot = ModContent.ProjectileType<StormBeam>();
            item.shootSpeed = 12f;
        }

        public override bool Shoot(Player player, ref Vector2 position, ref float speedX, ref float speedY, ref int type, ref int damage, ref float knockBack)
        {
            Projectile.NewProjectile(position.X, position.Y, speedX, speedY, type, (int)(damage * 0.8), knockBack, player.whoAmI, 0f, 0f);

            Vector2 spawnPos = new Vector2(player.MountedCenter.X + Main.rand.Next(-200, 201), player.MountedCenter.Y - 600f);
            Vector2 targetPos = Main.MouseWorld + new Vector2(Main.rand.Next(-30, 31), Main.rand.Next(-30, 31));
            Vector2 velocity = targetPos - spawnPos;
            velocity.Normalize();
            velocity *= 13f;

            Projectile.NewProjectile(spawnPos, velocity, type, (int)(damage * 0.6), knockBack, player.whoAmI);
            return false;
        }

        public override void MeleeEffects(Player player, Rectangle hitbox)
        {
            if (Main.rand.NextBool(5))
            {
                int num250 = Dust.NewDust(new Vector2((float)hitbox.X, (float)hitbox.Y), hitbox.Width, hitbox.Height, 187, (float)(player.direction * 2), 0f, 150, default, 1.3f);
                Main.dust[num250].velocity *= 0.2f;
            }
        }
    }
}
