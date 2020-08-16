using CalamityMod.Items.Accessories;
using CalamityMod.Items.Armor.Vanity;
using CalamityMod.Items.Materials;
using CalamityMod.Items.Mounts;
using CalamityMod.Items.Pets;
using CalamityMod.Items.Placeables.FurnitureCosmilite;
using CalamityMod.Items.Weapons.Magic;
using CalamityMod.Items.Weapons.Melee;
using CalamityMod.Items.Weapons.Ranged;
using CalamityMod.Items.Weapons.Rogue;
using CalamityMod.Items.Weapons.Summon;
using CalamityMod.NPCs.DevourerofGods;
using CalamityMod.World;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.ModLoader;

namespace CalamityMod.Items.TreasureBags
{
    public class DevourerofGodsBag : ModItem
    {
        public override int BossBagNPC => ModContent.NPCType<DevourerofGodsHeadS>();

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Treasure Bag");
            Tooltip.SetDefault("{$CommonItemTooltip.RightClickToOpen}");
        }

        public override void SetDefaults()
        {
            item.maxStack = 999;
            item.consumable = true;
            item.width = 24;
            item.height = 24;
            item.rare = 9;
            item.expert = true;
        }

        public override void PostDrawInWorld(SpriteBatch spriteBatch, Color lightColor, Color alphaColor, float rotation, float scale, int whoAmI)
        {
            Vector2 origin = new Vector2(18f, 20f); //18, 17
            spriteBatch.Draw(ModContent.GetTexture("CalamityMod/Items/TreasureBags/DevourerofGodsBagGlow"), item.Center - Main.screenPosition, null, Color.White, rotation, origin, 1f, SpriteEffects.None, 0f);
        }

        public override bool CanRightClick()
        {
            return true;
        }

        public override void PostUpdate() => CalamityUtils.ForceItemIntoWorld(item);

        public override void OpenBossBag(Player player)
        {
            player.TryGettingDevArmor();

            // Materials
            DropHelper.DropItem(player, ModContent.ItemType<CosmiliteBar>(), 30, 39);
            DropHelper.DropItem(player, ModContent.ItemType<CosmiliteBrick>(), 200, 320);

            // Weapons
			int[] weapons = new int[] {
				ModContent.ItemType<Excelsus>(),
				ModContent.ItemType<TheObliterator>(),
				ModContent.ItemType<Deathwind>(),
				ModContent.ItemType<DeathhailStaff>(),
				ModContent.ItemType<StaffoftheMechworm>()
			};

			bool droppedWeapon = false;
			for (int i = 0; i < weapons.Length; i++)
			{
				if (DropHelper.DropItemChance(player, weapons[i], 3) > 0)
					droppedWeapon = true;
			}

			if (DropHelper.DropItemFromSetChance(player, 0.3333f, ModContent.ItemType<EradicatorMelee>(), ModContent.ItemType<Eradicator>()))
				droppedWeapon = true;

			if (!droppedWeapon)
			{
				// Can't choose anything from an empty array.
				if (weapons is null || weapons.Length == 0)
					goto SKIPDROPS;

				// Resize the array and add the last weapon
				Array.Resize(ref weapons, weapons.Length + 1);
				weapons[weapons.Length - 1] = ModContent.ItemType<Eradicator>();

				// Choose which item to drop.
				int itemID = Main.rand.Next(weapons);
				if (itemID == ModContent.ItemType<Eradicator>() && Main.rand.NextBool(2))
					itemID = ModContent.ItemType<EradicatorMelee>();

				DropHelper.DropItem(player, itemID);
			}
			SKIPDROPS:

            DropHelper.DropItemChance(player, ModContent.ItemType<Skullmasher>(), DropHelper.RareVariantDropRateInt);
            DropHelper.DropItemChance(player, ModContent.ItemType<Norfleet>(), DropHelper.RareVariantDropRateInt);
            float dischargeChance = DropHelper.LegendaryDropRateFloat;
            DropHelper.DropItemCondition(player, ModContent.ItemType<CosmicDischarge>(), CalamityWorld.revenge, dischargeChance);

            // Equipment
            DropHelper.DropItem(player, ModContent.ItemType<NebulousCore>());
            bool vodka = player.Calamity().fabsolVodka;
            DropHelper.DropItemCondition(player, ModContent.ItemType<Fabsol>(), CalamityWorld.revenge && vodka);

            // Vanity
            DropHelper.DropItemChance(player, ModContent.ItemType<DevourerofGodsMask>(), 7);
            DropHelper.DropItemCondition(player, ModContent.ItemType<CosmicPlushie>(), CalamityWorld.death && player.difficulty == 2);
        }
    }
}
