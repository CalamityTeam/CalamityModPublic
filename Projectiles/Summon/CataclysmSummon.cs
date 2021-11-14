using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Summon
{
    public class CataclysmSummon : ModProjectile
    {
        public ref float Time => ref projectile.ai[0];
        public bool LookingAtPlayer => Time < 45f;
        public override string Texture => "CalamityMod/NPCs/SupremeCalamitas/SupremeCataclysm";
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Cataclysm");
            Main.projFrames[projectile.type] = 6;
            ProjectileID.Sets.TrailCacheLength[projectile.type] = 4;
            ProjectileID.Sets.TrailingMode[projectile.type] = 2;
            ProjectileID.Sets.MinionSacrificable[projectile.type] = false;
            ProjectileID.Sets.MinionTargettingFeature[projectile.type] = false;
        }

        public override void SetDefaults()
        {
            projectile.width = projectile.height = 100;
            projectile.netImportant = true;
            projectile.friendly = true;
            projectile.hostile = true;
            projectile.ignoreWater = true;
            projectile.minionSlots = 0f;
            projectile.timeLeft = 90000;
            projectile.penetrate = -1;
            projectile.Opacity = 0f;
            projectile.tileCollide = false;
            projectile.minion = true;
        }

        public override void AI()
        {
            if (LookingAtPlayer)
                projectile.frame = (int)Math.Round(MathHelper.Lerp(0f, 9f, Time / 45f));
            else
            {
                float punchInterpolant = ((Time - 45f) / 35f) % 1f;
                projectile.frame = (int)Math.Round(MathHelper.Lerp(12f, 21f, punchInterpolant));
            }

            Behavior(projectile, Main.player[projectile.owner], ref Time);
        }

        public static void Behavior(Projectile projectile, Player owner, ref float time)
        {
            if (time < 40f)
                FadeIn(projectile);

            else if (time > 95f)
            {
                projectile.Opacity -= 0.06f;
                if (projectile.Opacity < 0f)
                    projectile.Kill();
            }

            if (time < 40f)
            {
                projectile.velocity = projectile.SafeDirectionTo(owner.Center) * -5f;
                projectile.rotation = projectile.velocity.X * 0.01f;
                projectile.spriteDirection = (owner.Center.X < projectile.Center.X).ToDirectionInt();
            }
            else if (time == 40f)
            {
                projectile.velocity = projectile.SafeDirectionTo(owner.Center) * 31f;
                projectile.rotation = projectile.velocity.X * 0.0125f;
                projectile.damage = (int)(projectile.damage * 1.45);

                Main.PlaySound(SoundID.DD2_BetsyFlameBreath, owner.Center);
                Main.PlaySound(SoundID.DD2_WyvernDiveDown, owner.Center);
            }

            time++;
        }

        public static void FadeIn(Projectile projectile)
        {
            if (projectile.localAI[0] == 0f)
            {
                for (int i = 0; i < 40; i++)
                {
                    Dust brimstone = Dust.NewDustPerfect(projectile.Center + Main.rand.NextVector2Square(-65f, 65f), 267);
                    brimstone.velocity = -Vector2.UnitY * Main.rand.NextFloat(1.8f, 3.6f);
                    brimstone.scale = Main.rand.NextFloat(1.65f, 1.85f);
                    brimstone.fadeIn = Main.rand.NextFloat(0.7f, 0.9f);
                    brimstone.color = Color.Red;
                    brimstone.noGravity = true;
                }
                projectile.localAI[0] = 1f;
            }

            projectile.Opacity = MathHelper.Clamp(projectile.Opacity + 0.075f, 0f, 1f);
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
        {
            Texture2D texture = Main.projectileTexture[projectile.type];
            Rectangle frame = texture.Frame(3, 9, projectile.frame / 9, projectile.frame % 9);
            Vector2 origin = frame.Size() * 0.5f;
            for (int i = 0; i < projectile.oldPos.Length; i++)
            {
                float afterimageRot = projectile.oldRot[i];
                SpriteEffects sfxForThisAfterimage = projectile.oldSpriteDirection[i] == -1 ? SpriteEffects.FlipHorizontally : SpriteEffects.None;

                Vector2 drawPos = projectile.oldPos[i] + projectile.Size * 0.5f - Main.screenPosition + Vector2.UnitY * projectile.gfxOffY;
                Color color = projectile.GetAlpha(lightColor) * ((float)(projectile.oldPos.Length - i) / projectile.oldPos.Length);
                spriteBatch.Draw(texture, drawPos, frame, color, afterimageRot, origin, projectile.scale, sfxForThisAfterimage, 0f);
            }

            SpriteEffects direction = projectile.spriteDirection == -1 ? SpriteEffects.FlipHorizontally : SpriteEffects.None;
            spriteBatch.Draw(texture, projectile.Center - Main.screenPosition, frame, projectile.GetAlpha(lightColor), projectile.rotation, origin, projectile.scale, direction, 0f);
            return false;
        }

        public override void ModifyHitPlayer(Player target, ref int damage, ref bool crit) => damage = 70;

        public override bool CanDamage() => projectile.Opacity >= 1f;
    }
}
