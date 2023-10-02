using CalamityMod.Buffs.DamageOverTime;
using CalamityMod.Items.Weapons.Melee;
using CalamityMod.Sounds;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.IO;
using Terraria;
using Terraria.Audio;
using Terraria.Graphics.Shaders;
using Terraria.Localization;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Melee.Spears
{
    public class StreamGougeProj : ModProjectile
    {
        public override LocalizedText DisplayName => CalamityUtils.GetItemName<StreamGouge>();
        public int Time;

        public Player Owner => Main.player[Projectile.owner];

        public float SpinCompletion => Utils.GetLerpValue(0f, StreamGouge.SpinTime, Time, true);

        public ref float InitialDirection => ref Projectile.ai[0];

        public ref float SpinDirection => ref Projectile.ai[1];

        public override void SetDefaults()
        {
            Projectile.width = 40;
            Projectile.height = 40;
            Projectile.penetrate = -1;
            Projectile.DamageType = DamageClass.Melee;
            Projectile.friendly = true;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
            Projectile.hide = true;
            Projectile.timeLeft = 90000;
            Projectile.MaxUpdates = 2;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = Projectile.MaxUpdates * 13;
        }

        public override void SendExtraAI(BinaryWriter writer)
        {
            writer.Write(Time);
        }

        public override void ReceiveExtraAI(BinaryReader reader)
        {
            Time = reader.ReadInt32();
        }

        public override void AI()
        {
            // Play a strong slash sound on the first frame to accompany the spin.
            if (Time == 0f)
                SoundEngine.PlaySound(CommonCalamitySounds.MeatySlashSound, Projectile.Center);

            // Define the initial direction.
            if (InitialDirection == 0f)
            {
                InitialDirection = Projectile.velocity.ToRotation();
                SpinDirection = Main.rand.NextBool().ToDirectionInt();
                Projectile.netUpdate = true;
            }
            else
            {
                float stabOffset = (float)Math.Sin(Time / 3f) * 15f;
                float spearReach = MathHelper.Lerp(-10f, stabOffset + 90f, Utils.GetLerpValue(0f, StreamGouge.SpearFireTime, Time - StreamGouge.SpinTime, true));
                Projectile.velocity = InitialDirection.ToRotationVector2() * spearReach;
            }

            // Define the current rotation.
            Projectile.rotation = (float)Math.Pow(SpinCompletion, 0.82) * MathHelper.Pi * SpinDirection * 4f + InitialDirection - MathHelper.PiOver4 + MathHelper.Pi;

            // Manipulate player variables.
            DeterminePlayerVariables();

            // Create portals near the mouse once charged.
            if (Main.myPlayer == Projectile.owner && Time >= StreamGouge.SpinTime + StreamGouge.SpearFireTime && Time % 9f == 8f)
            {
                Vector2 portalSpawnPosition = Main.MouseWorld + Main.rand.NextVector2Unit() * Main.rand.NextFloat(50f, 140f);
                Vector2 spearVelocity = (Main.MouseWorld - portalSpawnPosition).SafeNormalize(Vector2.UnitY);
                Projectile.NewProjectile(Projectile.GetSource_FromThis(), portalSpawnPosition, spearVelocity, ModContent.ProjectileType<StreamGougePortal>(), Projectile.damage, Projectile.knockBack, Projectile.owner);
            }

            Time++;
        }

        public void DeterminePlayerVariables()
        {
            bool spearStillInUse = Owner.channel && !Owner.noItems && !Owner.CCed;
            Owner.direction = (Math.Cos(Projectile.rotation - MathHelper.Pi + MathHelper.PiOver4) > 0f).ToDirectionInt();
            Owner.heldProj = Projectile.whoAmI;
            Owner.itemTime = Owner.itemAnimation = 2;
            Owner.itemRotation = CalamityUtils.WrapAngle90Degrees(MathHelper.WrapAngle(Projectile.rotation - MathHelper.Pi + MathHelper.PiOver4));
            Projectile.Center = Owner.Center;

            // Die if the spear is no longer in use.
            if (!spearStillInUse)
                Projectile.Kill();
        }

        public void DrawPortal(Vector2 drawPosition, float opacity)
        {
            Texture2D portalTexture = ModContent.Request<Texture2D>("CalamityMod/Projectiles/Melee/StreamGougePortal").Value;
            Vector2 origin = portalTexture.Size() * 0.5f;
            Color baseColor = Color.White;
            float rotation = Main.GlobalTimeWrappedHourly * 6f;

            // Black portal.
            Color color = Color.Lerp(baseColor, Color.Black, 0.55f).MultiplyRGB(Color.DarkGray) * opacity;
            Main.EntitySpriteDraw(portalTexture, drawPosition, null, color, rotation, origin, Projectile.scale * 1.2f, SpriteEffects.None, 0);
            Main.EntitySpriteDraw(portalTexture, drawPosition, null, color, -rotation, origin, Projectile.scale * 1.2f, SpriteEffects.None, 0);

            Main.spriteBatch.SetBlendState(BlendState.Additive);

            // Cyan portal.
            color = Color.Lerp(baseColor, Color.Cyan, 0.55f) * opacity * 1.6f;
            Main.EntitySpriteDraw(portalTexture, drawPosition, null, color, rotation * 0.6f, origin, Projectile.scale * 1.2f, SpriteEffects.None, 0);

            // Magenta portal.
            color = Color.Lerp(baseColor, Color.Fuchsia, 0.55f) * opacity * 1.6f;
            Main.EntitySpriteDraw(portalTexture, drawPosition, null, color, rotation * -0.6f, origin, Projectile.scale * 1.2f, SpriteEffects.None, 0);

            Main.spriteBatch.SetBlendState(BlendState.AlphaBlend);
        }

        public override bool PreDraw(ref Color lightColor)
        {
            // Draw the spin smear texture.
            if (SpinCompletion >= 0f && SpinCompletion < 1f)
            {
                Texture2D smear = ModContent.Request<Texture2D>("CalamityMod/Particles/SemiCircularSmear").Value;

                Main.spriteBatch.EnterShaderRegion(BlendState.Additive);

                float rotation = Projectile.rotation - MathHelper.Pi / 5f;
                if (SpinDirection == -1f)
                    rotation += MathHelper.Pi;

                Color smearColor = Color.Fuchsia * CalamityUtils.Convert01To010(SpinCompletion) * 0.9f;
                Vector2 smearOrigin = smear.Size() * 0.5f;

                Main.EntitySpriteDraw(smear, Owner.Center - Main.screenPosition, null, smearColor, rotation, smearOrigin, Projectile.scale * 1.45f, 0, 0);
                Main.spriteBatch.ExitShaderRegion();
            }

            // Make the spear go through the portal.
            float portalOpacity = Utils.GetLerpValue(0.6f, 1f, SpinCompletion, true);
            bool portalIsInteractable = portalOpacity >= 1f;
            Vector2 portalDrawPosition = Owner.Center + InitialDirection.ToRotationVector2() * 130f - Main.screenPosition;

            Texture2D texture = ModContent.Request<Texture2D>(Texture).Value;
            if (portalIsInteractable)
            {
                Main.spriteBatch.EnterShaderRegion();
                Vector2 intersectionNormal = portalDrawPosition + Main.screenPosition - Projectile.Center;
                Vector2 worldOffset = Projectile.rotation.ToRotationVector2() * Utils.GetLerpValue(0f, StreamGouge.SpearFireTime, Time - StreamGouge.SpinTime, true) * 80f;
                Vector2 intersectionOffset = (portalDrawPosition + Main.screenPosition - Projectile.Center) * -1.5f;
                GameShaders.Misc["CalamityMod:IntersectionClip"].Shader.Parameters["uIntersectionPosition"].SetValue(portalDrawPosition + intersectionOffset);
                GameShaders.Misc["CalamityMod:IntersectionClip"].Shader.Parameters["uIntersectionNormal"].SetValue(intersectionNormal);
                GameShaders.Misc["CalamityMod:IntersectionClip"].Shader.Parameters["uIntersectionCutoffDirection"].SetValue(1f);
                GameShaders.Misc["CalamityMod:IntersectionClip"].Shader.Parameters["uWorldPosition"].SetValue(Projectile.Center - Main.screenPosition + worldOffset);
                GameShaders.Misc["CalamityMod:IntersectionClip"].Shader.Parameters["uSize"].SetValue(texture.Size());
                GameShaders.Misc["CalamityMod:IntersectionClip"].Shader.Parameters["uRotation"].SetValue(Projectile.rotation);
                GameShaders.Misc["CalamityMod:IntersectionClip"].Apply();
            }

            Vector2 drawPosition = Projectile.Center - Main.screenPosition;
            Vector2 origin = texture.Size() * 0.5f;
            Main.EntitySpriteDraw(texture, drawPosition, null, Projectile.GetAlpha(lightColor), Projectile.rotation, origin, 1f, 0, 0);

            if (portalIsInteractable)
                Main.spriteBatch.ExitShaderRegion();

            // Draw the portal once ready.
            DrawPortal(portalDrawPosition, portalOpacity);

            return false;
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            target.AddBuff(ModContent.BuffType<GodSlayerInferno>(), 300);
        }
    }
}
