using CalamityMod.Buffs.Summon;
using CalamityMod.CalPlayer;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
namespace CalamityMod.Projectiles.Summon
{
    public class WitherBlossom : ModProjectile
    {
        public Player Owner => Main.player[projectile.owner];
        public ref float OffsetAngle => ref projectile.ai[0];
        public ref float Time => ref projectile.ai[1];
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Wither Blossom");
            ProjectileID.Sets.MinionSacrificable[projectile.type] = true;
            ProjectileID.Sets.MinionTargettingFeature[projectile.type] = true;
        }

        public override void SetDefaults()
        {
            projectile.width = projectile.height = 30;
            projectile.netImportant = true;
            projectile.friendly = true;
            projectile.ignoreWater = true;
            projectile.minionSlots = 0.5f;
            projectile.timeLeft = 90000;
            projectile.penetrate = -1;
            projectile.tileCollide = false;
            projectile.minion = true;
        }

        public override void AI()
        {
            bool isCorrectMinion = projectile.type == ModContent.ProjectileType<WitherBlossom>();
            Player player = Main.player[projectile.owner];
            CalamityPlayer modPlayer = player.Calamity();
            player.AddBuff(ModContent.BuffType<WitherBlossomsBuff>(), 3600);
            if (isCorrectMinion)
            {
                if (player.dead)
                    modPlayer.witherBlossom = false;
                if (modPlayer.witherBlossom)
                    projectile.timeLeft = 2;
            }

            SetProjectileDamage();

            Time++;
            NPC potentialTarget = projectile.Center.MinionHoming(950f, Owner);
            if (Time % 50 == 49 && Main.myPlayer == projectile.owner && potentialTarget != null)
            {
                Vector2 shootVelocity = projectile.SafeDirectionTo(potentialTarget.Center).RotatedByRandom(0.25f) * 8f;
                Projectile.NewProjectile(projectile.Center, shootVelocity, ModContent.ProjectileType<WitherBolt>(), projectile.damage, projectile.knockBack, projectile.owner);
            }
            projectile.Center = player.Center + OffsetAngle.ToRotationVector2() * 150f;
            projectile.rotation += MathHelper.ToRadians(5f);
            OffsetAngle += MathHelper.ToRadians(4f);
        }

        public void SetProjectileDamage()
        {
            if (projectile.localAI[0] == 0f)
            {
                projectile.Calamity().spawnedPlayerMinionDamageValue = Owner.MinionDamage();
                projectile.Calamity().spawnedPlayerMinionProjectileDamageValue = projectile.damage;
                for (int i = 0; i < 36; i++)
                {
                    Dust dust = Dust.NewDustPerfect(projectile.Center, 179);
                    dust.noGravity = true;
                    dust.velocity = Main.rand.NextVector2Unit() * Main.rand.NextFloat(2f, 7f);
                }
                projectile.localAI[0] += 1f;
            }
            if (Owner.MinionDamage() != projectile.Calamity().spawnedPlayerMinionDamageValue)
            {
                int trueDamage = (int)(projectile.Calamity().spawnedPlayerMinionProjectileDamageValue /
                    projectile.Calamity().spawnedPlayerMinionDamageValue *
                    Owner.MinionDamage());
                projectile.damage = trueDamage;
            }
        }

        public override bool CanDamage() => false;
    }
}
