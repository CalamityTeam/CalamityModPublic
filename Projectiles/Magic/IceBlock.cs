using Terraria;
using Terraria.ModLoader;
using Terraria.ID;
using Microsoft.Xna.Framework;
using Terraria.Audio;

namespace CalamityMod.Projectiles.Magic
{
    public class IceBlock : ModProjectile, ILocalizedModType
    {
        public new string LocalizationCategory => "Projectiles.Magic";
        public override void SetDefaults()
        {
            Projectile.width = 58;
            Projectile.height = 58;
            Projectile.friendly = true;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
            Projectile.DamageType = DamageClass.Magic;
            Projectile.timeLeft = 140;
            Projectile.alpha = 255;
            Projectile.penetrate = -1;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 60;
            Projectile.coldDamage = true;
        }

        public override void AI()
        {
            if (Projectile.alpha > 20)
            {
                Projectile.alpha -= 12;
            }
            if(Projectile.alpha < 20)
            {
                Projectile.alpha = 20;
            }
            for (int i = 0; i < Main.maxProjectiles; i++)
            {
                Projectile proj = Main.projectile[i];
                if (proj.type == ModContent.ProjectileType<IceBarrageMain>() && proj.owner == Main.myPlayer)
                {
                    Vector2 pos1 = new Vector2(proj.Center.X, proj.Center.Y - (proj.height * 0.5f) - 44f);
                    Vector2 pos2 = new Vector2(proj.Center.X + (proj.width * 0.5f) + 48f, proj.Center.Y);
                    Vector2 pos3 = new Vector2(proj.Center.X, proj.Center.Y + (proj.height * 0.5f) + 44f);
                    Vector2 pos4 = new Vector2(proj.Center.X - (proj.width * 0.5f) - 49f, proj.Center.Y);
                    switch (Projectile.ai[0])
                    {
                        case 0: Projectile.Center = pos1;
                                break;
                        case 1: Projectile.Center = pos2;
                                break;
                        case 2: Projectile.Center = pos3;
                                break;
                        case 3: Projectile.Center = pos4;
                                break;
                        default: break;
                    }
                }
            }
            switch (Projectile.ai[0])
            {
                case 1: Projectile.rotation = (MathHelper.Pi * 0.5f);
                        break;
                case 2: Projectile.rotation = MathHelper.Pi;
                        break;
                case 3: Projectile.rotation = (MathHelper.Pi * 1.5f);
                        break;
                default: break;
            }
        }

        public override void OnKill(int timeLeft)
        {
            for (int i = 0; i < 40; i++)
            {
                int dustType = Main.rand.NextBool() ? 68 : 67;
                if (Main.rand.NextBool(4))
                {
                    dustType = 80;
                }
                Vector2 direction = new Vector2(Main.rand.NextFloat(-4f, 4f), Main.rand.NextFloat(-4f, 4f));
                int dust = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, dustType, direction.X, direction.Y, 50, default, 1.5f);
                Main.dust[dust].noGravity = true;
            }
            SoundEngine.PlaySound(SoundID.NPCHit5, Projectile.Center);
            for (int i = 0; i< 8; i++)
            {
                Vector2 projdir = new Vector2(Main.rand.NextFloat(-10f, 10f), Main.rand.NextFloat(-10f, 10f));
                Vector2 projpos = Projectile.Center + new Vector2(Main.rand.NextFloat(-50f, 50f), Main.rand.NextFloat(-50f, 50f));
                Projectile.NewProjectile(Projectile.GetSource_FromThis(), projpos, projdir, ModContent.ProjectileType<IceBlockIcicle>(), (int)(Projectile.damage * 0.2f), 4f, Projectile.owner, Main.rand.Next(0, 2), 0f);
            }
        }
    }
}
