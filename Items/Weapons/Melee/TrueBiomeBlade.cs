using CalamityMod.Items.Materials;
using CalamityMod.Items.Placeables;
using CalamityMod.DataStructures;
using CalamityMod.Particles;
using CalamityMod.Projectiles.Melee;
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
using static CalamityMod.Items.Weapons.Melee.TrueBiomeBlade;
using static Terraria.ModLoader.ModContent;

namespace CalamityMod.Items.Weapons.Melee
{
    public class TrueBiomeBlade : ModItem
    {
        public Attunement mainAttunement = null;
        public Attunement secondaryAttunement = null;
        public int Combo = 0;
        public int StoredLunges = 2;
        public int PowerLungeCounter = 0;

        #region stats
        public static int DefaultAttunement_BaseDamage = 160;
        public static int DefaultAttunement_SigilTime = 600;

        public static int EvilAttunement_BaseDamage = 320;
        public static int EvilAttunement_Lifesteal = 4;
        public static int EvilAttunement_BounceIFrames = 10;
        public static float EvilAttunement_SlashDamageBoost = 3f;
        public static int EvilAttunement_SlashIFrames = 60;

        public static int ColdAttunement_BaseDamage = 320;
        public static float ColdAttunement_SecondSwingBoost = 1.8f;
        public static float ColdAttunement_ThirdSwingBoost = 3f;
        public static float ColdAttunement_MistDamageReduction = 0.2f;

        public static int HotAttunement_BaseDamage = 320;
        public static int HotAttunement_ShredIFrames = 8;
        public static float HotAttunement_ShotDamageBoost = 2;
        public static int HotAttunement_LocalIFrames = 30; //Be warned its got one extra update so all the iframes should be divided in 2
        public static int HotAttunement_LocalIFramesCharged = 16;

        public static int TropicalAttunement_BaseDamage = 160;
        public static float TropicalAttunement_ChainDamageReduction = 0.5f;
        public static float TropicalAttunement_VineDamageReduction = 0.5f;
        public static int TropicalAttunement_LocalIFrames = 60; //Be warned its got 2 extra updates so all the iframes should be divided in 3

        public static int HolyAttunement_BaseDamage = 160;
        public static float HolyAttunement_BaseDamageReduction = 0.2f;
        public static float HolyAttunement_FullChargeDamageBoost = 2f;
        public static int HolyAttunement_LocalIFrames = 16; //Be warned its got 1 extra update yadda yadda

        public static int AstralAttunement_BaseDamage = 500;
        public static int AstralAttunement_DashHitIFrames = 30;
        public static float AstralAttunement_FullChargeBoost = 1f; //The EXTRA damage boost. So putting 1 here will make it deal double damage. Putting 0.5 here will make it deal 1.5x the damage.
        public static float AstralAttunement_MonolithDamageBoost = 2f;

