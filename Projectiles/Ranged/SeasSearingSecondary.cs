using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
namespace CalamityMod.Projectiles.Ranged
{
    public class SeasSearingSecondary : ModProjectile, ILocalizedModType
    {
        public new string LocalizationCategory => "Projectiles.Ranged";
        public override string Texture => "CalamityMod/Projectiles/InvisibleProj";

        public override void SetDefaults()
        {
            Projectile.width = 16;
            Projectile.height = 32;
            Projectile.friendly = true;
            Projectile.DamageType = DamageClass.Ranged;
            Projectile.penetrate = 1;
            Projectile.timeLeft = 600;
        }

        public override void AI()
        {
            if (Projectile.scale <= 3.6f)
            {
                Projectile.scale *= 1.01f;
                Projectile.width = (int)(16f * Projectile.scale);
                Projectile.height = (int)(32f * Projectile.scale);
            }
            Projectile.rotation = (float)Math.Atan2((double)Projectile.velocity.Y, (double)Projectile.velocity.X) + 1.57f;
            Projectile.localAI[0] += 1f;
            if (Projectile.localAI[0] > 4f)
            {
                for (int i = 0; i < 3; i++)
                {
                    int aquaDust = Dust.NewDust(new Vector2(Projectile.position.X, Projectile.position.Y), Projectile.width, Projectile.height, 217, 0f, 0f, 100, new Color(60, Main.DiscoG, 190), Projectile.scale);
                    Main.dust[aquaDust].noGravity = true;
                    Main.dust[aquaDust].velocity *= 0f;
                    int waterDust = Dust.NewDust(new Vector2(Projectile.position.X, Projectile.position.Y), Projectile.width, Projectile.height, 202, 0f, 0f, 100, new Color(60, Main.DiscoG, 190), Projectile.scale);
                    Main.dust[waterDust].noGravity = true;
                    Main.dust[waterDust].velocity *= 0f;
                }
            }
        }

        public override void OnKill(int timeLeft)
        {
            if (Projectile.owner == Main.myPlayer)
            {
                int endoftime = Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center.X, Projectile.Center.Y, 0f, 0f, ModContent.ProjectileType<TyphoonBubble>(), Projectile.damage, Projectile.knockBack, Projectile.owner, 0f, 0f);
                Main.projectile[endoftime].localAI[1] = 1f;
            }
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone) => target.AddBuff(BuffID.Venom, 180);
    }
}
