using Terraria.DataStructures;
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
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;
using static Terraria.ModLoader.ModContent;

namespace CalamityMod.Items.Weapons.Melee
{
    public class TrueBiomeBlade : ModItem, ILocalizedModType
    {
        public new string LocalizationCategory => "Items.Weapons.Melee";
        public Attunement mainAttunement = null;
        public Attunement secondaryAttunement = null;
        public int Combo = 0;
        public float ComboResetTimer = 0f;
        public int StoredLunges = 2;
        public int PowerLungeCounter = 0;

        #region stats
        public static int BaseDamage = 80;

        public static int DefaultAttunement_BaseDamage = 80;
        public static int DefaultAttunement_SigilTime = 1200;
        public static int DefaultAttunement_BeamTime = 60;
        public static float DefaultAttunement_HomingAngle = MathHelper.PiOver4;

        public static int EvilAttunement_BaseDamage = 155;
        public static int EvilAttunement_Lifesteal = 2;
        public static int EvilAttunement_BounceIFrames = 10;
        public static float EvilAttunement_SlashDamageBoost = 3f;
        public static int EvilAttunement_SlashIFrames = 60;

        public static int ColdAttunement_BaseDamage = 150;
        public static float ColdAttunement_SecondSwingBoost = 1.15f;
        public static float ColdAttunement_ThirdSwingBoost = 1.3f;
        public static float ColdAttunement_MistDamageReduction = 0.09f;

        public static int HotAttunement_BaseDamage = 122;
        public static int HotAttunement_FullChargeDamage = 160;
        public static int HotAttunement_ShredIFrames = 8;
        public static float HotAttunement_ShotDamageBoost = 4.5f;
        public static int HotAttunement_LocalIFrames = 30; //Be warned its got one extra update so all the iframes should be divided in 2
        public static int HotAttunement_LocalIFramesCharged = 16;
        public static float HotAttunement_ShredDecayRate = 0.65f; //How much charge is lost per frame.

        public static int TropicalAttunement_BaseDamage = 120;
        public static float TropicalAttunement_ChainDamageReduction = 0.6f;
        public static float TropicalAttunement_VineDamageReduction = 0.3f;
        public static float TropicalAttunement_SweetSpotDamageMultiplier = 1.2f; //It also crits, so be mindful of that
        public static int TropicalAttunement_LocalIFrames = 60; //Be warned its got 2 extra updates so all the iframes should be divided in 3

        public static int HolyAttunement_BaseDamage = 60;
        public static float HolyAttunement_BaseDamageReduction = 0.4f;
        public static float HolyAttunement_FullChargeDamageBoost = 2f;
        public static float HolyAttunement_ThrowDamageBoost = 3.8f;
        public static int HolyAttunement_LocalIFrames = 16; //Be warned its got 1 extra update yadda yadda

        public static int AstralAttunement_BaseDamage = 225;
        public static int AstralAttunement_DashHitIFrames = 20;
        public static float AstralAttunement_FullChargeBoost = 2.5f; //The EXTRA damage boost. So putting 1 here will make it deal double damage. Putting 0.5 here will make it deal 1.5x the damage.
        public static float AstralAttunement_MonolithDamageBoost = 1.25f;
        public static float AstralAttunement_MonolithDamageFalloff = 0.25f; //Damage multiplier for all subsequent hits after the first one.

        public static int MarineAttunement_BaseDamage = 300;
        public static float MarineAttunement_InWaterDamageMultiplier = 1.5f;

        #endregion


        public override void SetStaticDefaults()
        {
            //Theres potential for flavor text as well but im not a writer
        }

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
            var mainAttunementTooltip = list.FirstOrDefault(x => x.Text.Contains("[ATT1]") && x.Mod == "Terraria");
            var secondaryAttunementTooltip = list.FirstOrDefault(x => x.Text.Contains("[ATT2]") && x.Mod == "Terraria");

            if (effectDescTooltip != null)
            {
                //Default stuff
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
        #endregion

        public override void SetDefaults()
        {
            Item.width = Item.height = 68;
            Item.damage = BaseDamage;
            Item.DamageType = DamageClass.Melee;
            Item.useAnimation = 21;
            Item.useTime = 21;
            Item.useTurn = true;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.knockBack = 7.5f;
            Item.UseSound = SoundID.Item1;
            Item.autoReuse = true;
            Item.value = CalamityGlobalItem.Rarity6BuyPrice;
            Item.rare = ItemRarityID.LightPurple;
            Item.shoot = ProjectileID.PurificationPowder;
            Item.shootSpeed = 12f;
        }

        #region Saving and syncing attunements

        public override ModItem Clone(Item item)
        {
            var clone = base.Clone(item);
            if (Main.mouseItem.type == ItemType<TrueBiomeBlade>())
                item.ModItem?.HoldItem(Main.player[Main.myPlayer]);
            if (clone is TrueBiomeBlade a && item.ModItem is TrueBiomeBlade a2)
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

            tag.Add("mainAttunement", attunement1);
            tag.Add("secondaryAttunement", attunement2);
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
                mainAttunement = Attunement.attunementArray[(int)MathHelper.Clamp((float)mainAttunement.id, (float)AttunementID.TrueDefault, (float)AttunementID.Marine)];

            if (secondaryAttunement != null)
                secondaryAttunement = Attunement.attunementArray[(int)MathHelper.Clamp((float)secondaryAttunement.id, (float)AttunementID.TrueDefault, (float)AttunementID.Marine)];
        }

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
                Item.noUseGraphic = false;
                Item.useStyle = ItemUseStyleID.Swing;
                Item.noMelee = false;
                Item.channel = false;
                Item.shoot = ProjectileID.PurificationPowder;
                Item.shootSpeed = 12f;
                Item.UseSound = SoundID.Item1;
                Combo = 0;

                Combo = 0;
                PowerLungeCounter = 0;
            }

