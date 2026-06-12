using CalamityMod.Buffs.DamageOverTime;
using CalamityMod.Particles;
using CalamityMod.Projectiles.Typeless;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;
using static CalamityMod.Projectiles.Ranged.TheHiveHoldout;
namespace CalamityMod.Projectiles.Ranged
{
    public class HiveMissile : ModProjectile, ILocalizedModType
    {
        public new string LocalizationCategory => "Projectiles.Ranged";
        public ref float RocketID => ref Projectile.ai[0];
        public ref float ProjectileSpeed => ref Projectile.ai[1];
        public ref float Time => ref Projectile.ai[2];
        public bool HasHit = false;

        public override void SetDefaults()
        {
            Projectile.width = 25;
            Projectile.height = 25;
            Projectile.friendly = true;
            Projectile.penetrate = 1;
            Projectile.timeLeft = 100;
            Projectile.DamageType = DamageClass.Ranged;
            Projectile.extraUpdates = 2;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = -1;
        }

        public override void AI()
        {
            // Rotates towards its velocity.
            Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.PiOver2;

            if (Projectile.wet && (RocketID == ItemID.DryRocket || RocketID == ItemID.WetRocket || RocketID == ItemID.LavaRocket || RocketID == ItemID.HoneyRocket))
            {
                HasHit = true;
                Projectile.Kill();
            }

            Time++;

            if (Projectile.timeLeft <= 50)
            {
                Projectile.velocity *= 0.965f;
                if (Time % 5 == 0)
                {
                    Dust dust = Dust.NewDustPerfect(Projectile.Center + Main.rand.NextVector2Circular(10, 10) - Projectile.velocity * 3.5f, Main.rand.NextBool(3) ? DustEffectsID : 303, Projectile.velocity.RotatedByRandom(0.3f) * Main.rand.NextFloat(0.4f, 0.9f), 0, default, Main.rand.NextFloat(0.5f, 0.9f));
                    dust.noGravity = false;
                    if (dust.type != DustEffectsID)
                        dust.color = Main.rand.NextBool(3) ? EffectsColor : StaticEffectsColor;
                }
            }

            if (Main.dedServ)
                return;

            // The projectile will fade away as its time alive is ending.
            Projectile.alpha = (int)Utils.Remap(Projectile.timeLeft, 40f, 0f, 0f, 255f);

            if (Time % 3 == 0)
            {
                Dust trailDust = Dust.NewDustDirect(Projectile.Center, Projectile.width, Projectile.height, Main.rand.NextBool() ? 303 : DustEffectsID, Scale: Main.rand.NextFloat(0.3f, 0.6f));
                trailDust.noGravity = true;
                trailDust.noLight = true;
                trailDust.noLightEmittence = true;
                trailDust.velocity = -Projectile.velocity * Main.rand.NextFloat(0.2f, 0.8f);
                if (trailDust.type != DustEffectsID)
                    trailDust.color = Main.rand.NextBool(3) ? EffectsColor : StaticEffectsColor;
            }
            if (Time > 5f)
            {
                float sizeBonus = Projectile.timeLeft < 30 ? Time * 0.003f : 0;
                Color smokeColor = Color.Lerp(Color.Black, Color.Lime, 0.25f);
                Particle smoke = new HeavySmokeParticle(Projectile.Center - Projectile.velocity * 2, -Projectile.velocity.RotatedByRandom(sizeBonus) * Main.rand.NextFloat(0.2f, 0.6f), smokeColor * 0.65f, 6, Main.rand.NextFloat(0.3f, 0.45f) + sizeBonus, 0.23f - (sizeBonus * 0.3f), Main.rand.NextFloat(-0.2f, 0.2f), false);
                GeneralParticleHandler.SpawnParticle(smoke);

                if (Main.rand.NextBool())
                {
                    Dust dust = Dust.NewDustPerfect(Projectile.Center + Main.rand.NextVector2Circular(10, 10) - Projectile.velocity * 2.5f, DustID.SteampunkSteam, -Projectile.velocity.RotatedByRandom(0.05f) * Main.rand.NextFloat(0.05f, 0.4f), 0, default, Main.rand.NextFloat(0.8f, 1.4f));
                    dust.noGravity = false;
                    dust.color = Color.Black;
                    dust.alpha = Main.rand.Next(90, 220 + 1);
                }
            }

            Lighting.AddLight(Projectile.Center, StaticEffectsColor.ToVector3() * 0.7f);
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D tex = Terraria.GameContent.TextureAssets.Projectile[Type].Value;
            Main.EntitySpriteDraw(tex, Projectile.Center - Main.screenPosition, null, Projectile.GetAlpha(lightColor), Projectile.rotation, tex.Size() / 2f, Projectile.scale, SpriteEffects.None, 0);
            return false;
        }
        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            HasHit = true;
            target.AddBuff(ModContent.BuffType<Plague>(), 90);
        }
        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            HasHit = true;
            return true;
        }
        public override void ModifyHitNPC(NPC target, ref NPC.HitModifiers modifiers)
        {
            if (Projectile.numHits > 1)
                Projectile.damage = (int)(Projectile.damage * 0.6f);
            if (Projectile.damage < 1)
                Projectile.damage = 1;
        }
        public override void OnKill(int timeLeft)
        {
            if (HasHit == true)
            {
                var info = new CalamityUtils.RocketBehaviorInfo((int)RocketID)
                {
                    // Since we use our own spawning method for the cluster rockets, we don't need them to shoot anything,
                    // we'll do it ourselves.
                    clusterProjectileID = ProjectileID.None,
                    destructiveClusterProjectileID = ProjectileID.None,
                };

                bool isClusterRocket = (RocketID == ItemID.ClusterRocketI || RocketID == ItemID.ClusterRocketII);
                SoundStyle fire = new("CalamityMod/Sounds/Custom/PlagueSounds/PlagueBoom", 4);
                SoundEngine.PlaySound(fire with { Volume = 0.5f, Pitch = -0.3f }, Projectile.Center);

                int blastRadius = (int)(Projectile.RocketBehavior(info) * 0.7f);
                Projectile.ExpandHitboxBy((float)blastRadius);
                Projectile.damage = (int)(Projectile.damage * 0.5f);
                Projectile.penetrate = -1;
                Projectile.Damage();

                Particle blastRing = new CustomPulse(
                Projectile.Center,
                Vector2.Zero,
                StaticEffectsColor * 0.8f,
                "CalamityMod/Particles/FlameExplosion",
                Vector2.One,
                Main.rand.NextFloat(-10, 10),
                Projectile.width / 22815f,
                Projectile.width / 2275f,
                10);
                GeneralParticleHandler.SpawnParticle(blastRing);

                for (int k = 0; k < 15; k++)
                {
                    Dust dust2 = Dust.NewDustPerfect(Projectile.Center, DustID.SteampunkSteam, new Vector2(8, 8).RotatedByRandom(100) * Main.rand.NextFloat(0.05f, 0.8f));
                    dust2.scale = Main.rand.NextFloat(0.75f, 0.95f);
                    dust2.noGravity = true;
                    dust2.color = Main.rand.NextBool(3) ? EffectsColor : StaticEffectsColor;
                }
                var projAmount = isClusterRocket ? 3f : 2f;
                if (Main.player[Projectile.owner].strongBees)
                    projAmount++;
                for (int k = 0; k < projAmount; k++)
                {
                    int BEES = Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center, new Vector2(4, 10).RotatedByRandom(4) * Main.rand.NextFloat(0.2f, 0.8f), ModContent.ProjectileType<BasicPlagueBee>(), (int)(Projectile.damage * (isClusterRocket ? 0.2f : 0.3f)), 0f, Projectile.owner, 0f, 0f, isClusterRocket ? 2f : 1f);
                    if (BEES.WithinBounds(Main.maxProjectiles))
                    {
                        Main.projectile[BEES].penetrate = 1;
                        Main.projectile[BEES].DamageType = DamageClass.Ranged;
                    }
                }
            }
        }
    }
}
