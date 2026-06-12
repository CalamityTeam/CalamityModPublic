using System;
using System.IO;
using CalamityMod.CalPlayer;
using CalamityMod.Dusts;
using CalamityMod.NPCs;
using CalamityMod.Particles;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Typeless
{
    [PierceResistException]
    public class TransformerBlob : ModProjectile, ILocalizedModType
    {
        public new string LocalizationCategory => "Projectiles.Typeless";
        public override string Texture => "CalamityMod/Projectiles/InvisibleProj";
        public Player Owner => Main.player[Projectile.owner];
        public CalamityPlayer moddedOwner => Owner.Calamity();
        private float radius = 25f;
        public bool visuals => moddedOwner.transformerVisual;
        public float visualMult => (!visuals && !powered) ? 0.2f : visuals ? 1 : 0.4f;
        public ref float layer => ref Projectile.ai[0];
        public bool canDamage = false;
        public float speed = 250;
        public float rotationAngle = 0;
        public int time = 0;
        public int currentLayer = 0;
        public float sine = 1;
        public float rotSpeed = 1f;
        public int savedFrame = 0;
        public bool powered => Projectile.localAI[0] == 5;
        public int poweredTimerMax = 140;
        public int poweredTimer = -1;
        public Color cl1 = Color.LightSkyBlue;
        public Color cl2 = Color.DodgerBlue;
        public float poweredLerp => (float)Math.Pow(Utils.GetLerpValue(poweredTimerMax, 90, poweredTimer, true), 4);
        public override void SendExtraAI(BinaryWriter writer) // These MP syncs cause tons of errors so they're disabled until we can fix that
        {
            //writer.Write(time);
            //writer.Write(currentLayer);
            //writer.Write(speed);
            //writer.Write(rotSpeed);
            //writer.Write(rotationAngle);
            //writer.Write(Projectile.localAI[0]);

            //writer.WriteFlags(canDamage);
        }
        public override void ReceiveExtraAI(BinaryReader reader)
        {
            //time = reader.Read();
            //currentLayer = reader.Read();
            //speed = reader.ReadSingle();
            //rotSpeed = reader.ReadSingle();
            //rotationAngle = reader.ReadSingle();
            //Projectile.localAI[0] = reader.ReadSingle();

            //reader.ReadFlags(out canDamage);
        }
        public override void SetStaticDefaults()
        {
            Main.projFrames[Type] = 16;
            ProjectileID.Sets.NoLiquidDistortion[Type] = true;
        }
        public override void SetDefaults()
        {
            Projectile.width = 42;
            Projectile.height = 56;
            Projectile.friendly = true;
            Projectile.DamageType = AverageDamageClass.Instance;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = false;
            Projectile.penetrate = -1;
            Projectile.timeLeft = 600;
            Projectile.extraUpdates = 1;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 60 * Projectile.MaxUpdates;
            Projectile.ArmorPenetration = 25;
        }
        public override void AI()
        {
            Player Owner = Main.player[Projectile.owner];

            Lighting.AddLight(Projectile.Center, cl1.ToVector3() * 0.5f);

            sine = (float)Math.Sin(Main.GlobalTimeWrappedHourly * (Projectile.ai[1] % 2 == 0 ? 10 : 6) / MathHelper.Pi) * 0.4f;
            if (!powered)
                poweredTimerMax = (int)(140 + (MathHelper.Clamp(Utils.Remap(Owner.ownedProjectileCounts[ModContent.ProjectileType<TransformerBlob>()], 1, 30, 7, 6, false), 1, 120)) * Projectile.ai[1]);
            if (time == 0)
            {
                Projectile.netUpdate = true;
                rotationAngle = Projectile.ai[2];
                Projectile.frame = 8;
            }
            if (poweredTimer == -1 && powered)
            {
                poweredTimer = poweredTimerMax;
                savedFrame = Projectile.frame;
            }
            
            if (time >= 40)
            {
                Projectile.frameCounter++;
                if (Projectile.frameCounter > (6) * Projectile.MaxUpdates)
                {
                    if (powered && Projectile.frame == 8)
                        Projectile.frame = 8;
                    else
                        Projectile.frame++;
                    Projectile.frameCounter = 0;
                }
                if (Projectile.frame >= 16)
                    Projectile.frame = 0;
            }
            else
                Projectile.frame = (int)MathHelper.Lerp(savedFrame, 8, poweredLerp);

            layer = (int)(Utils.GetLerpValue(0, 10, Projectile.ai[1]) + 0.9f);
            
            if (time >= 90)
                canDamage = true;

            if (!moddedOwner.transformer || Owner.dead)
            {
                Projectile.Kill();
            }

            if (layer != currentLayer)
            {
                currentLayer = (int)layer;
                if (time > 60)
                    time = 60;
            }
            rotationAngle = MathHelper.Lerp(rotationAngle, Projectile.ai[2], 0.025f);
            if (time >= 40)
            {
                if (poweredTimer == 1)
                {
                    Projectile.netUpdate = true;
                    if (visuals)
                        Owner.SetScreenshake(3.5f);
                    Projectile.numHits = 0;
                    Projectile.velocity = Utils.DirectionTo(Owner.Center, Owner.Calamity().mouseWorld) * 12;
                    Projectile.extraUpdates = 8;
                    for (int i = 0; i < Main.maxNPCs; i++)
                        Projectile.localNPCImmunity[i] = 0;

                    for (int i = 0; i <= 9; i++)
                    {
                        float variance = Main.rand.NextFloat(-0.6f, 0.6f);
                        int dustStyle = 278;
                        Dust dust2 = Dust.NewDustPerfect(Projectile.Center, dustStyle, Projectile.velocity);
                        dust2.scale = Main.rand.NextFloat(0.9f, 1.2f) - Math.Abs(variance);
                        dust2.velocity = (Projectile.velocity * 2).RotatedBy(variance) * Main.rand.NextFloat(0.3f, 1f) * (1 - Math.Abs(variance));
                        dust2.noGravity = true;
                        dust2.color = cl1;
                    }

                    if (visuals)
                    { 
                        SoundStyle fire = new("CalamityMod/Sounds/Item/OmicronBeam");
                        SoundEngine.PlaySound(fire with { Volume = 0.3f, Pitch = Math.Clamp(Main.rand.NextFloat(0.1f, 0.2f) + Projectile.ai[1] * 0.02f, 0, 1), MaxInstances = 1 }, Projectile.Center);
                    }
                }
                else if (poweredTimer == 0)
                {
                    float sine = (float)Math.Sin(Projectile.timeLeft * 0.575f / MathHelper.Pi);

                    Vector2 offset = Projectile.velocity.SafeNormalize(Vector2.UnitX).RotatedBy(MathHelper.PiOver2) * sine * 20f;
                    float targetDist = Vector2.Distance(Owner.Center, Projectile.Center);
                    if (targetDist < 1400)
                    {
                        if (visuals)
                        {
                            Particle spark = new GlowSparkParticle(Projectile.Center - Projectile.velocity, -Projectile.velocity * 0.3f, false, 21, 0.04f, cl1 * 0.65f, new Vector2(0.6f, 0.5f), true, false, 0.7f);
                            GeneralParticleHandler.SpawnParticle(spark);
                        }
                        if (time % 2 == 0)
                        {
                            Vector2 dustVel = (-Projectile.velocity).RotatedByRandom(0.3f);
                            Dust dust = Dust.NewDustPerfect(Projectile.Center + offset, ModContent.DustType<VoidDustInverted>(), dustVel * Main.rand.NextFloat(0.1f, 0.8f));
                            dust.noGravity = true;
                            dust.scale = Main.rand.NextFloat(0.4f, 0.6f);
                            dust.color = new Color(30, 30, 30);
                            dust.noLightEmittence = true;
                        }
                    }

                    Projectile.rotation = (Projectile.velocity.RotatedBy(MathHelper.ToRadians(-90))).ToRotation();
                }
                else
                {
                    Vector2 centerPoint = (powered ? Vector2.Lerp(Owner.Center, Vector2.Lerp(Owner.Calamity().mouseWorld, Owner.Center, 0.6f), poweredLerp) : Owner.Center);
                    float layerDist = (Utils.Remap(Owner.ownedProjectileCounts[ModContent.ProjectileType<TransformerBlob>()], 1, 20, 60, 55, false) * layer);
                    float fadeValue = MathHelper.Clamp(Utils.Remap(Projectile.ai[1], 1, 10, 0.3f, 0.1f, false), 0.3f, 0);
                    float positioning = (-60 - layerDist + 30 * sine) * MathHelper.Clamp((powered ? (1 - poweredLerp) : 1), fadeValue, 1);
                    Projectile.velocity = ((centerPoint + new Vector2(0, positioning).RotatedBy(rotationAngle * -rotSpeed + Main.GlobalTimeWrappedHourly * 2.5f * (1 - layer * (powered ? 0.33f : 0.15f)) * (layer % 2 == 0 ? -1 : 1))) - Projectile.Center) / (speed);
                    Projectile.rotation = Projectile.rotation.AngleLerp(sine, 0.02f);
                }
                if (powered && poweredTimer != 0)
                {
                    rotSpeed *= Utils.Remap(Projectile.ai[1], 1, 10, 1.0067f, 1.0058f, false);

                    Projectile.rotation = Projectile.rotation.AngleLerp(Utils.DirectionTo(Owner.Center, Owner.Calamity().mouseWorld).RotatedBy(MathHelper.ToRadians(-90)).ToRotation(), poweredLerp);
                }
            }
            else
            {
                Projectile.velocity *= 0.965f;
                Projectile.rotation = (Projectile.velocity.RotatedBy(MathHelper.ToRadians(-90))).ToRotation();
            }
            speed = MathHelper.Lerp(speed, 1, (float)Math.Pow(Utils.GetLerpValue(0, 180, time), 2));

            if (poweredTimer != 0)
                Projectile.timeLeft++;
            if (poweredTimer > 0)
                poweredTimer--;
            time++;
        }
        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            if (target.life <= 0 && target.realLife == -1)
                Projectile.numHits--;
        }
        public override void ModifyHitNPC(NPC target, ref NPC.HitModifiers modifiers)
        {
            if (poweredTimer == 0)
            {
                float minMult = 0.05f;
                int hitsToMinMult = 4;
                float damageMult = Utils.Remap(Projectile.numHits, 0, hitsToMinMult, 1, minMult, true) * (Projectile.numHits == 0 ? 1.5f : 1); // +50% damage on first hit, if it kills an enemy it keeps the bonus
                modifiers.SourceDamage *= (damageMult + (Main.zenithWorld ? 0.2f * Projectile.ai[1] : 0));
            }
            else
                modifiers.SourceDamage *= 0.2f;

            target.MoveNPC(Utils.DirectionTo(Owner.Center, target.Center), (poweredTimer == 0 ? 7 : 3), false, Owner);

            if (Projectile.numHits == 0 && poweredTimer == 0)
            {
                for (int i = 0; i <= 6; i++)
                {
                    float variance = Main.rand.NextFloat(-0.4f, 0.4f);
                    Vector2 fxVel = (Projectile.velocity * 3).RotatedBy(variance) * Main.rand.NextFloat(0.3f, 1f) * (1 - Math.Abs(variance));
                    Particle spark2 = new SparkParticle(Projectile.Center + fxVel, fxVel, false, 45, Main.rand.NextFloat(0.8f, 1f) - Math.Abs(variance), Main.rand.NextBool(4) ? cl2 : cl1);
                    GeneralParticleHandler.SpawnParticle(spark2);
                }
            }
        }
        public override void OnKill(int timeLeft)
        {
            for (int i = 0; i < 14; i++)
            {
                Vector2 dustVel = (Vector2.One * 5).RotatedByRandom(100);
                Dust dust = Dust.NewDustPerfect(Projectile.Center, ModContent.DustType<VoidDustInverted>(), dustVel * Main.rand.NextFloat(0.1f, 0.8f));
                dust.noGravity = true;
                dust.scale = Main.rand.NextFloat(0.8f, 1.1f);
                dust.color = new Color(30, 30, 30);
                dust.noLightEmittence = true;
            }
        }
        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D orbTexture = ModContent.Request<Texture2D>("CalamityMod/Items/Accessories/TheTransformer").Value;
            Texture2D bTexture = ModContent.Request<Texture2D>("CalamityMod/Particles/BloomCircle").Value;
            Rectangle frame = orbTexture.Frame(1, 16, 0, Projectile.frame);
            Vector2 origin = frame.Size() * 0.5f;

            Vector2 baseDrawPos = Projectile.Center - Main.screenPosition + 
                (poweredTimer != 0 ? new Vector2(0, Owner.gfxOffY) : Vector2.Zero);

            if (powered)
            {
                Color auraColor = cl1 with { A = 0 } * poweredLerp * visualMult;
                for (int i = 0; i < 2; i++) 
                {
                    float bScale2 = 0.25f;
                    Main.EntitySpriteDraw(bTexture, baseDrawPos, null, cl2 with { A = 0 } * Utils.GetLerpValue(poweredTimerMax, 180, poweredTimer, true) * visualMult, Projectile.rotation, bTexture.Size() * 0.5f, Vector2.Lerp(new Vector2(0.6f, 1.4f), Vector2.One, MathHelper.Min(Utils.GetLerpValue(8, 15, Projectile.frame, true), Utils.GetLerpValue(8, 0, Projectile.frame, true))) * bScale2 * Projectile.scale, SpriteEffects.None, 0);
                }
            }
            Main.EntitySpriteDraw(orbTexture, baseDrawPos, frame, Color.White * visualMult, Projectile.rotation, origin, Projectile.scale, SpriteEffects.None, 0);

            for (int i = 0; i < 10; i++)
            {
                Color auraColor = cl1 with { A = 0 } * (powered ? (float)Math.Pow(Utils.GetLerpValue(poweredTimerMax, 20, poweredTimer, true), 3) : sine) * 0.6f * visualMult;
                Vector2 drawOffset = (MathHelper.TwoPi * i / 10f).ToRotationVector2() * 4;
                Main.EntitySpriteDraw(orbTexture, baseDrawPos + drawOffset, frame, auraColor, Projectile.rotation, origin, Projectile.scale, SpriteEffects.None);
            }
            return false;
        }
        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox) => CalamityUtils.CircularHitboxCollision(Projectile.Center, radius, targetHitbox);
        public override bool? CanDamage() => canDamage ? null : false;
        public override bool? CanCutTiles() => false;
    }
}
