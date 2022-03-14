using CalamityMod.Items.Materials;
using CalamityMod.Buffs.Potions;
using CalamityMod.DataStructures;
using CalamityMod.CalPlayer;
using CalamityMod.Projectiles.Melee;
using CalamityMod.Tiles.Furniture.CraftingStations;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using CalamityMod.Items.Placeables;
using CalamityMod.Particles;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Terraria.ModLoader.IO;
using static CalamityMod.CalamityUtils;
using static CalamityMod.Items.Weapons.Melee.FourSeasonsGalaxia;
using static Terraria.ModLoader.ModContent;


namespace CalamityMod.Items.Weapons.Melee
{
    public class FourSeasonsGalaxia : ModItem
    {
        public Attunement mainAttunement = null;

        //Used for passive effects. On hit proc is never used but its just there so i can pass it as a reference in the passiveeffect function
        public int UseTimer = 0;
        public bool OnHitProc = false;

        #region stats
        public static int BaseDamage = 800;

        public static int PhoenixAttunement_BaseDamage = 800;
        public static int PhoenixAttunement_LocalIFrames = 20; //Remember its got one extra update
        public static float PhoenixAttunement_BoltDamageReduction = 0.5f;
        public static float PhoenixAttunement_BoltThrowDamageMultiplier = 1.5f;
        public static float PhoenixAttunement_BaseDamageReduction = 0.5f;
        public static float PhoenixAttunement_FullChargeDamageBoost = 2.1f;
        public static float PhoenixAttunement_ThrowDamageBoost = 3.6f;

        public static int PolarisAttunement_BaseDamage = 1200;
        public static int PolarisAttunement_FullChargeDamage = 1900;
        public static int PolarisAttunement_ShredIFrames = 10;
        public static int PolarisAttunement_LocalIFrames = 30; //Be warned its got one extra update so all the iframes should be divided in 2
        public static int PolarisAttunement_LocalIFramesCharged = 16;
        public static float PolarisAttunement_SlashDamageBoost = 13f; //Keep in mind the slice always crits
        public static int PolarisAttunement_SlashBoltsDamage = 1300;
        public static int PolarisAttunement_SlashIFrames = 60;
        public static float PolarisAttunement_ShotDamageBoost = 2f; //The shots fired if the dash connects
        public static float PolarisAttunement_ShredChargeupGain = 1.1f; //How much charge is gainted per second.

        public static int AndromedaAttunement_BaseDamage = 2100;
        public static int AndromedaAttunement_DashHitIFrames = 60;
        public static float AndromedaAttunement_FullChargeBoost = 5f; //The EXTRA damage boost. So putting 1 here will make it deal double damage. Putting 0.5 here will make it deal 1.5x the damage.
        public static float AndromedaAttunement_MonolithDamageBoost = 1.75f;
        public static float AndromedaAttunement_BoltsDamageReduction = 0.2f; //The shots fired as it charges

        public static int AriesAttunement_BaseDamage = 1100;
        public static int AriesAttunement_LocalIFrames = 10;
        public static int AriesAttunement_Reach = 600;
        public static float AriesAttunement_ChainDamageReduction = 0.2f;
        public static float AriesAttunement_OnHitBoltDamageReduction = 0.5f;

        public static int CancerPassiveDamage = 3000;
        public static int CancerPassiveLifeSteal = 3;
        public static float CancerPassiveLifeStealProc = 0.4f;
        public static int CapricornPassiveDebuffTime = 200;


        #endregion

        public override string Texture => "CalamityMod/Items/Weapons/Melee/Galaxia"; //Base sprite for stuff like item browser and shit. yeah

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Galaxia");
            Tooltip.SetDefault("FUNCTION_DESC\n" +
                               "FUNCTION_EXTRA\n" +
                               "FUNCTION_PASSIVE\n" +
                               "Upgrading the sword let it break free from its earthly boundaries. You now have access to every single attunement at all times!\n" +
                               "Use RMB to cycle the sword's attunement forward or backwards depending on the position of your cursor\n" +
                               "Active Attunement : None\n" +
                               "Passive Blessing : None\n"); ;
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
                    else //Fake the existence of a main attunement
                    {
                        l.overrideColor = Attunement.attunementArray[(int)AttunementID.Phoenix].tooltipColor;
                        l.text = Attunement.attunementArray[(int)AttunementID.Phoenix].function_description;
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
                        l.overrideColor = Attunement.attunementArray[(int)AttunementID.Phoenix].tooltipColor;
                        l.text = Attunement.attunementArray[(int)AttunementID.Phoenix].function_description_extra;
                    }
                    continue;
                }

