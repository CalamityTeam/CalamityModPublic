using Terraria.DataStructures;
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
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;
using static Terraria.ModLoader.ModContent;

namespace CalamityMod.Items.Weapons.Melee
{
    // TODO -- CANNOT RENAME this and True Biome Blade to "TrueBiomeBlade" and "BiomeBlade" internally without corrupting existing items
    public class OmegaBiomeBlade : ModItem, ILocalizedModType
    {
        public new string LocalizationCategory => "Items.Weapons.Melee";
        public Attunement mainAttunement = null;
        public Attunement secondaryAttunement = null;
        public Projectile MeatHook;

        //Used for passive effects
        public int UseTimer = 0;
        public bool OnHitProc = false;

        #region stats
        public static int BaseDamage = 200;

        public static int WhirlwindAttunement_BaseDamage = 140;
        public static int WhirlwindAttunement_LocalIFrames = 20; //Remember its got one extra update
        public static int WhirlwindAttunement_SigilTime = 1000;
        public static float WhirlwindAttunement_BeamDamageReduction = 0.5f;
        public static float WhirlwindAttunement_BaseDamageReduction = 0.3f;
        public static float WhirlwindAttunement_FullChargeDamageBoost = 0.9f;
        public static float WhirlwindAttunement_ThrowDamageBoost = 3.3f;

        public static int WhirlwindAttunement_PassiveBaseDamage = 200;


        public static int SuperPogoAttunement_BaseDamage = 200;
        public static int SuperPogoAttunement_FullChargeDamage = 380;
        public static int SuperPogoAttunement_ShredIFrames = 10;
        public static int SuperPogoAttunement_LocalIFrames = 30; //Be warned its got one extra update so all the iframes should be divided in 2
        public static int SuperPogoAttunement_LocalIFramesCharged = 16;
        public static float SuperPogoAttunement_SlashDamageBoost = 2.5f; //Keep in mind the slice always crits
        public static int SuperPogoAttunementSlashLifesteal = 4;
        public static int SuperPogoAttunement_SlashIFrames = 20;
        public static float SuperPogoAttunement_ShotDamageBoost = 2.5f;
        public static float SuperPogoAttunement_ShredDecayRate = 0.65f;//How much charge is lost per frame.

        public static int SuperPogoAttunement_PassiveLifeSteal = 7;


        public static int ShockwaveAttunement_BaseDamage = 550;
        public static int ShockwaveAttunement_DashHitIFrames = 20;
        public static float ShockwaveAttunement_FullChargeBoost = 2.5f; //The EXTRA damage boost. So putting 1 here will make it deal double damage. Putting 0.5 here will make it deal 1.5x the damage.
        public static float ShockwaveAttunement_MonolithDamageBoost = 1.1f;
        public static float ShockwaveAttunement_MonolithDamageFalloff = 0.15f; //Damage multiplier for all subsequent hits after the first one.
        public static float ShockwaveAttunement_BlastDamageReduction = 0.8f;

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

