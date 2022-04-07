using CalamityMod.CalPlayer;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Armor
{
    [AutoloadEquip(EquipType.Head)]
    public class SnowRuffianMask : ModItem
    {
        private bool shouldBoost = false;

        public override void Load()
        {
            if (Main.netMode != NetmodeID.Server)
            {
                Mod.AddEquipTexture(new SnowRuffianWings(), this, EquipType.Wings, "CalamityMod/Items/Armor/SnowRuffianWings");
            }
        }

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Snow Ruffian Mask");
            Tooltip.SetDefault("2% increased rogue damage");
        }

        public override void SetDefaults()
        {
            Item.width = 18;
            Item.height = 18;
            Item.value = Item.buyPrice(0, 0, 75, 0);
            Item.rare = ItemRarityID.Blue;
            Item.defense = 1; //4
        }

        public override bool IsArmorSet(Item head, Item body, Item legs)
        {
            return body.type == ModContent.ItemType<SnowRuffianChestplate>() && legs.type == ModContent.ItemType<SnowRuffianGreaves>();
        }

        public override void UpdateArmorSet(Player player)
        {
            CalamityPlayer modPlayer = player.Calamity();
            modPlayer.snowRuffianSet = true;
            modPlayer.rogueStealthMax += 0.5f;
            player.setBonus = "5% increased rogue damage\n" +
                "You can glide to negate fall damage\n" +
                "Rogue stealth builds while not attacking and slower while moving, up to a max of 50\n" +
                "Once you have built max stealth, you will be able to perform a Stealth Strike\n" +
                "Rogue stealth only reduces when you attack, it does not reduce while moving\n" +
                "The higher your rogue stealth the higher your rogue damage, crit, and movement speed";
            player.Calamity().throwingDamage += 0.05f;
            player.Calamity().wearingRogueArmor = true;
            if (player.controlJump)
            {
                player.noFallDmg = true;
                player.UpdateJumpHeight();
                if (shouldBoost && !player.mount.Active)
                {
                    player.velocity.X *= 1.1f;
                    shouldBoost = false;
                }

            }
            else if (!shouldBoost && player.velocity.Y == 0)
            {
                shouldBoost = true;
            }
        }

        public override void UpdateEquip(Player player)
        {
            player.Calamity().throwingDamage += 0.02f;
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddRecipeGroup("AnySnowBlock", 10)
                .AddRecipeGroup("AnyIceBlock", 5)
                .AddIngredient(ItemID.BorealWood, 15)
                .AddTile(TileID.Anvils)
                .Register();
        }
    }
    public class SnowRuffianWings : EquipTexture
    {
        public override void VerticalWingSpeeds(Player player, ref float ascentWhenFalling, ref float ascentWhenRising, ref float maxCanAscendMultiplier, ref float maxAscentMultiplier, ref float constantAscend)
        {
            ascentWhenFalling = 0.5f;
            ascentWhenRising = 0f;
            maxCanAscendMultiplier = 0f;
            maxAscentMultiplier = 0f;
            constantAscend = 0f;
        }

        public override void HorizontalWingSpeeds(Player player, ref float speed, ref float acceleration)
        {
            speed = 2f;
            acceleration *= 1.25f;
        }
    }
}
