using CalamityMod.Tiles.Abyss;
using CalamityMod.Walls;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Audio;

namespace CalamityMod.Projectiles.Ranged
{
    public class MortarRoundProj : ModProjectile
    {
        public override string Texture => "CalamityMod/Items/Ammo/MortarRound";

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Mortar");
        }

        public override void SetDefaults()
        {
            Projectile.width = 14;
            Projectile.height = 14;
            Projectile.friendly = true;
            Projectile.penetrate = 1;
            Projectile.timeLeft = 150;
            Projectile.DamageType = DamageClass.Ranged;
        }

        public override void AI()
        {
            Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.PiOver2;
            if (Projectile.velocity.Length() >= 8f)
            {
                for (int d = 0; d < 2; d++)
                {
                    float xOffset = 0f;
                    float yOffset = 0f;
                    if (d == 1)
                    {
                        xOffset = Projectile.velocity.X * 0.5f;
                        yOffset = Projectile.velocity.Y * 0.5f;
                    }
                    int fire = Dust.NewDust(new Vector2(Projectile.position.X + 3f + xOffset, Projectile.position.Y + 3f + yOffset) - Projectile.velocity * 0.5f, Projectile.width - 8, Projectile.height - 8, DustID.Fire, 0f, 0f, 100, default, 1f);
                    Main.dust[fire].scale *= 2f + (float)Main.rand.Next(10) * 0.1f;
                    Main.dust[fire].velocity *= 0.2f;
                    Main.dust[fire].noGravity = true;
                    int smoke = Dust.NewDust(new Vector2(Projectile.position.X + 3f + xOffset, Projectile.position.Y + 3f + yOffset) - Projectile.velocity * 0.5f, Projectile.width - 8, Projectile.height - 8, DustID.Smoke, 0f, 0f, 100, default, 0.5f);
                    Main.dust[smoke].fadeIn = 1f + (float)Main.rand.Next(5) * 0.1f;
                    Main.dust[smoke].velocity *= 0.05f;
                }
            }
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
        {
            Texture2D tex = ModContent.Request<Texture2D>(Texture).Value;
            spriteBatch.Draw(tex, Projectile.Center - Main.screenPosition, null, Projectile.GetAlpha(lightColor), Projectile.rotation, tex.Size() / 2f, Projectile.scale, SpriteEffects.None, 0f);
            return false;
        }

        public override void Kill(int timeLeft)
        {
            CalamityGlobalProjectile.ExpandHitboxBy(Projectile, 200);
            Projectile.maxPenetrate = Projectile.penetrate = -1;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 10;
            Projectile.knockBack *= 5f;
            Projectile.Damage();
            SoundEngine.PlaySound(SoundID.Item14, Projectile.Center);

            for (int d = 0; d < 40; d++)
            {
                int smoke = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.Smoke, 0f, 0f, 100, default, 2f);
                Main.dust[smoke].velocity *= 3f;
                if (Main.rand.NextBool(2))
                {
                    Main.dust[smoke].scale = 0.5f;
                    Main.dust[smoke].fadeIn = 1f + (float)Main.rand.Next(10) * 0.1f;
                }
            }
            for (int d = 0; d < 70; d++)
            {
                int fire = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.Fire, 0f, 0f, 100, default, 3f);
                Main.dust[fire].noGravity = true;
                Main.dust[fire].velocity *= 5f;
                fire = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.Fire, 0f, 0f, 100, default, 2f);
                Main.dust[fire].velocity *= 2f;
            }
            CalamityUtils.ExplosionGores(Projectile.Center, 3);

            CalamityGlobalProjectile.ExpandHitboxBy(Projectile, 14);

            if (Projectile.owner == Main.myPlayer)
            {
                CalamityUtils.ExplodeandDestroyTiles(Projectile, 5, false, new List<int>()
                {
                    ModContent.TileType<AbyssGravel>(),
                    ModContent.TileType<Voidstone>()
                },
                new List<int>()
                {
                    ModContent.WallType<AbyssGravelWall>(),
                    ModContent.WallType<VoidstoneWallUnsafe>()
                });
            }
        }
    }
}
