using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;
using Terraria.ID;

namespace CalamityMod.Items.Weapons.Melee
{
    public class DragonRage : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Dragon Rage");
            Tooltip.SetDefault("Fires a spread of five fire waves");
        }

        public override void SetDefaults()
        {
            item.width = 68;
            item.damage = 800;
            item.melee = true;
            item.useAnimation = 25;
            item.useTime = 25;
            item.useTurn = true;
            item.useStyle = 1;
            item.knockBack = 7.5f;
            item.UseSound = SoundID.Item1;
            item.autoReuse = true;
            item.height = 82;
            item.value = Item.buyPrice(1, 80, 0, 0);
            item.rare = 10;
            item.shoot = ModContent.ProjectileType<Projectiles.DragonRage>();
            item.shootSpeed = 14f;
            item.Calamity().postMoonLordRarity = 14;
        }

        public override bool Shoot(Player player, ref Vector2 position, ref float speedX, ref float speedY, ref int type, ref int damage, ref float knockBack)
        {
            for (int index = 0; index < 5; ++index)
            {
                float SpeedX = speedX + (float)Main.rand.Next(-25, 26) * 0.05f;
                float SpeedY = speedY + (float)Main.rand.Next(-25, 26) * 0.05f;
                Projectile.NewProjectile(position.X, position.Y, SpeedX, SpeedY, type, (int)((double)damage * 0.75), knockBack, player.whoAmI, 0.0f, 0.0f);
            }
            return false;
        }

        public override void MeleeEffects(Player player, Rectangle hitbox)
        {
            if (Main.rand.NextBool(3))
            {
                int dust = Dust.NewDust(new Vector2(hitbox.X, hitbox.Y), hitbox.Width, hitbox.Height, 244);
            }
        }

        public override void OnHitNPC(Player player, NPC target, int damage, float knockback, bool crit)
        {
            target.AddBuff(BuffID.Daybreak, 360);
        }
    }
}
