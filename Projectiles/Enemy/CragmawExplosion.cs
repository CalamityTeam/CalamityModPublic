using CalamityMod.Buffs.StatDebuffs;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using Terraria;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Enemy
{
    public class CragmawExplosion : ModProjectile
    {
        public int FrameX = 0;
        public int FrameY = 0;
        public int CurrentFrame => FrameY + FrameX * 14;

        public override string Texture => "CalamityMod/Projectiles/Rogue/SulphuricNukesplosion";

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Nuclear Explosion");
        }

        public override void SetDefaults()
        {
            projectile.width = 140;
            projectile.height = 290;
            projectile.hostile = true;
            projectile.penetrate = -1;
            projectile.timeLeft = 300;
            projectile.tileCollide = false;
            projectile.hide = true;
        }

        public override void AI()
        {
            projectile.frameCounter++;
            if (projectile.frameCounter % 6 == 0)
            {
                FrameY += 1;
                if (FrameY >= 7)
                {
                    FrameX += 1;
                    FrameY = 0;
                }
                if (FrameX >= 2)
                    projectile.Kill();
            }
        }

        public override void OnHitPlayer(Player target, int damage, bool crit)
        {
            target.AddBuff(ModContent.BuffType<Irradiated>(), 300);
        }

        public override void DrawBehind(int index, List<int> drawCacheProjsBehindNPCsAndTiles, List<int> drawCacheProjsBehindNPCs, List<int> drawCacheProjsBehindProjectiles, List<int> drawCacheProjsOverWiresUI)
        {
            drawCacheProjsBehindNPCsAndTiles.Add(index);
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
        {
            Rectangle frame = new Rectangle(FrameX * projectile.width, FrameY * projectile.height, projectile.width, projectile.height);
            spriteBatch.Draw(ModContent.GetTexture("CalamityMod/Projectiles/Rogue/SulphuricNukesplosion"), projectile.Center - Main.screenPosition, frame, Color.White, projectile.rotation, projectile.Size / 2f, 1f, SpriteEffects.None, 0f);
            return false;
        }
    }
}
