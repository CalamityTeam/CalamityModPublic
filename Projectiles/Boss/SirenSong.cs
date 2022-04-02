using CalamityMod.NPCs;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.IO;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Boss
{
    public class SirenSong : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Musical Note");
            ProjectileID.Sets.TrailCacheLength[projectile.type] = 4;
            ProjectileID.Sets.TrailingMode[projectile.type] = 0;
        }

        public override void SetDefaults()
        {
            projectile.width = 24;
            projectile.height = 58;
            projectile.hostile = true;
            projectile.tileCollide = false;
            projectile.penetrate = -1;
            projectile.timeLeft = 960;
            projectile.Opacity = 0f;
        }

        public override void SendExtraAI(BinaryWriter writer)
        {
            writer.Write(projectile.localAI[0]);
        }

        public override void ReceiveExtraAI(BinaryReader reader)
        {
            projectile.localAI[0] = reader.ReadSingle();
        }

        public override void AI()
        {
            projectile.velocity *= 0.985f;

            if (projectile.timeLeft < 30)
                projectile.Opacity = MathHelper.Clamp(projectile.timeLeft / 30f, 0f, 1f);
            else
                projectile.Opacity = MathHelper.Clamp(1f - ((projectile.timeLeft - 930) / 30f), 0f, 1f);

            if (projectile.localAI[0] == 0f)
            {
                projectile.scale += 0.01f;
                if (projectile.scale >= 1.1f)
                    projectile.localAI[0] = 1f;
            }
            else if (projectile.localAI[0] == 1f)
            {
                projectile.scale -= 0.01f;
                if (projectile.scale <= 0.9f)
                    projectile.localAI[0] = 0f;
            }

            Lighting.AddLight(projectile.Center, 0.7f * projectile.Opacity, 0.5f * projectile.Opacity, 0f);

            if (CalamityGlobalNPC.leviathan != -1)
            {
                if (Main.npc[CalamityGlobalNPC.leviathan].active)
                    projectile.extraUpdates = 1;
            }
        }

        public override bool CanHitPlayer(Player target) => projectile.Opacity == 1f;

        public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
        {
            lightColor.R = (byte)(255 * projectile.Opacity);
            lightColor.G = (byte)(255 * projectile.Opacity);
            lightColor.B = (byte)(255 * projectile.Opacity);
            CalamityUtils.DrawAfterimagesCentered(projectile, ProjectileID.Sets.TrailingMode[projectile.type], lightColor, 1);
            return false;
        }

        public override void OnHitPlayer(Player target, int damage, bool crit)
        {
            if (projectile.Opacity != 1f)
                return;

            target.AddBuff(BuffID.Confused, 120);
        }
    }
}
