using System;
using System.Collections.Generic;
using System.IO;
using CalamityMod.Graphics.Primitives;
using CalamityMod.Projectiles.BaseProjectiles;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Terraria;
using Terraria.Audio;
using Terraria.Enums;
using Terraria.Graphics.Shaders;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Magic
{
    public class Teslabeam : BaseLaserbeamProjectile, ILocalizedModType
    {
        public new string LocalizationCategory => "Projectiles.Magic";
        public override string Texture => "CalamityMod/Projectiles/InvisibleProj";
        public Player Owner => Main.player[Projectile.owner];
        public override Color LightCastColor => new Color(92, 144, 245);
        public override float Lifetime => 18000f;
        public override float MaxScale => 1f;
        public override float MaxLaserLength => 1600f; //100 tiles
        public override Texture2D LaserBeginTexture => ModContent.Request<Texture2D>("CalamityMod/Projectiles/InvisibleProj", AssetRequestMode.ImmediateLoad).Value;
        public override Texture2D LaserMiddleTexture => ModContent.Request<Texture2D>("CalamityMod/Projectiles/InvisibleProj", AssetRequestMode.ImmediateLoad).Value;
        public override Texture2D LaserEndTexture => ModContent.Request<Texture2D>("CalamityMod/Projectiles/InvisibleProj", AssetRequestMode.ImmediateLoad).Value;

        List<Vector2> offsetPoints = new List<Vector2>();
        NPC Victim;
        public float damageMultiplier = 1f; // a multiplier for how much damage the beam deals
        public bool damageShouldDecay = false; // if this is true, the weapon's damage will decay if the grace period is over
        public float decayGracePeriod = 0; // while the grace period is above 0, the weapon's damage won't decay

        public const float MaxDamageMultiplier = 5f; // the maximum damage multiplier
        public const float GracePeriod = 10; // this controls the amount of frames that the grace period lasts for. It is combined with the beam's i frame cooldown
        public const float DamagePerHit = 0.1f; // how much the damage multiplier increases for each enemy hit
        public const float DamageDecayPerFrame = 0.01f; // how much the damage multiplier decreases if damage is decaying
        public const float AimResponsiveness = 0.965f; // Last Prism is 0.92f. Lower makes the laser turn faster.

        public override void SetStaticDefaults()
        {
            Main.projFrames[Projectile.type] = 1;
        }

        public override void SetDefaults()
        {
            Projectile.width = 10;
            Projectile.height = 10;
            Projectile.friendly = true;
            Projectile.DamageType = DamageClass.Magic;
            Projectile.penetrate = -1;
            Projectile.tileCollide = false;
            Projectile.hide = true;
            Projectile.timeLeft = 18000;
            Projectile.usesIDStaticNPCImmunity = true;
            Projectile.idStaticNPCHitCooldown = 6;
        }
        public override void SendExtraAI(BinaryWriter writer)
        {
            writer.Write(damageMultiplier);
            writer.Write(decayGracePeriod);
            writer.Write(damageShouldDecay);
        }

        public override void ReceiveExtraAI(BinaryReader reader)
        {
            damageMultiplier = reader.ReadSingle();
            decayGracePeriod = reader.ReadSingle();
            damageShouldDecay = reader.ReadBoolean();
        }

        public override bool PreAI()
        {
            // Multiplayer support here, only run this code if the client running it is the owner of the projectile
            if (Projectile.owner == Main.myPlayer)
            {
                Vector2 rrp = Owner.RotatedRelativePoint(Owner.MountedCenter, true);
                UpdateAim(rrp);
                Projectile.direction = Main.MouseWorld.X > Owner.Center.X ? 1 : -1;
                Projectile.netUpdate = true;
            }

            int dir = Math.Sign(Projectile.velocity.X);
            Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.PiOver2;
            Projectile.Center = Owner.Center + Projectile.velocity * 56f; //Distance offset
            Projectile.timeLeft = 18000; //Infinite lifespan
            Owner.ChangeDir(dir);
            Owner.heldProj = Projectile.whoAmI;
            Owner.itemTime = 2;
            Owner.itemAnimation = 2;
            Owner.itemRotation = ((Projectile.rotation + MathHelper.PiOver2).ToRotationVector2() * -Owner.direction).ToRotation();

            if (!Owner.channel)
            {
                Projectile.Kill();
                return false;
            }
            // Do we still have enough mana? If not, we kill the projectile because we cannot use it anymore
            if (Owner.miscCounter % 10 == 0 && !Owner.CheckMana(Owner.ActiveItem(), -1, true))
            {
                Projectile.Kill();
                return false;
            }

            Projectile.ai[2]++;

            // Play zap sounds idly
            if (Projectile.ai[2] % 5 == 0)
            {
                SoundEngine.PlaySound(SoundID.DD2_LightningBugZap with { Pitch = 1.1f }, Projectile.Center);
            }
            // If the weapon's target hasn't just been hit, tick down the grace period timer
            if (decayGracePeriod > 0 && damageShouldDecay)
            {
                decayGracePeriod--;
            }

            // If the grace period timer runs out, start decaying damage
            if (damageShouldDecay && damageMultiplier > 1f && decayGracePeriod <= 0)
            {
                damageMultiplier = MathHelper.Max(damageMultiplier - DamageDecayPerFrame, 1f);
            }
            // Update the beam's damage
            Projectile.damage = (int)MathHelper.Clamp(Projectile.originalDamage * damageMultiplier, 0, Projectile.originalDamage * MaxDamageMultiplier);
            // After damage has been set, set the decay flag to true. If the beams hits the target, this will be set back to false, but if not, damage will start decaying in the next tick
            damageShouldDecay = true;

            return true;
        }

        internal float WidthFunction(float completionRatio)
        {
            return MathHelper.Clamp(completionRatio * 15, 1, 1.5f);
        }

        internal Color ColorFunction(float completionRatio)
        {
            return new Color(174, 227, 244); // directly color picked from the source material
        }

        internal float BackgroundWidthFunction(float completionRatio) => WidthFunction(completionRatio) * 4f;

        internal Color BackgroundColorFunction(float completionRatio)
        {
            return new Color(92, 144, 245) * 0.6f; // directly color picked from the source material
        }
        public override bool PreDraw(ref Color lightColor)
        {
            Main.spriteBatch.EnterShaderRegion();
            GameShaders.Misc["CalamityMod:TeslaTrail"].SetShaderTexture(ModContent.Request<Texture2D>("CalamityMod/ExtraTextures/Trails/ZapTrail"));

            // every 2 frames update the offsets are randomized. This is effectively control for the lightning's fps
            if (Projectile.ai[2] % 2 == 0)
            {
                offsetPoints.Clear();
                for (int i = 0; i <= 75; i++)
                {
                    Vector2 baseVec = Vector2.Zero;
                    float width = 16;
                    if (i > 0)
                    {
                        baseVec += Main.rand.NextVector2Square(-width, width);
                    }
                    offsetPoints.Add(Main.rand.NextVector2Square(-width, width));
                }
            }
            // do not try to draw the lightning if the offset list isn't filled yet or an index error will occur
            if (offsetPoints.Count < 75)
                return false;

            // the final list of points that will be used to draw the lightning which combines a series of points travelling up the beam with the random values from the offset list
            List<Vector2> finalPoints = new List<Vector2>();
            for (int i = 0; i <= 75; i++)
            {
                Vector2 baseVec = Vector2.Lerp(Projectile.Center, Projectile.Center + Projectile.velocity * LaserLength, i / 73.5f);
                float width = 16 + (i / 75 * 10);
                if (i > 0)
                {
                    baseVec += offsetPoints[i];
                }
                finalPoints.Add(baseVec);
            }

            // 29FEB2024: Ozzatron: hopefully ported this correctly to the new prim system by Toasty
            PrimitiveRenderer.RenderTrail(finalPoints, new(BackgroundWidthFunction, BackgroundColorFunction, smoothen: false, shader: GameShaders.Misc["CalamityMod:TeslaTrail"]), 75);
            PrimitiveRenderer.RenderTrail(finalPoints, new(WidthFunction, ColorFunction, smoothen: false, shader: GameShaders.Misc["CalamityMod:TeslaTrail"]), 75);

            Main.spriteBatch.ExitShaderRegion();
            return false;
        }
        // Gently adjusts the aim vector of the laser to point towards the mouse.
        private void UpdateAim(Vector2 source)
        {
            Vector2 aimVector = Vector2.Normalize(Main.MouseWorld - source);
            if (aimVector.HasNaNs())
                aimVector = -Vector2.UnitY;
            aimVector = Vector2.Normalize(Vector2.Lerp(aimVector, Vector2.Normalize(Projectile.velocity), AimResponsiveness));

            if (aimVector != Projectile.velocity)
                Projectile.netUpdate = true;
            Projectile.velocity = aimVector;
        }

        public override bool ShouldUpdatePosition() => false;

        // Update CutTiles so the laser will cut tiles (like grass).
        public override void CutTiles()
        {
            DelegateMethods.tilecut_0 = TileCuttingContext.AttackProjectile;
            Vector2 unit = Projectile.velocity;
            Utils.PlotTileLine(Projectile.Center, Projectile.Center + unit * LaserLength, Projectile.width + 16, DelegateMethods.CutTiles);
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            // if the beam's damage multiplier is at default value, the hit enemy becomes the beam's new target
            if (damageMultiplier <= 1f)
            {
                Victim = target;
            }
            // if the beam hits its current target, disable damage decay for the frame, reset its grace period, and increase its damage
            if (Victim == target)
            {
                damageShouldDecay = false;
                decayGracePeriod = Projectile.idStaticNPCHitCooldown + GracePeriod;
                if (damageMultiplier < MaxDamageMultiplier)
                {
                    damageMultiplier = Math.Min(damageMultiplier + DamagePerHit, MaxDamageMultiplier);
                }
            }
            target.AddBuff(BuffID.Electrified, 30);
        }
    }
}
