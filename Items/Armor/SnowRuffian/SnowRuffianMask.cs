using CalamityMod.CalPlayer;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using CalamityMod.Items.BaseItems;

namespace CalamityMod.Items.Armor.SnowRuffian
{
    [AutoloadEquip(EquipType.Head)]
    public class SnowRuffianMask : StealthGrantingArmorPiece
    {
        private bool shouldBoost = false;

        public static bool HasRuffianArmorSet(Player player) => player.armor[0].type == ModContent.ItemType<SnowRuffianMask>() && player.armor[1].type == ModContent.ItemType<SnowRuffianChestplate>() && player.armor[2].type == ModContent.ItemType<SnowRuffianGreaves>();
        public override float StealthBoost => 0.5f;
        public override bool HasArmorSet(Player player) => HasRuffianArmorSet(player);
        

        public override void Load()
        {
            if (Main.netMode != NetmodeID.Server)
            {
                EquipLoader.AddEquipTexture(Mod, "CalamityMod/Items/Armor/SnowRuffian/SnowRuffianWings", EquipType.Wings, this, equipTexture: new SnowRuffianWings());
            }
        }

        public override void SetStaticDefaults()
        {
            SacrificeTotal = 1;
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
            var modPlayer = player.Calamity();
            modPlayer.snowRuffianSet = true;
            player.setBonus = "5% increased rogue damage\n" +
                "You can glide to negate fall damage";
            player.GetDamage<ThrowingDamageClass>() += 0.05f;

            StealthSetBonus(player);

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
            player.GetDamage<ThrowingDamageClass>() += 0.02f;
        }

        public override void AddRecipes()
        {
            CreateRecipe().
                AddRecipeGroup("AnySnowBlock", 10).
                AddRecipeGroup("AnyIceBlock", 5).
                AddIngredient(ItemID.BorealWood, 15).
                AddTile(TileID.Anvils).
                Register();
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
