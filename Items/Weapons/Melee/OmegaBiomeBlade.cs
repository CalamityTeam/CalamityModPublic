using CalamityMod.Items.Materials;
using CalamityMod.Items.Placeables;
using CalamityMod.DataStructures;
using CalamityMod.Particles;
using CalamityMod.Projectiles.Melee;
using CalamityMod.Items.Placeables.Ores;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;
using static CalamityMod.CalamityUtils;
using static CalamityMod.Items.Weapons.Melee.OmegaBiomeBlade;
using static Terraria.ModLoader.ModContent;

namespace CalamityMod.Items.Weapons.Melee
{
    public class OmegaBiomeBlade : ModItem
    {
        public Attunement mainAttunement = null;
        public Attunement secondaryAttunement = null;
        public Projectile MeatHook;

        //Used for passive effects
        public int UseTimer = 0;
        public bool OnHitProc = false;

        #region stats
        public static int WhirlwindAttunement_BaseDamage = 400;
        public static int WhirlwindAttunement_LocalIFrames = 20; //Remember its got one extra update
        public static int WhirlwindAttunement_SigilTime = 1200;
        public static float WhirlwindAttunement_BeamDamageReduction = 0.5f;
        public static float WhirlwindAttunement_BaseDamageReduction = 0.2f;
        public static float WhirlwindAttunement_FullChargeDamageBoost = 2f;

        public static int WhirlwindAttunement_PassiveBaseDamage = 200;


        public static int SuperPogoAttunement_BaseDamage = 500;
        public static int SuperPogoAttunement_ShredIFrames = 10;
        public static int SuperPogoAttunement_LocalIFrames = 30; //Be warned its got one extra update so all the iframes should be divided in 2
        public static int SuperPogoAttunement_LocalIFramesCharged = 16;
        public static float SuperPogoAttunement_SlashDamageBoost = 5f; //Keep in mind the slice always crits
        public static int SuperPogoAttunementSlashLifesteal = 6;
        public static int SuperPogoAttunement_SlashIFrames = 60;
        public static float SuperPogoAttunement_ShotDamageBoost = 2f;

        public static int SuperPogoAttunement_PassiveLifeSteal = 10;


        public static int ShockwaveAttunement_BaseDamage = 2500;
        public static int ShockwaveAttunement_DashHitIFrames = 60;
        public static float ShockwaveAttunement_FullChargeBoost = 1f; //The EXTRA damage boost. So putting 1 here will make it deal double damage. Putting 0.5 here will make it deal 1.5x the damage.
        public static float ShockwaveAttunement_MonolithDamageBoost = 2f;
        public static float ShockwaveAttunement_BlastDamageReduction = 0.6f;

        public static int ShockwaveAttunement_PassiveBaseDamage = 200;


        public static int FlailBladeAttunement_BaseDamage = 320;
        public static int FlailBladeAttunement_LocalIFrames = 30;
        public static int FlailBladeAttunement_FlailTime = 10;
        public static int FlailBladeAttunement_Reach = 400;
        public static float FlailBladeAttunement_ChainDamageReduction = 0.5f;
        public static float FlailBladeAttunement_GhostChainDamageReduction = 0.5f;

        public static int FlailBladeAttunement_PassiveBaseDamage = 500;

        //Proc coefficients. aka the likelihood of any given attack to trigger a on-hit passive.
        public static float WhirlwindAttunement_WhirlwindProc = 0.24f;
        public static float WhirlwindAttunement_SwordThrowProc = 1f;
        public static float WhirlwindAttunement_SwordBeamProc = 0.05f;

        public static float SuperPogoAttunement_ShredderProc = 0.1f;
        public static float SuperPogoAttunement_WheelProc = 0.4f;
        public static float SuperPogoAttunement_DashProc = 1f;

        public static float ShockwaveAttunement_SwordProc = 1f;
        public static float ShockwaveAttunement_MonolithProc = 1f;
        public static float ShockwaveAttunement_BlastProc = 0.5f;

