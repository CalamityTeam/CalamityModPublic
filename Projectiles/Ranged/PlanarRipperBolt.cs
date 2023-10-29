using CalamityMod.CalPlayer;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Audio;

namespace CalamityMod.Projectiles.Ranged
{
    public class PlanarRipperBolt : ModProjectile, ILocalizedModType
    {
        public new string LocalizationCategory => "Projectiles.Ranged";
        public override string Texture => "CalamityMod/Projectiles/Rogue/ShockGrenadeBolt";

        public static int frameWidth = 12;
        public static int frameHeight = 26;

        public override void SetStaticDefaults()
        {
            Main.projFrames[Projectile.type] = 4;
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 8;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 0;
        }

        public override void SetDefaults()
        {
            Projectile.width = 10;
            Projectile.height = 10;
            Projectile.friendly = true;
            Projectile.extraUpdates = 10;
            Projectile.timeLeft = 600;
            Projectile.DamageType = DamageClass.Ranged;
            Projectile.aiStyle = ProjAIStyleID.Arrow;
            AIType = ProjectileID.BulletHighVelocity;
            Projectile.Calamity().pointBlankShotDuration = CalamityGlobalProjectile.DefaultPointBlankDuration;
        }

        public override void AI()
        {
            Projectile.frameCounter++;
            if (Projectile.frameCounter > 6)
            {
                Projectile.frame++;
                Projectile.frameCounter = 0;
            }
            if (Projectile.frame >= Main.projFrames[Projectile.type])
            {
                Projectile.frame = 0;
            }
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            CalamityPlayer modPlayer = Main.player[Projectile.owner].Calamity();
            target.AddBuff(BuffID.Electrified, 180);
            if (Projectile.owner == Main.myPlayer)
            {
                if (target.life <= 0)
                {
                    Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center.X, Projectile.Center.Y, 0f, 0f, ModContent.ProjectileType<PlanarRipperExplosion>(), Projectile.damage, Projectile.knockBack, Projectile.owner, 0f, 0f);
                }
                if (hit.Crit)
                {
                    if (modPlayer.planarSpeedBoost < 20)
                    {
                        modPlayer.planarSpeedBoost++;
                    }
                }
            }
        }

        public override void OnHitPlayer(Player target, Player.HurtInfo info)
        {
            CalamityPlayer modPlayer = Main.player[Projectile.owner].Calamity();
            target.AddBuff(BuffID.Electrified, 180);
            if (Projectile.owner == Main.myPlayer)
            {
                if (target.statLife <= 0)
                {
                    Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center.X, Projectile.Center.Y, 0f, 0f, ModContent.ProjectileType<PlanarRipperExplosion>(), Projectile.damage, Projectile.knockBack, Projectile.owner, 0f, 0f);
                }
                if (modPlayer.planarSpeedBoost < 20)
                {
                    modPlayer.planarSpeedBoost++;
                }
            }
        }

        public override bool PreDraw(ref Color lightColor)
        {
            CalamityUtils.DrawAfterimagesCentered(Projectile, ProjectileID.Sets.TrailingMode[Projectile.type], lightColor, 2);
            return false;
        }

        public override void OnKill(int timeLeft)
        {
            Projectile.position = Projectile.Center;
            Projectile.width = Projectile.height = 10;
            Projectile.position.X = Projectile.position.X - (float)(Projectile.width / 2);
            Projectile.position.Y = Projectile.position.Y - (float)(Projectile.height / 2);
            Projectile.maxPenetrate = -1;
            Projectile.penetrate = -1;
            Projectile.damage = (int)(Projectile.damage * 0.6f);
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 10;
            Projectile.Damage();

            SoundStyle sound = Main.rand.NextBool() ? SoundID.Item93 : SoundID.Item92;
            SoundEngine.PlaySound(sound with { Volume = sound.Volume * 0.5f}, Projectile.position);

            for (int i = 0; i < 5; i++)
            {
                int dust = Dust.NewDust(Projectile.Center, 1, 1, 132, Projectile.velocity.X, Projectile.velocity.Y, 0, default, 0.5f);
                Main.dust[dust].noGravity = true;
            }
            int rando = Main.rand.Next(10, 20);
            for (int i = 0; i < rando; i++)
            {
                int dusty = Dust.NewDust(Projectile.Center - Projectile.velocity / 2f, 0, 0, 135, 0f, 0f, 100, default, 2f);
                Main.dust[dusty].velocity *= 2f;
                Main.dust[dusty].noGravity = true;
            }
        }
    }
}
