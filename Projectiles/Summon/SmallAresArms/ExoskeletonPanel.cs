using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ModLoader;
using Terraria.ID;
using CalamityMod.Items.Weapons.Summon;
using Terraria.GameContent;
using Terraria.Audio;
using System.Linq;
using System;
using System.Collections.Generic;

namespace CalamityMod.Projectiles.Summon.SmallAresArms
{
    public class ExoskeletonPanel : ModProjectile, ILocalizedModType
    {
        public new string LocalizationCategory => "Projectiles.Summon";
        public enum IconType
        {
            Inactive,
            Plasma,
            Tesla,
            Laser,
            Gauss
        }

        public class IconState
        {
            public int PanelFlashTimer;

            public int MousePressFrameCountdown;

            public bool BeingHoveredOver;

            public bool PlacedInPanel;

            public IconType CurrentState;

            public Texture2D IconTexture
            {
                get
                {
                    Texture2D plasmaTexture = ModContent.Request<Texture2D>("CalamityMod/Projectiles/Summon/SmallAresArms/ExoskeletonPanelPlasma").Value;
                    Texture2D teslaTexture = ModContent.Request<Texture2D>("CalamityMod/Projectiles/Summon/SmallAresArms/ExoskeletonPanelTesla").Value;
                    Texture2D laserTexture = ModContent.Request<Texture2D>("CalamityMod/Projectiles/Summon/SmallAresArms/ExoskeletonPanelLaser").Value;
                    Texture2D gaussTexture = ModContent.Request<Texture2D>("CalamityMod/Projectiles/Summon/SmallAresArms/ExoskeletonPanelGauss").Value;
                    return CurrentState switch
                    {
                        IconType.Plasma => plasmaTexture,
                        IconType.Tesla => teslaTexture,
                        IconType.Laser => laserTexture,
                        IconType.Gauss => gaussTexture,
                        _ => null,
                    };
                }
            }

            public Rectangle Frame
            {
                get
                {
                    int frame = 0;
                    if (BeingHoveredOver)
                        frame = PlacedInPanel ? 3 : 1;
                    if (MousePressFrameCountdown >= 1)
                        frame = 2;
                    if (PanelFlashTimer >= 1)
                        frame = (int)Math.Round(MathHelper.Lerp(4f, 6f, PanelFlashTimer / 16f));

                    return IconTexture?.Frame(1, 7, 0, frame) ?? default;
                }
            }

            public IconState(bool hover, bool panel, IconType state)
            {
                BeingHoveredOver = hover;
                PlacedInPanel = panel;
                CurrentState = state;
            }

            public void Update()
            {
                if (PanelFlashTimer >= 1)
                    PanelFlashTimer++;
                if (PanelFlashTimer >= 16)
                    PanelFlashTimer = 0;

                if (MousePressFrameCountdown > 0)
                    MousePressFrameCountdown--;
            }
        }

        public IconType ClickedIcon = IconType.Inactive;

        public IconState[] SelectionIcons = new IconState[]
        {
            new(false, false, IconType.Plasma),
            new(false, false, IconType.Tesla),
            new(false, false, IconType.Laser),
            new(false, false, IconType.Gauss),
        };

        public IconState[] PanelIcons = new IconState[]
        {
            new(false, true, IconType.Inactive),
            new(false, true, IconType.Inactive),
            new(false, true, IconType.Inactive),
            new(false, true, IconType.Inactive),
        };

        public bool ShouldDeleteArmIndex = false;

        public int ArmIDToSpawn = -1;

        public int ArmIndex = -1;

        public bool FadeOut => Projectile.ai[0] == 1f;

        public Vector2 PlayerOffset;

        public ref float Time => ref Projectile.ai[1];

        public static Rectangle MouseRectangle => new((int)Main.MouseScreen.X, (int)Main.MouseScreen.Y, 2, 2);
        
        public override void SetStaticDefaults()
        {
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
            Projectile.hide = true;
        }

