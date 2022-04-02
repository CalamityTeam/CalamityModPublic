using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Magic
{
    public class BlushieStaffProj : ModProjectile
    {
        public override string Texture => "CalamityMod/Projectiles/InvisibleProj";

        private const int xRange = 600;
        private const int yRange = 320;

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Staff of Blushie");
        }

        public override void SetDefaults()
        {
            projectile.width = 2;
            projectile.height = 2;
            projectile.friendly = true;
            projectile.tileCollide = false;
            projectile.ignoreWater = true;
            projectile.magic = true;
            projectile.Calamity().PierceResistHarshness = 0.06f;
            projectile.Calamity().PierceResistCap = 0.4f;
        }

        public override void AI()
        {
            Player player = Main.player[projectile.owner];
            if (Main.myPlayer == projectile.owner)
            {
                if (!player.channel || player.noItems || player.CCed || projectile.ai[0] > 3600f)
                {
                    projectile.Kill();
                }
            }
            projectile.Center = player.MountedCenter;
            projectile.timeLeft = 2;
            player.itemTime = 2;
            player.itemAnimation = 2;

            projectile.ai[0] += 1f;
            projectile.damage = (int)(projectile.ai[0] - 120f); // Max damage = 3480
            if (projectile.damage >= 100 && Main.myPlayer == projectile.owner)
            {
                if (player.statMana <= 0 && player.manaFlower)
                {
                    player.QuickMana();
                }
                if (player.statMana > 0)
                {
                    player.statMana--;
                }
                else
                {
                    projectile.Kill();
                }
            }

            if (projectile.localAI[1] == 0f)
            {
                projectile.localAI[1] = 37f;
                projectile.localAI[0] = Main.rand.Next(xRange);
            }
            projectile.localAI[0] = Next(projectile.localAI[0]);
        }

        private float Next(float seed)
        {
            return (seed * projectile.localAI[1] + 101f) % (4 * xRange * yRange);
        }

        public override bool CanDamage()
        {
            return projectile.ai[0] > 120f;
        }

        public override void ModifyDamageHitbox(ref Rectangle hitbox)
        {
            hitbox.X -= xRange;
            hitbox.Width += 2 * xRange;
            hitbox.Y -= yRange;
            hitbox.Height += 2 * yRange;
        }

        public override void ModifyHitNPC(NPC target, ref int damage, ref float knockback, ref bool crit, ref int hitDirection)
        {
            damage += target.defense * 2;
        }

        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
            projectile.penetrate++;
            target.AddBuff(BuffID.Daybreak, 300);
            target.AddBuff(BuffID.OnFire, 300);
            target.AddBuff(BuffID.Frostburn, 300);
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
        {
            spriteBatch.End();
            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.NonPremultiplied, Main.DefaultSamplerState, DepthStencilState.None, Main.instance.Rasterizer, null, Main.GameViewMatrix.TransformationMatrix);

            Vector2 center = projectile.Center - Main.screenPosition;
            Vector2 aura = center + new Vector2(-xRange, yRange);
            Texture2D texture = ModContent.GetTexture("CalamityMod/ExtraTextures/BlushieStaffAura");
            Rectangle frame = new Rectangle(50, 0, 50, 32);
            int count = 2 * xRange / 50;
            SpriteEffects effects = projectile.ai[0] % 30f < 15f ? SpriteEffects.None : SpriteEffects.FlipHorizontally;
            for (int k = 0; k < count; k++)
            {
                if (k == 0)
                {
                    frame.X = effects == SpriteEffects.None ? 0 : 100;
                }
                else if (k == count - 1)
                {
                    frame.X = effects == SpriteEffects.None ? 100 : 0;
                }
                else
                {
                    frame.X = 50;
                }
                spriteBatch.Draw(texture, aura, frame, Color.White, 0f, new Vector2(0f, 32f), 1f, effects, 0f);
                aura.X += 50f;
            }

            Vector2 topLeftGear = center + new Vector2(-400f, -100f);
            Vector2 topRightGear = center + new Vector2(200f, -200f);
            Vector2 bottomLeftGear = center + new Vector2(-xRange / 2, yRange / 2);
            Vector2 bottomRightGear = center + new Vector2(xRange - 100, yRange - 100);

            float alpha = projectile.ai[0] / 60f;
            if (alpha > 0f)
            {
                if (alpha > 1f)
                {
                    alpha = 1f;
                }
                DrawChains(spriteBatch, topLeftGear, center, alpha);
                DrawChains(spriteBatch, center, bottomLeftGear, alpha);
                DrawChains(spriteBatch, bottomLeftGear, topLeftGear, alpha);
                DrawChains(spriteBatch, center, bottomRightGear, alpha);
                DrawChains(spriteBatch, bottomRightGear, topRightGear, alpha);
                DrawChains(spriteBatch, topRightGear, center, alpha);
                DrawChains(spriteBatch, new Vector2(bottomLeftGear.X, center.Y + yRange), bottomLeftGear, alpha);
                DrawChains(spriteBatch, bottomRightGear, new Vector2(bottomRightGear.X, center.Y + yRange), alpha);
            }

            float scale = 1f;
            if (projectile.ai[0] < 60f)
            {
                scale = 4f - 3f * projectile.ai[0] / 60f;
            }
            texture = ModContent.GetTexture("CalamityMod/ExtraTextures/BlushieStaffGear");
            Vector2 origin = new Vector2(48f, 48f);
            spriteBatch.Draw(texture, center, null, Color.White, projectile.ai[0] / 20f, origin, 1.5f * scale, SpriteEffects.None, 0f);
            spriteBatch.Draw(texture, topLeftGear, null, Color.White, projectile.ai[0] / 10f, origin, scale, SpriteEffects.None, 0f);
            spriteBatch.Draw(texture, topRightGear, null, Color.White, -projectile.ai[0] / 8f, origin, 0.75f * scale, SpriteEffects.None, 0f);
            spriteBatch.Draw(texture, bottomLeftGear, null, Color.White, -projectile.ai[0] / 15f, origin, 1.4f * scale, SpriteEffects.None, 0f);
            spriteBatch.Draw(texture, bottomRightGear, null, Color.White, projectile.ai[0] / 10f, origin, scale, SpriteEffects.None, 0f);

            float seed = projectile.localAI[0];
            texture = ModContent.GetTexture("CalamityMod/ExtraTextures/BlushieStaffFire");
            Vector2 topLeft = center + new Vector2(-xRange, -yRange);
            for (int k = (int)projectile.ai[0] - 60; k < (int)projectile.ai[0]; k++)
            {
                if (k > 120)
                {
                    spriteBatch.Draw(texture, topLeft + new Vector2(seed % (2 * xRange), seed % (2 * yRange) + k - projectile.ai[0]), Color.White);
                }
                seed = Next(seed);
            }

            spriteBatch.End();
            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, Main.instance.Rasterizer, null, Main.GameViewMatrix.TransformationMatrix);
            return false;
        }

        private void DrawChains(SpriteBatch spriteBatch, Vector2 start, Vector2 end, float alpha)
        {
            Texture2D texture = ModContent.GetTexture("CalamityMod/ExtraTextures/BlushieStaffChain");
            Vector2 unit = end - start;
            float distance = unit.Length() - 36f;
            unit.Normalize();
            float rotation = unit.ToRotation();
            start += unit * 18f;
            end -= unit * 18f;
            float offset = projectile.ai[0] * 2f % texture.Width;
            start += unit * offset;
            distance -= offset;
            int count = (int)(distance / texture.Width);
            Color color = Color.White * alpha;
            Vector2 origin = new Vector2(texture.Width / 2, texture.Height / 2);
            for (int k = 0; k <= count; k++)
            {
                spriteBatch.Draw(texture, start, null, color, rotation, origin, 1f, SpriteEffects.None, 0f);
                start += unit * texture.Width;
            }
        }
    }
}
