using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.ModLoader;
namespace CalamityMod.Projectiles.Magic
{
    public class MelterAmp : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Melter Amp");
            Main.projFrames[Projectile.type] = 3;
        }

        public override void SetDefaults()
        {
            Projectile.width = 34;
            Projectile.height = 38;
            Projectile.friendly = true;
            Projectile.DamageType = DamageClass.Magic;
            Projectile.netImportant = true;
            Projectile.penetrate = -1;
            Projectile.timeLeft = 6000;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 20;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
        }

        public override void AI()
        {
            bool flag64 = Projectile.type == ModContent.ProjectileType<MelterAmp>();
            Player player = Main.player[Projectile.owner];
            if (flag64)
            {
                if (player.dead)
                {
                    Projectile.active = false;
                    return;
                }
                if (player.ownedProjectileCounts[ModContent.ProjectileType<MelterAmp>()] > 1)
                {
                    Projectile.active = false;
                    return;
                }
                if (!player.ActiveItem().magic || player.ActiveItem().shoot != ModContent.ProjectileType<MelterNote1>())
                {
                    Projectile.active = false;
                    return;
                }
            }
            Lighting.AddLight(Projectile.Center, 0.75f, 0.75f, 0.75f);
            if (Projectile.ai[0] > 0f)
            {
                Projectile.ai[0] += 1f;
                if (Projectile.ai[0] > 6f)
                {
                    Projectile.ai[0] = 0f;
                }
            }
            if (Main.myPlayer == Projectile.owner && Projectile.ai[0] == 0f)
            {
                Projectile.ai[0] = 1f;
                int Damage = Projectile.damage;
                int type;
                Projectile.netUpdate = true;
                float num127 = 20f;
                Vector2 vector11 = new Vector2(Projectile.position.X + (float)Projectile.width * 0.5f, Projectile.position.Y + (float)Projectile.height * 0.5f);
                float num128 = (float)Main.mouseX + Main.screenPosition.X - vector11.X;
                float num129 = (float)Main.mouseY + Main.screenPosition.Y - vector11.Y;
                if (player.gravDir == -1f)
                {
                    num129 = Main.screenPosition.Y + (float)Main.screenHeight - (float)Main.mouseY - vector11.Y;
                }
                float num130 = (float)Math.Sqrt((double)(num128 * num128 + num129 * num129));
                if (num130 == 0f)
                {
                    vector11 = new Vector2(player.position.X + (float)(player.width / 2), player.position.Y + (float)(player.height / 2));
                    num128 = Projectile.position.X + (float)Projectile.width * 0.5f - vector11.X;
                    num129 = Projectile.position.Y + (float)Projectile.height * 0.5f - vector11.Y;
                    num130 = (float)Math.Sqrt((double)(num128 * num128 + num129 * num129));
                }
                num130 = num127 / num130;
                num128 *= num130;
                num129 *= num130;
                float VelocityX = num128;
                float VelocityY = num129;
                int note = Main.rand.Next(0, 2);
                if (note == 0)
                {
                    Damage = (int)(Projectile.damage * 1.5f);
                    type = ModContent.ProjectileType<MelterNote1>();
                }
                else
                {
                    VelocityX *= 1.5f;
                    VelocityY *= 1.5f;
                    type = ModContent.ProjectileType<MelterNote2>();
                }
                Projectile.NewProjectile(Projectile.Center.X, Projectile.Center.Y, VelocityX, VelocityY, type, Damage, Projectile.knockBack, Projectile.owner, 0.0f, 0.0f);
            }
            Projectile.frameCounter++;
            if (Projectile.frameCounter > 5)
            {
                Projectile.frame++;
                Projectile.frameCounter = 0;
            }
            if (Projectile.frame > 2)
            {
                Projectile.frame = 0;
            }
        }
    }
}