        public override void AI()
        {
            int plasmaCannonID = ModContent.ProjectileType<ExoskeletonPlasmaCannon>();
            int teslaCannonID = ModContent.ProjectileType<ExoskeletonTeslaCannon>();
            int laserCannonID = ModContent.ProjectileType<ExoskeletonLaserCannon>();
            int gaussNukeID = ModContent.ProjectileType<ExoskeletonGaussNukeCannon>();
            int[] arms = new int[]
            {
                plasmaCannonID,
                teslaCannonID,
                laserCannonID,
                gaussNukeID,
            };
            
            // Initialize things.
            if (PlayerOffset == Vector2.Zero)
                PlayerOffset = Main.MouseWorld - Main.LocalPlayer.Center;

            // Dynamically update panel icons.
            for (int i = 0; i < 4; i++)
                PanelIcons[i].CurrentState = IconType.Inactive;
            for (int i = 0; i < Main.maxProjectiles; i++)
            {
                if (!arms.Contains(Main.projectile[i].type) || Main.projectile[i].owner != Projectile.owner || !Main.projectile[i].active)
                    continue;

                IconType stateFromID = IconType.Inactive;
                if (Main.projectile[i].type == plasmaCannonID)
                    stateFromID = IconType.Plasma;
                if (Main.projectile[i].type == teslaCannonID)
                    stateFromID = IconType.Tesla;
                if (Main.projectile[i].type == laserCannonID)
                    stateFromID = IconType.Laser;
                if (Main.projectile[i].type == gaussNukeID)
                    stateFromID = IconType.Gauss;

                PanelIcons[(int)Main.projectile[i].ai[0]].CurrentState = stateFromID;
            }

            // Handle fade effects.
            if (Time >= AresExoskeleton.BoxParticleLifetime)
                Projectile.Opacity = MathHelper.Clamp(Projectile.Opacity - FadeOut.ToDirectionInt() * 0.0225f, 0f, 1f);
            if (FadeOut && Projectile.Opacity <= 0f)
                Projectile.Kill();

            if (Main.player[Projectile.owner].dead || !Main.player[Projectile.owner].active)
                Projectile.Kill();
            Time++;

            // Create and destroy arms as necessary.
            if (Main.myPlayer != Projectile.owner)
                return;

            if (ArmIDToSpawn >= 0)
            {
                bool armAlreadyExists = false;
                for (int i = 0; i < Main.maxProjectiles; i++)
                {
                    if (!arms.Contains(Main.projectile[i].type) || Main.projectile[i].owner != Projectile.owner || !Main.projectile[i].active || Main.projectile[i].ai[0] != ArmIndex)
                        continue;

                    armAlreadyExists = true;
                    break;
                }

                if (!armAlreadyExists)
                {
                    SoundEngine.PlaySound(SoundID.Zombie66, Projectile.Center);
                    int cannon = Projectile.NewProjectile(Projectile.GetSource_FromAI(), Projectile.Center, Vector2.Zero, ArmIDToSpawn, Projectile.damage, 0f, Projectile.owner, ArmIndex);
                    if (Main.projectile.IndexInRange(cannon))
                        Main.projectile[cannon].originalDamage = Projectile.originalDamage;
                }
            }
            if (ShouldDeleteArmIndex)
            {
                SoundEngine.PlaySound(SoundID.Item74, Projectile.Center);
                for (int i = 0; i < Main.maxProjectiles; i++)
                {
                    if (!arms.Contains(Main.projectile[i].type) || Main.projectile[i].owner != Projectile.owner || !Main.projectile[i].active || Main.projectile[i].ai[0] != ArmIndex)
                        continue;
                    
                    Main.projectile[i].Kill();
                }
            }

            ShouldDeleteArmIndex = false;
            ArmIDToSpawn = -1;
            ArmIndex = -1;
        }

