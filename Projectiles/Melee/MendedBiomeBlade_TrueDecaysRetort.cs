using CalamityMod.Items.Weapons.Melee;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.IO;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using static Terraria.ModLoader.ModContent;
using Terraria.Audio;
using CalamityMod.Sounds;

namespace CalamityMod.Projectiles.Melee
{
    public class TrueDecaysRetort : ModProjectile, ILocalizedModType
    {
        public new string LocalizationCategory => "Projectiles.Melee";
        public override string Texture => "CalamityMod/Projectiles/Melee/MendedBiomeBlade_DecaysRetort";
        private bool initialized = false;
        public Vector2 direction = Vector2.Zero;
        public ref float MaxTime => ref Projectile.ai[0];
        public ref float CanLunge => ref Projectile.ai[1];
        public float Timer => MaxTime - Projectile.timeLeft;
        public bool ChargedUp;
        public Player Owner => Main.player[Projectile.owner];
        public const float LungeSpeed = 16;
        public ref float CanBounce => ref Projectile.localAI[0];
        public ref float dashTimer => ref Projectile.localAI[1];
        public const float maxDash = 20f;

        private Vector2 PowerLungeStart;

        public override void SetStaticDefaults()
        {
        }
        public override void SetDefaults()
        {
            Projectile.DamageType = DamageClass.Melee;
            Projectile.width = Projectile.height = 84;
            Projectile.width = Projectile.height = 84;
            Projectile.tileCollide = false;
            Projectile.friendly = true;
            Projectile.penetrate = -1;
            Projectile.extraUpdates = 1;
        }

        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            float collisionPoint = 0f;
            float bladeLength = 140f * Projectile.scale;
            Vector2 displace = direction * ((float)Math.Sin(Timer / MaxTime * 3.14f) * 60);

            return Collision.CheckAABBvLineCollision(targetHitbox.TopLeft(), targetHitbox.Size(), Owner.Center + displace, Owner.Center + displace + (direction * bladeLength), 24, ref collisionPoint);
        }

        public override void AI()
        {
            if (!initialized) //Initialization
            {
                CanBounce = 1f;
                Projectile.timeLeft = (int)MaxTime;
                direction = Owner.SafeDirectionTo(Owner.Calamity().mouseWorld, Vector2.Zero);
                direction.Normalize();
                Projectile.rotation = direction.ToRotation();
                if (CanLunge == 1f && !ChargedUp)
                    Lunge();
                SoundEngine.PlaySound(SoundID.Item103, Projectile.Center);
                initialized = true;
                Projectile.netUpdate = true;
                Projectile.netSpam = 0;
            }

            if (ChargedUp && dashTimer == 0f)
                PowerLunge();

            if (dashTimer >= 1f)
            {
                if (dashTimer < maxDash)
                {
                    Owner.fallStart = (int)(Owner.position.Y / 16f);
                    Owner.velocity = direction * 60f;
                    Projectile.timeLeft = (int)(MaxTime / 2f);
                    dashTimer++;
                }

                if (dashTimer == maxDash)
                {
                    Owner.velocity *= 0.1f; //Abrupt stop
                    Owner.Calamity().LungingDown = false;

                    SoundEngine.PlaySound(CommonCalamitySounds.SwiftSliceSound, Owner.Center);

                    if (Owner.whoAmI == Main.myPlayer)
                    {
                        Projectile proj = Projectile.NewProjectileDirect(Projectile.GetSource_FromThis(), Owner.Center - PowerLungeStart / 2f, Vector2.Zero, ProjectileType<DecaysRetortDash>(), (int)(Projectile.damage * TrueBiomeBlade.EvilAttunement_SlashDamageBoost), 0, Owner.whoAmI);
                        if (proj.ModProjectile is DecaysRetortDash dash)
                        {
                            dash.DashStart = PowerLungeStart;
                            dash.DashEnd = Owner.Center;
                        }
                    }
                    dashTimer = maxDash + 1;
                }
            }

            //Manage position and rotation
            Projectile.scale = 1f + ((float)Math.Sin(Timer / MaxTime * MathHelper.Pi) * 0.6f); //SWAGGER
            Projectile.Center = Owner.Center + (direction * ((float)Math.Sin(Timer / MaxTime * MathHelper.Pi) * 60));

            Lighting.AddLight(Projectile.Center, new Vector3(0.9f, 0f, 0.35f) * (float)Math.Sin(Timer / MaxTime * MathHelper.Pi));

            //Make the owner look like theyre holding the sword bla bla
            Owner.heldProj = Projectile.whoAmI;
            Owner.ChangeDir(Math.Sign(direction.X));
            Owner.itemRotation = direction.ToRotation();
            if (Owner.direction != 1)
            {
                Owner.itemRotation -= 3.14f;
            }
            Owner.itemRotation = MathHelper.WrapAngle(Owner.itemRotation);
        }

