using CalamityMod.Events;
using CalamityMod.Items.Materials;
using CalamityMod.NPCs.Polterghast;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Audio;
using Terraria.GameContent.Creative;

namespace CalamityMod.Items.SummonItems
{
    public class NecroplasmicBeacon : ModItem
    {
        public override void SetStaticDefaults()
        {
            CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1;
            DisplayName.SetDefault("Necroplasmic Beacon");
            Tooltip.SetDefault("It's spooky\n" +
                "Summons Polterghast when used in the dungeon\n" +
                "Not consumable");
        }

        public override void SetDefaults()
        {
            Item.width = 42;
            Item.height = 58;
            Item.useAnimation = 10;
            Item.useTime = 10;
            Item.useStyle = ItemUseStyleID.HoldUp;
            Item.consumable = false;
            Item.rare = ItemRarityID.Purple;
            Item.Calamity().customRarity = CalamityRarity.Turquoise;
        }

        public override void PostDrawInWorld(SpriteBatch spriteBatch, Color lightColor, Color alphaColor, float rotation, float scale, int whoAmI)
        {
            Item.DrawItemGlowmaskSingleFrame(spriteBatch, rotation, ModContent.Request<Texture2D>("CalamityMod/Items/SummonItems/NecroplasmicBeaconGlow").Value);
        }

        public override bool CanUseItem(Player player)
        {
            return player.ZoneDungeon && !NPC.AnyNPCs(ModContent.NPCType<Polterghast>()) && !BossRushEvent.BossRushActive;
        }

        public override bool? UseItem(Player player)
        {
            SoundEngine.PlaySound(SoundID.Roar, player.position, 0);
            if (Main.netMode != NetmodeID.MultiplayerClient)
                NPC.SpawnOnPlayer(player.whoAmI, ModContent.NPCType<Polterghast>());
            else
                NetMessage.SendData(MessageID.SpawnBoss, -1, -1, null, player.whoAmI, ModContent.NPCType<Polterghast>());

            return true;
        }

        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient(ItemID.SpookyWood, 25).
                AddIngredient<Phantoplasm>(50).
                AddTile(TileID.LunarCraftingStation).
                Register();
        }
    }
}
