using CalamityMod.CalPlayer;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Summon
{
    public class Urchin : ModProjectile
    {
        public int dust = 3;

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Urchin");
            ProjectileID.Sets.MinionSacrificable[projectile.type] = true;
        }

        public override void SetDefaults()
        {
            projectile.width = 30;
            projectile.height = 30;
            projectile.netImportant = true;
            projectile.friendly = true;
            projectile.ignoreWater = true;
            projectile.minion = true;
            projectile.minionSlots = 0f;
            projectile.timeLeft = 18000;
            projectile.penetrate = -1;
            projectile.tileCollide = false;
            projectile.timeLeft *= 5;
        }

        public override void AI()
        {
            Player player = Main.player[projectile.owner];
            CalamityPlayer modPlayer = player.Calamity();
            if (!modPlayer.urchin)
            {
                projectile.active = false;
                return;
            }
            bool correctMinion = projectile.type == ModContent.ProjectileType<Urchin>();
            if (correctMinion)
            {
                if (player.dead)
                {
                    modPlayer.vUrchin = false;
                }
                if (modPlayer.vUrchin)
                {
                    projectile.timeLeft = 2;
                }
            }
            dust--;
            if (dust >= 0)
            {
                projectile.Calamity().spawnedPlayerMinionDamageValue = player.MinionDamage();
                projectile.Calamity().spawnedPlayerMinionProjectileDamageValue = projectile.damage;
                int dustAmt = 50;
                for (int d = 0; d < dustAmt; d++)
                {
                    int index = Dust.NewDust(new Vector2(projectile.position.X, projectile.position.Y + 16f), projectile.width, projectile.height - 16, 179, 0f, 0f, 0, default, 1f);
                    Main.dust[index].velocity *= 2f;
                    Main.dust[index].scale *= 1.15f;
                }
            }
            if (player.MinionDamage() != projectile.Calamity().spawnedPlayerMinionDamageValue)
            {
                int damage2 = (int)((float)projectile.Calamity().spawnedPlayerMinionProjectileDamageValue /
                    projectile.Calamity().spawnedPlayerMinionDamageValue *
                    player.MinionDamage());
                projectile.damage = damage2;
            }
            float scalar = (float)Main.mouseTextColor / 200f - 0.35f;
            scalar *= 0.2f;
            projectile.scale = scalar + 0.95f;
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
            if (projectile.owner == Main.myPlayer)
            {
                if (projectile.ai[0] != 0f)
                {
                    projectile.ai[0] -= 1f;
                    return;
                }
                bool foundTarget = false;
                float maxDist = 300f;
                for (int i = 0; i < Main.maxNPCs; i++)
                {
                    NPC npc = Main.npc[i];
                    if (npc.CanBeChasedBy(projectile, false))
                    {
                        if (Vector2.Distance(projectile.Center, npc.Center) < maxDist && Collision.CanHit(projectile.position, projectile.width, projectile.height, npc.position, npc.width, npc.height))
                        {
                            foundTarget = true;
                            break;
                        }
                    }
                }
                if (foundTarget)
                {
                    int projAmt = Main.rand.Next(3, 7);
                    for (int u = 0; u < projAmt; u++)
                    {
                        Vector2 source = new Vector2(projectile.Center.X - 4f, projectile.Center.Y);
                        Vector2 velocity = CalamityUtils.RandomVelocity(100f, 70f, 100f);
                        int spore = Projectile.NewProjectile(source, velocity, ModContent.ProjectileType<UrchinSpike>(), projectile.damage, 1f, projectile.owner, 0f, 0f);
                        Main.projectile[spore].minion = true;
                        Main.projectile[spore].minionSlots = 0f;
                    }
                    Main.PlaySound(SoundID.Item42, projectile.position);
                    projectile.ai[0] = 60f;
                }
            }
        }

        public override bool CanDamage()
        {
            return false;
        }
    }
}
