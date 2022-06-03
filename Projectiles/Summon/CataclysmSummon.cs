using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Audio;

namespace CalamityMod.Projectiles.Summon
{
    public class CataclysmSummon : ModProjectile
    {
        public ref float Time => ref Projectile.ai[0];
        public bool LookingAtPlayer => Time < 45f;
        public override string Texture => "CalamityMod/NPCs/SupremeCalamitas/SupremeCataclysm";
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Cataclysm");
            Main.projFrames[Projectile.type] = 6;
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 4;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 2;
            ProjectileID.Sets.MinionSacrificable[Projectile.type] = false;
            ProjectileID.Sets.MinionTargettingFeature[Projectile.type] = false;
        }

        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = 100;
            Projectile.netImportant = true;
            Projectile.friendly = true;
            Projectile.hostile = true;
            Projectile.ignoreWater = true;
            Projectile.minionSlots = 0f;
            Projectile.timeLeft = 90000;
            Projectile.penetrate = -1;
            Projectile.Opacity = 0f;
            Projectile.tileCollide = false;
            Projectile.minion = true;
        }

        public override void AI()
        {
            if (LookingAtPlayer)
                Projectile.frame = (int)Math.Round(MathHelper.Lerp(0f, 9f, Time / 45f));
            else
            {
                float punchInterpolant = ((Time - 45f) / 35f) % 1f;
                Projectile.frame = (int)Math.Round(MathHelper.Lerp(12f, 21f, punchInterpolant));
            }

            Behavior(Projectile, Main.player[Projectile.owner], ref Time);
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

                SoundEngine.PlaySound(SoundID.DD2_BetsyFlameBreath, owner.Center);
                SoundEngine.PlaySound(SoundID.DD2_WyvernDiveDown, owner.Center);
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

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D texture = ModContent.Request<Texture2D>(Texture).Value;
            Rectangle frame = texture.Frame(3, 9, Projectile.frame / 9, Projectile.frame % 9);
            Vector2 origin = frame.Size() * 0.5f;
            for (int i = 0; i < Projectile.oldPos.Length; i++)
            {
                float afterimageRot = Projectile.oldRot[i];
                SpriteEffects sfxForThisAfterimage = Projectile.oldSpriteDirection[i] == -1 ? SpriteEffects.FlipHorizontally : SpriteEffects.None;

                Vector2 drawPos = Projectile.oldPos[i] + Projectile.Size * 0.5f - Main.screenPosition + Vector2.UnitY * Projectile.gfxOffY;
                Color color = Projectile.GetAlpha(lightColor) * ((float)(Projectile.oldPos.Length - i) / Projectile.oldPos.Length);
                Main.EntitySpriteDraw(texture, drawPos, frame, color, afterimageRot, origin, Projectile.scale, sfxForThisAfterimage, 0);
            }

            SpriteEffects direction = Projectile.spriteDirection == -1 ? SpriteEffects.FlipHorizontally : SpriteEffects.None;
            Main.EntitySpriteDraw(texture, Projectile.Center - Main.screenPosition, frame, Projectile.GetAlpha(lightColor), Projectile.rotation, origin, Projectile.scale, direction, 0);
            return false;
        }

        public override void ModifyHitPlayer(Player target, ref int damage, ref bool crit) => damage = 70;

        public override bool? CanDamage() => Projectile.Opacity >= 1f;
    }
}