        public void Lunge()
        {
            if (Main.myPlayer != Projectile.owner)
                return;
            Owner.velocity = direction.SafeNormalize(Vector2.UnitX * Owner.direction) * LungeSpeed;
        }

        public void PowerLunge()
        {
            Owner.Calamity().LungingDown = true;
            PowerLungeStart = Owner.Center;
            dashTimer = 1f;
            Owner.GiveIFrames(TrueBiomeBlade.EvilAttunement_SlashIFrames);
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone) => OnHitEffects(!target.canGhostHeal || Main.player[Projectile.owner].moonLeech);
        public override void OnHitPlayer(Player target, Player.HurtInfo info) => OnHitEffects(Main.player[Projectile.owner].moonLeech);

        private void OnHitEffects(bool cannotLifesteal)
        {
            if (ChargedUp)
                return;

            Projectile.netUpdate = true;
            Projectile.netSpam = 0;

            if (Main.myPlayer != Owner.whoAmI || CanBounce == 0f)
                return;

            if (!cannotLifesteal) //trolled
            {
                Owner.statLife += TrueBiomeBlade.EvilAttunement_Lifesteal;
                Owner.HealEffect(TrueBiomeBlade.EvilAttunement_Lifesteal); //Idk if its too much or what but at the same time its close range as fuck
            }

            // Bounce off
            float bounceStrength = Math.Max((LungeSpeed / 2f), Owner.velocity.Length());
            bounceStrength *= Owner.velocity.Y == 0 ? 0.2f : 1f; //Reduce the bounce if the player is on the ground
            Owner.velocity = -direction.SafeNormalize(Vector2.Zero) * MathHelper.Clamp(bounceStrength, 0f, 22f);
            CanBounce = 0f;
            Owner.GiveIFrames(TrueBiomeBlade.EvilAttunement_BounceIFrames); // i frames for free!
            if (Owner.whoAmI == Main.myPlayer)
            {
                if (Owner.HeldItem.ModItem is TrueBiomeBlade sword)
                {
                    sword.PowerLungeCounter++;
                    if (sword.PowerLungeCounter == 3)
                        SoundEngine.PlaySound(SoundID.Item79); //indicate the charge with a sound effect
                }
            }
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D handle = Request<Texture2D>("CalamityMod/Projectiles/Melee/MendedBiomeBlade").Value;
            Texture2D tex = Request<Texture2D>("CalamityMod/Projectiles/Melee/MendedBiomeBlade_DecaysRetort").Value;

            float drawAngle = direction.ToRotation();
            float drawRotation = drawAngle + MathHelper.PiOver4;

            Vector2 displace = direction * ((float)Math.Sin(Timer / MaxTime * 3.14f) * 60);
            Vector2 drawOrigin = new Vector2(0f, handle.Height);
            Vector2 drawOffset = Owner.Center + direction * 10f - Main.screenPosition;

            Main.EntitySpriteDraw(handle, drawOffset + displace, null, lightColor, drawRotation, drawOrigin, Projectile.scale, 0f, 0);

            //Turn on additive blending
            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.Additive, Main.DefaultSamplerState, DepthStencilState.None, Main.Rasterizer, null, Main.GameViewMatrix.TransformationMatrix);
            //Update the parameters
            drawOrigin = new Vector2(0f, tex.Height);

            Main.EntitySpriteDraw(tex, drawOffset + displace, null, Color.Lerp(Color.White, lightColor, 0.5f) * 0.9f, drawRotation, drawOrigin, Projectile.scale, 0f, 0);

            if (dashTimer > 0f && dashTimer < maxDash)
            {
                float thrustRatio = (float)Math.Sin(dashTimer / maxDash * MathHelper.Pi);
                Main.EntitySpriteDraw(tex, drawOffset + displace, null, Color.Lerp(Color.White, lightColor, 0.5f) * 0.9f, drawRotation, drawOrigin, Projectile.scale * (1 + thrustRatio * 0.2f), 0f, 0);
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
            writer.Write(CanBounce);
            writer.Write(ChargedUp);
        }
        public override void ReceiveExtraAI(BinaryReader reader)
        {
            initialized = reader.ReadBoolean();
            direction = reader.ReadVector2();
            CanBounce = reader.ReadSingle();
            ChargedUp = reader.ReadBoolean();
        }
    }
}
