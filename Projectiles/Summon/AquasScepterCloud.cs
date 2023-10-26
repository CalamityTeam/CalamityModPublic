using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.Graphics.Shaders;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Summon
{
    public class AquasScepterCloud : ModProjectile, ILocalizedModType
    {
        public new string LocalizationCategory => "Projectiles.Summon";
        Texture2D flashTex = ModContent.Request<Texture2D>("CalamityMod/Projectiles/Summon/AquasScepterCloudFlash").Value;
        Texture2D glowTex = ModContent.Request<Texture2D>("CalamityMod/Projectiles/Summon/AquasScepterCloudGlowmask").Value;
        public ref float LightningTimer => ref Projectile.ai[0];
        public ref float RainTimer => ref Projectile.ai[1];
        public int DrawFlashTimer = 0;

        public sealed override void SetDefaults()
        {
            Projectile.width = 238;
            Projectile.height = 98;
            Projectile.hide = true;
            Projectile.tileCollide = false;
            Projectile.timeLeft = Projectile.SentryLifeTime;
            Projectile.friendly = true;
            Projectile.sentry = true;
            Projectile.DamageType = DamageClass.Summon;
            Projectile.penetrate = -1;
        }

        public override void DrawBehind(int index, List<int> behindNPCsAndTiles, List<int> behindNPCs, List<int> behindProjectiles, List<int> overPlayers, List<int> overWiresUI)
        {
            behindProjectiles.Add(index);
            behindNPCs.Add(index);
        }

        public override bool? CanDamage() => false;
        public override Color? GetAlpha(Color lightColor) => Color.White;
        public override void AI()
        {
            float distanceFromTarget = 700f;
            var targetCenter = Projectile.position;
            bool foundTarget = false;

            LightningTimer++;
            RainTimer++;

            if (RainTimer >= 3f) // Spawns a raindrop every 3 frames, displaced down and randomly along the length of the cloud
            {
                Projectile.NewProjectile(Projectile.GetSource_FromThis(), (Projectile.Center.X + Main.rand.Next(-100, 101)), (Projectile.Bottom.Y + 24f), 0f, 15f, ModContent.ProjectileType<AquasScepterRaindrop>(), Projectile.damage, 0, Projectile.owner);

                RainTimer = 0f;
            }


            for (int i = 0; i < Main.maxNPCs; i++)
            {
                NPC npc = Main.npc[i];

                if (npc.CanBeChasedBy())
                {
                    float between = Vector2.Distance(npc.Center, Projectile.Center);
                    bool closest = Vector2.Distance(Projectile.Center, targetCenter) > between;
                    bool inRange = between < distanceFromTarget;

                    if ((closest && inRange) || !foundTarget)
                    {
                        distanceFromTarget = between;
                        targetCenter = npc.Center;
                        foundTarget = true;
                    }
                }
            }
            if (foundTarget)
            {
                if (distanceFromTarget < 300)
                {
                    if (LightningTimer >= 60f) //Every  60 AI cycles, plays the lightning sound and spawns 2 projectiles: the tesla aura for dealing damage in an aoe, and the cloud flash to simulate the brightness of the main cloud changing.
                    { 
                        SoundEngine.PlaySound(new SoundStyle("CalamityMod/Sounds/Item/LightningAura"), Projectile.Center);
                        Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center.X, Projectile.Bottom.Y, 0f, 0f, ModContent.ProjectileType<AquasScepterTeslaAura>(), (int)(Projectile.damage * 7.2f), 16, Projectile.owner);
                        LightningTimer = 0f;
                        DrawFlashTimer = 27;
                    }
                }
            }
        }
        public override void PostDraw(Color lightColor)
        {
            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.LinearClamp, DepthStencilState.Default, RasterizerState.CullNone, null, Main.GameViewMatrix.ZoomMatrix);

            MiscShaderData msd = GameShaders.Misc["CalamityMod:WavyOpacity"];
            msd.SetShaderTexture(ModContent.Request<Texture2D>("CalamityMod/ExtraTextures/GreyscaleGradients/BlobbyNoise"), 1);
            msd.UseOpacity(0.7f);
            DrawData dd = new()
            {
                texture = glowTex,
                position = Projectile.position - Main.screenPosition,
                sourceRect = glowTex.Bounds,
            };
            msd.Apply(dd);
            Vector2 glowPos = Projectile.position - Main.screenPosition + (glowTex.Size() * 0.5f);
            Main.EntitySpriteDraw(glowTex, glowPos, null, dd.color, 0f, glowTex.Size() * 0.5f, 1f, SpriteEffects.None);
            Main.spriteBatch.End();
            Main.spriteBatch.Begin();

            if (Main.rand.NextBool(240))
            {
                DrawFlashTimer += Main.rand.Next(15, 23);
                if (DrawFlashTimer > 27)
                    DrawFlashTimer = 27;
            }
                
            if (DrawFlashTimer > 0)
            { 
                float opacity = 1f - ((27 - DrawFlashTimer) / 27f);
                Vector2 drawPosition = Projectile.position - Main.screenPosition + (flashTex.Size() * 0.5f);
                Main.EntitySpriteDraw(flashTex, drawPosition, null, Color.White * opacity, 0f, flashTex.Size() * 0.5f, 1f, SpriteEffects.None);
                DrawFlashTimer--;
            }
        }
    }
}
