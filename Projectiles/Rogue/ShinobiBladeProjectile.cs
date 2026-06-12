using CalamityMod.Items.Weapons.Rogue;
using CalamityMod.Particles;
using CalamityMod.Projectiles.Healing;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Rogue
{
    public class ShinobiBladeProjectile : ModProjectile, ILocalizedModType
    {
        public new string LocalizationCategory => "Projectiles.Rogue";
        public override string Texture => "CalamityMod/Items/Weapons/Rogue/ShinobiBlade";

        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.TrailCacheLength[Type] = 3;
            ProjectileID.Sets.TrailingMode[Type] = 0;
        }

        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = 16;
            Projectile.friendly = true;
            Projectile.penetrate = 1;
            Projectile.DamageType = RogueDamageClass.Instance;
            Projectile.MaxUpdates = 5;
            Projectile.timeLeft = 60 * Projectile.MaxUpdates;
        }

        public override void AI()
        {
            Projectile.spriteDirection = Projectile.direction = (Projectile.velocity.X > 0).ToDirectionInt();
            Projectile.rotation = Projectile.velocity.ToRotation() + (Projectile.spriteDirection == 1 ? 0f : MathHelper.Pi) + MathHelper.PiOver2 * Projectile.spriteDirection;

            if (Main.rand.NextBool(5))
            {
                Dust trail = Dust.NewDustDirect(Projectile.position, Projectile.width, Projectile.height, DustID.MagicMirror);
                trail.noGravity = true;
            }
        }

        public override bool PreDraw(ref Color lightColor)
        {
            CalamityUtils.DrawAfterimagesCentered(Projectile, ProjectileID.Sets.TrailingMode[Type], lightColor, 1);
            return false;
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            NPC firstTarget = Main.npc[(int)Projectile.ai[0]];

            // 8 hits total
            if (Projectile.Calamity().stealthStrike && Projectile.ai[1] < 8f && (Projectile.ai[1] == 0f || firstTarget != null))
            {
                Vector2 targetPos = Projectile.ai[1] == 0f ? target.Center : firstTarget.Center;
                Vector2 offset = Vector2.UnitX.RotatedByRandom(MathHelper.TwoPi) * Main.rand.NextFloat(80f, 120f);
                Vector2 eVelocity = Vector2.UnitX.RotatedBy(offset.ToRotation() + MathHelper.Pi) * 4f;
                int realTarget = Projectile.ai[1] == 0f ? target.whoAmI : firstTarget.whoAmI;
                Projectile echo = Projectile.NewProjectileDirect(Projectile.GetSource_FromThis(), targetPos + offset, eVelocity, Projectile.type, Projectile.damage, Projectile.knockBack, Projectile.owner, realTarget, Projectile.ai[1] + 1);
                echo.Calamity().stealthStrike = true;
                echo.tileCollide = false;

                Vector2 slashVel = Projectile.velocity.SafeNormalize(Vector2.UnitX);
                GlowSparkParticle slash = new(Projectile.Center, slashVel, false, 12, 0.06f, Color.DarkBlue, Vector2.One, true, shrinkSpeed: 0.9f);
                GeneralParticleHandler.SpawnParticle(slash);

                SoundEngine.PlaySound(WulfrumKnife.TileHitSound, Projectile.Center);
            }

            if (target.life <= 0)
                Main.player[Projectile.owner].SpawnLifeStealProjectile(target, Projectile, ModContent.ProjectileType<ShinobiHealOrb>(), 5, 0f);
        }

        public override void OnKill(int timeLeft)
        {
            int dustType = 42;
            for (int i = 0; i < 5; i++)
            {
                Dust crumble = Dust.NewDustDirect(Projectile.Center, 1, 1, dustType, Projectile.velocity.X / 2, Projectile.velocity.Y / 2, 0, default, 1.5f);
                crumble.noGravity = true;
            }
        }
    }
}
