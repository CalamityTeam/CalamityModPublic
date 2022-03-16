using CalamityMod.Buffs.DamageOverTime;
using CalamityMod.Items.Weapons.Melee;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.IO;
using Terraria;
using Terraria.Graphics.Shaders;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Melee.Spears
{
    public class StreamGougeProj : ModProjectile
    {
        public int Time;

        public Player Owner => Main.player[projectile.owner];

        public float SpinCompletion => Utils.InverseLerp(0f, StreamGouge.SpinTime, Time, true);

        public ref float InitialDirection => ref projectile.ai[0];

        public ref float SpinDirection => ref projectile.ai[1];

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Stream Gouge");
        }

        public override void SetDefaults()
        {
            projectile.width = 40;
            projectile.height = 40;
            projectile.penetrate = -1;
            projectile.melee = true;
            projectile.friendly = true;
            projectile.tileCollide = false;
            projectile.ignoreWater = true;
            projectile.hide = true;
            projectile.timeLeft = 90000;
            projectile.MaxUpdates = 2;
            projectile.usesLocalNPCImmunity = true;
            projectile.localNPCHitCooldown = projectile.MaxUpdates * 13;
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
                Main.PlaySound(mod.GetLegacySoundSlot(SoundType.Custom, "Sounds/Custom/MeatySlash"), projectile.Center);

            // Define the initial direction.
            if (InitialDirection == 0f)
            {
                InitialDirection = projectile.velocity.ToRotation();
                SpinDirection = Main.rand.NextBool().ToDirectionInt();
                projectile.netUpdate = true;
            }
            else
            {
                float stabOffset = (float)Math.Sin(Time / 3f) * 15f;
                float spearReach = MathHelper.Lerp(-10f, stabOffset + 90f, Utils.InverseLerp(0f, StreamGouge.SpearFireTime, Time - StreamGouge.SpinTime, true));
                projectile.velocity = InitialDirection.ToRotationVector2() * spearReach;
            }

            // Define the current rotation.
            projectile.rotation = (float)Math.Pow(SpinCompletion, 0.82) * MathHelper.Pi * SpinDirection * 4f + InitialDirection - MathHelper.PiOver4 + MathHelper.Pi;

            // Manipulate player variables.
            DeterminePlayerVariables();

            // Create portals near the mouse once charged.
            if (Main.myPlayer == projectile.owner && Time >= StreamGouge.SpinTime + StreamGouge.SpearFireTime && Time % 9f == 8f)
            {
                Vector2 portalSpawnPosition = Main.MouseWorld + Main.rand.NextVector2Unit() * Main.rand.NextFloat(50f, 140f);
                Vector2 spearVelocity = (Main.MouseWorld - portalSpawnPosition).SafeNormalize(Vector2.UnitY);
                Projectile.NewProjectile(portalSpawnPosition, spearVelocity, ModContent.ProjectileType<StreamGougePortal>(), projectile.damage, projectile.knockBack, projectile.owner);
            }

            Time++;
        }

        public void DeterminePlayerVariables()
        {
            bool spearStillInUse = Owner.channel && !Owner.noItems && !Owner.CCed;
            Owner.direction = (Math.Cos(projectile.rotation - MathHelper.Pi + MathHelper.PiOver4) > 0f).ToDirectionInt();
            Owner.heldProj = projectile.whoAmI;
            Owner.itemTime = Owner.itemAnimation = 2;
            Owner.itemRotation = CalamityUtils.WrapAngle90Degrees(MathHelper.WrapAngle(projectile.rotation - MathHelper.Pi + MathHelper.PiOver4));
            projectile.Center = Owner.Center;

            // Die if the spear is no longer in use.
            if (!spearStillInUse)
                projectile.Kill();
        }

        public void DrawPortal(SpriteBatch spriteBatch, Vector2 drawPosition, float opacity)
        {
            Texture2D portalTexture = ModContent.GetTexture("CalamityMod/Projectiles/Melee/StreamGougePortal");
            Vector2 origin = portalTexture.Size() * 0.5f;
            Color baseColor = Color.White;
            float rotation = Main.GlobalTime * 6f;

            // Black portal.
            Color color = Color.Lerp(baseColor, Color.Black, 0.55f).MultiplyRGB(Color.DarkGray) * opacity;
            spriteBatch.Draw(portalTexture, drawPosition, null, color, rotation, origin, projectile.scale * 1.2f, SpriteEffects.None, 0f);
            spriteBatch.Draw(portalTexture, drawPosition, null, color, -rotation, origin, projectile.scale * 1.2f, SpriteEffects.None, 0f);

            spriteBatch.SetBlendState(BlendState.Additive);

            // Cyan portal.
            color = Color.Lerp(baseColor, Color.Cyan, 0.55f) * opacity * 1.6f;
            spriteBatch.Draw(portalTexture, drawPosition, null, color, rotation * 0.6f, origin, projectile.scale * 1.2f, SpriteEffects.None, 0f);

            // Magenta portal.
            color = Color.Lerp(baseColor, Color.Fuchsia, 0.55f) * opacity * 1.6f;
            spriteBatch.Draw(portalTexture, drawPosition, null, color, rotation * -0.6f, origin, projectile.scale * 1.2f, SpriteEffects.None, 0f);

            spriteBatch.SetBlendState(BlendState.AlphaBlend);
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
        {
            // Draw the spin smear texture.
            if (SpinCompletion >= 0f && SpinCompletion < 1f)
            {
                Texture2D smear = ModContent.GetTexture("CalamityMod/Particles/SemiCircularSmear");

                spriteBatch.EnterShaderRegion(BlendState.Additive);

                float rotation = projectile.rotation - MathHelper.Pi / 5f;
                if (SpinDirection == -1f)
                    rotation += MathHelper.Pi;

                Color smearColor = Color.Fuchsia * CalamityUtils.Convert01To010(SpinCompletion) * 0.9f;
                Vector2 smearOrigin = smear.Size() * 0.5f;

                spriteBatch.Draw(smear, Owner.Center - Main.screenPosition, null, smearColor, rotation, smearOrigin, projectile.scale * 1.45f, 0, 0);
                spriteBatch.ExitShaderRegion();
            }

            // Make the spear go through the portal.
            float portalOpacity = Utils.InverseLerp(0.6f, 1f, SpinCompletion, true);
            bool portalIsInteractable = portalOpacity >= 1f;
            Vector2 portalDrawPosition = Owner.Center + InitialDirection.ToRotationVector2() * 130f - Main.screenPosition;

            Texture2D texture = Main.projectileTexture[projectile.type];
            if (portalIsInteractable)
            {
                spriteBatch.EnterShaderRegion();
                Vector2 intersectionNormal = portalDrawPosition + Main.screenPosition - projectile.Center;
                Vector2 worldOffset = projectile.rotation.ToRotationVector2() * -45f;
                Vector2 intersectionOffset = (portalDrawPosition + Main.screenPosition - projectile.Center) * -1.5f;
                GameShaders.Misc["CalamityMod:IntersectionClip"].Shader.Parameters["uIntersectionPosition"].SetValue(portalDrawPosition + intersectionOffset);
                GameShaders.Misc["CalamityMod:IntersectionClip"].Shader.Parameters["uIntersectionNormal"].SetValue(intersectionNormal);
                GameShaders.Misc["CalamityMod:IntersectionClip"].Shader.Parameters["uIntersectionCutoffDirection"].SetValue(-1f);
                GameShaders.Misc["CalamityMod:IntersectionClip"].Shader.Parameters["uWorldPosition"].SetValue(projectile.Center - Main.screenPosition + worldOffset);
                GameShaders.Misc["CalamityMod:IntersectionClip"].Shader.Parameters["uSize"].SetValue(texture.Size());
                GameShaders.Misc["CalamityMod:IntersectionClip"].Shader.Parameters["uRotation"].SetValue(projectile.rotation);
                GameShaders.Misc["CalamityMod:IntersectionClip"].Apply();
            }

            Vector2 drawPosition = projectile.Center - Main.screenPosition;
            Vector2 origin = texture.Size() * 0.5f;
            spriteBatch.Draw(texture, drawPosition, null, projectile.GetAlpha(lightColor), projectile.rotation, origin, 1f, 0, 0);

            if (portalIsInteractable)
                spriteBatch.ExitShaderRegion();

            // Draw the portal once ready.
            DrawPortal(spriteBatch, portalDrawPosition, portalOpacity);

            return false;
        }

        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
            target.AddBuff(ModContent.BuffType<GodSlayerInferno>(), 300);
        }
    }
}
