using CalamityMod.Dusts;
using CalamityMod.Items.Weapons.Summon;
using CalamityMod.Particles;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Summon
{
    public class MutatedTruffleVortex : ModProjectile, ILocalizedModType
    {
        public new string LocalizationCategory => "Projectiles.Summon";

        public Player Owner => Main.player[Projectile.owner];
        public NPC Target => Projectile.Center.MinionHoming(MutatedTruffle.EnemyDistanceDetection, Owner);

        // In frames.
        private const int TimeFullScale = 120;
        private const int FadeoutTime = 180;

        public override void SetStaticDefaults() => ProjectileID.Sets.MinionShot[Type] = true;

        public override void SetDefaults()
        {
            Projectile.DamageType = DamageClass.Summon;
            Projectile.localNPCHitCooldown = 15;
            Projectile.timeLeft = MutatedTruffle.VortexTimeUntilNextState + TimeFullScale + FadeoutTime;
            Projectile.width = Projectile.height = 408;
            Projectile.penetrate = -1;

            Projectile.friendly = true;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.netImportant = true;
        }

        public override void AI()
        {
            if (Target is not null)
                Projectile.Center = Projectile.Center.MoveTowards(Target.Center, Utils.Remap(Projectile.timeLeft, FadeoutTime, 0f, Target.velocity.Length() + 10f, 0f));

            Projectile.scale = (Projectile.timeLeft > FadeoutTime) ? Utils.Remap(Projectile.timeLeft, MutatedTruffle.VortexTimeUntilNextState + TimeFullScale + FadeoutTime, MutatedTruffle.VortexTimeUntilNextState, 0f, 1f) : Utils.Remap(Projectile.timeLeft, FadeoutTime, 0f, 1f, 0f);
            Projectile.rotation += Utils.Remap(Projectile.timeLeft, FadeoutTime, 0f, MathHelper.PiOver4 / 6f, 0f);

            if (!Main.dedServ)
            {
                Projectile.alpha = (int)Utils.Remap(Projectile.timeLeft, FadeoutTime, 0f, 0f, 255f);

                float dustDistance = Projectile.width * Projectile.scale + 25f;
                Dust vortexDust = Dust.NewDustPerfect(Projectile.Center + Main.rand.NextVector2CircularEdge(dustDistance, dustDistance), (int)CalamityDusts.SulfurousSeaAcid, Scale: 2f);
                vortexDust.velocity = vortexDust.position.DirectionTo(Projectile.Center) * 10f;
                vortexDust.noGravity = true;

                if (Projectile.soundDelay == 0)
                {
                    Projectile.soundDelay = 174;
                    SoundEngine.PlaySound(new SoundStyle("CalamityMod/Sounds/Custom/OldDukeVortex") with { Volume = (Projectile.timeLeft > FadeoutTime) ? .4f : 0f }, Projectile.Center);
                }
            }
        }

        public override void OnSpawn(IEntitySource source)
        {
            Projectile.scale = 0f;
            Projectile.netUpdate = true;
            Projectile.netSpam = 0;
        }

        public override bool? CanDamage()
        {
            if (Target is not null)
                return Projectile.getRect().Intersects(Target.getRect()) ? null : false;
            else
                return false;
        }

        public override void OnKill(int timeLeft)
        {
            if (!Main.dedServ)
            {
                Particle vortexDeath = new DirectionalPulseRing(Projectile.Center, Vector2.Zero, Color.GreenYellow, Vector2.One, 0f, .05f, .6f, 15);
                GeneralParticleHandler.SpawnParticle(vortexDeath);
            }
        }
    }
}
