using CalamityMod.Items.Materials;
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
using static Terraria.ModLoader.ModContent;

namespace CalamityMod.Items.Weapons.Melee
{
    public class TrueBiomeBlade : ModItem
    {
        public Attunement mainAttunement = null;
        public Attunement secondaryAttunement = null;
        public int Combo = 0;
        public float ComboResetTimer = 0f;
        public int StoredLunges = 2;
        public int PowerLungeCounter = 0;

        #region stats
        public static int DefaultAttunement_BaseDamage = 160;
        public static int DefaultAttunement_SigilTime = 1200;
        public static int DefaultAttunement_BeamTime = 60;
        public static float DefaultAttunement_HomingAngle = MathHelper.PiOver4;

        public static int EvilAttunement_BaseDamage = 320;
        public static int EvilAttunement_Lifesteal = 4;
        public static int EvilAttunement_BounceIFrames = 10;
        public static float EvilAttunement_SlashDamageBoost = 3f;
        public static int EvilAttunement_SlashIFrames = 60;

        public static int ColdAttunement_BaseDamage = 200;
        public static float ColdAttunement_SecondSwingBoost = 1.2f;
        public static float ColdAttunement_ThirdSwingBoost = 1.6f;
        public static float ColdAttunement_MistDamageReduction = 0.04f;

        public static int HotAttunement_BaseDamage = 320;
        public static int HotAttunement_ShredIFrames = 8;
        public static float HotAttunement_ShotDamageBoost = 3.5f;
        public static int HotAttunement_LocalIFrames = 30; //Be warned its got one extra update so all the iframes should be divided in 2
        public static int HotAttunement_LocalIFramesCharged = 16;

        public static int TropicalAttunement_BaseDamage = 160;
        public static float TropicalAttunement_ChainDamageReduction = 0.5f;
        public static float TropicalAttunement_VineDamageReduction = 0.5f;
        public static int TropicalAttunement_LocalIFrames = 60; //Be warned its got 2 extra updates so all the iframes should be divided in 3

        public static int HolyAttunement_BaseDamage = 160;
        public static float HolyAttunement_BaseDamageReduction = 0.2f;
        public static float HolyAttunement_FullChargeDamageBoost = 2.4f;
        public static float HolyAttunement_ThrowDamageBoost = 3f;
        public static int HolyAttunement_LocalIFrames = 16; //Be warned its got 1 extra update yadda yadda

        public static int AstralAttunement_BaseDamage = 500;
        public static int AstralAttunement_DashHitIFrames = 30;
        public static float AstralAttunement_FullChargeBoost = 5f; //The EXTRA damage boost. So putting 1 here will make it deal double damage. Putting 0.5 here will make it deal 1.5x the damage.
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
            recipe.AddIngredient(ItemID.SoulofFright, 1);
            recipe.AddIngredient(ItemID.SoulofMight, 1);
            recipe.AddIngredient(ItemID.SoulofSight, 1);
            recipe.AddIngredient(ItemID.PixieDust, 2);
            recipe.AddIngredient(ItemType<Stardust>(), 10);
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
            player.Calamity().mouseWorldListener = true;

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
            {
                if (mainAttunement.id < AttunementID.TrueDefault || mainAttunement.id > AttunementID.Marine)
                    mainAttunement = Attunement.attunementArray[(int)MathHelper.Clamp((float)mainAttunement.id, (float)AttunementID.TrueDefault, (float)AttunementID.Marine)];
                mainAttunement.ApplyStats(item);
            }

            if (mainAttunement != null && mainAttunement.id != AttunementID.TrueCold && mainAttunement.id != AttunementID.TrueTropical)
                Combo = 0;

            if (player.Calamity().mouseRight && CanUseItem(player) && player.whoAmI == Main.myPlayer && !Main.mapFullscreen)
            {
                //Don't shoot out a visual blade if you already have one out
                if (Main.projectile.Any(n => n.active && n.type == ProjectileType<BiomeBladeHoldout>() && n.owner == player.whoAmI))
                    return;

                bool mayAttune = player.StandingStill() && !player.mount.Active && player.CheckSolidGround(1, 3);
                Vector2 displace = new Vector2(18f, 0f);
                Projectile.NewProjectile(player.Top + displace, Vector2.Zero, ProjectileType<BiomeBladeHoldout>(), 0, 0, player.whoAmI, mayAttune ? 0f : 1f);
            }
        }

        public override void UpdateInventory(Player player)
        {
            if (mainAttunement != null && mainAttunement.id == AttunementID.TrueCold && CanUseItem(player))
                ComboResetTimer -= 0.02f; //Make the combo counter get closer to being reset

            if (ComboResetTimer < 0)
                Combo = 0;
        }

        public override bool CanUseItem(Player player)
        {
            return !Main.projectile.Any(n => n.active && n.owner == player.whoAmI &&
            (n.type == ProjectileType<TrueBitingEmbrace>() || 
             n.type == ProjectileType<TrueGrovetendersTouch>() ||
             n.type == ProjectileType<TrueAridGrandeur>() ||
             n.type == ProjectileType<HeavensMight>() ||
             n.type == ProjectileType<ExtantAbhorrence>() ||
             n.type == ProjectileType<GestureForTheDrowned>()));  
        }

        public override bool Shoot(Player player, ref Vector2 position, ref float speedX, ref float speedY, ref int type, ref int damage, ref float knockBack)
        {
            if (mainAttunement == null)
                return false;

            ComboResetTimer = 1f;
            return mainAttunement.Shoot(player, ref position, ref speedX, ref speedY, ref type, ref damage, ref knockBack, ref Combo, ref StoredLunges, ref PowerLungeCounter);
        }


        //This is only used for the purity sigil effect of the default attunement
        public override void OnHitNPC(Player player, NPC target, int damage, float knockBack, bool crit)
        {
            if (mainAttunement == null || mainAttunement.id != AttunementID.TrueDefault || player.whoAmI != Main.myPlayer)
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
            Main.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.Additive, null, null, null, null, Main.UIScaleMatrix);
            spriteBatch.Draw(itemTexture, position + displacement, null, BiomeEnergyParticles.CenterColor, 0f, origin, scale, SpriteEffects.None, 0f);
            spriteBatch.Draw(itemTexture, position - displacement, null, BiomeEnergyParticles.CenterColor, 0f, origin, scale, SpriteEffects.None, 0f);
            spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Deferred, null, null, null, null, null, Main.UIScaleMatrix);

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
}
