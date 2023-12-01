using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Audio;
using CalamityMod.Buffs.DamageOverTime;

namespace CalamityMod.Projectiles.Melee
{
    public class DarkBall : ModProjectile, ILocalizedModType
    {
        public new string LocalizationCategory => "Projectiles.Melee";
        public override void SetDefaults()
        {
            Projectile.width = 20;
            Projectile.height = 20;
            Projectile.scale = 0.9f;
            Projectile.friendly = true;
            Projectile.penetrate = -1;
            Projectile.timeLeft = 200;
            Projectile.DamageType = DamageClass.Melee;
            Projectile.tileCollide = false;
        }

        public override void AI()
        {
            try
            {
                int tileXMin = (int)(Projectile.position.X / 16f) - 1;
                int tileXMax = (int)((Projectile.position.X + (float)Projectile.width) / 16f) + 2;
                int tileYMin = (int)(Projectile.position.Y / 16f) - 1;
                int tileYMax = (int)((Projectile.position.Y + (float)Projectile.height) / 16f) + 2;
                if (tileXMin < 0)
                {
                    tileXMin = 0;
                }
                if (tileXMax > Main.maxTilesX)
                {
                    tileXMax = Main.maxTilesX;
                }
                if (tileYMin < 0)
                {
                    tileYMin = 0;
                }
                if (tileYMax > Main.maxTilesY)
                {
                    tileYMax = Main.maxTilesY;
                }
                for (int i = tileXMin; i < tileXMax; i++)
                {
                    for (int j = tileYMin; j < tileYMax; j++)
                    {
                        if (Main.tile[i, j] != null && Main.tile[i, j].HasUnactuatedTile && (Main.tileSolid[(int)Main.tile[i, j].TileType] || (Main.tileSolidTop[(int)Main.tile[i, j].TileType] && Main.tile[i, j].TileFrameY == 0)))
                        {
                            Vector2 projPos;
                            projPos.X = (float)(i * 16);
                            projPos.Y = (float)(j * 16);
                            if (Projectile.position.X + (float)Projectile.width - 4f > projPos.X && Projectile.position.X + 4f < projPos.X + 16f && Projectile.position.Y + (float)Projectile.height - 4f > projPos.Y && Projectile.position.Y + 4f < projPos.Y + 16f)
                            {
                                Projectile.velocity.X = 0f;
                                Projectile.velocity.Y = -0.2f;
                            }
                        }
                    }
                }
            } catch
            {
            }
            if (Projectile.owner == Main.myPlayer && Projectile.timeLeft <= 3)
            {
                Projectile.tileCollide = false;
                Projectile.ai[1] = 0f;
                Projectile.alpha = 255;
                Projectile.position.X = Projectile.position.X + (float)(Projectile.width / 2);
                Projectile.position.Y = Projectile.position.Y + (float)(Projectile.height / 2);
                Projectile.width = 128;
                Projectile.height = 128;
                Projectile.position.X = Projectile.position.X - (float)(Projectile.width / 2);
                Projectile.position.Y = Projectile.position.Y - (float)(Projectile.height / 2);
                Projectile.knockBack = 8f;
            }
            Projectile.ai[0] += 1f;
            if (Projectile.ai[0] > 10f)
            {
                Projectile.ai[0] = 10f;
                if (Projectile.velocity.Y == 0f && Projectile.velocity.X != 0f)
                {
                    Projectile.velocity.X = Projectile.velocity.X * 0.97f;
                    if ((double)Projectile.velocity.X > -0.01 && (double)Projectile.velocity.X < 0.01)
                    {
                        Projectile.velocity.X = 0f;
                        Projectile.netUpdate = true;
                    }
                }
                Projectile.velocity.Y = Projectile.velocity.Y + 0.2f;
            }
            Projectile.rotation += Projectile.velocity.X * 0.1f;
        }

        public override void OnKill(int timeLeft)
        {
            SoundEngine.PlaySound(SoundID.NPCDeath21, Projectile.position);
            Projectile.position.X = Projectile.position.X + (float)(Projectile.width / 2);
            Projectile.position.Y = Projectile.position.Y + (float)(Projectile.height / 2);
            Projectile.width = 22;
            Projectile.height = 22;
            Projectile.position.X = Projectile.position.X - (float)(Projectile.width / 2);
            Projectile.position.Y = Projectile.position.Y - (float)(Projectile.height / 2);
            for (int i = 0; i < 20; i++)
            {
                int corruptDust = Dust.NewDust(new Vector2(Projectile.position.X, Projectile.position.Y), Projectile.width, Projectile.height, 14, 0f, 0f, 100, default, 1.5f);
                Main.dust[corruptDust].velocity *= 1.4f;
            }
            for (int j = 0; j < 10; j++)
            {
                int corruptDust2 = Dust.NewDust(new Vector2(Projectile.position.X, Projectile.position.Y), Projectile.width, Projectile.height, 14, 0f, 0f, 100, default, 2.5f);
                Main.dust[corruptDust2].noGravity = true;
                Main.dust[corruptDust2].velocity *= 5f;
                corruptDust2 = Dust.NewDust(new Vector2(Projectile.position.X, Projectile.position.Y), Projectile.width, Projectile.height, 14, 0f, 0f, 100, default, 1.5f);
                Main.dust[corruptDust2].velocity *= 3f;
            }
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            target.AddBuff(ModContent.BuffType<BrainRot>(), 120);
            Projectile.Kill();
        }
    }
}
