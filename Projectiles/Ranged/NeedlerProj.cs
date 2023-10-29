using CalamityMod.Buffs.DamageOverTime;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
namespace CalamityMod.Projectiles.Ranged
{
    public class NeedlerProj : ModProjectile, ILocalizedModType
    {
        public new string LocalizationCategory => "Projectiles.Ranged";
        public override void SetDefaults()
        {
            Projectile.width = 10;
            Projectile.height = 10;
            Projectile.alpha = 255;
            Projectile.friendly = true;
            Projectile.ignoreWater = true;
            Projectile.penetrate = 3;
            Projectile.alpha = 255;
            Projectile.DamageType = DamageClass.Ranged;
            Projectile.extraUpdates = 2;
            Projectile.aiStyle = ProjAIStyleID.Nail;
            AIType = ProjectileID.NailFriendly;
            Projectile.usesIDStaticNPCImmunity = true;
            Projectile.idStaticNPCHitCooldown = 1;
        }

        public override void AI()
        {
            Projectile.alpha -= 10;
            if (Projectile.alpha < 0)
            {
                Projectile.alpha = 0;
            }
            Projectile.localAI[1] += 1f;
            if (Projectile.localAI[1] > 4f)
            {
                for (int i = 0; i < 2; i++)
                {
                    int dusty = Dust.NewDust(new Vector2(Projectile.position.X, Projectile.position.Y), Projectile.width, Projectile.height, 46, 0f, 0f, 100, default, 0.75f);
                    Main.dust[dusty].noGravity = true;
                    Main.dust[dusty].velocity *= 0f;
                }
            }
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone) => target.AddBuff(BuffID.Venom, 180);

        public override void OnHitPlayer(Player target, Player.HurtInfo info) => target.AddBuff(BuffID.Venom, 180);

        public override void OnKill(int timeLeft)
        {
            Projectile.position = Projectile.Center;
            Projectile.width = Projectile.height = 48;
            Projectile.position.X = Projectile.position.X - (float)(Projectile.width / 2);
            Projectile.position.Y = Projectile.position.Y - (float)(Projectile.height / 2);
            for (int j = 0; j < 3; j++)
            {
                int dust = Dust.NewDust(new Vector2(Projectile.position.X, Projectile.position.Y), Projectile.width, Projectile.height, 46, 0f, 0f, 100, new Color(Main.DiscoR, 203, 103), 1.2f);
                Main.dust[dust].velocity *= 3f;
                if (Main.rand.NextBool())
                {
                    Main.dust[dust].scale = 0.5f;
                    Main.dust[dust].fadeIn = 1f + (float)Main.rand.Next(10) * 0.1f;
                }
            }
            for (int k = 0; k < 5; k++)
            {
                int dust2 = Dust.NewDust(new Vector2(Projectile.position.X, Projectile.position.Y), Projectile.width, Projectile.height, 39, 0f, 0f, 100, new Color(Main.DiscoR, 203, 103), 1.7f);
                Main.dust[dust2].noGravity = true;
                Main.dust[dust2].velocity *= 5f;
                dust2 = Dust.NewDust(new Vector2(Projectile.position.X, Projectile.position.Y), Projectile.width, Projectile.height, 44, 0f, 0f, 100, new Color(Main.DiscoR, 203, 103), 1f);
                Main.dust[dust2].velocity *= 2f;
            }
        }
    }
}
