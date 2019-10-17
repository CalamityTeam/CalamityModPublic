using Microsoft.Xna.Framework;
using Terraria; using CalamityMod.Projectiles; using Terraria.ModLoader; using CalamityMod.Dusts;
using Terraria.ModLoader; using CalamityMod.Dusts; using CalamityMod.Buffs; using CalamityMod.Items; using CalamityMod.NPCs; using CalamityMod.Projectiles; using CalamityMod.Tiles; using CalamityMod.Walls;

namespace CalamityMod.Projectiles
{
    public class MadAlchemistsCocktailGasCloud : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Mad Alchemist's Cocktail Gas Cloud");
        }

        public override void SetDefaults()
        {
            projectile.width = 32;
            projectile.height = 32;
            projectile.friendly = true;
            projectile.magic = true;
            projectile.ignoreWater = true;
            projectile.tileCollide = false;
            projectile.penetrate = -1;
            projectile.usesLocalNPCImmunity = true;
            projectile.localNPCHitCooldown = 30;
        }

        public override void AI()
        {
            projectile.ai[1] += 1f;
            if (projectile.ai[1] > 60f)
            {
                projectile.ai[0] += 10f;
            }
            if (projectile.ai[0] > 255f)
            {
                projectile.Kill();
                projectile.ai[0] = 255f;
            }
            projectile.alpha = (int)(100.0 + (double)projectile.ai[0] * 0.7);
            projectile.rotation += projectile.velocity.X * 0.1f;
            projectile.rotation += (float)projectile.direction * 0.003f;
            projectile.velocity *= 0.96f;
            Rectangle rectangle5 = new Rectangle((int)projectile.position.X, (int)projectile.position.Y, projectile.width, projectile.height);
            int num3;
            for (int num894 = 0; num894 < 1000; num894 = num3 + 1)
            {
                if (num894 != projectile.whoAmI && Main.projectile[num894].active)
                {
                    Rectangle value43 = new Rectangle((int)Main.projectile[num894].position.X, (int)Main.projectile[num894].position.Y, Main.projectile[num894].width, Main.projectile[num894].height);
                    if (rectangle5.Intersects(value43))
                    {
                        Vector2 vector105 = Main.projectile[num894].Center - projectile.Center;
                        if (vector105.X == 0f && vector105.Y == 0f)
                        {
                            if (num894 < projectile.whoAmI)
                            {
                                vector105.X = -1f;
                                vector105.Y = 1f;
                            }
                            else
                            {
                                vector105.X = 1f;
                                vector105.Y = -1f;
                            }
                        }
                        vector105.Normalize();
                        vector105 *= 0.005f;
                        projectile.velocity -= vector105;
                        Projectile projectile2 = Main.projectile[num894];
                        projectile2.velocity += vector105;
                    }
                }
                num3 = num894;
            }
        }
    }
}
