using Microsoft.Xna.Framework;
using Terraria;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;
namespace CalamityMod.Projectiles.Rogue
{
    public class DuststormInABottleProj : ModProjectile, ILocalizedModType
    {
        public new string LocalizationCategory => "Projectiles.Rogue";
        public override string Texture => "CalamityMod/Items/Weapons/Rogue/DuststormInABottle";

        int lifetime => AIState == 1 ? 300 : 150;
        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.TrailCacheLength[Type] = 3;
            ProjectileID.Sets.TrailingMode[Type] = 0;
        }

        public override void SetDefaults()
        {
            Projectile.width = 24;
            Projectile.height = 24;
            Projectile.friendly = true;
            Projectile.tileCollide = true;
            Projectile.penetrate = -1;
            Projectile.MaxUpdates = 2;
            Projectile.timeLeft = lifetime;
            Projectile.DamageType = RogueDamageClass.Instance;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = -1;
        }
        ref float Timer => ref Projectile.ai[0];
        ref float TimerMax => ref Projectile.ai[1];
        ref float AIState => ref Projectile.ai[2];

        int gravTimer = 0;
        bool Stealth => Projectile.Calamity().stealthStrike;

        public override void AI()
        {
            Timer++;
            gravTimer++;

            Projectile.rotation += 0.175f * Projectile.direction * Projectile.velocity.Length();
            if (gravTimer > 20)
            {
                Projectile.velocity.Y += 0.22f;
            }

            if (AIState == 1)
            {
                if (Timer % 10 == 0 && Main.myPlayer == Projectile.owner)
                {
                    Projectile.NewProjectileDirect(Projectile.GetSource_FromThis(), Projectile.Center, Main.rand.NextVector2Circular(5, 5), ModContent.ProjectileType<DuststormCloud>(), (int)(Projectile.damage * 0.5f), Projectile.knockBack, Projectile.owner);
                }
            }
            if (Timer > TimerMax && AIState == 0)
            {
                if (!Stealth)
                {
                    if (Main.myPlayer == Projectile.owner)
                        Projectile.NewProjectileDirect(Projectile.GetSource_FromThis(), Projectile.Center, Vector2.Zero, ModContent.ProjectileType<DuststormCloudExplosion>(), Projectile.damage, Projectile.knockBack, Projectile.owner);
                    Projectile.Kill();
                    return;
                }
                AIState = 1;
                Timer = 0;
                Projectile.timeLeft = lifetime;
            }
        }
        public override bool PreDraw(ref Color lightColor)
        {
            Main.EntitySpriteDraw(TextureAssets.Projectile[Type].Value, Projectile.Center - Main.screenPosition, null, lightColor, Projectile.rotation, TextureAssets.Projectile[Type].Size() * 0.5f, Projectile.scale, 0);
            return false;
        }

        public override bool? CanHitNPC(NPC target)
        {
            return Projectile.velocity.Length() > 1f;
        }
        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            if (!Stealth)
            {
                Projectile.NewProjectileDirect(Projectile.GetSource_FromThis(), Projectile.Center, Vector2.Zero, ModContent.ProjectileType<DuststormCloudExplosion>(), Projectile.damage, Projectile.knockBack, Projectile.owner);
                Projectile.Kill();
                return;
            }
            if (Projectile.numHits == 0)
            {
                Projectile.NewProjectileDirect(Projectile.GetSource_FromThis(), Projectile.Center, Vector2.Zero, ModContent.ProjectileType<DuststormCloudExplosion>(), Projectile.damage, Projectile.knockBack, Projectile.owner);
            }
            return;
        }

        public override bool TileCollideStyle(ref int width, ref int height, ref bool fallThrough, ref Vector2 hitboxCenterFrac)
        {
            if (gravTimer > 25)
                fallThrough = false;
            return base.TileCollideStyle(ref width, ref height, ref fallThrough, ref hitboxCenterFrac);
        }
        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            Projectile.velocity *= 0.95f;
            return false;
        }
    }
}
