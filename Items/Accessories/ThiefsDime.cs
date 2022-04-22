using CalamityMod.Projectiles.Rogue;
using Terraria;
using Terraria.ModLoader;
using Terraria.ID;
using Terraria.GameContent.Creative;

namespace CalamityMod.Items.Accessories
{
    public class ThiefsDime : ModItem
    {
        public override void SetStaticDefaults()
        {
            CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1;
            DisplayName.SetDefault("Thief's Dime");
            Tooltip.SetDefault("Those scurvy dogs don’t know the first thing about making bank\n" +
            "Summons a coin that revolves around you and steals money from enemies");
        }

        public override void SetDefaults()
        {
            Item.width = 20;
            Item.height = 20;
            Item.value = Item.buyPrice(0, 36, 0, 0);
            Item.rare = ItemRarityID.Pink;
            Item.accessory = true;
        }

        public override bool CanEquipAccessory(Player player, int slot, bool modded)
        {
            return !player.Calamity().thiefsDime;
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.Calamity().thiefsDime = true;
            if (player.whoAmI == Main.myPlayer)
            {
                if (player.ownedProjectileCounts[ModContent.ProjectileType<ThiefsDimeProj>()] < 1)
                {
                    var source = player.GetSource_Accessory(Item);
                    Projectile.NewProjectile(source, player.Center.X, player.Center.Y, 0f, 0f, ModContent.ProjectileType<ThiefsDimeProj>(), (int)(100 * player.RogueDamage()), 6f, Main.myPlayer, 0f, 0f);
                }
            }
        }
    }
}
