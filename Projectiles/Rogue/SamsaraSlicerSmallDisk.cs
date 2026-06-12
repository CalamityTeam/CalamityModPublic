using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Rogue
{
    public class SamsaraSlicerSmallDisk : ModProjectile, ILocalizedModType
    {
        public new string LocalizationCategory => "Projectiles.Rogue";
        public Projectile Parent = null;
        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.TrailCacheLength[Type] = 10;
            ProjectileID.Sets.TrailingMode[Type] = 1;
        }

        public override void SetDefaults()
        {
            Projectile.width = 40;
            Projectile.height = 40;
            Projectile.ignoreWater = true;
            Projectile.friendly = true;
            Projectile.tileCollide = false;
            Projectile.penetrate = -1;
            Projectile.aiStyle = -1;
            Projectile.DamageType = RogueDamageClass.Instance;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 5;
            Projectile.ArmorPenetration = 20;
            Projectile.Calamity().CannotProc = true;
        }

        public override void AI()
        {
            if (Parent != null)
            {
                if (!Parent.active) Parent = null;
            }

            Player player = Main.player[Projectile.owner];

            if (Projectile.Calamity().stealthStrike)
            {
                Projectile.localNPCHitCooldown = 3;
                Projectile.usesLocalNPCImmunity = true;
            }

            bool returning = false;

            Projectile.ai[1]++;

            Projectile.rotation += MathHelper.ToRadians(12f);

            float Vel = 15;
            if (Projectile.Calamity().stealthStrike) Vel = 20;

            float length = 10;
            if (Projectile.Calamity().stealthStrike) length = 15;

            if (Projectile.ai[1] > length)
            {
                Vel += (Projectile.ai[1] - length);
                if (Parent != null)
                {
                    if (Parent.active)
                    {
                        Projectile.velocity = Vector2.Lerp(Projectile.velocity, Projectile.DirectionTo(Parent.Center) * Vel, Projectile.Calamity().stealthStrike ? 0.25f : 0.35f);

                        if ((Projectile.Center + Projectile.velocity).Distance(Parent.Center) < Projectile.velocity.Length())
                        {
                            Projectile.Kill();
                        }
                    }
                    else returning = true;
                }
                else returning = true;
            }

            if (Parent != null && Parent.active)
            {
                Projectile.velocity = Projectile.velocity.RotatedBy(MathHelper.ToRadians(8f));
            }

            if (Parent != null && !returning)
            {
                Projectile.position += Parent.velocity;
            }

            if (Parent == null)
            {
                Projectile.timeLeft = 5;
                Projectile.position += player.velocity;
                Projectile.velocity = Projectile.DirectionTo(player.Center) * 4f;
                Projectile.scale *= 0.9f;
                if (Projectile.Distance(player.Center) < Projectile.velocity.Length())
                {
                    Projectile.Kill();
                }
            }
        }

        public override void OnSpawn(IEntitySource source)
        {
            Parent = Main.projectile[(int)Projectile.ai[0]];
        }

        public override bool PreDraw(ref Color lightColor)
        {
            for (int i = 1; i < Projectile.oldPos.Length; i++)
            {
                Vector2 oldP1 = Projectile.oldPos[i];
                Vector2 oldP2 = Projectile.oldPos[i - 1];

                Projectile.oldPos[i] = oldP2 + (oldP2.DirectionTo(oldP1) * MathHelper.Min(oldP2.Distance(oldP1), 5));
            }

            CalamityUtils.DrawAfterimagesCentered(Projectile, ProjectileID.Sets.TrailingMode[Type], new Color(0.2f, 1f, 0f, 0f), 2);
            return false;
        }
    }
}
