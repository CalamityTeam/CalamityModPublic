using Terraria; using CalamityMod.Projectiles; using Terraria.ModLoader;
using Terraria.ID;
using Terraria.ModLoader; using CalamityMod.Buffs; using CalamityMod.Items; using CalamityMod.NPCs; using CalamityMod.Projectiles; using CalamityMod.Tiles; using CalamityMod.Walls;

namespace CalamityMod.Items
{
    public class OldPowerCell : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Old Power Cell");
            Tooltip.SetDefault("Summons the ancient golem when used in the Temple");
        }

        public override void SetDefaults()
        {
            item.width = 28;
            item.height = 18;
            item.maxStack = 20;
            item.rare = 7;
            item.useAnimation = 45;
            item.useTime = 45;
            item.useStyle = 4;
            item.consumable = true;
        }

        public override bool CanUseItem(Player player)
        {
            bool canSummon = false;
            if ((double)player.Center.Y > Main.worldSurface * 16.0)
            {
                int num = (int)player.Center.X / 16;
                int num2 = (int)player.Center.Y / 16;
                Tile tile = Framing.GetTileSafely(num, num2);
                if (tile.wall == 87)
                {
                    canSummon = true;
                }
            }
            return canSummon && !NPC.AnyNPCs(NPCID.Golem);
        }

        public override bool UseItem(Player player)
        {
            NPC.SpawnOnPlayer(player.whoAmI, NPCID.Golem);
            Main.PlaySound(SoundID.Roar, player.position, 0);
            return true;
        }

        public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);
            recipe.AddIngredient(ItemID.LunarTabletFragment, 10);
            recipe.AddIngredient(null, "EssenceofCinder", 5);
            recipe.AddTile(TileID.MythrilAnvil);
            recipe.SetResult(this);
            recipe.AddRecipe();
        }
    }
}
