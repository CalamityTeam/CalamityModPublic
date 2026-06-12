using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using CalamityMod.DataStructures;
using CalamityMod.Items.BaseItems;
using CalamityMod.Items.Materials;
using CalamityMod.Particles;
using CalamityMod.Projectiles.Melee;
using CalamityMod.Systems;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;
using static Terraria.ModLoader.ModContent;

namespace CalamityMod.Items.Weapons.Melee
{
    [LegacyName("BiomeBlade")]
    public class BrokenBiomeBlade : CustomUseProjItem, ILocalizedModType
    {
        public new string LocalizationCategory => "Items.Weapons.Melee";
        public Attunement mainAttunement = null;
        public Attunement secondaryAttunement = null;
        public int Combo = 0;
        public float ComboResetTimer = 0f;
        public int CanLunge = 1;

        #region stats
        public static int BaseDamage = 38;

        public static int DefaultAttunement_BaseDamage = 38;

        public static int EvilAttunement_BaseDamage = 50;
        public static int EvilAttunement_Lifesteal = 2;
        public static int EvilAttunement_BounceIFrames = 10;

        public static int ColdAttunement_BaseDamage = 40;
        public static float ColdAttunement_ThirdSwingBoost = 1.15f;

        public static int HotAttunement_BaseDamage = 42;
        public static int HotAttunement_ShredPlayerIFrames = 6;
        public static int HotAttunement_LocalIFrames = 30; //Be warned its got one extra update so all the iframes should be divided in 2
        public static float HotAttunement_ShredDecayRate = 1f; //How much charge is lost per frame.
        #endregion

        public override void ModifyTooltips(List<TooltipLine> list)
        {
            if (list == null)
                return;

            SafeCheckAttunements();

            Player player = Main.LocalPlayer;
            if (player is null)
                return;

            var effectDescTooltip = list.FirstOrDefault(x => x.Text.Contains("[FUNC]") && x.Mod == "Terraria");
            var mainAttunementTooltip = list.FirstOrDefault(x => x.Text.Contains("[ATT1]") && x.Mod == "Terraria");
            var secondaryAttunementTooltip = list.FirstOrDefault(x => x.Text.Contains("[ATT2]") && x.Mod == "Terraria");


            //Default stuff
            if (effectDescTooltip != null)
            {
                effectDescTooltip.Text = this.GetLocalizedValue("DefaultFunction");
                effectDescTooltip.OverrideColor = new Color(163, 163, 163);
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
                    mainAttunementTooltip.OverrideColor = mainAttunement.tooltipColor;
                }
            }
            else if (mainAttunementTooltip != null)
            {
                mainAttunementTooltip.Text = mainAttunementTooltip.Text.Replace("ATT1", Language.GetTextValue("LegacyInterface.23"));
                mainAttunementTooltip.OverrideColor = new Color(163, 163, 163);
            }

            //If theres a secondary attunement
            if (secondaryAttunement != null && secondaryAttunementTooltip != null)
            {
                secondaryAttunementTooltip.Text = secondaryAttunementTooltip.Text.Replace("ATT2", secondaryAttunement.AttunementName.ToString());
                secondaryAttunementTooltip.OverrideColor = Color.Lerp(secondaryAttunement.tooltipColor, Color.Gray, 0.5f);
            }
            else if (secondaryAttunementTooltip != null)
            {
                secondaryAttunementTooltip.Text = secondaryAttunementTooltip.Text.Replace("ATT2", Language.GetTextValue("LegacyInterface.23"));
                secondaryAttunementTooltip.OverrideColor = new Color(163, 163, 163);
            }
        }

        public override void SetDefaults()
        {
            Item.width = Item.height = 36;
            Item.damage = BaseDamage;
            Item.DamageType = DamageClass.Melee;
            Item.useAnimation = 30;
            Item.useTime = 30;
            Item.useTurn = true;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.shoot = ProjectileID.PurificationPowder;
            Item.knockBack = 5f;
            Item.autoReuse = true;
            Item.value = CalamityGlobalItem.RarityOrangeBuyPrice;
            Item.rare = ItemRarityID.Orange;
            Item.shootSpeed = 12f;
        }

