using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
namespace CalamityMod.Projectiles.Rogue
{
    public class BrackishSpear : ModProjectile
    {
        public override string Texture => "CalamityMod/Projectiles/Magic/AtlantisSpear";

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Atlantis Spear");
        }

        public override void SetDefaults()
        {
            Projectile.width = 52;
            Projectile.height = 52;
            Projectile.aiStyle = ProjAIStyleID.Vilethorn;
            Projectile.friendly = true;
            Projectile.ignoreWater = true;
            Projectile.alpha = 255;
            Projectile.penetrate = -1;
            Projectile.tileCollide = false;
            Projectile.DamageType = RogueDamageClass.Instance;
            AIType = ProjectileID.CrystalVileShardHead;
            Projectile.usesIDStaticNPCImmunity = true;
            Projectile.idStaticNPCHitCooldown = 6;
        }

        public override void AI()
        {
            Projectile.rotation = (float)Math.Atan2((double)Projectile.velocity.Y, (double)Projectile.velocity.X) + 0.785f;
            if (Projectile.ai[0] == 0f)
            {
                Projectile.alpha -= 50;
                if (Projectile.alpha <= 0)
                {
                    Projectile.alpha = 0;
                    Projectile.ai[0] = 1f;
                    if (Projectile.ai[1] == 0f)
                    {
                        Projectile.ai[1] += 1f;
                        Projectile.position += Projectile.velocity * 1f;
                    }
                    if (Main.myPlayer == Projectile.owner)
                    {
                        int projType = Projectile.type;
                        if (Projectile.ai[1] >= (float)(12 + Main.rand.Next(2)))
                        {
                            projType = ModContent.ProjectileType<BrackishSpear2>();
                        }
                        int dmg = Projectile.damage;
                        float kBack = Projectile.knockBack;
                        int number = Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.position.X + Projectile.velocity.X + (float)(Projectile.width / 2), Projectile.position.Y + Projectile.velocity.Y + (float)(Projectile.height / 2), Projectile.velocity.X, Projectile.velocity.Y, projType, dmg, kBack, Projectile.owner, 0f, Projectile.ai[1] + 1f);
                        NetMessage.SendData(MessageID.SyncProjectile, -1, -1, null, number, 0f, 0f, 0f, 0, 0, 0);
                    }
                }
            }
            else
            {
                if (Projectile.alpha < 170 && Projectile.alpha + 5 >= 170)
                {
                    for (int num55 = 0; num55 < 8; num55++)
                    {
                        int num56 = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, 206, Projectile.velocity.X * 0.005f, Projectile.velocity.Y * 0.005f, 200, default, 1f);
                        Main.dust[num56].noGravity = true;
                        Main.dust[num56].velocity *= 0.5f;
                    }
                }
                Projectile.alpha += 7;
                if (Projectile.alpha >= 255)
                {
                    Projectile.Kill();
                }
            }
            if (Main.rand.NextBool(4))
            {
                Dust.NewDust(Projectile.position + Projectile.velocity, Projectile.width, Projectile.height, 206, Projectile.velocity.X * 0.005f, Projectile.velocity.Y * 0.005f);
            }
        }

        public override Color? GetAlpha(Color lightColor) => new Color(200, 200, 200, Projectile.alpha);

        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
            target.AddBuff(BuffID.Venom, 90);
        }

        public override void OnHitPvp(Player target, int damage, bool crit)
        {
            target.AddBuff(BuffID.Venom, 90);
        }

        public override void Kill(int timeLeft)
        {
            int numProj = 2;
            float rotation = MathHelper.ToRadians(20);
            for (int i = 0; i < numProj; i++)
            {
                Vector2 perturbedSpeed = new Vector2(Projectile.velocity.X, Projectile.velocity.Y).RotatedBy(MathHelper.Lerp(-rotation, rotation, i / (numProj - 1)));
                int projectile2 = Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center.X, Projectile.Center.Y, perturbedSpeed.X, perturbedSpeed.Y, ModContent.ProjectileType<BrackishSpear2>(), Projectile.damage, Projectile.knockBack, Projectile.owner, 0f, 0f);
                Main.projectile[projectile2].penetrate = 1;
            }
            for (int k = 0; k < 3; k++)
            {
                Dust.NewDust(Projectile.position + Projectile.velocity, Projectile.width, Projectile.height, 206, Projectile.oldVelocity.X * 0.005f, Projectile.oldVelocity.Y * 0.005f);
            }
        }
    }
}
