using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Melee.Yoyos
{
    public class QuagmireYoyo : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Quagmire");
            ProjectileID.Sets.YoyosLifeTimeMultiplier[Projectile.type] = 12f;
            ProjectileID.Sets.YoyosMaximumRange[Projectile.type] = 400f;
            ProjectileID.Sets.YoyosTopSpeed[Projectile.type] = 15f;
        }

        public override void SetDefaults()
        {
            Projectile.aiStyle = 99;
            Projectile.width = 16;
            Projectile.height = 16;
            Projectile.scale = 1.25f;
            Projectile.friendly = true;
            Projectile.DamageType = DamageClass.Melee;
            Projectile.penetrate = -1;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 10;
        }

        public override void AI()
        {
            if ((Projectile.position - Main.player[Projectile.owner].position).Length() > 3200f) //200 blocks
                Projectile.Kill();
            if (Main.rand.NextBool(5))
                Dust.NewDust(Projectile.position + Projectile.velocity, Projectile.width, Projectile.height, 44, Projectile.velocity.X * 0.5f, Projectile.velocity.Y * 0.5f);
            if (Projectile.owner == Main.myPlayer)
            {
                int proj;
                if (Main.rand.NextBool(10))
                {
                    proj = Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center.X, Projectile.Center.Y, Projectile.velocity.X * 0.35f, Projectile.velocity.Y * 0.35f, ProjectileID.SporeGas, (int)(Projectile.damage * 0.5), Projectile.knockBack, Projectile.owner);
                    if (proj.WithinBounds(Main.maxProjectiles))
                    {
                        Main.projectile[proj].DamageType = DamageClass.Melee;
                        Main.projectile[proj].usesLocalNPCImmunity = true;
                        Main.projectile[proj].localNPCHitCooldown = 30;
                    }
                }
                if (Main.rand.NextBool(30))
                {
                    proj = Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center.X, Projectile.Center.Y, Projectile.velocity.X * 0.25f, Projectile.velocity.Y * 0.25f, ProjectileID.SporeGas2, (int)(Projectile.damage * 0.6), Projectile.knockBack, Projectile.owner);
                    if (proj.WithinBounds(Main.maxProjectiles))
                    {
                        Main.projectile[proj].DamageType = DamageClass.Melee;
                        Main.projectile[proj].usesLocalNPCImmunity = true;
                        Main.projectile[proj].localNPCHitCooldown = 30;
                    }
                }
                if (Main.rand.NextBool(50))
                {
                    proj = Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center.X, Projectile.Center.Y, Projectile.velocity.X * 0.15f, Projectile.velocity.Y * 0.15f, ProjectileID.SporeGas3, (int)(Projectile.damage * 0.7), Projectile.knockBack, Projectile.owner);
                    if (proj.WithinBounds(Main.maxProjectiles))
                    {
                        Main.projectile[proj].DamageType = DamageClass.Melee;
                        Main.projectile[proj].usesLocalNPCImmunity = true;
                        Main.projectile[proj].localNPCHitCooldown = 30;
                    }
                }
            }
        }

        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
            target.AddBuff(BuffID.Venom, 180);
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D tex = ModContent.Request<Texture2D>(Texture).Value;
            Main.EntitySpriteDraw(tex, Projectile.Center - Main.screenPosition, null, Projectile.GetAlpha(lightColor), Projectile.rotation, tex.Size() / 2f, Projectile.scale, SpriteEffects.None, 0);
            return false;
        }
    }
}
