using System;
using System.Collections.Generic;
using System.IO;
using CalamityMod.Items.Materials;
using CalamityMod.Projectiles.Magic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;

namespace CalamityMod.Items.Weapons.Magic
{
    public class SHPC : LegendaryItem, ILocalizedModType
    {
        public new string LocalizationCategory => "Items.Weapons.Magic";
        public static readonly SoundStyle FireSound = new("CalamityMod/Sounds/Item/AnomalysNanogunMPFBShot");
        public static readonly SoundStyle VacuumStart = new SoundStyle("CalamityMod/Sounds/Item/SHPCVacuumStart") with { Volume = 0.5f };
        public static readonly SoundStyle VacuumLoop = new SoundStyle("CalamityMod/Sounds/Item/SHPCVacuumLoop") with { Volume = 0.5f };
        public static readonly SoundStyle VacuumEnd = new SoundStyle("CalamityMod/Sounds/Item/SHPCVacuumEnd") with { Volume = 0.5f };

        public const int ShotsPerSoul = 50;
        public int storedSoulpower = 0;
        public int storedSoulType = ItemID.SoulofLight; // Determines bar color and soul effects for projectile
        public int recoilProgress = 0;

        public const float LightExplosionSizeMult = 1.5f;
        public const float NightExplosionTimeMult = 2.5f;
        public const int FlightDirectHitFlightBoost = 25; // The amount of flight time restored on direct hits with Flight bombs, in frames
        public const float MightKnockbackStrength = 20f; // The value to multiply the unit vector by when applying velocity to enemies launched by Might bombs
        public const float SightHomingRange = 320f; // Range of homing for Sight bombs, in pixels
        public const int FrightFlatDamage = 20;

        public override Color? TooltipExtensionColor => new Color(31, 251, 255);

        public override void SetStaticDefaults() => ItemID.Sets.ItemsThatAllowRepeatedRightClick[Type] = true;

        public override void SetDefaults()
        {
            Item.width = 124;
            Item.height = 52;
            Item.damage = 117;
            Item.DamageType = DamageClass.Magic;
            Item.mana = 30;
            Item.useAnimation = Item.useTime = 60;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.noMelee = true;
            Item.knockBack = 3f;
            Item.UseSound = FireSound;
            Item.autoReuse = true;
            Item.shoot = ModContent.ProjectileType<SHPB>();
            Item.shootSpeed = 20f;

            Item.value = CalamityGlobalItem.RarityPinkBuyPrice;
            Item.rare = ItemRarityID.Pink;
        }

        #region Weapon-Specific Functions
        public static int FindSoulForAmmo(Player player)
        {
            int soul = -1;
            bool foundItem = false;

            // First check ammo slots
            for (int s = 54; s < 58; s++)
            {
                if (player.inventory[s].stack > 0 && player.inventory[s].ammo == ItemID.SoulofLight)
                {
                    soul = player.inventory[s].type;
                    foundItem = true;
                    break;
                }
            }

            // If you didn't find anything in ammo slots, check normal inventory
            if (!foundItem)
            {
                for (int i = 0; i < 54; i++)
                {
                    if (player.inventory[i].stack > 0 && player.inventory[i].ammo == ItemID.SoulofLight)
                    {
                        soul = player.inventory[i].type;
                        break;
                    }
                }
            }

            return soul;
        }

        public override bool CanRightClick()
        {
            if (!Main.keyState.PressingShift())
                return false;
            return true;
        }

        public override void RightClick(Player player)
        {
            if (storedSoulpower > 0)
            {
                storedSoulpower = 0;
            }
        }

        public override bool ConsumeItem(Player player) => false;

        public Color FindColorForSoul()
        {
            Color returnColor = new(0, 0, 0);
            switch (storedSoulType)
            {
                case ItemID.SoulofLight:
                    returnColor = new(240, 29, 196);
                    break;
                case ItemID.SoulofNight:
                    returnColor = new(123, 29, 220);
                    break;
                case ItemID.SoulofFlight:
                    returnColor = new(106, 240, 250);
                    break;
                case ItemID.SoulofMight:
                    returnColor = new(4, 51, 222);
                    break;
                case ItemID.SoulofSight:
                    returnColor = new(79, 255, 124);
                    break;
                case ItemID.SoulofFright:
                    returnColor = new(255, 96, 20);
                    break;
            }
            return returnColor;
        }

