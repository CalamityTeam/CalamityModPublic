using Terraria;
using Terraria.ModLoader;
using Terraria.ID;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using CalamityMod.World;

namespace CalamityMod.Items.TheDevourerofGods
{
	public class DevourerofGodsBag : ModItem
	{
        public override int BossBagNPC => mod.NPCType("DevourerofGodsHeadS");

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
			spriteBatch.Draw(mod.GetTexture("Items/TheDevourerofGods/DevourerofGodsBagGlow"), item.Center - Main.screenPosition, null, Color.White, rotation, origin, 1f, SpriteEffects.None, 0f);
		}

		public override bool CanRightClick()
		{
			return true;
		}

		public override void OpenBossBag(Player player)
		{
            player.TryGettingDevArmor();

            // Materials
            DropHelper.DropItem(player, mod.ItemType("CosmiliteBar"), 30, 39);

            // Weapons
            DropHelper.DropItemChance(player, mod.ItemType("Excelsus"), 3);
            DropHelper.DropItemChance(player, mod.ItemType("EradicatorMelee"), 3);
            float dischargeChance = CalamityWorld.defiled ? DropHelper.DefiledDropRateFloat : DropHelper.LegendaryDropRateFloat;
            DropHelper.DropItemCondition(player, mod.ItemType("CosmicDischarge"), CalamityWorld.revenge, dischargeChance);
            DropHelper.DropItemChance(player, mod.ItemType("TheObliterator"), 3);
            DropHelper.DropItemChance(player, mod.ItemType("Deathwind"), 3);
            DropHelper.DropItemChance(player, mod.ItemType("Skullmasher"), DropHelper.RareVariantDropRateInt);
            DropHelper.DropItemChance(player, mod.ItemType("Norfleet"), DropHelper.RareVariantDropRateInt);
            DropHelper.DropItemChance(player, mod.ItemType("DeathhailStaff"), 3);
            DropHelper.DropItemChance(player, mod.ItemType("StaffoftheMechworm"), 3);
            DropHelper.DropItemChance(player, mod.ItemType("Eradicator"), 3);

            // Equipment
            DropHelper.DropItem(player, mod.ItemType("NebulousCore"));
            bool vodka = player.GetModPlayer<CalamityPlayer>(mod).fabsolVodka;
            DropHelper.DropItemCondition(player, mod.ItemType("Fabsol"), CalamityWorld.revenge && vodka);

            // Vanity
            DropHelper.DropItemChance(player, mod.ItemType("DevourerofGodsMask"), 7);
            DropHelper.DropItemCondition(player, mod.ItemType("CosmicPlushie"), CalamityWorld.death && player.difficulty == 2);
		}
	}
}
