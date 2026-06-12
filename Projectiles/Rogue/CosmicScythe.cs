using CalamityMod.Buffs.DamageOverTime;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Rogue
{
    public class CosmicScythe : ModProjectile, ILocalizedModType
    {
        public new string LocalizationCategory => "Projectiles.Rogue";
        public override string Texture => "CalamityMod/Projectiles/Boss/SignusScythe";

        private int originalDamage;

        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.CultistIsResistantTo[Type] = true;
            ProjectileID.Sets.TrailCacheLength[Type] = 4;
            ProjectileID.Sets.TrailingMode[Type] = 0;
        }

        public override void SetDefaults()
        {
            Projectile.width = 26;
            Projectile.height = 26;
            Projectile.friendly = true;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
            Projectile.alpha = 100;
            Projectile.DamageType = RogueDamageClass.Instance;
            Projectile.MaxUpdates = 2;
            Projectile.timeLeft = 150 * Projectile.MaxUpdates;
        }

        public override void AI()
        {
            Projectile.rotation += 0.5f * (float)Projectile.direction;
            int shadow = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.ShadowbeamStaff, 0f, 0f, 100, default, 1f);
            Main.dust[shadow].noGravity = true;
            Main.dust[shadow].velocity *= 0f;
            Projectile.velocity *= 0.95f;
            if (Projectile.timeLeft <= 135 * Projectile.MaxUpdates)
            {
                if (Projectile.timeLeft > 120 * Projectile.MaxUpdates)
                    Projectile.velocity *= 1.06f;
                CalamityUtils.HomeInOnNPC(Projectile, true, 640f, 20f, 20f);
            }
        }

        public override bool? CanDamage() => Projectile.timeLeft > 135 * Projectile.MaxUpdates ? false : base.CanDamage();

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone) => target.AddBuff(ModContent.BuffType<Laceration>(), 240);

        public override bool PreDraw(ref Color lightColor)
        {
            CalamityUtils.DrawAfterimagesCentered(Projectile, ProjectileID.Sets.TrailingMode[Type], lightColor, 1);
            return false;
        }

        public override void OnKill(int timeLeft)
        {
            SoundEngine.PlaySound(SoundID.Item14, Projectile.Center);
            for (int d = 0; d < 4; d++)
            {
                Dust shadow = Dust.NewDustDirect(Projectile.position, Projectile.width, Projectile.height, DustID.Shadowflame, 0f, 0f, 100, default, 2f);
                shadow.velocity *= 3f;
                if (Main.rand.NextBool())
                {
                    shadow.scale = 0.5f;
                    shadow.fadeIn = 1f + (float)Main.rand.Next(10) * 0.1f;
                }
            }
            for (int d = 0; d < 6; d++)
            {
                Dust shadow = Dust.NewDustDirect(Projectile.position, Projectile.width, Projectile.height, DustID.ShadowbeamStaff, 0f, 0f, 100, default, 3f);
                shadow.noGravity = true;
                shadow.velocity *= 5f;
                shadow = Dust.NewDustDirect(Projectile.position, Projectile.width, Projectile.height, DustID.ShadowbeamStaff, 0f, 0f, 100, default, 2f);
                shadow.velocity *= 2f;
            }
        }
    }
}
