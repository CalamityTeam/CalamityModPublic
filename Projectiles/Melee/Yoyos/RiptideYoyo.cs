using System.IO;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Melee.Yoyos
{
    public class RiptideYoyo : ModProjectile
    {
        private const int MaxUpdates = 2;

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Riptide");
            ProjectileID.Sets.YoyosLifeTimeMultiplier[Projectile.type] = 7.5f;
            ProjectileID.Sets.YoyosMaximumRange[Projectile.type] = 220f;
            ProjectileID.Sets.YoyosTopSpeed[Projectile.type] = 12.5f;
        }

        public override void SetDefaults()
        {
            Projectile.aiStyle = ProjAIStyleID.Yoyo;
            Projectile.width = 16;
            Projectile.height = 16;
            Projectile.scale = 1f;
            Projectile.friendly = true;
            Projectile.DamageType = DamageClass.MeleeNoSpeed;
            Projectile.alpha = 150;
            Projectile.penetrate = -1;
            Projectile.MaxUpdates = MaxUpdates;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 10 * MaxUpdates;
        }

        public override void SendExtraAI(BinaryWriter writer)
        {
            writer.Write(Projectile.localAI[1]);
        }

        public override void ReceiveExtraAI(BinaryReader reader)
        {
            Projectile.localAI[1] = reader.ReadSingle();
        }

        public override void AI()
        {
            if ((Projectile.position - Main.player[Projectile.owner].position).Length() > 3200f) // 200 blocks
                Projectile.Kill();

            Projectile.localAI[1]++;
            if (Projectile.localAI[1] % (15f * MaxUpdates) == 0f)
            {
                float xVelocity = 0f;
                float yVelocity = -10f;
                float xIncrement = 1.2f;
                float yIncrement = 0.2f;
                switch (Projectile.localAI[1] / (15f * MaxUpdates))
                {
                    case 1:
                        break;

                    case 2:
                        xVelocity = 5f;
                        yVelocity = -5f;
                        xIncrement = 0.7f;
                        yIncrement = 0.7f;
                        break;

                    case 3:
                        xVelocity = 10f;
                        yVelocity = 0f;
                        xIncrement = 0.2f;
                        yIncrement = 1.2f;
                        break;

                    case 4:
                        xVelocity = 5f;
                        yVelocity = 5f;
                        xIncrement = -0.7f;
                        yIncrement = 0.7f;
                        break;

                    case 5:
                        xVelocity = 0f;
                        yVelocity = 10f;
                        xIncrement = -1.2f;
                        yIncrement = -0.2f;
                        break;

                    case 6:
                        xVelocity = -5f;
                        yVelocity = 5f;
                        xIncrement = -0.7f;
                        yIncrement = -0.7f;
                        break;

                    case 7:
                        xVelocity = -10f;
                        yVelocity = 0f;
                        xIncrement = -0.2f;
                        yIncrement = -1.2f;
                        break;

                    case 8:
                        Projectile.localAI[1] = 0f;
                        xVelocity = -10f;
                        yVelocity = -10f;
                        xIncrement = 0.7f;
                        yIncrement = -0.7f;
                        break;
                }

                SoundEngine.PlaySound(SoundID.Item21, Projectile.position);
                Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center, new Vector2(xVelocity, yVelocity), ModContent.ProjectileType<AquaStream>(), Projectile.damage, 0f, Projectile.owner, xIncrement, yIncrement);
            }
        }
    }
}
