using System;
using System.IO;
using CalamityMod.Items.Weapons.Melee;
using CalamityMod.Particles;
using CalamityMod.Sounds;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.Audio;
using Terraria.Graphics.Shaders;
using Terraria.ID;
using Terraria.ModLoader;
using static Terraria.ModLoader.ModContent;

namespace CalamityMod.Projectiles.Melee
{
    public class SanguineFury : ModProjectile, ILocalizedModType
    {
        public new string LocalizationCategory => "Projectiles.Melee";
        public override string Texture => "CalamityMod/Projectiles/Melee/TrueBiomeBlade_SanguineFury";
        private bool initialized = false;
        Vector2 direction = Vector2.Zero;
        public ref float Shred => ref Projectile.ai[0]; //How much the attack is, attacking
        public float ShredRatio => MathHelper.Clamp(Shred / (maxShred * 0.5f), 0f, 1f);
        public ref float PogoCooldown => ref Projectile.ai[1]; //Cooldown for the pogo
        public ref float BounceTime => ref Projectile.localAI[0];
        public ref float ChargeSoundCooldown => ref Projectile.localAI[1];
        public Player Owner => Main.player[Projectile.owner];
        public bool CanPogo => Owner.velocity.Y != 0 && PogoCooldown <= 0; //Only pogo when in the air and if the cooldown is zero

        public const float pogoStrenght = 16f; //How much the player gets pogoed up
        public const float maxShred = 500; //How much shred you get

        public Projectile Wheel;
        public bool Dashing;
        public Vector2 DashStart;

        public override void SetDefaults()
        {
            Projectile.DamageType = DamageClass.Melee;
            Projectile.ContinuouslyUpdateDamageStats = true;
            Projectile.width = Projectile.height = 70;
            Projectile.tileCollide = false;
            Projectile.friendly = true;
            Projectile.penetrate = -1;
            Projectile.extraUpdates = 1;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = OmegaBiomeBlade.SuperPogoAttunement_LocalIFrames;
            Projectile.timeLeft = OmegaBiomeBlade.SuperPogoAttunement_LocalIFrames;
        }

        public override bool? CanDamage() => Projectile.timeLeft <= 2; //Prevent spam click abuse

        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            float collisionPoint = 0f;
            float bladeLength = 130 * Projectile.scale;
            float bladeWidth = 86 * Projectile.scale;

            return Collision.CheckAABBvLineCollision(targetHitbox.TopLeft(), targetHitbox.Size(), Owner.Center, Owner.Center + (direction * bladeLength), bladeWidth, ref collisionPoint);
        }

        public void Pogo()
        {
            if (CanPogo && Main.myPlayer == Owner.whoAmI)
            {
                Owner.velocity = -direction.SafeNormalize(Vector2.Zero) * pogoStrenght; //Bounce
                Owner.fallStart = (int)(Owner.position.Y / 16f);
                PogoCooldown = 30; //Cooldown
                SoundEngine.PlaySound(SoundID.DD2_MonkStaffGroundImpact, Projectile.position);

                Vector2 hitPosition = Owner.Center + (direction * 100 * Projectile.scale);
                BounceTime = 20f; //Used only for animation

                for (int i = 0; i < 8; i++)
                {
                    Vector2 hitPositionDisplace = direction.RotatedBy(MathHelper.PiOver2) * Main.rand.NextFloat(-10f, 10f);
                    Vector2 flyDirection = -direction.RotatedBy(Main.rand.NextFloat(-MathHelper.PiOver4, MathHelper.PiOver4));
                    Particle smoke = new SmallSmokeParticle(hitPosition + hitPositionDisplace, flyDirection * 9f, Color.Crimson, new Color(130, 130, 130), Main.rand.NextFloat(1.8f, 2.6f), 155 - Main.rand.Next(30));
                    GeneralParticleHandler.SpawnParticle(smoke);

                    Particle Glow = new StrongBloom(hitPosition - hitPositionDisplace * 3, -direction * 6 * Main.rand.NextFloat(0.5f, 1f), Color.Crimson * 0.5f, 0.01f + Main.rand.NextFloat(0f, 0.2f), 20 + Main.rand.Next(40));
                    GeneralParticleHandler.SpawnParticle(Glow);
                }
                for (int i = 0; i < 3; i++)
                {
                    Vector2 hitPositionDisplace = direction.RotatedBy(MathHelper.PiOver2) * Main.rand.NextFloat(-10f, 10f);
                    Vector2 flyDirection = -direction.RotatedBy(Main.rand.NextFloat(-MathHelper.PiOver4, MathHelper.PiOver4));

                    Particle Rock = new StoneDebrisParticle(hitPosition - hitPositionDisplace * 3, flyDirection * Main.rand.NextFloat(3f, 6f), Color.Beige, 1f + Main.rand.NextFloat(0f, 0.4f), 30 + Main.rand.Next(50), 0.1f);
                    GeneralParticleHandler.SpawnParticle(Rock);
                }
            }
        }

