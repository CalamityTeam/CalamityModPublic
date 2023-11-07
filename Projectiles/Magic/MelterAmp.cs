using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.ModLoader;
namespace CalamityMod.Projectiles.Magic
{
    public class MelterAmp : ModProjectile, ILocalizedModType
    {
        public new string LocalizationCategory => "Projectiles.Magic";
        public override void SetStaticDefaults()
        {
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
            bool isProjectile = Projectile.type == ModContent.ProjectileType<MelterAmp>();
            Player player = Main.player[Projectile.owner];
            if (isProjectile)
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
                if (!player.ActiveItem().CountsAsClass<MagicDamageClass>() || player.ActiveItem().shoot != ModContent.ProjectileType<MelterNote1>())
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
                Vector2 projAimDirection = new Vector2(Projectile.position.X + (float)Projectile.width * 0.5f, Projectile.position.Y + (float)Projectile.height * 0.5f);
                float ampXDirection = (float)Main.mouseX + Main.screenPosition.X - projAimDirection.X;
                float ampYDirection = (float)Main.mouseY + Main.screenPosition.Y - projAimDirection.Y;
                if (player.gravDir == -1f)
                {
                    ampYDirection = Main.screenPosition.Y + (float)Main.screenHeight - (float)Main.mouseY - projAimDirection.Y;
                }
                float ampAimDistance = (float)Math.Sqrt((double)(ampXDirection * ampXDirection + ampYDirection * ampYDirection));
                if (ampAimDistance == 0f)
                {
                    projAimDirection = new Vector2(player.position.X + (float)(player.width / 2), player.position.Y + (float)(player.height / 2));
                    ampXDirection = Projectile.position.X + (float)Projectile.width * 0.5f - projAimDirection.X;
                    ampYDirection = Projectile.position.Y + (float)Projectile.height * 0.5f - projAimDirection.Y;
                    ampAimDistance = (float)Math.Sqrt((double)(ampXDirection * ampXDirection + ampYDirection * ampYDirection));
                }
                ampAimDistance = 20f / ampAimDistance;
                ampXDirection *= ampAimDistance;
                ampYDirection *= ampAimDistance;
                float VelocityX = ampXDirection;
                float VelocityY = ampYDirection;
                int noteType = Main.rand.Next(0, 2);
                if (noteType == 0)
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
                Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center.X, Projectile.Center.Y, VelocityX, VelocityY, type, Damage, Projectile.knockBack, Projectile.owner, 0.0f, 0.0f);
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
