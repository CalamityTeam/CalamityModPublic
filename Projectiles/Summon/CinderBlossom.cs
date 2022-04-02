using CalamityMod.Buffs.Summon;
using CalamityMod.CalPlayer;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
namespace CalamityMod.Projectiles.Summon
{
    public class CinderBlossom : ModProjectile
    {
        public float Time
        {
            get => projectile.ai[0];
            set => projectile.ai[0] = value;
        }
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Cinder Blossom");
            ProjectileID.Sets.MinionSacrificable[projectile.type] = true;
            ProjectileID.Sets.MinionTargettingFeature[projectile.type] = true;
        }

        public override void SetDefaults()
        {
            projectile.width = 36;
            projectile.height = 40;
            projectile.netImportant = true;
            projectile.friendly = true;
            projectile.ignoreWater = true;
            projectile.minionSlots = 1f;
            projectile.timeLeft = 18000;
            projectile.penetrate = -1;
            projectile.tileCollide = false;
            projectile.timeLeft *= 5;
            projectile.minion = true;
        }

        public override void AI()
        {
            bool isCorrectMinion = projectile.type == ModContent.ProjectileType<CinderBlossom>();
            Player player = Main.player[projectile.owner];
            CalamityPlayer modPlayer = player.Calamity();
            player.AddBuff(ModContent.BuffType<CinderBlossomBuff>(), 3600);
            if (isCorrectMinion)
            {
                if (player.dead)
                {
                    modPlayer.cinderBlossom = false;
                }
                if (modPlayer.cinderBlossom)
                {
                    projectile.timeLeft = 2;
                }
            }
            projectile.Center = player.Center + Vector2.UnitY * (player.gfxOffY - 60f);
            if (player.gravDir == -1f)
                projectile.position.Y += 120f;
            projectile.position.X = (int)projectile.position.X;
            projectile.position.Y = (int)projectile.position.Y;
            projectile.position = projectile.position.Floor();
            projectile.rotation += MathHelper.ToRadians(1.5f) * player.direction;

            if (projectile.localAI[0] == 0f)
            {
                Initialize(player);
                projectile.localAI[0] = 1f;
            }
            if (player.MinionDamage() != projectile.Calamity().spawnedPlayerMinionDamageValue)
            {
                int trueDamage = (int)((float)projectile.Calamity().spawnedPlayerMinionProjectileDamageValue /
                    projectile.Calamity().spawnedPlayerMinionDamageValue *
                    player.MinionDamage());
                projectile.damage = trueDamage;
            }
            if (projectile.owner == Main.myPlayer)
            {
                NPC potentialTarget = projectile.Center.MinionHoming(700f, player);
                if (potentialTarget != null)
                {
                    if (Time++ % 35f == 34f && Collision.CanHit(projectile.position, projectile.width, projectile.height, potentialTarget.position, potentialTarget.width, potentialTarget.height))
                    {
                        Vector2 velocity = projectile.SafeDirectionTo(potentialTarget.Center) * Main.rand.NextFloat(10f, 18f);
                        Projectile.NewProjectile(projectile.Center, velocity, ModContent.ProjectileType<Cinder>(), projectile.damage, projectile.knockBack, projectile.owner);
                    }
                }
            }
            Lighting.AddLight(projectile.Center, Color.Orange.ToVector3());
        }
        public void Initialize(Player player)
        {
            projectile.Calamity().spawnedPlayerMinionDamageValue = player.MinionDamage();
            projectile.Calamity().spawnedPlayerMinionProjectileDamageValue = projectile.damage;
            for (int i = 0; i < 36; i++)
            {
                Dust dust = Dust.NewDustPerfect(projectile.Center, DustID.Fire);
                dust.noGravity = true;
                dust.velocity = Vector2.UnitY.RotatedByRandom(MathHelper.TwoPi) * Main.rand.NextFloat(2f, 6f);
            }
        }

        public override bool CanDamage() => false;
    }
}
