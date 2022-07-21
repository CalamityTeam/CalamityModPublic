using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ModLoader;
using Terraria.ID;
using CalamityMod.Items.Weapons.Summon;
using Terraria.GameContent;
using Terraria.Audio;

namespace CalamityMod.Projectiles.Summon.SmallAresArms
{
    public class ExoskeletonPanel : ModProjectile
    {
        public int ArmIDToDelete = -1;

        public int ArmIDToSpawn = -1;

        public bool FadeOut => Projectile.ai[0] == 1f;

        public Vector2 PlayerOffset;
        
        public static Rectangle MouseRectangle => new((int)Main.MouseScreen.X, (int)Main.MouseScreen.Y, 2, 2);

        public static readonly Color HoverColor = Color.Cyan;

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("UI");
            ProjectileID.Sets.DrawScreenCheckFluff[Type] = 9999999;
        }

        public override void SetDefaults()
        {
            Projectile.width = 40;
            Projectile.height = 40;
            Projectile.penetrate = -1;
            Projectile.ignoreWater = true;
            Projectile.timeLeft = 36000;
            Projectile.Opacity = 0f;
            Projectile.tileCollide = false;
        }

        public override void AI()
        {
            if (PlayerOffset == Vector2.Zero)
                PlayerOffset = Main.MouseWorld - Main.LocalPlayer.Center;

            // Handle fade effects.
            Projectile.Opacity = MathHelper.Clamp(Projectile.Opacity - FadeOut.ToDirectionInt() * 0.0225f, 0f, 1f);
            if (FadeOut && Projectile.Opacity <= 0f)
                Projectile.Kill();

            // Create and destroy arms as necessary.
            if (Main.myPlayer != Projectile.owner)
                return;

            if (ArmIDToSpawn >= 0)
            {
                SoundEngine.PlaySound(SoundID.Zombie66, Projectile.Center);
                int cannon = Projectile.NewProjectile(Projectile.GetSource_FromAI(), Projectile.Center, Vector2.Zero, ArmIDToSpawn, Projectile.damage, 0f, Projectile.owner);
                if (Main.projectile.IndexInRange(cannon))
                    Main.projectile[cannon].originalDamage = Projectile.originalDamage;
            }
            if (ArmIDToDelete >= 0)
            {
                SoundEngine.PlaySound(SoundID.Item74, Projectile.Center);
                for (int i = 0; i < Main.maxProjectiles; i++)
                {
                    if (Main.projectile[i].type != ArmIDToDelete || Main.projectile[i].owner != Projectile.owner || !Main.projectile[i].active)
                        continue;

                    Main.projectile[i].Kill();
                }
            }

            ArmIDToDelete = -1;
            ArmIDToSpawn = -1;
        }

