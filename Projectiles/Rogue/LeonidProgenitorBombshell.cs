using CalamityMod.Buffs.DamageOverTime;
using CalamityMod.Dusts;
using CalamityMod.Projectiles.Ranged;
using CalamityMod.Items.Weapons.Rogue;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Rogue
{
    public class LeonidProgenitorBombshell : ModProjectile
    {
        public override string Texture => "CalamityMod/Items/Weapons/Rogue/LeonidProgenitor";

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Leonid Bombshell");
        }

        public override void SetDefaults()
        {
            projectile.width = 32;
            projectile.height = 32;
            projectile.friendly = true;
            projectile.ignoreWater = true;
            projectile.penetrate = 1;
            projectile.timeLeft = 180;
            projectile.Calamity().rogue = true;
        }

        public override void AI()
        {
            int randomDust = Utils.SelectRandom(Main.rand, new int[]
            {
                ModContent.DustType<AstralOrange>(),
                ModContent.DustType<AstralBlue>()
            });
            int astral = Dust.NewDust(new Vector2(projectile.position.X, projectile.position.Y), projectile.width, projectile.height, randomDust, 0f, 0f, 100, CalamityUtils.ColorSwap(LeonidProgenitor.blueColor, LeonidProgenitor.purpleColor, 1f), 0.8f);
            Main.dust[astral].noGravity = true;
            Main.dust[astral].velocity *= 0f;

            projectile.ai[0] += 1f;
            if (projectile.ai[0] > 10f)
            {
                projectile.ai[0] = 10f;
                if (projectile.velocity.Y == 0f && projectile.velocity.X != 0f)
                {
                    projectile.velocity.X = projectile.velocity.X * 0.97f;
                    if (projectile.velocity.X > -0.01f && projectile.velocity.X < 0.01f)
                    {
                        projectile.velocity.X = 0f;
                        projectile.netUpdate = true;
                    }
                }
                projectile.velocity.Y += 0.2f;
            }
            projectile.rotation += projectile.velocity.X * 0.1f;
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
        {
            Texture2D tex = Main.projectileTexture[projectile.type];
            spriteBatch.Draw(tex, projectile.Center - Main.screenPosition, null, lightColor, projectile.rotation, tex.Size() / 2, projectile.scale, SpriteEffects.None, 0f);
            return false;
        }

        public override void PostDraw(SpriteBatch spriteBatch, Color lightColor)
        {
            Texture2D tex = ModContent.GetTexture("CalamityMod/Items/Weapons/Rogue/LeonidProgenitorGlow");
            spriteBatch.Draw(tex, projectile.Center - Main.screenPosition, null, Color.White, projectile.rotation, tex.Size() / 2, projectile.scale, SpriteEffects.None, 0f);
        }

        public override void Kill(int timeLeft)
        {
            Main.PlaySound(SoundID.Item62, projectile.position);
            if (Main.myPlayer == projectile.owner)
            {
                int flash = Projectile.NewProjectile(projectile.Center, Vector2.Zero, ModContent.ProjectileType<Flash>(), projectile.damage, 0f, projectile.owner, 0f, 1f);
                if (flash.WithinBounds(Main.maxProjectiles))
                {
                    Main.projectile[flash].Calamity().forceRogue = true;
                    Main.projectile[flash].usesLocalNPCImmunity = true;
                    Main.projectile[flash].localNPCHitCooldown = 10;
                }

                Vector2 pos = new Vector2(projectile.Center.X + projectile.width * 0.5f + Main.rand.Next(-201, 201), Main.screenPosition.Y - 600f - Main.rand.Next(50));
                Vector2 velocity = (projectile.Center - pos) / 40f;
                int dmg = projectile.damage / 2;
                Projectile.NewProjectile(pos, velocity, ModContent.ProjectileType<LeonidCometBig>(), dmg, projectile.knockBack, projectile.owner, 0f, 0.5f + (float)Main.rand.NextDouble() * 0.3f);
            }
        }

        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
            target.AddBuff(ModContent.BuffType<AstralInfectionDebuff>(), 240);
            StealthStrikeEffect();
        }

        public override void OnHitPvp(Player target, int damage, bool crit)
        {
            target.AddBuff(ModContent.BuffType<AstralInfectionDebuff>(), 240);
            StealthStrikeEffect();
        }

        private void StealthStrikeEffect()
        {
            if (!projectile.Calamity().stealthStrike || Main.myPlayer != projectile.owner)
                return;

            Vector2 spinningpoint = new Vector2(0f, 6f);
            float num1 = MathHelper.ToRadians(45f);
            int cometAmt = 5;
            float num3 = -(num1 * 2f) / (cometAmt - 1f);
            for (int projIndex = 0; projIndex < cometAmt; ++projIndex)
            {
                int index2 = Projectile.NewProjectile(projectile.Center, spinningpoint.RotatedBy((double)num1 + (double)num3 * (double)projIndex, new Vector2()), ModContent.ProjectileType<LeonidCometSmall>(), projectile.damage, projectile.knockBack, projectile.owner, 0f, -1f);
                Projectile proj = Main.projectile[index2];
                for (int index3 = 0; index3 < projectile.localNPCImmunity.Length; ++index3)
                {
                    proj.localNPCImmunity[index3] = projectile.localNPCImmunity[index3];
                }
            }
        }
    }
}
