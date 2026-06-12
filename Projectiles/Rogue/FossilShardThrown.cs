using CalamityMod.Buffs.StatDebuffs;
using CalamityMod.Items.Weapons.Rogue;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
namespace CalamityMod.Projectiles.Rogue
{
    public class FossilShardThrown : ModProjectile, ILocalizedModType
    {
        public new string LocalizationCategory => "Projectiles.Rogue";
        public override string Texture => "CalamityMod/Projectiles/Ranged/FossilShard";

        public override void SetDefaults()
        {
            Projectile.width = 10;
            Projectile.height = 10;
            Projectile.friendly = true;
            Projectile.penetrate = 4;
            Projectile.aiStyle = ProjAIStyleID.GroundProjectile;
            Projectile.timeLeft = 600;
            AIType = ProjectileID.SpikyBall;
            Projectile.DamageType = RogueDamageClass.Instance;
            Projectile.localNPCHitCooldown = 15;
            Projectile.usesLocalNPCImmunity = true;
        }

        public override bool PreAI()
        {
            if (Projectile.originalDamage == 0)
            {
                Projectile.originalDamage = SpearofPaleolith.ShardBaseDamage;
                Projectile.ContinuouslyUpdateDamageStats = true;
            }

            Projectile.velocity.X *= 0.975f;
            var tile = Main.tile[(Projectile.Bottom + Vector2.UnitY).ToTileCoordinates()];
            if (tile.HasUnactuatedTile && tile.TileType == TileID.ConveyorBeltLeft)
            {
                Projectile.velocity.X = 2;
            }
            else if (tile.HasUnactuatedTile && tile.TileType == TileID.ConveyorBeltRight)
            {
                Projectile.velocity.X = -2;
            }
                return true;
        }
        public override bool TileCollideStyle(ref int width, ref int height, ref bool fallThrough, ref Vector2 hitboxCenterFrac)
        {
            fallThrough = false;
            return true;
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone) => target.AddBuff(ModContent.BuffType<ArmorCrunch>(), 60);

        public override void OnHitPlayer(Player target, Player.HurtInfo info) => target.AddBuff(ModContent.BuffType<ArmorCrunch>(), 60);

        public override void OnKill(int timeLeft)
        {
            Dust.NewDust(Projectile.position + Projectile.velocity, Projectile.width, Projectile.height, DustID.Sand, Projectile.oldVelocity.X * 0.5f, Projectile.oldVelocity.Y * 0.5f);
        }
    }
}
