using CalamityMod.Buffs.Summon;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
namespace CalamityMod.Projectiles.DraedonsArsenal
{
    public class MountedScannerSummon : ModProjectile
    {
        public float AngularOffsetRelativeToPlayer
        {
            get => projectile.ai[0];
            set => projectile.ai[0] = value;
        }
        public float Time
        {
            get => projectile.ai[1];
            set => projectile.ai[1] = value;
        }
        public const float OffsetDistanceFromPlayer = 60f;
        public const float LaserFireRate = 40f;
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Mounted Scanner");
            ProjectileID.Sets.MinionSacrificable[projectile.type] = true;
            ProjectileID.Sets.MinionTargettingFeature[projectile.type] = true;
            ProjectileID.Sets.NeedsUUID[projectile.type] = true;
        }

        public override void SetDefaults()
        {
            projectile.width = 24;
            projectile.height = 18;
            projectile.netImportant = true;
            projectile.friendly = true;
            projectile.minionSlots = 1;
            projectile.timeLeft = 18000;
            projectile.penetrate = -1;
            projectile.timeLeft *= 5;
            projectile.minion = true;
            projectile.tileCollide = false;
        }
        public override void AI()
        {
            Player player = Main.player[projectile.owner];
            projectile.Center = player.Center + AngularOffsetRelativeToPlayer.ToRotationVector2() * OffsetDistanceFromPlayer;
            if (projectile.localAI[0] == 0f)
            {
                projectile.Calamity().spawnedPlayerMinionDamageValue = player.MinionDamage();
                projectile.Calamity().spawnedPlayerMinionProjectileDamageValue = projectile.damage;
            }
            AdjustDamage(player);
            GrantBuffs(player);

            NPC potentialTarget = projectile.Center.MinionHoming(960f, player);
            if (potentialTarget is null)
            {
                AdjustVisualValues_Idle(player);
                projectile.localAI[0] = 0f;
            }
            else
            {
                AttackTarget(potentialTarget);
                projectile.localAI[0] = 1f;
            }
            Time++;
        }
        // While this projectile cannot attack, the projectiles it shoots derive from the damage.
        public void AdjustDamage(Player player)
        {
            if (player.MinionDamage() != projectile.Calamity().spawnedPlayerMinionDamageValue)
            {
                int trueDamage = (int)(projectile.Calamity().spawnedPlayerMinionProjectileDamageValue /
                    projectile.Calamity().spawnedPlayerMinionDamageValue *
                    player.MinionDamage());
                projectile.damage = trueDamage;
            }
        }
        public void GrantBuffs(Player player)
        {
            bool isCorrectProjectile = projectile.type == ModContent.ProjectileType<MountedScannerSummon>();
            player.AddBuff(ModContent.BuffType<MountedScannerBuff>(), 3600);
            if (isCorrectProjectile)
            {
                if (player.dead)
                    player.Calamity().mountedScanner = false;
                if (player.Calamity().mountedScanner)
                    projectile.timeLeft = 2;
            }
        }
        public void AdjustVisualValues_Idle(Player player)
        {
            if (player.velocity.Length() > 1.5f)
            {
                projectile.spriteDirection = (player.velocity.X > 0).ToDirectionInt();
                projectile.rotation = player.velocity.ToRotation() + (projectile.spriteDirection == -1).ToInt() * MathHelper.Pi;
            }
            else
            {
                projectile.spriteDirection = 1;
                projectile.rotation = projectile.rotation.AngleLerp(MathHelper.PiOver2, 0.075f);
            }
        }
        public void AttackTarget(NPC target)
        {
            projectile.spriteDirection = 1;
            projectile.rotation = projectile.AngleTo(target.Center);
            if (!Collision.CanHitLine(projectile.position, projectile.width, projectile.height, target.position, target.width, target.height))
                return;

            if (Time % 80f == 79f)
            {
                if (projectile.owner == Main.myPlayer)
                {
                    Projectile.NewProjectile(projectile.Center,
                                         projectile.SafeDirectionTo(target.Center, Vector2.UnitY),
                                         ModContent.ProjectileType<MountedScannerLaser>(),
                                         projectile.damage,
                                         projectile.knockBack,
                                         projectile.owner,
                                         0f,
                                         projectile.whoAmI);
                }
                Main.PlaySound(mod.GetLegacySoundSlot(SoundType.Item, "Sounds/Item/LaserCannon"), projectile.Center);
            }
        }
        public override bool CanDamage() => false;
    }
}