            else
                mainAttunement.ApplyStats(Item);

            if (mainAttunement != null && mainAttunement.id != AttunementID.TrueCold && mainAttunement.id != AttunementID.TrueTropical)
                Combo = 0;

            if (player.Calamity().mouseRight && CanUseItem(player) && player.whoAmI == Main.myPlayer && !Main.mapFullscreen)
            {
                //Don't shoot out a visual blade if you already have one out
                if (Main.projectile.Any(n => n.active && n.type == ProjectileType<BiomeBladeHoldout>() && n.owner == player.whoAmI))
                    return;

                var source = player.GetSource_ItemUse(Item);
                bool mayAttune = player.StandingStill() && !player.mount.Active && player.CheckSolidGround(1, 3);
                Vector2 displace = new Vector2(18f, 0f);
                Projectile.NewProjectile(source, player.Top + displace, Vector2.Zero, ProjectileType<BiomeBladeHoldout>(), 0, 0, player.whoAmI, mayAttune ? 0f : 1f);
            }
        }

        public override void UpdateInventory(Player player)
        {
            SafeCheckAttunements();

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

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            if (mainAttunement == null)
                return false;

            ComboResetTimer = 1f;
            return mainAttunement.Shoot(player, source, ref position, ref velocity.X, ref velocity.Y, ref type, ref damage, ref knockback, ref Combo, ref StoredLunges, ref PowerLungeCounter);
        }


        //This is only used for the purity sigil effect of the default attunement
        public override void OnHitNPC(Player player, NPC target, NPC.HitInfo hit, int damageDone)
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
            var source = player.GetSource_ItemUse(Item);
            Projectile.NewProjectile(source, target.Center, Vector2.Zero, ProjectileType<PurityProjectionSigil>(), 0, 0, player.whoAmI, target.whoAmI);
        }

        internal static ChargingEnergyParticleSet BiomeEnergyParticles = new ChargingEnergyParticleSet(-1, 2, Color.White, Color.White, 0.04f, 20f);
        internal static void UpdateAllParticleSets()
        {
            BiomeEnergyParticles.Update();
        }

        public override bool PreDrawInInventory(SpriteBatch spriteBatch, Vector2 position, Rectangle frame, Color drawColor, Color itemColor, Vector2 origin, float scale)
        {
            Texture2D itemTexture = Request<Texture2D>("CalamityMod/Projectiles/Melee/MendedBiomeBlade").Value;

            if (mainAttunement == null)
            {
                spriteBatch.Draw(itemTexture, position, null, drawColor, 0f, origin, scale, SpriteEffects.None, 0f);
                return false;
            }


            // Draw all particles.

            Vector2 particleDrawCenter = position + new Vector2(12f, 16f) * Main.inventoryScale - frame.Size() * 0.18f;

            BiomeEnergyParticles.EdgeColor = mainAttunement.energyParticleEdgeColor;
            BiomeEnergyParticles.CenterColor = mainAttunement.energyParticleCenterColor;
            BiomeEnergyParticles.InterpolationSpeed = 0.1f;
            BiomeEnergyParticles.DrawSet(particleDrawCenter + Main.screenPosition);

            Vector2 displacement = Vector2.UnitX.RotatedBy(Main.GlobalTimeWrappedHourly * 3f) * 2f * (float)Math.Sin(Main.GlobalTimeWrappedHourly);

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
            Texture2D itemTexture = Request<Texture2D>("CalamityMod/Projectiles/Melee/MendedBiomeBlade").Value; //Use the "projectile" sprite which is flipped so its consistent in lighting with the rest of the line, since its actual sprite is flipped so the swings may look normal
            spriteBatch.Draw(itemTexture, Item.Center - Main.screenPosition, null, lightColor, rotation, Item.Size * 0.5f, scale, SpriteEffects.None, 0f);
            return false;
        }

        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient<BrokenBiomeBlade>().
                AddIngredient(ItemID.SoulofFright).
                AddIngredient(ItemID.SoulofMight).
                AddIngredient(ItemID.SoulofSight).
                AddIngredient(ItemID.PixieDust, 2).
                AddIngredient<Stardust>(10).
                AddTile(TileID.MythrilAnvil).
                Register();
        }
    }
}
