using CalamityMod.Items.Weapons.Rogue;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
namespace CalamityMod.Projectiles.Rogue
{
    public class CrystalPiercerProjectile : ModProjectile
    {
        public override string Texture => "CalamityMod/Items/Weapons/Rogue/CrystalPiercer";

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Crystal Piercer");
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 6;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 0;
        }

        public override void SetDefaults()
        {
            Projectile.width = 12;
            Projectile.height = 12;
            Projectile.friendly = true;
            Projectile.penetrate = -1;
            Projectile.aiStyle = 113;
            Projectile.timeLeft = 600;
            aiType = ProjectileID.BoneJavelin;
            Projectile.Calamity().rogue = true;
            Projectile.localNPCHitCooldown = 10;
        }

        public override void AI()
        {
            if (Main.rand.NextBool(3))
            {
                Dust.NewDust(Projectile.position + Projectile.velocity, Projectile.width, Projectile.height, 173, Projectile.velocity.X * 0.1f, Projectile.velocity.Y * 0.1f);
            }
            Projectile.spriteDirection = Projectile.direction = (Projectile.velocity.X > 0).ToDirectionInt();
            Projectile.rotation = Projectile.velocity.ToRotation() + (Projectile.spriteDirection == 1 ? 0f : MathHelper.Pi);
            Projectile.rotation += Projectile.spriteDirection * MathHelper.ToRadians(45f);

            if (Projectile.Calamity().stealthStrike)
            {
                if (Projectile.timeLeft % 4 == 0)
                {
                    if (Projectile.owner == Main.myPlayer)
                    {
                        Projectile.NewProjectile(Projectile.Center.X + Main.rand.NextFloat(-15f, 15f), Projectile.Center.Y + Main.rand.NextFloat(-15f, 15f), Projectile.velocity.X, Projectile.velocity.Y, ModContent.ProjectileType<CrystalPiercerShard>(), (int)(Projectile.damage * 0.4), Projectile.knockBack * 0.4f, Projectile.owner, 0f, 0f);
                    }
                }
            }
        }

        public override bool PreDraw(ref Color lightColor)
        {
            CalamityUtils.DrawAfterimagesCentered(Projectile, ProjectileID.Sets.TrailingMode[Projectile.type], lightColor, 1);
            return false;
        }

        //glowmask effect if stealth strike
        public override Color? GetAlpha(Color lightColor)
        {
            if (Projectile.Calamity().stealthStrike)
                return new Color(200, 200, 200, 200);
            else
                return null;
        }

        public override void Kill(int timeLeft)
        {
            for (int k = 0; k < 5; k++)
            {
                Dust.NewDust(Projectile.position + Projectile.velocity, Projectile.width, Projectile.height, 173, Projectile.oldVelocity.X * 0.5f, Projectile.oldVelocity.Y * 0.5f);
            }
            if (Main.rand.NextBool(2))
            {
                Item.NewItem((int)Projectile.position.X, (int)Projectile.position.Y, Projectile.width, Projectile.height, ModContent.ItemType<CrystalPiercer>());
            }
        }

        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
            target.AddBuff(BuffID.Frostburn, 120);
        }

        public override void OnHitPvp(Player target, int damage, bool crit)
        {
            target.AddBuff(BuffID.Frostburn, 120);
        }
    }
}