        public override bool PreDraw(ref Color lightColor)
        {
            // This UI only renders for the player that created it.
            if (Main.myPlayer != Projectile.owner)
                return false;

            int plasmaCannonID = ModContent.ProjectileType<ExoskeletonPlasmaCannon>();
            int teslaCannonID = ModContent.ProjectileType<ExoskeletonTeslaCannon>();
            int laserCannonID = ModContent.ProjectileType<ExoskeletonLaserCannon>();
            int gaussNukeID = ModContent.ProjectileType<ExoskeletonGaussNukeCannon>();
            Texture2D panelTexture = ModContent.Request<Texture2D>(Texture).Value;
            Texture2D cancelTexture = ModContent.Request<Texture2D>("CalamityMod/Projectiles/Summon/SmallAresArms/ExoskeletonPanelCancel").Value;
            Texture2D plasmaTexture = ModContent.Request<Texture2D>("CalamityMod/Projectiles/Summon/SmallAresArms/ExoskeletonPanelPlasma").Value;
            Texture2D teslaTexture = ModContent.Request<Texture2D>("CalamityMod/Projectiles/Summon/SmallAresArms/ExoskeletonPanelTesla").Value;
            Texture2D laserTexture = ModContent.Request<Texture2D>("CalamityMod/Projectiles/Summon/SmallAresArms/ExoskeletonPanelLaser").Value;
            Texture2D gaussTexture = ModContent.Request<Texture2D>("CalamityMod/Projectiles/Summon/SmallAresArms/ExoskeletonPanelGauss").Value;
            Vector2 drawPosition = (Main.LocalPlayer.Center + PlayerOffset - Main.screenPosition).Floor();
            if (Main.LocalPlayer.ownedProjectileCounts[plasmaCannonID] >= 1)
                plasmaTexture = cancelTexture;
            if (Main.LocalPlayer.ownedProjectileCounts[teslaCannonID] >= 1)
                teslaTexture = cancelTexture;
            if (Main.LocalPlayer.ownedProjectileCounts[laserCannonID] >= 1)
                laserTexture = cancelTexture;
            if (Main.LocalPlayer.ownedProjectileCounts[gaussNukeID] >= 1)
                gaussTexture = cancelTexture;

            Color plasmaColor = Color.White;
            Color teslaColor = Color.White;
            Color laserColor = Color.White;
            Color gaussColor = Color.White;
            Rectangle plasmaIconArea = Utils.CenteredRectangle(drawPosition + new Vector2(-66f, 10f) * Projectile.scale, plasmaTexture.Size() * Projectile.scale);
            Rectangle teslaIconArea = Utils.CenteredRectangle(drawPosition + new Vector2(-26f, 10f) * Projectile.scale, plasmaTexture.Size() * Projectile.scale);
            Rectangle laserIconArea = Utils.CenteredRectangle(drawPosition + new Vector2(26f, 10f) * Projectile.scale, plasmaTexture.Size() * Projectile.scale);
            Rectangle gaussIconArea = Utils.CenteredRectangle(drawPosition + new Vector2(66f, 10f) * Projectile.scale, plasmaTexture.Size() * Projectile.scale);

            // Handle hover behaviors. If an arm needs to be destroyed or spawned, it will happen on the next frame.
            bool hoveringOverAnySlot = false;
            bool sufficientSlots = Main.LocalPlayer.maxMinions >= AresExoskeleton.MinionSlotsPerCannon;
            if (MouseRectangle.Intersects(plasmaIconArea))
            {
                Main.blockMouse = true;
                hoveringOverAnySlot = true;
                plasmaColor = HoverColor;
                if (Main.mouseLeftRelease && Main.mouseLeft)
                {
                    if (plasmaTexture == cancelTexture)
                        ArmIDToDelete = plasmaCannonID;
                    else if (sufficientSlots)
                        ArmIDToSpawn = plasmaCannonID;
                }
            }
            if (MouseRectangle.Intersects(teslaIconArea))
            {
                Main.blockMouse = true;
                hoveringOverAnySlot = true;
                teslaColor = HoverColor;
                if (Main.mouseLeftRelease && Main.mouseLeft)
                {
                    if (teslaTexture == cancelTexture)
                        ArmIDToDelete = teslaCannonID;
                    else if (sufficientSlots)
                        ArmIDToSpawn = teslaCannonID;
                }
            }
            if (MouseRectangle.Intersects(laserIconArea))
            {
                Main.blockMouse = true;
                hoveringOverAnySlot = true;
                laserColor = HoverColor;
                if (Main.mouseLeftRelease && Main.mouseLeft)
                {
                    if (laserTexture == cancelTexture)
                        ArmIDToDelete = laserCannonID;
                    else if (sufficientSlots)
                        ArmIDToSpawn = laserCannonID;
                }
            }
            if (MouseRectangle.Intersects(gaussIconArea))
            {
                Main.blockMouse = true;
                hoveringOverAnySlot = true;
                gaussColor = HoverColor;
                if (Main.mouseLeftRelease && Main.mouseLeft)
                {
                    if (gaussTexture == cancelTexture)
                        ArmIDToDelete = gaussNukeID;
                    else if (sufficientSlots)
                        ArmIDToSpawn = gaussNukeID;
                }
            }

            plasmaColor = Projectile.GetAlpha(plasmaColor);
            teslaColor = Projectile.GetAlpha(teslaColor);
            laserColor = Projectile.GetAlpha(laserColor);
            gaussColor = Projectile.GetAlpha(gaussColor);

            Main.EntitySpriteDraw(panelTexture, drawPosition, null, Projectile.GetAlpha(Color.White), 0f, panelTexture.Size() * 0.5f, Projectile.scale, 0, 0);
            Main.EntitySpriteDraw(plasmaTexture, plasmaIconArea.Center.ToVector2(), null, plasmaColor, 0f, plasmaTexture.Size() * 0.5f, Projectile.scale, 0, 0);
            Main.EntitySpriteDraw(teslaTexture, teslaIconArea.Center.ToVector2(), null, teslaColor, 0f, teslaTexture.Size() * 0.5f, Projectile.scale, 0, 0);
            Main.EntitySpriteDraw(laserTexture, laserIconArea.Center.ToVector2(), null, laserColor, 0f, laserTexture.Size() * 0.5f, Projectile.scale, 0, 0);
            Main.EntitySpriteDraw(gaussTexture, gaussIconArea.Center.ToVector2(), null, gaussColor, 0f, gaussTexture.Size() * 0.5f, Projectile.scale, 0, 0);

            // Tell the player if they don't have enough summon slots.
            if (hoveringOverAnySlot && !sufficientSlots)
            {
                Utils.DrawBorderStringFourWay(Main.spriteBatch, FontAssets.MouseText.Value, "Insufficient minion slots!", Main.MouseScreen.X + 20f, Main.MouseScreen.Y + 12f, Color.Cyan, Color.Black, new Vector2(0f, 0.5f));
            }

            return false;
        }

        // This is a UI. It should not do damage.
        public override bool? CanDamage() => false;
    }
}
