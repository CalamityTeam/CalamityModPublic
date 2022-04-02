using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Summon
{
    public class CatastropheSummon : ModProjectile
    {
        public ref float Time => ref projectile.ai[0];
        public bool LookingAtPlayer => Time < 45f;
        public override string Texture => "CalamityMod/NPCs/SupremeCalamitas/SupremeCatastrophe";
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Catastrophe");
            Main.projFrames[projectile.type] = 6;
            ProjectileID.Sets.TrailCacheLength[projectile.type] = 4;
            ProjectileID.Sets.TrailingMode[projectile.type] = 2;
            ProjectileID.Sets.MinionSacrificable[projectile.type] = false;
            ProjectileID.Sets.MinionTargettingFeature[projectile.type] = false;
        }

        public override void SetDefaults()
        {
            projectile.width = projectile.height = 120;
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
                projectile.frame = (int)Math.Round(MathHelper.Lerp(0f, 6f, Time / 45f));
            else
            {
                float slashInterpolant = ((Time - 45f) / 27f) % 1f;
                projectile.frame = (int)Math.Round(MathHelper.Lerp(6f, 15f, slashInterpolant));
            }
            CataclysmSummon.Behavior(projectile, Main.player[projectile.owner], ref Time);
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
        {
            Texture2D texture = Main.projectileTexture[projectile.type];
            Rectangle frame = texture.Frame(2, 8, projectile.frame / 8, projectile.frame % 8);
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