        public static float FlailBladeAttunement_BladeProc = 0.1f;
        public static float FlailBladeAttunement_ChainProc = 0.05f;
        public static float FlailBladeAttunement_GhostChainProc = 0.1f;
        #endregion

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("True Biome Blade");
            Tooltip.SetDefault("FUNCTION_DESC\n" +
                               "FUNCTION_EXTRA\n" +
                               "FUNCTION_PASSIVE\n" +
                               "Holding down RMB for 2 seconds attunes the weapon to the powers of the surrounding biome\n" +
                               "Using RMB for a shorter period of time switches your active and passive attunements around\n" +
                               "Active attunement : None\n" +
                               "Passive attunement: None\n");
        }

        #region tooltip editing
        public override void ModifyTooltips(List<TooltipLine> list)
        {
            Player player = Main.player[Main.myPlayer];
            if (player is null)
                return;

            foreach (TooltipLine l in list)
            {
                if (l.text.StartsWith("FUNCTION_DESC"))
                {
                    if (mainAttunement != null)
                    {
                        l.overrideColor = mainAttunement.tooltipColor;
                        l.text = mainAttunement.function_description;
                    }
                    else
                    {
                        l.overrideColor = new Color(163, 163, 163);
                        l.text = "Does nothing.. yet";
                    }
                }

                if (l.text.StartsWith("FUNCTION_EXTRA"))
                {
                    if (mainAttunement != null)
                    {
                        l.overrideColor = mainAttunement.tooltipColor;
                        l.text = mainAttunement.function_description_extra;
                    }
                    else
                    {
                        l.overrideColor = new Color(163, 163, 163);
                        l.text = "It seems that upgrading the blade expanded the scope of the previous attunements";
                    }
                }

                if (l.text.StartsWith("FUNCTION_PASSIVE"))
                {
                    if (secondaryAttunement != null)
                    {
                        l.overrideColor = secondaryAttunement.tooltipColor;
                        l.text = secondaryAttunement.passive_description;
                    }
                    else
                    {
                        l.overrideColor = new Color(163, 163, 163);
                        l.text = "Your secondary attunement can now provide passive bonuses";
                    }
                }

                if (l.text.StartsWith("Active attunement"))
                {
                    if (mainAttunement != null)
                    {
                        l.overrideColor = Color.Lerp(mainAttunement.tooltipColor, mainAttunement.tooltipColor2, 0.5f + (float)Math.Sin(Main.GlobalTime) * 0.5f);
                        l.text = "Active Attumenent : [" + mainAttunement.name + "]";
                    }
                    else
                    {
                        l.overrideColor = new Color(163, 163, 163);
                        l.text = "Active Attumenent : [None]";
                    }
                }

                if (l.text.StartsWith("Passive attunement"))
                {
                    if (secondaryAttunement != null)
                    {
                        l.overrideColor = Color.Lerp(Color.Lerp(secondaryAttunement.tooltipColor, secondaryAttunement.tooltipColor2, 0.5f + (float)Math.Sin(Main.GlobalTime) * 0.5f), Color.Gray, 0.5f);
                        l.text = "Passive Attumenent : [" + secondaryAttunement.name + "]";
                    }
                    else
                    {
                        l.overrideColor = new Color(163, 163, 163);
                        l.text = "Passive Attumenent : [None]";
                    }
                }
            }
        }

    #endregion

        public override void SetDefaults()
        {
            item.width = item.height = 92;
            item.damage = 130;
            item.melee = true;
            item.useAnimation = 18;
            item.useTime = 18;
            item.useTurn = true;
            item.useStyle = ItemUseStyleID.SwingThrow;
            item.knockBack = 8;
            item.UseSound = SoundID.Item1;
            item.autoReuse = true;
			item.value = CalamityGlobalItem.Rarity11BuyPrice;
			item.rare = ItemRarityID.Purple;
			item.shoot = ProjectileID.PurificationPowder;
            item.shootSpeed = 15f;
        }