        #region Saving and syncing attunements
        public override ModItem Clone(Item item)
        {
            var clone = base.Clone(item);

            if (Main.mouseItem.type == ItemType<BrokenBiomeBlade>())
                item.ModItem?.HoldItem(Main.LocalPlayer);

            if (clone is BrokenBiomeBlade a && item.ModItem is BrokenBiomeBlade a2)
            {
                a.mainAttunement = a2.mainAttunement;
                a.secondaryAttunement = a2.secondaryAttunement;
            }

            //As funny as a Broken Broken Biome Blade would be, its also quite funny to make it turn into that. This is only done for a new instance of the item since the goblin tinkerer changes prevent it from happening through reforging
            if (clone.Item.prefix == PrefixID.Broken)
            {
                clone.Item.Prefix(PrefixID.Legendary);
                clone.Item.prefix = PrefixID.Legendary;
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

            mainAttunement = AttunementSystem.FindOrNull(attunement1);
            secondaryAttunement = AttunementSystem.FindOrNull(attunement2);

            if (mainAttunement == secondaryAttunement)
                secondaryAttunement = null;

            SafeCheckAttunements();
        }

        public override void NetSend(BinaryWriter writer)
        {
            writer.Write(mainAttunement != null ? (byte)mainAttunement.id : AttunementSystem.EmptyID);
            writer.Write(secondaryAttunement != null ? (byte)secondaryAttunement.id : AttunementSystem.EmptyID);
        }

        public override void NetReceive(BinaryReader reader)
        {
            mainAttunement = AttunementSystem.FindOrNull(reader.ReadInt32());
            secondaryAttunement = AttunementSystem.FindOrNull(reader.ReadInt32());
        }
        #endregion

        public override void ModifyWeaponDamage(Player player, ref StatModifier damage)
        {
            damage += (mainAttunement?.DamageMultiplier ?? 1f) - 1f;
        }

        public void SafeCheckAttunements()
        {
            if (mainAttunement != null)
                mainAttunement = AttunementSystem.FindOrNull(ClampAttunementRange((int)mainAttunement.id));

            if (secondaryAttunement != null)
                secondaryAttunement = AttunementSystem.FindOrNull(ClampAttunementRange((int)secondaryAttunement.id));

            if (mainAttunement == secondaryAttunement)
                secondaryAttunement = null;
        }

        private static int ClampAttunementRange(int input)
        {
            if (input < (int)AttunementID.Default)
                return (int)AttunementID.Default;

            if (input > (int)AttunementID.Evil)
                return (int)AttunementID.Evil;

            return input;
        }

        public override void HoldItem(Player player)
        {
            var source = player.GetSource_ItemUse(Item);
            player.Calamity().rightClickListener = true;
            player.Calamity().mouseWorldListener = true;

            if (player.velocity.Y == 0) //Reset the lunge ability on ground contact
                CanLunge = 1;

            //Change the swords function based on its attunement
            if (mainAttunement == null)
            {
                Item.noUseGraphic = false;
                Item.useStyle = ItemUseStyleID.Swing;
                Item.noMelee = false;
                Item.channel = false;
                Item.shoot = ProjectileID.PurificationPowder;
                Item.shootSpeed = 12f;
                Item.UseSound = SoundID.Item1;
                Combo = 0;
            }

            else
                mainAttunement.ApplyStats(Item);

            if (mainAttunement != null && mainAttunement.id != AttunementID.Cold)
                Combo = 0;

            if (player.Calamity().mouseRight && CanUseItem(player) && player.whoAmI == Main.myPlayer && !Main.mapFullscreen && !Main.blockMouse)
            {
                //Don't shoot out a visual blade if you already have one out
                if (Main.projectile.Any(n => n.active && n.type == ProjectileType<BrokenBiomeBladeHoldout>() && n.owner == player.whoAmI))
                    return;


                bool mayAttune = player.StandingStill() && !player.mount.Active && player.CheckSolidGround(1, 3);
                Vector2 displace = new Vector2(18f, 0f);
                Projectile.NewProjectile(source, player.Top + displace, Vector2.Zero, ProjectileType<BrokenBiomeBladeHoldout>(), 0, 0, player.whoAmI, mayAttune ? 0f : 1f);
            }
        }

        public override void UpdateInventory(Player player)
        {
            SafeCheckAttunements();

            if (mainAttunement != null && mainAttunement.id == AttunementID.Cold && CanUseItem(player))
                ComboResetTimer -= 0.02f; //Make the combo counter get closer to being reset

            if (ComboResetTimer < 0)
                Combo = 0;
        }

        public override bool CanUseItem(Player player)
        {
            bool isRightClicking = player.altFunctionUse != ItemAlternativeFunctionID.None;
            return !isRightClicking && !Main.projectile.Any(n => n.active && n.owner == player.whoAmI &&
            (n.type == ProjectileType<BitingEmbrace>() ||
             n.type == ProjectileType<AridGrandeur>()));
        }

        // 03FEB2024: Ozzatron: added so the Iban Blades don't break Overhaul compatibility. Weapons are functionally unchanged.
        public override bool AltFunctionUse(Player player) => true;

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            if (mainAttunement == null || player.altFunctionUse != ItemAlternativeFunctionID.None)
                return false;

            int powerLungeCounter = 0; //Unused here
            ComboResetTimer = 1f;
            return mainAttunement.Shoot(player, source, ref position, ref velocity.X, ref velocity.Y, ref type, ref damage, ref knockback, ref Combo, ref CanLunge, ref powerLungeCounter);
        }

