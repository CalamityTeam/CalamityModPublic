using CalamityMod.Buffs.StatDebuffs;
using CalamityMod.DataStructures;
using CalamityMod.Particles;
using CalamityMod.Items.Weapons.Melee;
using Terraria.Graphics.Shaders;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using static Terraria.ModLoader.ModContent;
using static CalamityMod.CalamityUtils;
using Terraria.Audio;

namespace CalamityMod.Projectiles.Melee
{
    public class TrueDecaysRetort : ModProjectile
    {
        public override string Texture => "CalamityMod/Projectiles/Melee/MendedBiomeBlade_DecaysRetort";
        private bool initialized = false;
        public Vector2 direction = Vector2.Zero;
        public ref float MaxTime => ref projectile.ai[0];
        public ref float CanLunge => ref projectile.ai[1];
        public float Timer => MaxTime - projectile.timeLeft;
        public bool ChargedUp;
        public Player Owner => Main.player[projectile.owner];
        public const float LungeSpeed = 16;
        public ref float CanBounce => ref projectile.localAI[0];
        public ref float dashTimer => ref projectile.localAI[1];
        public const float maxDash = 20f;

        private Vector2 PowerLungeStart;

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Decay's Retort");
        }
        public override void SetDefaults()
        {
            projectile.melee = true;
            projectile.width = projectile.height = 84;
            projectile.width = projectile.height = 84;
            projectile.tileCollide = false;
            projectile.friendly = true;
            projectile.penetrate = -1;
            projectile.extraUpdates = 1;
        }

        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            float collisionPoint = 0f;
            float bladeLenght = 140f * projectile.scale;
            Vector2 displace = direction * ((float)Math.Sin(Timer / MaxTime * 3.14f) * 60);

            return Collision.CheckAABBvLineCollision(targetHitbox.TopLeft(), targetHitbox.Size(), Owner.Center + displace, Owner.Center + displace + (direction * bladeLenght), 24, ref collisionPoint);
        }

        public override void AI()
        {
            if (!initialized) //Initialization
            {
                CanBounce = 1f;
                projectile.timeLeft = (int)MaxTime;
                direction = Owner.SafeDirectionTo(Owner.Calamity().mouseWorld, Vector2.Zero);
                direction.Normalize();
                projectile.rotation = direction.ToRotation();
                if (CanLunge == 1f && !ChargedUp)
                    Lunge();
                Main.PlaySound(SoundID.Item103, projectile.Center);
                initialized = true;
                projectile.netUpdate = true;
                projectile.netSpam = 0;
            }

            if (ChargedUp && dashTimer == 0f)
                PowerLunge();

            if (dashTimer >= 1f)
            {
                if (dashTimer < maxDash)
                {
                    Owner.fallStart = (int)(Owner.position.Y / 16f);
                    Owner.velocity = direction * 60f;
                    projectile.timeLeft = (int)(MaxTime / 2f);
                    dashTimer++;
                }

                if (dashTimer == maxDash)
                {
                    Owner.velocity *= 0.1f; //Abrupt stop
                    Owner.Calamity().LungingDown = false;

                    Main.PlaySound(mod.GetLegacySoundSlot(Terraria.ModLoader.SoundType.Custom, "Sounds/Custom/SwiftSlice"), Owner.Center);

                    if (Owner.whoAmI == Main.myPlayer)
                    {
                        Projectile proj = Projectile.NewProjectileDirect(Owner.Center - PowerLungeStart / 2f, Vector2.Zero, ProjectileType<DecaysRetortDash>(), (int)(projectile.damage * TrueBiomeBlade.EvilAttunement_SlashDamageBoost), 0, Owner.whoAmI);
                        if (proj.modProjectile is DecaysRetortDash dash)
                        {
                            dash.DashStart = PowerLungeStart;
                            dash.DashEnd = Owner.Center;
                        }
                    }
                    dashTimer = maxDash + 1;
                }
            }

            //Manage position and rotation
            projectile.scale = 1f + ((float)Math.Sin(Timer / MaxTime * MathHelper.Pi) * 0.6f); //SWAGGER
            projectile.Center = Owner.Center + (direction * ((float)Math.Sin(Timer / MaxTime * MathHelper.Pi) * 60));

            Lighting.AddLight(projectile.Center, new Vector3(0.9f, 0f, 0.35f) * (float)Math.Sin(Timer / MaxTime * MathHelper.Pi));

            //Make the owner look like theyre holding the sword bla bla
            Owner.heldProj = projectile.whoAmI;
            Owner.direction = Math.Sign(direction.X);
            Owner.itemRotation = direction.ToRotation();
            if (Owner.direction != 1)
            {
                Owner.itemRotation -= 3.14f;
            }
            Owner.itemRotation = MathHelper.WrapAngle(Owner.itemRotation);
        }

        public void Lunge()
        {
            if (Main.myPlayer != projectile.owner)
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

        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit) => OnHitEffects(!target.canGhostHeal || Main.player[projectile.owner].moonLeech);
        public override void OnHitPvp(Player target, int damage, bool crit) => OnHitEffects(Main.player[projectile.owner].moonLeech);

        private void OnHitEffects(bool cannotLifesteal)
        {
            if (ChargedUp)
                return;

            projectile.netUpdate = true;
            projectile.netSpam = 0;

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
                if (Owner.HeldItem.modItem is TrueBiomeBlade sword)
                {
                    sword.PowerLungeCounter++;
                    if (sword.PowerLungeCounter == 3)
                        Main.PlaySound(SoundID.Item79); //indicate the charge with a sound effect
                }
            }
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
        {
            Texture2D handle = GetTexture("CalamityMod/Projectiles/Melee/MendedBiomeBlade");
            Texture2D tex = GetTexture("CalamityMod/Projectiles/Melee/MendedBiomeBlade_DecaysRetort");

            float drawAngle = direction.ToRotation();
            float drawRotation = drawAngle + MathHelper.PiOver4;

            Vector2 displace = direction * ((float)Math.Sin(Timer / MaxTime * 3.14f) * 60);
            Vector2 drawOrigin = new Vector2(0f, handle.Height);
            Vector2 drawOffset = Owner.Center + direction * 10f - Main.screenPosition;

            spriteBatch.Draw(handle, drawOffset + displace, null, lightColor, drawRotation, drawOrigin, projectile.scale, 0f, 0f);

            //Turn on additive blending
            spriteBatch.End();
            spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.Additive, Main.DefaultSamplerState, DepthStencilState.None, Main.instance.Rasterizer, null, Main.GameViewMatrix.TransformationMatrix);
            //Update the parameters
            drawOrigin = new Vector2(0f, tex.Height);

            spriteBatch.Draw(tex, drawOffset + displace, null, Color.Lerp(Color.White, lightColor, 0.5f) * 0.9f, drawRotation, drawOrigin, projectile.scale, 0f, 0f);

            if (dashTimer > 0f && dashTimer < maxDash)
            {
                float thrustRatio = (float)Math.Sin(dashTimer / maxDash * MathHelper.Pi);
                spriteBatch.Draw(tex, drawOffset + displace, null, Color.Lerp(Color.White, lightColor, 0.5f) * 0.9f, drawRotation, drawOrigin, projectile.scale * (1 + thrustRatio * 0.2f), 0f, 0f);
            }

            //Back to normal
            spriteBatch.End();
            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, Main.instance.Rasterizer, null, Main.GameViewMatrix.TransformationMatrix);

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