        #region tooltip editing
        public override void ModifyTooltips(List<TooltipLine> list)
        {
            if (list == null)
                return;

            SafeCheckAttunements();

            Player player = Main.player[Main.myPlayer];
            if (player is null)
                return;


            var effectDescTooltip = list.FirstOrDefault(x => x.Text.Contains("[FUNC]") && x.Mod == "Terraria");
            var passiveDescTooltip = list.FirstOrDefault(x => x.Text.Contains("[PASS]") && x.Mod == "Terraria");
            var mainAttunementTooltip = list.FirstOrDefault(x => x.Text.Contains("[ATT1]") && x.Mod == "Terraria");
            var secondaryAttunementTooltip = list.FirstOrDefault(x => x.Text.Contains("[ATT2]") && x.Mod == "Terraria");

            //Default stuff
            if (effectDescTooltip != null)
            {
                effectDescTooltip.Text = this.GetLocalizedValue("DefaultFunction");
                effectDescTooltip.OverrideColor = new Color(163, 163, 163);
            }

            if (passiveDescTooltip != null)
            {
                passiveDescTooltip.Text = this.GetLocalizedValue("DefaultPassive");
                passiveDescTooltip.OverrideColor = new Color(163, 163, 163);
            }

            //If theres a main attunement
            if (mainAttunement != null)
            {
                if (effectDescTooltip != null)
                {
                    effectDescTooltip.Text = Lang.SupportGlyphs(mainAttunement.FunctionText.ToString());
                    effectDescTooltip.OverrideColor = mainAttunement.tooltipColor;
                }

                if (mainAttunementTooltip != null)
                {
                    mainAttunementTooltip.Text = mainAttunementTooltip.Text.Replace("ATT1", mainAttunement.AttunementName.ToString());
                    mainAttunementTooltip.OverrideColor = Color.Lerp(mainAttunement.tooltipColor, mainAttunement.tooltipColor2, 0.5f + (float)Math.Sin(Main.GlobalTimeWrappedHourly) * 0.5f);
                }
            }
            else if (mainAttunementTooltip != null)
            {
                mainAttunementTooltip.Text = mainAttunementTooltip.Text.Replace("ATT1", Language.GetTextValue("LegacyInterface.23"));
                mainAttunementTooltip.OverrideColor = new Color(163, 163, 163);
            }

            //If theres a secondary attunement
            if (secondaryAttunement != null)
            {
                if (passiveDescTooltip != null)
                {
                    passiveDescTooltip.Text = secondaryAttunement.PassiveDesc.ToString();
                    passiveDescTooltip.OverrideColor = secondaryAttunement.tooltipColor;
                }

                if (secondaryAttunementTooltip != null)
                {
                    secondaryAttunementTooltip.Text = secondaryAttunementTooltip.Text.Replace("ATT2", secondaryAttunement.AttunementName.ToString());
                    secondaryAttunementTooltip.OverrideColor = Color.Lerp(Color.Lerp(secondaryAttunement.tooltipColor, secondaryAttunement.tooltipColor2, 0.5f + (float)Math.Sin(Main.GlobalTimeWrappedHourly) * 0.5f), Color.Gray, 0.5f);
                }
            }
            else if (secondaryAttunementTooltip != null)
            {
                secondaryAttunementTooltip.Text = secondaryAttunementTooltip.Text.Replace("ATT2", Language.GetTextValue("LegacyInterface.23"));
                secondaryAttunementTooltip.OverrideColor = new Color(163, 163, 163);
            }
        }

    #endregion

        public override void SetDefaults()
        {
            Item.width = Item.height = 92;
            Item.damage = BaseDamage;
            Item.DamageType = DamageClass.Melee;
            Item.useAnimation = 18;
            Item.useTime = 18;
            Item.useTurn = true;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.knockBack = 8;
            Item.UseSound = SoundID.Item1;
            Item.autoReuse = true;
            Item.value = CalamityGlobalItem.Rarity9BuyPrice;
            Item.rare = ItemRarityID.Cyan;
            Item.shoot = ProjectileID.PurificationPowder;
            Item.shootSpeed = 15f;
        }

        #region Saving and syncing attunements
        public override ModItem Clone(Item item)
        {
            var clone = base.Clone(item);
            if (Main.mouseItem.type == ItemType<OmegaBiomeBlade>())
                item.ModItem?.HoldItem(Main.player[Main.myPlayer]);
            if (clone is OmegaBiomeBlade a && item.ModItem is OmegaBiomeBlade a2)
            {
                a.mainAttunement = a2.mainAttunement;
                a.secondaryAttunement = a2.secondaryAttunement;
            }

            return clone;
        }

        public override void SaveData(TagCompound tag)
        {
            int attunement1 = mainAttunement == null ? -1 : (int)mainAttunement.id;
            int attunement2 = secondaryAttunement == null ? -1 : (int)secondaryAttunement.id;
            tag["mainAttunement"] = attunement1;
            tag["secondaryAttunement"] = attunement2;
        }

        public override void LoadData(TagCompound tag)
        {
            int attunement1 = tag.GetInt("mainAttunement");
            int attunement2 = tag.GetInt("secondaryAttunement");

            mainAttunement = Attunement.attunementArray[attunement1 != -1 ? attunement1 : Attunement.attunementArray.Length - 1];
            secondaryAttunement = Attunement.attunementArray[attunement2 != -1 ? attunement2 : Attunement.attunementArray.Length - 1];

            if (mainAttunement == secondaryAttunement)
                secondaryAttunement = null;

            SafeCheckAttunements();
        }

        public override void NetSend(BinaryWriter writer)
        {
            writer.Write(mainAttunement != null ? (byte)mainAttunement.id : Attunement.attunementArray.Length - 1);
            writer.Write(secondaryAttunement != null ? (byte)secondaryAttunement.id : Attunement.attunementArray.Length - 1);
        }

        public override void NetReceive(BinaryReader reader)
        {
            mainAttunement = Attunement.attunementArray[reader.ReadInt32()];
            secondaryAttunement = Attunement.attunementArray[reader.ReadInt32()];
        }

        #endregion

        public override void ModifyWeaponDamage(Player player, ref StatModifier damage)
        {
            if (mainAttunement == null)
                return;

            damage += (mainAttunement?.DamageMultiplier ?? 1f) - 1f;
        }

