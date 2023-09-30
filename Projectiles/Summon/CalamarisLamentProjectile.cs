using CalamityMod.Items.Weapons.Summon;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Summon
{
    public class CalamarisLamentProjectile : ModProjectile, ILocalizedModType
    {
        public new string LocalizationCategory => "Projectiles.Summon";

        public ref float TargetShotID => ref Projectile.ai[0];
        public NPC TargetShot => Main.npc[(int)TargetShotID];

        public override void SetStaticDefaults()
        {
            Main.projFrames[Type] = 5;
            ProjectileID.Sets.MinionShot[Type] = true;
            ProjectileID.Sets.TrailingMode[Type] = 2;
            ProjectileID.Sets.TrailCacheLength[Type] = 4;
            ProjectileID.Sets.DrawScreenCheckFluff[Type] = (int)CalamarisLament.EnemyDistanceDetection;
        }

        public override void SetDefaults()
        {
            Projectile.DamageType = DamageClass.Summon;
            Projectile.localNPCHitCooldown = -1;
            Projectile.timeLeft = 240;
            Projectile.width = Projectile.height = 28;

            Projectile.friendly = true;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
            Projectile.usesLocalNPCImmunity = true;
        }

        public override void AI()
        {
            if (TargetShot is not null && TargetShot.active)
            {
                float inertia = 20f;
                Projectile.velocity = (Projectile.velocity * inertia + Projectile.SafeDirectionTo(TargetShot.Center) * CalamarisLament.ShootingProjectileSpeed) / (inertia + 1f);
                Projectile.extraUpdates = 1;

                Projectile.netUpdate = true;
                Projectile.netSpam = 0;
            }
            else
                Projectile.extraUpdates = 0;

            Projectile.rotation = Projectile.velocity.ToRotation() - MathHelper.PiOver2;
            Projectile.alpha = (int)Utils.Remap(Projectile.timeLeft, 30f, 0f, 0f, 255f);

            Dust trailDust = Dust.NewDustDirect(Projectile.position, Projectile.width, Projectile.height, 109, Scale: Main.rand.NextFloat(0.5f, 0.8f), Alpha: 127);
            trailDust.noGravity = true;
            trailDust.noLight = true;
            trailDust.alpha = (int)Utils.Remap(Projectile.timeLeft, 30f, 0f, 127f, 0f);

            Projectile.frameCounter++;
            if (Projectile.frameCounter >= 4)
            {
                Projectile.frame = (Projectile.frame + 1) % Main.projFrames[Type];
                Projectile.frameCounter = 0;
            }
        }

        // Made for DoG so the projectile can go through segments without spamming numbers
        // and hit DoG's head and/or tail reliably and without penetration.
        public override bool? CanDamage() => Projectile.getRect().Intersects(TargetShot.getRect()) ? null : false;

        public override void OnKill(int timeLeft)
        {
            for (int i = 0; i < 15; i++)
            {
                Dust deathDust = Dust.NewDustPerfect(Projectile.Center, 109, Projectile.velocity.SafeNormalize(Vector2.Zero).RotatedByRandom(MathHelper.PiOver4) * Main.rand.NextFloat(3f, 7f), Scale: Main.rand.NextFloat(0.5f, 1.5f), Alpha: 127);
                deathDust.noGravity = true;
                deathDust.noLight = true;
            }

            SoundEngine.PlaySound(SoundID.NPCDeath28, Projectile.Center);
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D texture = ModContent.Request<Texture2D>(Texture).Value;
            Vector2 drawPosition = Projectile.Center - Main.screenPosition;
            Rectangle frame = texture.Frame(1, Main.projFrames[Type], 0, Projectile.frame);
            Vector2 origin = frame.Size() * 0.5f;

            CalamityUtils.DrawAfterimagesCentered(Projectile, ProjectileID.Sets.TrailingMode[Type], lightColor);

            Main.EntitySpriteDraw(texture, drawPosition, frame, Projectile.GetAlpha(lightColor), Projectile.rotation, origin, Projectile.scale, SpriteEffects.None);

            return false;
        }
    }
}
