using System;
using CalamityMod.Particles;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;
using CalamityMod.Buffs.StatBuffs;
using CalamityMod.Items.Weapons.Magic;

namespace CalamityMod.Projectiles.Magic
{
    public class ManaChargedCoral : ModProjectile, ILocalizedModType
    {
        public new string LocalizationCategory => "Projectiles.Magic";
        public Player Owner => Main.player[Projectile.owner];
        public static float FullMana => 180f;
        public ref float ManaCharge => ref Projectile.ai[0];
        public NPC StuckNPC => Projectile.numHits > 0 ? Main.npc[(int)Projectile.ai[1]] : null;
        public ref float DetachmentEffectsComplete => ref Projectile.localAI[0];
        public bool Stuck => (StuckNPC != null && StuckNPC.active && ManaCharge < FullMana);
        public bool HasStuck => (Projectile.numHits > 0);

        Vector2 offsetFromStuckNPC;
        float rotationFromStuckNPC;

        public override void SetStaticDefaults()
        {
            Main.projFrames[Projectile.type] = 3;
        }

        public override void SetDefaults()
        {
            Projectile.width = 30;
            Projectile.height = 30;
            Projectile.friendly = true;
            Projectile.penetrate = -1;
            Projectile.timeLeft = 360;
            Projectile.DamageType = DamageClass.Magic;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 10;
        }

        public override bool ShouldUpdatePosition()
        {
            return !Stuck;
        }

        public override bool? CanDamage()
        {
            return Projectile.numHits == 0;
        }

        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            if (HasStuck)
            {
                Projectile.velocity.X *= 0.86f;
                return false;
            }
            return true;
        }

