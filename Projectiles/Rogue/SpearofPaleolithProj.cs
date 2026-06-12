using System.Linq;
using CalamityMod.Buffs.StatDebuffs;
using CalamityMod.Items.Weapons.Rogue;
using CalamityMod.Particles;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Rogue
{
    public class SpearofPaleolithProj : ModProjectile, ILocalizedModType
    {
        public new string LocalizationCategory => "Projectiles.Rogue";
        public override string Texture => "CalamityMod/Items/Weapons/Rogue/SpearofPaleolith";

        public static int Lifetime => 600;

        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.TrailCacheLength[Type] = 24;
            ProjectileID.Sets.TrailingMode[Type] = 2;
        }
        public override void SetDefaults()
        {
            Projectile.width = 12;
            Projectile.height = 12;
            Projectile.friendly = true;
            Projectile.ignoreWater = true;
            Projectile.penetrate = 1;
            Projectile.extraUpdates = 5;
            Projectile.timeLeft = Lifetime * Projectile.MaxUpdates;
            Projectile.DamageType = RogueDamageClass.Instance;
        }
        public float StuckEnemyID
        {
            get { return Projectile.ai[0]; }
            set { Projectile.ai[0] = value; }
        }
        public float StuckEnemyDistance
        {
            get { return Projectile.ai[1]; }
            set { Projectile.ai[1] = value; }
        }
        public float StuckEnemyRotation
        {
            get { return Projectile.ai[2]; }
            set { Projectile.ai[2] = value; }
        }
        public override void AI()
        {
            if (StuckEnemyID > 0)
            {
                Projectile.tileCollide = false;
                if (!Main.npc[(int)StuckEnemyID - 1].active)
                {
                    StuckEnemyID = 0;
                    Projectile.velocity = -Vector2.UnitY.RotatedByRandom(0.25f) * Main.rand.NextFloat(0, 1f);
                    Projectile.tileCollide = true;
                    return;
                }
                Projectile.Center = Main.npc[(int)StuckEnemyID - 1].Center + Vector2.UnitX.RotatedBy(StuckEnemyRotation) * StuckEnemyDistance;
                return;
            }

            Projectile.velocity *= 0.998f;
            if (Projectile.timeLeft < (Lifetime - 25) * Projectile.MaxUpdates)
                Projectile.velocity.Y += 0.02f;

            Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.PiOver4;
            if (Main.rand.NextBool(20))
            {
                Dust.NewDust(Projectile.position + Projectile.velocity, Projectile.width, Projectile.height, DustID.Teleporter, Projectile.velocity.X * 0.5f, Projectile.velocity.Y * 0.5f);
            }

        }

        public override bool PreDraw(ref Color lightColor)
        {
            for (var i = 0; i < ProjectileID.Sets.TrailCacheLength[Type]; i += 3)
            {
                Main.EntitySpriteDraw(TextureAssets.Projectile[Type].Value, Projectile.oldPos[i] + Projectile.Size * 0.5f - Main.screenPosition, null, lightColor * Projectile.Opacity * (i>0 ? (1 - i / (float)ProjectileID.Sets.TrailCacheLength[Type]) * 0.25f : 1), Projectile.rotation, TextureAssets.Projectile[Type].Size() * 0.5f, Projectile.scale, 0);
            }
            return false;
        }

        public override void OnKill(int timeLeft)
        {
            for (int i = 0; i <= 5; i++)
            {
                Dust.NewDust(Projectile.position + Projectile.velocity, Projectile.width, Projectile.height, DustID.Teleporter, Projectile.oldVelocity.X*2.5f, Projectile.oldVelocity.Y*2.5f);
            }
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            if (Projectile.Calamity().stealthStrike)
            {
                Projectile.stopsDealingDamageAfterPenetrateHits = true;
                StuckEnemyID = target.whoAmI + 1;
                StuckEnemyDistance = Projectile.Distance(target.Center);
                StuckEnemyRotation = Projectile.DirectionFrom(target.Center).ToRotation();
                Projectile.timeLeft = Lifetime * Projectile.MaxUpdates;
            }

            bool strongSplit = Main.projectile.Any(x => x.active && x.owner == Projectile.owner && x.type == Projectile.type && x.ai[0] == (target.whoAmI + 1));
            int shardCount = strongSplit ? SpearofPaleolith.ImpaledShardCount : SpearofPaleolith.NormalShardCount;
            for (var i = 0; i < shardCount; i++)
            {
                var p = Projectile.NewProjectileDirect(Projectile.GetSource_FromThis(), Projectile.Center, new Vector2(0,-1).RotatedByRandom(1) * (strongSplit ? 10 : 7.5f) * Main.rand.NextFloat(0.9f,1.1f), ModContent.ProjectileType<FossilShardThrown>(), 0 /*Shard dmg is set in its AI*/, Projectile.knockBack, Projectile.owner, 0f, 0f);
                p.localNPCImmunity[target.whoAmI] = 30;
            }
            if (strongSplit)
            {

                SoundEngine.PlaySound(SoundID.DeerclopsRubbleAttack with { PitchVariance = 0.5f}, Projectile.Center);
                Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center, Vector2.Zero, ModContent.ProjectileType<PaleoShatterExplosion>(), 0, 0, Projectile.owner);
            }
            else
            {
                for (int i = 0; i < shardCount; i++)
                {
                    int sparkLifetime = Main.rand.Next(8, 22);
                    float sparkScale = Main.rand.NextFloat(0.5f, 1f);
                    var sparkColor = Main.rand.NextBool() ? Color.DarkGoldenrod : Color.SandyBrown;

                    if (Main.rand.NextBool(5))
                        sparkScale *= 1.4f;

                    Vector2 sparkVelocity = Vector2.UnitY.RotatedByRandom(1) * MathHelper.Lerp(-7.5f, -15, Main.rand.NextFloat());
                    SparkParticle spark = new SparkParticle(target.Center, sparkVelocity, false, sparkLifetime, sparkScale, sparkColor);
                    GeneralParticleHandler.SpawnParticle(spark);
                }
            }
            SoundEngine.PlaySound(SoundID.DeerclopsStep with { Pitch = 0.5f, PitchVariance = 0.5f, Volume = 1f, }, Projectile.Center);
            SoundEngine.PlaySound(SoundID.DD2_CrystalCartImpact with { Pitch = -0.5f, PitchVariance = 0.5f, Volume = 1f, }, Projectile.Center);

            target.AddBuff(ModContent.BuffType<ArmorCrunch>(), 120);
        }

        public override void OnHitPlayer(Player target, Player.HurtInfo info) => target.AddBuff(ModContent.BuffType<ArmorCrunch>(), 120);
    }
}
