using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
namespace CalamityMod.Projectiles.Rogue
{
    public class Crystalline2 : ModProjectile, ILocalizedModType
    {
        public new string LocalizationCategory => "Projectiles.Rogue";
        public override string Texture => "CalamityMod/Items/Weapons/Rogue/Crystalline";

        public override void SetDefaults()
        {
            Projectile.width = 10;
            Projectile.height = 10;
            Projectile.friendly = true;
            Projectile.penetrate = 1;
            //Projectile.aiStyle = ProjAIStyleID.StickProjectile;
            Projectile.timeLeft = 30;
            //AIType = ProjectileID.BoneJavelin;
            Projectile.DamageType = RogueDamageClass.Instance;
        }

        public override void AI()
        {
            Projectile.localAI[0]++;
            Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.ToRadians(45f);
            if (Projectile.localAI[0] == 10f && Projectile.ai[1] == 1f)
            {
                int numProj = 2;
                float rotation = MathHelper.ToRadians(50);
                if (Projectile.owner == Main.myPlayer)
                {
                    for (int i = 0; i < numProj + 1; i++)
                    {
                        Vector2 perturbedSpeed = new Vector2(Projectile.velocity.X * 0.8f, Projectile.velocity.Y * 0.8f).RotatedBy(MathHelper.Lerp(-rotation, rotation, i / (numProj - 1)));
                        int proj = Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center, perturbedSpeed, ModContent.ProjectileType<Crystalline2>(), (int)(Projectile.damage * 0.5f), Projectile.knockBack, Projectile.owner, 0f, 2f);
                        Main.projectile[proj].timeLeft = 20;
                    }
                }
            }
        }

        public override bool PreDraw(ref Color lightColor)
        {
            if (Projectile.timeLeft == (Projectile.ai[1] == 2f ? 20 : 30))
                return false;
            Texture2D tex = ModContent.Request<Texture2D>(Texture).Value;
            Main.EntitySpriteDraw(tex, Projectile.Center - Main.screenPosition, null, Projectile.GetAlpha(lightColor), Projectile.rotation, tex.Size() / 2f, Projectile.scale, SpriteEffects.None, 0);
            return false;
        }

        public override void OnKill(int timeLeft)
        {
            for (int k = 0; k < 5; k++)
            {
                Dust.NewDust(Projectile.position + Projectile.velocity, Projectile.width, Projectile.height, 154, Projectile.oldVelocity.X * 0.5f, Projectile.oldVelocity.Y * 0.5f);
            }
            if (Projectile.ai[1] >= 1f)
            {
                for (int i = 0; i < 3; i++)
                {
                    Vector2 projspeed = new Vector2(Main.rand.NextFloat(-8f, 8f), Main.rand.NextFloat(-8f, 8f));
                    int shard = Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center, projspeed, ProjectileID.CrystalShard, (int)(Projectile.damage * 0.4f), 2f, Projectile.owner);
                    if (shard.WithinBounds(Main.maxProjectiles))
                        Main.projectile[shard].DamageType = RogueDamageClass.Instance;
                }
            }
        }
    }
}