        public int TransferColorToProj()
        {
            int projai = 0;
            switch (storedSoulType)
            {
                case ItemID.SoulofLight:
                    projai = 0;
                    break;
                case ItemID.SoulofNight:
                    projai = 1;
                    break;
                case ItemID.SoulofFlight:
                    projai = 2;
                    break;
                case ItemID.SoulofMight:
                    projai = 3;
                    break;
                case ItemID.SoulofSight:
                    projai = 4;
                    break;
                case ItemID.SoulofFright:
                    projai = 5;
                    break;
            }
            return projai;
        }
        #endregion

        public override Vector2? HoldoutOffset() => new Vector2(-35, -10);
        public override bool AltFunctionUse(Player player) => true;

        public override void OnCreated(ItemCreationContext context)
        {
            if (context is RecipeItemCreationContext)
                storedSoulpower = ShotsPerSoul;
        }

        public override bool CanUseItem(Player player)
        {
            Item.channel = Item.noUseGraphic = player.altFunctionUse == 2;
            Item.UseSound = player.altFunctionUse == 2 ? null : FireSound;
            return ((player.altFunctionUse == 0 && (storedSoulpower > 0 || (FindSoulForAmmo(player) != -1))) || player.altFunctionUse == 2) &&
                player.ownedProjectileCounts[ModContent.ProjectileType<SHPV>()] <= 0;
        }

        public override bool? UseItem(Player player)
        {
            if (player.altFunctionUse != 2)
            {
                if (storedSoulpower > 0)
                    storedSoulpower--;

                if (storedSoulpower <= 0)
                {
                    bool ammoConsumed = false;

                    if (FindSoulForAmmo(player) != -1)
                    {
                        int soulType = FindSoulForAmmo(player);
                        player.ConsumeItem(soulType);
                        storedSoulType = soulType;
                        ammoConsumed = true;
                    }

                    if (ammoConsumed)
                        storedSoulpower = ShotsPerSoul;
                }
            }

            return base.UseItem(player);
        }

        public override void ModifyManaCost(Player player, ref float reduce, ref float mult)
        {
            if (player.altFunctionUse == 2)
                mult *= 0f;
        }

        public override float UseSpeedMultiplier(Player player)
        {
            if (player.altFunctionUse == 2)
                return 6f;

            return 1f;
        }

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            if (player.altFunctionUse == 2)
            {
                Projectile.NewProjectile(source, position, velocity, ModContent.ProjectileType<SHPV>(), damage, 0f, player.whoAmI);
                return false;
            }
            else
            {
                Projectile.NewProjectile(source, position + new Vector2(0, -10) + velocity * 3f, velocity, ModContent.ProjectileType<SHPB>(), damage, knockback, player.whoAmI, TransferColorToProj());
                return false;
            }
        }

        public override void PostDrawInInventory(SpriteBatch spriteBatch, Vector2 position, Rectangle frame, Color drawColor, Color itemColor, Vector2 origin, float scale)
        {
            float barScale = 2.5f;
            var barBG = ModContent.Request<Texture2D>("CalamityMod/UI/MiscTextures/GenericBarBack").Value;
            var barFG = ModContent.Request<Texture2D>("CalamityMod/UI/MiscTextures/GenericBarFront").Value;

            Vector2 drawPos = position + new Vector2((frame.Width - barBG.Width * 0.5f) * scale, (frame.Height + 45f) * scale);
            Rectangle frameCrop = new Rectangle(0, 0, (int)(storedSoulpower / (float)ShotsPerSoul * barFG.Width), barFG.Height);
            Color colorBG = Color.Black;
            Color colorFG = Color.Lerp(Color.DarkGray, FindColorForSoul(), storedSoulpower / (float)ShotsPerSoul);

            spriteBatch.Draw(barBG, drawPos, null, colorBG, 0f, origin, scale * barScale, 0f, 0f);
            spriteBatch.Draw(barFG, drawPos, frameCrop, colorFG * 0.8f, 0f, origin, scale * barScale, 0f, 0f);

            CalamityUtils.DrawBorderStringEightWay(spriteBatch, FontAssets.MouseText.Value, storedSoulpower.ToString(), drawPos + new Vector2(-200, -60) * scale, Color.GreenYellow, Color.Black, scale * 2.5f);
        }

