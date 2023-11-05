using System;
using CalamityMod.Dusts;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Typeless
{
    public class PauldronDash : ModProjectile, ILocalizedModType
    {
        public new string LocalizationCategory => "Projectiles.Typeless";
        public override string Texture => "CalamityMod/Projectiles/InvisibleProj";
        public Player Owner => Main.player[Projectile.owner];
        private static float ExplosionRadius = 75f;

        public override void SetDefaults()
        {
            //These shouldn't matter because its circular
            Projectile.width = 75;
            Projectile.height = 75;
            Projectile.friendly = true;
            Projectile.DamageType = AverageDamageClass.Instance;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = false;
            Projectile.penetrate = 1;
            Projectile.timeLeft = 4;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = -1;
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            target.AddBuff(BuffID.OnFire3, 240);
            target.AddBuff(ModContent.BuffType<Buffs.StatDebuffs.ArmorCrunch>(), 300);
            SoundEngine.PlaySound(SoundID.DD2_ExplosiveTrapExplode with { Volume = 0.6f, PitchVariance = 0.4f }, Projectile.Center);

            for (int i = 0; i <= 8; i++)
            {
                Dust dust = Dust.NewDustPerfect(target.Center, Main.rand.NextBool() ? 174 : 127, new Vector2(0, -2).RotatedByRandom(MathHelper.ToRadians(30f)) * Main.rand.NextFloat(2f, 4.5f), 0, default, Main.rand.NextFloat(2.8f, 3.4f));
                dust.noGravity = false;
            }
            for (int i = 0; i <= 5; i++)
            {
                Dust dust2 = Dust.NewDustPerfect(target.Center, Main.rand.NextBool() ? 174 : 127, new Vector2(0, -3).RotatedByRandom(MathHelper.ToRadians(8f)) * Main.rand.NextFloat(1f, 5f), 0, default, Main.rand.NextFloat(2.8f, 3.4f));
                dust2.noGravity = false;
            }

            Projectile.NewProjectile(Projectile.GetSource_FromThis(), target.Center, Vector2.Zero, ModContent.ProjectileType<PauldronExplosion>(), Projectile.damage / 2, 15f, Projectile.owner);
        }

        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox) => CalamityUtils.CircularHitboxCollision(Projectile.Center, ExplosionRadius, targetHitbox);
        public override bool? CanDamage() => base.CanDamage();
        public override void ModifyHitNPC(NPC target, ref NPC.HitModifiers modifiers)
        {
            modifiers.HitDirectionOverride = Math.Sign(Owner.direction);
        }

        public override bool? CanCutTiles() => false;
    }
}
