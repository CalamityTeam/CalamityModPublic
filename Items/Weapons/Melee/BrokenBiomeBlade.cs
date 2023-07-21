using Terraria.DataStructures;
using CalamityMod.DataStructures;
using CalamityMod.Items.Materials;
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
using static Terraria.ModLoader.ModContent;
using Terraria.ModLoader.IO;
using Terraria.GameContent;

namespace CalamityMod.Items.Weapons.Melee
{
    [LegacyName("BiomeBlade")]
    public class BrokenBiomeBlade : ModItem, ILocalizedModType
    {
        public new string LocalizationCategory => "Items.Weapons.Melee";
        public Attunement mainAttunement = null;
        public Attunement secondaryAttunement = null;
        public int Combo = 0;
        public float ComboResetTimer = 0f;
        public int CanLunge = 1;

        #region stats
        public static int BaseDamage = 47;

        public static int DefaultAttunement_BaseDamage = 47;

        public static int EvilAttunement_BaseDamage = 63;
        public static int EvilAttunement_Lifesteal = 2;
        public static int EvilAttunement_BounceIFrames = 10;

        public static int ColdAttunement_BaseDamage = 58;
        public static float ColdAttunement_SecondSwingBoost = 1.14f;
        public static float ColdAttunement_ThirdSwingBoost = 1.4f;

        public static int HotAttunement_BaseDamage = 70;
        public static int HotAttunement_FullChargeDamage = 105;
        public static int HotAttunement_ShredIFrames = 8;
        public static int HotAttunement_LocalIFrames = 30; //Be warned its got one extra update so all the iframes should be divided in 2
        public static int HotAttunement_LocalIFramesCharged = 16;
        public static float HotAttunement_ShredDecayRate = 0.7f; //How much charge is lost per frame.

        public static int TropicalAttunement_BaseDamage = 65;
        public static float TropicalAttunement_ChainDamageReduction = 0.6f;
        public static float TropicalAttunement_SweetSpotDamageMultiplier = 1.2f; //It also crits, so be mindful of that
        public static int TropicalAttunement_LocalIFrames = 60; //Be warned its got 2 extra updates so all the iframes should be divided in 3
        #endregion

        public override void SetStaticDefaults()
        {
            //Theres potential for flavor text as well but im not a writer
        }

        public override void ModifyTooltips(List<TooltipLine> list)
        {
            if (list == null)
                return;

            SafeCheckAttunements();

            Player player = Main.player[Main.myPlayer];
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
            Item.value = CalamityGlobalItem.Rarity3BuyPrice;
            Item.rare = ItemRarityID.Orange;
            Item.shootSpeed = 12f;
        }

        #region Saving and syncing attunements
        public override ModItem Clone(Item item)
        {
            var clone = base.Clone(item);

            if (Main.mouseItem.type == ItemType<BrokenBiomeBlade>())
                item.ModItem?.HoldItem(Main.player[Main.myPlayer]);

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
            int attunement1 = mainAttunement == null? -1 : (int)mainAttunement.id;
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
            damage += (mainAttunement?.DamageMultiplier ?? 1f) - 1f;
        }

        public void SafeCheckAttunements()
        {
            if (mainAttunement != null)
                mainAttunement = Attunement.attunementArray[(int)MathHelper.Clamp((float)mainAttunement.id, (float)AttunementID.Default, (float)AttunementID.Evil)];

            if (secondaryAttunement != null)
                secondaryAttunement = Attunement.attunementArray[(int)MathHelper.Clamp((float)secondaryAttunement.id, (float)AttunementID.Default, (float)AttunementID.Evil)];
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
            return !Main.projectile.Any(n => n.active && n.owner == player.whoAmI &&
            (n.type == ProjectileType<BitingEmbrace>() ||
             n.type == ProjectileType<GrovetendersTouch>() ||
             n.type == ProjectileType<AridGrandeur>()));
        }

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            if (mainAttunement == null)
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
            Texture2D itemTexture = TextureAssets.Item[Item.type].Value;
            Rectangle itemFrame = (Main.itemAnimations[Item.type] == null) ? itemTexture.Frame() : Main.itemAnimations[Item.type].GetFrame(itemTexture);

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
                AddIngredient(ItemType<AerialiteBar>(), 5).
                AddIngredient(ItemID.HellstoneBar, 5).
                AddIngredient(ItemID.DirtBlock, 50).
                AddIngredient(ItemID.StoneBlock, 50).
                AddTile(TileID.Anvils).
                Register();
        }
    }
}


