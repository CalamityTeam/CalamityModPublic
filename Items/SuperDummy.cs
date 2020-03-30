using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using CalamityMod.NPCs.NormalNPCs;

namespace CalamityMod.Items
{
    public class SuperDummy : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Super Dummy");
            Tooltip.SetDefault("Creates a super dummy\n" +
                "Regenerates 1 million life per second\n" +
                "Will not die when taking damage over time from debuffs\n" +
                "Right click to kill all super dummies");
        }

        public override void SetDefaults()
        {
            item.damage = 0;
            item.width = 20;
            item.height = 30;
            item.useTime = 15;
            item.useAnimation = 15;
            item.useStyle = 1;
            item.useTurn = true;
            item.value = 0;
            item.rare = 1;
            item.autoReuse = true;
        }

        public override bool AltFunctionUse(Player player)
        {
            return true;
        }

        // DEFECT -- Make Super Dummies work in multiplayer by spawning correctly.
        public override bool UseItem(Player player)
        {
            if (player.altFunctionUse == 2)
            {
                for (int i = 0; i < Main.maxNPCs; i++)
                {
                    if (Main.npc[i].type == ModContent.NPCType<SuperDummyNPC>())
                    {
                        Main.npc[i].life = 0;
                        Main.npc[i].lifeRegen = 0;
                        Main.npc[i].checkDead();
                        Main.npc[i].netUpdate = true;
                    }
                }
            }
            else if (player.whoAmI == Main.myPlayer)
            {
                int x = (int)Main.MouseWorld.X - 9;
                int y = (int)Main.MouseWorld.Y - 20;
                NPC.NewNPC(x, y, ModContent.NPCType<SuperDummyNPC>());
            }
            return true;
        }

        public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);
            recipe.AddIngredient(ItemID.TargetDummy);
            recipe.AddTile(TileID.Anvils);
            recipe.SetResult(this);
            recipe.AddRecipe();
        }
    }
}
