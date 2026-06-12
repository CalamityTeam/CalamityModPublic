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
    public class HiveNuke : ModProjectile, ILocalizedModType
    {
        public new string LocalizationCategory => "Projectiles.Ranged";
        public ref float RocketID => ref Projectile.ai[0];
        public ref float ProjectileSpeed => ref Projectile.ai[1];
        public bool HasHit = false;
        public float Time = 0;
        public bool BonusEffectMode;
        public bool SetLifetime = false;
        private Player Owner => Main.player[Projectile.owner];
        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.TrailCacheLength[Type] = 20;
            ProjectileID.Sets.TrailingMode[Type] = 2;
        }
        public override void SetDefaults()
        {
            Projectile.width = 25;
            Projectile.height = 25;
            Projectile.friendly = true;
            Projectile.penetrate = 1;
            Projectile.timeLeft = 1500;
            Projectile.DamageType = DamageClass.Ranged;
            Projectile.extraUpdates = 15;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = -1;
        }

        public override void AI()
        {
            BonusEffectMode = Projectile.ai[2] == 2;
            if (BonusEffectMode)
            {
                if (RocketID == ItemID.RocketII || RocketID == ItemID.RocketIV || RocketID == ItemID.MiniNukeII)
                {
                    if (!SetLifetime)
                    {
                        Projectile.timeLeft = 60;
                        Projectile.extraUpdates = 0;
                        Projectile.damage = 0;
                        Projectile.alpha = 255;
                        SetLifetime = true;
                    }

                    var info = new CalamityUtils.RocketBehaviorInfo((int)RocketID);
                    int blastRadius = (int)(Projectile.RocketBehavior(info) * 5f);
                    if (Time % 5 == 0)
                    {
                        Projectile.ExplodeTiles((int)(blastRadius * Utils.Remap(Time, 60f, 1f, 1f, 0f, true)), info.respectStandardBlastImmunity, info.tilesToCheck, info.wallsToCheck);
                    }
                }
                else
                {
                    Point center = Projectile.Center.ToTileCoordinates();
                    if (RocketID == ItemID.DryRocket)
                    {
                        DelegateMethods.f_1 = 10.5f * Utils.Remap(Time, 60f, 1f, 1f, 0f, true);
                        if (Time == 0)
                        {
                            Utils.PlotTileArea(center.X, center.Y, DelegateMethods.SpreadDry);
                        }
                    }
                    if (RocketID == ItemID.WetRocket)
                    {
                        DelegateMethods.f_1 = 10.5f * Utils.Remap(Time, 60f, 1f, 1f, 0f, true);
                        if (Time == 0)
                        {
                            Utils.PlotTileArea(center.X, center.Y, DelegateMethods.SpreadWater);
                        }
                    }
                    if (RocketID == ItemID.LavaRocket)
                    {
                        DelegateMethods.f_1 = 10.5f * Utils.Remap(Time, 60f, 1f, 1f, 0f, true);
                        if (Time == 0)
                        {
                            Utils.PlotTileArea(center.X, center.Y, DelegateMethods.SpreadLava);
                        }
                    }
                    if (RocketID == ItemID.HoneyRocket)
                    {
                        DelegateMethods.f_1 = 10.5f * Utils.Remap(Time, 60f, 1f, 1f, 0f, true);
                        if (Time == 0)
                        {
                            Utils.PlotTileArea(center.X, center.Y, DelegateMethods.SpreadHoney);
                        }
                    }
                }
                Time++;
            }
            else
            {
                // Rotates towards its velocity.
                Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.PiOver2;

                if (Projectile.wet && (RocketID == ItemID.DryRocket || RocketID == ItemID.WetRocket || RocketID == ItemID.LavaRocket || RocketID == ItemID.HoneyRocket))
                    Projectile.Kill();

                Time++;

                if (Projectile.timeLeft <= 30)
                    Projectile.velocity *= 0.95f;

                if (Main.dedServ)
                    return;

                // The projectile will fade away as its time alive is ending.
                Projectile.alpha = (int)Utils.Remap(Projectile.timeLeft, 10f, 0f, 0f, 255f);

                if (Time % 3 == 0)
                {
                    Dust trailDust = Dust.NewDustDirect(Projectile.Center, Projectile.width, Projectile.height, Main.rand.NextBool() ? 303 : DustEffectsID, Scale: Main.rand.NextFloat(0.3f, 0.6f));
                    trailDust.noGravity = true;
                    trailDust.noLight = true;
                    trailDust.noLightEmittence = true;
                    trailDust.velocity = -Projectile.velocity.RotatedByRandom(0.5) * Main.rand.NextFloat(0.2f, 0.8f);
                    if (trailDust.type != DustEffectsID)
                        trailDust.color = Main.rand.NextBool(3) ? EffectsColor : StaticEffectsColor;
                }
                if (Time > 5f)
                {
                    Color smokeColor = Color.Lerp(Color.Black, Color.Lime, 0.25f);
                    Particle smoke = new HeavySmokeParticle(Projectile.Center - Projectile.velocity * 2, -Projectile.velocity * Main.rand.NextFloat(0.2f, 0.6f), smokeColor * 0.65f, 9, Main.rand.NextFloat(0.45f, 0.6f), 0.23f, Main.rand.NextFloat(-0.2f, 0.2f), false);
                    GeneralParticleHandler.SpawnParticle(smoke);
                    Dust dust = Dust.NewDustPerfect(Projectile.Center + Main.rand.NextVector2Circular(10, 10) - Projectile.velocity * 2.5f, DustID.SteampunkSteam, -Projectile.velocity.RotatedByRandom(0.05f) * Main.rand.NextFloat(0.2f, 0.9f), 0, default, Main.rand.NextFloat(0.9f, 1.6f));
                    dust.noGravity = false;
                    dust.color = Color.Black;
                    dust.alpha = Main.rand.Next(90, 220 + 1);
                }

                Lighting.AddLight(Projectile.Center, StaticEffectsColor.ToVector3() * 0.7f);
            }
            
        }
        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox) => CalamityUtils.CircularHitboxCollision(Projectile.Center, Projectile.width * 0.4f, targetHitbox);

        public override bool PreDraw(ref Color lightColor)
        {
            if (BonusEffectMode) { return false; }
            Texture2D texture = ModContent.Request<Texture2D>("CalamityMod/Projectiles/StarProj").Value;
            if (Time > 6)
                CalamityUtils.DrawAfterimagesCentered(Projectile, ProjectileID.Sets.TrailingMode[Type], EffectsColor * 0.4f, 1, texture);
            Texture2D tex = Terraria.GameContent.TextureAssets.Projectile[Type].Value;
            Main.EntitySpriteDraw(tex, Projectile.Center - Main.screenPosition, null, Projectile.GetAlpha(lightColor), Projectile.rotation, tex.Size() / 2f, Projectile.scale, SpriteEffects.None, 0);
            return false;
        }
        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            HasHit = true;
            target.AddBuff(ModContent.BuffType<Plague>(), 60);
        }
        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            HasHit = true;
            return true;
        }
        public override void ModifyHitNPC(NPC target, ref NPC.HitModifiers modifiers)
        {
            if (Projectile.numHits > 1)
                Projectile.damage = (int)(Projectile.damage * 0.7f);
            if (Projectile.damage < 1)
                Projectile.damage = 1;
        }
        public override void OnKill(int timeLeft)
        {
            bool isClusterRocket = (RocketID == ItemID.ClusterRocketI || RocketID == ItemID.ClusterRocketII);
            if (HasHit == true && !BonusEffectMode)
            {
                if (Main.zenithWorld)
                {
                    SoundStyle bees = new("CalamityMod/Sounds/Custom/BEES/bees", 12);
                    SoundEngine.PlaySound(bees with { Volume = 1.5f }, Projectile.Center);
                    SoundStyle fire = new("CalamityMod/Sounds/Item/TheHiveNuke");
                    SoundEngine.PlaySound(fire with { Volume = 0.35f }, Projectile.Center);
                }
                else
                {
                    SoundStyle fire = new("CalamityMod/Sounds/Item/TheHiveNuke");
                    SoundEngine.PlaySound(fire with { Volume = 0.9f }, Projectile.Center);
                }

                Owner.SetScreenshake(9.5f);

                var info = new CalamityUtils.RocketBehaviorInfo((int)RocketID)
                {
                    // Since we use our own spawning method for the cluster rockets, we don't need them to shoot anything,
                    // we'll do it ourselves.
                    clusterProjectileID = ProjectileID.None,
                    destructiveClusterProjectileID = ProjectileID.None,
                };

                int blastRadius = (int)(Projectile.RocketBehavior(info) * 5f);
                Projectile.ExpandHitboxBy((float)blastRadius);
                if (RocketID == ItemID.RocketII || RocketID == ItemID.RocketIV || RocketID == ItemID.MiniNukeII || RocketID == ItemID.DryRocket || RocketID == ItemID.WetRocket || RocketID == ItemID.LavaRocket || RocketID == ItemID.HoneyRocket)
                    Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center, Vector2.Zero, ModContent.ProjectileType<HiveNuke>(), 0, 0f, Projectile.owner, RocketID, 0f, 2f);
                Projectile.damage = (int)(Projectile.damage * 0.7f);
                Projectile.penetrate = -1;
                Projectile.Damage();

                for (int k = 0; k < 3; k++)
                {
                    Particle blastRing = new CustomPulse(Projectile.Center, Vector2.Zero, StaticEffectsColor * 0.8f, "CalamityMod/Particles/FlameExplosion", Vector2.One, Main.rand.NextFloat(-10, 10), Projectile.width / 22815f, Projectile.width / (2275f + 520 * k), 20 + 4 * k);
                    GeneralParticleHandler.SpawnParticle(blastRing);
                }

                for (int k = 0; k < 30; k++)
                {
                    GlowOrbParticle orb = new GlowOrbParticle(Projectile.Center, new Vector2(30, 30).RotatedByRandom(100) * Main.rand.NextFloat(0.05f, 0.8f), false, 60, Main.rand.NextFloat(0.8f, 1.4f), StaticEffectsColor, true, true);
                    GeneralParticleHandler.SpawnParticle(orb);
                }

                for (int k = 0; k < 45; k++)
                {
                    if (Main.rand.NextBool(5))
                    {
                        Dust dust2 = Dust.NewDustPerfect(Projectile.Center, Main.rand.NextBool(3) ? DustEffectsID : 303, new Vector2(30, 30).RotatedByRandom(100) * Main.rand.NextFloat(0.05f, 0.8f));
                        dust2.scale = Main.rand.NextFloat(0.85f, 1.25f);
                        dust2.noGravity = true;
                        if (dust2.type != DustEffectsID)
                            dust2.color = Main.rand.NextBool(3) ? EffectsColor : StaticEffectsColor;
                    }
                    else
                    {
                        Dust dust = Dust.NewDustPerfect(Projectile.Center, DustID.SteampunkSteam, new Vector2(35, 35).RotatedByRandom(100) * Main.rand.NextFloat(0.05f, 0.8f), 0, default, Main.rand.NextFloat(0.9f, 1.7f));
                        dust.noGravity = false;
                        dust.color = Color.Black;
                        dust.alpha = Main.rand.Next(90, 220 + 1);
                    }
                }

                var projAmount = isClusterRocket ? 30f : 20f;
                if (Main.player[Projectile.owner].strongBees)
                    projAmount *= 1.25f;
                for (int k = 0; k < projAmount; k++)
                {
                    int BEES = Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center, new Vector2(10, 10).RotatedByRandom(100) * Main.rand.NextFloat(0.2f, 0.8f), ModContent.ProjectileType<BasicPlagueBee>(), (int)(Projectile.damage * (isClusterRocket ? 0.03f : 0.04f)), 0f, Projectile.owner, 0f, 0f, isClusterRocket ? 2f : 1f);
                    if (BEES.WithinBounds(Main.maxProjectiles))
                    {
                        Main.projectile[BEES].penetrate = 1;
                        Main.projectile[BEES].DamageType = DamageClass.Ranged;
                    }
                }
            }
            else if (!BonusEffectMode)
            {
                for (int i = 0; i <= 15; i++)
                {
                    Dust dust = Dust.NewDustPerfect(Projectile.Center + Main.rand.NextVector2Circular(10, 10) - Projectile.velocity * 3.5f, Main.rand.NextBool(3) ? DustEffectsID : 303, Projectile.velocity.RotatedByRandom(0.3f) * Main.rand.NextFloat(0.4f, 0.9f), 0, default, Main.rand.NextFloat(0.5f, 0.9f));
                    dust.noGravity = false;
                    if (dust.type != DustEffectsID)
                        dust.color = Main.rand.NextBool(3) ? EffectsColor : StaticEffectsColor;
                }
            }
        }
    }
}
