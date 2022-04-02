using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
namespace CalamityMod.Projectiles.Summon
{
    public class Hive : ModProjectile
    {
        public override string Texture => "CalamityMod/NPCs/Astral/Hive";

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Hive");
            Main.projFrames[projectile.type] = 6;
            ProjectileID.Sets.MinionTargettingFeature[projectile.type] = true;
        }

        public override void SetDefaults()
        {
            projectile.width = 38;
            projectile.height = 62;
            projectile.ignoreWater = true;
            projectile.tileCollide = true;
            projectile.sentry = true;
            projectile.timeLeft = Projectile.SentryLifeTime;
            projectile.penetrate = -1;
        }

        public override void AI()
        {
            Player player = Main.player[projectile.owner];
            if (projectile.localAI[0] == 0f)
            {
                projectile.Calamity().spawnedPlayerMinionDamageValue = player.MinionDamage();
                projectile.Calamity().spawnedPlayerMinionProjectileDamageValue = projectile.damage;
                projectile.localAI[0] += 1f;
            }
            if (player.MinionDamage() != projectile.Calamity().spawnedPlayerMinionDamageValue)
            {
                int damage2 = (int)((float)projectile.Calamity().spawnedPlayerMinionProjectileDamageValue /
                    projectile.Calamity().spawnedPlayerMinionDamageValue *
                    player.MinionDamage());
                projectile.damage = damage2;
            }

            projectile.frameCounter++;
            if (projectile.frameCounter > 5)
            {
                projectile.frame++;
                projectile.frameCounter = 0;
            }
            if (projectile.frame > 5)
            {
                projectile.frame = 0;
            }
            projectile.velocity.Y += 0.5f;

            if (projectile.velocity.Y > 10f)
            {
                projectile.velocity.Y = 10f;
            }

            projectile.StickToTiles(false, false);

            int target = 0;
            float num633 = 800f;
            Vector2 vector46 = projectile.position;
            bool flag25 = false;
            if (Main.player[projectile.owner].HasMinionAttackTargetNPC)
            {
                NPC npc = Main.npc[Main.player[projectile.owner].MinionAttackTargetNPC];
                if (npc.CanBeChasedBy(projectile, false))
                {
                    float num646 = Vector2.Distance(npc.Center, projectile.Center);
                    if (!flag25 && num646 < num633)
                    {
                        vector46 = npc.Center;
                        flag25 = true;
                        target = npc.whoAmI;
                    }
                }
            }
            if (!flag25)
            {
                for (int num645 = 0; num645 < Main.maxNPCs; num645++)
                {
                    NPC nPC2 = Main.npc[num645];
                    if (nPC2.CanBeChasedBy(projectile, false))
                    {
                        float num646 = Vector2.Distance(nPC2.Center, projectile.Center);
                        if (!flag25 && num646 < num633)
                        {
                            num633 = num646;
                            vector46 = nPC2.Center;
                            flag25 = true;
                            target = num645;
                        }
                    }
                }
            }
            if (projectile.owner == Main.myPlayer && flag25)
            {
                if (projectile.ai[0] != 0f)
                {
                    projectile.ai[0] -= 1f;
                    return;
                }
                projectile.ai[1] += 1f;
                if ((projectile.ai[1] % 15f) == 0f)
                {
                    float velocityX = Main.rand.NextFloat(-0.4f, 0.4f);
                    float velocityY = Main.rand.NextFloat(-0.3f, -0.5f);
                    Projectile.NewProjectile(projectile.Center.X, projectile.Center.Y, velocityX, velocityY, ModContent.ProjectileType<Hiveling>(), projectile.damage, projectile.knockBack, projectile.owner, (float)target, 0f);
                }
            }
        }

        public override bool OnTileCollide(Vector2 oldVelocity) => false;

        public override bool CanDamage() => false;
    }
}
