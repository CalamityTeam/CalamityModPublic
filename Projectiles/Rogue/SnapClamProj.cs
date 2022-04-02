using CalamityMod.Buffs.DamageOverTime;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
namespace CalamityMod.Projectiles.Rogue
{
    public class SnapClamProj : ModProjectile
    {
        public int clamCounter = 0;
        public bool openClam = true;
        public bool onEnemy = false;

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Snap Clam");
            Main.projFrames[projectile.type] = 2;
        }

        public override void SetDefaults()
        {
            projectile.width = 20;
            projectile.height = 13;
            projectile.friendly = true;
            projectile.Calamity().rogue = true;
            projectile.ignoreWater = true;
            projectile.penetrate = -1;
        }

        public override void AI()
        {
            if (openClam && !onEnemy)
            {
                ++clamCounter;
                if (clamCounter >= 30)
                {
                    openClam = false;
                    projectile.damage = (int)(projectile.damage * 0.8);
                }
            }
            if (projectile.ai[0] == 0f)
            {
                projectile.velocity.X = projectile.velocity.X * 0.99f;
                projectile.velocity.Y = projectile.velocity.Y + 0.15f;
                projectile.rotation += 0.4f * (float)projectile.direction;
                projectile.spriteDirection = projectile.direction;
            }
            //Sticky Behaviour
            projectile.StickyProjAI(15);
            if (openClam && !onEnemy)
            {
                projectile.frame = 1;
            }
            else
            {
                projectile.frame = 0;
            }
        }

        public override void ModifyHitNPC(NPC target, ref int damage, ref float knockback, ref bool crit, ref int hitDirection)
        {
            if (openClam)
            {
                onEnemy = true;
                projectile.ModifyHitNPCSticky(2, false);
            }
        }

        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            if (targetHitbox.Width > 8 && targetHitbox.Height > 8)
            {
                targetHitbox.Inflate(-targetHitbox.Width / 8, -targetHitbox.Height / 8);
            }
            return null;
        }

        public override void Kill(int timeLeft)
        {
            Main.PlaySound(SoundID.Item10, projectile.position);
            projectile.position.X = projectile.position.X + (float)(projectile.width / 2);
            projectile.position.Y = projectile.position.Y + (float)(projectile.height / 2);
            projectile.width = 13;
            projectile.height = 20;
            projectile.position.X = projectile.position.X - (float)(projectile.width / 2);
            projectile.position.Y = projectile.position.Y - (float)(projectile.height / 2);
            for (int num194 = 0; num194 < 20; num194++)
            {
                int num195 = Dust.NewDust(new Vector2(projectile.position.X, projectile.position.Y), projectile.width, projectile.height, 14, 0f, 0f, 0, new Color(115, 124, 124), 1f);
                Main.dust[num195].noGravity = true;
                Main.dust[num195].velocity *= 2f;
            }
        }

        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
            target.AddBuff(ModContent.BuffType<SnapClamDebuff>(), 240);
        }
    }
}
