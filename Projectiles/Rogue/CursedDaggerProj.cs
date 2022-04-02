using CalamityMod.Dusts;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Rogue
{
    public class CursedDaggerProj : ModProjectile
    {
        public override string Texture => "CalamityMod/Items/Weapons/Rogue/CursedDagger";

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Cursed Dagger");
        }

        public override void SetDefaults()
        {
            projectile.width = 12;
            projectile.height = 12;
            projectile.friendly = true;
            projectile.penetrate = 3;
            projectile.aiStyle = 2;
            projectile.timeLeft = 600;
            aiType = ProjectileID.ThrowingKnife;
            projectile.Calamity().rogue = true;
            projectile.localNPCHitCooldown = 10;
        }

        public override void AI()
        {
            if (Main.rand.NextBool(4))
            {
                Dust.NewDust(projectile.position + projectile.velocity, projectile.width, projectile.height, (int)CalamityDusts.SulfurousSeaAcid, projectile.velocity.X * 0.5f, projectile.velocity.Y * 0.5f);
            }

            if (projectile.Calamity().stealthStrike)
            {
                if (projectile.timeLeft % 8 == 0)
                {
                    if (projectile.owner == Main.myPlayer)
                    {
                        Vector2 velocity = new Vector2(Main.rand.NextFloat(-14f, 14f), Main.rand.NextFloat(-14f, 14f));
                        int flame = Projectile.NewProjectile(projectile.Center, velocity, Main.rand.NextBool(2) ? ProjectileID.CursedFlameFriendly : ProjectileID.CursedDartFlame, (int)(projectile.damage * 0.5), projectile.knockBack * 0.5f, projectile.owner);
                        if (flame.WithinBounds(Main.maxProjectiles))
                        {
                            Main.projectile[flame].Calamity().forceRogue = true;
                            Main.projectile[flame].usesLocalNPCImmunity = true;
                            Main.projectile[flame].localNPCHitCooldown = 10;
                        }
                    }
                }
            }
        }

        public override void Kill(int timeLeft)
        {
            for (int i = 0; i <= 10; i++)
            {
                Dust.NewDust(projectile.position + projectile.velocity, projectile.width, projectile.height, (int)CalamityDusts.SulfurousSeaAcid, projectile.oldVelocity.X * 0.5f, projectile.oldVelocity.Y * 0.5f);
            }
        }

        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            projectile.penetrate--;
            if (projectile.penetrate <= 0)
            {
                projectile.Kill();
            }
            else
            {
                if (projectile.velocity.X != oldVelocity.X)
                {
                    projectile.velocity.X = -oldVelocity.X;
                }
                if (projectile.velocity.Y != oldVelocity.Y)
                {
                    projectile.velocity.Y = -oldVelocity.Y;
                }
            }
            return false;
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
        {
            Texture2D tex = Main.projectileTexture[projectile.type];
            spriteBatch.Draw(tex, projectile.Center - Main.screenPosition, null, projectile.GetAlpha(lightColor), projectile.rotation, tex.Size() / 2f, projectile.scale, SpriteEffects.None, 0f);
            return false;
        }

        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
            target.AddBuff(BuffID.CursedInferno, projectile.Calamity().stealthStrike ? 600 : 120);
        }

        public override void OnHitPvp(Player target, int damage, bool crit)
        {
            target.AddBuff(BuffID.CursedInferno, projectile.Calamity().stealthStrike ? 600 : 120);
        }
    }
}