        public void SafeCheckAttunements()
        {
            if (mainAttunement != null)
                mainAttunement = Attunement.attunementArray[(int)MathHelper.Clamp((float)mainAttunement.id, (float)AttunementID.Whirlwind, (float)AttunementID.Shockwave)];

            if (secondaryAttunement != null)
                secondaryAttunement = Attunement.attunementArray[(int)MathHelper.Clamp((float)secondaryAttunement.id, (float)AttunementID.Whirlwind, (float)AttunementID.Shockwave)];
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
                Item.noUseGraphic = false;
                Item.useStyle = ItemUseStyleID.Swing;
                Item.noMelee = false;
                Item.channel = false;
                Item.shoot = ProjectileID.PurificationPowder;
                Item.shootSpeed = 12f;
                Item.UseSound = SoundID.Item1;
            }

            else
                mainAttunement.ApplyStats(Item);


            if (player.whoAmI != Main.myPlayer)
                return;


            //PAssive effetcsts only happen on the side of the owner
            var source = player.GetSource_ItemUse(Item);
            if (secondaryAttunement != null)
            {
                if (secondaryAttunement.id != AttunementID.FlailBlade || MeatHook == null || !MeatHook.active)
                    MeatHook = null;

                if (secondaryAttunement.id == AttunementID.FlailBlade && MeatHook == null)
                {
                    int damage = (int)player.GetTotalDamage<MeleeDamageClass>().ApplyTo(FlailBladeAttunement_PassiveBaseDamage);
                    MeatHook = Projectile.NewProjectileDirect(source, player.Center, Vector2.Zero, ProjectileType<ChainedMeatHook>(), damage, 0f, player.whoAmI);
                }

                secondaryAttunement.PassiveEffect(player, source, ref UseTimer, ref OnHitProc, MeatHook);
            }


            if (player.Calamity().mouseRight && CanUseItem(player) && player.whoAmI == Main.myPlayer && !Main.mapFullscreen)
            {
                //Don't shoot out a visual blade if you already have one out
                if (Main.projectile.Any(n => n.active && n.type == ProjectileType<TrueBiomeBladeHoldout>() && n.owner == player.whoAmI))
                    return;

                Projectile.NewProjectile(source, player.Top, Vector2.Zero, ProjectileType<TrueBiomeBladeHoldout>(), 0, 0, player.whoAmI);
            }
        }

        public override void UpdateInventory(Player player)
        {
            SafeCheckAttunements();
        }

        public override bool CanUseItem(Player player)
        {
            return !Main.projectile.Any(n => n.active && n.owner == player.whoAmI &&
            (n.type == ProjectileType<SwordsmithsPride>() ||
             n.type == ProjectileType<MercurialTides>() ||
             n.type == ProjectileType<SanguineFury>() ||
             n.type == ProjectileType<LamentationsOfTheChained>()));
        }

        //No need for any wacky zany hijinx in the shoot method for once??? damn
        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback) => mainAttunement == null ? false : true;

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

            Texture2D itemTexture = Request<Texture2D>("CalamityMod/Items/Weapons/Melee/OmegaBiomeBladeExtra").Value;
            Rectangle itemFrame = (Main.itemAnimations[Item.type] == null) ? itemTexture.Frame() : Main.itemAnimations[Item.type].GetFrame(itemTexture);

            // Draw all particles.

            Vector2 particleDrawCenter = position + new Vector2(12f, 16f) * Main.inventoryScale - frame.Size() * 0.12f;

            BiomeEnergyParticles.EdgeColor = mainAttunement.tooltipColor2;
            BiomeEnergyParticles.CenterColor = mainAttunement.tooltipColor;
            BiomeEnergyParticles.InterpolationSpeed = 0.1f;
            BiomeEnergyParticles.DrawSet(particleDrawCenter + Main.screenPosition);

            Vector2 displacement = Vector2.UnitX.RotatedBy(Main.GlobalTimeWrappedHourly * 3f) * 2f * (float)Math.Sin(Main.GlobalTimeWrappedHourly);

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
            Texture2D itemTexture = Request<Texture2D>("CalamityMod/Items/Weapons/Melee/OmegaBiomeBladeExtra").Value;
            spriteBatch.Draw(itemTexture, Item.Center - Main.screenPosition, null, lightColor, rotation, Item.Size * 0.5f, scale, SpriteEffects.None, 0f);
            return false;
        }

        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient<TrueBiomeBlade>().
                AddIngredient<CoreofCalamity>().
                AddIngredient<AstralBar>(3).
                AddIngredient<LifeAlloy>(3).
                AddTile(TileID.LunarCraftingStation).
                Register();
        }
    }
}
