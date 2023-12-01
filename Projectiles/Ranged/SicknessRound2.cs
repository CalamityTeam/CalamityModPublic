using CalamityMod.Buffs.DamageOverTime;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
namespace CalamityMod.Projectiles.Ranged
{
    public class SicknessRound2 : ModProjectile, ILocalizedModType
    {
        public new string LocalizationCategory => "Projectiles.Ranged";
        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 3;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 0;
        }

        public override void SetDefaults()
        {
            Projectile.width = 4;
            Projectile.height = 4;
            Projectile.aiStyle = ProjAIStyleID.Arrow;
            Projectile.friendly = true;
            Projectile.DamageType = DamageClass.Ranged;
            Projectile.penetrate = 1;
            Projectile.timeLeft = 180;
            Projectile.light = 0.15f;
            Projectile.extraUpdates = 1;
            AIType = ProjectileID.WoodenArrowFriendly;
        }

        public override bool? CanHitNPC(NPC target) => Projectile.timeLeft < 150 && target.CanBeChasedBy(Projectile);

        public override bool PreDraw(ref Color lightColor)
        {
            CalamityUtils.DrawAfterimagesCentered(Projectile, ProjectileID.Sets.TrailingMode[Projectile.type], lightColor, 1);
            return false;
        }

        public override bool PreAI()
        {
            Vector2 dspeed = -Projectile.velocity * 0.5f;
            float x2 = Projectile.Center.X - Projectile.velocity.X / 10f;
            float y2 = Projectile.Center.Y - Projectile.velocity.Y / 10f;
            int dust = Dust.NewDust(new Vector2(x2, y2), 1, 1, 107, 0f, 0f, 0, default, 1f);
            Main.dust[dust].alpha = Projectile.alpha;
            Main.dust[dust].position.X = x2;
            Main.dust[dust].position.Y = y2;
            Main.dust[dust].velocity = dspeed;
            Main.dust[dust].noGravity = true;

            //Rotation
            Projectile.spriteDirection = Projectile.direction = (Projectile.velocity.X > 0).ToDirectionInt();
            Projectile.rotation = Projectile.velocity.ToRotation() + (Projectile.spriteDirection == 1 ? 0f : MathHelper.Pi) + MathHelper.ToRadians(90) * Projectile.direction;

            if (Projectile.timeLeft < 150)
                CalamityUtils.HomeInOnNPC(Projectile, !Projectile.tileCollide, 450f, 12f, 25f);

            return false;
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            target.AddBuff(ModContent.BuffType<Plague>(), 120);
        }

        public override void OnKill(int timeLeft)
        {
            if (Projectile.owner == Main.myPlayer)
                Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center, Vector2.Zero, ModContent.ProjectileType<Sickness>(), Projectile.damage, Projectile.knockBack, Projectile.owner);
        }
    }
}