        public static int MarineAttunement_BaseDamage = 160;
        #endregion


        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Biome Blade");
            Tooltip.SetDefault("FUNCTION_DESC\n" +
                               "FUNCTION_EXTRA\n" +
                               "Use RMB while standing still on the ground to attune the weapon to the powers of the surrounding biome\n" +
                               "Using RMB otherwise switches between the current attunement and an extra stored one\n" +
                               "Main attunement : None\n" +
                               "Secondary attunement: None\n"); //Theres potential for flavor text as well but im not a writer
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
                        l.text = "Repairing the blade seems to have improved its attuning capacities";
                    }
                }

                if (l.text.StartsWith("Main attunement"))
                {
                    if (mainAttunement != null)
                    {
                        l.overrideColor = mainAttunement.tooltipColor;
                        l.text = "Main Attumenent : [" + mainAttunement.name + "]";
                    }
                    else
                    {
                        l.overrideColor = new Color(163, 163, 163);
                        l.text = "Main Attumenent : [None]";
                    }
                }

                if (l.text.StartsWith("Secondary attunement"))
                {
                    if (secondaryAttunement != null)
                    {
                        l.overrideColor = Color.Lerp(secondaryAttunement.tooltipColor, Color.Gray, 0.5f);
                        l.text = "Secondary Attumenent : [" + secondaryAttunement.name + "]";
                    }
                    else
                    {
                        l.overrideColor = new Color(163, 163, 163);
                        l.text = "Secondary Attumenent : [None]";
                    }
                }
            }
        }

        #endregion

        public override void SetDefaults()
        {
            item.width = item.height = 68;
            item.damage = 160;
            item.melee = true;
            item.useAnimation = 21;
            item.useTime = 21;
            item.useTurn = true;
            item.useStyle = ItemUseStyleID.SwingThrow;
            item.knockBack = 7.5f;
            item.UseSound = SoundID.Item1;
            item.autoReuse = true;
            item.value = Item.buyPrice(0, 80, 0, 0);
            item.rare = ItemRarityID.Yellow;
            item.shoot = ProjectileID.PurificationPowder;
            item.shootSpeed = 12f;
        }

        public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);
            recipe.AddIngredient(ItemType<BiomeBlade>());
            recipe.AddIngredient(ItemType<LivingShard>(), 5);
            recipe.AddIngredient(ItemID.PixieDust, 2);
            recipe.AddIngredient(ItemType<Stardust>(), 10);
            recipe.AddIngredient(ItemType<EssenceofChaos>(), 5); 
            recipe.AddIngredient(ItemType<EssenceofCinder>(), 5);
            recipe.AddIngredient(ItemType<EssenceofEleum>(), 5);
            recipe.AddTile(TileID.MythrilAnvil);
            recipe.SetResult(this);
            recipe.AddRecipe();
        }

        #region Saving and syncing attunements
        public override bool CloneNewInstances => true;

        public override ModItem Clone(Item item)
        {
            var clone = base.Clone(item);
            if (Main.mouseItem.type == ItemType<TrueBiomeBlade>())
                item.modItem.HoldItem(Main.player[Main.myPlayer]);
            (clone as TrueBiomeBlade).mainAttunement = (item.modItem as TrueBiomeBlade).mainAttunement;
            (clone as TrueBiomeBlade).secondaryAttunement = (item.modItem as TrueBiomeBlade).secondaryAttunement;

            return clone;
        }

        public override ModItem Clone() //ditto
        {
            var clone = base.Clone();
            (clone as TrueBiomeBlade).mainAttunement = mainAttunement;
            (clone as TrueBiomeBlade).secondaryAttunement = secondaryAttunement;

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

            mainAttunement = Attunement.attunementArray[attunement1 != -1 ? attunement1 : Attunement.attunementArray.Length - 1];
            secondaryAttunement = Attunement.attunementArray[attunement2 != -1 ? attunement2 : Attunement.attunementArray.Length - 1];

            if (mainAttunement == secondaryAttunement)
                secondaryAttunement = null;
        }

        public override void NetSend(BinaryWriter writer)
        {
            writer.Write(mainAttunement != null ? (byte)mainAttunement.id : Attunement.attunementArray.Length - 1);
            writer.Write(secondaryAttunement != null ? (byte)secondaryAttunement.id : Attunement.attunementArray.Length - 1);
        }

        public override void NetRecieve(BinaryReader reader)
        {
            mainAttunement = Attunement.attunementArray[reader.ReadByte()];
            secondaryAttunement = Attunement.attunementArray[reader.ReadByte()];
        }

        #endregion

        public override void HoldItem(Player player)
        {
            player.Calamity().rightClickListener = true;

            if (player.velocity.Y == 0) //reset the amount of lunges on ground contact
            {
                StoredLunges = 2;
                if (PowerLungeCounter != 3)
                    PowerLungeCounter = 0;
            }

            if (mainAttunement == null)
            {
                item.noUseGraphic = false;
                item.useStyle = ItemUseStyleID.SwingThrow;
                item.shoot = ProjectileID.PurificationPowder;
                item.shootSpeed = 12f;
                item.UseSound = SoundID.Item1;

                Combo = 0;
                PowerLungeCounter = 0;
            }

            else
                mainAttunement.ApplyStats(item);

            if (mainAttunement != null && mainAttunement.id != AttunementID.TrueCold && mainAttunement.id != AttunementID.TrueTropical)
                Combo = 0;

            if (player.Calamity().mouseRight && CanUseItem(player) && player.whoAmI == Main.myPlayer && !Main.mapFullscreen)
            {
                //Don't shoot out a visual blade if you already have one out
                if (Main.projectile.Any(n => n.active && n.type == ProjectileType<BiomeBladeHoldout>() && n.owner == player.whoAmI))
                    return;

                int x = (int)player.Center.X / 16;
                int y = (int)(player.position.Y + (float)player.height - 1f) / 16;
                Tile tileStandingOn = Main.tile[x, y + 1];

                bool mayAttune = player.StandingStill() && !player.mount.Active && tileStandingOn.IsTileSolidGround();
                Vector2 displace = new Vector2(18f, 0f);
                Projectile.NewProjectile(player.Top + displace, Vector2.Zero, ProjectileType<BiomeBladeHoldout>(), 0, 0, player.whoAmI, mayAttune ? 0f : 1f);
            }
        }

        public override bool CanUseItem(Player player)
        {
            return !Main.projectile.Any(n => n.active && n.owner == player.whoAmI &&
            (n.type == ProjectileType<TrueBitingEmbrace>() || 
             n.type == ProjectileType<TrueGrovetendersTouch>() ||
             n.type == ProjectileType<TrueAridGrandeur>() ||
             n.type == ProjectileType<HeavensMight>() ||
             n.type == ProjectileType<TheirAbhorrence>() ||
             n.type == ProjectileType<GestureForTheDrowned>()));  
        }

        public override bool Shoot(Player player, ref Vector2 position, ref float speedX, ref float speedY, ref int type, ref int damage, ref float knockBack)
        {
            if (mainAttunement == null)
                return false;

            return mainAttunement.Shoot(player, ref position, ref speedX, ref speedY, ref type, ref damage, ref knockBack, ref Combo, ref StoredLunges, ref PowerLungeCounter);
        }


        //This is only used for the purity sigil effect of the default attunement
        public override void OnHitNPC(Player player, NPC target, int damage, float knockBack, bool crit)
        {
            if (mainAttunement.id != AttunementID.TrueDefault || player.whoAmI != Main.myPlayer)
                return;

            foreach (Projectile proj in Main.projectile)
            {
                if (proj.active && proj.type == ProjectileType<PurityProjectionSigil>() && proj.owner == player.whoAmI)
                {
                    //Reset the timeleft on the sigil & give it its new target (or the same, it doesnt matter really.
                    proj.ai[0] = target.whoAmI;
                    proj.timeLeft = DefaultAttunement_SigilTime;
                    return;
                }
            }
            Projectile.NewProjectile(target.Center, Vector2.Zero, ProjectileType<PurityProjectionSigil>(), 0, 0, player.whoAmI, target.whoAmI);
        }

        internal static ChargingEnergyParticleSet BiomeEnergyParticles = new ChargingEnergyParticleSet(-1, 2, Color.White, Color.White, 0.04f, 20f);
        internal static void UpdateAllParticleSets()
        {
            BiomeEnergyParticles.Update();
        }

        public override bool PreDrawInInventory(SpriteBatch spriteBatch, Vector2 position, Rectangle frame, Color drawColor, Color itemColor, Vector2 origin, float scale)
        {
            Texture2D itemTexture = GetTexture("CalamityMod/Projectiles/Melee/MendedBiomeBlade");

            if (mainAttunement == null)
            {
                spriteBatch.Draw(itemTexture, position, null, drawColor, 0f, origin, scale, SpriteEffects.None, 0f);
                return false;
            }
                

            // Draw all particles.

            Vector2 particleDrawCenter = position + new Vector2(12f, 16f) * Main.inventoryScale;

            BiomeEnergyParticles.EdgeColor = mainAttunement.energyParticleEdgeColor;
            BiomeEnergyParticles.CenterColor = mainAttunement.energyParticleCenterColor;
            BiomeEnergyParticles.InterpolationSpeed = 0.1f;
            BiomeEnergyParticles.DrawSet(particleDrawCenter + Main.screenPosition);

            Vector2 displacement = Vector2.UnitX.RotatedBy(Main.GlobalTime * 3f) * 2f * (float)Math.Sin(Main.GlobalTime);

            spriteBatch.End();
            spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.Additive);
            spriteBatch.Draw(itemTexture, position + displacement, null, BiomeEnergyParticles.CenterColor, 0f, origin, scale, SpriteEffects.None, 0f);
            spriteBatch.Draw(itemTexture, position - displacement, null, BiomeEnergyParticles.CenterColor, 0f, origin, scale, SpriteEffects.None, 0f);
            spriteBatch.End();
            spriteBatch.Begin(default, default);

            spriteBatch.Draw(itemTexture, position, null, drawColor, 0f, origin, scale, SpriteEffects.None, 0f);
            return false;
        }

        public override bool PreDrawInWorld(SpriteBatch spriteBatch, Color lightColor, Color alphaColor, ref float rotation, ref float scale, int whoAmI)
        {
            Texture2D itemTexture = GetTexture("CalamityMod/Projectiles/Melee/MendedBiomeBlade"); //Use the "projectile" sprite which is flipped so its consistent in lighting with the rest of the line, since its actual sprite is flipped so the swings may look normal 
            spriteBatch.Draw(itemTexture, item.Center - Main.screenPosition, null, lightColor, rotation, item.Size * 0.5f, scale, SpriteEffects.None, 0f);
            return false;
        }

    }

    public class BiomeBladeHoldout : ModProjectile //Visuals
    {
        private Player Owner => Main.player[projectile.owner];
        public bool OwnerOnGround => Main.tile[(int)Owner.Center.X / 16, (int)(Owner.position.Y + (float)Owner.height - 1f) / 16 + 1].IsTileSolidGround() && Main.tile[(int)Owner.Center.X / 16 + Owner.direction, (int)(Owner.position.Y + (float)Owner.height - 1f) / 16 + 1].IsTileSolidGround();
        public bool OwnerCanUseItem => Owner.HeldItem == associatedItem ? (Owner.HeldItem.modItem as TrueBiomeBlade).CanUseItem(Owner) : false;
        public bool OwnerMayChannel => OwnerCanUseItem && Owner.Calamity().mouseRight && Owner.active && !Owner.dead && Owner.StandingStill() && !Owner.mount.Active && OwnerOnGround;
        public ref float ChanneledState => ref projectile.ai[0];
        public ref float ChannelTimer => ref projectile.ai[1];
        public ref float Initialized => ref projectile.localAI[0];

        private Item associatedItem;
        const int ChannelTime = 120;

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Mended Biome Blade");
        }
        public override string Texture => "CalamityMod/Items/Weapons/Melee/TrueBiomeBlade";
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

        public CurveSegment anticipation = new CurveSegment(EasingType.SineOut, 0f, 1f, 0.35f);
        public CurveSegment thrust = new CurveSegment(EasingType.ExpIn, 0.85f, 1.35f, -1.45f);
        public CurveSegment bounceback = new CurveSegment(EasingType.SineOut, 0.95f, -0.1f, 0.1f);
        internal float SwordHeight() => PiecewiseAnimation(ChannelTimer / (float)ChannelTime, new CurveSegment[] { anticipation, thrust, bounceback });

        public override void AI()
        {

            if (Initialized == 0f)
            {
                //If dropped, kill it instantly
                if (Owner.HeldItem.type != ItemType<TrueBiomeBlade>())
                {
                    projectile.Kill();
                    return;
                }

                if (Owner.whoAmI == Main.myPlayer)
                    Main.PlaySound(SoundID.DD2_DarkMageHealImpact);

                associatedItem = Owner.HeldItem;
                //Switch up the attunements
                Attunement temporaryAttunementStorage = (associatedItem.modItem as TrueBiomeBlade).mainAttunement;
                (associatedItem.modItem as TrueBiomeBlade).mainAttunement = (associatedItem.modItem as TrueBiomeBlade).secondaryAttunement;
                (associatedItem.modItem as TrueBiomeBlade).secondaryAttunement = temporaryAttunementStorage;
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

                projectile.Center = Owner.Center + new Vector2(16f * Owner.direction + Owner.direction < 0 ? -26f : 16f, -30 * SwordHeight() + 10f);
                projectile.rotation = MathHelper.PiOver4 + MathHelper.PiOver2; // No more silly turnaround with the repaired one?
                ChannelTimer++;
                projectile.timeLeft = 60;

                if (ChannelTimer >= ChannelTime)
                {
                    Attune((TrueBiomeBlade)associatedItem.modItem);
                    projectile.timeLeft = 120;
                    ChanneledState = 2f; //State where it stays invisible doing nothing. Acts as a cooldown

                    Color particleColor = (associatedItem.modItem as TrueBiomeBlade).mainAttunement.tooltipColor;

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
            }

            if (ChanneledState == 1f)
                projectile.position += Vector2.UnitY * -0.3f * (1f + projectile.timeLeft / 60f);
        }

        public void Attune(TrueBiomeBlade item)
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

            Attunement attunement = Attunement.attunementArray[(int)AttunementID.TrueDefault];

            if (desert || hell)
                attunement = Attunement.attunementArray[(int)AttunementID.TrueHot];
            if (snow)
                attunement = Attunement.attunementArray[(int)AttunementID.TrueCold];
            if (jungle || ocean)
                attunement = Attunement.attunementArray[(int)AttunementID.TrueTropical];
            if (evil)
                attunement = Attunement.attunementArray[(int)AttunementID.TrueEvil];
            if (holy)
                attunement = Attunement.attunementArray[(int)AttunementID.Holy];
            if (astral)
                attunement = Attunement.attunementArray[(int)AttunementID.Astral];
            if (marine)
                attunement = Attunement.attunementArray[(int)AttunementID.Marine];

            //If the owner already had the attunement , break out of it (And unswap)
            if (item.secondaryAttunement == attunement)
            {
                Main.PlaySound(SoundID.DD2_LightningBugZap, projectile.Center);
                item.secondaryAttunement = item.mainAttunement;
                item.mainAttunement = attunement;
                return;
            }

            Main.PlaySound(SoundID.DD2_MonkStaffGroundImpact, projectile.Center);
            item.mainAttunement = attunement;
        }

        public override void Kill(int timeLeft)
        {
            if (associatedItem == null)
            {
                return;
            }
            //If we swapped out the main attunement for the second one despite the second attunement being empty at the time, unswap them.
            if ((associatedItem.modItem as TrueBiomeBlade).mainAttunement == null && (associatedItem.modItem as TrueBiomeBlade).secondaryAttunement != null)
            {
                (associatedItem.modItem as TrueBiomeBlade).mainAttunement = (associatedItem.modItem as TrueBiomeBlade).secondaryAttunement;
                (associatedItem.modItem as TrueBiomeBlade).secondaryAttunement = null;
            }
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
        {
            if (ChanneledState == 0f && ChannelTimer > 6f)
                return base.PreDraw(spriteBatch, lightColor);

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