                if (l.text.StartsWith("FUNCTION_PASSIVE"))
                {
                    if (mainAttunement != null)
                    {
                        l.overrideColor = mainAttunement.tooltipPassiveColor;
                        l.text = mainAttunement.passive_description;
                    }
                    else
                    {
                        l.overrideColor = Attunement.attunementArray[(int)AttunementID.Phoenix].tooltipPassiveColor;
                        l.text = Attunement.attunementArray[(int)AttunementID.Phoenix].passive_description;
                    }
                    continue;
                }

                if (l.text.StartsWith("Active Attunement"))
                {
                    if (mainAttunement != null)
                    {
                        l.overrideColor = Color.Lerp(mainAttunement.tooltipColor, mainAttunement.tooltipColor2, 0.5f + (float)Math.Sin(Main.GlobalTime) * 0.5f);
                        l.text = "Active Attumenent : [" + mainAttunement.name + "]";
                    }
                    else
                    {
                        l.overrideColor = Attunement.attunementArray[(int)AttunementID.Phoenix].tooltipColor;
                        l.text = "Active Attumenent : [" + Attunement.attunementArray[(int)AttunementID.Phoenix].name + "]";
                    }
                    continue;
                }

                if (l.text.StartsWith("Passive Blessing"))
                {
                    if (mainAttunement != null)
                    {
                        l.overrideColor = mainAttunement.tooltipPassiveColor;
                        l.text = "Passive Blessing : [" + mainAttunement.passive_name + "]";
                    }
                    else
                    {
                        l.overrideColor = Attunement.attunementArray[(int)AttunementID.Phoenix].tooltipPassiveColor;
                        l.text = "Passive Blessing : ["+ Attunement.attunementArray[(int)AttunementID.Phoenix].passive_name + "]";
                    }
                    continue;
                }
            }
        }
        #endregion

        public override void SetDefaults()
        {
            item.width = item.height = 128;
            item.damage = BaseDamage;
            item.melee = true;
            item.useAnimation = 18;
            item.useTime = 18;
            item.useTurn = true;
            item.useStyle = ItemUseStyleID.SwingThrow;
            item.knockBack = 9f;
            item.UseSound = SoundID.Item1;
            item.autoReuse = true;
            item.value = Item.buyPrice(1, 80, 0, 0);
            item.rare = ItemRarityID.Red;
            item.shoot = ProjectileID.PurificationPowder;
            item.shootSpeed = 24f;
            item.Calamity().customRarity = CalamityRarity.DarkBlue;
        }

		public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);
            recipe.AddIngredient(ItemType<OmegaBiomeBlade>());
            recipe.AddIngredient(ItemType<CosmiliteBar>(), 8);
            recipe.AddIngredient(ItemType<DarksunFragment>(), 8);
            recipe.AddTile(TileType<CosmicAnvil>());
            recipe.SetResult(this);
            recipe.AddRecipe();
        }

        #region saving and syncing attunements
        public override bool CloneNewInstances => true;

        public override ModItem Clone(Item item)
        {
            var clone = base.Clone(item);
            if (Main.mouseItem.type == ItemType<FourSeasonsGalaxia>())
                item.modItem.HoldItem(Main.player[Main.myPlayer]);
            (clone as FourSeasonsGalaxia).mainAttunement = (item.modItem as FourSeasonsGalaxia).mainAttunement;

            return clone;
        }

        public override ModItem Clone() //ditto
        {
            var clone = base.Clone();
            (clone as FourSeasonsGalaxia).mainAttunement = mainAttunement;

            return clone;
        }

        public override TagCompound Save()
        {
            int attunement1 = mainAttunement == null ? -1 : (int)mainAttunement.id;
            TagCompound tag = new TagCompound
            {
                { "mainAttunement", attunement1 },
            };
            return tag;
        }

        public override void Load(TagCompound tag)
        {
            int attunement1 = tag.GetInt("mainAttunement");

            mainAttunement = Attunement.attunementArray[attunement1 != -1 ? attunement1 : Attunement.attunementArray.Length - 1];
        }

        public override void NetSend(BinaryWriter writer)
        {
            writer.Write(mainAttunement != null ? (byte)mainAttunement.id : Attunement.attunementArray.Length - 1);
        }

        public override void NetRecieve(BinaryReader reader)
        {
            mainAttunement = Attunement.attunementArray[reader.ReadByte()];
        }

        #endregion

        public override bool Shoot(Player player, ref Vector2 position, ref float speedX, ref float speedY, ref int type, ref int damage, ref float knockBack)
        {
            if (mainAttunement == null)
                return false;
            return true;
        }

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

            if (CanUseItem(player))
            {
                player.Calamity().LungingDown = false;
            }
            else
            {
                UseTimer++;
            }

            if (mainAttunement == null)
                mainAttunement = Attunement.attunementArray[(int)AttunementID.Phoenix];
            //Clamp the attunement to only be part of the 4 galaxia attunements
            else if (mainAttunement.id < AttunementID.Phoenix)
                mainAttunement = Attunement.attunementArray[(int)AttunementID.Phoenix];

            mainAttunement.ApplyStats(item);

            //Passive effects only jappen player side haha
            if (player.whoAmI != Main.myPlayer)
                return;

            mainAttunement.PassiveEffect(player, ref UseTimer, ref OnHitProc);

            if (player.Calamity().mouseRight && CanUseItem(player) && player.whoAmI == Main.myPlayer && !Main.mapFullscreen)
            {
                //Don't shoot out a visual blade if you already have one out
                if (Main.projectile.Any(n => n.active && n.type == ProjectileType<GalaxiaHoldout>() && n.owner == player.whoAmI))
                    return;

                Projectile.NewProjectile(player.Top, Vector2.Zero, ProjectileType<GalaxiaHoldout>(), 0, 0, player.whoAmI, 0, Math.Sign(player.position.X - Main.MouseWorld.X));
            }
        }

        public override bool CanUseItem(Player player)
        {
            return !Main.projectile.Any(n => n.active && n.owner == player.whoAmI &&
            (n.type == ProjectileType<PhoenixsPride>() ||
             n.type == ProjectileType<AndromedasStride>() ||
             n.type == ProjectileType<PolarisGaze>() ||
             n.type == ProjectileType<AriesWrath>()
            ));
        }

        public override bool PreDrawInInventory(SpriteBatch spriteBatch, Vector2 position, Rectangle frame, Color drawColor, Color itemColor, Vector2 origin, float scale)
        {
            if (mainAttunement == null)
                mainAttunement = Attunement.attunementArray[(int)AttunementID.Phoenix];

            Texture2D itemTexture = GetTexture((mainAttunement.id == AttunementID.Polaris || mainAttunement.id == AttunementID.Andromeda) ? "CalamityMod/Items/Weapons/Melee/GalaxiaDusk" : "CalamityMod/Items/Weapons/Melee/GalaxiaDawn");
            Texture2D outlineTexture = GetTexture((mainAttunement.id == AttunementID.Polaris || mainAttunement.id == AttunementID.Andromeda) ? "CalamityMod/Items/Weapons/Melee/GalaxiaDuskOutline" : "CalamityMod/Items/Weapons/Melee/GalaxiaDawnOutline");

            int currentFrame = ((int)Math.Floor(Main.GlobalTime * 15f)) % 7;
            Rectangle animFrame = new Rectangle(0, 128 * currentFrame, 126, 126);

            spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.NonPremultiplied, null, null, null, null, Main.UIScaleMatrix);

            spriteBatch.Draw(outlineTexture, position, animFrame, Color.White, 0f, origin, scale, SpriteEffects.None, 0f);

            spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Deferred, null, null, null, null, null, Main.UIScaleMatrix);


            spriteBatch.Draw(itemTexture, position, animFrame, Color.White, 0f, origin, scale, SpriteEffects.None, 0f);
            return false;
        }

        public override bool PreDrawInWorld(SpriteBatch spriteBatch, Color lightColor, Color alphaColor, ref float rotation, ref float scale, int whoAmI)
        {
            if (mainAttunement == null)
                mainAttunement = Attunement.attunementArray[(int)AttunementID.Phoenix];

            Texture2D itemTexture = GetTexture((mainAttunement.id == AttunementID.Polaris || mainAttunement.id == AttunementID.Andromeda) ? "CalamityMod/Items/Weapons/Melee/GalaxiaDusk" : "CalamityMod/Items/Weapons/Melee/GalaxiaDawn");
            Texture2D outlineTexture = GetTexture((mainAttunement.id == AttunementID.Polaris || mainAttunement.id == AttunementID.Andromeda) ? "CalamityMod/Items/Weapons/Melee/GalaxiaDuskOutline" : "CalamityMod/Items/Weapons/Melee/GalaxiaDawnOutline");

            int currentFrame = ((int)Math.Floor(Main.GlobalTime * 15f)) % 7;
            Rectangle animFrame = new Rectangle(0, 128 * currentFrame, 126, 126);

            spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.NonPremultiplied, null, null, null, null, Main.UIScaleMatrix);

            spriteBatch.Draw(outlineTexture, item.Center - Main.screenPosition, animFrame, lightColor, rotation, item.Size * 0.5f, scale, SpriteEffects.None, 0f);

            spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Deferred, null, null, null, null, null, Main.UIScaleMatrix);


            spriteBatch.Draw(itemTexture, item.Center - Main.screenPosition, animFrame, lightColor, rotation, item.Size * 0.5f, scale, SpriteEffects.None, 0f);
            return false;
        }
    }
}
