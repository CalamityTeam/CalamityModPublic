using CalamityMod.CalPlayer;
using CalamityMod.Dusts;
using CalamityMod.Buffs.Summon;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Summon
{
    public class AstralProbeSummon : ModProjectile
    {
        private double rotation = 0;
        private double rotationVariation = 0;

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Astral Probe");
            ProjectileID.Sets.MinionSacrificable[projectile.type] = true;
            ProjectileID.Sets.MinionTargettingFeature[projectile.type] = true;
        }

        public override void SetDefaults()
        {
            projectile.width = 22;
            projectile.height = 22;
            projectile.ignoreWater = true;
            projectile.minionSlots = 1f;
            projectile.timeLeft = 18000;
            projectile.tileCollide = false;
            projectile.friendly = true;
            projectile.timeLeft *= 5;
            projectile.penetrate = -1;
            projectile.minion = true;
        }

        public override void AI()
        {
            Player player = Main.player[projectile.owner];
            CalamityPlayer modPlayer = player.Calamity();
            if (projectile.localAI[0] == 0f)
            {
                projectile.Calamity().spawnedPlayerMinionDamageValue = player.MinionDamage();
                projectile.Calamity().spawnedPlayerMinionProjectileDamageValue = projectile.damage;

                rotationVariation = Main.rand.NextDouble() * 0.015;

                int dustAmt = 36;
                for (int dustIndex = 0; dustIndex < dustAmt; dustIndex++)
                {
                    Vector2 dustSource = Vector2.Normalize(projectile.velocity) * new Vector2((float)projectile.width / 2f, (float)projectile.height) * 0.75f;
                    dustSource = dustSource.RotatedBy((double)((float)(dustIndex - (dustAmt / 2 - 1)) * MathHelper.TwoPi / (float)dustAmt), default) + projectile.Center;
                    Vector2 dustVel = dustSource - projectile.Center;
                    int astral = Dust.NewDust(dustSource + dustVel, 0, 0, (Main.rand.NextBool(2) ? ModContent.DustType<AstralOrange>() : ModContent.DustType<AstralBlue>()), dustVel.X * 1.75f, dustVel.Y * 1.75f, 100, default, 1.1f);
                    Main.dust[astral].noGravity = true;
                    Main.dust[astral].velocity = dustVel;
                }
                projectile.localAI[0] += 1f;
            }
            if (player.MinionDamage() != projectile.Calamity().spawnedPlayerMinionDamageValue)
            {
                int damage2 = (int)((float)projectile.Calamity().spawnedPlayerMinionProjectileDamageValue /
                    projectile.Calamity().spawnedPlayerMinionDamageValue * player.MinionDamage());
                projectile.damage = damage2;
            }
            bool correctMinion = projectile.type == ModContent.ProjectileType<AstralProbeSummon>();
            player.AddBuff(ModContent.BuffType<AstralProbeBuff>(), 3600);
            if (correctMinion)
            {
                if (player.dead)
                {
                    modPlayer.aProbe = false;
                }
                if (modPlayer.aProbe)
                {
                    projectile.timeLeft = 2;
                }
            }
            NPC target = projectile.Center.MinionHoming(1000f, player, true, true);
            Vector2 vector = player.Center - projectile.Center;
            if (target != null)
            {
                projectile.spriteDirection = projectile.direction = ((target.Center.X - projectile.Center.X) > 0).ToDirectionInt();
                projectile.rotation = projectile.rotation.AngleTowards(projectile.AngleTo(target.Center) + (projectile.spriteDirection == 1 ? 0f : MathHelper.Pi), 0.1f);
            }
            else
            {
                projectile.spriteDirection = projectile.direction = (projectile.velocity.X > 0).ToDirectionInt();
                projectile.rotation = projectile.rotation.AngleLerp(vector.ToRotation() - (projectile.spriteDirection == 1 ? 0f : MathHelper.Pi * projectile.direction), 0.1f);
            }
            projectile.Center = player.Center + new Vector2(80, 0).RotatedBy(rotation);
            rotation += 0.03 + rotationVariation;
            if (rotation >= 360)
            {
                rotation = 0;
            }
            projectile.velocity.X = (vector.X > 0f) ? -0.000001f : 0f;

            if (projectile.ai[1] > 0f)
            {
                projectile.ai[1] += (float)Main.rand.Next(1, 3);
            }
            if (projectile.ai[1] > 80f)
            {
                projectile.ai[1] = 0f;
                projectile.netUpdate = true;
            }
            float speedMult = 6f;
            int projType = ModContent.ProjectileType<AstralProbeRound>();
            if (target != null && projectile.ai[1] == 0f)
            {
                Main.PlaySound(SoundID.Item, (int)projectile.position.X, (int)projectile.position.Y, 12, 0.5f, 0f);
                projectile.ai[1] += 1f;
                if (Main.myPlayer == projectile.owner)
                {
                    Vector2 velocity = target.Center - projectile.Center;
                    velocity.Normalize();
                    velocity *= speedMult;
                    Projectile.NewProjectile(projectile.Center, velocity, projType, projectile.damage, 0f, projectile.owner, target.whoAmI, 0f);
                    projectile.netUpdate = true;
                }
            }
        }

        public override bool CanDamage() => false;
    }
}
