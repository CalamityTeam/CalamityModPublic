using CalamityMod.Events;
using CalamityMod.Items.Materials;
using CalamityMod.NPCs.Cryogen;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Linq;
using System.Collections.Generic;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.SummonItems
{
    public class CryoKey : ModItem
    {
        public override void SetStaticDefaults()
        {
            SacrificeTotal = 1;
            DisplayName.SetDefault("Cryo Key");
            Tooltip.SetDefault("Summons Cryogen when used in the tundra\n" +
                "Enrages outside the tundra\n" +
                "Not consumable");
			ItemID.Sets.SortingPriorityBossSpawns[Type] = 7; // Mechanical Eye
        }

        public override void SetDefaults()
        {
            Item.width = 26;
            Item.height = 48;
            Item.rare = ItemRarityID.Pink;
            Item.useAnimation = 10;
            Item.useTime = 10;
            Item.useStyle = ItemUseStyleID.HoldUp;
        }

		public override void ModifyResearchSorting(ref ContentSamples.CreativeHelper.ItemGroup itemGroup)
		{
			itemGroup = ContentSamples.CreativeHelper.ItemGroup.BossItem;
		}

        public override bool CanUseItem(Player player)
        {
            return player.ZoneSnow && !NPC.AnyNPCs(ModContent.NPCType<Cryogen>()) && !BossRushEvent.BossRushActive;
        }

        public override bool? UseItem(Player player)
        {
            SoundEngine.PlaySound(SoundID.Roar, player.Center);
            if (Main.netMode != NetmodeID.MultiplayerClient)
                NPC.SpawnOnPlayer(player.whoAmI, ModContent.NPCType<Cryogen>());
            else
                NetMessage.SendData(MessageID.SpawnBoss, -1, -1, null, player.whoAmI, ModContent.NPCType<Cryogen>());

            return true;
        }

        public override bool PreDrawInInventory(SpriteBatch spriteBatch, Vector2 position, Rectangle frameI, Color drawColor, Color itemColor, Vector2 origin, float scale)
        {
            Texture2D texture = ModContent.Request<Texture2D>("CalamityMod/Items/SummonItems/CryoKey").Value;
            Color overlay = CalamityMod.Instance.legendaryMode ? Color.Red : Color.White;
            spriteBatch.Draw(texture, position, null, overlay, 0f, origin, scale, 0, 0);
            return false;
        }

        public override bool PreDrawInWorld(SpriteBatch spriteBatch, Color lightColor, Color alphaColor, ref float rotation, ref float scale, int whoAmI)
        {
            Texture2D texture = ModContent.Request<Texture2D>("CalamityMod/Items/SummonItems/CryoKey").Value;
            Color overlay = CalamityMod.Instance.legendaryMode ? Color.Red : lightColor;
            spriteBatch.Draw(texture, Item.position - Main.screenPosition, null, overlay, 0f, Vector2.Zero, 1f, 0, 0);
            return false;
        }

        public override void ModifyTooltips(List<TooltipLine> list)
        {
            Player player = Main.LocalPlayer;
            TooltipLine name = list.FirstOrDefault(x => x.Mod == "Terraria" && x.Name == "ItemName");
            TooltipLine line0 = list.FirstOrDefault(x => x.Mod == "Terraria" && x.Name == "Tooltip0");

            if (CalamityMod.Instance.legendaryMode)
            {
                name.Text = "Pyro Key";
                line0.Text = "Summons Cryogen when used in the tundra...?";
            }
            else
            {
                name.Text = "Cryo Key";
                line0.Text = "Summons Cryogen when used in the tundra";
            }
        }

        public override void AddRecipes()
        {
            CreateRecipe().
                AddRecipeGroup("AnyIceBlock", 50).
                AddIngredient(ItemID.SoulofLight, 5).
                AddIngredient(ItemID.SoulofNight, 5).
                AddIngredient<EssenceofEleum>(8).
                AddTile(TileID.Anvils).
                Register();
        }
    }
}