        internal static ChargingEnergyParticleSet BiomeEnergyParticles = new ChargingEnergyParticleSet(-1, 2, Color.DarkViolet, Color.White, 0.04f, 20f);
        internal static void UpdateAllParticleSets()
        {
            BiomeEnergyParticles.Update();
        }

        public override bool PreDrawInInventory(SpriteBatch spriteBatch, Vector2 position, Rectangle frame, Color drawColor, Color itemColor, Vector2 origin, float scale)
        {
            Texture2D itemTexture = TextureAssets.Item[Type].Value;
            Rectangle itemFrame = (Main.itemAnimations[Type] == null) ? itemTexture.Frame() : Main.itemAnimations[Type].GetFrame(itemTexture);

            if (mainAttunement == null)
                return true;

            // Draw all particles.

            Vector2 particleDrawCenter = position + new Vector2(12f, 16f) * Main.inventoryScale - frame.Size() * 0.3f;

            BiomeEnergyParticles.EdgeColor = mainAttunement.energyParticleEdgeColor;
            BiomeEnergyParticles.CenterColor = mainAttunement.energyParticleCenterColor;
            BiomeEnergyParticles.InterpolationSpeed = 0.1f;
            BiomeEnergyParticles.DrawSet(particleDrawCenter + Main.screenPosition);

            Vector2 displacement = Vector2.UnitX.RotatedBy(Main.GlobalTimeWrappedHourly * 3f) * 2f * (float)Math.Sin(Main.GlobalTimeWrappedHourly);

            spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.Additive, null, null, null, null, Main.UIScaleMatrix);

            spriteBatch.Draw(itemTexture, position + displacement, itemFrame, BiomeEnergyParticles.CenterColor, 0f, origin, scale, SpriteEffects.None, 0f);
            spriteBatch.Draw(itemTexture, position - displacement, itemFrame, BiomeEnergyParticles.CenterColor, 0f, origin, scale, SpriteEffects.None, 0f);

            spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Deferred, null, null, null, null, null, Main.UIScaleMatrix);


            return true;
        }

        public override void AddRecipes()
        {
            CreateRecipe().
                AddRecipeGroup("AnyWoodenSword").
                AddIngredient(ItemType<AerialiteBar>(), 10).
                AddIngredient(ItemID.HellstoneBar, 10).
                AddIngredient(ItemID.DirtBlock, 50).
                AddRecipeGroup("AnyStoneBlock", 50).
                AddTile(TileID.Anvils).
                Register();
        }
    }
}


