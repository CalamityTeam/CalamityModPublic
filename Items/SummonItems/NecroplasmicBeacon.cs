using CalamityMod.Events;
using CalamityMod.Items.Materials;
using CalamityMod.NPCs.Polterghast;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.SummonItems
{
    public class NecroplasmicBeacon : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Necroplasmic Beacon");
            Tooltip.SetDefault("It's spooky\n" +
                "Summons Polterghast when used in the dungeon\n" +
                "Not consumable");
        }

        public override void SetDefaults()
        {
            item.width = 42;
            item.height = 58;
            item.useAnimation = 45;
            item.useTime = 45;
            item.useStyle = ItemUseStyleID.HoldingUp;
            item.consumable = false;
            item.rare = ItemRarityID.Purple;
            item.Calamity().customRarity = CalamityRarity.Turquoise;
        }

        public override void PostDrawInWorld(SpriteBatch spriteBatch, Color lightColor, Color alphaColor, float rotation, float scale, int whoAmI)
        {
            item.DrawItemGlowmaskSingleFrame(spriteBatch, rotation, ModContent.GetTexture("CalamityMod/Items/SummonItems/NecroplasmicBeaconGlow"));
        }

        public override bool CanUseItem(Player player)
        {
            return player.ZoneDungeon && !NPC.AnyNPCs(ModContent.NPCType<Polterghast>()) && !BossRushEvent.BossRushActive;
        }

        public override bool UseItem(Player player)
        {
            Main.PlaySound(SoundID.Roar, player.position, 0);
            if (Main.netMode != NetmodeID.MultiplayerClient)
                NPC.SpawnOnPlayer(player.whoAmI, ModContent.NPCType<Polterghast>());
            else
                NetMessage.SendData(MessageID.SpawnBoss, -1, -1, null, player.whoAmI, ModContent.NPCType<Polterghast>());

            return true;
        }

        public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);
            recipe.AddIngredient(ItemID.SpookyWood, 25);
            recipe.AddIngredient(ModContent.ItemType<Phantoplasm>(), 50);
            recipe.AddTile(TileID.LunarCraftingStation);
            recipe.SetResult(this);
            recipe.AddRecipe();
        }
    }
}
