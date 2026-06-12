using CalamityMod.Dusts;
using CalamityMod.Particles;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Typeless
{
    public class PuddleSplash : ModProjectile, ILocalizedModType
    {
        public new string LocalizationCategory => "Projectiles.Typeless";
        public override string Texture => "CalamityMod/Projectiles/InvisibleProj";
        public ref float time => ref Projectile.ai[0];
        public Player Owner => Main.player[Projectile.owner];
        public override void SetDefaults()
        {
            Projectile.width = 165;
            Projectile.height = 80;
            Projectile.friendly = true;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
            Projectile.DamageType = AverageDamageClass.Instance;
            Projectile.penetrate = -1;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = -1;
            Projectile.usesIDStaticNPCImmunity = true;
            Projectile.idStaticNPCHitCooldown = 10;
            Projectile.timeLeft = 25;
        }

        public override void AI()
        {
            if (time == 0)
            {
                SoundStyle waterSound = new("CalamityMod/Sounds/Item/WaterSplash" + (Main.rand.NextBool() ? "1" : "2"));
                SoundEngine.PlaySound(waterSound with { Volume = 0.2f, Pitch = Main.rand.NextFloat(0.9f, 1f) }, Projectile.Center);
                for (int i = 0; i < 6; i++)
                {
                    BloodParticle water = new BloodParticle(Projectile.Center, (-Vector2.UnitY * Main.rand.NextFloat(6f, 12f)).RotatedByRandom(0.65f), 30, Main.rand.NextFloat(0.7f, 1.2f), Main.rand.NextBool() ? Color.CornflowerBlue : Color.SkyBlue);
                    GeneralParticleHandler.SpawnParticle(water);

                    Dust dust = Dust.NewDustPerfect(Projectile.Center, ModContent.DustType<LightDust>(), (-Vector2.UnitY * Main.rand.NextFloat(2f, 5f)).RotatedByRandom(0.65f), 0, default, Main.rand.NextFloat(1.1f, 1.4f));
                    dust.noGravity = false;
                    dust.color = Main.rand.NextBool() ? Color.CornflowerBlue : Color.SkyBlue;
                    dust.noLight = true;
                    dust.noLightEmittence = true;
                    dust.alpha = 100;
                }
                for (int i = 0; i < 2; i++)
                {
                    Particle blastRing = new CustomSpark(Projectile.Center, -Vector2.UnitY * 0.1f, "CalamityMod/Particles/HighResHollowCircleHardEdgeAlt", false, i == 0 ? 15 : 22, i == 0 ? 0.07f : 0.05f, i == 0 ? Color.CornflowerBlue : Color.SkyBlue, new Vector2(0.7f, i == 0 ? 1.3f : 0.9f), shrinkSpeed: i == 0 ? -0.6f : -0.5f);
                    GeneralParticleHandler.SpawnParticle(blastRing);
                }
            }
            time++;
        }
        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            target.AddBuff(BuffID.Wet, 180);
            Vector2 launchVel = Utils.DirectionTo(Owner.Center, target.Center) + Vector2.UnitY * -0.75f;
            target.MoveNPC(launchVel, 12, false, Owner);
        }
        public override bool? CanCutTiles() => false;
    }
}
