using System;
using CalamityMod.Buffs.DamageOverTime;
using CalamityMod.CalPlayer;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Melee
{
    public class Cyclone : ModProjectile, ILocalizedModType
    {
        public new string LocalizationCategory => "Projectiles.Melee";
        public int dustVortex = 0;

        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.TrailCacheLength[Type] = 6;
            ProjectileID.Sets.TrailingMode[Type] = 0;
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
            // Only begin colliding with tiles when the projectile is not inside tiles
            CalamityUtils.PreventTileCollisionUntilHitboxIsOutsideOfTiles(Projectile);

            Projectile.rotation += 2.5f;

            Projectile.alpha -= 5;
            Projectile.ai[1] += 1f;
            if (Projectile.alpha < 50)
            {
                Projectile.alpha = 50;
                if (Projectile.ai[1] >= 15f)
                {
                    for (int i = 1; i <= 6; i++)
                    {
                        Vector2 dustspeed = new Vector2(3f, 3f).RotatedBy(MathHelper.ToRadians(dustVortex));
                        int d = Dust.NewDust(Projectile.Center, Projectile.width / 2, Projectile.height / 2, DustID.Smoke, dustspeed.X, dustspeed.Y, 200, new Color(232, 251, 250, 200), 1.3f);
                        Main.dust[d].noGravity = true;
                        Main.dust[d].velocity = dustspeed;
                        dustVortex += 60;
                    }
                    dustVortex -= 355;
                    Projectile.ai[1] = 0f;
                }
            }

            float num472 = Projectile.Center.X;
            float num473 = Projectile.Center.Y;
            float num474 = 600f;
            for (int num475 = 0; num475 < Main.maxNPCs; num475++)
            {
                NPC npc = Main.npc[num475];
                if (npc.CanBeChasedBy(Projectile, false) && Collision.CanHit(Projectile.Center, 1, 1, npc.Center, 1, 1) && !CalamityPlayer.areThereAnyDamnBosses)
                {
                    float npcCenterX = npc.position.X + (float)(npc.width / 2);
                    float npcCenterY = npc.position.Y + (float)(npc.height / 2);
                    float num478 = Math.Abs(Projectile.position.X + (float)(Projectile.width / 2) - npcCenterX) + Math.Abs(Projectile.position.Y + (float)(Projectile.height / 2) - npcCenterY);
                    if (num478 < num474)
                    {
                        if (npc.position.X < num472)
                            npc.velocity.X += 0.05f;
                        else
                            npc.velocity.X -= 0.05f;

                        if (npc.position.Y < num473)
                            npc.velocity.Y += 0.05f;
                        else
                            npc.velocity.Y -= 0.05f;
                    }
                }
            }
        }

        public override Color? GetAlpha(Color lightColor) => new Color(204, 255, 255, Projectile.alpha);

        public override bool PreDraw(ref Color lightColor)
        {
            CalamityUtils.DrawAfterimagesCentered(Projectile, ProjectileID.Sets.TrailingMode[Type], lightColor, 1);
            return false;
        }
        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            target.AddBuff(ModContent.BuffType<WindChilled>(), 180);
        }

        public override void OnHitPlayer(Player target, Player.HurtInfo info)
        {
            target.AddBuff(ModContent.BuffType<WindChilled>(), 180);
        }

        public override void OnKill(int timeLeft)
        {
            SoundEngine.PlaySound(SoundID.Item60 with { Volume = SoundID.Item60.Volume * 0.6f }, Projectile.Center);

            for (int i = 0; i <= 360; i += 3)
            {
                Vector2 dustspeed = new Vector2(3f, 3f).RotatedBy(MathHelper.ToRadians(i));
                int d = Dust.NewDust(Projectile.Center, Projectile.width, Projectile.height, DustID.Smoke, dustspeed.X, dustspeed.Y, 200, new Color(232, 251, 250, 200), 1.4f);
                Main.dust[d].noGravity = true;
                Main.dust[d].position = Projectile.Center;
                Main.dust[d].velocity = dustspeed;
            }
        }
    }
}
