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
        public static int BaseDamage = 400;

        public static int WhirlwindAttunement_BaseDamage = 400;
        public static int WhirlwindAttunement_LocalIFrames = 20; //Remember its got one extra update
        public static int WhirlwindAttunement_SigilTime = 1200;
        public static float WhirlwindAttunement_BeamDamageReduction = 0.5f;
        public static float WhirlwindAttunement_BaseDamageReduction = 0.2f;
        public static float WhirlwindAttunement_FullChargeDamageBoost = 2.4f;
        public static float WhirlwindAttunement_ThrowDamageBoost = 3f;

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
        public static float ShockwaveAttunement_FullChargeBoost = 5f; //The EXTRA damage boost. So putting 1 here will make it deal double damage. Putting 0.5 here will make it deal 1.5x the damage.
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
                if (l.text == null)
                    continue;

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
                    continue;
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
                    continue;
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
                    continue;
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
                    continue;
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
                    continue;
                }
            }
        }

    #endregion

        public override void SetDefaults()
        {
            item.width = item.height = 92;
            item.damage = BaseDamage;
            item.melee = true;
            item.useAnimation = 18;
            item.useTime = 18;
            item.useTurn = true;
            item.useStyle = ItemUseStyleID.SwingThrow;
            item.knockBack = 8;
            item.UseSound = SoundID.Item1;
            item.autoReuse = true;
            item.value = CalamityGlobalItem.Rarity9BuyPrice;
            item.rare = ItemRarityID.Cyan;
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

        public override void ModifyWeaponDamage(Player player, ref float add, ref float mult, ref float flat)
        {
            if (mainAttunement == null)
                return;

            mult += mainAttunement.DamageMultiplier - 1;
        }

        public override void HoldItem(Player player)
        {
            player.Calamity().rightClickListener = true;
            player.Calamity().mouseWorldListener = true;

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
                item.noMelee = false;
                item.channel = false;
                item.shoot = ProjectileID.PurificationPowder;
                item.shootSpeed = 12f;
                item.UseSound = SoundID.Item1;
            }

            else
            {
                if (mainAttunement.id < AttunementID.Whirlwind || mainAttunement.id > AttunementID.Shockwave)
                    mainAttunement = Attunement.attunementArray[(int)MathHelper.Clamp((float)mainAttunement.id, (float)AttunementID.Whirlwind, (float)AttunementID.Shockwave)];

                mainAttunement.ApplyStats(item);
            }


            if (player.whoAmI != Main.myPlayer)
                return;


            //PAssive effetcsts only happen on the side of the owner
            if (secondaryAttunement != null)
            {
                if (secondaryAttunement.id != AttunementID.FlailBlade || MeatHook == null || !MeatHook.active)
                    MeatHook = null;

                if (secondaryAttunement.id == AttunementID.FlailBlade && MeatHook == null)
                {
                    MeatHook = Projectile.NewProjectileDirect(player.Center, Vector2.Zero, ProjectileType<ChainedMeatHook>(), (int)(FlailBladeAttunement_PassiveBaseDamage * player.MeleeDamage()), 0f, player.whoAmI);
                }

                secondaryAttunement.PassiveEffect(player, ref UseTimer, ref OnHitProc, MeatHook);
            }


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
            Main.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.Additive, null, null, null, null, Main.UIScaleMatrix);
            spriteBatch.Draw(itemTexture, position + displacement, itemFrame, BiomeEnergyParticles.CenterColor, 0f, origin, scale, SpriteEffects.None, 0f);
            spriteBatch.Draw(itemTexture, position - displacement, itemFrame, BiomeEnergyParticles.CenterColor, 0f, origin, scale, SpriteEffects.None, 0f);
            spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Deferred, null, null, null, null, null, Main.UIScaleMatrix);

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
}
