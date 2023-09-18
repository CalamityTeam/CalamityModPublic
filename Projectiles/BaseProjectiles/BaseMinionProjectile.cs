using CalamityMod.CalPlayer;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.BaseProjectiles
{
    /// <summary>
    /// An abstract class that contains the necessary code for a minion to work.<br/>
    /// Contains useful properties for minions, such as IFrames, Enemy Distance Detection, and sets the Owner and Target.
    /// Also has a <see cref="DoAnimation()"/> hook, with basic animation code that can be overriden.
    /// </summary>
    public abstract class BaseMinionProjectile : ModProjectile, ILocalizedModType
    {
        public new string LocalizationCategory => "Projectiles.Summon";

        #region Properties

        /// <summary>
        /// Input here the correspondent <see cref="ModContent.ProjectileType{T}"/> of the minion.
        /// </summary>
        public abstract int AssociatedProjectileTypeID { get; }

        /// <summary>
        /// Input here the correspondent <see cref="ModContent.BuffType{T}"/> of the minion.
        /// </summary>
        public abstract int AssociatedBuffTypeID { get; }

        /// <summary>
        /// Input the reference of the minion bool from <see cref="CalamityPlayer" /> with <see cref="ModdedOwner"/>.<br/>
        /// Example for Elemental Axe:
        /// <code>public override ref bool AssociatedMinionBool => ref ModdedOwner.eAxe;</code>
        /// </summary>
        public abstract ref bool AssociatedMinionBool { get; }

        /// <summary>
        /// The amount of minion slots this summon consumes.<br/>
        /// Defaults to 1f.
        /// </summary>
        public virtual float MinionSlots => 1f;

        /// <summary>
        /// The distance in which the minion can detect an enemy, in pixels.<br/>
        /// <see cref="ProjectileID.Sets.DrawScreenCheckFluff"/> is set to this value.<br/>
        /// Defaults to 1200f (75 tiles).
        /// </summary>
        public virtual float EnemyDistanceDetection => 1200f;

        /// <summary>
        /// The amount of local I-Frames this minion has.<br/>
        /// Multiplied by <see cref="Projectile.MaxUpdates"/> so changing <see cref="Projectile.extraUpdates"/> won't affect this.<br/>
        /// Defaults to 10.
        /// </summary>
        public int IFrames { get => Projectile.localNPCHitCooldown / Projectile.MaxUpdates; set => Projectile.localNPCHitCooldown = value * Projectile.MaxUpdates; }

        /// <summary>
        /// If <see langword="true"/>, makes the minion only be able to detect and attack enemies through tiles only when there are any bosses alive.<br/>
        /// Defaults to <see langword="false"/>.
        /// </summary>
        public virtual bool PreHardmodeMinionTileVision => false;

        /// <summary>
        /// If <see langword="false"/>, makes the minion not target certain NPCs (Like Abyss' or Sunken Sea's enemies) until they're hit.<br/>
        /// Defaults to <see langword="true"/>.
        /// </summary>
        public virtual bool PreventTargettingUntilTargetHit => true;

        /// <summary>
        /// The frames that it takes to go to the next frame of animation.<br/>
        /// Defaults to 5.
        /// </summary>
        public int FramesUntilNextAnimationFrame { get; set; } = 5;

        /// <summary>
        /// Set here <see cref="ProjectileID.Sets.TrailingMode"/>.<br/>
        /// Defaults to 2.
        /// </summary>
        public int TrailingMode { get; set; } = 2;

        /// <summary>
        /// Set here <see cref="ProjectileID.Sets.TrailCacheLength"/>.<br/>
        /// Defaults to 5.<br/>
        /// </summary>
        public int TrailCacheLength { get; set; } = 5;

        public Player Owner => Main.player[Projectile.owner];
        public CalamityPlayer ModdedOwner => Owner.Calamity();
        public NPC Target { get; set; }

        #endregion

        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.MinionTargettingFeature[Type] = true;
            ProjectileID.Sets.TrailingMode[Type] = TrailingMode;
            ProjectileID.Sets.TrailCacheLength[Type] = TrailCacheLength;
            ProjectileID.Sets.DrawScreenCheckFluff[Type] = (int)EnemyDistanceDetection;
        }

        public override void SetDefaults()
        {
            Projectile.DamageType = DamageClass.Summon;
            Projectile.minionSlots = MinionSlots;
            Projectile.penetrate = -1;

            Projectile.friendly = true;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
            Projectile.minion = true;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.netImportant = true;
        }

        public override bool PreAI()
        {
            Projectile.Calamity().overridesMinionDamagePrevention = !PreventTargettingUntilTargetHit;
            return true;
        }

        public override void AI()
        {
            CheckMinionExistence();
            DoAnimation();
            ChooseTarget();
            MinionAI();
        }

        #region AI Methods

        /// <summary>
        /// Where all of the actual AI of the minion will be placed.
        /// </summary>
        public abstract void MinionAI();

        /// <summary>
        /// The universal way all minions check if they can still exist.
        /// </summary>
        public virtual void CheckMinionExistence()
        {
            Owner.AddBuff(AssociatedBuffTypeID, 2);
            if (Type != AssociatedProjectileTypeID)
                return;

            if (Owner.dead)
                AssociatedMinionBool = false;
            if (AssociatedMinionBool)
                Projectile.timeLeft = 2;
        }

        /// <summary>
        /// Basic animation code that the majority of minions use.<br/>
        /// It will only run if the minion is set to have more than 1 frame of animation.<br/>
        /// If something more complex is needed, override it. Or leave it blank if not needed at all.
        /// </summary>
        public virtual void DoAnimation()
        {
            if (Main.projFrames[Type] <= 1)
                return;

            Projectile.frameCounter++;
            if (Projectile.frameCounter >= FramesUntilNextAnimationFrame * Projectile.MaxUpdates)
            {
                Projectile.frameCounter = 0;
                Projectile.frame = (Projectile.frame + 1) % Main.projFrames[Type];
            }
        }

        /// <summary>
        /// Where the null property <see cref="Target"/> is set to a valid target, a non-null value.
        /// </summary>
        public virtual void ChooseTarget() => Target = Owner.Center.MinionHoming(EnemyDistanceDetection, Owner, !PreHardmodeMinionTileVision || CalamityPlayer.areThereAnyDamnBosses);

        #endregion
    }
}