        public override void AI()
        {
            if (!initialized) //Initialization. Here its litterally just playing a sound tho lmfao
            {
                SoundEngine.PlaySound(SoundID.Item90, Projectile.Center);
                initialized = true;

                foreach (Projectile proj in Main.projectile)
                {
                    if (proj.active && proj.type == ProjectileType<SanguineFuryWheel>() && proj.owner == Owner.whoAmI)
                    {
                        // 15NOV2024: Ozzatron: clamped mouse position unnecessary, only used for direction
                        if (CalamityUtils.AngleBetween(Owner.Center - Owner.Calamity().mouseWorld, Owner.Center - proj.Center) > MathHelper.PiOver4)
                        {
                            proj.Kill();
                            break;
                        }

                        Wheel = proj;
                        Dashing = true;
                        DashStart = Owner.Center;
                        Wheel.timeLeft = 60;

                        // 17APR2024: Ozzatron: True Biome Blade's pogo gives iframes when striking enemies in a similar manner to a bonk dash.
                        // This is a fixed and intentionally very low number of iframes, and is not boosted by Cross Necklace.
                        Owner.GiveUniversalIFrames(OmegaBiomeBlade.SuperPogoAttunement_PlayerSliceIFrames);

                        break;
                    }
                }
            }

            if (Owner.CantUseHoldout())
            {
                Projectile.Kill();
                return;
            }

            if (Shred >= maxShred)
                Shred = maxShred;
            if (Shred < 0)
                Shred = 0;

            Lighting.AddLight(Projectile.Center, new Vector3(1f, 0.56f, 0.56f) * ShredRatio);

            // 15NOV2024: Ozzatron: clamped mouse position unnecessary, only used for direction
            //Manage position and rotation
            direction = Owner.SafeDirectionTo(Owner.Calamity().mouseWorld, Vector2.Zero);
            direction.Normalize();
            Projectile.rotation = direction.ToRotation();
            Projectile.Center = Owner.Center + (direction * 60);

            //Scaling based on shred
            Projectile.scale = 1.25f + (ShredRatio * 1f); //SWAGGER


            if ((Wheel == null || !Wheel.active) && Dashing)
            {
                Dashing = false;
                Owner.velocity *= 0.1f; //Abrupt stop

                SoundEngine.PlaySound(CommonCalamitySounds.MeatySlashSound, Projectile.Center);
                if (Owner.whoAmI == Main.myPlayer)
                {
                    Projectile proj = Projectile.NewProjectileDirect(Projectile.GetSource_FromThis(), Owner.Center - DashStart / 2f, Vector2.Zero, ProjectileType<SanguineFuryDash>(), (int)(Projectile.damage * OmegaBiomeBlade.SuperPogoAttunement_SliceDamageMult), 0, Owner.whoAmI);
                    if (proj.ModProjectile is SanguineFuryDash dash)
                    {
                        dash.DashStart = DashStart;
                        dash.DashEnd = Owner.Center;
                    }
                }
            }

            Owner.Calamity().LungingDown = false;

            if (Dashing)
            {
                Owner.Calamity().LungingDown = true;
                Owner.fallStart = (int)(Owner.position.Y / 16f);
                Owner.velocity = Owner.SafeDirectionTo(Wheel.Center, Vector2.Zero) * 60f;

                if (Owner.Distance(Wheel.Center) < 60f)
                    Wheel.active = false;
            }


            if (Collision.SolidCollision(Owner.Center + (direction * 100 * Projectile.scale) - Vector2.One * 5f, 10, 10) && !Dashing)
            {
                Pogo();
                Projectile.ForceNetUpdate();
            }

            //Make the owner look like theyre holding the sword bla bla
            Owner.heldProj = Projectile.whoAmI;
            Owner.ChangeDir(Math.Sign(direction.X));
            Owner.itemRotation = direction.ToRotation();
            if (Owner.direction != 1)
            {
                Owner.itemRotation -= MathHelper.Pi;
            }
            Owner.itemRotation = MathHelper.WrapAngle(Owner.itemRotation);
            Owner.itemTime = 2;
            Owner.itemAnimation = 2;

            //Play a sound when the blade throw is available
            if (ShredRatio > 0.25 && Owner.whoAmI == Main.myPlayer)
            {
                if (ChargeSoundCooldown <= 0)
                {
                    var chargeSound = SoundEngine.PlaySound(SoundID.DD2_SonicBoomBladeSlash with { Volume = SoundID.DD2_SonicBoomBladeSlash.Volume * 2.5f });
                    ChargeSoundCooldown = 20;
                }
            }
            else
            {
                ChargeSoundCooldown--;
            }

            Shred -= OmegaBiomeBlade.SuperPogoAttunement_ShredDecayRate;
            PogoCooldown--;
            BounceTime--;
            if (Projectile.timeLeft <= 2)
                Projectile.timeLeft = 2;
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone) => ShredTarget();
        public override void OnHitPlayer(Player target, Player.HurtInfo info) => ShredTarget();

