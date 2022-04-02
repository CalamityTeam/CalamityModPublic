using CalamityMod.Buffs.Summon;
using CalamityMod.CalPlayer;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
namespace CalamityMod.Projectiles.Summon
{
    public class MagicHat : ModProjectile
    {
        public const float Range = 1500.0001f;

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Magic Hat");
            ProjectileID.Sets.MinionSacrificable[projectile.type] = true;
            ProjectileID.Sets.MinionTargettingFeature[projectile.type] = true;
            ProjectileID.Sets.TrailCacheLength[projectile.type] = 8;
            ProjectileID.Sets.TrailingMode[projectile.type] = 1;
        }

        public override void SetDefaults()
        {
            projectile.width = 30;
            projectile.height = 30;
            projectile.netImportant = true;
            projectile.friendly = true;
            projectile.ignoreWater = true;
            projectile.minionSlots = 5f;
            projectile.timeLeft = 18000;
            projectile.penetrate = -1;
            projectile.tileCollide = false;
            projectile.timeLeft *= 5;
            projectile.minion = true;
        }

        public override void AI()
        {
            Player player = Main.player[projectile.owner];
            CalamityPlayer modPlayer = player.Calamity();
            CalamityGlobalProjectile modProj = projectile.Calamity();

            //set up minion buffs and bools
            bool hatExists = projectile.type == ModContent.ProjectileType<MagicHat>();
            player.AddBuff(ModContent.BuffType<MagicHatBuff>(), 3600);
            if (hatExists)
            {
                if (player.dead)
                {
                    modPlayer.magicHat = false;
                }
                if (modPlayer.magicHat)
                {
                    projectile.timeLeft = 2;
                }
            }

            //projectile movement
            projectile.Center = player.Center + Vector2.UnitY * (player.gfxOffY - 60f);
            if (player.gravDir == -1f)
            {
                projectile.position.Y += 120f;
                projectile.rotation = MathHelper.Pi;
            }
            else
            {
                projectile.rotation = 0f;
            }
            projectile.position.X = (int)projectile.position.X;
            projectile.position.Y = (int)projectile.position.Y;

            //Change the summons scale size a little bit to make it pulse in and out
            float scalar = (float)Main.mouseTextColor / 200f - 0.35f;
            scalar *= 0.2f;
            projectile.scale = scalar + 0.95f;

            //on summon dust and flexible damage
            if (projectile.localAI[0] == 0f)
            {
                int dustAmt = 50;
                for (int dustIndex = 0; dustIndex < dustAmt; dustIndex++)
                {
                    int dustEffects = Dust.NewDust(new Vector2(projectile.position.X, projectile.position.Y + 16f), projectile.width, projectile.height - 16, 234, 0f, 0f, 0, default, 1f);
                    Main.dust[dustEffects].velocity *= 2f;
                    Main.dust[dustEffects].scale *= 1.15f;
                }
                modProj.spawnedPlayerMinionDamageValue = player.MinionDamage();
                modProj.spawnedPlayerMinionProjectileDamageValue = projectile.damage;
                projectile.localAI[0] += 1f;
            }
            if (player.MinionDamage() != modProj.spawnedPlayerMinionDamageValue)
            {
                int damage2 = (int)((float)modProj.spawnedPlayerMinionProjectileDamageValue /
                    modProj.spawnedPlayerMinionDamageValue * player.MinionDamage());
                projectile.damage = damage2;
            }

            //finding an enemy, then shooting projectiles if it's detected
            if (projectile.owner == Main.myPlayer)
            {
                float detectionRange = Range;
                bool enemyDetected = false;

                for (int i = 0; i < Main.maxNPCs; i++)
                {
                    NPC npc = Main.npc[i];
                    if (npc.CanBeChasedBy(projectile, false))
                    {
                        float extraDistance = (npc.width / 2) + (npc.height / 2);

                        if (Vector2.Distance(npc.Center, projectile.Center) < (detectionRange + extraDistance))
                        {
                            enemyDetected = true;
                            break;
                        }
                    }
                }
                if (enemyDetected)
                {
                    projectile.ai[1] += 1f;
                    if (projectile.ai[1] % 5f == 0f)
                    {
                        int amount = Main.rand.Next(1, 2);
                        for (int i = 0; i < amount; i++)
                        {
                            int projType = Utils.SelectRandom(Main.rand, new int[]
                            {
                                ModContent.ProjectileType<MagicUmbrella>(),
                                ModContent.ProjectileType<MagicRifle>(),
                                ModContent.ProjectileType<MagicHammer>(),
                                ModContent.ProjectileType<MagicAxe>(),
                                ModContent.ProjectileType<MagicBird>()
                            });
                            float velocityX = Main.rand.NextFloat(-10f, 10f);
                            float velocityY = Main.rand.NextFloat(-15f, -8f);
                            Projectile.NewProjectile(projectile.oldPosition.X + (float)(projectile.width / 2), projectile.oldPosition.Y + (float)(projectile.height / 2), velocityX, velocityY, projType, projectile.damage, projectile.knockBack, projectile.owner, 0f, 0f);
                        }
                    }
                }
            }
        }

        //glowmask effect
        public override Color? GetAlpha(Color lightColor) => new Color(200, 200, 200, 200);

        //no contact damage
        public override bool CanDamage() => false;
    }
}
