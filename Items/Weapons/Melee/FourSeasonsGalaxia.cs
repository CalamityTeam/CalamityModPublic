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
        public static int PhoenixAttunement_LocalIFrames = 30; //Remember its got one extra update
        public static float PhoenixAttunement_BoltDamageReduction = 0.5f;
        public static float PhoenixAttunement_BoltThrowDamageMultiplier = 1f;
        public static float PhoenixAttunement_BaseDamageReduction = 0.5f;
        public static float PhoenixAttunement_FullChargeDamageBoost = 2.1f;
        public static float PhoenixAttunement_ThrowDamageBoost = 3.2f;

        public static int PolarisAttunement_BaseDamage = 1200;
        public static int PolarisAttunement_FullChargeDamage = 1900;
        public static int PolarisAttunement_ShredIFrames = 10;
        public static int PolarisAttunement_LocalIFrames = 30; //Be warned its got one extra update so all the iframes should be divided in 2
        public static int PolarisAttunement_LocalIFramesCharged = 16;
        public static float PolarisAttunement_SlashDamageBoost = 6f; //Keep in mind the slice always crits
        public static int PolarisAttunement_SlashBoltsDamage = 1300;
        public static int PolarisAttunement_SlashIFrames = 20;
        public static float PolarisAttunement_ShotDamageBoost = 0.8f; //The shots fired if the dash connects
        public static float PolarisAttunement_ShredChargeupGain = 1.1f; //How much charge is gainted per second.

        public static int AndromedaAttunement_BaseDamage = 2200;
        public static int AndromedaAttunement_DashHitIFrames = 20;
        public static float AndromedaAttunement_FullChargeBoost = 3.5f; //The EXTRA damage boost. So putting 1 here will make it deal double damage. Putting 0.5 here will make it deal 1.5x the damage.
        public static float AndromedaAttunement_MonolithDamageBoost = 1.2f;
        public static float AndromedaAttunement_BoltsDamageReduction = 0.2f; //The shots fired as it charges

        public static int AriesAttunement_BaseDamage = 950;
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
                               "FUNCTION_PASSIVE\n" +
                               "Upgrading the sword let it break free from its earthly boundaries. You now have access to every single attunement at all times!\n" +
                               "Use RMB to cycle the sword's attunement forward or backwards depending on the position of your cursor\n" +
                               "Active Attunement : None\n" +
                               "Passive Blessing : None\n"); ;
        }

        #region tooltip editing

        public override void ModifyTooltips(List<TooltipLine> list)
        {
            SafeCheckAttunements();

            Player player = Main.player[Main.myPlayer];
            if (player is null)
                return;

            var effectDescTooltip = list.FirstOrDefault(x => x.Name == "Tooltip0" && x.mod == "Terraria");
            var passiveDescTooltip = list.FirstOrDefault(x => x.Name == "Tooltip1" && x.mod == "Terraria");
            var mainAttunementTooltip = list.FirstOrDefault(x => x.Name == "Tooltip4" && x.mod == "Terraria");
            var blessingTooltip = list.FirstOrDefault(x => x.Name == "Tooltip5" && x.mod == "Terraria");

            //Default stuff gets skipped here. MainAttunement is set to true in SafeCheckAttunements() above

            //.. but just in case
            if (mainAttunement == null)
            {
                CalamityMod.Instance.Logger.Error("No main attunement on galaxia, couldn't edit its tooltip properly. How the hell did that happen.");
                return;
            }

            effectDescTooltip.text = mainAttunement.function_description + "\n" + mainAttunement.function_description_extra;
            effectDescTooltip.overrideColor = mainAttunement.tooltipColor;

            passiveDescTooltip.text = mainAttunement.passive_description;
            passiveDescTooltip.overrideColor = mainAttunement.tooltipPassiveColor;

            mainAttunementTooltip.text = "Active Attunement : [" + mainAttunement.name + "]";
            mainAttunementTooltip.overrideColor = Color.Lerp(mainAttunement.tooltipColor, mainAttunement.tooltipColor2, 0.5f + (float)Math.Sin(Main.GlobalTime) * 0.5f);

            blessingTooltip.text = "Passive Blessing : [" + mainAttunement.passive_name + "]";
            blessingTooltip.overrideColor = mainAttunement.tooltipPassiveColor;
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
            mainAttunement = Attunement.attunementArray[reader.ReadInt32()];
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

        public void SafeCheckAttunements()
        {
            if (mainAttunement == null)
                mainAttunement = Attunement.attunementArray[(int)AttunementID.Phoenix];

            else
                mainAttunement = Attunement.attunementArray[(int)MathHelper.Clamp((float)mainAttunement.id, (float)AttunementID.Phoenix, (float)AttunementID.Andromeda)];
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

            SafeCheckAttunements();

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

        public override void UpdateInventory(Player player)
        {
            SafeCheckAttunements();
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
