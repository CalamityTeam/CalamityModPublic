using System;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Audio;

namespace CalamityMod.Projectiles.Melee
{
    public class GalaxyBlastType3 : ModProjectile, ILocalizedModType
    {
        public new string LocalizationCategory => "Projectiles.Melee";
        public override string Texture => "CalamityMod/Projectiles/InvisibleProj";

        public override void SetDefaults()
        {
            Projectile.width = 10;
            Projectile.height = 10;
            Projectile.friendly = true;
            Projectile.aiStyle = ProjAIStyleID.Arrow;
            Projectile.penetrate = 1;
            Projectile.extraUpdates = 5;
            Projectile.tileCollide = false;
            Projectile.DamageType = DamageClass.Melee;
        }
        
        public override void SetStaticDefaults()
        {
            Main.projFrames[Projectile.type] = 4;
        }

        public override void AI()
        {
            if (Projectile.ai[1] != -1f && Projectile.position.Y > Projectile.ai[1])
            {
                Projectile.tileCollide = true;
            }
            if (Projectile.position.HasNaNs())
            {
                Projectile.Kill();
                return;
            }
            bool flag5 = WorldGen.SolidTile(Framing.GetTileSafely((int)Projectile.position.X / 16, (int)Projectile.position.Y / 16));
            Dust dust6 = Main.dust[Dust.NewDust(new Vector2(Projectile.position.X, Projectile.position.Y), Projectile.width, Projectile.height, 172, 0f, 0f, 0, default, 1f)];
            dust6.position = Projectile.Center;
            dust6.velocity = Vector2.Zero;
            dust6.noGravity = true;
            if (flag5)
            {
                dust6.noLight = true;
            }
            if (Projectile.ai[1] == -1f)
            {
                Projectile.ai[0] += 1f;
                Projectile.velocity = Vector2.Zero;
                Projectile.tileCollide = false;
                Projectile.penetrate = -1;
                Projectile.position = Projectile.Center;
                Projectile.width = Projectile.height = 140;
                Projectile.Center = Projectile.position;
                Projectile.alpha -= 10;
                if (Projectile.alpha < 0)
                {
                    Projectile.alpha = 0;
                }
                if (++Projectile.frameCounter >= Projectile.MaxUpdates * 3)
                {
                    Projectile.frameCounter = 0;
                    Projectile.frame++;
                }
                if (Projectile.ai[0] >= (float)(Main.projFrames[Projectile.type] * Projectile.MaxUpdates * 3))
                {
                    Projectile.Kill();
                }
                return;
            }
            Projectile.alpha = 255;
            if (Projectile.numUpdates == 0)
            {
                int num158 = -1;
                float num159 = 60f;
                for (int num160 = 0; num160 < Main.maxNPCs; num160++)
                {
                    NPC nPC = Main.npc[num160];
                    if (nPC.CanBeChasedBy(Projectile, false))
                    {
                        float num161 = Projectile.Distance(nPC.Center);
                        if (num161 < num159 && Collision.CanHitLine(Projectile.Center, 0, 0, nPC.Center, 0, 0))
                        {
                            num159 = num161;
                            num158 = num160;
                        }
                    }
                }
                if (num158 != -1)
                {
                    Projectile.ai[0] = 0f;
                    Projectile.ai[1] = -1f;
                    Projectile.netUpdate = true;
                }
            }
        }

        public override void Kill(int timeLeft)
        {
            SoundEngine.PlaySound(SoundID.Item14, Projectile.position);
            bool flag = WorldGen.SolidTile(Framing.GetTileSafely((int)Projectile.position.X / 16, (int)Projectile.position.Y / 16));
            for (int m = 0; m < 4; m++)
            {
                Dust.NewDust(new Vector2(Projectile.position.X, Projectile.position.Y), Projectile.width, Projectile.height, 172, 0f, 0f, 100, default, 1.5f);
            }
            for (int n = 0; n < 4; n++)
            {
                int num10 = Dust.NewDust(new Vector2(Projectile.position.X, Projectile.position.Y), Projectile.width, Projectile.height, 172, 0f, 0f, 0, default, 2.5f);
                Main.dust[num10].noGravity = true;
                Main.dust[num10].velocity *= 3f;
                if (flag)
                {
                    Main.dust[num10].noLight = true;
                }
                num10 = Dust.NewDust(new Vector2(Projectile.position.X, Projectile.position.Y), Projectile.width, Projectile.height, 172, 0f, 0f, 100, default, 1.5f);
                Main.dust[num10].velocity *= 2f;
                Main.dust[num10].noGravity = true;
                if (flag)
                {
                    Main.dust[num10].noLight = true;
                }
            }
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            target.AddBuff(BuffID.Ichor, 60);
            Projectile.Kill();
        }
    }
}
