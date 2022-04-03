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
            Projectile.width = 140;
            Projectile.height = 290;
            Projectile.hostile = true;
            Projectile.penetrate = -1;
            Projectile.timeLeft = 300;
            Projectile.tileCollide = false;
            Projectile.hide = true;
        }

        public override void AI()
        {
            Projectile.frameCounter++;
            if (Projectile.frameCounter % 6 == 0)
            {
                FrameY += 1;
                if (FrameY >= 7)
                {
                    FrameX += 1;
                    FrameY = 0;
                }
                if (FrameX >= 2)
                    Projectile.Kill();
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
            Rectangle frame = new Rectangle(FrameX * Projectile.width, FrameY * Projectile.height, Projectile.width, Projectile.height);
            spriteBatch.Draw(ModContent.Request<Texture2D>("CalamityMod/Projectiles/Rogue/SulphuricNukesplosion"), Projectile.Center - Main.screenPosition, frame, Color.White, Projectile.rotation, Projectile.Size / 2f, 1f, SpriteEffects.None, 0f);
            return false;
        }
    }
}
