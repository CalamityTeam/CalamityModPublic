using CalamityMod.Buffs.Pets;
using CalamityMod.Projectiles.Pets;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Pets
{
    public class CharredRelic : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Charred Relic");
            Tooltip.SetDefault("Contains a small amount of brimstone");
        }

        public override void SetDefaults()
        {
            item.CloneDefaults(ItemID.ZephyrFish);
            item.shoot = ModContent.ProjectileType<BrimlingPet>();
            item.buffType = ModContent.BuffType<BrimlingBuff>();

            item.value = Item.sellPrice(gold: 4);
            item.rare = ItemRarityID.Red;
            item.Calamity().devItem = true;

            item.UseSound = SoundID.NPCHit51;
        }

        public override void UseStyle(Player player)
        {
            if (player.whoAmI == Main.myPlayer && player.itemTime == 0)
            {
                player.AddBuff(item.buffType, 3600, true);
            }
        }
    }
}
