using CalamityMod.Buffs.Summon;
using CalamityMod.CalPlayer;
using CalamityMod.Items.Materials;
using CalamityMod.Projectiles.Summon;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.GameContent.Creative;

namespace CalamityMod.Items.Armor
{
    [AutoloadEquip(EquipType.Head)]
    public class VictideHelmet : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Victide Helmet");
            Tooltip.SetDefault("10% increased minion damage");
        }

        public override void SetDefaults()
        {
            Item.width = 18;
            Item.height = 18;
            Item.value = Item.buyPrice(0, 1, 50, 0);
            Item.rare = ItemRarityID.Green;
            Item.defense = 1; //8
            CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1;
        }

        public override bool IsArmorSet(Item head, Item body, Item legs)
        {
            return body.type == ModContent.ItemType<VictideBreastplate>() && legs.type == ModContent.ItemType<VictideLeggings>();
        }

        public override void UpdateArmorSet(Player player)
        {
            player.setBonus = "+3 life regen and 10% increased minion damage while submerged in liquid\n" +
                "Summons a sea urchin to protect you\n" +
                "When using any weapon you have a 10% chance to throw a returning seashell projectile\n" +
                "This seashell does true damage and does not benefit from any damage class\n" +
                "Provides increased underwater mobility and slightly reduces breath loss in the abyss\n" +
                "+1 max minion";
            CalamityPlayer modPlayer = player.Calamity();
            modPlayer.victideSet = true;
            modPlayer.urchin = true;
            player.maxMinions++;
            if (player.whoAmI == Main.myPlayer)
            {
                if (player.FindBuffIndex(ModContent.BuffType<VictideSummonSetBuff>()) == -1)
                {
                    player.AddBuff(ModContent.BuffType<VictideSummonSetBuff>(), 3600, true);
                }
                var source = player.GetProjectileSource_Item(Item);
                if (player.ownedProjectileCounts[ModContent.ProjectileType<Urchin>()] < 1)
                {
                    int p = Projectile.NewProjectile(source, player.Center.X, player.Center.Y, 0f, -1f, ModContent.ProjectileType<Urchin>(), (int)(7f * (player.GetDamage<GenericDamageClass>().Base + player.GetDamage(DamageClass.Summon).Base - 1f)), 0f, Main.myPlayer, 0f, 0f);
                    if (Main.projectile.IndexInRange(p))
                        Main.projectile[p].originalDamage = 7;
                }
            }
            player.ignoreWater = true;
            if (Collision.DrownCollision(player.position, player.width, player.height, player.gravDir))
            {
                player.GetDamage(DamageClass.Summon) += 0.1f;
                player.lifeRegen += 3;
            }
        }

        public override void UpdateEquip(Player player)
        {
            player.GetDamage(DamageClass.Summon) += 0.1f;
        }

        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient<VictideBar>(3).
                AddTile(TileID.Anvils).
                Register();
        }
    }
}