        public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);
            recipe.AddIngredient(ItemType<TrueBiomeBlade>());
            recipe.AddIngredient(ItemType<CoreofCalamity>());
            recipe.AddIngredient(ItemType<AstralBar>(), 3);
            recipe.AddIngredient(ItemType<BarofLife>(), 3);
            recipe.AddIngredient(ItemType<ExodiumClusterOre>(), 5);
            recipe.AddTile(TileID.LunarCraftingStation);
            recipe.SetResult(this);
            recipe.AddRecipe();
        }

        #region Saving and syncing attunements
        public override bool CloneNewInstances => true;

        public override ModItem Clone(Item item)
        {
            var clone = base.Clone(item);
            if (Main.mouseItem.type == ItemType<OmegaBiomeBlade>())
                item.modItem.HoldItem(Main.player[Main.myPlayer]);
            (clone as OmegaBiomeBlade).mainAttunement = (item.modItem as OmegaBiomeBlade).mainAttunement;
            (clone as OmegaBiomeBlade).secondaryAttunement = (item.modItem as OmegaBiomeBlade).secondaryAttunement;

            return clone;
        }

        public override ModItem Clone() //ditto
        {
            var clone = base.Clone();
            (clone as OmegaBiomeBlade).mainAttunement = mainAttunement;
            (clone as OmegaBiomeBlade).secondaryAttunement = secondaryAttunement;

            return clone;
        }

        public override TagCompound Save()
        {
            int attunement1 = mainAttunement == null ? -1 : (int)mainAttunement.id;
            int attunement2 = secondaryAttunement == null ? -1 : (int)secondaryAttunement.id;
            TagCompound tag = new TagCompound
            {
                { "mainAttunement", attunement1 },
                { "secondaryAttunement", attunement2 }
            };
            return tag;
        }

        public override void Load(TagCompound tag)
        {
            int attunement1 = tag.GetInt("mainAttunement");
            int attunement2 = tag.GetInt("secondaryAttunement");

            mainAttunement = AttunementHelper.IDtoAttunement(attunement1);
            secondaryAttunement = AttunementHelper.IDtoAttunement(attunement2);

            if (mainAttunement == secondaryAttunement)
                secondaryAttunement = null;
        }

        public override void NetSend(BinaryWriter writer)
        {
            writer.Write((byte)mainAttunement.id);
            writer.Write((byte)secondaryAttunement.id);
        }

        public override void NetRecieve(BinaryReader reader)
        {
            mainAttunement = AttunementHelper.IDtoAttunement(reader.ReadByte());
            secondaryAttunement = AttunementHelper.IDtoAttunement(reader.ReadByte());
        }

        #endregion

        public override void HoldItem(Player player)
        {
            player.Calamity().rightClickListener = true;

            //Reset the strong lunge thing just in case it didnt get caught beofre.

            if (CanUseItem(player))
            {
                player.Calamity().LungingDown = false;
            }
            else
            {
                UseTimer++;
            }

            if (mainAttunement == null)
            {
                item.noUseGraphic = false;
                item.useStyle = ItemUseStyleID.SwingThrow;
                item.shoot = ProjectileID.PurificationPowder;
                item.shootSpeed = 12f;
                item.UseSound = SoundID.Item1;
            }
            else
                mainAttunement.ApplyStats(ref item);

            if (player.whoAmI != Main.myPlayer)
                return;


            //PAssive effetcsts only happen on the side of the owner
            if (secondaryAttunement.id != AttunementID.FlailBlade || MeatHook == null || !MeatHook.active)
                MeatHook = null;
            if (secondaryAttunement.id == AttunementID.FlailBlade && MeatHook == null )
                MeatHook = Projectile.NewProjectileDirect(player.Center, Vector2.Zero, ProjectileType<ChainedMeatHook>(), (int)(FlailBladeAttunement_PassiveBaseDamage * player.MeleeDamage()), 0f, player.whoAmI);


            if (secondaryAttunement != null)
                secondaryAttunement.PassiveEffect(player, ref UseTimer, ref OnHitProc, MeatHook);

            if (player.Calamity().mouseRight && CanUseItem(player) && player.whoAmI == Main.myPlayer && !Main.mapFullscreen)
            {
                //Don't shoot out a visual blade if you already have one out
                if (Main.projectile.Any(n => n.active && n.type == ProjectileType<TrueBiomeBladeHoldout>() && n.owner == player.whoAmI))
                    return;

                Projectile.NewProjectile(player.Top, Vector2.Zero, ProjectileType<TrueBiomeBladeHoldout>(), 0, 0, player.whoAmI);
            }
        }

        public override bool CanUseItem(Player player)
        {
            return !Main.projectile.Any(n => n.active && n.owner == player.whoAmI &&
            (n.type == ProjectileType<SwordsmithsPride>() ||
             n.type == ProjectileType<MercurialTides>() ||
             n.type == ProjectileType<SanguineFury>() ||
             n.type == ProjectileType<LamentationsOfTheChained>()
            ));
        }

        //No need for any wacky zany hijinx in the shoot method for once??? damn
        public override bool Shoot(Player player, ref Vector2 position, ref float speedX, ref float speedY, ref int type, ref int damage, ref float knockBack) => mainAttunement == null ? false : true;

        internal static ChargingEnergyParticleSet BiomeEnergyParticles = new ChargingEnergyParticleSet(-1, 2, Color.White, Color.White, 0.04f, 20f);
        internal static void UpdateAllParticleSets()
        {
            BiomeEnergyParticles.Update();
        }

        public override bool PreDrawInInventory(SpriteBatch spriteBatch, Vector2 position, Rectangle frame, Color drawColor, Color itemColor, Vector2 origin, float scale)
        {
            if (mainAttunement == null)
                return true;

            position.Y -= 6f * scale;

            Texture2D itemTexture = GetTexture("CalamityMod/Items/Weapons/Melee/OmegaBiomeBladeExtra");
            Rectangle itemFrame = (Main.itemAnimations[item.type] == null) ? itemTexture.Frame() : Main.itemAnimations[item.type].GetFrame(itemTexture);

            // Draw all particles.

            Vector2 particleDrawCenter = position + new Vector2(12f, 16f) * Main.inventoryScale;

            BiomeEnergyParticles.EdgeColor = mainAttunement.tooltipColor2;
            BiomeEnergyParticles.CenterColor = mainAttunement.tooltipColor;
            BiomeEnergyParticles.InterpolationSpeed = 0.1f;
            BiomeEnergyParticles.DrawSet(particleDrawCenter + Main.screenPosition);

            Vector2 displacement = Vector2.UnitX.RotatedBy(Main.GlobalTime * 3f) * 2f * (float)Math.Sin(Main.GlobalTime);

            spriteBatch.End();
            spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.Additive);
            spriteBatch.Draw(itemTexture, position + displacement, itemFrame, BiomeEnergyParticles.CenterColor, 0f, origin, scale, SpriteEffects.None, 0f);
            spriteBatch.Draw(itemTexture, position - displacement, itemFrame, BiomeEnergyParticles.CenterColor, 0f, origin, scale, SpriteEffects.None, 0f);
            spriteBatch.End();
            spriteBatch.Begin(default, default);

            spriteBatch.Draw(itemTexture, position, itemFrame, Color.White, 0f, origin, scale, SpriteEffects.None, 0f);
            return false;
        }

        public override bool PreDrawInWorld(SpriteBatch spriteBatch, Color lightColor, Color alphaColor, ref float rotation, ref float scale, int whoAmI)
        {
            if (mainAttunement == null)
                return true;

            //Draw the charged version if you can
            Texture2D itemTexture = GetTexture("CalamityMod/Items/Weapons/Melee/OmegaBiomeBladeExtra");
            spriteBatch.Draw(itemTexture, item.Center - Main.screenPosition, null, lightColor, rotation, item.Size * 0.5f, scale, SpriteEffects.None, 0f);
            return false;
        }
    }

    public class TrueBiomeBladeHoldout : ModProjectile //Visuals
    {
        private Player Owner => Main.player[projectile.owner];
        public bool OwnerCanUseItem => Owner.HeldItem == associatedItem ? (Owner.HeldItem.modItem as OmegaBiomeBlade).CanUseItem(Owner) : false;
        public bool OwnerMayChannel => OwnerCanUseItem && Owner.Calamity().mouseRight && Owner.active && !Owner.dead;
        public ref float ChanneledState => ref projectile.ai[0];
        public ref float ChannelTimer => ref projectile.ai[1];
        public ref float Initialized => ref projectile.localAI[0];

        private Item associatedItem;
        const int ChannelTime = 120;

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("True Biome Blade");
        }
        public override string Texture => "CalamityMod/Items/Weapons/Melee/OmegaBiomeBlade";
        public bool drawIndrawHeldProjInFrontOfHeldItemAndArms = true;
        public override void SetDefaults()
        {
            projectile.width = projectile.height = 36;
            projectile.aiStyle = -1;
            projectile.friendly = true;
            projectile.penetrate = -1;
            projectile.timeLeft = 60;
            projectile.tileCollide = false;
            projectile.damage = 0;
        }

        public CurveSegment anticipation = new CurveSegment(EasingType.SineBump, 0f, 0f, -0.3f);
        public CurveSegment rise = new CurveSegment(EasingType.ExpIn, 0f, 0f, 1f);
        public CurveSegment overshoot = new CurveSegment(EasingType.SineBump, 0.80f, 1f, 0.1f);
        internal float SwordHeight() => PiecewiseAnimation(ChannelTimer / (float)ChannelTime, new CurveSegment[] {rise, overshoot });

        public override void AI()
        {

            if (Initialized == 0f)
            {
                //If dropped, kill it instantly
                if (Owner.HeldItem.type != ItemType<OmegaBiomeBlade>())
                {
                    projectile.Kill();
                    return;
                }

                if (Owner.whoAmI == Main.myPlayer)
                    Main.PlaySound(SoundID.DD2_DarkMageHealImpact);

                associatedItem = Owner.HeldItem;
                //Switch up the attunements
                Attunement temporaryAttunementStorage = (associatedItem.modItem as OmegaBiomeBlade).mainAttunement;
                (associatedItem.modItem as OmegaBiomeBlade).mainAttunement = (associatedItem.modItem as OmegaBiomeBlade).secondaryAttunement;
                (associatedItem.modItem as OmegaBiomeBlade).secondaryAttunement = temporaryAttunementStorage;
                Initialized = 1f;
            }

            if (!OwnerMayChannel && ChanneledState == 0f) //IF the channeling gets interrupted for any reason
            {
                projectile.Center = Owner.Top + new Vector2(18f, 0f);
                ChanneledState = 1f;
                projectile.timeLeft = 60;
                return;
            }

            if (ChanneledState == 0f)
            {
                Owner.heldProj = projectile.whoAmI;
                Owner.itemRotation = (-Vector2.UnitY).ToRotation();

                projectile.Center = Owner.Top + new Vector2(0f, -20f * SwordHeight() - 50f);
                projectile.rotation = -MathHelper.PiOver4; // No more silly turnaround with the repaired one?
                ChannelTimer++;
                projectile.timeLeft = 60;

                if (ChannelTimer == ChannelTime - 15)
                {
                    Attune((OmegaBiomeBlade)associatedItem.modItem);
                    Color particleColor = (associatedItem.modItem as OmegaBiomeBlade).mainAttunement.tooltipColor;

                    for (int i = 0; i <= 5; i++)
                    {
                        Vector2 displace = Vector2.UnitX * 20 * Main.rand.NextFloat(-1f, 1f);
                        Particle Glow = new GenericBloom(Owner.Bottom + displace, -Vector2.UnitY * Main.rand.NextFloat(1f, 5f), particleColor, 0.02f + Main.rand.NextFloat(0f, 0.2f), 20 + Main.rand.Next(30));
                        GeneralParticleHandler.SpawnParticle(Glow);
                    }
                    for (int i = 0; i <= 10; i++)
                    {
                        Vector2 displace = Vector2.UnitX * 16 * Main.rand.NextFloat(-1f, 1f);
                        Particle Sparkle = new GenericSparkle(Owner.Bottom + displace, -Vector2.UnitY * Main.rand.NextFloat(1f, 5f), particleColor, particleColor, 0.5f + Main.rand.NextFloat(-0.2f, 0.2f), 20 + Main.rand.Next(30), 1, 2f);
                        GeneralParticleHandler.SpawnParticle(Sparkle);
                    }
                }

                if (ChannelTimer >= ChannelTime - 15)
                {
                    Vector2 Shake = Main.rand.NextVector2Circular(6f, 6f);
                    projectile.Center += Shake;

                    Vector2 Bottom = projectile.Center + Vector2.UnitY * 20f;
                    Particle Sparkle = new ElectricSpark(Bottom, -Vector2.UnitY.RotatedByRandom(MathHelper.PiOver2) * Main.rand.NextFloat(10f, 20f), Color.White, Main.rand.NextBool() ? Main.rand.NextBool() ? Color.Goldenrod : Color.GreenYellow : Main.rand.NextBool() ? Color.Cyan : Color.Magenta, 1f + Main.rand.NextFloat(0f, 1f), 34, rotationSpeed : 0.1f, bloomScale : 4f);
                    GeneralParticleHandler.SpawnParticle(Sparkle);
                }

                if (ChannelTimer >= ChannelTime)
                {
                    projectile.timeLeft = 60;
                    ChanneledState = 2f; //State where it stays invisible doing nothing. Acts as a cooldown
                }
            }

            if (ChanneledState == 1f)
                projectile.position += Vector2.UnitY * -0.3f * (1f + projectile.timeLeft / 60f);
        }

        public void Attune(OmegaBiomeBlade item)
        {
            bool jungle = Owner.ZoneJungle;
            bool snow = Owner.ZoneSnow;
            bool evil = Owner.ZoneCorrupt || Owner.ZoneCrimson;
            bool desert = Owner.ZoneDesert;
            bool hell = Owner.ZoneUnderworldHeight;
            bool ocean = Owner.ZoneBeach;
            bool holy = Owner.ZoneHoly;
            bool astral = Owner.Calamity().ZoneAstral;
            bool marine = Owner.Calamity().ZoneAbyss || Owner.Calamity().ZoneSunkenSea;

            Attunement attunement = new WhirlwindAttunement();
            if (desert || hell)
                attunement = new SuperPogoAttunement();
            if (jungle || ocean || snow) //Check put after the desert check so ocean doesnt get overriden as desert
                attunement = new FlailBladeAttunement();
            if (evil) //Evil check separated so that it overrides corrupted beach & snow biomes
                attunement = new SuperPogoAttunement();
            if (astral || marine)
                attunement = new ShockwaveAttunement();
            if (holy)
                attunement = new WhirlwindAttunement(); //Putting holy attunement at the end so it may override hallowed variants of biomes

            //If the owner already had the attunement , break out of it (And unswap)
            if (item.secondaryAttunement == attunement)
            {
                Main.PlaySound(SoundID.DD2_LightningBugZap, projectile.Center);
                item.secondaryAttunement = item.mainAttunement;
                item.mainAttunement = attunement;
                return;
            }
            //Chunger
            Main.PlaySound(mod.GetLegacySoundSlot(SoundType.Custom, "Sounds/Custom/ThunderStrike"), projectile.Center);
            Particle thunder = new ThunderBoltVFX(projectile.Center + Vector2.UnitY * 20f, Main.rand.NextBool() ? Main.rand.NextBool() ? Color.Goldenrod : Color.GreenYellow : Main.rand.NextBool() ? Color.Cyan : Color.Magenta, 0f, 1.5f, Vector2.One, 1f, 15f, projectile, 20f);
            GeneralParticleHandler.SpawnParticle(thunder);

            if (Main.LocalPlayer.Calamity().GeneralScreenShakePower < 5)
                Main.LocalPlayer.Calamity().GeneralScreenShakePower = 5;

            item.mainAttunement = attunement;
        }

        public override void Kill(int timeLeft)
        {
            if (associatedItem == null)
            {
                return;
            }
            //If we swapped out the main attunement for the second one despite the second attunement being empty at the time, unswap them.
            if ((associatedItem.modItem as OmegaBiomeBlade).mainAttunement == null && (associatedItem.modItem as OmegaBiomeBlade).secondaryAttunement != null)
            {
                (associatedItem.modItem as OmegaBiomeBlade).mainAttunement = (associatedItem.modItem as OmegaBiomeBlade).secondaryAttunement;
                (associatedItem.modItem as OmegaBiomeBlade).secondaryAttunement = null;
            }
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
        {


            if (ChanneledState == 0f && ChannelTimer > 10f)
            {
                Texture2D tex = GetTexture(Texture);
                spriteBatch.Draw(tex, projectile.Center - Main.screenPosition, null, lightColor, projectile.rotation, tex.Size() / 2, 1, 0, 0);

                return false;

            }
            else if (ChanneledState == 1f)
            {
                Texture2D tex = GetTexture(Texture);
                Vector2 squishyScale = new Vector2(Math.Abs((float)Math.Sin(MathHelper.Pi + MathHelper.TwoPi * projectile.timeLeft / 30f)), 1f);
                SpriteEffects flip = (float)Math.Sin(MathHelper.Pi + MathHelper.TwoPi * projectile.timeLeft / 30f) > 0 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;
                spriteBatch.Draw(tex, projectile.position - Main.screenPosition, null, lightColor * (projectile.timeLeft / 60f), 0, tex.Size() / 2, squishyScale * (2f - (projectile.timeLeft / 60f)), flip, 0);

                return false;
            }

            else
                return false;
        }
    }
}
