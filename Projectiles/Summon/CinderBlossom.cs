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
            get => Projectile.ai[0];
            set => Projectile.ai[0] = value;
        }
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Cinder Blossom");
            ProjectileID.Sets.MinionSacrificable[Projectile.type] = true;
            ProjectileID.Sets.MinionTargettingFeature[Projectile.type] = true;
        }

        public override void SetDefaults()
        {
            Projectile.width = 36;
            Projectile.height = 40;
            Projectile.netImportant = true;
            Projectile.friendly = true;
            Projectile.ignoreWater = true;
            Projectile.minionSlots = 1f;
            Projectile.timeLeft = 18000;
            Projectile.penetrate = -1;
            Projectile.tileCollide = false;
            Projectile.timeLeft *= 5;
            Projectile.minion = true;
        }

        public override void AI()
        {
            bool isCorrectMinion = Projectile.type == ModContent.ProjectileType<CinderBlossom>();
            Player player = Main.player[Projectile.owner];
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
                    Projectile.timeLeft = 2;
                }
            }
            Projectile.Center = player.Center + Vector2.UnitY * (player.gfxOffY - 60f);
            if (player.gravDir == -1f)
                Projectile.position.Y += 120f;
            Projectile.position.X = (int)Projectile.position.X;
            Projectile.position.Y = (int)Projectile.position.Y;
            Projectile.position = Projectile.position.Floor();
            Projectile.rotation += MathHelper.ToRadians(1.5f) * player.direction;

            if (Projectile.localAI[0] == 0f)
            {
                Initialize(player);
                Projectile.localAI[0] = 1f;
            }
            if (player.MinionDamage() != Projectile.Calamity().spawnedPlayerMinionDamageValue)
            {
                int trueDamage = (int)((float)Projectile.Calamity().spawnedPlayerMinionProjectileDamageValue /
                    Projectile.Calamity().spawnedPlayerMinionDamageValue *
                    player.MinionDamage());
                Projectile.damage = trueDamage;
            }
            if (Projectile.owner == Main.myPlayer)
            {
                NPC potentialTarget = Projectile.Center.MinionHoming(700f, player);
                if (potentialTarget != null)
                {
                    if (Time++ % 35f == 34f && Collision.CanHit(Projectile.position, Projectile.width, Projectile.height, potentialTarget.position, potentialTarget.width, potentialTarget.height))
                    {
                        Vector2 velocity = Projectile.SafeDirectionTo(potentialTarget.Center) * Main.rand.NextFloat(10f, 18f);
                        Projectile.NewProjectile(Projectile.Center, velocity, ModContent.ProjectileType<Cinder>(), Projectile.damage, Projectile.knockBack, Projectile.owner);
                    }
                }
            }
            Lighting.AddLight(Projectile.Center, Color.Orange.ToVector3());
        }
        public void Initialize(Player player)
        {
            Projectile.Calamity().spawnedPlayerMinionDamageValue = player.MinionDamage();
            Projectile.Calamity().spawnedPlayerMinionProjectileDamageValue = Projectile.damage;
            for (int i = 0; i < 36; i++)
            {
                Dust dust = Dust.NewDustPerfect(Projectile.Center, DustID.Fire);
                dust.noGravity = true;
                dust.velocity = Vector2.UnitY.RotatedByRandom(MathHelper.TwoPi) * Main.rand.NextFloat(2f, 6f);
            }
        }

        public override bool CanDamage() => false;
    }
}
