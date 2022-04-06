using CalamityMod.Buffs.Summon;
using CalamityMod.CalPlayer;
using CalamityMod.Items.Materials;
using CalamityMod.Projectiles.Typeless;
using CalamityMod.Tiles.Furniture.CraftingStations;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;
using static CalamityMod.CalPlayer.CalamityPlayer;
using System.Linq;

namespace CalamityMod.Items.Armor
{
    [AutoloadEquip(EquipType.Head)]
    public class DemonshadeHelm : ModItem, IExtendedHat
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Demonshade Helm");
            Tooltip.SetDefault("30% increased damage and 15% increased critical strike chance");
        }

        public override void SetDefaults()
        {
            Item.width = 18;
            Item.height = 18;
            Item.defense = 50;
            Item.value = CalamityGlobalItem.Rarity16BuyPrice;
            Item.Calamity().customRarity = CalamityRarity.HotPink;
            Item.Calamity().devItem = true;
        }

        public override bool IsArmorSet(Item head, Item body, Item legs)
        {
            return body.type == ModContent.ItemType<DemonshadeBreastplate>() && legs.type == ModContent.ItemType<DemonshadeGreaves>();
        }

        public override void ArmorSetShadows(Player player)
        {
            player.armorEffectDrawShadow = true;
            player.armorEffectDrawOutlines = true;
        }

        public override void UpdateArmorSet(Player player)
        {
            string hotkey = CalamityKeybinds.SetBonusHotKey.GetAssignedKeys().Aggregate((x, y) => x + ", " + y);
            player.setBonus = "100% increased minion damage and +10 max minions\n" +
                "All attacks inflict the demon flame debuff\n" +
                "Shadowbeams and demon scythes will fire down when you are hit\n" +
                "A friendly red devil follows you around\n" +
                "Press " + hotkey + " to enrage nearby enemies with a dark magic spell for 10 seconds\n" +
                "This makes them do 25% more damage but they also take 125% more damage";
            CalamityPlayer modPlayer = player.Calamity();
            modPlayer.dsSetBonus = true;
            modPlayer.wearingRogueArmor = true;
            modPlayer.WearingPostMLSummonerSet = true;
            if (player.whoAmI == Main.myPlayer && !modPlayer.chibii)
            {
                modPlayer.redDevil = true;
                if (player.FindBuffIndex(ModContent.BuffType<DemonshadeSetDevilBuff>()) == -1)
                {
                    player.AddBuff(ModContent.BuffType<DemonshadeSetDevilBuff>(), 3600, true);
                }
                if (player.ownedProjectileCounts[ModContent.ProjectileType<DemonshadeRedDevil>()] < 1)
                {
                    int damage = (int)(10000 * player.AverageDamage());
                    Projectile.NewProjectile(player.Center.X, player.Center.Y, 0f, -1f, ModContent.ProjectileType<DemonshadeRedDevil>(), damage, 0f, Main.myPlayer, 0f, 0f);
                }
            }
            player.GetDamage(DamageClass.Summon) += 1f;
            player.maxMinions += 10;
        }

        public override void UpdateEquip(Player player)
        {
            player.allDamage += 0.3f;
            player.Calamity().AllCritBoost(15);
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient<ShadowspecBar>(12)
                .AddTile<DraedonsForge>()
                .Register();
        }

        public string ExtensionTexture => "CalamityMod/Items/Armor/DemonshadeHelm_Extension";
        public Vector2 ExtensionSpriteOffset(PlayerDrawInfo drawInfo) => new Vector2(-10f, -14f);
        public bool PreDrawExtension(PlayerDrawInfo drawInfo) => true;
    }
}
