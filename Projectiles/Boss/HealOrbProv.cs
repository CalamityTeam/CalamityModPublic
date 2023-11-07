using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.DataStructures;
using Terraria.ModLoader;
using Terraria.ID;

namespace CalamityMod.Projectiles.Boss
{
    public class HealOrbProv : ModProjectile, ILocalizedModType
    {
        public new string LocalizationCategory => "Projectiles.Boss";
        public override string Texture => "CalamityMod/Projectiles/InvisibleProj";

        public override void SetDefaults()
        {
            Projectile.width = 4;
            Projectile.height = 4;
            Projectile.friendly = true;
            Projectile.penetrate = 1;
            Projectile.tileCollide = false;
            Projectile.timeLeft = 300;
            Projectile.extraUpdates = 3;
        }

        public override void AI()
        {
            int playerTracker = (int)Projectile.ai[0];
            Vector2 projDirection = new Vector2(Projectile.position.X + (float)Projectile.width * 0.5f, Projectile.position.Y + (float)Projectile.height * 0.5f);
            float playerDistX = Main.player[playerTracker].Center.X - projDirection.X;
            float playerDistY = Main.player[playerTracker].Center.Y - projDirection.Y;
            float playerDistance = (float)Math.Sqrt((double)(playerDistX * playerDistX + playerDistY * playerDistY));
            if (playerDistance < 50f && Projectile.position.X < Main.player[playerTracker].position.X + (float)Main.player[playerTracker].width && Projectile.position.X + (float)Projectile.width > Main.player[playerTracker].position.X && Projectile.position.Y < Main.player[playerTracker].position.Y + (float)Main.player[playerTracker].height && Projectile.position.Y + (float)Projectile.height > Main.player[playerTracker].position.Y)
            {
                if (Projectile.owner == Main.myPlayer)
                {
                    int healAmount = (int)Projectile.ai[1];
                    Main.player[playerTracker].HealEffect(healAmount, false);
                    Main.player[playerTracker].statLife += healAmount;
                    if (Main.player[playerTracker].statLife > Main.player[playerTracker].statLifeMax2)
                    {
                        Main.player[playerTracker].statLife = Main.player[playerTracker].statLifeMax2;
                    }
                    // TODO -- but why
                    if (Main.player[playerTracker].statLife < 0)
                    {
                        Main.player[playerTracker].KillMe(PlayerDeathReason.ByCustomReason(CalamityUtils.GetText("Status.Death.ProvidenceAntiHealing").Format(Main.player[playerTracker].name)), 1000.0, 0, false);
                    }
                    NetMessage.SendData(MessageID.SpiritHeal, -1, -1, null, playerTracker, (float)healAmount, 0f, 0f, 0, 0, 0);
                }
                Projectile.Kill();
            }
            playerDistance = 2.5f / playerDistance;
            playerDistX *= playerDistance;
            playerDistY *= playerDistance;
            Projectile.velocity.X = (Projectile.velocity.X * 15f + playerDistX) / 16f;
            Projectile.velocity.Y = (Projectile.velocity.Y * 15f + playerDistY) / 16f;
            for (int i = 0; i < 1; i++)
            {
                float shortXVel = Projectile.velocity.X * 0.2f * (float)i;
                float shortYVel = -(Projectile.velocity.Y * 0.2f) * (float)i;
                int holyFire = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, 127, 0f, 0f, 100, default, 1f);
                Main.dust[holyFire].noGravity = true;
                Main.dust[holyFire].velocity *= 0f;
                Dust expr_154F9_cp_0 = Main.dust[holyFire];
                expr_154F9_cp_0.position.X -= shortXVel;
                Dust expr_15518_cp_0 = Main.dust[holyFire];
                expr_15518_cp_0.position.Y -= shortYVel;
            }
            return;
        }
    }
}