        public override bool PreDraw(ref Color lightColor)
        {
            // This UI only renders for the player that created it.
            if (Main.myPlayer != Projectile.owner)
                return false;

            Texture2D panelTexture = ModContent.Request<Texture2D>(Texture).Value;
            Texture2D plasmaTexture = ModContent.Request<Texture2D>("CalamityMod/Projectiles/Summon/SmallAresArms/ExoskeletonPanelPlasma").Value;
            Texture2D arrowTexture = ModContent.Request<Texture2D>("CalamityMod/Projectiles/Summon/SmallAresArms/Arrow").Value;
            Vector2 area = plasmaTexture.Frame(1, 7, 0, 0).Size();
            Vector2 drawPosition = (Main.LocalPlayer.Center + PlayerOffset - Main.screenPosition).Floor();

            Rectangle[] selectionIconAreas = new Rectangle[4]
            {
                Utils.CenteredRectangle(drawPosition + new Vector2(-62f, -70f) * Projectile.scale, area * Projectile.scale),
                Utils.CenteredRectangle(drawPosition + new Vector2(-22f, -70f) * Projectile.scale, area * Projectile.scale),
                Utils.CenteredRectangle(drawPosition + new Vector2(22f, -70f) * Projectile.scale, area * Projectile.scale),
                Utils.CenteredRectangle(drawPosition + new Vector2(62f, -70f) * Projectile.scale, area * Projectile.scale)
            };

            bool hoveringOverAnySlot = false;
            bool clickedAnIconOnPanel = false;
            bool sufficientSlots = Main.LocalPlayer.maxMinions > AresExoskeleton.MinionSlotsPerCannon;

            // Draw icon and handle hover behaviors.
            // If an arm needs to be destroyed or spawned, it will happen on the next frame in the AI update loop.
            for (int i = 0; i < 4; i++)
            {
                Texture2D selectionIconTexture = SelectionIcons[i].IconTexture;

                // Handle selection icon click stuff.
                SelectionIcons[i].BeingHoveredOver = false;
                PanelIcons[i].BeingHoveredOver = false;
                if (selectionIconAreas[i].Intersects(MouseRectangle))
                {
                    hoveringOverAnySlot = true;
                    SelectionIcons[i].BeingHoveredOver = true;
                    if (Main.mouseLeft && Main.mouseLeftRelease && Projectile.Opacity >= 1f)
                    {
                        ClickedIcon = SelectionIcons[i].CurrentState;
                        SelectionIcons[i].MousePressFrameCountdown = 15;
                    }
                }

                // Handle panel icon click stuff.
                Rectangle panelArea = selectionIconAreas[i];
                panelArea.Y += (int)(Projectile.scale * 78f);
                if (panelArea.Intersects(MouseRectangle))
                {
                    hoveringOverAnySlot = true;
                    PanelIcons[i].BeingHoveredOver = true;
                    if (Main.mouseLeft && Main.mouseLeftRelease && Projectile.Opacity >= 1f && sufficientSlots)
                    {
                        clickedAnIconOnPanel = true;
                        PanelIcons[i].CurrentState = ClickedIcon;
                        PanelIcons[i].PanelFlashTimer = 1;

                        switch (ClickedIcon)
                        {
                            case IconType.Plasma:
                                ArmIDToSpawn = ModContent.ProjectileType<ExoskeletonPlasmaCannon>();
                                break;
                            case IconType.Tesla:
                                ArmIDToSpawn = ModContent.ProjectileType<ExoskeletonTeslaCannon>();
                                break;
                            case IconType.Laser:
                                ArmIDToSpawn = ModContent.ProjectileType<ExoskeletonLaserCannon>();
                                break;
                            case IconType.Gauss:
                                ArmIDToSpawn = ModContent.ProjectileType<ExoskeletonGaussNukeCannon>();
                                break;
                            case IconType.Inactive:
                                ShouldDeleteArmIndex = true;
                                PanelIcons[i].PanelFlashTimer = 0;
                                break;
                        }
                        ArmIndex = i;
                    }
                }

                Main.EntitySpriteDraw(selectionIconTexture, selectionIconAreas[i].Center.ToVector2(), SelectionIcons[i].Frame, Projectile.GetAlpha(Color.White), 0f, area * 0.5f, Projectile.scale, 0, 0);
            }

            Main.EntitySpriteDraw(panelTexture, drawPosition, null, Projectile.GetAlpha(Color.White), 0f, panelTexture.Size() * 0.5f, Projectile.scale, 0, 0);

            // Draw panel icons.
            for (int i = 0; i < 4; i++)
            {
                Rectangle panelArea = selectionIconAreas[i];
                panelArea.Y += (int)(Projectile.scale * 78f);
                Texture2D panelIconTexture = PanelIcons[i].IconTexture;
                if (panelIconTexture is not null)
                    Main.EntitySpriteDraw(panelIconTexture, panelArea.Center.ToVector2(), PanelIcons[i].Frame, Projectile.GetAlpha(Color.White), 0f, area * 0.5f, Projectile.scale, 0, 0);

                SelectionIcons[i].Update();
                PanelIcons[i].Update();
            }

            // Draw an arrow at the mouse from the selected icon if one has been selected.
            if (ClickedIcon != IconType.Inactive)
            {
                Vector2 arrowDrawPosition = selectionIconAreas[(int)ClickedIcon - 1].Center.ToVector2();
                Vector2 arrowDirection = (Main.MouseScreen - arrowDrawPosition).SafeNormalize(Vector2.UnitY);
                arrowDrawPosition += arrowDirection * Projectile.scale * 24f;

                Main.EntitySpriteDraw(arrowTexture, arrowDrawPosition, null, Projectile.GetAlpha(Color.White), arrowDirection.ToRotation(), arrowTexture.Size() * 0.5f, Projectile.scale, 0, 0);
            }

            // Tell the player if they don't have enough summon slots.
            if (hoveringOverAnySlot && !sufficientSlots)
                Utils.DrawBorderStringFourWay(Main.spriteBatch, FontAssets.MouseText.Value, this.GetLocalizedValue("NoSlots"), Main.MouseScreen.X + 20f, Main.MouseScreen.Y + 12f, Color.Cyan, Color.Black, new Vector2(0f, 0.5f));

            // Reset the clicked icon if a click is made but not to a specific icon.
            if (!hoveringOverAnySlot && Main.mouseLeft && Main.mouseLeftRelease)
                ClickedIcon = IconType.Inactive;

            if (hoveringOverAnySlot)
            {
                Main.blockMouse = true;
                Main.LocalPlayer.mouseInterface = true;
            }

            // Reset the clicked icon and handle the arm creation/deletion if an icon was clicked on the panel.
            if (clickedAnIconOnPanel)
                ClickedIcon = IconType.Inactive;

            return false;
        }

        public override void DrawBehind(int index, List<int> behindNPCsAndTiles, List<int> behindNPCs, List<int> behindProjectiles, List<int> overPlayers, List<int> overWiresUI)
        {
            overWiresUI.Add(index);
        }

        // This is a UI. It should not do damage.
        public override bool? CanDamage() => false;
    }
}