        private void ShredTarget()
        {
            if (Main.myPlayer != Owner.whoAmI)
                return;

            if (Owner.HeldItem.ModItem is OmegaBiomeBlade sword && Main.rand.NextFloat() <= OmegaBiomeBlade.SuperPogoAttunement_ShredderProc)
                sword.OnHitProc = true;

            Owner.fallStart = (int)(Owner.position.Y / 16f);
            if (PogoCooldown <= 0)
            {
                SoundEngine.PlaySound(SoundID.NPCHit30, Projectile.Center); //Sizzle
                Shred += 62; //Augment the shredspeed

                // 17APR2024: Ozzatron: True Biome Blade's shred pogo gives iframes when striking enemies in a similar manner to a bonk dash.
                // This is a fixed and intentionally very low number of iframes, and is not boosted by Cross Necklace.
                Owner.GiveUniversalIFrames(OmegaBiomeBlade.SuperPogoAttunement_PlayerShredIFrames);

                PogoCooldown = 20;
            }
        }

        public override void OnKill(int timeLeft)
        {
            SoundEngine.PlaySound(SoundID.NPCHit43, Projectile.Center);
            if (ShredRatio > 0.8 && Owner.whoAmI == Main.myPlayer)
            {
                Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center, direction * 16f, ProjectileType<SanguineFuryWheel>(), (int)(Projectile.damage * OmegaBiomeBlade.SuperPogoAttunement_ShotDamageMult), Projectile.knockBack, Owner.whoAmI, Shred);
            }
            if (Dashing)
            {
                Owner.velocity *= 0.1f; //Abrupt stop
            }
            Owner.Calamity().LungingDown = false;
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D handle = Request<Texture2D>("CalamityMod/Items/Weapons/Melee/OmegaBiomeBlade").Value;
            Texture2D blade = Request<Texture2D>("CalamityMod/Projectiles/Melee/TrueBiomeBlade_SanguineFury").Value;

            int bladeAmount = 4;

            float drawAngle = direction.ToRotation();
            float drawRotation = drawAngle + MathHelper.PiOver4;

            Vector2 drawOrigin = new Vector2(0f, handle.Height);
            Vector2 drawOffset = Owner.Center + direction * 10f - Main.screenPosition;

            Main.EntitySpriteDraw(handle, drawOffset, null, lightColor, drawRotation, drawOrigin, Projectile.scale, 0f, 0);

            //Turn on additive blending
            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.Additive, Main.DefaultSamplerState, DepthStencilState.None, Main.Rasterizer, null, Main.GameViewMatrix.TransformationMatrix);

            GameShaders.Misc["CalamityMod:BasicTint"].UseOpacity(MathHelper.Clamp(BounceTime, 0f, 20f) / 20f);
            GameShaders.Misc["CalamityMod:BasicTint"].UseColor(new Color(207, 248, 255));
            GameShaders.Misc["CalamityMod:BasicTint"].Apply();

            //Update the parameters
            drawOrigin = new Vector2(0f, blade.Height);

            Main.EntitySpriteDraw(blade, drawOffset, null, Color.Lerp(Color.White, lightColor, 0.5f) * 0.9f, drawRotation, drawOrigin, Projectile.scale, 0f, 0);


            for (int i = 0; i < bladeAmount; i++) //Draw extra copies
            {
                blade = Request<Texture2D>("CalamityMod/Projectiles/Melee/TrueBiomeBlade_SanguineFuryExtra").Value;

                drawAngle = direction.ToRotation();

                float circleCompletion = (float)Math.Sin(Main.GlobalTimeWrappedHourly * 5 + i * MathHelper.PiOver2);
                drawRotation = drawAngle + MathHelper.PiOver4 + (circleCompletion * MathHelper.Pi / 10f) - (circleCompletion * (MathHelper.Pi / 9f) * ShredRatio);

                drawOrigin = new Vector2(0f, blade.Height);

                Vector2 drawOffsetStraight = Owner.Center + direction * (float)Math.Sin(Main.GlobalTimeWrappedHourly * 7) * 10 - Main.screenPosition; //How far from the player
                Vector2 drawDisplacementAngle = direction.RotatedBy(MathHelper.PiOver2) * circleCompletion.ToRotationVector2().Y * (20 + 40 * ShredRatio); //How far perpendicularly
                Vector2 drawOffsetFromBounce = direction * MathHelper.Clamp(BounceTime, 0f, 20f) / 20f * 20f;

                Main.EntitySpriteDraw(blade, drawOffsetStraight + drawDisplacementAngle + drawOffsetFromBounce, null, Color.Lerp(Color.White, lightColor, 0.5f) * 0.8f, drawRotation, drawOrigin, Projectile.scale, 0f, 0);
            }

            //Back to normal
            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, Main.Rasterizer, null, Main.GameViewMatrix.TransformationMatrix);

            return false;
        }

        public override void SendExtraAI(BinaryWriter writer)
        {
            writer.Write(initialized);
            writer.WriteVector2(direction);
        }
        public override void ReceiveExtraAI(BinaryReader reader)
        {
            initialized = reader.ReadBoolean();
            direction = reader.ReadVector2();
        }
    }
}
