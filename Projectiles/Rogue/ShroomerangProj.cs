using CalamityMod.Buffs.StatBuffs;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Rogue
{
    public class ShroomerangProj : ModProjectile
    {
        public override string Texture => "CalamityMod/Items/Weapons/Rogue/Shroomerang";

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Shroomerang");
        }

        public override void SetDefaults()
        {
            Projectile.width = 30;
            Projectile.height = 30;
            Projectile.friendly = true;
            Projectile.penetrate = -1;
            Projectile.aiStyle = 3;
            Projectile.timeLeft = 300;
            aiType = ProjectileID.WoodenBoomerang;
            Projectile.Calamity().rogue = true;
        }

        public override void AI()
        {
            Lighting.AddLight(Projectile.Center, (255 - Projectile.alpha) * 0f / 255f, (255 - Projectile.alpha) * 0.35f / 255f, (255 - Projectile.alpha) * 0.5f / 255f);
            if (Main.rand.NextBool(5))
            {
                Dust.NewDust(Projectile.position + Projectile.velocity, Projectile.width, Projectile.height, 56, Projectile.velocity.X * 0.5f, Projectile.velocity.Y * 0.5f);
            }
            if (Projectile.Calamity().stealthStrike)
            {
                if (Projectile.timeLeft % 15 == 0 && Projectile.owner == Main.myPlayer)
                {
                    Vector2 vector62 = Main.player[Projectile.owner].Center - Projectile.Center;
                    Vector2 vector63 = vector62 * -1f;
                    vector63.Normalize();
                    vector63 *= (float)Main.rand.Next(45, 65) * 0.1f;
                    vector63 = vector63.RotatedBy((Main.rand.NextDouble() - 0.5) * 1.5707963705062866, default);
                    Projectile.NewProjectile(Projectile.Center.X, Projectile.Center.Y, vector63.X, vector63.Y, ModContent.ProjectileType<ShroomerangSpore>(), (int)(Projectile.damage * 0.1), Projectile.knockBack * 0.2f, Projectile.owner, -10f, 0f);
                }
            }
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
        {
            Texture2D tex = Main.projectileTexture[Projectile.type];
            spriteBatch.Draw(tex, Projectile.Center - Main.screenPosition, null, Projectile.GetAlpha(lightColor), Projectile.rotation, tex.Size() / 2f, Projectile.scale, SpriteEffects.None, 0f);
            return false;
        }

        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
            Player player = Main.player[Projectile.owner];
            if (Projectile.Calamity().stealthStrike)
                player.AddBuff(ModContent.BuffType<Mushy>(), 720);
        }

        public override void OnHitPvp(Player target, int damage, bool crit)
        {
            Player player = Main.player[Projectile.owner];
            if (Projectile.Calamity().stealthStrike)
                player.AddBuff(ModContent.BuffType<Mushy>(), 720);
        }
    }
}
