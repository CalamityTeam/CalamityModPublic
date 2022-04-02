using CalamityMod.Buffs.StatDebuffs;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;
using Terraria.ID;

namespace CalamityMod.Projectiles.Ranged
{
    public class BigCoral : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Big Coral");
        }

        public override void SetDefaults()
        {
            projectile.width = 18;
            projectile.height = 34;
            projectile.friendly = true;
            projectile.ranged = true;
            projectile.penetrate = 1;
            projectile.aiStyle = 1;
            projectile.Calamity().pointBlankShotDuration = CalamityGlobalProjectile.basePointBlankShotDuration;
        }

        public override void AI()
        {
            projectile.velocity.X *= 0.999f;
            projectile.velocity.Y = projectile.velocity.Y + 0.025f;
            projectile.rotation = projectile.velocity.ToRotation();
            if (projectile.direction == -1)
                projectile.rotation += MathHelper.Pi;
        }

        public override void Kill(int timeLeft)
        {
            Main.PlaySound(SoundID.Item, (int)projectile.position.X, (int)projectile.position.Y, 10);
            projectile.position.X = projectile.position.X + (float)(projectile.width / 2);
            projectile.position.Y = projectile.position.Y + (float)(projectile.height / 2);
            projectile.width = 32;
            projectile.height = 32;
            projectile.position.X = projectile.position.X - (float)(projectile.width / 2);
            projectile.position.Y = projectile.position.Y - (float)(projectile.height / 2);
            for (int num621 = 0; num621 < 10; num621++)
            {
                int num195 = Dust.NewDust(new Vector2(projectile.position.X, projectile.position.Y), projectile.width, projectile.height, 51, projectile.oldVelocity.X / 4, projectile.oldVelocity.Y / 4, 0, new Color(234, 183, 100), 1.5f);
                Main.dust[num195].noGravity = true;
                Main.dust[num195].velocity *= 3f;
            }
        }

        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
            target.AddBuff(ModContent.BuffType<Eutrophication>(), 30);
        }

        public override void OnHitPvp(Player target, int damage, bool crit)
        {
            target.AddBuff(ModContent.BuffType<Eutrophication>(), 30);
        }
    }
}