        #region Recoil Stuff
        public override void HoldItem(Player player)
        {
            player.Calamity().mouseWorldListener = true;
            player.Calamity().rightClickListener = true;
        }
        public override void UseStyle(Player player, Rectangle heldItemFrame)
        {
            player.ChangeDir(Math.Sign((player.Calamity().mouseWorld - player.Center).X));
            float itemRotation = player.compositeFrontArm.rotation + MathHelper.PiOver2 * player.gravDir;

            Vector2 itemPosition = player.MountedCenter + itemRotation.ToRotationVector2() * 35f;
            Vector2 itemSize = new Vector2(Item.width, Item.height);
            Vector2 itemOrigin = new Vector2(-35, 0);

            if (player.altFunctionUse != 2)
            {
                recoilProgress++;
                if (recoilProgress < Item.useAnimation / 3)
                {
                    itemPosition -= (player.Calamity().mouseWorld - player.Center).SafeNormalize(Vector2.UnitX) * (Item.useAnimation / 3 - recoilProgress) * 0.75f;
                }
                else
                {
                    if (recoilProgress >= Item.useAnimation - 1)
                        recoilProgress = 0;
                }
            }

            CalamityUtils.CleanHoldStyle(player, itemRotation, itemPosition, itemSize, itemOrigin);
            base.UseStyle(player, heldItemFrame);
        }

        public override void UseItemFrame(Player player)
        {
            player.ChangeDir(Math.Sign((player.Calamity().mouseWorld - player.Center).X));
            float rotation = (player.Center - player.Calamity().mouseWorld).ToRotation() * player.gravDir + MathHelper.PiOver2;

            player.SetCompositeArmFront(true, Player.CompositeArmStretchAmount.Full, rotation);
        }
        #endregion

        public override void ModifyWeaponDamage(Player player, ref StatModifier damage)
        {
            // scaling legendary!!!!
            if (Main.zenithWorld)
            {
                bool plantera = NPC.downedPlantBoss;
                bool golem = NPC.downedGolemBoss;
                bool cultist = NPC.downedAncientCultist;
                bool moonLord = NPC.downedMoonlord;
                bool providence = DownedBossSystem.downedProvidence;
                bool devourerOfGods = DownedBossSystem.downedDoG;
                bool yharon = DownedBossSystem.downedYharon;
                float damageMult = 1f +
                    (plantera ? 0.1f : 0f) + //1.1
                    (golem ? 0.15f : 0f) + //1.25
                    (cultist ? 3.5f : 0f) + //4.75
                    (moonLord ? 4.5f : 0f) + //9.25
                    (providence ? 7.5f : 0f) + //16.75
                    (devourerOfGods ? 2.5f : 0f) + //19.25
                    (yharon ? 30f : 0f); //49.25
                damage *= damageMult;
            }
        }
        public override void ModifyTooltips(List<TooltipLine> list)
        {
            if (Main.zenithWorld)
                list.FindAndReplace("[GFB]", Lang.SupportGlyphs(this.GetLocalizedValue("TooltipGFB")));
            else
                list.FindAndReplace("[GFB]", Lang.SupportGlyphs(this.GetLocalization("TooltipNormal").Format(this.GetLocalizedValue(storedSoulpower == 0 ? "NoSoul" : "SoulDesc" + storedSoulType))));
        }

        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient<PlasmaDriveCore>().
                AddIngredient<SuspiciousScrap>(4).
                AddRecipeGroup("AnyMythrilBar", 10).
                AddIngredient(ItemID.SoulofFright, 5).
                AddIngredient(ItemID.SoulofMight, 5).
                AddIngredient(ItemID.SoulofSight, 5).
                AddTile(TileID.MythrilAnvil).
                Register();
        }

        #region Saving Ammo Amount and Soul Type
        public override ModItem Clone(Item item)
        {
            ModItem clone = base.Clone(item);
            if (clone is SHPC a && item.ModItem is SHPC a2)
            {
                a.storedSoulpower = a2.storedSoulpower;
                a.storedSoulType = a2.storedSoulType;
            }
            return clone;
        }

        public override void SaveData(TagCompound tag)
        {
            tag["ammoStored"] = storedSoulpower;
            tag["soulType"] = storedSoulType;
        }

        public override void LoadData(TagCompound tag)
        {
            storedSoulpower = tag.GetInt("ammoStored");
            storedSoulType = tag.GetInt("soulType");
        }

        public override void NetSend(BinaryWriter writer)
        {
            writer.Write(storedSoulpower);
            writer.Write(storedSoulType);
        }

        public override void NetReceive(BinaryReader reader)
        {
            storedSoulpower = reader.ReadInt32();
            storedSoulType = reader.ReadInt32();
        }
        #endregion
    }
}