        public override void AI()
        {
            if (Projectile.frame == 0)
                Projectile.frame = Main.rand.Next(3) + 1;

            //Stay anchored to the enemy it's stuck in and emit charge particles.
            if (Stuck)
            {
                Projectile.Center = StuckNPC.Center + offsetFromStuckNPC.RotatedBy(StuckNPC.rotation) * StuckNPC.scale;
                Projectile.rotation = StuckNPC.rotation + rotationFromStuckNPC;

                Projectile.tileCollide = false;
                Projectile.timeLeft++;
                ManaCharge++;

                if (Main.rand.NextBool(5))
                {
                    Particle lilStar = new CuteManaStarParticle(Projectile.Center + Main.rand.NextVector2Circular(19f, 19f), Main.rand.NextVector2Circular(4f, 4f) - Vector2.UnitY * 6f * Main.rand.NextFloat(0.6f, 1.2f), Main.rand.NextFloat(0.8f, 1.8f), lifetime: Main.rand.Next(14) + 14);
                    GeneralParticleHandler.SpawnParticle(lilStar);
                }
            }

            //Act as a magnetized collectible.
            else if (HasStuck)
            {
                if (DetachmentEffectsComplete != 1f)
                {
                    Projectile.tileCollide = true;
                    Projectile.timeLeft = 360;
                    DetachmentEffectsComplete = 1f;
                    Projectile.velocity = Vector2.UnitY.RotatedByRandom(MathHelper.PiOver2) * -1 * (Main.rand.NextFloat(5f) + 5f);
                }

                if (Projectile.velocity.X != 0)
                    Projectile.rotation += 0.02f * Math.Sign(Projectile.velocity.X) * Math.Clamp(Projectile.velocity.Length(), 0f, 5f);

                Projectile.velocity.X *= 0.987f;
                Projectile.velocity.Y += 0.8f;

                float distanceToOwner = (Owner.Center - Projectile.Center).Length();
                if (distanceToOwner < 170f)
                {
                    Projectile.timeLeft++;

                    Projectile.velocity = (Owner.Center - Projectile.Center).SafeNormalize(Vector2.Zero) * Math.Max(distanceToOwner * 0.09f, 8f);

                    if (distanceToOwner < 10f)
                        Projectile.Kill();
                }

                //particles

                if (Main.rand.NextBool())
                {
                    Dust dust = Dust.NewDustPerfect(Projectile.Center + Main.rand.NextVector2Circular(15f, 15f), 45, Vector2.UnitY * -7f, Alpha: Main.rand.Next(100) + 120, Scale: Main.rand.NextFloat(1f, 2f));
                    dust.noGravity = true; 
                }

                if (Main.rand.NextBool(5))
                {
                    Particle lilStar = new CuteManaStarParticle(Projectile.Center + Main.rand.NextVector2Circular(19f, 19f), -Vector2.UnitY * 6f * Main.rand.NextFloat(0.6f, 1.2f), Main.rand.NextFloat(0.8f, 1.8f), lifetime: Main.rand.Next(14) + 14);
                    GeneralParticleHandler.SpawnParticle(lilStar);
                }
            }

            //the flung state.
            else
            {
                float fallSpeed = Projectile.velocity.Y;
                if (Projectile.velocity.X != 0)
                    Projectile.rotation += 0.02f * Math.Sign(Projectile.velocity.X) * Math.Clamp(Projectile.velocity.Length(), 0f, 5f);
                
                if (Projectile.timeLeft < 345)
                    Projectile.velocity += Vector2.UnitY * 0.5f * (1 - Math.Clamp((Projectile.timeLeft - 310f) / 35f, 0f, 1f));

                Projectile.velocity *= 0.98f;

                if (Projectile.velocity.Y > 0)
                    Projectile.velocity.Y = Math.Clamp(Projectile.velocity.Y, 0,  Math.Max(18f, fallSpeed));

                //Sharticles
                if (Main.rand.NextBool())
                {
                    Dust dust = Dust.NewDustPerfect(Projectile.Center + Main.rand.NextVector2Circular(15f, 15f), 15, Projectile.velocity * 0.3f, Alpha: Main.rand.Next(100) + 120, Scale: Main.rand.NextFloat(1f, 2f));
                    dust.noGravity = true;
                }

                if (Main.rand.NextBool(3))
                {
                    Particle lilStar = new CuteManaStarParticle(Projectile.Center, Projectile.velocity * 0.2f + Main.rand.NextVector2Circular(5f, 5f) - Vector2.UnitY * 3f, Main.rand.NextFloat(0.8f, 1.8f), lifetime: Main.rand.Next(14) + 14);
                    GeneralParticleHandler.SpawnParticle(lilStar);
                }
            }
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            SoundEngine.PlaySound(SoundID.GlommerBounce, Projectile.Center);
            Projectile.velocity = Vector2.Zero;
            Projectile.ai[1] = target.whoAmI; //Assign the stuck npc.
            offsetFromStuckNPC = (Projectile.Center - target.Center).RotatedBy(-target.rotation) / target.scale;
            rotationFromStuckNPC = Projectile.rotation - target.rotation;
        }

