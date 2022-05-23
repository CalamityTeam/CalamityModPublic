using CalamityMod.CalPlayer;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
namespace CalamityMod.Projectiles.Rogue
{
    public class ThiefsDimeProj : ModProjectile
    {
        private double rotation = 0;

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Thief's Dime");
            Main.projFrames[Projectile.type] = 5;
            ProjectileID.Sets.MinionSacrificable[Projectile.type] = true;
        }

        public override void SetDefaults()
        {
            Projectile.width = 20;
            Projectile.height = 20;
            Projectile.ignoreWater = true;
            Projectile.minionSlots = 0f;
            Projectile.DamageType = RogueDamageClass.Instance;
            Projectile.timeLeft = 18000;
            Projectile.tileCollide = false;
            Projectile.friendly = true;
            Projectile.timeLeft *= 5;
            Projectile.penetrate = -1;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 10;
        }

        public override void AI()
        {
            Projectile.frameCounter++;
            if (Projectile.frameCounter > 6)
            {
                Projectile.frame++;
                Projectile.frameCounter = 0;
            }
            if (Projectile.frame > 4)
            {
                Projectile.frame = 0;
            }
            bool flag64 = Projectile.type == ModContent.ProjectileType<ThiefsDimeProj>();
            Player player = Main.player[Projectile.owner];
            CalamityPlayer modPlayer = player.Calamity();
            if (!modPlayer.thiefsDime)
            {
                Projectile.active = false;
                return;
            }
            if (flag64)
            {
                if (player.dead)
                {
                    modPlayer.tDime = false;
                }
                if (modPlayer.tDime)
                {
                    Projectile.timeLeft = 2;
                }
            }
            Lighting.AddLight(Projectile.Center, (255 - Projectile.alpha) * 0.15f / 255f, (255 - Projectile.alpha) * 0.15f / 255f, (255 - Projectile.alpha) * 0.01f / 255f);
            Vector2 vector = player.Center - Projectile.Center;
            Projectile.rotation = vector.ToRotation() - 1.57f;
            Projectile.Center = player.Center + new Vector2(80, 0).RotatedBy(rotation);
            rotation += 0.03;
            if (rotation >= 360)
            {
                rotation = 0;
            }
            Projectile.velocity.X = (vector.X > 0f) ? -0.000001f : 0f;
        }

        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
            target.AddBuff(BuffID.Midas, 360);
            Player player = Main.player[Projectile.owner];
            if ((target.damage > 5 || target.boss) && player.whoAmI == Main.myPlayer && !target.SpawnedFromStatue)
            {
                if (Main.rand.NextBool(5))
                {
                    Item.NewItem(Projectile.GetSource_FromThis(), (int)target.position.X, (int)target.position.Y, target.width, target.height, ItemID.SilverCoin, Main.rand.Next(10, 21));
                }
                if (Main.rand.NextBool(10))
                {
                    Item.NewItem(Projectile.GetSource_FromThis(), (int)target.position.X, (int)target.position.Y, target.width, target.height, ItemID.GoldCoin);
                }
            }
            else
            {
                return;
            }
        }
    }
}
