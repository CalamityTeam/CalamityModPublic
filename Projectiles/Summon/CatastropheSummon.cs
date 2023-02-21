using CalamityMod.Buffs.DamageOverTime;
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
        public ref float Time => ref Projectile.ai[0];
        public bool LookingAtPlayer => Time < 45f;
        public override string Texture => "CalamityMod/NPCs/SupremeCalamitas/SupremeCatastrophe";
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Catastrophe");
            Main.projFrames[Projectile.type] = 6;
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 4;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 2;
            ProjectileID.Sets.MinionSacrificable[Projectile.type] = false;
            ProjectileID.Sets.MinionTargettingFeature[Projectile.type] = false;
        }

        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = 120;
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
            Projectile.DamageType = DamageClass.Summon;
        }

        public override void AI()
        {
            if (LookingAtPlayer)
                Projectile.frame = (int)Math.Round(MathHelper.Lerp(0f, 6f, Time / 45f));
            else
            {
                float slashInterpolant = ((Time - 45f) / 27f) % 1f;
                Projectile.frame = (int)Math.Round(MathHelper.Lerp(6f, 15f, slashInterpolant));
            }
            CataclysmSummon.Behavior(Projectile, Main.player[Projectile.owner], ref Time);
        }

        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit) => target.AddBuff(ModContent.BuffType<VulnerabilityHex>(), 180);

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D texture = ModContent.Request<Texture2D>(Texture).Value;
            Rectangle frame = texture.Frame(2, 8, Projectile.frame / 8, Projectile.frame % 8);
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

        // TODO -- this damage should be after Terraria vanilla multipliers, so it won't one shot people
        public override void ModifyHitPlayer(Player target, ref int damage, ref bool crit)
        {
            if (Main.masterMode) damage = 320;
            else if (Main.expertMode) damage = 260;
            else damage = 200;
        }

        public override bool? CanDamage() => Projectile.Opacity >= 1f;
    }
}
