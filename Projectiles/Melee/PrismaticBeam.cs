using CalamityMod.Buffs.DamageOverTime;
using CalamityMod.Projectiles.BaseProjectiles;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.Enums;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Melee
{
    public class PrismaticBeam : BaseLaserbeamProjectile
    {
        public override string Texture => "CalamityMod/Projectiles/Magic/YharimsCrystalBeam";

        public static readonly Color[] Colors = new Color[]
        {
            new Color(255, 0, 0, 50), //Red
            new Color(255, 128, 0, 50), //Orange
            new Color(255, 255, 0, 50), //Yellow
            new Color(128, 255, 0, 50), //Lime
            new Color(0, 255, 0, 50), //Green
            new Color(0, 255, 128, 50), //Turquoise
            new Color(0, 255, 255, 50), //Cyan
            new Color(0, 128, 255, 50), //Light Blue
            new Color(0, 0, 255, 50), //Blue
            new Color(128, 0, 255, 50), //Purple
            new Color(255, 0, 255, 50), //Fuschia
            new Color(255, 0, 128, 50) //Hot Pink
        };
        public static readonly Color[] ColorSet = new Color[]
        {
            new Color(255, 0, 0, 50), //Red
            new Color(255, 255, 0, 50), //Yellow
            new Color(0, 255, 0, 50), //Green
            new Color(0, 255, 255, 50), //Cyan
            new Color(0, 0, 255, 50), //Blue
            new Color(255, 0, 255, 50), //Fuschia
        };

        public bool PlayedSound = false;

        public const int ChargeupTime = 100;

        public Player Owner => Main.player[projectile.owner];
        public override Color LaserOverlayColor => CalamityUtils.MulticolorLerp(Main.GlobalTime / ColorSet.Length % 1f, ColorSet);
        public override Color LightCastColor => LaserOverlayColor;
        public override float Lifetime => 300f;
        public override float MaxScale => 1.5f;
        public override float MaxLaserLength => 2200f;
        public override Texture2D LaserBeginTexture => ModContent.GetTexture("CalamityMod/ExtraTextures/Lasers/UltimaRayStart");
        public override Texture2D LaserMiddleTexture => ModContent.GetTexture("CalamityMod/ExtraTextures/Lasers/UltimaRayMid");
        public override Texture2D LaserEndTexture => ModContent.GetTexture("CalamityMod/ExtraTextures/Lasers/UltimaRayEnd");

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Prismatic Beam");
        }

        public override void SetDefaults()
        {
            projectile.width = 6;
            projectile.height = 6;
            projectile.friendly = true;
            projectile.ranged = true;
            projectile.melee = true;
            projectile.scale = 1.5f;
            projectile.penetrate = -1;
            projectile.tileCollide = false;
            projectile.hide = true;
            projectile.timeLeft = 300;
            projectile.usesLocalNPCImmunity = true;
            projectile.localNPCHitCooldown = 10;
            projectile.Calamity().PierceResistHarshness = 0.06f;
            projectile.Calamity().PierceResistCap = 0.4f;
        }

        public override void DetermineScale()
        {
            projectile.scale = Time < ChargeupTime ? 0f : Utils.InverseLerp(0f, 40f, projectile.timeLeft, true) * MaxScale;
        }

        public override float DetermineLaserLength()
        {
            return DetermineLaserLength_CollideWithTiles(5);
        }

        public override bool PreAI()
        {
            // Multiplayer support here, only run this code if the client running it is the owner of the projectile
            if (projectile.owner == Main.myPlayer)
            {
                projectile.velocity = (Main.MouseWorld - Owner.Center).SafeNormalize(Vector2.UnitX * Owner.direction);
                projectile.direction = Main.MouseWorld.X > Owner.Center.X ? 1 : -1;
                projectile.netUpdate = true;
            }

            int dir = projectile.direction;
            projectile.rotation = projectile.velocity.ToRotation() + MathHelper.PiOver2;
            projectile.Center = Owner.Center + projectile.velocity * 80f;
            Owner.ChangeDir(dir);
            Owner.heldProj = projectile.whoAmI;
            Owner.itemTime = 2;
            Owner.itemAnimation = 2;
            Owner.itemRotation = CalamityUtils.WrapAngle90Degrees(projectile.rotation - MathHelper.PiOver2);

            if (!Owner.channel)
            {
                projectile.Kill();
                return false;
            }

            if (Time < ChargeupTime)
            {
                // Crate charge-up dust.
                int dustCount = (int)(Time / 20f);
                Vector2 spawnPos = projectile.Center;
                for (int k = 0; k < dustCount + 1; k++)
                {
                    Dust dust = Dust.NewDustDirect(spawnPos, 1, 1, 267, projectile.velocity.X / 2f, projectile.velocity.Y / 2f);
                    dust.position += Main.rand.NextVector2Square(-10f, 10f);
                    dust.velocity = Main.rand.NextVector2Unit() * (10f - dustCount * 2f) / 10f;
                    dust.color = Main.rand.Next(Colors);
                    dust.scale = Main.rand.NextFloat(0.5f, 1f);
                    dust.noGravity = true;
                }
                DetermineScale();
                Time++;
                return false;
            }

            // Play a cool sound when fully charged.
            if (!PlayedSound)
            {
                Main.PlaySound(SoundID.Item68, projectile.position);
                PlayedSound = true;
            }
            return true;
        }

        public override bool ShouldUpdatePosition() => false;

        // Update CutTiles so the laser will cut tiles (like grass).
        public override void CutTiles()
        {
            DelegateMethods.tilecut_0 = TileCuttingContext.AttackProjectile;
            Vector2 unit = projectile.velocity;
            Utils.PlotTileLine(projectile.Center, projectile.Center + unit * LaserLength, projectile.width + 16, DelegateMethods.CutTiles);
        }

        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
            target.AddBuff(ModContent.BuffType<Nightwither>(), 150);
            target.AddBuff(BuffID.Daybreak, 150);
        }

        public override void OnHitPvp(Player target, int damage, bool crit)
        {
            target.AddBuff(ModContent.BuffType<Nightwither>(), 150);
            target.AddBuff(BuffID.Daybreak, 150);
        }
    }
}
