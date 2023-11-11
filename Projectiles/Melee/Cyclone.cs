using CalamityMod.CalPlayer;
using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Audio;

namespace CalamityMod.Projectiles.Melee
{
    public class Cyclone : ModProjectile, ILocalizedModType
    {
        public new string LocalizationCategory => "Projectiles.Melee";
        public int dustvortex = 0;
        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 6;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 0;
        }

        public override void SetDefaults()
        {
            Projectile.width = 56;
            Projectile.height = 56;
            Projectile.alpha = 255;
            Projectile.friendly = true;
            Projectile.DamageType = DamageClass.Melee;
            Projectile.timeLeft = 300;
            Projectile.extraUpdates = 2;
            Projectile.penetrate = 2;
            Projectile.ignoreWater = true;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = -1;
            Projectile.tileCollide = false;
        }

        public override void AI()
        {
            Projectile.ai[0]++;
            Projectile.ai[1]++;

            //Code so it doesnt collide on tiles instantly
            if (Projectile.ai[0] >= 12)
                Projectile.tileCollide = true;

            Projectile.rotation += 2.5f;
            Projectile.alpha -= 5;
            if (Projectile.alpha < 50)
            {
                Projectile.alpha = 50;
                if (Projectile.ai[1] >= 15)
                {

                    for (int i = 1; i <= 6; i++)
                    {
                        Vector2 dustspeed = new Vector2(3f, 3f).RotatedBy(MathHelper.ToRadians(dustvortex));
                        int d = Dust.NewDust(Projectile.Center, Projectile.width / 2, Projectile.height / 2, 31, dustspeed.X, dustspeed.Y, 200, new Color(232, 251, 250, 200), 1.3f);
                        Main.dust[d].noGravity = true;
                        Main.dust[d].velocity = dustspeed;
                        dustvortex += 60;
                    }
                    dustvortex -= 355;
                    Projectile.ai[1] = 0;
                }
            }
            float projX = Projectile.Center.X;
            float projY = Projectile.Center.Y;
            for (int i = 0; i < Main.maxNPCs; i++)
            {
                NPC npc = Main.npc[i];
                if (npc.CanBeChasedBy(Projectile, false) && Collision.CanHit(Projectile.Center, 1, 1, npc.Center, 1, 1) && !CalamityPlayer.areThereAnyDamnBosses)
                {
                    float npcCenterX = npc.position.X + (float)(npc.width / 2);
                    float npcCenterY = npc.position.Y + (float)(npc.height / 2);
                    float npcDistance = Math.Abs(Projectile.position.X + (float)(Projectile.width / 2) - npcCenterX) + Math.Abs(Projectile.position.Y + (float)(Projectile.height / 2) - npcCenterY);
                    if (npcDistance < 600f)
                    {
                        if (npc.position.X < projX)
                        {
                            npc.velocity.X += 0.05f;
                        }
                        else
                        {
                            npc.velocity.X -= 0.05f;
                        }
                        if (npc.position.Y < projY)
                        {
                            npc.velocity.Y += 0.05f;
                        }
                        else
                        {
                            npc.velocity.Y -= 0.05f;
                        }
                    }
                }
            }
        }

        public override Color? GetAlpha(Color lightColor)
        {
            return new Color(204, 255, 255, Projectile.alpha);
        }

        public override bool PreDraw(ref Color lightColor)
        {
            CalamityUtils.DrawAfterimagesCentered(Projectile, ProjectileID.Sets.TrailingMode[Projectile.type], lightColor, 1);
            return false;
        }

        public override void OnKill(int timeLeft)
        {
            SoundEngine.PlaySound(SoundID.Item60 with { Volume = SoundID.Item60.Volume * 0.6f}, Projectile.Center);

            for (int i = 0; i <= 360; i += 3)
            {
                Vector2 dustspeed = new Vector2(3f, 3f).RotatedBy(MathHelper.ToRadians(i));
                int d = Dust.NewDust(Projectile.Center, Projectile.width, Projectile.height, 31, dustspeed.X, dustspeed.Y, 200, new Color(232, 251, 250, 200), 1.4f);
                Main.dust[d].noGravity = true;
                Main.dust[d].position = Projectile.Center;
                Main.dust[d].velocity = dustspeed;
            }
        }
    }
}