        public override void OnKill(int timeLeft)
        {
            if ((Owner.Center - Projectile.Center).Length() < 10f && HasStuck)
            {
                SoundEngine.PlaySound(SoundID.Item28 with { Volume = SoundID.Item28.Volume * 0.8f }, Projectile.Center);

                //To reach full mana, it needs to have stayed at least half the time needed. If you just one shot an enemy with it, you'll only get 10 mana
                int manaGained = Math.Max(10, (int)Math.Floor(150 * Math.Clamp(ManaCharge * 2f / FullMana, 0f, 1f)));
                Owner.statMana += manaGained;
                if (Main.myPlayer == Owner.whoAmI)
                    Owner.ManaEffect(manaGained);

                if (Owner.statMana > Owner.statManaMax2)
                    Owner.statMana = Owner.statManaMax2;

                Owner.AddBuff(ModContent.BuffType<CoralSymbiosis>(), CoralSpout.SymbiosisTime);

                return;
            }

            //don't play the thuk sound & do the particles if it dies of natural causes
            if (timeLeft != 0)
            {
                SoundEngine.PlaySound(SoundID.Item171, Projectile.Center);

                //Particles, with extra ones if it crashed chargeless
                for (int i = 0; i < 8; i++)
                {
                    float angle = Main.rand.NextFloat(MathHelper.TwoPi);

                    Particle spike = new UrchinSpikeParticle(Projectile.Center + angle.ToRotationVector2() * 2f, angle.ToRotationVector2() * 6f, angle + MathHelper.PiOver2, Main.rand.NextFloat(1f, 1.3f), lifetime: Main.rand.Next(10) + 25);
                    GeneralParticleHandler.SpawnParticle(spike);
                }

                int dustCount = Main.rand.Next(7);
                for (int i = 0; i < dustCount; i++)
                {
                    int dustOpacity = (int)(200 * Main.rand.NextFloat(0.5f, 1f));
                    float dustScale = Main.rand.NextFloat(1f, 1.4f);
                    int dustType = CoralSpike.DustPick; //Epic reuse of static method babey
                    Vector2 dustVelocity = (dustType == 255 ? 1 : -1) * Projectile.velocity.RotatedByRandom(MathHelper.PiOver4) * 0.6f + Main.rand.NextVector2Circular(7f, 7f);

                    Dust dust = Dust.NewDustPerfect(Projectile.Center, dustType, dustVelocity, Alpha: dustOpacity, Scale: dustScale);
                    dust.noGravity = true;
                }

                int goreNumber = Main.rand.Next(4);

                for (int i = 0; i < goreNumber; i++)
                {
                    int goreID = Main.rand.NextBool() ? 266 : Main.rand.NextBool() ? 971 : 972;
                    Gore bone = Gore.NewGorePerfect(Projectile.GetSource_FromAI(), Projectile.position, Projectile.velocity * -0.2f + Main.rand.NextVector2Circular(5f, 5f), goreID);
                    bone.scale = Main.rand.NextFloat(0.6f, 1f) * (goreID == 972 ? 0.7f : 1f); //Shrink the larger bones
                    bone.type = goreID; //Gotta do that or else itll spawn gores from the general pool :(
                }
            }

            else if (HasStuck)
            {
                for (int i = 0; i < 14; i++)
                {
                    Vector2 direction = Main.rand.NextVector2Circular(10f, 10f);
                    Dust dust = Dust.NewDustPerfect(Projectile.Center + direction, 45, direction + Vector2.UnitY * -7f, Alpha: Main.rand.Next(100) + 120, Scale: Main.rand.NextFloat(1f, 2f));
                    dust.noGravity = true;
                }

                SoundEngine.PlaySound(SoundID.Item29 with { Volume = SoundID.Item29.Volume * 0.4f, Pitch = -0.3f }, Projectile.Center);
            }

        }

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D texture = ModContent.Request<Texture2D>(Texture).Value;
            Vector2 position = Projectile.Center - Main.screenPosition;
            int variant = (int)(Projectile.frame - 1);
            Rectangle frame = new Rectangle(0, 26 * variant, 24, 24);
            float scale = 1.3f;

            Main.EntitySpriteDraw(texture, position, frame, lightColor, Projectile.rotation, frame.Size() / 2f, scale, 0, 0);

            if (Stuck || HasStuck)
            {
                Main.spriteBatch.End();
                Main.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.Additive, Main.DefaultSamplerState, DepthStencilState.None, Main.Rasterizer, null, Main.GameViewMatrix.TransformationMatrix);

                Texture2D bloomTex = ModContent.Request<Texture2D>("CalamityMod/Particles/BloomCircle").Value;
                float bloomSize = (float)Math.Sin(Main.GlobalTimeWrappedHourly * 3f) * 0.1f + 0.4f;

                float period = (int)(FullMana / 4f);

                float pulseProgress = Main.GlobalTimeWrappedHourly % period / period;

                if (Stuck)
                {
                    pulseProgress = (ManaCharge % period) / period;
                }

                float overimageSize = scale + (float)Math.Sqrt(pulseProgress) * 0.4f;
                float overimageOpacity = (float)Math.Sin(pulseProgress * MathHelper.PiOver2 + MathHelper.PiOver2) * 0.7f;

                Main.EntitySpriteDraw(bloomTex, position, null, Color.DodgerBlue * 0.3f, 0, bloomTex.Size() / 2f, bloomSize, 0, 0);

                Main.EntitySpriteDraw(texture, position, frame, Color.DodgerBlue * overimageOpacity, Projectile.rotation, frame.Size() / 2f, overimageSize, 0, 0);

                Main.spriteBatch.End();
                Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, Main.Rasterizer, null, Main.GameViewMatrix.TransformationMatrix);
            }


            return false;
        }
    }
}
